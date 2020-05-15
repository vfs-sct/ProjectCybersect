using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GrappleState
{
    INACTIVE,
    SEEKING,
    ACTIVE,
    ENGAGED,
    RETURNING
}

public class Grapple : MonoBehaviour
{  
    private const int initialEdgePointArrayLength = 64;

    [SerializeField] private GameObject grapplePrefab;
    [SerializeField] private float grappleEngagementSpeed = 100.0f;
    [SerializeField] private float grappleAcceleration = 50.0f;
    [SerializeField] private float grappleSpeed = 25.0f;
    [SerializeField,
    Range(0, 1)]     private float grapplePerpendicularVelocityDecayAlpha = 0.1f;
    [SerializeField] private float grappleDisengageDistance = 1.0f;
    [SerializeField] private int grappleSnapPixels = 100;
    [SerializeField,
    Range(0, 1)]     private float grappleSnapAlpha = 0.25f;

    [HideInInspector] public GrappleState state = GrappleState.INACTIVE;
    [HideInInspector] public GrappleState previousState = GrappleState.INACTIVE;

    private FPSKinematicBody kb;
    private FPSInput input;
    private FPSLook look;
    private FPSMovement movement;

    private Vector3 grapplePoint;
    private Vector3 toGrapplePoint;
    private float distanceToGrapplePoint;

    private Transform grappleTransform;
    private Transform cameraTransform;
    private Camera cameraComponent;

    private Vector3[] edgePoints = new Vector3[initialEdgePointArrayLength];
    private int edgeCount = 0;
    private Transform seekerTransform;
    Vector3 seekerWorldPoint;

    private void AquireReferences()
    {
        kb = GetComponent<FPSKinematicBody>();
        input = GetComponent<FPSInput>();
        look = GetComponent<FPSLook>();
        movement = GetComponent<FPSMovement>();

        cameraTransform = transform.GetChild(0);
        cameraComponent = cameraTransform.GetChild(0).GetComponent<Camera>();
        seekerTransform = GameObject.Find("seeker").transform;
        seekerTransform.gameObject.SetActive(false);
    }

    private void CalculateEdgePoints()
    {
        BoxCollider[] colliders = GameObject.Find("map").GetComponentsInChildren<BoxCollider>();
        foreach (BoxCollider collider in colliders)
        {
            Vector3 scale = collider.transform.localScale;
            Vector3 s = scale/2.0f;
            Vector3 position = collider.transform.position;

            edgePoints[2*edgeCount + 0] = new Vector3(position.x + s.x, position.y + s.y, position.z + s.z);
            edgePoints[2*edgeCount + 1] = new Vector3(position.x - s.x, position.y + s.y, position.z + s.z);
            edgePoints[2*edgeCount + 2] = new Vector3(position.x + s.x, position.y + s.y, position.z - s.z);
            edgePoints[2*edgeCount + 3] = new Vector3(position.x - s.x, position.y + s.y, position.z - s.z);

            edgePoints[2*edgeCount + 4] = new Vector3(position.x + s.x, position.y + s.y, position.z + s.z);
            edgePoints[2*edgeCount + 5] = new Vector3(position.x + s.x, position.y + s.y, position.z - s.z);
            edgePoints[2*edgeCount + 6] = new Vector3(position.x - s.x, position.y + s.y, position.z + s.z);
            edgePoints[2*edgeCount + 7] = new Vector3(position.x - s.x, position.y + s.y, position.z - s.z);

            edgePoints[2*edgeCount + 8] = new Vector3(position.x + s.x, position.y - s.y, position.z + s.z);
            edgePoints[2*edgeCount + 9] = new Vector3(position.x - s.x, position.y - s.y, position.z + s.z);
            edgePoints[2*edgeCount + 10] = new Vector3(position.x + s.x, position.y - s.y, position.z - s.z);
            edgePoints[2*edgeCount + 11] = new Vector3(position.x - s.x, position.y - s.y, position.z - s.z);

            edgePoints[2*edgeCount + 12] = new Vector3(position.x + s.x, position.y - s.y, position.z + s.z);
            edgePoints[2*edgeCount + 13] = new Vector3(position.x + s.x, position.y - s.y, position.z - s.z);
            edgePoints[2*edgeCount + 14] = new Vector3(position.x - s.x, position.y - s.y, position.z + s.z);
            edgePoints[2*edgeCount + 15] = new Vector3(position.x - s.x, position.y - s.y, position.z - s.z);

            /* 8 horizontal edges in a cube */
            edgeCount += 8;

            if (edgeCount*2 == edgePoints.Length)
                Array.Resize(ref edgePoints, edgePoints.Length*2);
        }
    }

