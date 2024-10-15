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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMasterVol()
    {
        mastLabel.text = Mathf.RoundToInt(musicSlider.value + 80).ToString();

        audioMixer.SetFloat("MasterVol", mastSlider.value);
    }
}
