using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MakeTextBlink : MonoBehaviour
{
    public float SecondsPerCycle = 1;
    public TextMeshProUGUI text;
    private bool dimming = true;

    private void Start()
    {
        if (text == null)
        {
            text = GetComponent<TextMeshProUGUI>();
        }
        SecondsPerCycle *= 60;
    }

    void FixedUpdate()
    {
        if (dimming)
        {
            text.alpha -= 1.0f / SecondsPerCycle;
            if (text.alpha <= 0)
            {
                dimming = false;
            }
        }
        else
        {
            text.alpha += 1.0f / SecondsPerCycle;
            if (text.alpha >= 1)
            {
                dimming = true;
            }
        }
    }
}

