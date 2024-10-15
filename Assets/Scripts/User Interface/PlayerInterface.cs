using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class PlayerInterface : MonoBehaviour
{
    [SerializeField] Image playerHealthBar;
    [SerializeField] Image PlayerCastBar;
    [SerializeField] Image NormalAttack;
    [SerializeField] Image NormalSpecial;
    [SerializeField] Image Special;
    [SerializeField] GameObject damagePanel;
    [SerializeField] TextMeshProUGUI interactMessage;



    // Start is called before the first frame update
    void Start()
    {
       
    }

    private void Update()
    {

    }


    // Update is called once per frame
    public void UpdatePlayerHealth(int health, int maxHealth)
    {
        playerHealthBar.fillAmount = (float)health / maxHealth;
    }
    public void UpdatePlayerCast(int Cast, int maxCast)
    {
        PlayerCastBar.fillAmount = (float)Cast / maxCast;
    }

    public void UpdateInteractMessage(string message)
    {
        if (message == "")
        {
            interactMessage.enabled = false;
            return;
        }

        interactMessage.enabled = true;
        interactMessage.text = message;
    }
}
