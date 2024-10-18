#define _DEBUG

using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : Unit
{
    public static PlayerController instance;

    //Player logic variables
    [Header("Player Controller")]
    [SerializeField] Animator animator;

    //Player Shoot
    [Header("Player Abilities")]
    [SerializeField] Ability[] abilityPassive;
    [SerializeField] Ability[] abilities;
    readonly AbilityHandler[] abilityHandlers = new AbilityHandler[4];

    Ability movementAbility;

    [SerializeField] GameObject aoeTargetSelector;
    [SerializeField] LayerMask targetLayerMask;
    public int selectedAbility = -1;

    public AbilityHandler GetAbility(uint handlerID)
    {
        if (handlerID > 3) return null;

        return abilityHandlers[handlerID];
    }

    [Header("Player Audio")]
    [SerializeField] public AudioSource audioPlayer;
    [SerializeField] AudioClip[] audioDamage;
    [Range(0, 1)][SerializeField] float audioDamageVolume;
    [SerializeField] AudioClip[] audioWalk;
    [Range(0, 1)][SerializeField] float audioWalkVolume;
    [SerializeField] AudioClip[] audioJump;
    [Range(0, 1)][SerializeField] float audioJumpVolume;

    ITargetable target;
    bool canInteract;
    public bool IsDead;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        stats = new(healthBase, damageBase, defenseBase, speedBase, critChanceBase, critDamageBase, cooldownBase);

        UpdateStats();
        healthCurrent = healthMax;

        if (abilityPassive != null)
        {
            //abilityPassive[0] = Instantiate(abilityPassive[0], GetCastPos());
            //abilityHandlers[0].Setup(this, abilityPassive[0]);
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

        GameManager.instance.SetInteractMessage("");

        PlayerSpawnPoint _playerSpawn = FindFirstObjectByType<PlayerSpawnPoint>();
        transform.position = _playerSpawn.transform.position;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayerSpawnPoint _playerSpawn = FindFirstObjectByType<PlayerSpawnPoint>();
        transform.position = _playerSpawn.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsDead) return;
        UpdateTargeting();
        if (InputController.instance.Interact)
        {
            Interact();
        }

        if (abilityChannel != null)
        {
            float current = Time.time - castStart;
            float end = abilityHandler.GetAbility().Info().CastTime;
            GameManager.instance.UpdateCastbar(true, current, end);
        }

        for (int i = 0; i < InputController.instance.Ability.Length; i++)
        {
            if (InputController.instance.Ability[i] && abilityHandlers[i].ReadyToCast())
            {
                if (selectedAbility != i && selectedAbility != -1)
                {
                    InputController.instance.Ability[selectedAbility] = false;
                    InputController.instance.Ability[i] = false;
                    selectedAbility = -1;
                    aoeTargetSelector.SetActive(false);
                    break;
                }

                if (selectedAbility != -1)
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
                    if (!IsStunned() && abilityCasting == null)
                    {
                        CastAbility(abilityHandlers[i]);
                        abilityHandlers[i].StartCooldown();
                    }
                    InputController.instance.Ability[i] = false;
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
            if (!InputController.instance.Ability[selectedAbility] && !IsStunned() && abilityCasting == null)
            {
                CastAbility(abilityHandlers[selectedAbility]);
                abilityHandlers[selectedAbility].StartCooldown();
                aoeTargetSelector.SetActive(false);
                selectedAbility = -1;
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

            LayerMask _mask = LayerMask.NameToLayer("Targetable");

            //Check for EnemyAI targetables first
            if (Physics.Raycast(ray, out hit, 200.0f, ~_mask))
            {
                ITargetable targetHit = hit.collider.gameObject.GetComponentInParent<ITargetable>();

                if (targetHit != null)
                {
                    if (targetHit.GameObject().GetComponent<EnemyAI>() != null)
                    {
                        target?.OnTarget(false);
                        targetHit.OnTarget(true);
                        target = targetHit;
                    }
                }
                else
                {
                    target?.OnTarget(false);
                    target = null;
                }
            }

            //check for Interactables after enemies
            if (Physics.Raycast(ray, out hit, 10.0f, ~_mask) && target == null)
            {
                IInteractable targetHit = hit.collider.gameObject.GetComponentInParent<IInteractable>();

                if (targetHit != null)
                {
                    canInteract = true;
                    target?.OnTarget(false);
                    targetHit.OnTarget(true);
                    target = targetHit;
                }
                else
                {
                    canInteract = false;
                    target?.OnTarget(false);
                    target = null;
                }
            }
        }
        else
        {
            if (target != null)
            {
                target.OnTarget(false);
                target = null;
                canInteract = false;
            }
            screenCenter = new Vector3(0.5f, 0.45f, 0f);
            ray = Camera.main.ViewportPointToRay(screenCenter);

            if (Physics.Raycast(ray, out hit, 100.0f, ~targetLayerMask))
            {
                //hit.collider.CompareTag("Terrain") && 
                if (!hit.collider.CompareTag("Player") && aoeTargetSelector.activeInHierarchy)
                {
                    aoeTargetSelector.transform.position = Vector3.Lerp(aoeTargetSelector.transform.position, hit.point + Vector3.up, 10.0f * Time.deltaTime);
                }
            }
        }

        Debug.DrawRay(ray.GetPoint(0), ray.direction * 200.0f, Color.red);
    }

    #region Ability Handling
    AbilityHandler abilityHandler;
    Ability abilityCasting;
    Coroutine abilityChannel;
    float castStart;
    bool isChanneling;

    private Vector3 GetCastLocation()
    {
        Vector3 _castLoc = Vector3.zero;

        if (abilityCasting.Info().AbilityType == AbilityType.AREAOFEFFECT && abilityCasting.Info().IsTarget)
        {
            _castLoc = aoeTargetSelector.transform.position;
        }
        else if (target != null)
        {
            try
            {
                Unit hit = target.GameObject().GetComponentInParent<Unit>();
                if (hit != null)
                {
                    _castLoc = hit.transform.position;
                    _castLoc += ((hit.GetComponent<CapsuleCollider>().center) * hit.transform.lossyScale.magnitude);
                }
                else
                {
                    Vector3 screenCenter = new(0.5f, 0.5f, 0f);
                    Ray ray = Camera.main.ViewportPointToRay(screenCenter);

                    _castLoc = ray.GetPoint(abilityCasting.Info().AbilityRange);
                }
            }
            catch (Exception) { }
        }
        else
        {
            Vector3 screenCenter = new Vector3(0.5f, 0.5f, 0f);
            Ray ray = Camera.main.ViewportPointToRay(screenCenter);

            _castLoc = ray.GetPoint(abilityCasting.Info().AbilityRange);
        }

        return _castLoc;
    }

    private void CastAbility(AbilityHandler ability)
    {
        abilityHandler = ability;
        abilityCasting = Instantiate(ability.GetAbility(), GetCastPos(0).position, transform.rotation);
        abilityCasting.SetOwner(this);

        if (ability.IsMovementAbility())
        {
            abilityCasting.CastMovement();
            return;
        }

        abilityCasting.CastStart(this, GetCastLocation());
    }

    public override void OnCastStart(Unit other = null, Ability source = null, Damage damage = default)
    {
        string animation = animations[source.Info().AnimationType];
        animator.SetTrigger(animation);
    }

    public void CastByAnimation()
    {
        if (abilityCasting == null) return;

        float tickSpeed = abilityCasting.Info().EffectTickSpeed;
        castStart = Time.time - tickSpeed;
        if (abilityCasting.Info().CastType == CastType.CHANNEL)
        {
            isChanneling = true;
            abilityChannel = StartCoroutine(ChannelCast(abilityCasting.Info().EffectTickSpeed));
        }

        abilityCasting.Cast(GetCastPos(0).position, GetCastLocation());
    }

    public override void OnCast(Unit other = null, Ability source = null, Damage damage = default)
    {
        float tickSpeed = source.Info().EffectTickSpeed;
        float nextCast = castStart + source.Info().CastTime + tickSpeed;

        if (source.Info().CastType == CastType.CHANNEL && (nextCast < Time.time))
        {
            isChanneling = false;
        }
    }

    IEnumerator ChannelCast(float tickSpeed)
    {
        if (castStart + abilityHandler.GetAbility().Info().CastTime < Time.time + tickSpeed)
            isChanneling = false;

        yield return new WaitForSeconds(tickSpeed);
        try
        {
            abilityCasting = Instantiate(abilityHandler.GetAbility(), GetCastPos(0).position, transform.rotation);
            abilityCasting.SetOwner(this);

            abilityCasting.Cast(GetCastLocation());
        }
        catch (Exception) { }

        if (isChanneling)
            abilityChannel = StartCoroutine(ChannelCast(tickSpeed));
    }

    public override void OnCastEnd(Unit other = null, Ability source = null, Damage damage = default)
    {
        if (isChanneling) return;

        animator.SetTrigger("AttackEnd");
        abilityCasting = null;
        abilityHandler = null;
        abilityChannel = null;
        castStart = 0.0f;

        GameManager.instance.UpdateCastbar(false);
    }
    #endregion

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

        if (healthCurrent <= 0)
        {
            OnDeath();
        }
    }

    #region Trigger Events

    public override void OnDamaged(Unit other = null, Ability source = null, Damage damage = default)
    {
        animator.SetTrigger("Hit");
        GameManager.instance.UpdateHealthbar(healthCurrent, healthMax);
    }

    public override void OnStun(Unit other = null, Ability source = null, Damage damage = default)
    {
        animator.SetTrigger("Stun");
    }

    public override void OnStunEnd(Unit other = null, Ability source = null, Damage damage = default)
    {
        animator.SetTrigger("StunEnd");
    }

    public override void OnDeath(Unit other = null, Ability source = null, Damage damage = default)
    {
        if (IsDead) return;

        animator.SetTrigger("Death");
        StartCoroutine(ShowDeathScreen());
    }
    #endregion

    IEnumerator ShowDeathScreen()
    {
        IsDead = true;
        yield return new WaitForSeconds(3.0f);
        GameManager.instance.youLose();
    }

    public void Interact()
    {
        if (canInteract)
        {
            (target as IInteractable).OnInteract();
        }
    }
}
