// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoostBar : MonoBehaviour
{
    [SerializeField] private PlayerStats _player;

    private Image _boostBar;

    private void Start()
    {
        _boostBar = GetComponent<Image>();
    }

    private void Update()
    {
        // if it can't find the player, set hp to 0
        if(_player == null)
        {
            _boostBar.fillAmount = 0;
            return;
        }

        // set fill to health percent
        _boostBar.fillAmount = _player.boostPercent;
    }
}
