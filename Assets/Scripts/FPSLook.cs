using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSLook : MonoBehaviour
{
    [SerializeField] private float rotationMultiplier = 1.0f;
    [SerializeField] private float verticalRotationMultiplier = 1.0f;
    [SerializeField] private float maxPitch = 85;
    [SerializeField] private float minPitch = -70;

    private FPSInput input;
    private Transform viewTransform;
    private float cameraPitch = 0.0f;

    private void LockAndHideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
        input = GetComponent<FPSInput>();
        LockAndHideCursor();
        viewTransform = transform.GetChild(0);
    }

    private void Update()
    {

        float deltaYaw = input.mouseDeltaX*rotationMultiplier;
        float deltaPitch = -input.mouseDeltaY*rotationMultiplier*verticalRotationMultiplier;

        transform.Rotate(new Vector3(0.0f, deltaYaw, 0.0f));

        cameraPitch += deltaPitch;
        cameraPitch = Mathf.Clamp(cameraPitch, minPitch, maxPitch);

        viewTransform.localRotation = Quaternion.Euler(cameraPitch, 0.0f, 0.0f);
    }
}
