using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class buttonFuctions : MonoBehaviour
{
    public GameObject settingScreen;
    public GameObject graphicsScreen;
    public GameObject soundScreen;

    public void mainMenuSettings(GameObject menuSettings)
    {
        menuSettings.SetActive(!menuSettings.activeSelf);
    }

    public void Opensettings()
    {
        settingScreen.SetActive(true);
    }

    public void Graphicssettings()
    {
        graphicsScreen.SetActive(true);
        settingScreen.SetActive(false);
    }
    public void soundSettings()
    {
        soundScreen.SetActive(true);
        settingScreen.SetActive(false); 
    }

    public void Close()
    {
        settingScreen.SetActive(false);
        graphicsScreen.SetActive(false);
    }

    public void resume()
    {
        GameManager.instance.StateUnpause();
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.instance.StateUnpause();
    }

    public void Quit()
    {
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
    public void startgame()
    {
        SceneManager.LoadScene("Game Scene");
    }
}
