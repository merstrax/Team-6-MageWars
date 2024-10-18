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

    private List<string> _animations;

    public AnimationContainer()
    {
        _animations = new() {Idle, Walk, Run, Jump, Attack1, Attack2, Attack3, Attack4, Hit, Knockback, Stun, Death};
    }

    public void Inititialize()
    {
        _animations = new() {Idle, Walk, Run, Jump, Attack1, Attack2, Attack3, Attack4, Hit, Knockback, Stun, Death };
    }

    public string this[AnimationType type]
    {
        get
        {
            return _animations[(int)type];
        }
    }
}
