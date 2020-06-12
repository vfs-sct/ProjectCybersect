// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{   
    [SerializeField] private PlayerStats _player = null;

    [SerializeField] private float RespawnTime = 2f;

    private void Update()
    {
        RespawnPlayer();
    }

    private void RespawnPlayer()
    {
        //checks if the player is dead, respawn 2 seconds after
        if(_player.isDead)
        {
            RespawnTime -= Time.deltaTime;
            if(RespawnTime <= 0)
            {
                _player.transform.position = this.transform.position;
                _player.RespawnPlayer();
                RespawnTime = 2f;
            }
        }
    }
}
