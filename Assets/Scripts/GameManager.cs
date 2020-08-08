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

    [Header("Build")]
    [SerializeField] private Scene currentScene;
    [SerializeField] private int buildIndex;
    [SerializeField] private int buildCount;

    [Header("Portal")]
    [SerializeField] private GameObject _portal = null;

    private GameObject portalPoint;

    public int sceneIndex;

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

        CheckBuild();
        buildCount = SceneManager.sceneCountInBuildSettings - 1;
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

    private void CheckBuild()
    {
        currentScene = SceneManager.GetActiveScene();
        buildIndex = currentScene.buildIndex;
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
            portalPoint = GameObject.Find("PortalPoint");
            Instantiate(_portal, portalPoint.transform.position, Quaternion.identity);
        }
    }

    public void NextScene()
    {
        isPaused = true;
        CheckBuild();
        sceneIndex = currentScene.buildIndex + 1;

        if(buildIndex < buildCount)
        {
            isPaused = false;
            SceneManager.LoadScene(sceneIndex, LoadSceneMode.Single);
        }
        else if(buildIndex >= buildCount)
        {
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }
    }
}
