using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSGroundCheck : MonoBehaviour
{
    [SerializeField] private float groundCheckHeight = 0.05f;
    [SerializeField] private float groundCheckErrorCompensation = 0.01f;

    public bool grounded;

    private FPSKinematicBody kb;
    private CapsuleCollider capsuleCollider;
    private Vector3 boxcastOffset;

    private void Start()
    {
        kb = GetComponent<FPSKinematicBody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        float boxcastYOffset = -capsuleCollider.height/2.0f - groundCheckHeight/2.0f + groundCheckErrorCompensation;
        boxcastOffset = new Vector3(0.0f, boxcastYOffset, 0.0f);
    }

    private void CheckIfGrounded()
    {
        /* vertical offset for boxcast */
        Vector3 center = transform.position + boxcastOffset;
        Vector3 halfExtents = new Vector3(0.3f, groundCheckHeight/2.0f, 0.3f);
        /* 1<<9, all ground objects are on 9th layer */
        grounded = Physics.CheckBox(center, halfExtents, Quaternion.identity, 1 << 8);
        /* Because the box check may happen faster than the player can leave the ground when jumping,
           we have to check if the player has a positive velocity, which would indicate a jump happened */
    }

    private void FixedUpdate()
    {
        CheckIfGrounded();
    }
}
