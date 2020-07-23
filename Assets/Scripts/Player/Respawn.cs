// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    [SerializeField] private float _respawnTime = 2f;
    [SerializeField] private Transform _respawnPoint = null;

    private PlayerStats playerStats = null;
    private Rigidbody playerRB = null;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        playerRB = GetComponent<Rigidbody>();
    }

    IEnumerator ResPlayer()
    {
        yield return new WaitForSeconds(_respawnTime);
        playerStats.transform.position = _respawnPoint.position + (Vector3.up*2);
        playerRB.velocity = Vector3.zero;
        playerRB.angularVelocity = Vector3.zero;
        playerStats.RespawnPlayer();
    }

    public void RespawnPlayer()
    {
        StartCoroutine(ResPlayer());
    }
}
