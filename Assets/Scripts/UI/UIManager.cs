// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{   
    public GameObject _pauseMenu;
    public GameObject _debugMenu;

    private void Awake()
    {
        _pauseMenu.SetActive(false);
        _debugMenu.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            CloseDebug();

            if (_pauseMenu.activeInHierarchy) 
            {
                ContinueGame();   
            }
            else
            {
                PauseGame();
            }
        } 

        if(Input.GetKeyDown(KeyCode.BackQuote))
        {
            ContinueGame();

            if (_debugMenu.activeInHierarchy) 
            {
                CloseDebug();
            }
            else
            {
                OpenDebug();
            }
        }
    }
    
    private void PauseGame()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _pauseMenu.SetActive(true);
        Time.timeScale = 0;
    } 

    private void ContinueGame()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _pauseMenu.SetActive(false);
    }

    private void OpenDebug()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _debugMenu.SetActive(true);
        Time.timeScale = 0;
    } 

    private void CloseDebug()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _debugMenu.SetActive(false);
    }
}
