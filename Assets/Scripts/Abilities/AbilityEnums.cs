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
    STUN,
    SLOW,
    ROOT,
    KNOCKBACK
}

public enum EffectModifierType
{
    ADD,
    MULTIPLY
}

//These flags choose when this effect can trigger
[Flags] public enum EffectTriggerFlags
{
    ON_CAST_START = 0x01,
    ON_CAST = 0x02,
    ON_CAST_END = 0x04,
    ON_HIT = 0x08,
    ON_DAMAGE = 0x10,
    ON_DAMAGED = 0x20,
    ON_KILL = 0x40,
    ON_UPDATE = 0x80,
}

//These flags choose which attribute the effect is applied to
[Flags] public enum EffectAttributeFlags
{
    DAMAGE = 0x01,
    DEFENSE = 0x02,
    COOLDOWN = 0x04,
    MOVESPEED = 0x08,
    JUMPHEIGHT = 0x10,
    JUMPSPEED = 0x20,
    CRIT_CHANCE = 0x40,
    CRIT_DAMAGE = 0x80,
}

//These flags choose which element the effect is applied to
[Flags] public enum EffectElementFlags
{
    ARCANE = 0x01,
    FROST = 0x02,
    FIRE = 0x04,
    WIND = 0x08,
    PHYSICAL = 0x10
}
