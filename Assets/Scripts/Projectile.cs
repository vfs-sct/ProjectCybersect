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
    private GameObject releasee = null;

    public void Release(Vector3 dir, GameObject _releasee)
    {
        releasee = _releasee;
        rb = GetComponent<Rigidbody>();
        rb.velocity = dir.normalized*speed;
        transform.position = _releasee.transform.position;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "player")
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            playerStats.TakeDamage(damage);
        }
        else if (releasee != other.gameObject)
        {
            Destroy(gameObject);
        }
    }
}
