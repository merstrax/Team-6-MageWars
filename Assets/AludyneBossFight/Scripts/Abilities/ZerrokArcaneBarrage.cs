using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZerrokArcaneBarrage : Ability
{
    [SerializeField] Ability arcaneMissile;

    Coroutine abilityTimer;
    // Update is called once per frame
    protected override void Update()
    {
       abilityTimer ??= StartCoroutine(CastMissile());
    }

    IEnumerator CastMissile()
    {
        for(int i = 0; i < 5; i++)
        {
            float randX = Random.Range(10f, 35f) * (Random.Range(0, 2) * 2 - 1);
            float randZ = Random.Range(10f, 35f) * (Random.Range(0, 2) * 2 - 1);

            Vector3 location = new Vector3(randX, 20, randZ);
            Ability _missile = Instantiate(arcaneMissile, location, Quaternion.identity);

            randX = Random.Range(10f, 35f) * (Random.Range(0, 2) * 2 - 1);
            randZ = Random.Range(10f, 35f) * (Random.Range(0, 2) * 2 - 1);

            location = new Vector3(randX, 0, randZ);
            _missile.CastStart(owner, location);
        }

        yield return new WaitForSeconds(0.5f);

        abilityTimer = null;
    }
}
