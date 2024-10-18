using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSaves : MonoBehaviour
{
    public static GameSaves instance;

    public int PlayerClass;
    public int GodrickValleyFlags;

    // Start is called before the first frame update
    void Awake()
    {
        GodrickValleyFlags = PlayerPrefs.GetInt("GodrickValley", 0);
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
       SceneManager.LoadScene("Godrick Valley");
    }

    public void SaveFloatData(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
    }

    public void SaveIntData(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }

    public void SaveStringData(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }
}
