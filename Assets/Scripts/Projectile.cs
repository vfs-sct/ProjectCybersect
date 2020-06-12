using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public float damage;
    public float timeout = 5f;

    private float timer = 0f;

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

        if (timer > timeout)
            Destroy(gameObject);
        timer += Time.deltaTime;
    }
}
