using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSInput : MonoBehaviour
{
    [SerializeField] private KeyCode grappleKey = KeyCode.F;

    public static int movementX;
    public static int movementZ;

    public static float mouseDeltaX;
    public static float mouseDeltaY;
    public static bool leftMouseDown;
    public static bool rightMouseDown;

    public static bool spaceDown = false;
    public static bool grappleDown = false;
    public static bool shiftDown = false;

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

        leftMouseDown = Input.GetMouseButton(0);
        rightMouseDown = Input.GetMouseButton(1);
    }

    private void Update()
    {
        SetCurrentKeyboardState();
        SetCurrentMouseState();
    }
}
