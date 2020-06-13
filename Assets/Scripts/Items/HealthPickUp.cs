// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickUp : MonoBehaviour
{
    [SerializeField] private bool _shield = false;
    [SerializeField] private int _shieldAmount = 1;
    [SerializeField] private float _healthAmount = 25f;
    [SerializeField] private float _rotationSpeed = 1f;

    private void Update()
    {
        gameObject.transform.Rotate( Vector3.up * (_rotationSpeed * Time.deltaTime));
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            PickUp(other);
        }
    }

    private void PickUp(Collider player)
    {
        PlayerStats playerStats = player.GetComponent<PlayerStats>();

        if(_shield)
        {
            playerStats.Shield(_shieldAmount);
        }
        else
        {
            playerStats.HealDamage(_healthAmount);
        }

        Destroy(gameObject);
    }
}
