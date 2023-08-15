using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ObjectManager : MonoBehaviour
{
    public TextMeshProUGUI middleScreen;
    public GameManager gameManager;
    public int maxObjectGroups = 33;
    public List<ObjectGroupController> objectGroups;
    public GameObject objectGroupPrefab;
    public Vector3 poolPosition = new Vector3(0, 100, 0), objectStartPosition = new Vector3(0, -33, 0);
    public float seconds;


    public void MoveANewGroupIntoPlay(List<Vector3> positions, Color newColor)
    {
        //look for a group that isn't in game
        for(int i = 0; i < objectGroups.Count; i++)
        {
            //when we find one
            if(objectGroups[i].inGame == false)
            {
                //put it in game
                objectGroups[i].MoveToPlay(positions, newColor);
                //stop looking
                return;
            }
        }
    }

    public void Guess(GameObject guessedObject)
    {
        for(int i = 0; i < objectGroups.Count; i++)
        {
            if(objectGroups[i].transform.name == guessedObject.transform.name)
            {
                print("Resetting: " + i);
                objectGroups[i].RemoveFromPlay();
            }
        }
    }

    public void BombDetonate()
    {
        gameManager.UseBomb();
        StartCoroutine("BurstAllGroups");
    }

    IEnumerator BurstAllGroups()
    {
        gameManager.spawnRate = 1000;
        foreach(ObjectGroupController ogc in objectGroups)
        {
            if (ogc.inGame)
            {
                gameManager.soundManager.audioSource.PlayOneShot(gameManager.soundManager.correctGuessClip);
                ogc.bombed = true;
                ogc.StartCoroutine("Shrink");
                gameManager.score += 1000 * gameManager.activeCombo;
                Vector2 newPos = middleScreen.rectTransform.position;
                newPos.x += Random.Range(-20, 20);
                TextMeshProUGUI newScoreFeedback = Instantiate(gameManager.scoreTextFeedbackPrefab, newPos, Quaternion.identity, gameManager.mainCanvas.transform);
                newScoreFeedback.text = (gameManager.activeCombo).ToString() + ",000";
                gameManager.UpdateScoreText(gameManager.score, gameManager.scoreText);
                yield return new WaitForSeconds(0.3f);
            }
        }
        gameManager.missedOne = false;
    }

    IEnumerator WrongAllGroups()
    {
        foreach (ObjectGroupController ogc in objectGroups)
        {
            if (ogc.inGame)
            {
                ogc.Guess();
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    private void FixedUpdate()
    {
        seconds += 0.02f;
        int temp = 0;
        foreach(ObjectGroupController ogc in objectGroups)
        {
            if(ogc.inGame)
            {
                temp++;
            }
        }
        gameManager.activeGroups = temp;

        RaycastHit hit;

        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(Vector3.up * 1000, Vector3.down, out hit, 2000.0f))
        {
            if(hit.collider.transform.position.y >= 10)
            {
                Debug.Log("Out of bounds: " + hit.transform.name);
                ObjectGroupController tempogc = hit.transform.GetComponent<ObjectGroupController>();
                //StartCoroutine("WrongAllGroups");
                tempogc.RemoveFromPlay();
                if(gameManager.activeCombo > 10)
                    gameManager.activeCombo = (int)(gameManager.activeCombo / 2.0f);
                gameManager.missedOne = true;
                gameManager.soundManager.audioSource.PlayOneShot(gameManager.soundManager.audioClips[1]);
                gameManager.hapticManager.HeavyBoop();
            }
            if(seconds > 1)
            {
                seconds = 0;
                if (hit.collider.transform.position.y >= -20 && gameManager.activeCombo >= 3)
                    gameManager.activeCombo -= 2;
            }


        }

        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(10 * objectStartPosition, Vector3.up, out hit))
        {
            if (hit.collider.transform.position.y < objectStartPosition.y)
            {
                Rigidbody tempRB = hit.transform.GetComponent<Rigidbody>();
                tempRB.velocity = Vector3.up * 10.0f;
            }
        }
    }
}

    /*
    private void OnValidate()
    {
        if(objectGroups.Count == 0)
        {
            while(objectGroups.Count < maxObjectGroups)
            {
                GameObject temp = Instantiate(objectGroupPrefab, poolPosition, Quaternion.identity, this.transform);
                temp.transform.name = objectGroups.Count.ToString();
                objectGroups.Add(temp.GetComponent<ObjectGroupController>());
            }
        }
    }
    */
