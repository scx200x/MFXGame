using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : ManagerBase
{
    private float volumeNum;
    private float soundNum;
    private AudioSource volumeSource;
    private AudioSource soundSource;

    private static AudioManager Instance;

    public AudioManager()
    {
        volumeNum = soundNum = 0.5f;
    }

    public float VolumeNum
    {
        set
        {
            volumeNum = value;

            if (volumeSource != null)
            {
                volumeSource.volume = volumeNum;
            }
        }
        get { return volumeNum; }
    }
    
    public float SoundNum
    {
        set { soundNum = value; }
        get { return soundNum; }
    }

    public AudioSource VolumeSource
    {
        set { volumeSource = value; }
        get { return volumeSource; }
    }
    
    public AudioSource SoundSource
    {
        set { soundSource = value; }
        get { return soundSource; }
    }
    
    public override void OnInit()
    {
        VolumeSource = null;
        SoundSource = null;
    }

    public void SetAolumeSource(AudioSource source)
    {
        VolumeSource = source;
    }

    public void SetSoundSource(AudioSource source)
    {
        SoundSource = source;
    }

    public void PlaySound()
    {
        if (SoundSource)
        {
            soundSource.volume = soundNum;
            soundSource.Play();
        }
    }
    
    public static AudioManager GetInstance()
    {
        if (Instance == null)
        {
            Instance = new AudioManager();
            Record record = SaveSystem.LoadRecord();

            if (record != null)
            {
                Instance.VolumeNum = record.VolumePercent;
                Instance.SoundNum = record.SoundEffectPercent;
            }
        }

        return Instance;
    }
}