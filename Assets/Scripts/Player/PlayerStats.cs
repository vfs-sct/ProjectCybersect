// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("UI to close")]
    [SerializeField] private GameObject _pauseMenu = null;
    [SerializeField] private GameObject _debugMenu = null;

    [Header("Health")]
    [SerializeField] private float _currentHealth = 100f;
    [SerializeField] private float _maxHealth = 100f;

    [Header("Shield")]
    [SerializeField] private int _currentShield = 4;
    [SerializeField] private int _maxShield = 4;

    [Header("Boost")]
    [SerializeField] private int _currentBoost = 3;
    [SerializeField] private int _maxBoost = 3;

    [Header("Boost Recharge")]
    [SerializeField] private float _currentRechargeBoost = 3f;
    [SerializeField] private float _maxRechargeBoost = 3f;
    [SerializeField] private float _chargeTime = 2f;

    [Header("Public")]
    public bool isDead = false;
    public float healthPercent = 1f;
    public float shieldPercent = 1f;
    public float boostPercent = 1f;
    public float boostRechargePercent = 1f;

    private float boostRechargeTimer = 0f;

    private void Update()
    {
        CheckPlayer();
        UpdatePercent();
        RechargeBoost();
    }

    private void RechargeBoost()
    {
        if (_currentBoost/(float)_maxBoost > 0.9f) return;

        if (boostRechargeTimer >= 1f)
        {
            boostRechargeTimer = 0f;
            Boost(1);
        }

        boostRechargeTimer += Time.deltaTime/_chargeTime;
    }

    private void UpdatePercent()
    {
        healthPercent = _currentHealth / _maxHealth;
        shieldPercent = _currentShield / (float)_maxShield;
        boostPercent = _currentBoost / (float)_maxBoost;
        boostRechargePercent = _currentBoost/(float)_maxBoost + boostRechargeTimer*(1/3f);
    }

    private void CheckPlayer()
    {
        if((healthPercent <= 0) && !isDead)
        {
            isDead = true;
            _pauseMenu.SetActive(false);
            _debugMenu.SetActive(false);
        }
    }

    public void RespawnPlayer()
    {
        isDead = false;
        _currentHealth = _maxHealth;
        _currentShield = _maxShield;
        _currentBoost = _maxBoost;
    }

    public float ReadHealth()
    {
        return _currentHealth;
    }

    public int ReadShield()
    {
        return _currentShield;
    }
    
    public int ReadBoost()
    {
        return _currentBoost;
    }

    public void Boost(int boostAmount)
    {
        if(Mathf.Sign(boostAmount) == -1)
        {
            if(_currentBoost >= 1)
            {
                --_currentRechargeBoost;
                --_currentBoost;
            }
        }
        else
        {
            if(_currentBoost < _maxBoost)
            {
                ++_currentBoost;
            }
        }
    }

    public void Shield(int shieldAmount)
    {
        if(Mathf.Sign(shieldAmount) == -1)
        {
            if(_currentShield >= 1)
            {
                --_currentShield;
            }
        }
        else
        {
            if(_currentShield < _maxShield)
            {
                _currentShield = _currentShield + shieldAmount;
                if(_currentShield > _maxShield) _currentShield = _maxShield;
            }
        }
    }

    public void BreakShield()
    {
        _currentShield = 0;
    }

    public void TakeDamage(float damage)
    {
        if(_currentShield >= 1)
        {
            Shield(-1);
        }
        else
        {
            _currentHealth -= damage;
        }
    }

    public void HealDamage(float health)
    {
        if(_currentHealth < _maxHealth) _currentHealth += health;

        if(_currentHealth > _maxHealth) _currentHealth = _maxHealth;
    }
}
