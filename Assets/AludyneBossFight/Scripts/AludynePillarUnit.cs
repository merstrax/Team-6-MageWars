using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AludynePillarUnit : Unit
{
    public override void OnDeath(Unit other = null)
    {
        AludyneBossFight aludyneBossFight = FindAnyObjectByType<AludyneBossFight>();

        aludyneBossFight.OnDeath(this);

        base.OnDeath(other);
    }
}
