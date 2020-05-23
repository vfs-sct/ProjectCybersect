// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [Header("Volume Sliders")]
    [SerializeField] private Slider MasterSlider = null;
    [SerializeField] private Slider MusicSlider = null;
    [SerializeField] private Slider SFXSlider = null;

    [Header("Resolution")]
    [SerializeField] private Toggle WindowToggle = null;

    [Header("Gamma")]
    [SerializeField] private Slider GammaSlider = null;

    //Settings Data
    public float MasterVolume;
    public float MusicVolume;
    public float SFXVolume;

    public int Width;
    public int Height;
    public bool Fullscreen;

    public float GammaLevel;
    //Settings Data

    private void Awake()
    {
        string path = Application.persistentDataPath + "/settings.exq";
        if(File.Exists(path))
        {
            LoadData();
        }
        else
        {
            SaveData();
        }

        //Load Volume 
        MasterSlider.value = MasterVolume;
        MusicSlider.value = MusicVolume;
        SFXSlider.value = SFXVolume;

        //Load Resolution
        Screen.SetResolution(Width, Height, Fullscreen);
        WindowToggle.isOn = Fullscreen;

        //Load Gamma
        GammaSlider.value = GammaLevel;
    }

    public void Master(float volume)
    {
        MasterVolume = volume;
        SaveData();
    }

    public void Music(float volume)
    {
        MusicVolume = volume;
        SaveData();
    }

    public void SFX(float volume)
    {
        SFXVolume = volume;
        SaveData();
    }

    public void Resolution(bool fullscreen)
    {
        Fullscreen = fullscreen;
        Screen.SetResolution(Width, Height, Fullscreen);
        SaveData();
    }

    public void Gamma(float gamma)
    {
        GammaLevel = gamma;
        SaveData();
    }

    public void SaveData()
    {
        SaveSettings.SaveSetting(this);
    }

    public void LoadData()
    {
        SettingsData data = SaveSettings.LoadSetting();

        MasterVolume = data.MasterVolume;
        MusicVolume = data.MusicVolume;
        SFXVolume = data.SFXVolume;

        Width = data.Width;
        Height = data.Height;
        Fullscreen = data.Fullscreen;

        GammaLevel = data.GammaLevel;
    }
}
