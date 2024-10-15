using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AludyneUnitAI : EnemyAI
{

    public void SetSleeping()
    {
        SetHealthCurrent(1.0f);
        ApplyStatus(StatusFlag.INVULNERABLE);
        ApplyStatus(StatusFlag.STUNNED);
        UpdateInterface();
        OnTarget(false);
        IsTargetDisabled = true;

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
        IsTargetDisabled = false;
    }

    public override void OnDeath(Unit other = null, Ability source = null, Damage damage = default)
    {
        ApplyStatus(StatusFlag.INVULNERABLE);
        ApplyStatus(StatusFlag.STUNNED);

        animator.SetTrigger("Kneel");
        IsTargetDisabled = true;
    }

    public void DoDeathAnimation()
    {
        healthCurrent = 0;
        UpdateInterface();
        animator.SetTrigger("Death");
    }
}
