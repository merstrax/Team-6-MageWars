using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Graphics : MonoBehaviour
{
    public Toggle fullscreenTog, vsyncTog;

    public List<ResItem> resolutions = new();
    private int selectedRes;
    public TMP_Text resolutionLabel;

    [SerializeField] TMP_Dropdown qualityDropdown;
    [SerializeField] TMP_Dropdown textureDropdown;
    [SerializeField] TMP_Dropdown aaDropdown;

    // Start is called before the first frame update
    void Start()
    {
        LoadSettings();
        ApplyVideoSettings();
    }

    public void SetResolution(int resolutionIndex)
    {
        ResItem resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.horizontal, resolution.vertical, Screen.fullScreen);
    }

    public void SetTextureQuality(int textureIndex)
    {
        QualitySettings.globalTextureMipmapLimit = textureIndex;
        return;
    }

    public void SetAntiAliasing(int aaIndex)
    {
        QualitySettings.antiAliasing = aaIndex;
        return;
    }

    public void SetQuality(int qualityIndex)
    {

        if (qualityIndex != 4)
        {
            QualitySettings.SetQualityLevel(qualityDropdown.value);
        }

        switch (qualityIndex)
        {
            case 0:
                textureDropdown.value = 2;
                aaDropdown.value = 0;
                break;
            case 1:
                textureDropdown.value = 1;
                aaDropdown.value = 0;
                break;
            case 2:
                textureDropdown.value = 0;
                aaDropdown.value = 2;
                break;
        }
        return;
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("QualitySettingPreference",
                   qualityDropdown.value);
        PlayerPrefs.SetInt("ResolutionPreference",
                   selectedRes);
        PlayerPrefs.SetInt("TextureQualityPreference",
                   textureDropdown.value);
        PlayerPrefs.SetInt("AntiAliasingPreference",
                   aaDropdown.value);
        PlayerPrefs.SetInt("FullscreenPreference",
                   Convert.ToInt32(Screen.fullScreen));
    }

    public void LoadSettings()
    {
        qualityDropdown.value = PlayerPrefs.GetInt("QualitySettingPreference", 3);
        selectedRes = PlayerPrefs.GetInt("ResolutionPreference", 0);
        textureDropdown.value = PlayerPrefs.GetInt("TextureQualityPreference", 0);
        aaDropdown.value = PlayerPrefs.GetInt("AntiAliasingPreference", 3);
        Screen.fullScreen = Convert.ToBoolean(PlayerPrefs.GetInt("FullscreenPreference", 1));

        UpdateResLabel();
    }

    public void ResLeft()
    {
        selectedRes--;
        if (selectedRes < 0)
        {
            selectedRes = resolutions.Count - 1;
        }
        UpdateResLabel();
    }

    public void ResRight()
    {
        selectedRes++;
        if (selectedRes > resolutions.Count - 1)
        {
            selectedRes = 0;
        }
        UpdateResLabel();
    }

    public void UpdateResLabel()
    {
        resolutionLabel.text = resolutions[selectedRes].horizontal.ToString() + " x " + resolutions[selectedRes].vertical.ToString();
    }

    public void ApplyVideoSettings()
    {
        Screen.fullScreen = fullscreenTog.isOn;
        QualitySettings.vSyncCount = Convert.ToInt32(vsyncTog.isOn);

        ResItem resolution = resolutions[selectedRes];
        Screen.SetResolution(resolution.horizontal, resolution.vertical, Screen.fullScreen);
    }

    public void Apply()
    {
        ApplyVideoSettings();
        SaveSettings();
        GameManager.instance.ReturnToPrev();
    }

    public void Back()
    {
        LoadSettings();
        GameManager.instance.ReturnToPrev();
    }
}

[System.Serializable]
public class ResItem
{
    public int horizontal, vertical;
}
