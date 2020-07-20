// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CritTarget : MonoBehaviour
{
    private EnemyStats target;

    private void Awake()
    {
        target = GetComponentInParent<EnemyStats>();
    }

    public void TakeDamage(float damage)
    {
        target.health -= (damage * 2);
        if(target.health <= 0)
        {
            GameManager.Instance.EnemyKilled();
        }
    }
}
