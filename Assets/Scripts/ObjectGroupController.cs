using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGroupController : MonoBehaviour
{
    public GameManager gameManager;

    //Manager variables
    public bool inGame = false, growing = false, shrinking = false, bombed = false, missed = false;
    public int id = -1;
    public GameObject objectToSpawn;
    public int maxObjectsInGroup = 13;
    public List<GameObject> objectsInGroup;
    public BoxCollider boxCollider;

    //State variables
    public Color mainColor, deviantColor;
    public float colorVariance = 0.5f;
    public ParticleSystem explosionPrefab;
    public int tapCount = 0;

    //Movement variables
    public Rigidbody rigidBody;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        rigidBody = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        for (int i = 0; i < objectsInGroup.Count; i++)
        {
            objectsInGroup[i].transform.name = transform.name;
        }
    }

  
    public void MoveToPlay(List<Vector3> objectPositions, Color newColor)
    {
        inGame = true;
        boxCollider.isTrigger = false;
        bombed = false;
        missed = false;
        growing = false;
        shrinking = false;
        tapCount = 0;
        transform.position = gameManager.objectManager.objectStartPosition;
        mainColor = newColor;
        deviantColor = gameManager.randomColorGenerator.DeviateColor(mainColor);
        int correctGuess = Random.Range(0, objectPositions.Count);
        for (int i = 0; i < objectPositions.Count; i++)
        {
            objectsInGroup[i].transform.position = objectPositions[i] + transform.position;
            objectsInGroup[i].transform.GetComponent<MeshRenderer>().material.color = ((i == correctGuess) ? deviantColor : mainColor);
            objectsInGroup[i].transform.tag = ((i == correctGuess) ? "Correct" : "") + "Object";
            objectsInGroup[i].SetActive(true);
        }
        rigidBody.velocity = Vector3.up * gameManager.currentSpeedOfShapes;
        rigidBody.angularVelocity = Vector3.up * (gameManager.rotationSpeedOfShapes * Mathf.PI/6);
        for (int i = 0; objectsInGroup[i].activeSelf == true && i < objectsInGroup.Count; i++)
        {
            objectsInGroup[i].transform.localScale = Vector3.zero;
        }
        StartCoroutine("Grow");
    }


    public void Guess(GameObject guessedObject, Vector2 pos)
    {
        //Play sound
        float newPitch = ((guessedObject.transform.position.y + 44.0f) / 44.0f) * 2 + 1;
        gameManager.soundManager.SetPitch(newPitch);

        //If correct
        if (guessedObject.GetComponent<MeshRenderer>().material.color == deviantColor)
        {
            //Tell Game Manager
            gameManager.Guess(true, guessedObject.transform.position.y, pos, tapCount);
            //Play sound
            gameManager.soundManager.audioSource.PlayOneShot(gameManager.soundManager.correctGuessClip);
            //If this is a double tap, update the time of last double tap
            if (shrinking)
            {
                gameManager.secondsSinceLastDoubleTap = 0;
            }
            //play haptics
            gameManager.hapticManager.SoftBoop();
        }
        else
        {
            //Play sound
            gameManager.soundManager.audioSource.PlayOneShot(gameManager.soundManager.wrongGuessClip);
            //play haptics
            gameManager.hapticManager.RigidBoop();
            gameManager.Guess(false, guessedObject.transform.position.y, pos, tapCount);
        }
        //Create the particle effects
        for (int i = 0; objectsInGroup[i].activeSelf == true && i < objectsInGroup.Count; i++)
        {
            ParticleSystem esplosion = Instantiate(explosionPrefab, objectsInGroup[i].transform.position, objectsInGroup[i].transform.rotation);
            esplosion.startColor = mainColor;
        }

        tapCount++;
        //Start shrinking
        StartCoroutine("Shrink");
    }


    public void Guess()
    {
        //Play sound
        float newPitch = ((this.gameManager.transform.position.y + 44.0f) / 44.0f) * 2 + 1;
        gameManager.soundManager.SetPitch(newPitch);
        gameManager.soundManager.audioSource.PlayOneShot(gameManager.soundManager.wrongGuessClip);

        //play haptics
        gameManager.hapticManager.RigidBoop();

        //Create the particle effects
        for (int i = 0; objectsInGroup[i].activeSelf == true && i < objectsInGroup.Count; i++)
        {
            ParticleSystem esplosion = Instantiate(explosionPrefab, objectsInGroup[i].transform.position, objectsInGroup[i].transform.rotation);
            esplosion.startColor = mainColor;
        }

        tapCount++;

        //Start shrinking
        StartCoroutine("Shrink");
    }


    public IEnumerator Shrink()
    {
        yield return new WaitForSeconds(0.01f);

        growing = false;

        //Get smaller
        for (int i = 0; objectsInGroup[i].activeSelf == true && i < objectsInGroup.Count; i++)
            objectsInGroup[i].transform.localScale -= Vector3.one * 0.1f;

        //Keep going if we are bigger than 0
        if(objectsInGroup[0].transform.localScale.x > 0)
        {
            shrinking = true;
            StartCoroutine("Shrink");
        }
        else //If we are of size 0
        {
            shrinking = false;
            //Play haptics
            if(bombed)
            {
                gameManager.hapticManager.SoftBoop();
            }

            //Remove this group from play
            RemoveFromPlay();
        }
    }
    //Called by Shrink() after it finishes the destroy animations and particle effects
    public void RemoveFromPlay()
    {
        inGame = false;
        boxCollider.isTrigger = true;
        if(gameManager.activeGroups > 0)
            gameManager.activeGroups--;
        for (int i = 0; i < objectsInGroup.Count; i++)
        {
            objectsInGroup[i].transform.position = transform.position;
            objectsInGroup[i].SetActive(false);
        }
        rigidBody.velocity = Vector3.zero;
        transform.position = gameManager.objectManager.poolPosition;
    }

    public IEnumerator Grow()
    {
        if (shrinking == false)
        {
            for (int i = 0; objectsInGroup[i].activeSelf == true && i < objectsInGroup.Count; i++)
            {
                objectsInGroup[i].transform.localScale += Vector3.one * 0.1f;
            }
            yield return new WaitForSeconds(0.01f);
            if (objectsInGroup[0].transform.localScale.x <= 2)
            {
                growing = true;
                StartCoroutine("Grow");
            }
            else
            {
                growing = false;
            }
        }
    }
}

/*
    //Clear out children if there are any
        while(transform.childCount > 0)
        {
            Destroy(transform.GetChild(0));
        }
        public void OnValidate()
        {
            if(objectsInGroup.Count == 0)
            {
                for (int i = 0; i < maxObjectsInGroup; i++)
                {
                    GameObject temp = Instantiate(objectToSpawn, transform.position, Quaternion.identity, this.transform);
                    objectsInGroup.Add(temp);
                }
            }
        }
*/