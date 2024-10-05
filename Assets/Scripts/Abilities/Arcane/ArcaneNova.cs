using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneNova : Ability
{
    protected override void Cast(Transform end = null)
    {
        gameObject.transform.position = owner.transform.position;
        myCollider.enabled = true;
        CleanUp();
    }
}
