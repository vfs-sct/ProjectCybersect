// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
        GameManager.Instance.isPaused = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
