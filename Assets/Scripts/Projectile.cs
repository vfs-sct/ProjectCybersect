using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public float damage;

    private Rigidbody rb;

    public void Release(Vector3 dir)
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = dir.normalized*speed;
    }

    private void AlignToVelocity()
    {
        transform.rotation = Quaternion.LookRotation(Vector3.down, rb.velocity);
    }

    private void LateUpdate()
    {
        AlignToVelocity();
    }
}
