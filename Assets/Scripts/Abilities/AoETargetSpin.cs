using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoETargetSpin : MonoBehaviour
{
    [SerializeField] GameObject outerRing;
    [SerializeField] GameObject innerRing;

    [SerializeField] float outerSpinSpeed;
    [SerializeField] float innerSpinSpeed;


    [SerializeField] float scaleDelta;

    Vector3 originalScale = Vector3.one;

    public float currentScale;
    public float targetScale;
    bool shrink;

    [SerializeField] SphereCollider myCollider;

    // Update is called once per frame
    void Update()
    {
        outerRing.transform.Rotate(outerSpinSpeed * Time.deltaTime * Vector3.forward);
        innerRing.transform.Rotate(innerSpinSpeed * Time.deltaTime * Vector3.forward);

        if (shrink)
        {
            outerRing.transform.localScale = Vector3.Lerp(outerRing.transform.localScale, (1 - scaleDelta) * originalScale, Time.deltaTime * 2f);
            currentScale = outerRing.transform.localScale.x;
            targetScale = ((1 - scaleDelta) * originalScale).x;
            shrink = !(currentScale <= targetScale + 1);
        }
        else
        {
            outerRing.transform.localScale = Vector3.Lerp(outerRing.transform.localScale, (1 + scaleDelta) * originalScale, Time.deltaTime * 2f);
            currentScale = outerRing.transform.localScale.x;
            targetScale = ((1 + scaleDelta) * originalScale).x;
            shrink = currentScale >= targetScale - 1;
        }
    }

    public void SetScale(float scale)
    {
        outerRing.transform.localScale = (scale/2) * Vector3.one;
        originalScale = outerRing.transform.localScale;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        if (other.gameObject.TryGetComponent<ITargetable>(out var hit))
        {
            if(hit.GameObject().CompareTag("Enemy") || hit.GameObject().CompareTag("AludyneBossFight"))
                hit.OnTarget(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<ITargetable>(out var hit))
        {
            hit.OnTarget(false);
        }
    }
}
