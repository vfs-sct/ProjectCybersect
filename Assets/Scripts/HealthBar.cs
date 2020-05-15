// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private PlayerStats _player;

    private Image _healthBar;

    private void Start()
    {
        _healthBar = GetComponent<Image>();
    }

    private void Update()
    {
        // if it can't find the player, set hp to 0
        if(_player == null)
        {
            _healthBar.fillAmount = 0f;
            return;
        }

        // set fill to health percent
        _healthBar.fillAmount = _player.healthPercent;
    }
}
