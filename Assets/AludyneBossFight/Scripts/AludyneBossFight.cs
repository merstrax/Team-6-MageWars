using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class AludyneBossFight : MonoBehaviour
{
    enum Phases
    {
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
    Coroutine pillarHeal;

    [Header("Zerrok Info")]
    [SerializeField] GameObject zerrokObject;
    [SerializeField] Ability zerrokFireAbility;
    [SerializeField] Ability zerrokFrostAbility;
    [SerializeField] Ability zerrokWindAbility;
    [SerializeField] Ability zerrokArcaneAbility;
    [SerializeField] float zerrokAbilityTimer;
    [SerializeField] float zerrokPillarTimer;
    [SerializeField] int zerrokBurningDevotionCount;

    [SerializeField] Unit zerrokFlameElemental;

    string currentAbilityPhase;
    bool abilityPhaseFinished;
    AludynePillarUnit zerrokCurrentPillar;

    Coroutine abilityCoroutine;
    int zerrokBurningDevotionCurrent = 0;
    

    [Header("Lesser Elemental Info")]
    [SerializeField] Unit lesserElemental;
    [SerializeField] Transform[] lesserElementalSpawnLocations;
    [SerializeField] int lesserElementalSpawnCount;
    [SerializeField] float lesserElementalSpawnTime;

    Coroutine elementalSpawn;

    [Header("Aludyne Info")]
    [SerializeField] Unit aludyneUnit;
    [SerializeField] Ability aludyneChaosVolley;
    [SerializeField] Ability aludyneGroundRupture;
    [SerializeField] Ability aludyneErruption;

    Phases currentPhase = Phases.START;
    Coroutine fightStarted;
    // Start is called before the first frame update
    void Start()
    {
        if (aludyneUnit != null)
        {
            aludyneUnit.ApplyStatus(StatusFlag.INVULNERABLE);
            //TODO - Add stun to unit and AI
            aludyneUnit.SetHealthCurrent(1.0f);
        }
        pillarList = new List<AludynePillarUnit>() { arcanePillar, firePillar, frostPillar, windPillar };
    }

    // Update is called once per frame
    void Update()
    {
        if (fightStarted != null) return;

        switch (currentPhase)
        {
            case Phases.START:
                Debug.Log("Fight Started");
                fightStarted = StartCoroutine(Fight_Start());
                break;
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

    private void SelectAbility()
    {
        int rand = Random.Range(0, 2);
        /*switch (rand)
        {
            case 0:
                currentAbilityPhase = "ZerrokArcanePhase";
                break;
            case 1:
                currentAbilityPhase = "ZerrokFirePhase";
                zerrokBurningDevotionCurrent = 0;
                break;
            case 2:
                currentAbilityPhase = "ZerrokFrostPhase";
                break;
            case 3:
                currentAbilityPhase = "ZerrokWindPhase";
                break;
            default:
                break;
        }*/
        switch(rand)
        {
            case 0:
                currentAbilityPhase = "ZerrokFirePhase";
                zerrokBurningDevotionCurrent = 0;
                break;
            case 1:
                currentAbilityPhase = "ZerrokWindPhase";
                break;
        }
    }

    private void ZerrokArcanePhase()
    {
        abilityPhaseFinished = true;
    }

    private void ZerrokFirePhase()
    {
        if (zerrokBurningDevotionCurrent == 0)
        {
            //Play audio cue
        }

        if (zerrokBurningDevotionCurrent >= zerrokBurningDevotionCount && abilityCoroutine == null)
        {
            abilityPhaseFinished = true;
            return;
        }

        //Start Burning Devotion IEnumerator
        if (abilityCoroutine == null)
        {
            abilityCoroutine = StartCoroutine(CastBurningDevotion());
        }
    }

    IEnumerator CastBurningDevotion()
    {
        zerrokBurningDevotionCurrent++;
        Instantiate(zerrokFireAbility);

        yield return new WaitForSeconds(zerrokFireAbility.Info().EffectDuration * 2);

        abilityCoroutine = null;
    }

    private void ZerrokFrostPhase()
    {
        abilityPhaseFinished = true;
    }

    private void ZerrokWindPhase()
    {
        //Player Phase Audio

        if (abilityCoroutine != null) return;

        int whirlingTempestCount = 1 + (4 - pillarList.Count);

        for (int i = 0; i < whirlingTempestCount; i++)
        {
            float randX = Random.Range(-1.0f, 1.0f) * 35.0f;
            float randZ = Random.Range(-1.0f, 1.0f) * 35.0f;

            Ability ability = Instantiate(zerrokWindAbility, transform.position, Quaternion.identity);
            ability.transform.position = new Vector3(randX, 0.5f, randZ);
            //ability.SetOwner();
        }

        abilityCoroutine = StartCoroutine(WindPhaseTimer());
    }

    IEnumerator WindPhaseTimer()
    {
        yield return new WaitForSeconds(zerrokWindAbility.Info().EffectDuration);
        abilityPhaseFinished = true;
        abilityCoroutine = null;
    }

    private void UpdatePhase1()
    {
        if (GetPillarsRemaining() <= 0)
        {
            StartCoroutine(Phase1_End());
            return;
        }

        //Update to unit getcurrentpercent
        if (aludyneUnit.GetHealthCurrent() <= aludyneUnit.GetHealthMax())
        {
            StartCoroutine(Phase1_EndBad());
            return;
        }

        pillarHeal ??= StartCoroutine(PillarHeal());
        elementalSpawn ??= StartCoroutine(ElementalSpawn());

        if (abilityPhaseFinished)
        {
            SelectAbility();
            abilityPhaseFinished = false;
        }

        Invoke(currentAbilityPhase, 0.0f);
    }

    IEnumerator Fight_Start()
    {
        //Do RP here
        currentPhase = Phases.START;

        yield return new WaitForSeconds(5.0f); //Set to RP audio timing

        currentPhase = Phases.PHASE_1;
        fightStarted = null;
        SelectAbility();
    }

    IEnumerator Phase1_End()
    {
        currentPhase = Phases.PHASE_1_END;

        //Do cinematic stuff here

        yield return new WaitForSeconds(15.0f);

        currentPhase = Phases.PHASE_2;
        aludyneUnit.RemoveStatus(StatusFlag.INVULNERABLE);
    }

    IEnumerator Phase1_EndBad()
    {
        currentPhase = Phases.PHASE_1_END;

        yield return new WaitForSeconds(15.0f);

        currentPhase = Phases.PHASE_2;
    }

    private void UpdatePhase2()
    {
        //Add get health current percent to unit
        if (aludyneUnit.GetHealthCurrent() <= 0.05)
        {
            aludyneUnit.ApplyStatus(StatusFlag.INVULNERABLE);
            aludyneUnit.ApplyStatus(StatusFlag.STUNNED);
            StartCoroutine(Phase2_End());
            return;
        }
    }

    IEnumerator Phase2_End()
    {
        currentPhase = Phases.END;

        yield return new WaitForSeconds(15.0f);

        //Game complete logic
    }

    private int GetPillarsRemaining()
    {
        return pillarList.Count;
    }

    IEnumerator PillarHeal()
    {
        foreach (AludynePillarUnit unit in pillarList)
        {
            if (unit == null) continue;

            //Do Heal
        }

        yield return new WaitForSeconds(pillarHealTimer);
        pillarHeal = null;
    }

    IEnumerator ElementalSpawn()
    {
        for (int i = 0; i < lesserElementalSpawnCount; i++)
        {
            int spawnLoc = Random.Range(0, lesserElementalSpawnLocations.Length);

            Instantiate(lesserElemental, lesserElementalSpawnLocations[spawnLoc].position, lesserElementalSpawnLocations[spawnLoc].rotation);
        }

        yield return new WaitForSeconds(lesserElementalSpawnTime);
        elementalSpawn = null;
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
