// Copyright (c) 2020 by Yuya Yoshino

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Gun : MonoBehaviour
{
    [Header("Gun Effects")]
    [SerializeField] private Camera _playerCam = null;
    [SerializeField] private ParticleSystem _muzzleFlash = null;
    [SerializeField] private GameObject _impactEffect = null;
    [SerializeField] private float _impactTime = 2f;

    [Header("Gun Stats")]
    [SerializeField] private float _maxDamage = 10f;
    [SerializeField] private float _minDamage = 5f;
    [SerializeField] private float _rangeOffStart = 30f;
    [SerializeField] private float _rangeOffEnd = 50f;
    [SerializeField] private float _range = 100f;
    [SerializeField] private float _impactForce = 0f;
    [SerializeField] private float _fireRate = 15f;
    [SerializeField] private float _spreadAngle = 5f;

    [Header("Game Manager")]
    [SerializeField] private GameManager gameManager = null;

    private AudioSource gunShot = null;
    private ProceduralGunAnimation gunAnimation = null;
    private PlayerAmmo playerAmmo = null;
    private Grapple playerGrapple = null;

    private float TimeToFire = 0f;

    private void Awake()
    {
        gunShot = GetComponent<AudioSource>();
        gunAnimation = GetComponent<ProceduralGunAnimation>();
        playerAmmo = GetComponentInParent<PlayerAmmo>();
        playerGrapple = GetComponentInParent<Grapple>();
    }

    private void Update()
    {
        if(playerAmmo.currentARAmmo <= 0) return;
        //checks the fire input and firerate
        //also checks to see if the game is paused
        if(Input.GetButton("Fire1") && Time.time >= TimeToFire && !gameManager.isPaused)
        {
            TimeToFire = Time.time + 1f/_fireRate;
            Shoot();
        }
    }

    private float DamageFallOff(RaycastHit hit)
    {
        if (hit.distance <= _rangeOffStart) return _maxDamage;
        if (hit.distance >= _rangeOffEnd) return _minDamage;

        float fallOffRange = _rangeOffEnd - _rangeOffStart;
        float normalizedDistance = (hit.distance - _rangeOffStart) / fallOffRange;

        return Mathf.Round(Mathf.Lerp(_maxDamage, _minDamage, normalizedDistance));
    }

    private void Shoot()
    {
        gunAnimation.ApplyRecoil();
        _muzzleFlash.Play();
        _muzzleFlash.Play();
        playerAmmo.currentARAmmo--;
        gunShot.Play();

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
                critTarget.TakeDamage(DamageFallOff(hit));
            }

            Target target = hit.transform.GetComponent<Target>();
            if(target != null)
            {
                target.TakeDamage(DamageFallOff(hit));
            }

            Barrel barrel = hit.transform.GetComponent<Barrel>();
            if(barrel != null)
            {
                barrel.TakeDamage(_maxDamage);
            }

            if(hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * _impactForce);
            }

            GameObject Impact = Instantiate(_impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Impact.transform.SetParent(hit.transform);
            Destroy(Impact, _impactTime);
        }

    }
}
