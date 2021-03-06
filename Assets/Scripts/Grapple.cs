﻿using System;
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
    public GameObject grapplePrefab = null;
    public float grappleDistance = 50f;
    public float grappleEngagementSpeed = 100f;
    public float toPointAcceleration = 80f;
    public float toMouseAcceleration = 20f;
    public float toMouseAccelerationAngle = 90f;
    public float grappleSpeed = 25f;
    public float grappleCooldown = 10f;
    public float grappleDisengageDistance = 3f;
    public float rotationSmoothness = 0.5f;
    public GameObject mapGameObject;
    public float rotationTransitionSmoothness = 0.5f;

    [Header("Snapping")]
    public float grappleSnapDistance = 0.05f;
    [Range(0, 1)] 
    public float grappleSnapAlpha = 0.5f;

    [HideInInspector] public GrappleState state = GrappleState.INACTIVE;
    [HideInInspector] public GrappleCursorState cursorState = GrappleCursorState.OUT_OF_RANGE;
    [HideInInspector] public GrappleState previousState = GrappleState.INACTIVE;
    [HideInInspector] public float currentToMaxSpeedRatio = 0f;
    [HideInInspector] public bool onCooldown = false;

    [HideInInspector] public Vector3 grapplePoint;
    [HideInInspector] public Vector3 toGrapplePoint;
    [HideInInspector] public float distanceToGrapplePoint;

    // Private Members
    private FPSKinematicBody kb;
    private FPSLook look;
    private FPSMovement movement;

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

    private PlayerStats playerStats;

    private float timer = 0;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
    }

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

        look.UnlockRotation();
    }

    private void EngageGrapple()
    {
        state = GrappleState.ENGAGED;
    }

    private void DeactivateGrapple()
    {
        state = GrappleState.INACTIVE;
        Destroy(grappleTransform.gameObject);
    }

    private bool ActivateGrapple()
    {
        if (seekerWorldPoint != Vector3.zero)
        {
            grapplePoint = seekerWorldPoint;
            toGrapplePoint = grapplePoint - transform.position;
            distanceToGrapplePoint = toGrapplePoint.magnitude;
            toGrapplePoint.Normalize();

            if(distanceToGrapplePoint <= 2.1f) return false;

            state = GrappleState.ENGAGING;
            grappleAvailable = false;
            onCooldown = true;
            engagementDistanceTravelled = 0f;
            grappleTransform = Instantiate(grapplePrefab).transform;
            look.LockRotation();

            HideSeeker();

            return true;
        }

        return false;
    }

    private void CheckForBreak()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, toGrapplePoint, out hit, Mathf.Infinity, ~(1 << 10)))
        {
            if (Mathf.Abs(hit.point.sqrMagnitude - grapplePoint.sqrMagnitude) > 0.3f)
                DisengageGrapple();
        }

        if (currentToMaxSpeedRatio > 0.3f)
        {
            float velocityDot = Vector3.Dot(kb.velocity.normalized, toGrapplePoint.normalized);

            if (velocityDot <= 0.4f)
                DisengageGrapple();
        }
    }

    private void AlignPlayerWithVelocity()
    {
        if (currentToMaxSpeedRatio < 0.2f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(toGrapplePoint, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSmoothness*currentToMaxSpeedRatio);
    }

    private void ResetPlayerRotation()
    {
        Vector3 flatPlayerForward = transform.forward;
        flatPlayerForward.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(flatPlayerForward, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationTransitionSmoothness);
    }

    void Accelerate()
    {
        Vector3 velocityToGrapple = Vector3.Project(kb.velocity, toGrapplePoint);
        float velocityToGrappleDot = Vector3.Dot(kb.velocity.normalized, toGrapplePoint);

        if (velocityToGrapple.magnitude < grappleSpeed || velocityToGrappleDot <= 0f)
        {
            kb.velocity -= velocityToGrapple;
            velocityToGrapple += toGrapplePoint.normalized*toPointAcceleration*Time.fixedDeltaTime;
            if (velocityToGrapple.magnitude > grappleSpeed && velocityToGrappleDot > 0f)
                velocityToGrapple = velocityToGrapple.normalized*grappleSpeed;
            kb.velocity += velocityToGrapple;
        }

        Vector3 right = Vector3.Cross(toGrapplePoint, Vector3.up);
        Vector3 up = Vector3.Cross(toGrapplePoint, right);

        kb.velocity += right*-look.grappleLookDir.x*toMouseAcceleration*Time.fixedDeltaTime;
        kb.velocity += up*-look.grappleLookDir.y*toMouseAcceleration*Time.fixedDeltaTime;

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
    bool lastGrapple = false;
    private void HandleInput()
    {
        previousState = state;

        if (state == GrappleState.INACTIVE)
        {
            if (FPSInput.grappleDown && !lastGrapple && grappleAvailable)
            {
                state = GrappleState.SEEKING;
                ShowSeeker();
            }
        } 
        else if (state == GrappleState.SEEKING)
        {
            if (!FPSInput.grappleDown)
            {
                bool success = ActivateGrapple();
                if (!success)
                {
                    state = GrappleState.INACTIVE;
                    HideSeeker();
                }
            }
            else if (FPSInput.rightMouseDown)
            {
                state = GrappleState.INACTIVE;
                HideSeeker();
            }
        }
        else if (state == GrappleState.ENGAGED)
        {
            /* Detect if cancel key is down/pressed */
            bool cancel = FPSInput.spaceDown || FPSInput.rightMouseDown || playerStats.isDead;
            if (cancel)
                DisengageGrapple();
        }

        lastGrapple = FPSInput.grappleDown;
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
            {
                Accelerate();
                CheckForBreak();
                AlignPlayerWithVelocity();

                if (distanceToGrapplePoint <= grappleDisengageDistance)
                    DisengageGrapple();
            }
        }
        else if (state == GrappleState.SEEKING)
        {
            PositionSeeker();
        }
        else
        {
            ResetPlayerRotation();
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
        Vector3 velocity = kb.velocity;
        float dot = Vector3.Dot(velocity.normalized, toGrapplePoint);
        if (dot < 0f)
            dot = 0f;
        velocity *= dot;
        currentToMaxSpeedRatio = velocity.magnitude/grappleSpeed;
        currentToMaxSpeedRatio = Mathf.Clamp01(currentToMaxSpeedRatio);
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
