using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GrappleState
{
    INACTIVE,
    SEEKING,
    ENGAGING,
    ENGAGED,
    RETURNING
}

public enum GrappleCursorState
{
    UNLOCKED,
    LOCKED,
    OUT_OF_RANGE
}

public class Grapple : MonoBehaviour
{  
    private const int initialEdgePointArrayLength = 64;

    // Serialized Data
    [Header("Grappling")]
    [SerializeField] private GameObject grapplePrefab = null;
    [SerializeField] private float grappleDistance = 50f;
    [SerializeField] private float grappleEngagementSpeed = 100f;
    [SerializeField] private float grappleAcceleration = 80f;
    [SerializeField] private float grappleSpeed = 25f;
    [SerializeField] private float grappleCooldown = 10f;
    [SerializeField,
    Range(0, 1)]     private float grapplePerpendicularVelocityDecayAlpha = 0.1f;
    [SerializeField] private float grappleDisengageDistance = 3f;
    [SerializeField] private GameObject mapGameObject;

    [Header("Snapping")]
    [SerializeField] private float grappleSnapDistance = 0.05f;
    [SerializeField,
    Range(0, 1)]     private float grappleSnapAlpha = 0.5f;

    [HideInInspector] public GrappleState state = GrappleState.INACTIVE;
    [HideInInspector] public GrappleCursorState cursorState = GrappleCursorState.OUT_OF_RANGE;
    [HideInInspector] public GrappleState previousState = GrappleState.INACTIVE;
    [HideInInspector] public float currentToMaxSpeedRatio = 0f;
    [HideInInspector] public bool onCooldown = false;

    // Private Members
    private FPSKinematicBody kb;
    private FPSLook look;
    private FPSMovement movement;

    private Vector3 grapplePoint;
    private Vector3 toGrapplePoint;
    private float distanceToGrapplePoint;
    private bool grappleAvailable = true;
    private float engagementDistanceTravelled = 0f;

    private Transform grappleTransform;
    private Transform cameraTransform;
    private Camera cameraComponent;

    private Vector3[] edgePoints = new Vector3[initialEdgePointArrayLength];
    private int edgeCount = 0;
    private Transform seekerTransform;
    private Vector3 seekerNormalizedPosition = new Vector3(0.5f, 0.5f);
    Vector3 seekerWorldPoint;
    private float squaredGrappleDistance;

    private void AquireReferences()
    {
        kb = GetComponent<FPSKinematicBody>();
        look = GetComponent<FPSLook>();
        movement = GetComponent<FPSMovement>();

        cameraTransform = transform.GetChild(0);
        cameraComponent = cameraTransform.GetChild(0).GetComponent<Camera>();
        seekerTransform = GameObject.Find("seeker").transform;
        seekerTransform.gameObject.SetActive(false);
    }

    private void CalculateEdgePoints()
    {
        if (mapGameObject == null)
            return;

        BoxCollider[] colliders = mapGameObject.GetComponentsInChildren<BoxCollider>();
        foreach (BoxCollider collider in colliders)
        {
            Vector3 scale = collider.transform.localScale;
            Vector3 s = scale/2f;
            Vector3 position = collider.transform.position;

            edgePoints[2*edgeCount + 0] = new Vector3(position.x + s.x, position.y + s.y, position.z + s.z);
            edgePoints[2*edgeCount + 1] = new Vector3(position.x - s.x, position.y + s.y, position.z + s.z);
            edgePoints[2*edgeCount + 2] = new Vector3(position.x + s.x, position.y + s.y, position.z - s.z);
            edgePoints[2*edgeCount + 3] = new Vector3(position.x - s.x, position.y + s.y, position.z - s.z);

            edgePoints[2*edgeCount + 4] = new Vector3(position.x + s.x, position.y + s.y, position.z + s.z);
            edgePoints[2*edgeCount + 5] = new Vector3(position.x + s.x, position.y + s.y, position.z - s.z);
            edgePoints[2*edgeCount + 6] = new Vector3(position.x - s.x, position.y + s.y, position.z + s.z);
            edgePoints[2*edgeCount + 7] = new Vector3(position.x - s.x, position.y + s.y, position.z - s.z);

            /* 4 upper horizontal edges in a cube */
            edgeCount += 4;

            if (edgeCount*2 == edgePoints.Length)
                Array.Resize(ref edgePoints, edgePoints.Length*2);
        }
    }

