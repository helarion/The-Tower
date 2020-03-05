﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicHandler : MonoBehaviour
{
    [SerializeField] AudioClip[] listMusic;
    [SerializeField] AudioSource musicSource;
    float SFXVolume = 1;

    bool changedMusic = false;

    private static MusicHandler instance = null;

    // Game Instance Singleton
    public static MusicHandler Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        if(!musicSource.isPlaying && !changedMusic)
        {
            changedMusic = true;
            ChooseRandomMusic();
        }
    }

    private void ChooseRandomMusic()
    {
        int rand = Random.Range(0, listMusic.Length);
        musicSource.clip = listMusic[rand];
        musicSource.Play();
        changedMusic = false;
    }

    public void UpdateMusicVolume(float value)
    {
        musicSource.volume = value;
    }

    public void UpdateSFXVolume(float value)
    {
        SFXVolume = value;
        UpdateVolumeSources();        
    }

    public void UpdateVolumeSources()
    {
        AudioSource[] sources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource s in sources)
        {
            if (!s.tag.Equals("Music"))
            {
                s.volume = SFXVolume;
            }
        }
    }
}