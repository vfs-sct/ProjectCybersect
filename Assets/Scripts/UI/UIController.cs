// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{   
    [SerializeField] private GameObject _pauseUI = null;
    [SerializeField] private GameObject _pauseMenu = null;
    [SerializeField] private GameObject _optionMenu = null;
    [SerializeField] private GameObject _debugUI = null;

    private void Awake()
    {
        _pauseUI.SetActive(false);
        _debugUI.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            CloseDebug();

            if (_pauseUI.activeInHierarchy) 
            {
                _optionMenu.SetActive(false);
                _pauseMenu.SetActive(true);
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

            if (_debugUI.activeInHierarchy) 
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

        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _pauseUI.SetActive(true);
    } 

    private void ContinueGame()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _pauseUI.SetActive(false);
    }

    private void OpenDebug()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _debugUI.SetActive(true);
    } 

    private void CloseDebug()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _debugUI.SetActive(false);
    }
}
