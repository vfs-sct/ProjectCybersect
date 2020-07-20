// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Paused")]
    public bool isPaused = true;

    [Header("Num of Enemies")]
    public int enemyCountMap1 = 0;
    public int enemyCountMap2 = 0;
    public int enemyCountMap3 = 0;

    private int enemyCount = 0;

    private void Awake()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        int buildIndex = currentScene.buildIndex;

        if(Instance == null)
        {
            Instance = this;

            switch(buildIndex)
            {
                case 0:
                    break;
                case 1:
                    enemyCount = enemyCountMap1;
                    break;
                case 2:
                    enemyCount = enemyCountMap2;
                    break;
                case 3:
                    enemyCount = enemyCountMap3;
                    break;
            }

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        CheckPause();
    }

    public void EnemyKilled()
    {
        enemyCount--;
    }

    private void CheckPause()
    {
        if(isPaused)
        {
            //game is paused
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            //game is running
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
