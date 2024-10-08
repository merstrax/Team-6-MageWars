using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public struct Damage
{
    public float Amount { get; set; }
    public bool IsCritical { get; set; }

    public Damage(float amount, bool isCritical = false) { Amount = amount; IsCritical = isCritical; }
}

public interface IDamage
{
    protected const float DefenseCoefficient = 20.0f;
    //void TakeDamage(float amount, Unit other = null);
    void TakeDamage(Damage damage, Unit other = null);
}
