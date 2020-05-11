using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed = 5.0f;
    [SerializeField, Range(0, 100)] private float decelerationPercent = 60.0f;
    [SerializeField] private float movementAcceleration = 15.0f;
    [SerializeField] private float airMovementMultiplier = 0.1f;
    [SerializeField] public const float footstepFrequency = 2.0f;

    [Header("Boost")]
    [SerializeField] private float verticalBoostPower = 8.0f;
    [SerializeField] private float horizontalBoostPower = 12.0f; 
    [SerializeField] private float horizontalBoostAngle = 10;

    [Header("Jump")]
    [SerializeField] private float jumpPower = 3.0f;

    private FPSGroundCheck groundCheck;
    private FPSKinematicBody kb;
    private FPSInput input;
    private float horizontalBoostYOffset;

    private void ConvertDecelerationPercentToUsableConstant()
    {
        decelerationPercent = 1 - (decelerationPercent/100.0f);
    }

    private void CalculateHorizontalBoostYOffset()
    {
        horizontalBoostYOffset = Mathf.Tan(Mathf.Deg2Rad*horizontalBoostAngle);
    }

    private void Start()
    {
        kb = GetComponent<FPSKinematicBody>();
        input = GetComponent<FPSInput>();
        groundCheck = GetComponent<FPSGroundCheck>();

        ConvertDecelerationPercentToUsableConstant();
        CalculateHorizontalBoostYOffset();
    }

    private float AccelerationFunction(float x, int power, float constant)
    {
        if (x < 1)
            return 1 + constant;
        else
            return Mathf.Abs(Mathf.Pow(x - 1, power) + 1) + constant;
    }

    private void HorizontalMovement()
    {
        if (groundCheck.grounded)
        {
            Vector3 velocity = new Vector3(kb.velocityX, 0.0f, kb.velocityZ);
            velocity = Quaternion.Inverse(transform.rotation)*velocity;

            Vector2 targetVelocity = new Vector2(input.movementX, input.movementZ).normalized*movementSpeed;
            Vector2 currentVelocity = new Vector2(velocity.x, velocity.z);
            Vector2 difference = targetVelocity - currentVelocity;

            float distance = difference.magnitude;
            if (distance > float.Epsilon)
            {
                float constant = 0.0f;
                if (targetVelocity == Vector2.zero)
                    constant = -decelerationPercent;

                float accelerationMultiplier = AccelerationFunction(currentVelocity.magnitude/movementSpeed, 2, constant); 
                float deltaMagnitude = accelerationMultiplier*movementAcceleration*Time.fixedDeltaTime;
                float deltaToDistanceRatio = deltaMagnitude/distance;
                if (deltaToDistanceRatio > 1.0f)
                    deltaToDistanceRatio = 1.0f;

                velocity.x += difference.x*deltaToDistanceRatio;
                velocity.z += difference.y*deltaToDistanceRatio;
            }

            velocity = transform.rotation*velocity; 
            kb.velocityX = velocity.x;
            kb.velocityZ = velocity.z;
        }
        else
        {
            Vector3 deltaVelocity = new Vector3(input.movementX, 0, input.movementZ);
            deltaVelocity.Normalize();
            deltaVelocity = transform.rotation*deltaVelocity;
            deltaVelocity *= airMovementMultiplier;
            kb.velocityX += deltaVelocity.x;
            kb.velocityZ += deltaVelocity.z;
        }
    }

    bool lastSpaceDown = false;
    private void Jumping()
    {
        if (input.spaceDown && !lastSpaceDown && groundCheck.grounded)
            kb.velocityY = jumpPower;

        lastSpaceDown = input.spaceDown;
    }

    bool lastShiftDown = false;
    private void Boosting()
    {
        if (input.shiftDown && !lastShiftDown)
        {
            if (input.movementZ != 0 || input.movementX != 0)
            {
                Vector3 boost = new Vector3(input.movementX, 0.0f, input.movementZ).normalized;
                boost = transform.rotation*boost;
                if (groundCheck.grounded)
                {
                    boost.y = horizontalBoostYOffset;
                    boost.Normalize();
                }
                boost *= horizontalBoostPower;

                kb.velocityX = boost.x;
                kb.velocityZ = boost.z;
                kb.velocityY = boost.y;
            }
            else
            {
                kb.velocityY = verticalBoostPower;
            }
        }

        lastShiftDown = input.shiftDown;
    }

    private void FixedUpdate()
    {
        HorizontalMovement();
        Jumping();
        Boosting();
    }
}
