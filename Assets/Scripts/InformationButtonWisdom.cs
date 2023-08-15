using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class InformationButtonWisdom : MonoBehaviour
{
    public List<string> wisdoms;
    int index = 0;
    public Button myButton;

    public PostMessageAndFade promptText;

    void Start()
    {
        promptText = GameObject.Find("PromptText").GetComponent<PostMessageAndFade>();
    }

    public void ShowNextMessage()
    {
        //If a message is not showing
        if(promptText.showingMessages == false)
        {
            //queue this one up
            promptText.QueueUpMessage(wisdoms[index]);
        }
        else //if a message is showing
        {
            promptText.messageQueue.Add(wisdoms[index]);
        }
        StartCoroutine("CountDown");
        //Increment the index
        if(index >= wisdoms.Count - 1)
        {
            index = 0;
        }
        else
        {
            index++;
        }
    }

    public IEnumerator CountDown()
    {
        myButton.interactable = false;
        if (promptText.showingMessages == true)
        {
            yield return new WaitForSeconds(0.1f);
            StartCoroutine("CountDown");
        }
        else
        {
            myButton.interactable = true;
        }
    }

    public string PullRandom()
    {
        return wisdoms[Random.Range(0, wisdoms.Count)];
    }
}
