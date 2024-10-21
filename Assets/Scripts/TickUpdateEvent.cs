using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickUpdateEvent : MonoBehaviour
{
    public float tickTime = 0.2f;

    private float _tickerTime;

    public delegate void TickAction();
    public static event TickAction OnTickUpdate;

    private void Update()
    {
        _tickerTime += Time.deltaTime;
        
        if(_tickerTime >= tickTime)
        {
            _tickerTime = 0;
            TickEvent();
        }
    }

    private void TickEvent()
    {
        OnTickUpdate();
    }
}
