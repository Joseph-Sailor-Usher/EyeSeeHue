using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class PauseButtonController : MonoBehaviour
{
    public List<Button> toggles;
    public Button playMusic, muteMusic;
    public bool hideMenu = true;

    public void toggleToggles()
    {
        hideMenu = !hideMenu;
        foreach(Button g in toggles)
        {
            g.gameObject.SetActive(!g.gameObject.activeSelf);
        }
        if(hideMenu)
        {
            playMusic.gameObject.SetActive(false);
            muteMusic.gameObject.SetActive(false);
        }
        else
        {
            if (PlayerPrefs.GetInt("PlayEffects") == 1)
            {
                playMusic.gameObject.SetActive(true);
                muteMusic.gameObject.SetActive(false);
            }
            else
            {
                playMusic.gameObject.SetActive(false);
                muteMusic.gameObject.SetActive(true);
            }
        }
    }
}
