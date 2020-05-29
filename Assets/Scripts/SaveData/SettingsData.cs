// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettingsData
{
    public float MasterVolume = 1f;
    public float MusicVolume = 0.5f;
    public float SFXVolume = 0.5f;

    public int Width = 1920;
    public int Height = 1080;
    public bool Fullscreen = true;

    public float GammaLevel = 0f;

    public SettingsData(OptionsMenu settings)
    {
        MasterVolume = settings.MasterVolume;
        MusicVolume = settings.MusicVolume;
        SFXVolume = settings.SFXVolume;

        Width = settings.Width;
        Height = settings.Height;
        Fullscreen = settings.Fullscreen;

        GammaLevel = settings.GammaLevel;
    }
}
