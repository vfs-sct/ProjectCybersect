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

    [Header("Landing")]
    public float landingShakeAmount = 2f;
    public float landingShakeSmoothness = 1.0f;

    private Grapple grapple;
    private FPSKinematicBody kinematicBody;
    private Camera cameraComponent;

    private float cameraShake = 0.0f;

    private float pitch = 0.0f;
    private float yaw = 0.0f;
    private float fov;
    private float targetFov;
    private float startFov;

    private void Start()
    {
        GameObject player = GameObject.Find("player");
        grapple = player.GetComponent<Grapple>();
        kinematicBody = player.GetComponent<FPSKinematicBody>();

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

    float lastVelocityY = 0f;
    private void FixedUpdate()
    {
        if (grapple.state != grapple.previousState)
        {
            if (grapple.state == GrappleState.ENGAGING)
                cameraShake = grappleActivationShakeAmount;
        }

        if (grapple.state == GrappleState.ENGAGED)
            targetFov = startFov + (grappleFOVMultiplier - 1)*startFov*grapple.currentToMaxSpeedRatio;
        else
            targetFov = startFov;

        if (PlayerState.state != PlayerState.previousState)
        {
            if (PlayerState.previousState == PlayerStates.AIRBORNE && PlayerState.state != PlayerStates.GRAPPLING)
                cameraShake = (float)System.Math.Tanh(-lastVelocityY*landingShakeSmoothness)*landingShakeAmount;
        }

        lastVelocityY = kinematicBody.velocity.y;
    }
}