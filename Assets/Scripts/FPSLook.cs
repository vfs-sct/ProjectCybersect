using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSLook : MonoBehaviour
{
    [SerializeField] private float rotationMultiplier = 1.0f;
    [SerializeField] private float verticalRotationMultiplier = 1.0f;
    [SerializeField] private float grappleRotationMultiplier = 1.0f;
    [SerializeField] private float transitionSmoothness = 0.5f;
    [SerializeField] private float grappleRotationDegrees = 8f;
    [SerializeField] private float maxPitch = 85;
    [SerializeField] private float minPitch = -70;

    public Vector2 grappleLookDir = Vector2.zero;
    public float grappleLookDirClamp = 3f;

    private Transform viewTransform;
    private Grapple grapple;

    private bool rotationLocked = false;
    private float cameraPitch = 0f;
    private float cameraYaw = 0f;
    private float transitionAlpha = 0f;
    private float normalizedGrappleLookDirSpeed;

    private void LockAndHideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
        LockAndHideCursor();
        viewTransform = transform.GetChild(0);
        grapple = GetComponent<Grapple>();
    }

    private void EvaluateTransitionAlpha()
    {
        if (rotationLocked)
            transitionAlpha = Mathf.Lerp(transitionAlpha, 1f, transitionSmoothness*grapple.currentToMaxSpeedRatio);
        else
            transitionAlpha = Mathf.Lerp(transitionAlpha, 0f, transitionSmoothness);

        transitionAlpha = Mathf.Clamp01(transitionAlpha);
    }

    private float TanhInverse(float x)
    {
        return (1f/2f)*Mathf.Log((1f + x)/(1f - x));
    }

    private float NormalizedToTanh(float x, float difference)
    {
        return -TanhInverse((float)System.Math.Tanh(x) - difference) + x;
    }

    private void Update()
    {
        if(GameManager.Instance.isPaused) return;
        
        float deltaYaw = FPSInput.mouseDeltaX*rotationMultiplier;
        float deltaPitch = -FPSInput.mouseDeltaY*rotationMultiplier*verticalRotationMultiplier;

        if (!rotationLocked)
        {
            transform.Rotate(new Vector3(0.0f, deltaYaw, 0.0f));

            cameraPitch += deltaPitch;
            cameraPitch = Mathf.Clamp(cameraPitch, minPitch, maxPitch);

            float yaw = Mathf.Lerp(0f, cameraYaw, transitionAlpha);
            viewTransform.localRotation = Quaternion.identity;
            viewTransform.Rotate(new Vector3(cameraPitch, 0f, 0f), Space.Self);
            viewTransform.Rotate(new Vector3(0f, yaw, 0f), Space.World);
        }
        else
        {
            Vector2 delta = new Vector2(deltaYaw*grappleRotationMultiplier,
                                        -deltaPitch*grappleRotationMultiplier);

            float sqrMagnitude = grappleLookDir.sqrMagnitude;
            Vector3 grappleDirPrime = (grappleLookDir + delta);
            float sqrMagnitudePrime = grappleDirPrime.sqrMagnitude;
            if (sqrMagnitudePrime < sqrMagnitude)
            {
                float magnitude = Mathf.Sqrt(sqrMagnitude);
                float magnitudePrime = Mathf.Sqrt(sqrMagnitudePrime);
                float targetMagnitudeDifference = magnitude - magnitudePrime;
                grappleLookDir = grappleDirPrime.normalized*(magnitude - NormalizedToTanh(magnitude, targetMagnitudeDifference));
            }
            else
            {
                grappleLookDir += delta;
            }

            if (grappleLookDir.sqrMagnitude > grappleLookDirClamp*grappleLookDirClamp)
                grappleLookDir = grappleLookDir.normalized*grappleLookDirClamp;

            cameraYaw = (float)System.Math.Tanh(grappleLookDir.x)*grappleRotationDegrees;
            float targetPitch = (float)System.Math.Tanh(grappleLookDir.y)*grappleRotationDegrees;
            float pitch = Mathf.Lerp(cameraPitch, targetPitch, transitionAlpha);

            viewTransform.localRotation = Quaternion.identity;
            viewTransform.Rotate(new Vector3(pitch, 0f, 0f), Space.Self);
            viewTransform.Rotate(new Vector3(0f, cameraYaw, 0f), Space.World);
        }
    }

    private void FixedUpdate()
    {
        EvaluateTransitionAlpha();
    }

    public void LockRotation()
    {
        if (rotationLocked)
            return;

        grappleLookDir = Vector2.zero;

        transitionAlpha = 0f;
        cameraYaw = 0f;
        rotationLocked = true;
    }

    public void UnlockRotation()
    {
        if (!rotationLocked)
            return;

        transitionAlpha = 1f;
        cameraPitch = 0f;
        rotationLocked = false;
    }
}
