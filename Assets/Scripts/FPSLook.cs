using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSLook : MonoBehaviour
{
    [SerializeField] private float rotationMultiplier = 1.0f;
    [SerializeField] private float verticalRotationMultiplier = 1.0f;
    [SerializeField] private float maxPitch = 85;
    [SerializeField] private float minPitch = -70;
    [SerializeField, Range(0, 1)] private float cameraYawAlpha = 0.2f;
    [SerializeField] private float cameraYawClamp = 50.0f;

    [HideInInspector] public bool rotationLocked = false;

    private Transform viewTransform;

    private float cameraPitch = 0.0f;
    private float targetCameraYaw = 0.0f;
    private float cameraYaw = 0.0f;

    private void LockAndHideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
        LockAndHideCursor();
        viewTransform = transform.GetChild(0);
    }

    private void Update()
    {
        float deltaYaw = FPSInput.mouseDeltaX*rotationMultiplier;
        float deltaPitch = -FPSInput.mouseDeltaY*rotationMultiplier*verticalRotationMultiplier;

        if (rotationLocked)
        {
            targetCameraYaw += deltaYaw;
            targetCameraYaw = Mathf.Clamp(targetCameraYaw, -cameraYawClamp, cameraYawClamp);
        }
        else
        {
            targetCameraYaw = 0.0f;
            transform.Rotate(new Vector3(0.0f, deltaYaw, 0.0f));
        }

        cameraYaw = Mathf.Lerp(cameraYaw, targetCameraYaw, cameraYawAlpha);

        cameraPitch += deltaPitch;
        cameraPitch = Mathf.Clamp(cameraPitch, minPitch, maxPitch);

        viewTransform.localRotation = Quaternion.Euler(cameraPitch, cameraYaw, 0.0f);
    }
}
