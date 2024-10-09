using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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

    public GameObject checkPointPopUp;
    public GameObject player;

    public PlayerController playerScript;
    private PlayerInput playerInput;
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
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        menuSettings.SetActive(true);

    }

    // Update is called once per frame
    public void OnPause(InputAction.CallbackContext context)
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
    }

    public void StateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(isPaused);
        menuActive = null;
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

}
