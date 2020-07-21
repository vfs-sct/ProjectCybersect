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

    public int enemyCount = 0;

    public Scene currentScene;
    public int buildIndex;

    private void Awake()
    {
        currentScene = SceneManager.GetActiveScene();
        buildIndex = currentScene.buildIndex;

        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
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
    }

    private void Update()
    {
        CheckPause();
        CheckEndGame();
    }

    public void EnemyKilled()
    {
        enemyCount--;
    }

    private void CheckPause()
    {
        if(buildIndex == 0) return;

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

    private void CheckEndGame()
    {
        if(buildIndex == 0) return;

        if(enemyCount == 0)
        {
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }
    }
}
