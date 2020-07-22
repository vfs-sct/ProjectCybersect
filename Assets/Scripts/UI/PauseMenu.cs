﻿// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Pause Menu")]
    [SerializeField] private GameObject _pauseMenu = null;

    public void Continue()
    {
        GameManager.Instance.isPaused = false;
        _pauseMenu.SetActive(false);
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        GameManager.Instance.isPaused = true;
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
}
