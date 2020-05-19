// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public OptionsSetting settings;

    public float MasterVolume = 1f;
    public float MusicVolume = 0.5f;
    public float SFXVolume = 0.5f;

    public void PlayGame()
    {

    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
