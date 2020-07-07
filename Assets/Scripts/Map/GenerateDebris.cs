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
    [SerializeField] private Vector3 _range;

    private void Awake()
    {
        for (int i = 0; i < _numOfDebris; i++)
        {
            SpawnDebris();
        }
    }

    private void SpawnDebris()
    {
        Vector3 randPos = new Vector3(Random.Range(-_range.x,_range.x), 
                                           Random.Range(-_range.y,_range.y), 
                                           Random.Range(-_range.z, _range.z)) + this.transform.position;
        GameObject debris = Instantiate(_debris, randPos, transform.rotation = Random.rotation);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, _range*2);
    }
}
