using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class Config : MonoBehaviour
{
    public Text VolumePercent;
    public Text SoundEffectPercent;
    public Slider VolumeSlider;
    public Slider SoundEffectSlider;
    public Dropdown ResultionDropdown;

    private float vp, sp;
    private int idx;

    void Start()
    {

    }
    
    public void UpdateResolution(int index)
    {
        AudioManager.GetInstance().PlaySound();
        Utility.SetResolution(index);
        idx = index;
        SaveRecord();
    }

    public void UpdatePercent()
    {
        vp = VolumeSlider.value;
        VolumePercent.text = Math.Floor(VolumeSlider.value * 100).ToString();
        SaveRecord();
        AudioManager.GetInstance().VolumeNum = vp;
    }

    public void UpdateSoundPercent()
    {
        sp = SoundEffectSlider.value;
        SoundEffectPercent.text = Math.Floor(SoundEffectSlider.value * 100).ToString();
        SaveRecord();
        AudioManager.GetInstance().SoundNum = sp;
    }

    public void Close()
    {
        AudioManager.GetInstance().PlaySound();
        this.gameObject.SetActive(false);
    }

    public void UpdateUI()
    {
        Record record = SaveSystem.LoadRecord();

        if (record == null)
        {
            VolumeSlider.value = 0.5f;
            SoundEffectSlider.value = 0.5f;
            vp = sp = 0.5f;
            idx = 0;
            
            SaveRecord();
        }
        else
        {
            VolumeSlider.value = record.VolumePercent;
            SoundEffectSlider.value = record.SoundEffectPercent;
            ResultionDropdown.value = record.ResulationIndex;
            UpdatePercent();
            UpdateSoundPercent();
        }
    }

    private void SaveRecord()
    {
        Record record = new Record();
        record.ResulationIndex = idx;
        record.VolumePercent = vp;
        record.SoundEffectPercent = sp;
        
        SaveSystem.SaveRecord(record);
    }
}