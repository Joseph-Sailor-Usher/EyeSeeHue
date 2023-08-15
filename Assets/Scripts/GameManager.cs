using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEditor;
using Lofelt.NiceVibrations;

public class GameManager : MonoBehaviour
{
//VARIABLES
    //Managers and generators
    public ObjectManager objectManager;
    public SoundManager soundManager;
    public MenuManager menuManager;
    public RandomColorGenerator randomColorGenerator;
    public ShapeGenerator shapeGenerator;
    public AdManager adManager;
    public HapticManager hapticManager;

    //Aesthetics
    public ConstantlyRotate petalRotator;
    public ParticleSystem confettiBois;
    public TextMeshProUGUI scoreTextFeedbackPrefab;


    //References to objects
    public Canvas mainCanvas;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI lastScoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI timeText;
    public PostMessageAndFade promptDisplay;
    public PostMessageAndFade promptDisplayUpper;
    public Button showAdForTime, stop, pressForMoreTime;
    public Button redeemButton;


    //BOMB
    public GameObject bombPrefab, activeBomb;
    public Vector3 bombSpawnPos;
    public bool bombActive = false;

    //Standard variables
    public bool playingGame = false;
    private float fiftiethsOfASecondFromStart = 0;
    public int activeGroups = 0;
    public int minutesRemaining, secondsRemaining;
    public int level = 0, prevLevel = 0;
    public bool missedOne = false;

    //Tutorial variables
    public int secondsSinceLastDoubleTap = 0;

    //Big Gametype modifications
    public bool infinite = false;
    public bool timeExtensions = false;
    public float[] radii = { 3.0f, 4.0f, 5.0f, 5.5f, 6.0f, 6.5f, 7.0f, 7.0f, 7.5f, 8.0f };
    public float[] rotationSpeeds = { 0.6f, 0.3f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f };
    public int[] maxActiveGroupsLevels = { 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7 };
    public int[] numberOfPointsInShapeLevels = { 3, 3, 4, 5, 5, 6, 3, 4, 5, 5, 6, 7 };
    public float[] varianceByLevel = { 0.6f, 0.55f, 0.5f, 0.5f, 0.5f, 0.48f, 0.45f, 0.43f, 0.4f, 0.35f };


    //Skill measurement variables
    public int guesses = 0, correctGuesses = 0;
    public float guessAccuracy = 0.0f;
    private List<float> guessHeights = new List<float>();
    private List<float> guessTimes = new List<float>();
    public float avgGuessTime, avgGuessHeight, lastGuessTime;
    public int score = 0;
    public int activeCombo = 1, bestCombo = 1;
    public float highestSpeedDuringComboRunHandled = 10.0f;

    //Difficulty variables
    public float shapeColorVariance = 50;
    public int numberOfPointsInShape = 7, maxPointsInShape = 13;
    public float shapeRadius = 2;
    public float speedOfShapes = 1.0f, rotationSpeedOfShapes = 1.0f, currentSpeedOfShapes = 1.0f, rampSpeed = 0.1f;
    public float spawnRate = 1.0f, lastSpawnTime = 0;
    public int maxActiveGroups = 9;
    public float maxTimeToExtendCombo = 1.0f, doubleComboActivationTime = 0.3f;


//METHODS
    private void Start()
    {
        if (PlayerPrefs.HasKey("AdsDisabled"))
        {
            Debug.Log(PlayerPrefs.GetInt("AdsDisabled"));
        }
        Time.timeScale = 1;
        //Get references
        objectManager = GameObject.Find("ObjectManager").GetComponent<ObjectManager>();
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        menuManager = GameObject.Find("MenuManager").GetComponent<MenuManager>();
        petalRotator = GameObject.Find("PetalRotator").GetComponent<ConstantlyRotate>();
        adManager = GameObject.Find("AdManager").GetComponent<AdManager>();
        hapticManager = GameObject.Find("HapticManager").GetComponent<HapticManager>();
        mainCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        randomColorGenerator = GetComponent <RandomColorGenerator>();
        shapeGenerator = GetComponent<ShapeGenerator>();

        //Load in the Highscore if there is one or hide it if there is not one
        if (PlayerPrefs.HasKey("Highscore") == false)
        {
            PlayerPrefs.SetInt("HighScore", 0);
            UpdateScoreText(0, highScoreText);
            UpdateScoreText(0, lastScoreText);
        }
        else
        {
            UpdateScoreText(PlayerPrefs.GetInt("Highscore"), highScoreText);
        }
        if(PlayerPrefs.HasKey("LastScore"))
        {
            UpdateScoreText(PlayerPrefs.GetInt("LastScore"), lastScoreText);
        }

        adManager.LoadBanner();
        //PlayerPrefs.SetInt("Highscore", 0);
    }

