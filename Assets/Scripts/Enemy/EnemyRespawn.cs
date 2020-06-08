// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRespawn : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] private GameObject _target = null;
    [SerializeField] private float _respawnTime = 3f;

    private void Awake()
    {
        GameObject Enemy;
        Enemy = Instantiate(_target,transform.position,Quaternion.identity) as GameObject;
        Enemy.transform.parent = transform;
    }

    private void Update()
    {
        if(transform.childCount < 1)
        {
            _respawnTime -= Time.deltaTime;

            if(_respawnTime <= 0f)
            {
                GameObject Enemy;
                Enemy = Instantiate(_target,transform.position,Quaternion.identity) as GameObject;
                Enemy.transform.parent = transform;
                _respawnTime = 3f;
            }
        }
    }
}
