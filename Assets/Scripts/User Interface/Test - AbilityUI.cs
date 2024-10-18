using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Test_AbilityUI : MonoBehaviour
{
    [SerializeField] TMP_Text playerName;
    [SerializeField] TMP_Text playerHealth;
    [SerializeField] TMP_Text playerAbility1;
    [SerializeField] TMP_Text playerAbility2;
    [SerializeField] TMP_Text playerAbility3;
    [SerializeField] TMP_Text playerAbility4;
    [SerializeField] TMP_Text playerDamage;
    [SerializeField] TMP_Text playerDefense;
    [SerializeField] TMP_Text playerCritChance;
    [SerializeField] TMP_Text playerCritDamage;
    [SerializeField] TMP_Text playerEffects;

    PlayerController player;

    bool updateReady = true;
    float updateTimer = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerController.instance;
        playerName.text += " " + player.GetUnitName();
    }

    // Update is called once per frame
    void Update()
    {
        if (updateReady)
            StartCoroutine(UpdateUI());
    }

    IEnumerator UpdateUI()
    {
        updateReady = false;

        playerHealth.text = "Health: " + player.GetHealthCurrent() + " / " + player.GetHealthMax();

        playerAbility1.text = player.GetAbility(0) != null ? player.GetAbility(0).ToString() + " : " + player.GetAbility(0).GetAbility().GetAbilityInfo().GetDescription(player) : "null";
        playerAbility2.text = player.GetAbility(1) != null ? player.GetAbility(1).ToString() + " : " + player.GetAbility(1).GetAbility().GetAbilityInfo().GetDescription(player) : "null";
        playerAbility3.text = player.GetAbility(2) != null ? player.GetAbility(2).ToString() + " : " + player.GetAbility(2).GetAbility().GetAbilityInfo().GetDescription(player) : "null";
        playerAbility4.text = player.GetAbility(3) != null ? player.GetAbility(3).ToString() + " : " + player.GetAbility(3).GetAbility().GetAbilityInfo().GetDescription(player) : "null";

        playerDamage.text = "Damage: " + player.GetDamageBonus() + " (" + (player.GetDamageModifier() * 100) + "%)";
        playerDefense.text = "Defense: " + player.GetDefenseBonus() + " (" + (player.GetDefenseModifier() * 100) + "%)";
        playerCritChance.text = "Critical Chance:" + (player.GetCritChanceBonus() * 100) + "%";
        playerCritDamage.text = "Critical Damage:" + (player.GetCritDamageBonus() * 100) + "%";

        playerEffects.text = "";

        foreach (var effect in player.GetEffects())
            playerEffects.text += effect.ToString() + "\n";

        yield return new WaitForSeconds(updateTimer * Time.deltaTime);

        updateReady = true;
    }
}