    private void FixedUpdate()
    {
        if(playingGame)
        {
            //Count fiftieths of a second from startgame
            fiftiethsOfASecondFromStart++;

            //Spawn new group if it's time and there aren't too many active groups
            if (maxActiveGroups > activeGroups)
            {
                if((fiftiethsOfASecondFromStart - lastSpawnTime) >= (spawnRate * 50)) //If it's time to spawn a new one
                {
                    //Setup the color in the RCG
                    activeGroups++;
                    randomColorGenerator.variance = shapeColorVariance;
                    lastSpawnTime = fiftiethsOfASecondFromStart;
                    objectManager.MoveANewGroupIntoPlay(shapeGenerator.GenerateShape(numberOfPointsInShape, shapeRadius), randomColorGenerator.GenerateColor());
                    rotationSpeedOfShapes *= -1;
                }
            }

            if(activeGroups <= 0)
            {
                missedOne = false;
            }

            //Every second
            if(fiftiethsOfASecondFromStart % 50 == 0)
            {
                maxTimeToExtendCombo = avgGuessTime + 0.1f;
                //count down seconds and update time
                if(infinite)
                {
                    secondsRemaining++;
                    if(secondsRemaining == 60)
                    {
                        minutesRemaining++;
                        secondsRemaining = 0;
                        if(minutesRemaining % 2 == 0)
                        {
                            GrantBomb();
                        }
                    }
                }
                else
                {
                    if (secondsRemaining == 0)
                    {
                        if(minutesRemaining == 1 && timeExtensions == false)
                        {
                            GrantBomb();
                            promptDisplay.QueueUpMessage("1:00");
                        }
                        else if(minutesRemaining == 2)
                        {
                            if(timeExtensions == false)
                                promptDisplay.QueueUpMessage("2:00");
                        }
                        else if(minutesRemaining == 0 && secondsRemaining == 30)
                        {
                            promptDisplay.QueueUpMessage("0:30");
                        }
                        else if (minutesRemaining == 0 && secondsRemaining == 5)
                        {
                            promptDisplay.QueueUpMessage("0:05");
                        }
                        if (minutesRemaining <= 0)
                        {
                            EndGame();
                        }
                        else
                        {
                            secondsRemaining = 59;
                        }
                        if(minutesRemaining > 0)
                        {
                            minutesRemaining--;
                        }
                    }
                    else
                    {
                        secondsRemaining--;
                    }
                }
                UpdateTimeText();

                //Adjust speed with a slow build up and an instant drop
                if(currentSpeedOfShapes < speedOfShapes)
                {
                    currentSpeedOfShapes += rampSpeed;
                }
                else
                {
                    currentSpeedOfShapes = speedOfShapes;
                }

                //Decrement combo if time runs out
                if (Time.time - lastGuessTime > maxTimeToExtendCombo)
                {
                    if(activeCombo > 10)
                    {
                        if (activeCombo / 2 > 10)
                            activeCombo = (int)((float)activeCombo * 0.8f);
                        else
                            activeCombo = 10;
                    }
                    else
                    {
                        if (activeCombo > 1)
                            activeCombo--;
                    }
                }
                UpdateComboText();

                //Respond to skill based on combo
                if (activeCombo >= 400)
                {
                    spawnRate = (avgGuessTime < 0.4f) ? avgGuessTime : 0.3f;
                    speedOfShapes = 26.0f;

                } //200
                if (activeCombo >= 200)
                {
                    spawnRate = (avgGuessTime < 0.5f) ? avgGuessTime : 0.4f;
                    speedOfShapes = 18.0f;
    
                } //100
                if (activeCombo >= 100)
                {
                    spawnRate = (avgGuessTime < 0.6f) ? avgGuessTime : 0.5f;
                    speedOfShapes = 14.0f;

                } //50
                else if (activeCombo >= 50)
                {
                    spawnRate = (avgGuessTime < 0.7f) ? avgGuessTime : 0.6f;
                    speedOfShapes = 12.0f;

                } //25
                else if (activeCombo >= 25)
                {
                    spawnRate = (avgGuessTime < 0.8f) ? avgGuessTime : 0.7f;
                    speedOfShapes = 10.0f;

                } //10
                else if (activeCombo >= 10)
                {
                    spawnRate = (avgGuessTime < 0.8f) ? avgGuessTime : 0.8f;
                    speedOfShapes = 9.0f;

                } //5
                else if (activeCombo >= 5)
                {
                    spawnRate = 0.8f;
                    //spawnRate = (avgGuessTime < 0.8f) ? avgGuessTime : 0.8f;
                    spawnRate = 0.9f;
                    speedOfShapes = 10;

                } //2
                else if (activeCombo >= 2)
                {
                    spawnRate = 0.9f;
                    speedOfShapes = 7.0f;

                } //1
                else
                {
                    spawnRate = 1.2f;
                    speedOfShapes = 7.0f;
                }
                if(!infinite)
                    speedOfShapes += minutesRemaining * 0.1f;

                //Change particle emission rate
                var emission = confettiBois.emission;
                emission.rateOverTime = ((activeCombo < 50) ? 50 : activeCombo);

                //Count seconds since last double tap
                secondsSinceLastDoubleTap++;
                if(secondsSinceLastDoubleTap > 30)
                {
                    secondsSinceLastDoubleTap = 0;
                    promptDisplay.QueueUpMessage("Double tap");
                }
            }

            //Update the petal speed to match the skill
            petalRotator.rotationSpeed = speedOfShapes / 20.0f;
        }
    }

