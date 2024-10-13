using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZerrokFlameWave : Ability
{
    [SerializeField] Unit lesserElemental;

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        Unit _unit = other.GetComponent<Unit>();
        if (_unit != null && _unit.GetUnitName() == lesserElemental.GetUnitName())
        {
            DoEffectApply(_unit, Info().EffectAbility);
        }
    }
}
