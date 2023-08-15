using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class DelayedStart : MonoBehaviour
{
    public InformationButtonWisdom textArchive;
    public Button myButton;
    public UnityEvent functions;

    public PostMessageAndFade promptText;

    void Start()
    {
        promptText = GameObject.Find("PromptText").GetComponent<PostMessageAndFade>();
    }

    public void ShowNextMessage()
    {
        if (promptText.showingMessages == false)
        {
            promptText.QueueUpMessage(textArchive.PullRandom());
        }
        else
        {
            promptText.messageQueue.Add(textArchive.PullRandom());
        }
        StartCoroutine("CountDown");
    }

    public IEnumerator CountDown()
    {
        myButton.interactable = false;
        yield return new WaitForSeconds(3.0f);
        myButton.interactable = true;
        functions.Invoke();
    }
}
