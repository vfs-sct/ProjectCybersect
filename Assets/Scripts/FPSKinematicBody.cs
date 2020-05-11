using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSKinematicBody : MonoBehaviour
{
    private const float gravity = 9.81f;

    [SerializeField, Range(0, 1)] private float frictionAlpha = 0.9f;

    [HideInInspector] public float velocityX = 0; 
    [HideInInspector] public float velocityY = 0;
    [HideInInspector] public float velocityZ = 0;

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
        velocityY -= gravity*Time.fixedDeltaTime;
    }

    private void MoveObject()
    {
        Vector3 deltaPosition = new Vector3(velocityX, velocityY, velocityZ)*Time.fixedDeltaTime;
        transform.position += deltaPosition;
    }

    private void HandleCollisions()
    {
        foreach (Collider collider in colliders)
        {
            if (collider == objectCollider)
                continue;

            Vector3 clearDirection;
            float clearDistance;
            bool result = Physics.ComputePenetration(objectCollider, transform.position, transform.rotation, 
                                                     collider, collider.transform.position, collider.transform.rotation, 
                                                     out clearDirection, out clearDistance);
            if (result)
            {
                transform.position += clearDirection*clearDistance;

                Vector3 velocity = new Vector3(velocityX, velocityY, velocityZ);
                if (!Mathf.Approximately(Vector3.Dot(clearDirection, velocity), -1.0f))
                {
                    Vector3 velocityProjectedOnNormal = Vector3.Project(velocity, clearDirection);
                    velocity -= velocityProjectedOnNormal;

                    if (!groundCheck.grounded)
                    {
                        velocityX = frictionAlpha*velocityX;
                        velocityZ = frictionAlpha*velocityZ;
                        if (velocityY > 0.0f)
                            velocityY = frictionAlpha*velocityY;
                    }

                    velocityX = velocity.x;
                    velocityY = velocity.y;
                    velocityZ = velocity.z;
                }
                else
                {
                    velocityX = 0;
                    velocityY = 0;
                    velocityZ = 0;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        Gravity();
        MoveObject();
        HandleCollisions();
    }
}
