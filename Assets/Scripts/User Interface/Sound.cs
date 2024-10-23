using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class Sound : MonoBehaviour
{
    public AudioMixer audioMixer;

    public TMP_Text mastLabel, musicLabel, sfxLabel;

    public Slider mastSlider, musicSlider, sfxSlider;

    // Start is called before the first frame update
    void Start()
    {
        LoadSettings();
        Apply();
    }

    public void SetMasterVol()
    {
        float _val = 100 * ((mastSlider.value + 80) / 80);

        mastLabel.text = _val.ToString("0");
    }
    public void SetMusicVol()
    {
        float _val = 100 * ((musicSlider.value + 80) / 80);

        musicLabel.text = _val.ToString("0");
    }
    public void SetSFXVol()
    {
        float _val = 100 * ((sfxSlider.value + 80) / 80);

        sfxLabel.text = _val.ToString("0");
    }

    public void LoadSettings()
    {
        mastSlider.value = PlayerPrefs.GetFloat("MasterVolumePreference", 1.0f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolumePreference", 1.0f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
    }
    public void Accept()
    {
        Apply();
        GameManager.instance.ReturnToPrev();
    }
    public void Back()
    {
        LoadSettings();
        GameManager.instance.ReturnToPrev();
    }
    private void Apply()
    {
        float _val = (80 * (mastSlider.value / 100)) - 80;
        audioMixer.SetFloat("MasterVol", _val);
        PlayerPrefs.SetFloat("MasterVolumePreference", _val);

        _val = (80 * (musicSlider.value / 100)) - 80;
        audioMixer.SetFloat("MusicVol", _val);
        PlayerPrefs.SetFloat("MusicVolumePreference", _val);

        _val = (80 * (sfxSlider.value / 100)) - 80;
        audioMixer.SetFloat("SFXVol", _val);
        PlayerPrefs.SetFloat("SFXVolumePreference", _val);

    }
}
