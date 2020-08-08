// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatBars : MonoBehaviour
{
    [Header("Image")]
    [SerializeField] private Image _healthBar = null;
    [SerializeField] private Image _shieldBar = null;
    [SerializeField] private Image _boostBar = null;
    [SerializeField] private Image _boostRechargeBar = null;

    [Header("Player")]
    [SerializeField] private PlayerStats playerStats = null;

    private void Update()
    {
        UpdateBar();
    }

    private void UpdateBar()
    {
        // if it can't find the player, set hp to 0
        if(playerStats == null)
        {
            //health
            playerStats.healthPercent = 0;
            _healthBar.fillAmount = 0f;

            //shield
            playerStats.shieldPercent = 0;
            _shieldBar.fillAmount = 0;

            //boost
            playerStats.boostPercent = 0;
            _boostBar.fillAmount = 0;

            //boost recharge
            playerStats.boostRechargePercent = 0;
            _boostRechargeBar.fillAmount = 0;
            return;
        }

        // set fill to bar percent
        _healthBar.fillAmount = playerStats.healthPercent;
        _shieldBar.fillAmount = playerStats.shieldPercent;
        _boostBar.fillAmount = playerStats.boostPercent;
        _boostRechargeBar.fillAmount = playerStats.boostRechargePercent;
    }
}
