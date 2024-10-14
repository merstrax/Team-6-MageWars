using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZerrokArcaneMissile : Ability
{
    [SerializeField] Ability arcaneExplosion;

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger || other.gameObject.CompareTag(gameObject.tag))
            return;

        Unit _unit = other.GetComponent<Unit>();
        if (_unit != owner && _unit != null && !other.CompareTag(gameObject.tag))
        {
            SetOther(_unit);
            OnHit(_unit);
        }

        Instantiate(arcaneExplosion, transform.position, Quaternion.Euler(0, 0, 0));
    }
}
