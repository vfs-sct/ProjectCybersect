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

    private Material dissolveMat = null;
    private float timer = 0f;
    private bool isDissolved = false;

    [Header("Stats")]
    public bool isDead = false;
    public float health = 100f;

    private void Awake()
    {
        GameManager.Instance.EnemiesCounted();

        /*foreach(Transform child in transform)
        {   
            //sets the shader
            if(child.GetComponent<Renderer>())
            {
                dissolveMat = child.GetComponent<Renderer>().material;
                dissolveMat.shader = _shader;
            }
        }*/
    }

    private void Update()
    {
        SetOnDeath();
        DropShieldOnDeath();
    }

    private void SetOnDeath()
    {
        if(isDead)
        {
            isDissolved = true;
            /*
            //animates the death shader
            timer += Time.deltaTime/_dissolveSpeed;
            
            foreach(Transform child in transform)
            {
                //removes any child "Bullet"  object in the enemy
                if(child.gameObject.tag == "Bullet")
                {
                    Destroy(child.gameObject);
                }
                else
                {
                    dissolveMat = child.GetComponent<Renderer>().material;

                    dissolveMat.SetFloat("Vector1_E189AF9C", timer);

                    if(dissolveMat.GetFloat("Vector1_E189AF9C") > 1)
                    {
                        isDissolved = true;
                    }
                }
            }*/

        }

    }

    private void DropShieldOnDeath()
    {
            if(isDissolved)
            {
                //get and random num and spawn a shield pickup if the number is 1
                if(Random.Range(1,_chance) == 1) Instantiate(_shieldPrefab,this.transform.position, Quaternion.identity);
                Destroy(this.gameObject);
            }
    }
}
