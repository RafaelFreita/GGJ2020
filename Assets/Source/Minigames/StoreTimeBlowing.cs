using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LUT.Events.Primitives;

public class StoreTimeBlowing : MonoBehaviour
{

    private float time;
    private bool isBlowing;
    
    private void Update()
    {
        if (isBlowing)
        {
            time += Time.deltaTime;
        }
    }

    public void OnBlowingStateChange(bool state)
    {
        // If not blowing and start blowing, restart the timer
        if(state && !isBlowing)
        {
            time = 0.0f;
        }
        isBlowing = state;
    }
}
