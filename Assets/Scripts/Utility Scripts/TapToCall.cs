using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class TapToCall : MonoBehaviour
{
    public UnityEvent functionToInvokeOnTap;

    void Update()
    {
        foreach(Touch touch in Input.touches)
        {
            if(touch.phase == TouchPhase.Began)
            {
                functionToInvokeOnTap.Invoke();
            }
        }
    }
}
