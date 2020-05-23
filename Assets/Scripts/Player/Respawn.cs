// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{   
    [SerializeField] private GameObject _playerPrefab = null;

    private float RespawnTime = 2f;
    private bool CanRespawn = false;

    GameObject player;

    private void Update()
    {
        CheckForPlayer();
        RespawnPlayer();
    }

    private void CheckForPlayer()
    {
        if(GameObject.Find("player"))
        {
            CanRespawn = false;
        }
        else
        {
            CanRespawn = true;
        }
    }

    private void RespawnPlayer()
    {
        if(CanRespawn)
        {
            RespawnTime -= Time.deltaTime;
            if(RespawnTime < 0)
            {
                RespawnTime = 2f;
                CanRespawn = false;
                player = Instantiate(_playerPrefab, transform.position, Quaternion.identity);
                player.name = "player";
            }
        }
    }
}
