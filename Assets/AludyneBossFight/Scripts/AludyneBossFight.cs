using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AludyneBossFight : MonoBehaviour
{
    enum Phases
    {
        START, PHASE_1, PHASE_1_END, PHASE_2_START, PHASE_2, END
    }

    [Header("Elemental Pillars")]
    [SerializeField] AludynePillarUnit frostPillar;
    [SerializeField] AludynePillarUnit firePillar;
    [SerializeField] AludynePillarUnit arcanePillar;
    [SerializeField] AludynePillarUnit windPillar;
    [SerializeField] float pillarHealAmount;
    [SerializeField] float pillarHealTimer;

    List<AludynePillarUnit> pillarList;
    Coroutine pillarHeal;

    [Header("Zerrok Info")]
    [SerializeField] Unit zerrokUnit;
    [SerializeField] Ability zerrokFireAbility;
    [SerializeField] Ability zerrokFrostAbility;
    [SerializeField] Ability zerrokWindAbility;
    [SerializeField] Ability zerrokArcaneAbility;
    [SerializeField] float zerrokAbilityTimer;
    [SerializeField] float zerrokPillarTimer;
    [SerializeField] int zerrokBurningDevotionCount;

    [SerializeField] Unit zerrokFlameElemental;

    string currentAbilityPhase = "";
    bool abilityPhaseFinished;
    AludynePillarUnit zerrokCurrentPillar;

    Coroutine pillarSelect;
    Coroutine abilityCoroutine;
    int zerrokBurningDevotionCurrent = 0;

    [Header("Lesser Elemental Info")]
    [SerializeField] Unit lesserElemental;
    [SerializeField] Transform[] lesserElementalSpawnLocations;
    [SerializeField] int lesserElementalSpawnCount;
    [SerializeField] float lesserElementalSpawnTime;

    Coroutine elementalSpawn;

    [Header("Aludyne Info")]
    [SerializeField] AludyneUnitAI aludyneUnit;
    [SerializeField] Ability aludyneChaosVolley;
    [SerializeField] Ability aludyneGroundRupture;
    [SerializeField] Ability aludyneErruption;

    [Header("Audio and Effects")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] audioClips;

    Phases currentPhase = Phases.START;
    Coroutine roleplayCoroutine;


    void Start()
    {
        if (zerrokUnit != null)
        {
            zerrokUnit.ApplyStatus(StatusFlag.INVULNERABLE);
        }

        pillarList = new List<AludynePillarUnit>() { arcanePillar, firePillar, frostPillar, windPillar };
        foreach (AludynePillarUnit pillar in pillarList)
        {
            pillar.ApplyStatus(StatusFlag.INVULNERABLE);
        }
    }

    void Update()
    {
        if (roleplayCoroutine != null) return;

        switch (currentPhase)
        {
            case Phases.START:
                roleplayCoroutine = StartCoroutine(Fight_Start());
                break;
            case Phases.PHASE_1:
                UpdatePhase1();
                break;
            case Phases.PHASE_1_END:
                roleplayCoroutine = StartCoroutine(Phase1_End());
                break;
            case Phases.PHASE_2_START:
                roleplayCoroutine = StartCoroutine(Phase2_Start());
                break;
            case Phases.PHASE_2:
                UpdatePhase2();
                break;
            case Phases.END:
                roleplayCoroutine = StartCoroutine(Phase2_End());
                break;
            default:
                break;
        }
    }

    IEnumerator Fight_Start()
    {
        audioSource.Stop();
        audioSource.clip = audioClips[0];
        audioSource.Play();

        aludyneUnit.SetSleeping();

        yield return new WaitForSeconds(audioClips[0].length + 1.0f);
        //yield return new WaitForSeconds(3.0f);

        currentPhase = Phases.PHASE_1;
        roleplayCoroutine = null;
        StartCoroutine(Phase1_Start());
    }

    #region Phase 1
    //Phase1
    private void UpdatePhase1()
    {
        if (pillarList.Count <= 0)
        {
            currentPhase = Phases.PHASE_1_END;
            return;
        }

        //Update to unit getcurrentpercent
        if (aludyneUnit.GetHealthCurrent() >= aludyneUnit.GetHealthMax())
        {
            currentPhase = Phases.PHASE_1_END;
            return;
        }

        pillarHeal ??= StartCoroutine(PillarHeal());
        elementalSpawn ??= StartCoroutine(ElementalSpawn());

        if (pillarSelect == null)
        {
            SelectPillar();
        }

        if (abilityPhaseFinished)
        {
            SelectAbility();
            abilityPhaseFinished = false;
        }

        if (currentAbilityPhase != "")
            Invoke(currentAbilityPhase, 0.0f);
    }

    IEnumerator Phase1_Start()
    {
        audioSource.Stop();
        audioSource.clip = audioClips[1];
        audioSource.Play();

        foreach (AludynePillarUnit pillar in pillarList)
        {
            pillar.RemoveStatus(StatusFlag.INVULNERABLE);
        }

        yield return new WaitForSeconds(audioClips[1].length);

        SelectAbility();
    }

    IEnumerator Phase1_End()
    {
        audioSource.Stop();
        audioSource.clip = audioClips[6];
        audioSource.Play();

        yield return new WaitForSeconds(audioClips[6].length);

        Destroy(zerrokUnit.gameObject);
        aludyneUnit.HealPercent(new Damage(0.15f));
        roleplayCoroutine = null;
        currentPhase = Phases.PHASE_2_START;
    }

    IEnumerator Phase1_EndBad()
    {
        currentPhase = Phases.PHASE_1_END;

        yield return new WaitForSeconds(15.0f);

        currentPhase = Phases.PHASE_2;
    }

    #region Zerrok Ability Handling
    private void SelectAbility()
    {
        if (pillarList.Count <= 1)
        {
            zerrokBurningDevotionCurrent = 0;
            currentAbilityPhase = pillarList[0].PillarAbility();
            return;
        }

        int rand = Random.Range(0, pillarList.Count);
        string oldAbility = currentAbilityPhase;
        currentAbilityPhase = pillarList[rand].PillarAbility();

        if (oldAbility == currentAbilityPhase)
            SelectAbility();

        if (currentAbilityPhase == "ZerrokFirePhase")
        {
            zerrokBurningDevotionCurrent = 0;
        }
    }

    private void ZerrokArcanePhase()
    {
        if (abilityCoroutine != null) return;
        audioSource.Stop();
        audioSource.clip = audioClips[3];
        audioSource.Play();

        Ability _arcaneBarrage = Instantiate(zerrokArcaneAbility);
        _arcaneBarrage.SetOwner(zerrokUnit);

        Destroy(_arcaneBarrage.gameObject, _arcaneBarrage.Info().EffectDuration);
        abilityCoroutine = StartCoroutine(ArcaneWaveTimer());
    }

    IEnumerator ArcaneWaveTimer()
    {
        yield return new WaitForSeconds(zerrokArcaneAbility.Info().EffectDuration);
        abilityPhaseFinished = true;
        abilityCoroutine = null;
    }

    private void ZerrokFirePhase()
    {
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
        if (zerrokBurningDevotionCurrent == 0)
        {
            audioSource.Stop();
            audioSource.clip = audioClips[2];
            audioSource.Play();
        }

        zerrokBurningDevotionCurrent++;
        Instantiate(zerrokFireAbility);

        yield return new WaitForSeconds(zerrokFireAbility.Info().EffectDuration * 2);

        abilityCoroutine = null;
    }

    private void ZerrokFrostPhase()
    {
        if (abilityCoroutine != null) return;
        audioSource.Stop();
        audioSource.clip = audioClips[4];
        audioSource.Play();

        Ability frost;

        foreach (EnemyAI enemyAI in FindObjectsOfType<EnemyAI>(false))
        {
            if (enemyAI == aludyneUnit || enemyAI == null) continue;

            frost = Instantiate(zerrokFrostAbility, enemyAI.transform.position, Quaternion.identity);
            frost.CastStart(null, enemyAI.transform.position);
            frost.Cast();
        }

        abilityCoroutine = StartCoroutine(FrostWaveTimer());
    }

    IEnumerator FrostWaveTimer()
    {
        yield return new WaitForSeconds(audioClips[4].length + zerrokAbilityTimer);
        abilityPhaseFinished = true;
        abilityCoroutine = null;
    }

    private void ZerrokWindPhase()
    {
        //Player Phase Audio

        if (abilityCoroutine != null) return;

        audioSource.Stop();
        audioSource.clip = audioClips[5];
        audioSource.Play();

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
    #endregion

    #region Pillar Handling
    private void SelectPillar()
    {
        if (pillarSelect != null) return;

        if (pillarList.Count <= 1)
        {
            zerrokCurrentPillar = pillarList[0];
            zerrokUnit.transform.position = zerrokCurrentPillar.transform.position + (Vector3.up * 5f);

            zerrokCurrentPillar.RemoveStatus(StatusFlag.INVULNERABLE);
            pillarSelect = StartCoroutine(PillarSelectTimer());
            return;
        }

        int rand = Random.Range(0, pillarList.Count);
        AludynePillarUnit oldPillar = zerrokCurrentPillar;
        zerrokCurrentPillar = pillarList[rand];

        if (oldPillar == zerrokCurrentPillar)
        {
            SelectPillar();
            return;
        }

        if (oldPillar != null)
            oldPillar.RemoveStatus(StatusFlag.INVULNERABLE);

        zerrokCurrentPillar.ApplyStatus(StatusFlag.INVULNERABLE);

        zerrokUnit.transform.position = zerrokCurrentPillar.transform.position + (Vector3.up * 5f);
        pillarSelect = StartCoroutine(PillarSelectTimer());
    }

    IEnumerator PillarSelectTimer()
    {
        yield return new WaitForSeconds(zerrokPillarTimer);
        pillarSelect = null;
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

            aludyneUnit.HealPercent(new Damage(pillarHealAmount));
        }

        yield return new WaitForSeconds(pillarHealTimer);
        pillarHeal = null;
    }

    public void OnDeath(AludynePillarUnit unit)
    {
        pillarList.Remove(unit);

        if (zerrokCurrentPillar == unit)
        {
            //Do Zerrok pillar swap}
        }
    }
    #endregion
    #endregion

    #region Phase 2
    private void UpdatePhase2()
    {
        //Add get health current percent to unit
        if (aludyneUnit.GetHealthPercent() <= 0.05)
        {
            currentPhase = Phases.END;
            aludyneUnit.OnDeath();
            return;
        }
    }

    IEnumerator Phase2_Start()
    {
        audioSource.Stop();
        audioSource.clip = audioClips[7];
        audioSource.Play();

        aludyneUnit.preAwaken();

        yield return new WaitForSeconds(audioClips[7].length + 1.0f);

        aludyneUnit.Awaken();
        
        currentPhase = Phases.PHASE_2;
        roleplayCoroutine = null;
    }

    IEnumerator Phase2_End()
    {
        audioSource.Stop();
        audioSource.clip = audioClips[10];
        audioSource.Play();

        yield return new WaitForSeconds(15.0f);

        //Game complete logic
        aludyneUnit.DoDeathAnimation();
    }
    #endregion

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
}
