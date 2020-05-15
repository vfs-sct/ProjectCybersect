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
    public int shieldPercent = 1;
    public int boostPercent = 1;

    private void Update()
    {
        healthPercent = _currentHealth / _maxHealth;
        shieldPercent = _currentShield / _maxShield;
        boostPercent = _currentBoost / _maxBoost;
    }
}
