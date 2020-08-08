// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickUp : MonoBehaviour
{
    [SerializeField] private int _shieldAmount = 1;
    [SerializeField] private float _healthAmount = 25f;
    [SerializeField] private float _rotationSpeed = 1f;

    private void Update()
    {
        //rotate pickup
        gameObject.transform.Rotate(Vector3.up * (_rotationSpeed * Time.deltaTime));
    }

    private void OnTriggerEnter(Collider other)
    {
        //check for player
        if(other.CompareTag("Player"))
        {
            PickUp(other);
        }
    }

    private void PickUp(Collider player)
    {
        PlayerStats playerStats = player.GetComponent<PlayerStats>();

        playerStats.Shield(_shieldAmount);
        playerStats.HealDamage(_healthAmount);

        Destroy(gameObject);
    }
}
