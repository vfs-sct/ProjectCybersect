// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GenerateDebris : MonoBehaviour
{
    [Header("Debris Model")]
    [SerializeField] private GameObject _debris = null;

    [Header("# of Debris")]
    [SerializeField] private int _numOfDebris = 0;

    [Header("Range")]
    [SerializeField] private float _xRange = 0f;
    [SerializeField] private float _yRange = 0f;
    [SerializeField] private float _zRange = 0f;

    private void Awake()
    {
        for (int i = 0; i < _numOfDebris; i++)
        {
            SpawnDebris();
        }
    }

    private void SpawnDebris()
    {
        Vector3 randPos = new Vector3(Random.Range(-_xRange,_xRange), 
                                           Random.Range(-_yRange,_yRange), 
                                           Random.Range(-_zRange, _zRange)) + this.transform.position;
        GameObject debris = Instantiate(_debris, randPos, transform.rotation = Random.rotation);
    }
}
