using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextFallAndDelete : MonoBehaviour
{
    public RectTransform myRect;
    public float speed = 3.0f;

    private void Start()
    {
        myRect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        myRect.anchoredPosition += Vector2.up * speed * Time.deltaTime;
        if(myRect.anchoredPosition.x < 0)
        {
            myRect.anchoredPosition += Vector2.left * speed * Time.deltaTime;
        }
        else
        {
            myRect.anchoredPosition += Vector2.right * speed * Time.deltaTime / 4.0f;
        }
        if (myRect.anchoredPosition.x < -Screen.width || myRect.anchoredPosition.x > Screen.width)
        {
            Destroy(this.gameObject);
        }
    }
}
