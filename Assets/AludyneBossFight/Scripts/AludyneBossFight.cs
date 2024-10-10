using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AludyneBossFight : MonoBehaviour
{
    enum Phases{
        START, PHASE_1, PHASE_1_END, PHASE_2, END
    }

    [Header("Elemental Pillars")]
    [SerializeField] AludynePillarUnit frostPillar;
    [SerializeField] AludynePillarUnit firePillar;
    [SerializeField] AludynePillarUnit arcanePillar;
    [SerializeField] AludynePillarUnit windPillar;
    [SerializeField] float pillarHealAmount;
    [SerializeField] float pillarHealTimer;

    List<AludynePillarUnit> pillarList;

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
    [SerializeField] float zerrokPillarTimer;

    [SerializeField] Unit zerrokFlameElemental;

    AludynePillarUnit zerrokCurrentPillar;

    [Header("Lesser Elemental Info")]
    [SerializeField] Unit lesserElemental;
    [SerializeField] Transform[] lesserElementalSpawnLocations;
    [SerializeField] int lesserElementalSpawnCount;
    [SerializeField] float lesserElementalSpawnTime;

    bool doElementalSpawn;

    [Header("Aludyne Info")]
    [SerializeField] Unit aludyneUnit;
    [SerializeField] Ability aludyneChaosVolley;
    [SerializeField] Ability aludyneGroundRupture;
    [SerializeField] Ability aludyneErruption;

    Phases currentPhase = Phases.START;

    // Start is called before the first frame update
    void Start()
    {
        aludyneUnit.SetInvulnerable(true);
        //TODO - Add stun to unit and AI
        aludyneUnit.SetHealthCurrent(1.0f);

        pillarList.Add(arcanePillar);
        pillarList.Add(firePillar);
        pillarList.Add(frostPillar);
        pillarList.Add(windPillar);
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentPhase)
        {
            case Phases.PHASE_1:
                UpdatePhase1();
                break;
            case Phases.PHASE_2:
                UpdatePhase2();
                break;
            default:
                break;
        }
        
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

    private void UpdatePhase1()
    {
        if (GetPillarsRemaining() <= 0)
        {
            StartCoroutine(Phase1_End());
            return;
        }

        //Update to unit getcurrentpercent
        if(aludyneUnit.GetHealthCurrent() <= aludyneUnit.GetHealthMax())
        {
            StartCoroutine(Phase1_EndBad());
            return;
        }

        if (doPillarHeal)
        {
            StartCoroutine(PillarHeal());
        }

        if(doElementalSpawn)
        {
            StartCoroutine(ElementalSpawn());
        }
    }

    IEnumerator Phase1_End()
    {
        currentPhase = Phases.PHASE_1_END;

        //Do cinematic stuff here

        yield return new WaitForSeconds(pillarHealTimer);

        currentPhase = Phases.PHASE_2;
        aludyneUnit.SetInvulnerable(false);
    }

    IEnumerator Phase1_EndBad()
    {
        currentPhase = Phases.PHASE_1_END;

        yield return new WaitForSeconds(pillarHealTimer);

        currentPhase = Phases.PHASE_2;
    }

    private void UpdatePhase2()
    {
        //Add get health current percent to unit
        if(aludyneUnit.GetHealthCurrent() <= 0.05)
        {
            aludyneUnit.SetInvulnerable(true);
            //TODO - Set Stunned
            StartCoroutine(Phase2_End());
            return;
        }
    }

    IEnumerator Phase2_End()
    {
        currentPhase = Phases.END;

        yield return new WaitForSeconds(pillarHealTimer);

        //Game complete logic
    }

    private int GetPillarsRemaining()
    {
        return pillarList.Count;
    }

    IEnumerator PillarHeal()
    {
        doPillarHeal = false;

        foreach (AludynePillarUnit unit in pillarList)
        {
            if(unit == null) continue;

            //Do Heal
        }

        yield return new WaitForSeconds(pillarHealTimer);
        doPillarHeal = true;
    }

    IEnumerator ElementalSpawn()
    {
        doElementalSpawn = false;

        for(int i = 0; i < lesserElementalSpawnCount; i++)
        {
            int spawnLoc = Random.Range(0, lesserElementalSpawnLocations.Length);

            Instantiate(lesserElemental, lesserElementalSpawnLocations[spawnLoc].position, lesserElementalSpawnLocations[spawnLoc].rotation);
        }

        yield return new WaitForSeconds(lesserElementalSpawnTime);
        doElementalSpawn = true;
    }

    public void OnDeath(AludynePillarUnit unit)
    {
        pillarList.Remove(unit);

        isFrostDead = frostPillar == null;
        isFireDead = firePillar == null;
        isArcaneDead = arcanePillar == null;
        isWindDead = windPillar == null;

        if (zerrokCurrentPillar == unit)
        {
            //Do Zerrok pillar swap}
        }
    }
}
