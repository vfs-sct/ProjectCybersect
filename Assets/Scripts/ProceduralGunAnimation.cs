using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGunAnimation : MonoBehaviour
{
    // Serialized Data
    [SerializeField] private float transitionAlpha = 10f;

    [Header("Sway")]
    [SerializeField] private float rollAngleRange = 10f;
    [SerializeField] private float pitchAngleRange = 5f;
    [SerializeField] private float smoothness = 0.14f;

    [Header("Bob")]
    [SerializeField] private float bobOffset = 1f;

    [Header("Breathing")]
    [SerializeField] private float breathOffset = 1.2f;
    [SerializeField] private float breathFrequency = 0.2f;

    [Header("Delay")]
    [SerializeField] private float delaySmoothness = 0.15f;
    [SerializeField] private float delayOffset = 0.5f;

    // Private Members
    private FPSGroundCheck groundCheck;
    private FPSKinematicBody kinematicBody;

    private void Start()
    {
        GameObject player = GameObject.Find("player");
        groundCheck = player.GetComponent<FPSGroundCheck>();
        kinematicBody = player.GetComponent<FPSKinematicBody>();

        bobOffset /= 100f;
        breathOffset /= 100f;
        delayOffset /= 10f;
    }

    private Quaternion ComputeSway()
    {
        Vector3 angles = new Vector3();

        float rollAngle = (float)System.Math.Tanh(-FPSInput.mouseDeltaX*smoothness)*rollAngleRange;
        float pitchAngle = (float)System.Math.Tanh(-FPSInput.mouseDeltaY*smoothness)*pitchAngleRange;
        angles.z = rollAngle;
        angles.x = pitchAngle;

        return Quaternion.Euler(angles);
    }

    private Vector3 ComputeBob()
    {
        Vector3 bob = new Vector3();

        if (PlayerState.state == PlayerStates.MOVING)
        {
            float sinArg = Mathf.PI*FPSMovement.footstepFrequency*Time.time;
            float sin = Mathf.Sin(sinArg)*bobOffset;

            bob.y = Mathf.Abs(sin)*2f;
            bob.x = sin;
        }

        return bob;
    }

    private Vector3 ComputeBreathing()
    {
        Vector3 breathing = new Vector3();

        float sinArg = Mathf.PI*Time.time*breathFrequency;
        float sin = Mathf.Sin(sinArg);
        /* Creates pronounced peaks resulting in more realistic breathing */
        sin *= sin*sin;
        sin = Mathf.Abs(sin);
        breathing.y = sin*breathOffset;

        return breathing;
    }

    private Vector3 ComputeAirborneDelay()
    {
        Vector3 delay = new Vector3();

        delay.y = (float)System.Math.Tanh(-kinematicBody.velocityY*delaySmoothness)*delayOffset;

        return delay;
    }

    private void LateUpdate()
    {
        Quaternion targetRotation = Quaternion.identity;
        Vector3 targetPosition = Vector3.zero;

        targetRotation *= ComputeSway();

        targetPosition += ComputeBob();
        targetPosition += ComputeBreathing();
        targetPosition += ComputeAirborneDelay();

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, 
                                               transitionAlpha*Time.deltaTime);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, 
                                                  transitionAlpha*Time.deltaTime);
    }
}
