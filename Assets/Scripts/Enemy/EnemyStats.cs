// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private int _randomRange = 10;
    [SerializeField] private GameObject _shieldPrefab = null;

    public float health = 100f;

    private void Update()
    {
        CheckDeath();
    }

    private void CheckDeath()
    {
        if(health <= 0f)
        {
            //get and random num and spawn a shield pickup if the number is 1
            if(GetRandom() == 1) Instantiate(_shieldPrefab,this.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private int GetRandom()
    {
        int selectedNum = Random.Range(1,_randomRange);
        return selectedNum;
    }
}