    public void Guess(bool correct, float height, Vector2 touchPosition, int tapCount)
    {
        //Update variables
        guesses++;

        //Update skill measurement variables
        //Guess time
        if (guessTimes.Count > 10)
            guessTimes.RemoveAt(guessTimes.Count - 1);
        guessTimes.Add(Time.time - lastGuessTime);
        foreach (float f in guessTimes)
            avgGuessTime += f;
        avgGuessTime /= guessTimes.Count;
        //Guess height
        guessHeights.Add(height);
        foreach (float f in guessHeights)
            avgGuessHeight += f;
        avgGuessHeight /= guessHeights.Count;

        int tempScore = 0;
        if(correct)
        {
            if(timeExtensions)
            {
                activeCombo += 3;
            }
            //Award points
            correctGuesses++;
            //Reward them for clicking higher up
            if(height < -30)
            {
                tempScore += activeCombo;
            }
            else
            {
                tempScore += 10 * activeCombo;
            }

            //Increment combo if we tapped them fast enough
            if (Time.time - lastGuessTime < maxTimeToExtendCombo)
            {
                //Only give them points if they are clicking it faster than they are created
                activeCombo++;
                //If we tapped a group high up increment it again
                if(height > -30 && Time.time - lastGuessTime < doubleComboActivationTime)
                {
                    activeCombo++;
                }
            }

            if(tapCount == 0)
            {
                //Create a new touch feedback instance
                TextMeshProUGUI newScoreFeedback = Instantiate(scoreTextFeedbackPrefab, touchPosition, Quaternion.identity, mainCanvas.transform);
                newScoreFeedback.text = tempScore.ToString();
            }
        }
        else
        {
            if(activeCombo > 10)
            {
                activeCombo = (int)(activeCombo * 0.8f);
            }
        }

        //Adjust combo
        if(activeCombo >= 400)
        {
            if(level < 10)
            {
                if (timeExtensions == false)
                    soundManager.audioSource.PlayOneShot(soundManager.audioClips[0]);
            }
            prevLevel = level;
            level = 10;
        }
        else if (activeCombo >= 300)
        {
            if (level < 9)
            {
                if(timeExtensions == false)
                    soundManager.audioSource.PlayOneShot(soundManager.audioClips[0]);
            }
            else if (level > 9)
            {
                if (missedOne)
                {
                    missedOne = false;
                }
                else
                {
                    if(timeExtensions == false)
                        soundManager.audioSource.PlayOneShot(soundManager.audioClips[1]);
                }
            }
            prevLevel = level;
            level = 9;
        }
        else if (activeCombo >= 200)
        {
            if (level < 8)
            {
                if(timeExtensions == false)
                    soundManager.audioSource.PlayOneShot(soundManager.audioClips[0]);
            }
            else if (level > 8)
            {
                if (missedOne)
                {
                    missedOne = false;
                }
                else
                {
                    if(timeExtensions == false)
                        soundManager.audioSource.PlayOneShot(soundManager.audioClips[1]);
                }
            }
            prevLevel = level;
            level = 8;
        }
        else if (activeCombo >= 100)
        {
            if (level < 7)
            {
                if(timeExtensions == false)
                    soundManager.audioSource.PlayOneShot(soundManager.audioClips[0]);
            }
            else if (level > 7)
            {
                if (missedOne)
                {
                    missedOne = false;
                }
                else
                {
                    if(timeExtensions == false)
                        soundManager.audioSource.PlayOneShot(soundManager.audioClips[1]);
                }
            }
            prevLevel = level;
            level = 7;
        }
        else if (activeCombo >= 75)
        {
            if (level < 6)
            {
                if(timeExtensions == false)
                    soundManager.audioSource.PlayOneShot(soundManager.audioClips[0]);
            }
            else if (level > 6)
            {
                if (missedOne)
                {
                    missedOne = false;
                }
                else
                {
                    if(timeExtensions == false)
                        soundManager.audioSource.PlayOneShot(soundManager.audioClips[1]);
                }
            }
            prevLevel = level;
            level = 6;
        }
        else if (activeCombo >= 50)
        {
            if (level < 5)
            {
                if(timeExtensions == false)
                    soundManager.audioSource.PlayOneShot(soundManager.audioClips[0]);
            }
            else if (level > 5)
            {
                if (missedOne)
                {
                    missedOne = false;
                }
                else
                {
                    if(timeExtensions == false)
                        soundManager.audioSource.PlayOneShot(soundManager.audioClips[1]);
                }
            }
            prevLevel = level;
            level = 5;
        }
        else if (activeCombo >= 25)
        {
            if (level < 4)
            {
                if(timeExtensions == false)
                    soundManager.audioSource.PlayOneShot(soundManager.audioClips[0]);
            }
            else if (level > 4)
            {
                if (missedOne)
                {
                    missedOne = false;
                }
                else
                {
                    if(timeExtensions == false)
                        soundManager.audioSource.PlayOneShot(soundManager.audioClips[1]);
                }
            }
            prevLevel = level;
            level = 4;
        }
        else if (activeCombo >= 10)
        {
            if (level < 3)
            {
                if(timeExtensions == false)
                    soundManager.audioSource.PlayOneShot(soundManager.audioClips[0]);
            }
            else if (level > 3)
            {
                if (missedOne)
                {
                    missedOne = false;
                }
                else
                {
                    if(timeExtensions == false)
                        soundManager.audioSource.PlayOneShot(soundManager.audioClips[1]);
                }
            }
            prevLevel = level;
            level = 3;
        }
        else if (activeCombo >= 5)
        {
            if (level < 2)
            {
                if(timeExtensions == false)
                    soundManager.audioSource.PlayOneShot(soundManager.audioClips[0]);
            }
            else if (level > 2)
            {
                if (missedOne)
                {
                    missedOne = false;
                }
                else
                {
                    if(timeExtensions == false)
                        soundManager.audioSource.PlayOneShot(soundManager.audioClips[1]);
                }
            }
            prevLevel = level;
            level = 2;
        }
        else if (activeCombo >= 2)
        {
            if (level < 1)
            {
                if(timeExtensions == false)
                    soundManager.audioSource.PlayOneShot(soundManager.audioClips[0]);
            }
            else if (level > 1)
            {
                if(missedOne)
                {
                    missedOne = false;
                }
                else
                {
                    if(timeExtensions == false)
                        soundManager.audioSource.PlayOneShot(soundManager.audioClips[1]);
                }
            }
            prevLevel = level;
            level = 1;
        }
        else
        {
            if (level > 0 && activeCombo > 2)
            {
                if (missedOne)
                {
                    missedOne = false;
                }
                else
                {
                    if(timeExtensions == false)
                        soundManager.audioSource.PlayOneShot(soundManager.audioClips[1]);
                }
            }
            prevLevel = level;
            level = 0;
        }

        //Update the color
        petalRotator.ChangeColor(level);

        //Update the number of points in the shapes
        numberOfPointsInShape = numberOfPointsInShapeLevels[level];
        //Update radius
        shapeRadius = radii[level];

        //Update rotation speed
        rotationSpeedOfShapes = rotationSpeeds[level] * (rotationSpeedOfShapes < 0 ? -1 : 1);

        //update the number of active groups
        maxActiveGroups = maxActiveGroupsLevels[level];

        //Update the color variance
        shapeColorVariance = varianceByLevel[level];


        if (timeExtensions)
        {
            //If we went up a level
            if(level > prevLevel)
            {
                /*
                if(level == 2)
                {
                    UpdateClock(5);
                    promptDisplay.QueueUpMessage("+5s");
                }
                else if (level == 3)
                {
                    UpdateClock(10);
                    promptDisplay.QueueUpMessage("+10s");
                }
                */
                if(score < 50000)
                {
                    if (level == 4)
                    {
                        UpdateClock(1);
                        promptDisplayUpper.QueueUpMessage("+1s");
                    }
                    else if (level == 5)
                    {
                        UpdateClock(10);
                        promptDisplayUpper.QueueUpMessage("+10s");
                    }
                    else if (level == 6)
                    {
                        UpdateClock(10);
                        promptDisplayUpper.QueueUpMessage("+10s");
                    }
                    else if (level == 7)
                    {
                        UpdateClock(10);
                        promptDisplayUpper.QueueUpMessage("+10s");
                    }
                    else if (level == 8)
                    {
                        UpdateClock(10);
                        promptDisplayUpper.QueueUpMessage("+10s");
                    }
                    else if (level == 9)
                    {
                        UpdateClock(20);
                        promptDisplayUpper.QueueUpMessage("+20s");
                    }
                    else if (level == 10)
                    {
                        UpdateClock(20);
                        promptDisplayUpper.QueueUpMessage("+20s");
                    }
                }
                else if (score < 100000)
                {
                    if (level == 4)
                    {
                        UpdateClock(1);
                        promptDisplayUpper.QueueUpMessage("+1s");
                    }
                    else if (level == 5)
                    {
                        UpdateClock(10);
                        promptDisplayUpper.QueueUpMessage("+9s");
                    }
                    else if (level == 6)
                    {
                        UpdateClock(10);
                        promptDisplayUpper.QueueUpMessage("+9s");
                    }
                    else if (level == 7)
                    {
                        UpdateClock(10);
                        promptDisplayUpper.QueueUpMessage("+9s");
                    }
                    else if (level == 8)
                    {
                        UpdateClock(10);
                        promptDisplayUpper.QueueUpMessage("+9s");
                    }
                    else if (level == 9)
                    {
                        UpdateClock(20);
                        promptDisplayUpper.QueueUpMessage("+18s");
                    }
                    else if (level == 10)
                    {
                        UpdateClock(20);
                        promptDisplayUpper.QueueUpMessage("+36s");
                    }
                }
                else if (score < 200000)
                {
                    if (level == 4)
                    {
                        UpdateClock(1);
                        promptDisplayUpper.QueueUpMessage("+1s");
                    }
                    else if (level == 5)
                    {
                        UpdateClock(10);
                        promptDisplayUpper.QueueUpMessage("+8s");
                    }
                    else if (level == 6)
                    {
                        UpdateClock(10);
                        promptDisplayUpper.QueueUpMessage("+8s");
                    }
                    else if (level == 7)
                    {
                        UpdateClock(10);
                        promptDisplayUpper.QueueUpMessage("+8s");
                    }
                    else if (level == 8)
                    {
                        UpdateClock(10);
                        promptDisplayUpper.QueueUpMessage("+8s");
                    }
                    else if (level == 9)
                    {
                        UpdateClock(20);
                        promptDisplayUpper.QueueUpMessage("+16s");
                    }
                    else if (level == 10)
                    {
                        UpdateClock(20);
                        promptDisplayUpper.QueueUpMessage("+32s");
                    }
                }
                else if (score < 300000)
                {
                    if (level == 4)
                    {
                        UpdateClock(1);
                        promptDisplayUpper.QueueUpMessage("+1s");
                    }
                    else if (level == 5)
                    {
                        UpdateClock(10);
                        promptDisplayUpper.QueueUpMessage("+7s");
                    }
                    else if (level == 6)
                    {
                        UpdateClock(10);
                        promptDisplayUpper.QueueUpMessage("+7s");
                    }
                    else if (level == 7)
                    {
                        UpdateClock(10);
                        promptDisplayUpper.QueueUpMessage("+7s");
                    }
                    else if (level == 8)
                    {
                        UpdateClock(10);
                        promptDisplayUpper.QueueUpMessage("+7s");
                    }
                    else if (level == 9)
                    {
                        UpdateClock(20);
                        promptDisplayUpper.QueueUpMessage("+14s");
                    }
                    else if (level == 10)
                    {
                        UpdateClock(20);
                        promptDisplayUpper.QueueUpMessage("+28s");
                    }
                }
                else if (score < 400000)
                {
                    if (level == 4)
                    {
                        UpdateClock(1);
                        promptDisplayUpper.QueueUpMessage("+1s");
                    }
                    else if (level == 5)
                    {
                        UpdateClock(10);
                        promptDisplayUpper.QueueUpMessage("+6s");
                    }
                    else if (level == 6)
                    {
                        UpdateClock(10);
                        promptDisplayUpper.QueueUpMessage("+6s");
                    }
                    else if (level == 7)
                    {
                        UpdateClock(10);
                        promptDisplayUpper.QueueUpMessage("+6s");
                    }
                    else if (level == 8)
                    {
                        UpdateClock(10);
                        promptDisplayUpper.QueueUpMessage("+6s");
                    }
                    else if (level == 9)
                    {
                        UpdateClock(20);
                        promptDisplayUpper.QueueUpMessage("+12s");
                    }
                    else if (level == 10)
                    {
                        UpdateClock(20);
                        promptDisplayUpper.QueueUpMessage("+24s");
                    }
                }
                else if (score < 800000)
                {
                    if (level == 4)
                    {
                        UpdateClock(1);
                        promptDisplayUpper.QueueUpMessage("+1s");
                    }
                    else if (level == 5)
                    {
                        UpdateClock(10);
                        promptDisplayUpper.QueueUpMessage("+5s");
                    }
                    else if (level == 6)
                    {
                        UpdateClock(10);
                        promptDisplayUpper.QueueUpMessage("+5s");
                    }
                    else if (level == 7)
                    {
                        UpdateClock(10);
                        promptDisplayUpper.QueueUpMessage("+5s");
                    }
                    else if (level == 8)
                    {
                        UpdateClock(10);
                        promptDisplayUpper.QueueUpMessage("+5s");
                    }
                    else if (level == 9)
                    {
                        UpdateClock(20);
                        promptDisplayUpper.QueueUpMessage("+10s");
                    }
                    else if (level == 10)
                    {
                        UpdateClock(20);
                        promptDisplayUpper.QueueUpMessage("+20s");
                    }
                }
                else if (score < 900000)
                {
                    if (level == 4)
                    {
                        UpdateClock(1);
                        promptDisplayUpper.QueueUpMessage("+1s");
                    }
                    else if (level == 5)
                    {
                        UpdateClock(10);
                        promptDisplayUpper.QueueUpMessage("+4s");
                    }
                    else if (level == 6)
                    {
                        UpdateClock(10);
                        promptDisplayUpper.QueueUpMessage("+4s");
                    }
                    else if (level == 7)
                    {
                        UpdateClock(10);
                        promptDisplayUpper.QueueUpMessage("+4s");
                    }
                    else if (level == 8)
                    {
                        UpdateClock(10);
                        promptDisplayUpper.QueueUpMessage("+4s");
                    }
                    else if (level == 9)
                    {
                        UpdateClock(20);
                        promptDisplayUpper.QueueUpMessage("+8s");
                    }
                    else if (level == 10)
                    {
                        UpdateClock(20);
                        promptDisplayUpper.QueueUpMessage("+16s");
                    }
                }
                else if (score < 900000)
                {
                    if (level == 4)
                    {
                        UpdateClock(1);
                        promptDisplayUpper.QueueUpMessage("+1s");
                    }
                    else if (level == 5)
                    {
                        UpdateClock(10);
                        promptDisplayUpper.QueueUpMessage("+3s");
                    }
                    else if (level == 6)
                    {
                        UpdateClock(10);
                        promptDisplayUpper.QueueUpMessage("+3s");
                    }
                    else if (level == 7)
                    {
                        UpdateClock(10);
                        promptDisplayUpper.QueueUpMessage("+3s");
                    }
                    else if (level == 8)
                    {
                        UpdateClock(10);
                        promptDisplayUpper.QueueUpMessage("+3s");
                    }
                    else if (level == 9)
                    {
                        UpdateClock(20);
                        promptDisplayUpper.QueueUpMessage("+6s");
                    }
                    else if (level == 10)
                    {
                        UpdateClock(20);
                        promptDisplayUpper.QueueUpMessage("+12s");
                    }
                }
                else if (score < 1000000)
                {
                    if (level == 4)
                    {
                        UpdateClock(1);
                        promptDisplayUpper.QueueUpMessage("+1s");
                    }
                    else if (level == 5)
                    {
                        UpdateClock(10);
                        promptDisplayUpper.QueueUpMessage("+2s");
                    }
                    else if (level == 6)
                    {
                        UpdateClock(10);
                        promptDisplayUpper.QueueUpMessage("+2s");
                    }
                    else if (level == 7)
                    {
                        UpdateClock(10);
                        promptDisplayUpper.QueueUpMessage("+2s");
                    }
                    else if (level == 8)
                    {
                        UpdateClock(10);
                        promptDisplayUpper.QueueUpMessage("+2s");
                    }
                    else if (level == 9)
                    {
                        UpdateClock(20);
                        promptDisplayUpper.QueueUpMessage("+4s");
                    }
                    else if (level == 10)
                    {
                        UpdateClock(20);
                        promptDisplayUpper.QueueUpMessage("+8s");
                    }
                }

                //Display the level we just got too
                if (level >= 4)
                {
                    //if (promptDisplay.messageQueue.Count < 2)
                        //promptDisplay.QueueUpMessage("lvl " + (level - 1).ToString());
                    redeemButton.gameObject.SetActive(true);
                }
                else
                {
                    redeemButton.gameObject.SetActive(false);
                }

            }
            else if(level < prevLevel)
            {
                ResetComboAndLevel();
            }
        }

        score += tempScore;
        UpdateComboText();
        UpdateScoreText(score, scoreText);
        lastGuessTime = Time.time;
    }

