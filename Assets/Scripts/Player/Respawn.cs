// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{   
    [SerializeField] private GameObject _player = null;

    private PlayerStats Player;

    [SerializeField] private float RespawnTime = 2f;

    private void Awake()
    {
        Player = _player.GetComponent<PlayerStats>();
    }

    private void Update()
    {
        RespawnPlayer();
    }

    private void RespawnPlayer()
    {
        if(Player.isDead)
        {
            RespawnTime -= Time.deltaTime;
            if(RespawnTime < 0)
            {
                RespawnTime = 2f;
                _player.transform.position = this.transform.position;
                Player.RespawnPlayer();
            }
        }
    }
}
