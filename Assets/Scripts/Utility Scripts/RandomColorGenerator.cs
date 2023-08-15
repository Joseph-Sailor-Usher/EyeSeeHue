using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RandomColorGenerator : MonoBehaviour
{
    public float minHue = 0.0f, maxHue = 1.0f, minSaturation = 0.9f, maxSaturation = 1.0f, minVal = 0.5f, maxVal = 0.98f;
    public Color GenerateColor()
    {
        Color newColorLo = new Color();
        Color newColorHi = new Color();
        newColorLo = Random.ColorHSV(0.0f, 0.60f, minSaturation, maxSaturation, minVal, maxVal);
        newColorHi = Random.ColorHSV(0.85f, 1.0f, minSaturation, maxSaturation, minVal, maxVal);
        return (Random.Range(0, 10) < 7.0f ? newColorLo : newColorHi);
    }
    public float variance;
    public Color DeviateColor(Color originalColor)
    {
        float H, S, V;
        Color.RGBToHSV(originalColor, out H, out S, out V);
        //V += ((Random.Range(0, 10) > 3) ? variance : -variance);
        V += variance;
        originalColor = Color.HSVToRGB(H, S, V);
        return originalColor;
    }
}
