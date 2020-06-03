// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillY : MonoBehaviour
{
    [SerializeField] private PlayerStats _player = null;

    private void Update()
    {
        if(this.transform.position.y <= -10)
        {
            _player.BreakShield();
            _player.TakeDamage(999999);
        }
    }
}
