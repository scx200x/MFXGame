using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingltionCreator<AudioManager>
{
    private float volumeNum;
    private float soundNum;
    private AudioSource volumeSource;
    private AudioSource soundSource;
    
    public AudioManager()
    {
        volumeNum = soundNum = 0.5f;
        
        Record record = SaveSystem.LoadRecord();

        if (record != null)
        {
            VolumeNum = record.VolumePercent;
            SoundNum = record.SoundEffectPercent;
        }
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
    
    public void OnInit()
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

    public void PlayVolume()
    {
        if (VolumeSource)
        {
            VolumeSource.volume = volumeNum;
            VolumeSource.loop = true;
            VolumeSource.Play();
        }
    }

    public void SetVolumeSourceRes(string resPath)
    {
        ResourceInfo resourceInfo = ResourcesManager.GetInstance().LoadAsset(resPath);

        if (resourceInfo.LoadObj != null)
        {
            VolumeSource.clip = (AudioClip)resourceInfo.LoadObj;
        }
    }
    
    public void SetSoundSourceRes(string resPath)
    {
        ResourceInfo resourceInfo = ResourcesManager.GetInstance().LoadAsset(resPath);

        if (resourceInfo.LoadObj != null)
        {
            SoundSource.clip = (AudioClip)resourceInfo.LoadObj;
        }
    }
    
    public static AudioManager GetInstance()
    {
        return Instance;
    }
}