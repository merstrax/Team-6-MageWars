#define _DEBUG

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : Unit
{
    //Player logic variables
    [Header("Player Controller")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] Animator animator;

    [Header("Player Stats")]

    //Player Movement
    [Header("Player Movement")]
    [Range(0, 10)][SerializeField] float speed;
    [Range(0, 3)][SerializeField] float sprintMod;
    [Range(0, 3)][SerializeField] int jumpMax;
    [Range(0, 15)][SerializeField] int jumpSpeed;
    [Range(0, 50)][SerializeField] int gravity;
    [SerializeField] float dashTiming;

    //Player Shoot
    [Header("Player Abilities")]
    [SerializeField] Ability abilityPassive;
    Ability abilityPassiveHandler;
    [SerializeField] Ability ability1;
    Ability ability1Handler;
    [SerializeField] Ability ability2;
    Ability ability2Handler;
    [SerializeField] Ability ability3;
    Ability ability3Handler;
    [SerializeField] Ability ability4;
    Ability ability4Handler;

    [Header("Player Audio")]
    [SerializeField] AudioSource audioPlayer;
    [SerializeField] AudioClip[] audioDamage;
    [Range(0, 1)][SerializeField] float audioDamageVolume;
    [SerializeField] AudioClip[] audioWalk;
    [Range(0, 1)][SerializeField] float audioWalkVolume;
    [SerializeField] AudioClip[] audioJump;
    [Range(0, 1)][SerializeField] float audioJumpVolume;

    Vector3 playerVel;
    

    int jumpCount;

    bool isSprinting;
    public bool IsSprinting() { return isSprinting; }

    Ability lastAbility;

    public enum InputDirection
    {
        NONE,
        LEFT,
        RIGHT,
        UP, 
        DOWN,
    }

    InputDirection lastInput;
    float lastInputTime;
    bool doDash;
    Ability movementAbility;

    // Start is called before the first frame update
    void Start()
    {
        if (abilityPassive != null)
        {
            abilityPassiveHandler = Instantiate(abilityPassive, GetCastPos());
            abilityPassiveHandler.SetAsHandler(this);
        }

        if (ability1 != null)
        {
            ability1Handler = Instantiate(ability1, GetCastPos());
            ability1Handler.SetAsHandler(this);
            if(ability1Handler.IsMovementAbility())
            {
                movementAbility = ability1Handler;
            }
        }

        if (ability2 != null) { 
            ability2Handler = Instantiate(ability2, GetCastPos());
            ability2Handler.SetAsHandler(this);
            if (ability2Handler.IsMovementAbility())
            {
                movementAbility = ability2Handler;
            }
        }

        if (ability3 != null)
        {
            ability3Handler = Instantiate(ability3, GetCastPos());
            ability3Handler.SetAsHandler(this);
            if (ability3Handler.IsMovementAbility())
            {
                movementAbility = ability3Handler;
            }
        }

        if (ability4 != null)
        {
            ability4Handler = Instantiate(ability4, GetCastPos());
            ability4Handler.SetAsHandler(this);
            if (ability4Handler.IsMovementAbility())
            {
                movementAbility = ability4Handler;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 400, Color.red);

        if (Input.GetButtonDown("Ability1") && ability1Handler.ReadyToCast())
        {
            lastAbility = ability1Handler;
            CastAbility(ability1);
        }

        if (Input.GetButtonDown("Ability2") && ability2Handler.ReadyToCast())
        {
            lastAbility = ability2Handler;
            CastAbility(ability2);
        }

        if (Input.GetButtonDown("Ability3") && ability3Handler.ReadyToCast())
        {
            lastAbility = ability3Handler;
            CastAbility(ability3);
        }

        if (Input.GetButtonDown("Ability4") && ability4Handler.ReadyToCast())
        {
            lastAbility = ability4Handler;
            CastAbility(ability4);
        }

        UpdateMovement();
        UpdateDash();
        UpdateSprint();
    }

    void UpdateDash()
    {
        if (!movementAbility.ReadyToCast()) return;

        if (Input.GetButtonDown("Left"))
        {
            if (lastInput == InputDirection.LEFT && lastInputTime + dashTiming > Time.time && CanDash())
            {
                doDash = true;
            }
            else
            {
                lastInput = InputDirection.LEFT;
                lastInputTime = Time.time;
                doDash = false;
            }
        }else if (Input.GetButtonDown("Right"))
        {
            if (lastInput == InputDirection.RIGHT && lastInputTime + dashTiming > Time.time && CanDash())
            {
                doDash = true;
            }
            else
            {
                lastInput = InputDirection.RIGHT;
                lastInputTime = Time.time;
                doDash = false;
            }
        }
        else if (Input.GetButtonDown("Up"))
        {
            if (lastInput == InputDirection.UP && lastInputTime + dashTiming > Time.time && CanDash())
            {
                doDash = true;
            }
            else
            {
                lastInput = InputDirection.UP;
                lastInputTime = Time.time;
                doDash = false;
            }
        }
        else if (Input.GetButtonDown("Down"))
        {
            if (lastInput == InputDirection.DOWN && lastInputTime + dashTiming > Time.time && CanDash())
            {
                doDash = true;
            }
            else
            {
                lastInput = InputDirection.DOWN;
                lastInputTime = Time.time;
                doDash = false;
            }
        }

        if(HasMovementAbility() && doDash && movementAbility.ReadyToCast())
        {
            movementAbility.CastMovement(lastInput);
            doDash = false;
            lastInput = InputDirection.NONE;
            lastAbility = movementAbility;
        }
    }

    bool CanDash()
    {
        return !doDash && movementAbility.ReadyToCast();
    }

    bool HasMovementAbility()
    {
        return movementAbility != null;
    }

    void UpdateMovement()
    {
        //Reset jump variables
        if (controller.isGrounded)
        {
            playerVel = Vector3.zero;
            jumpCount = 0;
            animator.SetBool("Jumping", false);
        }

        //Transform and move based on local space
        moveDir = Input.GetAxis("Horizontal") * transform.right +
                    Input.GetAxis("Vertical") * transform.forward;
        
        controller.Move(speed * Time.deltaTime * moveDir);

        float agentSpeed = Input.GetAxis("Horizontal");
        float animSpeed = animator.GetFloat("MoveX");
        animator.SetFloat("MoveX", Mathf.Lerp(animSpeed, agentSpeed, Time.deltaTime * 5.0f));

        agentSpeed = Input.GetAxis("Vertical");
        animSpeed = animator.GetFloat("MoveZ");
        animator.SetFloat("MoveZ", Mathf.Lerp(animSpeed, agentSpeed, Time.deltaTime * 5.0f));

        //Jump logic
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            animator.Play("JumpStart");
            animator.SetBool("Jumping", true);
            jumpCount++;
            playerVel.y = jumpSpeed;
        }

        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;
    }
    

    void UpdateSprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            isSprinting = false;
        }
    }

    private void CastAbility(Ability ability)
    {
        if(ability.IsMovementAbility())
        {
            movementAbility.CastMovement();
            doDash = false;
            lastInput = InputDirection.NONE;
            return;
        }

        Ability _ability = Instantiate(ability, GetCastPos().position, transform.rotation);

        Vector3 screenCenter = new Vector3(0.5f, 0.5f, 0f);
        Ray ray = Camera.main.ViewportPointToRay(screenCenter);

        _ability.StartCast(this, ray.GetPoint(200.0f));
    }

    public override void OnCast(Ability ability = null)
    {
        lastAbility.StartCooldown();
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
