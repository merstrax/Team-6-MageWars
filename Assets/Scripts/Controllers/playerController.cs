#define _DEBUG

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : Unit
{
    //Player logic variables
    [Header("Player Controller")]
    [SerializeField] PlayerInputController inputController;

    //Player Movement
    [Header("Player Movement")]
    [SerializeField] float dashTiming;

    //Player Shoot
    [Header("Player Abilities")]
    [SerializeField] Ability abilityPassive;
    [SerializeField] Ability[] abilities;
    readonly Ability[] abilityHandlers = new Ability[4];
    Ability movementAbility;

    [Header("Player Audio")]
    [SerializeField] AudioSource audioPlayer;
    [SerializeField] AudioClip[] audioDamage;
    [Range(0, 1)][SerializeField] float audioDamageVolume;
    [SerializeField] AudioClip[] audioWalk;
    [Range(0, 1)][SerializeField] float audioWalkVolume;
    [SerializeField] AudioClip[] audioJump;
    [Range(0, 1)][SerializeField] float audioJumpVolume;
   
    // Start is called before the first frame update
    void Start()
    {
        if (abilityPassive != null)
        {
            abilityPassive = Instantiate(abilityPassive, GetCastPos());
            abilityPassive.SetAsHandler(this);
        }

        for (int i = 0; i < abilities.Length; i++)
        {
            if (abilities[i] != null)
            {
                abilityHandlers[i] = Instantiate(abilities[i], GetCastPos());
                abilityHandlers[i].SetAsHandler(this);
                if (abilities[i].IsMovementAbility())
                {
                    movementAbility = abilityHandlers[i];
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        Vector3 screenCenter = new Vector3(0.5f, 0.66f, 0f);
        Ray ray = Camera.main.ViewportPointToRay(screenCenter);
        Debug.DrawRay(ray.GetPoint(0), ray.direction * 200.0f, Color.red);
#endif

        for(int i = 0; i < inputController.ability.Length; i++)
        {
            if (inputController.ability[i] && abilityHandlers[i].ReadyToCast())
            {
                CastAbility(abilities[i]);
                abilityHandlers[i].StartCooldown();
                inputController.ability[i] = false;
            }
        }
    }

    private void CastAbility(Ability ability)
    {
        if(ability.IsMovementAbility())
        {
            movementAbility.CastMovement();
            return;
        }

        Ability _ability = Instantiate(ability, GetCastPos().position, transform.rotation);

        Vector3 screenCenter = new Vector3(0.5f, 0.66f, 0f);
        Ray ray = Camera.main.ViewportPointToRay(screenCenter);

        _ability.StartCast(this, ray.GetPoint(200.0f));
    }

    public override void TakeDamage(float amount, Unit other = null)
    {
        healthCurrent -= (amount * defenseModifier);

        if (healthCurrent <= 0)
        {
            //Player Death Handling
        }
    }
}
