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

    [Header("Ability Icons")]
    [SerializeField] AbilityIcon ability1;
    [SerializeField] AbilityIcon ability2;
    [SerializeField] AbilityIcon ability3;
    [SerializeField] AbilityIcon ability4;
    [SerializeField] TMP_Text FireObjective;
    [SerializeField] TMP_Text ArcaneObjective;
    [SerializeField] TMP_Text ForstObjective;
    [SerializeField] TMP_Text WindObjective;


    List<AbilityIcon> abilities;

    // Start is called before the first frame update
    void Start()
    {
        PlayerCastBar.enabled = false;
        interactMessage.enabled = false;

        abilities = new List<AbilityIcon>() { ability1, ability2, ability3, ability4};
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

    #region Abilities
    public void UpdateAbilityCooldown(int ability, bool enabled, float cooldownCurrent = 0, float cooldownMax = 0)
    {
        abilities[ability].SetCooldownOverlay(enabled);
        abilities[ability].UpdateCooldown(cooldownCurrent, cooldownMax);
    }

    public void UpdateAbilityIcon(int ability, Image icon)
    {
        abilities[ability].SetAbilityIcon(icon);
    }

    public void UpdateAbilityKeybind(int ability, Image icon)
    {
        abilities[ability].SetKeybindIcon(icon);
    }
    #endregion
}
