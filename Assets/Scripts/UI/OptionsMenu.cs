// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [Header("Volume Sliders")]
    [SerializeField] private Slider _masterSlider = null;
    [SerializeField] private Slider _musicSlider = null;
    [SerializeField] private Slider _sfxSlider = null;

    [Header("Resolution")]
    [SerializeField] private Toggle _windowToggle = null;

    [Header("Gamma")]
    [SerializeField] private Slider _gammaSlider = null;


    [Header("Setting Data")]
    public float MasterVolume;
    public float MusicVolume;
    public float SFXVolume;

    public int Width;
    public int Height;
    public bool Fullscreen;

    public float GammaLevel;

    private void Start()
    {
        //Check for settings file
        string path = Application.persistentDataPath + "/settings.exq";
        if(File.Exists(path))
        {
            //load data
            LoadData();
        }
        else
        {
            //create new data
            SaveData();
        }

        //Load Volume 
        _masterSlider.value = MasterVolume;
        _musicSlider.value = MusicVolume;
        _sfxSlider.value = SFXVolume;

        //Load Resolution
        Screen.SetResolution(Width, Height, Fullscreen);
        _windowToggle.isOn = Fullscreen;

        //Load Gamma
        _gammaSlider.value = GammaLevel;
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
