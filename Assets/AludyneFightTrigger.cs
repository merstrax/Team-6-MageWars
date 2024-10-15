using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AludyneFightTrigger : MonoBehaviour
{
    [SerializeField] AludyneBossFight fightObject;
    [SerializeField] GameObject wall;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            fightObject.gameObject.SetActive(true);
            wall.SetActive(true);
        }
    }
}
