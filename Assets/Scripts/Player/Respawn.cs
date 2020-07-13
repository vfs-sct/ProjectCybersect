// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{   
    [SerializeField] private float RespawnTime = 2f;

    private GameObject player = null;
    private PlayerStats playerStats = null;
    private Rigidbody playerRB = null;
    private bool respawning = false;

    private void Awake()
    {
        player = GameObject.Find("player");
        playerStats = player.GetComponent<PlayerStats>();
        playerRB = player.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        RespawnPlayer();
    }

    IEnumerator ResPlayer()
    {
        yield return new WaitForSeconds(RespawnTime);
        playerRB.velocity = Vector3.zero;
        playerRB.angularVelocity = Vector3.zero;
        playerStats.transform.position = this.transform.position + Vector3.up;
        playerStats.RespawnPlayer();
    }

    private void RespawnPlayer()
    {
        if(playerStats.isDead && !respawning)
        {
            respawning = true;
            StartCoroutine("ResPlayer");
        }
    }
}
