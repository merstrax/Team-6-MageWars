using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityIcon : MonoBehaviour
{
    [SerializeField] Image abilityIcon;
    [SerializeField] Image abilityKeybind;
    [SerializeField] Image cooldownOverlay;
    [SerializeField] TextMeshProUGUI cooldownText;

    private void Start()
    {
        cooldownOverlay.enabled = false;
        cooldownText.enabled = false;
    }

    public void UpdateCooldown(float cooldown, float cooldownMax)
    {
        cooldownText.text = cooldown.ToString("0.0");
        cooldownOverlay.fillAmount = cooldown / cooldownMax;
    }

    public void SetAbilityIcon(Image newIcon)
    {
        abilityIcon = newIcon;
    }

    public void SetKeybindIcon(Image newKeybind)
    {
        abilityKeybind = newKeybind;
    }

    public void SetCooldownOverlay(bool enabled = true)
    {
        cooldownOverlay.enabled = enabled;
        cooldownText.enabled = enabled;
    }
}
