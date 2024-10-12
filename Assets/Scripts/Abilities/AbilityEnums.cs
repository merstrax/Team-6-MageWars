using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElementType
{
    NONE,
    ARCANE,
    FROST,
    FIRE,
    WIND,
    PHYSICAL
}

public enum AbilityType
{
    PROJECTILE,
    AREAOFEFFECT,
    EFFECT,
    MOVEMENT,
    TELEPORT,
    JUMP
}

public enum CastType
{
    INSTANT,
    CASTTIME,
    CHANNEL
}

public enum CooldownType
{
    NORMAL,
    START,
    FINISH,
}

public enum ResourceType
{
    NONE,
    CHARGES,
    COMBO
}

public enum EffectType
{
    DAMAGE,
    HEAL,
    STATUS,
    MODIFIER,
}

public enum EffectTargetType
{
    OWNER,
    OTHER,
}

public enum EffectStatusType
{
    NONE,
    SLOWED = 1,
    ROOTED = 2,
    STUNNED = 4,
    INVULNERABLE = 8,
    CASTING = 16,
}

public enum EffectModifierType
{
    NONE,
    ADD,
    MULTIPLY
}

//These flags choose when this effect can trigger
[Flags] public enum TriggerFlags
{
    NONE = 0,
    ON_CAST_START = 1,
    ON_CAST = 2,
    ON_CAST_END = 4,
    ON_HIT = 8,
    ON_DAMAGE = 16,
    ON_DAMAGED = 32,
    ON_KILL = 64,
    ON_UPDATE = 128,
    ON_HEAL = 256,
    ON_INTERRUPT = 512,
    ON_SLOW= 1024,
    ON_ROOT = 2048,
    ON_STUN = 4096,
    ON_DEATH = 8192,
}



//These flags choose which element the effect is applied to
[Flags] public enum EffectElementFlags
{
    NONE = 0,
    ARCANE = 1,
    FROST = 2,
    FIRE = 4,
    WIND = 8,
    PHYSICAL = 16
}
