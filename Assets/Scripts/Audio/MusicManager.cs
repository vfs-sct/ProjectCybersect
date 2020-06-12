// Copyright (c) 2020 by Yuya Yoshino

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource _mainMenu = null;
    [SerializeField] private AudioSource _gameplay = null;

    private string musicTracker = "MusicTracker";
    private float addend = 0.01f;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject); 
    }

    private void Update()
    {
        //Handle main menu music
        if (GameObject.Find(musicTracker).tag == "Music_MainMenu")
        {
            //Play main menu music if not yet playing
            if (_mainMenu.isPlaying == false)
            {
                _mainMenu.Play();
            }

            //Fade in (and crossfade, if necessary)
            _mainMenu.volume += addend;

            if(_gameplay.isPlaying == true)
            {
                _gameplay.volume -= addend;
                
                if (_gameplay.volume == 0.0f)
                {
                    _gameplay.Stop();
                }
            }
        }
        //Handle gameplay music
        else if (GameObject.Find(musicTracker).tag == "Music_GamePlay")
        {
            //Play in game music if not playing
            if(_gameplay.isPlaying == false)
            {
                _gameplay.Play();
            }

            //Fade in (and crossfade, if necessary)
            _gameplay.volume += addend;

            if(_mainMenu.isPlaying == true)
            {
                _mainMenu.volume -= addend;

                if(_mainMenu.volume == 0.0f)
                {
                    _mainMenu.Stop();
                }
            }
        }
    }
}
