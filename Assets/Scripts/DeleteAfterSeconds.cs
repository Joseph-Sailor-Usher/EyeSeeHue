using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteAfterSeconds : MonoBehaviour
{
    public int secondsAlive = 2;
    private float fiftiethsOfSecondsAlive = 0;

    void FixedUpdate()
    {
        fiftiethsOfSecondsAlive++;
        if(fiftiethsOfSecondsAlive == secondsAlive * 50)
        {
            Destroy(this.gameObject);
        }
    }
}
