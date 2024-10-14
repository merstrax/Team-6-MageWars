using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AludyneUnitAI : enemyAI
{

    public void SetSleeping()
    {
        SetHealthCurrent(1.0f);
        ApplyStatus(StatusFlag.INVULNERABLE);
        ApplyStatus(StatusFlag.STUNNED);
        UpdateInterface();
        TargetOutline(false);
        SetTargetable(false);

        animator.SetTrigger("Sleep");
    }

    public void preAwaken()
    {
        animator.SetTrigger("Awaken");
    }

    public void Awaken()
    {
        RemoveStatus(StatusFlag.INVULNERABLE);
        RemoveStatus(StatusFlag.STUNNED);
        SetTargetable(true);
    }

    public override void OnDeath(Unit other = null, Ability source = null, Damage damage = default)
    {
        ApplyStatus(StatusFlag.INVULNERABLE);
        ApplyStatus(StatusFlag.STUNNED);

        animator.SetTrigger("Kneel");
        SetTargetable(false);
    }

    public void DoDeathAnimation()
    {
        healthCurrent = 0;
        UpdateInterface();
        animator.SetTrigger("Death");
    }
}
