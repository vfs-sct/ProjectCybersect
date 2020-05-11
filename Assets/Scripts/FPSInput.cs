using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSInput : MonoBehaviour
{
    [SerializeField] private KeyCode grappleKey = KeyCode.F;

    [HideInInspector] public int movementX;
    [HideInInspector] public int movementZ;

    [HideInInspector] public float mouseDeltaX;
    [HideInInspector] public float mouseDeltaY;

    [HideInInspector] public bool spaceDown = false;
    [HideInInspector] public bool grappleDown = false;
    [HideInInspector] public bool shiftDown = false;

    private void SetCurrentKeyboardState()
    {
        movementX = 0;
        movementZ = 0;

        if (Input.GetKey(KeyCode.D))
            ++movementX;
        if (Input.GetKey(KeyCode.A))
            --movementX;
        if (Input.GetKey(KeyCode.W))
            ++movementZ;
        if (Input.GetKey(KeyCode.S))
            --movementZ;

        spaceDown = Input.GetKey(KeyCode.Space);
        shiftDown = Input.GetKey(KeyCode.LeftShift);
        grappleDown = Input.GetKey(grappleKey);
    }

    private void SetCurrentMouseState()
    {
        mouseDeltaX = Input.GetAxisRaw("Mouse X");
        mouseDeltaY = Input.GetAxisRaw("Mouse Y");
    }

    private void Update()
    {
        SetCurrentKeyboardState();
        SetCurrentMouseState();
    }
}
