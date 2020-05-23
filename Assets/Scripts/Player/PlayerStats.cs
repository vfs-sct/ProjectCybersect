// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float _currentHealth = 100f;
    [SerializeField] private float _maxHealth = 100f;

    [Header("Shield")]
    [SerializeField] private int _currentShield = 4;
    [SerializeField] private int _maxShield = 4;

    [Header("Boost")]
    [SerializeField] private int _currentBoost = 3;
    [SerializeField] private int _maxBoost = 3;

    public float healthPercent = 100f;
    public float shieldPercent = 1f;
    public float boostPercent = 1f;

    private void Update()
    {
        KillPlayer();
        healthPercent = _currentHealth / _maxHealth;
        shieldPercent = (float)_currentShield / (float)_maxShield;
        boostPercent = (float)_currentBoost / (float)_maxBoost;
    }

    private void KillPlayer()
    {
        if(healthPercent <= 0)
        {
            Destroy(gameObject);
        }
    }
    
    public int ReadBoost()
    {
        return _currentBoost;
    }

    public void UseBoost()
    {
        if(_currentBoost >= 1)
        {
            --_currentBoost;
        }
    }

    public void AddBoost()
    {
        if(_currentBoost < _maxBoost)
        {
            ++_currentBoost;
        }
    }

    public void UseShield()
    {
        if(_currentShield >= 1)
        {
            --_currentShield;
        }
    }

    public void AddShield()
    {
        if(_currentShield < _maxShield)
        {
            ++_currentShield;
        }
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
    }

    public void HealDamage(float health)
    {
        if(_currentHealth < _maxHealth)
        _currentHealth += health;
    }
}
