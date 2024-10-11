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

        //outerRing.transform.localScale = Vector3.Lerp(outerRing.transform.localScale, currentScale * originalScale, Time.deltaTime);
    }

    public void SetScale(float scale)
    {
        outerRing.transform.localScale = scale * Vector3.one;
        originalScale = outerRing.transform.localScale;
        //originalScaleValue = scale;
    }
}
