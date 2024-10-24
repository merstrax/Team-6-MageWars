using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class Sound : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] Slider volumeMaster;
    [SerializeField] Slider volumeMusic;
    [SerializeField] Slider volumeEffects;

    [SerializeField] AudioMixer volumeMixer;

    void Start()
    {
        float _vol = PlayerPrefs.GetFloat("volumeMaster", 0.0f);
        volumeMaster.value = PlayerPrefs.GetFloat("volumeMaster", 0.75f);
        UpdateSlider(volumeMaster);

        _vol = PlayerPrefs.GetFloat("volumeEffects", 0.0f);
        volumeEffects.value = PlayerPrefs.GetFloat("volumeEffects", 0.75f);
        UpdateSlider(volumeEffects);

        _vol = PlayerPrefs.GetFloat("volumeBG", 0.0f);
        volumeMusic.value = PlayerPrefs.GetFloat("volumeBG", 0.75f);
        UpdateSlider(volumeMusic);

        LoadSettings();

        gameObject.SetActive(false);
    }

    void LoadSettings()
    {
        ChangeVolume();
    }

    public void Accept()
    {
        ChangeVolume();
        GameManager.instance.ReturnToPrev();
    }

    public void Back()
    {
        LoadSettings();
        GameManager.instance.ReturnToPrev();
    }

    public void ChangeVolume()
    {
        volumeMixer.SetFloat("volumeMaster", Mathf.Log10(volumeMaster.value) * 20);
        PlayerPrefs.SetFloat("volumeMaster", volumeMaster.value);

        volumeMixer.SetFloat("volumeEffects", Mathf.Log10(volumeEffects.value) * 20);
        PlayerPrefs.SetFloat("volumeEffects", volumeEffects.value);

        volumeMixer.SetFloat("volumeBG", Mathf.Log10(volumeMusic.value) * 20);
        PlayerPrefs.SetFloat("volumeBG", volumeMusic.value);
    }

    public void UpdateSlider(Slider slider)
    {
        slider.GetComponentInChildren<TMP_Text>().text = (slider.value * 100.0f).ToString("N0");
    }

    public void UpdateInput(Slider slider)
    {
        string _value = slider.GetComponentInChildren<TMP_Text>().text;
        if (_value.Length > 0)
            slider.value = float.Parse(_value);
    }
}
