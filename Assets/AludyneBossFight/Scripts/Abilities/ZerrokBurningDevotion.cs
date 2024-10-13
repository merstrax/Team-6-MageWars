using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZerrokBurningDevotion : Ability
{
    [SerializeField] Ability flameWave;
    [SerializeField] int flameWaveCount;
    [SerializeField] float finalScale;

    List<Ability> flameWaves;

    protected override void Start()
    {
        base.Start();
        flameWaves = new List<Ability>();
        float rot = 60;

        for(int i = 0; i < flameWaveCount; i++)
        {
            flameWaves.Add(Instantiate(flameWave, transform));
            flameWaves[i].transform.rotation = Quaternion.Euler(0, rot * i, 0);
        }
    }

    protected override void Update()
    {
        float elapsed = (Time.time - EffectTimeApplied) / Info().EffectDuration;
        float scale = ((finalScale - 1) * elapsed) + 1;

        if (elapsed >= 1)
            Destroy(transform.gameObject);

        foreach (Ability ability in flameWaves)
        {
            ability.transform.localScale = new Vector3(scale, scale, scale);
        }

        
    }
}
