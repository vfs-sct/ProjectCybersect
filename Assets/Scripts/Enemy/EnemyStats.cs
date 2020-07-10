// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyStats : MonoBehaviour
{
    [Header("Pick Up")]
    [SerializeField] private GameObject _shieldPrefab = null;
    [SerializeField] private int _chance = 10;

    [Header("Shader")]
    [SerializeField] private Shader _shader = null;
    [SerializeField] private float _dissolveSpeed = 0f;

    private Material _dissolveMat = null;
    private float timer = 0f;
    private bool isDissolved = false;

    public bool isDead = false;
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
                if(Random.Range(1,_chance) == 1) Instantiate(_shieldPrefab,this.transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }
}
