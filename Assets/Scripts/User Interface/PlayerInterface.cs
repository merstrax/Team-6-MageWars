using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class PlayerInterface : MonoBehaviour
{
    [SerializeField] Image playerHealthBar;
    [SerializeField] Image PlayerCastBar;
    [SerializeField] Image PlayerCastBarFill;
    [SerializeField] Image NormalAttack;
    [SerializeField] Image NormalSpecial;
    [SerializeField] Image Special;
    [SerializeField] GameObject damagePanel;
    [SerializeField] TextMeshProUGUI interactMessage;



    // Start is called before the first frame update
    void Start()
    {
        PlayerCastBar.enabled = false;
    }

    private void Update()
    {

    }


    // Update is called once per frame
    public void UpdatePlayerHealth(float health, float maxHealth)
    {
        playerHealthBar.fillAmount = health / maxHealth;
    }

    public void UpdatePlayerCast(bool show, float Cast, float maxCast)
    {
        PlayerCastBar.enabled = show;
        PlayerCastBarFill.enabled = show;
        PlayerCastBarFill.fillAmount = Cast / maxCast;
    }

    public void UpdateInteractMessage(string message)
    {
        if (message == "")
        {
            interactMessage.enabled = false;
            return;
        }

        interactMessage.enabled = true;
        interactMessage.text = "Press [F] - To " + message;
    }
}
