// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMenu : MonoBehaviour
{
    GameObject player;
    PlayerStats playerStats;

    private void Awake()
    {
        player = GameObject.Find("player");
        playerStats = player.GetComponent<PlayerStats>();
    }

    public void DamagePlayer()
    {
        playerStats.TakeDamage(10f);
    }

    public void HealPlayer()
    {
        playerStats.HealDamage(10f);
    }

    public void AddBoost()
    {
        playerStats.AddBoost();
    }

    public void RemoveShield()
    {
        playerStats.UseShield();
    }

    public void AddShield()
    {
        playerStats.AddShield();
    }
}
