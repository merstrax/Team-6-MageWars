using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AludyneFightTrigger : MonoBehaviour
{
    [SerializeField] AludyneBossFight fightObject;
    [SerializeField] GameObject wall;
    [SerializeField] Image bossHealthBar;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            fightObject.enabled = true;
            if(wall != null)
                wall.SetActive(true);

            if(bossHealthBar != null)
                bossHealthBar.enabled = true;
        }
    }
}
