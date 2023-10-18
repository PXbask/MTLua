using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    AudioSource musicAudio;
    AudioSource soundAudio;
    private float MusicVolume
    {
        get { return PlayerPrefs.GetFloat("MusicVolume", 1); }
        set
        {
            musicAudio.volume = value;
            PlayerPrefs.SetFloat("MusicVolume", value);
        }
    }
    private float SoundVolume
    {
        get { return PlayerPrefs.GetFloat("SoundVolume", 1); }
        set
        {
            soundAudio.volume= value;
            PlayerPrefs.SetFloat("SoundVolume", value);
        }
    }
    private void Awake()
    {
        musicAudio = gameObject.AddComponent<AudioSource>();
        musicAudio.playOnAwake = false;
        musicAudio.loop = true;
        soundAudio = gameObject.AddComponent<AudioSource>();
        soundAudio.playOnAwake = false;
        soundAudio.loop = false;
    }
    /// <summary>
    /// ≤•∑≈“Ù¿÷
    /// </summary>
    /// <param name="name"></param>
    public void PlayMusic(string name)
    {
        if (MusicVolume < 0.1) return;
        string oldName = string.Empty;
        if(musicAudio.clip!= null)
        {
            oldName= musicAudio.clip.name;
        }
        if (oldName.Equals(name))
        {
            musicAudio.Play();
            return;
        }
        Manager.Resources.LoadMusic(name, (UnityEngine.Object obj) =>
        {
            musicAudio.clip = obj as AudioClip;
            musicAudio.Play();
        });
    }
    public void PauseMusic()
    {
        musicAudio.Pause();
    }
    public void UnPauseMusic()
    {
        musicAudio.UnPause();
    }
    public void StopMusic()
    {
        musicAudio.Stop();
    }
    public void PlaySound(string name)
    {
        if (SoundVolume < 0.1) return;
        Manager.Resources.LoadSound(name, (UnityEngine.Object obj) =>
        {
            soundAudio.PlayOneShot(obj as AudioClip);
        });
    }
    public void SetMusicVolume(float volume)
    {
        this.MusicVolume = volume;
    }
    public void SetSoundVolume(float volume)
    {
        this.SoundVolume = volume;
    }
}