    public void UpdateClock(int seconds)
    {
        minutesRemaining += seconds / 60;
        secondsRemaining += seconds % 60;
        if(secondsRemaining >= 60)
        {
            minutesRemaining += secondsRemaining / 60;
            secondsRemaining = secondsRemaining % 60;
        }
    }

    //Aesthetic methods
    public void UpdateScoreText(int newScore, TextMeshProUGUI output)
    {
        if(score > 9999999)
        {
            output.text = "| Chill |";
            return;
        }
        string updatedScoreText = "<mspace=0.64em>|";
        int digitsInScore = (int)Mathf.Log10(newScore) + 1;
        for(int i = 0; i < 7 - digitsInScore; i++)
        {
            updatedScoreText += "0";
        }
        updatedScoreText += newScore.ToString();
        updatedScoreText += "|";
        if(newScore == 0)
        {
            output.text = "<mspace=0.64em>|0000000|";
        }
        else
        {
            output.text = updatedScoreText;
        }
    }
    public void UpdateComboText()
    {
        comboText.text = "Combo " + activeCombo.ToString() + "x";
    }
    public void UpdateTimeText()
    {
        timeText.text = minutesRemaining.ToString() + ":";
        if(secondsRemaining < 10)
        {
            timeText.text += "0";
        }
        timeText.text += secondsRemaining.ToString();
    }

