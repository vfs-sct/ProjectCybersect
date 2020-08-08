// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    private EnemyStats target;

    private void Awake()
    {
        target = GetComponentInParent<EnemyStats>();
    }

    public void TakeDamage(float damage)
    {
        target.health -= damage;
        if(target.health <= 0 && target.isDead == false)
        {
            target.isDead = true;
            GameManager.Instance.EnemyKilled();
        }
    }
}
