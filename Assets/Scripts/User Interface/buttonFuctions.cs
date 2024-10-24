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
        GameManager.instance.ToggleSettingsMenu();
    }

    public void Graphicssettings()
    {
        GameManager.instance.ToggleGraphicsMenu();

    }
    public void soundSettings()
    {
        GameManager.instance.ToggleSoundMenu();
    }

    public void CreditsPanel()
    {
        GameManager.instance.ToddleCredits();
    }

    public void Close()
    {
       GameManager.instance.ReturnToPrev();
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
        GameManager.instance.ShowLoadingScreen();
        SceneManager.LoadScene("LoadGameScene");
    }
}
