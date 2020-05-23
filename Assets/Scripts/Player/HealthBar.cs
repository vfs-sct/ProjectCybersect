// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private PlayerStats _player;
    private GameObject Player;
    private Image _healthBar;

    private void Start()
    {
        _healthBar = GetComponent<Image>();
        FindPlayer();
    }

    private void Update()
    {
        UpdateBar();
        FindPlayer();
    }

    private void FindPlayer()
    {
        Player = GameObject.Find("player");
        _player = Player.GetComponent<PlayerStats>();
    }

    private void UpdateBar()
    {
        // if it can't find the player, set hp to 0
        if(_player == null)
        {
            _player.healthPercent = 0;
            _healthBar.fillAmount = 0f;
            return;
        }

        // set fill to health percent
        _healthBar.fillAmount = _player.healthPercent;
    }
}