    private void CalculateSquaredGrappleDistance()
    {
        squaredGrappleDistance = grappleDistance*grappleDistance;
    }

    private void Start()
    {
        AquireReferences();
        CalculateEdgePoints();
        CalculateSquaredGrappleDistance();
    }

    private void DisengageGrapple()
    {
        state = GrappleState.RETURNING;
        engagementDistanceTravelled = 0f;
        kb.gravityMultiplier = 1f;
        movement.canMove = true;
    }

    private void EngageGrapple()
    {
        state = GrappleState.ENGAGED;
    }

    private void DeactivateGrapple()
    {
        state = GrappleState.INACTIVE;
        Destroy(grappleTransform.gameObject);
        look.rotationLocked = false;
    }

    private void ActivateGrapple()
    {
        if (seekerWorldPoint != Vector3.zero)
        {
            grapplePoint = seekerWorldPoint;
            toGrapplePoint = grapplePoint - transform.position;
            distanceToGrapplePoint = toGrapplePoint.magnitude;
            toGrapplePoint.Normalize();

            state = GrappleState.ENGAGING;
            grappleAvailable = false;
            onCooldown = true;
            engagementDistanceTravelled = 0f;
            grappleTransform = Instantiate(grapplePrefab).transform;
            look.rotationLocked = true;
            movement.canMove = false;

            HideSeeker();
        }
    }

