// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private GameObject _shieldPrefab = null;
    [SerializeField] private int _randomRange = 10;
    [SerializeField] private Material _dissolveMat = null;
    [SerializeField] private float _dissolveSpeed = 0f;
    [SerializeField] private Shader _shader = null;

    private float timer = 0f;

    public bool isDead = false;
    public bool isDissolved = false;
    public float health = 100f;

    private void Awake()
    {
        if(GetComponent<Renderer>())
        {
            _dissolveMat = GetComponent<Renderer>().material;
            _dissolveMat.shader = _shader;
        }
    }

    private void Update()
    {
        CheckDeath();
        PlayDeath();
    }

    private void PlayDeath()
    {
        if(isDead)
        {
            //animates the death shader
            timer += Time.deltaTime/_dissolveSpeed;
            _dissolveMat.SetFloat("Vector1_E189AF9C", timer);

            if(_dissolveMat.GetFloat("Vector1_E189AF9C") > 1)
            {
                isDissolved = true;
            }
        }
    }

    private void CheckDeath()
    {
        if(health <= 0f)
        {
            isDead = true;

            if(isDissolved)
            {
                //get and random num and spawn a shield pickup if the number is 1
                if(Random.Range(1,_randomRange) == 1) Instantiate(_shieldPrefab,this.transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }
}
