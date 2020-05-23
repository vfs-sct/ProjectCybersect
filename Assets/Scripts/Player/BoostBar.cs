// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoostBar : MonoBehaviour
{
    private PlayerStats _player;
    private GameObject Player;
    private Image _boostBar;

    private void Start()
    {
        _boostBar = GetComponent<Image>();
        FindPlayer();
    }

    private void Update()
    {
        FindPlayer();
        UpdateBar();
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
            _player.boostPercent = 0;
            _boostBar.fillAmount = 0;
            return;
        }

        // set fill to health percent
        _boostBar.fillAmount = _player.boostPercent;
    }
}
