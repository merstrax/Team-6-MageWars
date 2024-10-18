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

    [SerializeField] PillarObjective frostPillar_wgl;
    [SerializeField] PillarObjective firePillar_wgl;
    [SerializeField] PillarObjective arcanePillar_wgl;
    [SerializeField] PillarObjective windPillar_wgl;
    private PillarFlags pillarFlags;

    [SerializeField] AludyneAltar aludyneAltar;
    [SerializeField] AludyneAltar aludyneAltar_wgl;
    private bool canEnterAltar;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

#if PLATFORM_WEBGL
        frostPillar_wgl.SetComplete(pillarFlags.HasFlag(PillarFlags.FROST));
        firePillar_wgl.SetComplete(pillarFlags.HasFlag(PillarFlags.FIRE));
        arcanePillar_wgl.SetComplete(pillarFlags.HasFlag(PillarFlags.ARCANE));
        windPillar_wgl.SetComplete(pillarFlags.HasFlag(PillarFlags.WIND));

        canEnterAltar = pillarFlags.HasFlag(PillarFlags.EVERYTHING);
        aludyneAltar_wgl.SetComplete(true);

        frostPillar.gameObject.SetActive(false);
        firePillar.gameObject.SetActive(false);
        arcanePillar.gameObject.SetActive(false);
        windPillar.gameObject.SetActive(false);
        aludyneAltar.gameObject.SetActive(false);
#else
        frostPillar.SetComplete(pillarFlags.HasFlag(PillarFlags.FROST));
        firePillar.SetComplete(pillarFlags.HasFlag(PillarFlags.FIRE));
        arcanePillar.SetComplete(pillarFlags.HasFlag(PillarFlags.ARCANE));
        windPillar.SetComplete(pillarFlags.HasFlag(PillarFlags.WIND));

        canEnterAltar = pillarFlags.HasFlag(PillarFlags.EVERYTHING);
        aludyneAltar.SetComplete(true);

        frostPillar_wgl.gameObject.SetActive(false);
        firePillar_wgl.gameObject.SetActive(false);
        arcanePillar_wgl.gameObject.SetActive(false);
        windPillar_wgl.gameObject.SetActive(false);
        aludyneAltar_wgl.gameObject.SetActive(false);
#endif
    }

    internal void SetPillarComplete(PillarFlags type)
    {
        pillarFlags |= type;

        canEnterAltar = pillarFlags.HasFlag(PillarFlags.EVERYTHING);
        if (canEnterAltar)
        {
            aludyneAltar.SetComplete(false);
        }
    }
}
