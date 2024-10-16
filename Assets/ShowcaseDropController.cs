using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowcaseDropController : MonoBehaviour
{
    [SerializeField] GameObject damage;
    [SerializeField] GameObject heal;
    [SerializeField] GameObject health;
    [SerializeField] GameObject speed;

    Coroutine respawnDrops;

    // Update is called once per frame
    void Update()
    {
        if (respawnDrops == null)
        {
            Instantiate(damage, transform.position + new Vector3(-7.5f, 0f, -5f), Quaternion.identity);
            Instantiate(health, transform.position + new Vector3(2.5f, 0f, -5f), Quaternion.identity);
            Instantiate(speed, transform.position + new Vector3(7.5f, 0f, -5f), Quaternion.identity);
            Instantiate(heal, transform.position + new Vector3(-2.5f, 0f, -5f), Quaternion.identity);

            respawnDrops = StartCoroutine(Respawn());
        }
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(10.0f);
        respawnDrops = null;
    }
}
