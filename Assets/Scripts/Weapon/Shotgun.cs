// Copyright (c) 2020 by Yuya Yoshino

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Shotgun : MonoBehaviour
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
    [SerializeField] private int _pellets = 7;
    [SerializeField] private float _spreadAngle = 5f;

    private AudioSource gunShot = null;
    private GameManager gameManager = null;
    private PlayerAmmo playerAmmo = null;
    private float timeToFire = 0f;

    private void Awake()
    {
        gunShot = GetComponent<AudioSource>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerAmmo = GameObject.Find("player").GetComponent<PlayerAmmo>();
    }

    private void Update()
    {
        if(playerAmmo.currentSGAmmo <= 0) return;
        //checks the fire input and firerate
        //also checks to see if the game is paused
        if(Input.GetButton("Fire1") && Time.time >= timeToFire && !gameManager.isPaused)
        {
            timeToFire = Time.time + 1f/_fireRate;
            Shoot();
        }
    }

    private void Shoot()
    {
        _muzzleFlash.Play();
        playerAmmo.currentSGAmmo--;
        gunShot.Play();

        for(int i = 0; i < _pellets; i++)
        {
            //shot spread
            Quaternion rotation = Quaternion.LookRotation(_playerCam.transform.forward);
            Quaternion randomRotation = Random.rotation;
            rotation = Quaternion.RotateTowards(rotation, randomRotation, Random.Range(0f, _spreadAngle));

            //checks raycast hit
            RaycastHit hit;
            if(Physics.Raycast(_playerCam.transform.position, rotation * Vector3.forward, out hit, _range))
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
                //Destroy(Impact, 0.1f);
            }
        }
    }
}