    //BOMB
    public void GrantBomb()
    {
        if (bombActive == true)
            return;
        activeBomb = Instantiate(bombPrefab, bombSpawnPos, Quaternion.identity);
        activeBomb.GetComponent<BombScript>().gameManager = this;
        bombActive = true;
    }
    public void UseBomb()
    {
        bombActive = false;
        UpdateScoreText(score, scoreText);
        //StartCoroutine("SlowMo");
    }
    private int SlowMoTime = 30, activeSlowMoTime = 0;
    private IEnumerator SlowMo()
    {
        activeSlowMoTime++;
        if (activeSlowMoTime == SlowMoTime)
        {
            activeSlowMoTime = 0;
            Time.timeScale = 1.0f;
        }
        else
        {
            Time.timeScale = 0.8f;
            yield return new WaitForSeconds(1);
            StartCoroutine("SlowMo");
        }
        if(activeSlowMoTime == 5)
        {
            promptDisplay.QueueUpMessage((SlowMoTime - activeSlowMoTime).ToString());
        }
        Debug.Log("Time scale: " + Time.timeScale);
    }

    public void SetInfinite(bool newVal)
    {
        infinite = newVal;
    }
    public void SetTimeExtensions(bool newVal)
    {
        timeExtensions = newVal;
    }
    public void ReloadMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ResetComboAndLevel()
    {
        if (playingGame == false) return;

        if(activeCombo > 10)
        {
            //Apply points
            promptDisplayUpper.QueueUpMessage("+" + (activeCombo * 100));
            score += activeCombo * 100;
            activeCombo = 1;
            level = 0;
            prevLevel = 0;
            UpdateComboText();
            hapticManager.HeavyBoop();
            soundManager.audioSource.PlayOneShot(soundManager.audioClips[0]);
            redeemButton.gameObject.SetActive(false);
        }
    }

