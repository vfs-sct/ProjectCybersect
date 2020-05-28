// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatBars : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerStats _player = null;

    [Header("Image")]
    [SerializeField] private Image _healthBar = null;
    [SerializeField] private Image _shieldBar = null;
    [SerializeField] private Image _boostBar = null;

    private void Update()
    {
        UpdateBar();
    }

    private void UpdateBar()
    {
        // if it can't find the player, set hp to 0
        if(_player == null)
        {
            //health
            _player.healthPercent = 0;
            _healthBar.fillAmount = 0f;

            //shield
            _player.shieldPercent = 0;
            _shieldBar.fillAmount = 0;

            //boost
            _player.boostPercent = 0;
            _boostBar.fillAmount = 0;
            return;
        }

        // set fill to bar percent
        _healthBar.fillAmount = _player.healthPercent;
        _shieldBar.fillAmount = _player.shieldPercent;
        _boostBar.fillAmount = _player.boostPercent;
    }
}
