using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class buttonFuctions : MonoBehaviour
{
    public void mainMenuSettings(GameObject menuSettings)
    {
        menuSettings.SetActive(!menuSettings.activeSelf);
    }

    public void settings()
    {
        GameManager.instance.ToggleSettings();
    }

    public void resume()
    {
        GameManager.instance.stateUnpause();
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
