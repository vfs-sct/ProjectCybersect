// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public float health = 100f;

    private void Update()
    {
        if(health <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
