// Copyright (c) 2020 by Yuya Yoshino

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Gun Effects")]
    [SerializeField] private Camera _playerCam = null;
    [SerializeField] private ParticleSystem _muzzleFlash = null;
    [SerializeField] private GameObject _impactEffect = null;

    [Header("Gun Stats")]
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _range = 100f;
    [SerializeField] private float _impactForce = 0f;
    [SerializeField] private float _fireRate = 15f;

    private GameManager gameManager = null;

    private float TimeToFire = 0f;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        //checks the fire input and firerate
        //also checks to see if the game is paused
        if(Input.GetButton("Fire1") && Time.time >= TimeToFire && !gameManager.isPaused)
        {
            TimeToFire = Time.time + 1f/_fireRate;
            Shoot();
        }
    }

    private void Shoot()
    {
        _muzzleFlash.Play();

        RaycastHit hit;
        if(Physics.Raycast(_playerCam.transform.position, _playerCam.transform.forward, out hit, _range))
        {
            CritTarget critTarget = hit.transform.GetComponent<CritTarget>();
            if(critTarget != null)
            {
                critTarget.TakeDamage(_damage);
            }

            Target target = hit.transform.GetComponent<Target>();
            if(target != null)
            {
                target.TakeDamage(_damage);
            }

            if(hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * _impactForce);
            }

            GameObject Impact = Instantiate(_impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(Impact, 0.1f);
        }

    }
}
