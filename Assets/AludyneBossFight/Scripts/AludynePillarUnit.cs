using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AludynePillarUnit : EnemyAI
{
    [SerializeField] string pillarAbility;

    protected override void Start()
    {
        healthRegen = enemyStats.healthRegen;
        healthBase = enemyStats.healthBase;
        damageBase = enemyStats.damageBase;
        defenseBase = enemyStats.defenseBase;
        speedBase = enemyStats.speedBase;
        critChanceBase = enemyStats.critChanceBase;
        critDamageBase = enemyStats.critDamageBase;
        cooldownBase = enemyStats.cooldownBase;
        aggroRange = enemyStats.aggroRange;
        kiteRange = enemyStats.kiteRange;
        dropChance = enemyStats.dropChance;
        viewAngle = enemyStats.viewAngle;
        SetupTarget(targetMaterial);

        stats = new(healthBase, damageBase, defenseBase, speedBase, critChanceBase, critDamageBase, cooldownBase);

        UpdateStats();
        healthCurrent = healthMax;

        UpdateInterface();
    }

    protected override void Update(){}

    public override void OnDeath(Unit other = null, Ability source = null, Damage damage = default)
    {
        AludyneBossFight aludyneBossFight = FindAnyObjectByType<AludyneBossFight>();

        aludyneBossFight.OnDeath(this);

        DeathAfterAnimation();
    }

    public string PillarAbility()
    {
        return pillarAbility;
    }
    public override void OnCastStart(Unit other = null, Ability source = null, Damage damage = default) { }
    public override void OnCastEnd(Unit other = null, Ability source = null, Damage damage = default) { }
    public override void OnDamaged(Unit other = null, Ability source = null, Damage damage = default)
    {

    }

    public override void OnSlow(Unit other = null, Ability source = null, Damage damage = default)
    {

    }

    public override void OnStun(Unit other = null, Ability source = null, Damage damage = default)
    {

    }

    public override void OnStunEnd(Unit other = null, Ability source = null, Damage damage = default)
    {

    }

    public override void OnRoot(Unit other = null, Ability source = null, Damage damage = default)
    {

    }
}
