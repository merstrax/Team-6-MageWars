using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("User Interface")]
    [SerializeField] Canvas mainInterface;
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuSettings;
    [SerializeField] PlayerInterface playerInterface;

    [SerializeField] PlayerController playerController;
    
    public GameObject checkPointPopUp;
    public GameObject player;

    float timeScaleOrig;

    private bool isPaused;
    public bool IsPaused()
    {
        return isPaused;
    }

    public Canvas GetMainCanvas() { return mainInterface; }

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;

        if (PlayerController.instance == null)
        {
            Instantiate(playerController);
        }

        player = GameObject.FindWithTag("Player");
    }


    // Update is called once per frame
    public void OnPause()
    {
        if (menuActive == null)
        {
            StatePause();
            menuActive = menuPause;
            menuActive.SetActive(isPaused);
        }
        else if (menuActive == menuSettings)
        {
            ToggleSettings();
        }
        else if (menuActive == menuPause)
        {
            StateUnpause();
        }
    }

    public void StatePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        InputController.instance.GetComponent<PlayerInput>().enabled = false;
    }

    public void StateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(isPaused);
        menuActive = null;
        InputController.instance.GetComponent<PlayerInput>().enabled = true;
    }

    public void Victory()
    {
        PlayerController.instance.audioPlayer.Play();
        StatePause();
        menuActive = menuWin;
        menuActive.SetActive(true);
    }

    public void youLose()
    {
        StatePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void ToggleSettings()
    {
        StatePause();
        menuActive.SetActive(false);
        if (menuActive == menuSettings)
        {
            menuActive = menuPause;
            menuActive.SetActive(true);
        }   
        else
        {
            menuActive = menuSettings;
            menuActive.SetActive(true);
        }
    }

    public void UpdateCastbar(bool Show, float Cast = 1f, float maxCast = 1f)
    {
        playerInterface.UpdatePlayerCast(Show, Cast, maxCast);
    }

    public void UpdateHealthbar(float healthCurrent, float healthMax)
    {
        playerInterface.UpdatePlayerHealth(healthCurrent, healthMax);
    }

    internal void SetInteractMessage(string message)
    {
        playerInterface.UpdateInteractMessage(message);
    }
}
