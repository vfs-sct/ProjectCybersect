using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{  
    [Header("Grappling")]
    [SerializeField] private GameObject grapplePrefab;
    [SerializeField] private float grappleAcceleration = 50.0f;
    [SerializeField] private float grappleSpeed = 25.0f;
    [SerializeField, Range(0, 1)] private float grapplePerpendicularVelocityDecayAlpha = 0.1f;
    [SerializeField] private float grappleDisengageDistance = 1.0f;

    public bool grappling { get; private set; }

    private FPSKinematicBody kb;
    private FPSInput input;

    private Vector3 grapplePoint;
    private Transform grappleTransform;
    private Transform cameraTransform;

    private void Start()
    {
        kb = GetComponent<FPSKinematicBody>();
        input = GetComponent<FPSInput>();

        cameraTransform = transform.GetChild(0);
    }

    private void DisengageGrapple()
    {
        if (grappling)
        {
            Destroy(grappleTransform.gameObject);
            grappling = false;
        }
    }

    private void EngageGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity, ~(1 << 10)))
        {
            grappling = true;
            grapplePoint = hit.point;
            grappleTransform = Instantiate(grapplePrefab).transform;
        }
    }

    private void CheckForBreak()
    {
        Vector3 toGrapplePoint = grapplePoint - transform.position;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, toGrapplePoint, out hit, Mathf.Infinity, ~(1 << 10)))
        {
            if (!Mathf.Approximately(Vector3.Distance(transform.position, hit.point), toGrapplePoint.magnitude))
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
        Vector3 toGrapple = (grapplePoint - transform.position).normalized;
        float distanceToGrapplePoint = Vector3.Distance(transform.position, grapplePoint);

        if (distanceToGrapplePoint <= grappleDisengageDistance)
            DisengageGrapple();

        Vector3 velocity = new Vector3(kb.velocityX, kb.velocityY, kb.velocityZ);
        Vector3 velocityToGrapple = Vector3.Project(velocity, toGrapple);
        float velocityToGrappleDot = Vector3.Dot(velocity.normalized, toGrapple);
        if (velocityToGrapple.magnitude < grappleSpeed || velocityToGrappleDot <= 0.0f)
        {
            velocity -= velocityToGrapple;
            velocityToGrapple += toGrapple*grappleAcceleration*Time.fixedDeltaTime;
            if (velocityToGrapple.magnitude > grappleSpeed && velocityToGrappleDot > 0.0f)
                velocityToGrapple = velocityToGrapple.normalized*grappleSpeed;
            velocity += velocityToGrapple;
        }
        
        if (!Mathf.Approximately(velocityToGrappleDot, 1.0f))
        {
            Vector3 rightPerpendicularVelocityToGrapple = Vector3.Project(velocity, transform.right);
            Vector3 upPerpendicularToGrapple = Vector3.Cross(toGrapple, transform.right);
            Vector3 upPerpendicularVelocityToGrapple = Vector3.Project(velocity, upPerpendicularToGrapple);

            velocity -= rightPerpendicularVelocityToGrapple*grapplePerpendicularVelocityDecayAlpha;
            velocity -= upPerpendicularVelocityToGrapple*grapplePerpendicularVelocityDecayAlpha;
        }

        kb.velocityX = velocity.x;
        kb.velocityY = velocity.y;
        kb.velocityZ = velocity.z;
    }

    private void UpdateGrappleTransform()
    {
        if (!grappling)
            return;

        Vector3 velocity = new Vector3(kb.velocityX, kb.velocityY, kb.velocityZ);
        Vector3 predictedPosition = transform.position + velocity*Time.fixedDeltaTime;

        Vector3 toGrapplePoint = grapplePoint - predictedPosition;

        grappleTransform.rotation = Quaternion.LookRotation(toGrapplePoint);
        grappleTransform.position = (predictedPosition + grapplePoint)/2.0f;
        grappleTransform.localScale = new Vector3(1.0f, 1.0f, Vector3.Distance(predictedPosition, grapplePoint));
    }

    bool lastGrappleDown = false;
    private void Grappling()
    {
        /* User input grapple engage/disengage logic */
        if (input.grappleDown && !lastGrappleDown)
            EngageGrapple();
        else if (!input.grappleDown && lastGrappleDown)
            DisengageGrapple();

        if (grappling && input.spaceDown)
            DisengageGrapple();

        if (grappling)
        {
            CheckForBreak();
            AlignPlayerToGrapplePoint();
            ComputeVelocity();
        }

        lastGrappleDown = input.grappleDown;
    }

    private void FixedUpdate()
    {
        Grappling();
        UpdateGrappleTransform();
    }
}
