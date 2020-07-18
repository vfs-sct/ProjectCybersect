// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{   
    [Header("UI")]
    [SerializeField] private GameObject _pauseUI = null;
    [SerializeField] private GameObject _debugUI = null;

    [Header("Menu")]
    [SerializeField] private GameObject _pauseMenu = null;
    [SerializeField] private GameObject _optionMenu = null;

    private void Awake()
    {
        _pauseUI.SetActive(false);
        _debugUI.SetActive(false);
    }

    private void Update()
    {   
        //pause menu
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

        //debug menu
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
        GameManager.Instance.isPaused = true;
        _pauseUI.SetActive(true);
    } 

    private void ContinueGame()
    {
        GameManager.Instance.isPaused = false;
        _pauseUI.SetActive(false);
    }

    private void OpenDebug()
    {
        GameManager.Instance.isPaused = true;
        _debugUI.SetActive(true);
    } 

    private void CloseDebug()
    {
        GameManager.Instance.isPaused = false;
        _debugUI.SetActive(false);
    }
}
