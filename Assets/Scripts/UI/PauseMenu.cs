// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{   
    public GameObject _pauseMenu;

    private void Awake()
    {
        _pauseMenu.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            if (!_pauseMenu.activeInHierarchy) 
            {
                Debug.Log("paused");
                PauseGame();
            }

            if (_pauseMenu.activeInHierarchy) 
            {
                Debug.Log("unpaused");
                ContinueGame();   
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
}
