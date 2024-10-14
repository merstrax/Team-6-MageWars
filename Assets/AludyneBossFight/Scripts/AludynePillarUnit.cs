using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AludynePillarUnit : Unit
{
    [SerializeField] string pillarAbility;

    public override void OnDeath(Unit other = null, Ability source = null, Damage damage = default)
    {
        AludyneBossFight aludyneBossFight = FindAnyObjectByType<AludyneBossFight>();

        aludyneBossFight.OnDeath(this);

        base.OnDeath(other);
    }

    public string PillarAbility()
    {
        return pillarAbility;
    }
}
