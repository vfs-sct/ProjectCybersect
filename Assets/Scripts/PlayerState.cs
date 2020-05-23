using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerStates
{
    IDLE,
    MOVING,
    GRAPPLING,
    AIRBORNE
}

public class PlayerState : MonoBehaviour
{
    public static PlayerStates state = PlayerStates.IDLE;
    public static PlayerStates previousState = PlayerStates.IDLE;

    private FPSGroundCheck groundCheck;
    private FPSMovement movement;
    private Grapple grapple;

    private void Start()
    {
        movement = GetComponent<FPSMovement>();
        groundCheck = GetComponent<FPSGroundCheck>();
        grapple = GetComponent<Grapple>();
    }

    private void ComputeState()
    {
        if (grapple.state != GrappleState.INACTIVE && grapple.state != GrappleState.SEEKING)
            state = PlayerStates.GRAPPLING;
        else if (!groundCheck.grounded)
            state = PlayerStates.AIRBORNE;
        else if (FPSInput.movementZ != 0 || FPSInput.movementX != 0)
            state = PlayerStates.MOVING;
        else
            state = PlayerStates.IDLE;
    }

    private void FixedUpdate()
    {
        previousState = state;
        ComputeState();
    }
}
