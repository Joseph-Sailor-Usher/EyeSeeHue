using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PostMessageAndFade : MonoBehaviour
{
    public List<string> messageQueue;
    public bool showingMessages = false;
    public float holdTime = 1.0f, fadeTime = 1.0f;
    public float currentHoldTime = 1.0f, currentFadeTime = 1.0f;
    public TextMeshProUGUI displayText;

    private void Start()
    {
        displayText = GetComponent<TextMeshProUGUI>();
    }

    public void QueueUpMessage(string newMessage)
    {
        messageQueue.Add(newMessage);
        if(showingMessages == false)
        {
            StartCoroutine("FadeMessage");
        }
    }

    public IEnumerator FadeMessage()
    {
        //If this is the first message
        if(!showingMessages)
        {
            showingMessages = true;
            if(displayText.text.Length < 1)
                displayText.text = messageQueue[0];
        }

        //if holding
        if(currentHoldTime > 0)
        {
            currentHoldTime -= 0.05f;
        } //if fading
        else if(currentFadeTime > 0)
        {
            currentFadeTime -= 0.05f;
            displayText.alpha -= 0.05f;
        } //else reset
        else
        {
            messageQueue.RemoveAt(0);
            currentFadeTime = fadeTime;
            currentHoldTime = holdTime;
            displayText.alpha = 1;
            displayText.text = "";
            //Try to load another message
            if(messageQueue.Count > 0)
            {
                showingMessages = true;
                displayText.text = messageQueue[0];
            }
            else
            {
                showingMessages = false;
            }
        }

        if (messageQueue.Count > 0)
        {
            yield return new WaitForSeconds(0.05f);
            StartCoroutine("FadeMessage");
        }
        else
        {
            showingMessages = false;
        }
    }
}
