// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldBar : MonoBehaviour
{
    [SerializeField] private PlayerStats _player;

    private Image _shieldBar;

    private void Start()
    {
        _shieldBar = GetComponent<Image>();
    }

    private void Update()
    {
        // if it can't find the player, set hp to 0
        if(_player == null)
        {
            _shieldBar.fillAmount = 0;
            return;
        }

        // set fill to health percent
        _shieldBar.fillAmount = _player.shieldPercent;
    }
}
