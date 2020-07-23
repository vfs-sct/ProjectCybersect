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

    [Header("Enemies")]
    public int enemyCount = 0;

    public Scene currentScene;
    public int buildIndex;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        currentScene = SceneManager.GetActiveScene();
        buildIndex = currentScene.buildIndex;
    }

    private void Update()
    {
        CheckPause();
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

    public void EnemiesCounted()
    {
        enemyCount++;
    }

    public void EnemyKilled()
    {
        enemyCount--;
        if(enemyCount == 0)
        {
            isPaused = true;
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }
    }
}
