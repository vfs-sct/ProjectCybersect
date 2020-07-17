// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillY : MonoBehaviour
{
    [SerializeField] private float _killY = 0f;
    
    private PlayerStats _player = null;

    private void Awake()
    {
        _player = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        if(_player.isDead) return;

        //kill if below _killY
        if(this.transform.position.y <= _killY)
        {
            _player.BreakShield();
            _player.TakeDamage(999999);
        }
    }
}