    public void StartGame()
    {
        stop.gameObject.SetActive(false);
        showAdForTime.gameObject.SetActive(false);
        if(infinite)
        {
            minutesRemaining = 0;
            secondsRemaining = 0;
        }
        if(timeExtensions)
        {
            minutesRemaining = 0;
            secondsRemaining = 30;
        }
        if(infinite == false && timeExtensions == false)
        {
            minutesRemaining = 3;
            secondsRemaining = 0;
        }
        UpdateTimeText();
        playingGame = true;
        score = 0;
        UpdateScoreText(score, scoreText);
    }

    public void PauseGame()
    {
        if(Time.timeScale < 1)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }
    }

    public void HideStopandAd()
    {
        stop.gameObject.SetActive(false);
        showAdForTime.gameObject.SetActive(false);
    }
    public void EndGame()
    {
        promptDisplay.messageQueue.Clear();
        if(timeExtensions && playingGame)
        {
            if(PlayerPrefs.HasKey("AdsDisabled") && PlayerPrefs.GetInt("AdsDisabled") == 1)
            {
                pressForMoreTime.gameObject.SetActive(true);
            }
            else
            {
                promptDisplay.QueueUpMessage("Watch an ad, time +0:30?");
                showAdForTime.gameObject.SetActive(true);
            }
            redeemButton.gameObject.SetActive(false);
            stop.gameObject.SetActive(true);
            playingGame = false;
        }
        else
        {
            //Save highscore if you're playing non infinite
            if(infinite == false && timeExtensions == false && PlayerPrefs.GetInt("Highscore") < score)
            {
                PlayerPrefs.SetInt("Highscore", score);
            }
            PlayerPrefs.SetInt("LastScore", score);
            PlayerPrefs.Save();

            //Load the scene again to guarentee a clean reset
            ReloadMenu();
        }
    }
}
