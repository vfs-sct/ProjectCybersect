using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSKinematicBody : MonoBehaviour
{
    public const float gravity = 9.81f;

    [SerializeField, Range(0, 1)] private float frictionAlpha = 0.9f;

    [HideInInspector] public Vector3 velocity = Vector3.zero; 
    [HideInInspector] public float gravityMultiplier = 1.0f;

    private FPSGroundCheck groundCheck;
    private Collider objectCollider;
    private Collider[] colliders;

    private void Start()
    {
        objectCollider = GetComponent<Collider>();
        colliders = FindObjectsOfType<Collider>();
        groundCheck = GetComponent<FPSGroundCheck>();
    }

    private void Gravity()
    {
        velocity.y -= gravity*Time.fixedDeltaTime*gravityMultiplier;
    }

    private void MoveObject()
    {
        Vector3 deltaPosition = velocity*Time.fixedDeltaTime;
        transform.position += deltaPosition;
    }

    private void ComputeCollisionVelocity(Vector3 normal)
    {
        Vector3 velocityProjectedOnNormal = Vector3.Project(velocity, normal);
        velocity -= velocityProjectedOnNormal;

        if (!groundCheck.grounded)
        {
            velocity.x = frictionAlpha*velocity.x;
            velocity.z = frictionAlpha*velocity.z;
            if (velocity.y > 0.0f)
                velocity.y = frictionAlpha*velocity.y;
        }
    }

    private void CheckForCollisions()
    {
        foreach (Collider collider in colliders)
        {   
            if(collider == null)
                continue;
            if (collider == objectCollider)
                continue;
            if (collider.isTrigger)
                continue;

            Vector3 clearDirection;
            float clearDistance;
            bool result = Physics.ComputePenetration(objectCollider, transform.position, transform.rotation, 
                                                     collider, collider.transform.position, collider.transform.rotation, 
                                                     out clearDirection, out clearDistance);
            if (result)
            {
                transform.position += clearDirection*clearDistance;
                ComputeCollisionVelocity(clearDirection);
            }
        }
    }

    private void FixedUpdate()
    {
        Gravity();
        MoveObject();
        CheckForCollisions();
    }
}
