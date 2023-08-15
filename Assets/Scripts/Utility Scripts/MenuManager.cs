using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameManager gameManager;
    public List<GameObject> titleMenu, titleMenuAdFree, gameMenu, settingsMenu;

    private void Start()
    {
        ChangeMenuTo("Title");
    }

    void Awake()
    {
        if(gameManager == null)
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if(titleMenu.Count == 0)
            titleMenu.Add(GameObject.Find("TitleMenu"));
        if(gameMenu.Count == 0)
            gameMenu.Add(GameObject.Find("GameMenu"));
        if(settingsMenu.Count == 0)
            settingsMenu.Add(GameObject.Find("SettingsMenu"));
    }

    public void ChangeMenuTo(string newActiveMenu)
    {
        //Hide all the others
        hideAll();
        //show the new one
        switch(newActiveMenu)
        {
            case("Title"):
            {
                for (int i = 0; i < titleMenu.Count; i++)
                {
                    titleMenu[i].SetActive(true);
                }
                if (PlayerPrefs.HasKey("AdsDisabled"))
                {
                    if(PlayerPrefs.GetInt("AdsDisabled") == 1)
                    {
                        for (int i = 0; i < titleMenuAdFree.Count; i++)
                        {
                            titleMenuAdFree[i].SetActive(true);
                        }
                    }
                }
                break;
            }
            case("Game"):
            {
                for (int i = 0; i < gameMenu.Count; i++)
                {
                    gameMenu[i].SetActive(true);
                }
                break;
            }
            case("Settings"):
            {
                for (int i = 0; i < settingsMenu.Count; i++)
                {
                    settingsMenu[i].SetActive(true);
                }
                break;
            }
            default:
            {
                print("Error switching to menu: " + newActiveMenu);
                break;
            }
        }
    }
    private void hideAll()
    {
        for(int i = 0; i < titleMenu.Count; i++)
            titleMenu[i].SetActive(false);
        for (int i = 0; i < titleMenuAdFree.Count; i++)
            titleMenuAdFree[i].SetActive(false);
        for (int i = 0; i < gameMenu.Count; i++)
            gameMenu[i].SetActive(false);
        for (int i = 0; i < settingsMenu.Count; i++)
            settingsMenu[i].SetActive(false);
    }
    
}
