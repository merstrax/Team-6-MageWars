using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationType
{
    IDLE,
    WALK,
    RUN,
    JUMP,
    ATTACK_1,
    ATTACK_2,
    ATTACK_3,
    ATTACK_4,
    HIT,
    KNOCKBACK,
    STUN,
    DEATH
}

[CreateAssetMenu]
public class AnimationContainer : ScriptableObject
{
    public string Idle;
    public string Walk;
    public string Run;
    public string Jump;
    public string Attack1;
    public string Attack2;
    public string Attack3;
    public string Attack4;
    public string Hit;
    public string Knockback;
    public string Stun;
    public string Death;

    private List<int> _animations;

    public AnimationContainer()
    {
        int _idle = Animator.StringToHash(Idle);
        int _walk = Animator.StringToHash(Walk);
        int _run = Animator.StringToHash(Run);
        int _jump = Animator.StringToHash(Jump);
        int _attack1 = Animator.StringToHash(Attack1);
        int _attack2 = Animator.StringToHash(Attack2);
        int _attack3 = Animator.StringToHash(Attack3);
        int _attack4 = Animator.StringToHash(Attack4);
        int _hit = Animator.StringToHash(Hit);
        int _knockback = Animator.StringToHash(Knockback);
        int _stun = Animator.StringToHash(Stun);
        int _death = Animator.StringToHash(Death);

        _animations = new() { _idle, _walk, _run, _jump, _attack1, _attack2, _attack3, _attack4, _hit, _knockback, _stun, _death };
    }

    public void Inititialize()
    {
        int _idle = Animator.StringToHash(Idle);
        int _walk = Animator.StringToHash(Walk);
        int _run = Animator.StringToHash(Run);
        int _jump = Animator.StringToHash(Jump);
        int _attack1 = Animator.StringToHash(Attack1);
        int _attack2 = Animator.StringToHash(Attack2);
        int _attack3 = Animator.StringToHash(Attack3);
        int _attack4 = Animator.StringToHash(Attack4);
        int _hit = Animator.StringToHash(Hit);
        int _knockback = Animator.StringToHash(Knockback);
        int _stun = Animator.StringToHash(Stun);
        int _death = Animator.StringToHash(Death);

        _animations = new() { _idle, _walk, _run, _jump, _attack1, _attack2, _attack3, _attack4, _hit, _knockback, _stun, _death };
    }

    public int this[AnimationType type]
    {
        get
        {
            return _animations[(int)type];
        }
    }
}
