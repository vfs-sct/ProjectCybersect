// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    public float health = 50f;

    [Header("Input")]
    [SerializeField] private AudioSource _explosionSound = null;
    [SerializeField] private ParticleSystem _explosionEffect = null;
    [SerializeField] private int _explosionRadius = 400;

    private SphereCollider _sphereCollider = null;

    private void Awake()
    {
        _sphereCollider = GetComponent<SphereCollider>();
        _sphereCollider.radius = 1;
    }

    private void Update()
    {
        CheckDeath();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
    }

    private void CheckDeath()
    {
        if(health <= 0f)
        {
            _sphereCollider.radius = _explosionRadius;
            _sphereCollider.isTrigger = true;
            _explosionSound.Play();
            _explosionEffect.Play();
            Destroy(gameObject,0.1f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Target target = other.transform.GetComponent<Target>();
        PlayerStats player = other.transform.GetComponent<PlayerStats>();

        if(target != null)
        {
            target.TakeDamage(200f);
        }

        if(player != null)
        {
            player.BreakShield();
            player.TakeDamage(50f);
        }
    }
}
