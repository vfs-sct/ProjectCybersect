using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnimator : MonoBehaviour
{
    [Header("Shake")]
    public float shakeFreqency = 6.0f;
    public float shakeHalfLife = 0.05f;

    [Header("Grappling")]
    public float grappleActivationShakeAmount = 2.5f;
    public float grappleFOVMultiplier = 1.3f;
    public float grappleFOVAlpha = 0.1f;

    private Grapple grapple;
    private Camera cameraComponent;

    private float cameraShake = 0.0f;

    private float pitch = 0.0f;
    private float yaw = 0.0f;
    private float fov;
    private float targetFov;
    private float startFov;

    private void Start()
    {
        grapple = GameObject.Find("player").GetComponent<Grapple>();
        cameraComponent = GetComponent<Camera>();
        startFov = cameraComponent.fieldOfView;
        fov = startFov;
        targetFov = fov;
    }

    private float HalfLife(float x, float dt, float halfLife)
    {
        return x*Mathf.Pow(1.0f/2.0f, dt/halfLife);
    }

    float sinTimer = 0.0f;
    private void CalculateCameraShake()
    {
        if (cameraShake < 0.01f)
            return;

        float shakeX = Mathf.Sin(sinTimer*shakeFreqency*Mathf.PI)*cameraShake;
        float shakeY = Mathf.Sin(sinTimer*shakeFreqency*2.0f*Mathf.PI)*cameraShake;
        pitch += shakeX;
        yaw += shakeY;

        cameraShake = HalfLife(cameraShake, Time.deltaTime, shakeHalfLife);
        sinTimer += Time.deltaTime;
    }

    private void CalculateFOV()
    {
        if (fov == targetFov)
            return;

        fov = Mathf.Lerp(fov, targetFov, grappleFOVAlpha);
        cameraComponent.fieldOfView = fov;
    }

    private void Update()
    {
        pitch = 0;
        yaw = 0;

        CalculateCameraShake();
        CalculateFOV();

        transform.localEulerAngles = new Vector3(pitch, yaw, 0.0f);
    }

    private void FixedUpdate()
    {
        if (grapple.state != grapple.previousState)
        {
            if (grapple.state == GrappleState.ACTIVE)
                cameraShake = grappleActivationShakeAmount;
        }

        if (grapple.state == GrappleState.ENGAGED)
            targetFov = startFov + (grappleFOVMultiplier - 1)*startFov*grapple.currentToMaxSpeedRatio;
        else
            targetFov = startFov;
    }
}
/*
if (PlayerState.state != State.AIRBORNE && PlayerState.previousState == State.AIRBORNE)
{
    cameraShake = shakeAmount;
    depressionROC = landingDepressionAmount;
}

depression += depressionROC;
depression -= (float)System.Math.Tanh((double)depression*landingDepressionRecoverySmoothness)*landingDepressionRecoverySpeed;
rotation.x += depression*landingDepressionPitch;
depressionROC = HalfLife(depressionROC, Time.deltaTime, landingDepressionIncreaseHalfLife);
transform.parent.localPosition = Vector3.down*depression*landingDepressionOffset + Vector3.up*cameraHeight;*/
