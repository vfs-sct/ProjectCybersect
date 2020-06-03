// Copyright (c) 2020 by Yuya Yoshino

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private Camera _playerCam;
    [SerializeField] private ParticleSystem _muzzleFlash = null;
    [SerializeField] private GameObject _impactEffect = null;

    public float damage = 10f;
    public float range = 100f;

    private void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        _muzzleFlash.Play();

        RaycastHit hit;
        if(Physics.Raycast(_playerCam.transform.position, _playerCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

            CritTarget critTarget = hit.transform.GetComponent<CritTarget>();
            if(critTarget != null)
            {
                critTarget.TakeDamage(damage);
            }

            Target target = hit.transform.GetComponent<Target>();
            if(target != null)
            {
                target.TakeDamage(damage);
            }

            GameObject Impact = Instantiate(_impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(Impact, 2f);
        }

    }
}
