// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCounter : MonoBehaviour
{
    public int EnemyCount = 0;

    public void EnemyKilled()
    {
        EnemyCount--;
    }
}
