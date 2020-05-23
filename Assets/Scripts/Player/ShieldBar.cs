// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldBar : MonoBehaviour
{
    private PlayerStats _player;
    private GameObject Player;
    private Image _shieldBar;

    private void Start()
    {
        _shieldBar = GetComponent<Image>();
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
        if(Player == null)
        {   
            _player.shieldPercent = 0;
            _shieldBar.fillAmount = 0;
            return;
        }

        // set fill to health percent
        _shieldBar.fillAmount = _player.shieldPercent;
    }
}
