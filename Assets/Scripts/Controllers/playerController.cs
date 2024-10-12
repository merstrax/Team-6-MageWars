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

    [SerializeField] GameObject aoeTargetSelector;
    [SerializeField] LayerMask targetLayerMask;
    public int selectedAbility = -1;

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

    Unit target;

    // Start is called before the first frame update
    protected override void Start()
    {
        stats = new(healthBase, damageBase, defenseBase, speedBase, critChanceBase, critDamageBase, cooldownBase);

        UpdateStats();
        healthCurrent = healthMax;

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
        UpdateTargeting();
        

        for (int i = 0; i < inputController.ability.Length; i++)
        {
            if (inputController.ability[i] && abilityHandlers[i].ReadyToCast())
            {
                if (selectedAbility != i && selectedAbility != -1)
                {
                    inputController.ability[selectedAbility] = false;
                    inputController.ability[i] = false;
                    selectedAbility = -1;
                    aoeTargetSelector.SetActive(false);
                    break;
                }

                if(selectedAbility != -1)
                {
                    continue;
                }

                if (abilityHandlers[i].GetAbility().Info().AbilityType == AbilityType.AREAOFEFFECT && abilityHandlers[i].GetAbility().Info().IsTarget)
                {
                    aoeTargetSelector.SetActive(true);
                    aoeTargetSelector.GetComponent<AoETargetSpin>().SetScale(1.8f * abilityHandlers[i].GetAbility().GetComponent<SphereCollider>().radius);
                    selectedAbility = i;
                }
                else
                {
                    if (!IsStunned())
                    {
                        CastAbility(abilityHandlers[i]);
                        abilityHandlers[i].StartCooldown();
                    }
                    inputController.ability[i] = false;
                }
                break;
            }
        }

        if (IsStunned())
        {
            aoeTargetSelector.SetActive(false);
            selectedAbility = -1;
        }

        if (selectedAbility != -1)
        {
            if (!inputController.ability[selectedAbility] && !IsStunned())
            {
                CastAbility(abilityHandlers[selectedAbility]);
                abilityHandlers[selectedAbility].StartCooldown();
                aoeTargetSelector.SetActive(false);
                selectedAbility= -1;
                //inputController.ability[selectedAbility] = false;
            }
        }
    }

    private void UpdateTargeting()
    {
        Vector3 screenCenter;
        Ray ray;
        RaycastHit hit;

        if (selectedAbility == -1)
        {
            screenCenter = new Vector3(0.5f, 0.5f, 0f);
            ray = Camera.main.ViewportPointToRay(screenCenter);

            if (Physics.Raycast(ray, out hit, 200.0f))
            {
                Unit targetHit = hit.collider.gameObject.GetComponentInParent<Unit>();

                if (targetHit != null && !hit.collider.CompareTag("Player"))
                {
                    if (target != null) target.TargetOutline(false);
                    targetHit.TargetOutline();
                    target = targetHit;
                }
                else if (target != null)
                {
                    target.TargetOutline(false);
                    target = null;
                }
            }
        }
        else
        {
            if (target != null)
            {
                target.TargetOutline(false);
                target = null;
            }
            screenCenter = new Vector3(0.5f, 0.45f, 0f);
            ray = Camera.main.ViewportPointToRay(screenCenter);

            if (Physics.Raycast(ray, out hit, 100.0f, ~targetLayerMask))
            {
                //hit.collider.CompareTag("Terrain") && 
                if (aoeTargetSelector.activeInHierarchy)
                {
                    aoeTargetSelector.transform.position = Vector3.Lerp(aoeTargetSelector.transform.position, hit.point + (Vector3.up * 1.05f), 10.0f * Time.deltaTime);
                }
            }
        }

        Debug.DrawRay(ray.GetPoint(0), ray.direction * 200.0f, Color.red);
    }

    public override void TargetOutline(bool target = true) { } //Stop Target Outline from hitting player

    private void CastAbility(AbilityHandler ability)
    {
        Ability _ability = Instantiate(ability.GetAbility(), GetCastPos().position, transform.rotation);
        _ability.SetOwner(this);

        if (ability.IsMovementAbility())
        {
            _ability.CastMovement();
            return;
        }

        Vector3 toCastPos;

        if (_ability.Info().AbilityType == AbilityType.AREAOFEFFECT && _ability.Info().IsTarget)
        {
            toCastPos = aoeTargetSelector.transform.position;
        }
        else if (target == null)
        {
            Vector3 screenCenter = new Vector3(0.5f, 0.55f, 0f);
            Ray ray = Camera.main.ViewportPointToRay(screenCenter);

            toCastPos = ray.GetPoint(50.0f);
        }
        else
        {
            toCastPos = target.transform.position;
            toCastPos.y += (target.GetComponent<CapsuleCollider>().height * 0.75f);
        }

        _ability.StartCast(this, toCastPos);
    }

    public override void TakeDamage(Damage damage, Ability source = null, Unit other = null)
    {
        if (IsInvulnerable()) return;

        damage.Amount = CalculateDefense(damage.Amount);

        if (other != null)
        {
            other.ProccessEvent(TriggerFlags.ON_DAMAGE, this, source, damage);
        }

        ProccessEvent(TriggerFlags.ON_DAMAGED, other, source, damage);

        healthCurrent -= damage.Amount;
    }
}
