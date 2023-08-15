using System.Collections.Generic;
using UnityEngine.Advertisements;
using UnityEngine.Purchasing;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class DisableAds : MonoBehaviour
{
    public List<GameObject> buttonsWithAds, buttonsWithoutAds;

    public void ShowAdButtons()
    {
        foreach (GameObject go in buttonsWithAds)
            go.SetActive(true);
        foreach (GameObject go in buttonsWithoutAds)
            go.SetActive(false);
    }

    public void HideAdButtons()
    {
        SetButtonsActiveStatus(buttonsWithoutAds, true);
        MoveButtonsActiveStatus(buttonsWithAds);
    }

    private void SetButtonsActiveStatus(List<GameObject> buttons, bool newStatus)
    {
        foreach(GameObject go in buttons)
            go.SetActive(newStatus);
    }
    private void MoveButtonsActiveStatus(List<GameObject> buttons)
    {
        foreach (GameObject go in buttons)
            go.transform.position = Vector3.down * 1000000;
    }
}
