// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    [Header("Volume Sliders")]
    [SerializeField] private Slider _masterSlider = null;
    [SerializeField] private Slider _musicSlider = null;
    [SerializeField] private Slider _sfxSlider = null;

    [Header("Resolution")]
    [SerializeField] private TMP_Dropdown _resolutions = null;
    [SerializeField] private Toggle _fullscreenToggle = null;

    //[Header("Gamma")]
    //[SerializeField] private Slider _gammaSlider = null;

    [Header("Audio")]
    [SerializeField] private AudioMixer _audioMixer = null;

    private bool Fullscreen;

    Resolution[] resolutions;

    private void Start()
    {
        AddResolutions();

        if(!PlayerPrefs.HasKey("MasterVolume"))
        {
            Defaults();
            return;
        }

        LoadVolumes();
        LoadResolution();
        LoadOther();
    }

    private void AddResolutions()
    {
        resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();
        _resolutions.ClearOptions();

        int currentResolutionIndex = 0;
        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if(resolutions[i].width == Screen.currentResolution.width &&
               resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        _resolutions.AddOptions(options);
        _resolutions.value = currentResolutionIndex;
        _resolutions.RefreshShownValue();
    }

    private void LoadVolumes()
    {
        _masterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        _audioMixer.SetFloat("masterVolume", _masterSlider.value);
        _musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        _audioMixer.SetFloat("musicVolume", _musicSlider.value);
        _sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        _audioMixer.SetFloat("sfxVolume", _sfxSlider.value);
    }

    private void LoadResolution()
    {
        Fullscreen = (PlayerPrefs.GetInt("Fullscreen") == 1) ? true : false;
        _fullscreenToggle.isOn = Fullscreen;
        Screen.fullScreen = Fullscreen;
        Screen.SetResolution(PlayerPrefs.GetInt("Width"), 
                            PlayerPrefs.GetInt("Height"),
                            Fullscreen);
    }

    private void LoadOther()
    {
        //_gammaSlider.value = PlayerPrefs.GetFloat("Gamma");
    }

    public void Master(float volume)
    {
        PlayerPrefs.SetFloat("MasterVolume", volume);
        _audioMixer.SetFloat("masterVolume", volume);
        PlayerPrefs.Save();
    }

    public void Music(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        _audioMixer.SetFloat("musicVolume", volume);
        PlayerPrefs.Save();
    }

    public void SFX(float volume)
    {
        PlayerPrefs.SetFloat("SFXVolume", volume);
        _audioMixer.SetFloat("sfxVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        PlayerPrefs.SetInt("Width", resolution.width);
        PlayerPrefs.SetInt("Height", resolution.height);
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.Save();
    }

    public void ChangeWindow(bool fullscreen)
    {
        if(fullscreen) 
        {
            PlayerPrefs.SetInt("Fullscreen", 1);
        }
        else 
        {
            PlayerPrefs.SetInt("Fullscreen", 0);
        }

        Fullscreen = fullscreen;
        Screen.fullScreen = Fullscreen;
        PlayerPrefs.Save();
    }

    public void Gamma(float gamma)
    {
        PlayerPrefs.SetFloat("Gamma", gamma);
        PlayerPrefs.Save();
    }

    private void Defaults()
    {
        _masterSlider.value = 0f;
        _musicSlider.value = 0f;
        _sfxSlider.value = -40f;
        _audioMixer.SetFloat("masterVolume", _masterSlider.value);
        _audioMixer.SetFloat("musicVolume", _musicSlider.value);
        _audioMixer.SetFloat("musicVolume", _sfxSlider.value);
        Screen.SetResolution(1920, 1080, true);
        //_gammaSlider.value = 0f;
    }
}
