using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneNova : Ability
{
    public override void Cast(Vector3 end = default)
    {
        gameObject.transform.position = owner.transform.position;
        myCollider.enabled = true;
        CleanUp();
    }
}