    private void Start()
    {
        AquireReferences();
        CalculateEdgePoints();
    }

    private void DisengageGrapple()
    {
        if (state == GrappleState.ENGAGED)
        {
            state = GrappleState.RETURNING;
            grappleTimer = distanceToGrapplePoint/grappleEngagementSpeed;
        }
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
        movement.enabled = true;
    }

    private void ActivateGrapple()
    {
        if (seekerWorldPoint != Vector3.zero)
        {
            grapplePoint = seekerWorldPoint;
            toGrapplePoint = grapplePoint - transform.position;
            distanceToGrapplePoint = toGrapplePoint.magnitude;
            toGrapplePoint.Normalize();

            state = GrappleState.ACTIVE;
            grappleTimer = 0.0f;
            grappleTransform = Instantiate(grapplePrefab).transform;
            look.rotationLocked = true;
            movement.enabled = false;

            HideSeeker();
        }
    }

    private void CheckForBreak()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, toGrapplePoint, out hit, Mathf.Infinity, ~0))
        {
            if (Mathf.Abs(hit.point.sqrMagnitude - grapplePoint.sqrMagnitude) > 0.1f)
                DisengageGrapple();
        }
    }

    private void AlignPlayerToGrapplePoint()
    {
        Vector3 direction = grapplePoint - transform.position;
        direction.y = 0.0f;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    void ComputeVelocity()
    {
        Vector3 velocity = new Vector3(kb.velocityX, kb.velocityY, kb.velocityZ);
        Vector3 velocityToGrapple = Vector3.Project(velocity, toGrapplePoint);
        float velocityToGrappleDot = Vector3.Dot(velocity.normalized, toGrapplePoint);

        if (velocityToGrapple.magnitude < grappleSpeed || velocityToGrappleDot <= 0.0f)
        {
            velocity -= velocityToGrapple;
            velocityToGrapple += toGrapplePoint*grappleAcceleration*Time.fixedDeltaTime;
            if (velocityToGrapple.magnitude > grappleSpeed && velocityToGrappleDot > 0.0f)
                velocityToGrapple = velocityToGrapple.normalized*grappleSpeed;
            velocity += velocityToGrapple;
        }
        
        if (!Mathf.Approximately(velocityToGrappleDot, 1.0f))
        {
            Vector3 rightPerpendicularVelocityToGrapple = Vector3.Project(velocity, transform.right);
            Vector3 upPerpendicularToGrapple = Vector3.Cross(toGrapplePoint, transform.right);
            Vector3 upPerpendicularVelocityToGrapple = Vector3.Project(velocity, upPerpendicularToGrapple);

            velocity -= rightPerpendicularVelocityToGrapple*grapplePerpendicularVelocityDecayAlpha;
            velocity -= upPerpendicularVelocityToGrapple*grapplePerpendicularVelocityDecayAlpha;
        }

        kb.velocityX = velocity.x;
        kb.velocityY = velocity.y;
        kb.velocityZ = velocity.z;
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
        Vector3 sCenter = new Vector3(cameraComponent.pixelWidth/2.0f, 
                                      cameraComponent.pixelHeight/2.0f, 0.0f);
        Vector3 point = new Vector3(0, 0);
        float distance = float.MaxValue;
        Vector3 sPoint = new Vector3(0, 0);
        bool found = false;

        for (int i = 0; i < edgeCount; ++i)
        {
            Vector3 p1 = edgePoints[i*2 + 0];
            Vector3 p2 = edgePoints[i*2 + 1];
            Vector3 sp1 = cameraComponent.WorldToScreenPoint(p1);
            Vector3 sp2 = cameraComponent.WorldToScreenPoint(p2);

            if (sp1.z < 0 || sp2.z < 0)
                continue;

            bool check1 = sCenter.x < sp1.x;
            bool check2 = sCenter.x < sp2.x;
            if (check1 == check2)
                continue;

            float alpha = Mathf.InverseLerp(sp1.x, sp2.x, sCenter.x);
            float y = Mathf.Lerp(sp1.y, sp2.y, alpha);
            float tDistance = Mathf.Abs(y - sCenter.y);
            if (tDistance > grappleSnapPixels)
                continue;
            if (tDistance > distance && found)
                continue;

            Vector3 screenPoint = new Vector3(sCenter.x, y, 1);
            Vector3 tpoint = cameraComponent.ScreenToWorldPoint(screenPoint);
            tpoint -= cameraTransform.position;
            {
                float yDist = p1.y - cameraTransform.position.y;
                if (tpoint.y == 0)
                    continue;
                float ratio = yDist/tpoint.y;
                tpoint *= Mathf.Abs(ratio);
            }
            tpoint += cameraTransform.position;

            Vector3 tSPoint = new Vector3(sCenter.x, y);
            Ray cameraRay = cameraComponent.ScreenPointToRay(tSPoint);
            RaycastHit hit;
            if (Physics.Raycast(cameraRay, out hit))
                if (Mathf.Abs(hit.point.sqrMagnitude - tpoint.sqrMagnitude) > 0.1f)
                    continue;

            point = tpoint;
            sPoint = tSPoint;
            found = true;
            distance = tDistance;
        }

        if (found)
        {
            seekerTransform.position = Vector3.Lerp(seekerTransform.position, sPoint, grappleSnapAlpha);
            seekerWorldPoint = point;
        }
        else
        {
            seekerTransform.position = Vector3.Lerp(seekerTransform.position, sCenter, grappleSnapAlpha);

            Ray cameraRay = cameraComponent.ScreenPointToRay(sCenter);
            RaycastHit hit;
            if (Physics.Raycast(cameraRay, out hit))
                seekerWorldPoint = hit.point;
            else
                seekerWorldPoint = Vector3.zero;
        }
    }

    float grappleTimer = 0.0f;
    bool lastGrappleDown = false;
    private void Grappling()
    {
        previousState = state;

        /* Grapple seeking logic */
        if (input.grappleDown && state == GrappleState.INACTIVE)
        {
            state = GrappleState.SEEKING;
            ShowSeeker();
        }
        else if (!input.grappleDown && state == GrappleState.SEEKING)
        {
            state = GrappleState.INACTIVE;
            HideSeeker();
        }

        bool grapplePressed = false;
        if (input.grappleDown && !lastGrappleDown)
            grapplePressed = true;
        bool cancelKeyDown = input.spaceDown || input.shiftDown || grapplePressed;

        /* Grapple engage/disengage logic */
        if (state == GrappleState.SEEKING && input.leftMouseDown)
            ActivateGrapple();
        else if (state == GrappleState.ENGAGED && cancelKeyDown)
            DisengageGrapple();
        else if (distanceToGrapplePoint <= grappleDisengageDistance)
            DisengageGrapple();

        if (state != GrappleState.INACTIVE && state != GrappleState.SEEKING)
        {
            toGrapplePoint = grapplePoint - transform.position;
            distanceToGrapplePoint = toGrapplePoint.magnitude;
            toGrapplePoint.Normalize();

            CheckForBreak();
            if (state == GrappleState.ENGAGED)
                ComputeVelocity();
            AlignPlayerToGrapplePoint();
        }
        else if (state == GrappleState.SEEKING)
        {
            PositionSeeker();
        }

        /* Timing logic */
        if (state == GrappleState.ACTIVE)
        {
            if (grappleTimer*grappleEngagementSpeed >= distanceToGrapplePoint)
                EngageGrapple();

            grappleTimer += Time.fixedDeltaTime;
        }
        else if (state == GrappleState.RETURNING)
        {
            if (grappleTimer <= 0.0f)
                DeactivateGrapple();

            grappleTimer -= Time.fixedDeltaTime;
        }

        lastGrappleDown = input.grappleDown;
    }

    private void FixedUpdate()
    {
        Grappling();
    }

    private void UpdateGrappleTransform()
    {
        if (state == GrappleState.INACTIVE || state == GrappleState.SEEKING)
            return;

        Vector3 newToGrapplePoint = grapplePoint - transform.position;
        float newDistanceToGrapplePoint = Vector3.Distance(transform.position, grapplePoint);

        grappleTransform.rotation = Quaternion.LookRotation(newToGrapplePoint);
        grappleTransform.position = transform.position;

        Vector3 scale = new Vector3(1.0f, 1.0f, newDistanceToGrapplePoint);
        if (state == GrappleState.RETURNING || state == GrappleState.ACTIVE)
        {
            float multiplier = grappleTimer*grappleEngagementSpeed/scale.z;
            scale.z *= multiplier;
        }
        grappleTransform.localScale = scale;
    }

    private void LateUpdate()
    {
        UpdateGrappleTransform();
    }

    public float GetPercentOfMaxSpeed()
    {
        Vector3 velocity = new Vector3(kb.velocityX, kb.velocityY, kb.velocityZ);
        float dot = Vector3.Dot(velocity.normalized, toGrapplePoint);
        if (dot < 0.0f)
            dot = 0.0f;
        velocity *= dot;
        return velocity.magnitude/grappleSpeed;
    }
}
