using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AludyneBossFight : MonoBehaviour
{
    [Header("Elemental Pillars")]
    [SerializeField] AludynePillarUnit frostPillar;
    [SerializeField] AludynePillarUnit firePillar;
    [SerializeField] AludynePillarUnit arcanePillar;
    [SerializeField] AludynePillarUnit windPillar;
    [SerializeField] float pillarHealAmount;
    [SerializeField] float pillarHealTimer;

    List<Unit> pillarList;

    int pillarsRemaining;
    bool isFrostDead;
    bool isFireDead;
    bool isArcaneDead;
    bool isWindDead;
    bool doPillarHeal;

    [Header("Zerrok Info")]
    [SerializeField] GameObject zerrokObject;
    [SerializeField] Ability zerrokFireAbility;
    [SerializeField] Ability zerrokFrostAbility;
    [SerializeField] Ability zerrokWindAbility;
    [SerializeField] Ability zerrokArcaneAbility;
    [SerializeField] float zerrokAbilityTimer;

    [SerializeField] Unit zerrokFlameElemental;

    [Header("Lesser Elemental Info")]
    [SerializeField] Unit lesserElemental;
    [SerializeField] Transform[] lesserElementalSpawnLocations;
    [SerializeField] int lesserElementalSpawnCount;
    [SerializeField] float lesserElementalSpawnTime;

    [Header("Aludyne Info")]
    [SerializeField] Unit aludyneUnit;
    [SerializeField] Ability aludyneChaosVolley;
    [SerializeField] Ability aludyneGroundRupture;
    [SerializeField] Ability aludyneErruption;

    // Start is called before the first frame update
    void Start()
    {
        aludyneUnit.SetHealthCurrent(1.0f);

        pillarList.Add(arcanePillar);
        pillarList.Add(firePillar);
        pillarList.Add(frostPillar);
        pillarList.Add(windPillar);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ZerrokArcanePhase()
    {

    }

    private void ZerrokFirePhase()
    {

    }

    private void ZerrokFrostPhase()
    {

    }

    private void ZerrokWindPhase()
    {

    }

    private void Phase1_Complete()
    {

    }

    IEnumerator PillarHeal()
    {
        doPillarHeal = false;

        for(int i = 0; i < pillarList.Count; i++)
        {
            if (pillarList[i] == null)
            {
                pillarList.RemoveAt(i);
                continue;
            }
            //Do heal
        }

        yield return new WaitForSeconds(pillarHealTimer);
        doPillarHeal = true;
    }

    public void OnDeath(Unit unit)
    {

    }
}

public class Pillar : Unit
{
    
}
