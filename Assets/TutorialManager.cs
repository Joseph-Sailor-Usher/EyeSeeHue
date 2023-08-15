using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorial;

    public void Show()
    {
        tutorial.SetActive(true);
    }
    public void Hide()
    {
        tutorial.SetActive(false);
    }
}
