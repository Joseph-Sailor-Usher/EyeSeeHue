using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;

public class SwapSprite : MonoBehaviour
{
    public UnityEngine.UI.Image myImage;
    public Sprite a, b;

    public UnityEvent onToggle;

    private void Start()
    {
        myImage = GetComponent<UnityEngine.UI.Image>();
        myImage.sprite = b;
    }

    public void SwapImage()
    {
        if(myImage.sprite == a)
        {
            myImage.sprite = b;
        }
        else
        {
            myImage.sprite = a;
        }
        onToggle.Invoke();
    }
}
