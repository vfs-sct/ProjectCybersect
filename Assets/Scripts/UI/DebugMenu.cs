// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMenu : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;

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
        playerStats.Boost(1);
    }

    public void RemoveShield()
    {
        playerStats.Shield(-1);
    }

    public void AddShield()
    {
        playerStats.Shield(1);
    }
}
