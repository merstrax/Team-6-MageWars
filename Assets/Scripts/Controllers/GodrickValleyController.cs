using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum PillarFlags
{
    NONE,
    FROST,
    FIRE,
    ARCANE = 4,
    WIND = 8,

    EVERYTHING = FROST | FIRE | ARCANE | WIND,
}

public class GodrickValleyController : MonoBehaviour
{
    public static GodrickValleyController instance;

    [SerializeField] PillarObjective frostPillar;
    [SerializeField] PillarObjective firePillar;
    [SerializeField] PillarObjective arcanePillar;
    [SerializeField] PillarObjective windPillar;
    private PillarFlags pillarFlags;

    private bool canEnterAltar;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        frostPillar.SetComplete(pillarFlags.HasFlag(PillarFlags.FROST));
        firePillar.SetComplete(pillarFlags.HasFlag(PillarFlags.FIRE));
        arcanePillar.SetComplete(pillarFlags.HasFlag(PillarFlags.ARCANE));
        windPillar.SetComplete(pillarFlags.HasFlag(PillarFlags.WIND));

        canEnterAltar = pillarFlags.HasFlag(PillarFlags.EVERYTHING);

        Debug.Log((int)PillarFlags.EVERYTHING);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void SetPillarComplete(PillarFlags type)
    {
        pillarFlags |= type;

        canEnterAltar = pillarFlags.HasFlag(PillarFlags.EVERYTHING);
    }
}
