using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ClassStats : ScriptableObject
{
    [Header("Class Information")]
    public string ElementName;
    public ElementType ElementType;

    [Header("Abilities")]
    public Ability Ability1;
    public Ability Ability2;
    public Ability Ability3;
    public Ability Ability4;

    [Header("Passives")]
    public AbilityStats[] passives;
}
