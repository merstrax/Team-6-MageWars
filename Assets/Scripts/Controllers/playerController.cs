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
    readonly AbilityHandler[] abilityHandlers = new AbilityHandler[4];
    Ability movementAbility;

    public AbilityHandler GetAbility(uint handlerID)
    {
        if(handlerID > 3) return null;

        return abilityHandlers[handlerID];
    }

    [Header("Player Audio")]
    [SerializeField] AudioSource audioPlayer;
    [SerializeField] AudioClip[] audioDamage;
    [Range(0, 1)][SerializeField] float audioDamageVolume;
    [SerializeField] AudioClip[] audioWalk;
    [Range(0, 1)][SerializeField] float audioWalkVolume;
    [SerializeField] AudioClip[] audioJump;
    [Range(0, 1)][SerializeField] float audioJumpVolume;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        if (abilityPassive != null)
        {
            abilityPassive = Instantiate(abilityPassive, GetCastPos());
            abilityHandlers[0].Setup(this, abilityPassive);
        }
        
        for (int i = 0; i < abilities.Length; i++)
        {
            if (abilities[i] != null)
            {
                abilityHandlers[i] = gameObject.AddComponent<AbilityHandler>();
                abilityHandlers[i].Setup(this, abilities[i]);
                if (abilityHandlers[i].IsMovementAbility())
                {
                    movementAbility = abilityHandlers[i].GetAbility();
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
                CastAbility(abilityHandlers[i]);
                abilityHandlers[i].StartCooldown();
                inputController.ability[i] = false;
            }
        }
    }

    private void CastAbility(AbilityHandler ability)
    {
        Ability _ability = Instantiate(ability.GetAbility(), GetCastPos().position, transform.rotation);
        _ability.SetOwner(this);

        if (ability.IsMovementAbility())
        {
            
            _ability.CastMovement();
            return;
        }

        Vector3 screenCenter = new Vector3(0.5f, 0.66f, 0f);
        Ray ray = Camera.main.ViewportPointToRay(screenCenter);

        _ability.StartCast(this, ray.GetPoint(200.0f));
    }

    public override void TakeDamage(Damage damage, Unit other = null)
    {
        healthCurrent -= CalculateDefense(damage.Amount);

        if (healthCurrent <= 0)
        {
            //Player Death Handling
        }
    }
}