    private void CheckForBreak()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, toGrapplePoint, out hit, Mathf.Infinity, ~(1 << 10)))
        {
            if (Mathf.Abs(hit.point.sqrMagnitude - grapplePoint.sqrMagnitude) > 0.1f)
                DisengageGrapple();
        }
    }

    private void AlignPlayerToGrapplePoint()
    {
        Vector3 direction = grapplePoint - transform.position;
        direction.y = 0f;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    void ComputeVelocity()
    {
        Vector3 velocity = new Vector3(kb.velocityX, kb.velocityY, kb.velocityZ);
        Vector3 velocityToGrapple = Vector3.Project(velocity, toGrapplePoint);
        float velocityToGrappleDot = Vector3.Dot(velocity.normalized, toGrapplePoint);

        if (velocityToGrapple.magnitude < grappleSpeed || velocityToGrappleDot <= 0f)
        {
            velocity -= velocityToGrapple;
            velocityToGrapple += toGrapplePoint*grappleAcceleration*Time.fixedDeltaTime;
            if (velocityToGrapple.magnitude > grappleSpeed && velocityToGrappleDot > 0f)
                velocityToGrapple = velocityToGrapple.normalized*grappleSpeed;
            velocity += velocityToGrapple;
        }

        if (!Mathf.Approximately(velocityToGrappleDot, 1f))
        {
            Vector3 rightPerpendicularVelocityToGrapple = Vector3.Project(velocity, transform.right);
            Vector3 upPerpendicularToGrapple = Vector3.Cross(toGrapplePoint, transform.right);
            Vector3 upPerpendicularVelocityToGrapple = Vector3.Project(velocity, upPerpendicularToGrapple);

            velocity -= rightPerpendicularVelocityToGrapple*grapplePerpendicularVelocityDecayAlpha*
                        currentToMaxSpeedRatio;
            velocity -= upPerpendicularVelocityToGrapple*grapplePerpendicularVelocityDecayAlpha*
                        currentToMaxSpeedRatio;
        }

        kb.velocityX = velocity.x;
        kb.velocityY = velocity.y;
        kb.velocityZ = velocity.z;

        kb.gravityMultiplier = 1 - currentToMaxSpeedRatio;
    }

    private void ShowSeeker()
    {
        seekerTransform.gameObject.SetActive(true);
    }

    private void HideSeeker()
    {
        seekerTransform.gameObject.SetActive(false);
    }

    private void PositionSeeker()
    {
        Vector3 sCenter = new Vector3(0.5f, 0.5f);
        Vector3 point = new Vector3(0, 0);
        float distance = float.MaxValue;
        Vector3 sPoint = new Vector3(0.5f, 0.5f);
        bool edgeFound = false;

        for (int i = 0; i < edgeCount; ++i)
        {
            Vector3 p1 = edgePoints[i*2 + 0];
            Vector3 p2 = edgePoints[i*2 + 1];

            if ((p1 - transform.position).sqrMagnitude > squaredGrappleDistance ||
                (p2 - transform.position).sqrMagnitude > squaredGrappleDistance)
                continue;

            Vector3 sp1 = cameraComponent.WorldToViewportPoint(p1);
            Vector3 sp2 = cameraComponent.WorldToViewportPoint(p2);

            if (sp1.z < 0 || sp2.z < 0)
                continue;

            if (sp1.x < 0 || sp1.x > 1 || sp1.y < 0 || sp1.y > 1 ||
                sp2.x < 0 || sp2.x > 1 || sp2.y < 0 || sp2.y > 1)
                continue;

            Vector3 leftBound = (sp1.x < sp2.x) ? sp1 : sp2;
            Vector3 rightBound = (sp2.x > sp1.x) ? sp2 : sp1;
            if (sCenter.x < leftBound.x - grappleSnapDistance || 
                sCenter.x > rightBound.x + grappleSnapDistance)
                continue;
            float x = Mathf.Clamp(sCenter.x, leftBound.x, rightBound.x);

            float alpha = Mathf.InverseLerp(leftBound.x, rightBound.x, x);
            float y = Mathf.Lerp(leftBound.y, rightBound.y, alpha);

            Vector2 pointOnEdge = new Vector2(x, y);
            Vector2 cursorPoint = new Vector2(sCenter.x, sCenter.y);
            float tDistance = Vector2.Distance(pointOnEdge, cursorPoint);
            if (tDistance > grappleSnapDistance)
                continue;
            if (tDistance > distance && edgeFound)
                continue;

            Vector3 screenPoint = new Vector3(x, y, 1);
            Vector3 tpoint = cameraComponent.ViewportToWorldPoint(screenPoint);
            tpoint -= cameraTransform.position;
            {
                float yDist = p1.y - cameraTransform.position.y;
                if (tpoint.y == 0)
                    continue;
                float ratio = yDist/tpoint.y;
                tpoint *= Mathf.Abs(ratio);
            }
            tpoint += cameraTransform.position;

            Vector3 tSPoint = new Vector3(x, y);
            Ray cameraRay = cameraComponent.ViewportPointToRay(tSPoint);
            RaycastHit hit;
            if (Physics.Raycast(cameraRay, out hit))
                if (Mathf.Abs(hit.point.sqrMagnitude - tpoint.sqrMagnitude) > 0.1f)
                    continue;

            point = tpoint;
            sPoint = tSPoint;
            edgeFound = true;
            distance = tDistance;
        }

        if (edgeFound)
        {
            seekerWorldPoint = point;
            cursorState = GrappleCursorState.LOCKED;
        }
        else
        {
            Ray cameraRay = cameraComponent.ViewportPointToRay(sCenter);
            RaycastHit hit;
            if (Physics.Raycast(cameraRay, out hit))
            {
                if (hit.distance > grappleDistance)
                {
                    seekerWorldPoint = Vector3.zero;
                    cursorState = GrappleCursorState.OUT_OF_RANGE;
                }
                else
                {
                    seekerWorldPoint = hit.point;
                    cursorState = GrappleCursorState.UNLOCKED;
                }
            }
            else
            {
                seekerWorldPoint = Vector3.zero;
                cursorState = GrappleCursorState.OUT_OF_RANGE;
            }
        }

        seekerNormalizedPosition = Vector3.Lerp(seekerNormalizedPosition, sPoint, grappleSnapAlpha);
        Vector3 screenSize = new Vector3(cameraComponent.pixelWidth, cameraComponent.pixelHeight);
        seekerTransform.position = new Vector3(seekerNormalizedPosition.x*screenSize.x,
                                               seekerNormalizedPosition.y*screenSize.y);
    }

    float grappleAvailabilityTimer = 0f;
    private void ComputeGrappleAvailability()
    {
        if (!grappleAvailable)
        {
            if (grappleAvailabilityTimer >= grappleCooldown)
            {
                grappleAvailabilityTimer = 0f;
                grappleAvailable = true;
                onCooldown = false;
            }

            grappleAvailabilityTimer += Time.fixedDeltaTime;
        }
    }

    bool lastLeftMouseDown = false;
    bool lastGrappleDown = false;
    private void HandleInput()
    {
        previousState = state;

        /* Grapple seeking logic */
        if (FPSInput.grappleDown && state == GrappleState.INACTIVE)
        {
            state = GrappleState.SEEKING;
            ShowSeeker();
        }
        else if (!FPSInput.grappleDown && state == GrappleState.SEEKING)
        {
            state = GrappleState.INACTIVE;
            HideSeeker();
        }

        /* Grapple engage/disengage logic */
        if (state == GrappleState.SEEKING && FPSInput.leftMouseDown && !lastLeftMouseDown)
        {
            if (grappleAvailable)
                ActivateGrapple();
        }
        else if (state == GrappleState.ENGAGED)
        {
            /* Detect if cancel key is down/pressed */
            bool grapplePressed = false;
            if (FPSInput.grappleDown && !lastGrappleDown)
                grapplePressed = true;
            bool cancelKeyDown = FPSInput.spaceDown || FPSInput.shiftDown || grapplePressed;

            if (cancelKeyDown)
                DisengageGrapple();
            else if (distanceToGrapplePoint <= grappleDisengageDistance)
                DisengageGrapple();
        }

        lastGrappleDown = FPSInput.grappleDown;
        lastLeftMouseDown = FPSInput.leftMouseDown;
    }

    private void Grappling()
    {
        if (state != GrappleState.INACTIVE && state != GrappleState.SEEKING)
        {
            toGrapplePoint = grapplePoint - transform.position;
            distanceToGrapplePoint = toGrapplePoint.magnitude;
            toGrapplePoint.Normalize();

            if (state == GrappleState.ENGAGED)
            CheckForBreak();
            if (state == GrappleState.ENGAGED)
                ComputeVelocity();
            AlignPlayerToGrapplePoint();
        }
        else if (state == GrappleState.SEEKING)
        {
            PositionSeeker();
        }

        /* Engagement/return logic */
        if (state == GrappleState.ENGAGING)
        {
            if (engagementDistanceTravelled >= distanceToGrapplePoint)
                EngageGrapple();
        }
        else if (state == GrappleState.RETURNING)
        {
            if (engagementDistanceTravelled >= distanceToGrapplePoint)
                DeactivateGrapple();
        }
    }

    private void ComputeSpeedRatio()
    {
        Vector3 velocity = new Vector3(kb.velocityX, kb.velocityY, kb.velocityZ);
        float dot = Vector3.Dot(velocity.normalized, toGrapplePoint);
        if (dot < 0f)
            dot = 0f;
        velocity *= dot;
        currentToMaxSpeedRatio = velocity.magnitude/grappleSpeed;
    }

    private void FixedUpdate()
    {
        ComputeGrappleAvailability();
        HandleInput();
        Grappling();
        ComputeSpeedRatio();
    }

    private void UpdateGrappleTransform()
    {
        if (state == GrappleState.INACTIVE || state == GrappleState.SEEKING)
            return;

        float newDistanceToGrapplePoint = Vector3.Distance(transform.position, grapplePoint);

        Vector3 newToGrapplePoint = grapplePoint - transform.position;
        grappleTransform.rotation = Quaternion.LookRotation(newToGrapplePoint);
        grappleTransform.position = transform.position;

        Vector3 scale = new Vector3(1f, 1f, newDistanceToGrapplePoint);
        if (state == GrappleState.ENGAGING)
            scale.z *= engagementDistanceTravelled/distanceToGrapplePoint;
        else if (state == GrappleState.RETURNING)
            scale.z *= 1 - engagementDistanceTravelled/distanceToGrapplePoint;
        grappleTransform.localScale = scale;
    }

    private void ComputeEngagementDistance()
    {
        engagementDistanceTravelled += Time.deltaTime*grappleEngagementSpeed;
    }

    private void LateUpdate()
    {
        UpdateGrappleTransform();
        ComputeEngagementDistance();
    }
}
