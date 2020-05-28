// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMenu : MonoBehaviour
{
    [SerializeField] private PlayerStats _player = null;

    public void DamagePlayer()
    {
        _player.TakeDamage(10f);
    }

    public void HealPlayer()
    {
        _player.HealDamage(10f);
    }

    public void AddBoost()
    {
        _player.AddBoost();
    }

    public void RemoveShield()
    {
        _player.UseShield();
    }

    public void AddShield()
    {
        _player.AddShield();
    }
}
