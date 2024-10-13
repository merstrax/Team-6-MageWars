using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZerrokWhirlingWind : Ability
{
    [SerializeField] AbilityStats whirlingWindSlow;

    protected override void Update()
    {
        base.Update();

        transform.LookAt(GameManager.instance.player.transform.position);
        myRigidbody.velocity = transform.forward * AbilityInfo.AbilitySpeed;
        //myRigidbody.MovePosition(GameManager.instance.player.transform.position);
    }

    protected override void OnDamage(Unit other)
    {
        base.OnDamage(other);
        DoEffectApply(other, whirlingWindSlow);
    }
}
