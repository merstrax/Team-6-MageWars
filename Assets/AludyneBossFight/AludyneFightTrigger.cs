using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AludyneFightTrigger : MonoBehaviour
{
    [SerializeField] AludyneBossFight fightObject;
    [SerializeField] GameObject wall;
    [SerializeField] GameObject bossHealthBar;

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        PlayerController controller = other.GetComponent<PlayerController>();

        if(controller != null)
        {
            fightObject.enabled = true;
            if(wall != null)
                wall.SetActive(true);

            if(bossHealthBar != null)
                bossHealthBar.SetActive(true);
        }
    }
}
