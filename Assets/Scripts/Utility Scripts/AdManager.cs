using System.Collections;
using System.Collections.Generic;
using UnityEngine.Advertisements;
using UnityEngine;
using System;

/*
 * Check if ads are disabled, and go inactive if true
 * else
 *  show a banner
 *  listen for buttons to request interstitial ads
 */

public class AdManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    //GameId and platformInformation
    string gameId = "4815227";
    string platformInformation = "iOS";
    bool showAds = true;

    //Ad variables
    [SerializeField] bool _testMode = false;

    //Banner
    [SerializeField] BannerPosition _bannerPosition = BannerPosition.BOTTOM_CENTER;
    string _banner_adUnitId = "Banner_iOS";
    string _interstitial_adUnitId  = "Interstitial_iOS";

    //Game Variables
    GameManager gameManager;


    void Awake()
    {
        //If we have a key
        if(PlayerPrefs.HasKey("AdsDisabled"))
        {
            //If it is true
            if(PlayerPrefs.GetInt("AdsDisabled") == 1)
            {
                if (Advertisement.isInitialized)
                {
                    //turn the ads off, but first, hide the banner
                    HideBannerAd();
                    showAds = false;
                }
            }
        }
        else //If we haven't paid to remove ads
        {
            //Reference the gamemanager
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            //Initialize ads
            InitializeAds();
            //Show a banner
            Advertisement.Banner.SetPosition(_bannerPosition);
        }
    }

    //INIT
    public void InitializeAds()
    {
        //Sets gameid and sets this script as the initialization listener
        Advertisement.Initialize(gameId, _testMode, this);
    }

    public void OnInitializationComplete()
    {
        //Debug.Log("Unity Ads initialization complete.");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        //Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

    //LOAD
    public void OnUnityAdsAdLoaded(string placementId)
    {
        //Show an ad once one's been loaded
        if(placementId == _interstitial_adUnitId && showAds == true)
        {
            Advertisement.Show(_interstitial_adUnitId, this);
        }
        //Debug.Log($"Unity Ads Loaded");

    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        //Debug.Log($"Unity Ads Failed to Load");
    }

    //SHOW
    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        //Debug.Log($"Unity Ads Failed to Show");
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        //Debug.Log($"Unity Ads Began Showing");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        //Debug.Log($"Unity Ad was clicked");
        gameManager.ReloadMenu();
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        //Debug.Log($"Unity Ad was completed");
        if (gameManager.timeExtensions)
        {
            if(placementId == _interstitial_adUnitId)
            {
                gameManager.playingGame = true;
                gameManager.HideStopandAd();
                gameManager.minutesRemaining = 0;
                gameManager.secondsRemaining = 30;
                gameManager.UpdateTimeText();
            }
        }
        else if(placementId == _interstitial_adUnitId)
        {
            gameManager.StartGame();
            gameManager.GrantBomb();
        }
    }

    //INTERSTITIAL METHODS
    public void LoadInterstitialAd()
    {
        if(showAds == true)
            Advertisement.Load(_interstitial_adUnitId, this);
        //Debug.Log("Loading: " + _interstitial_adUnitId);
    }


    //BANNER METHODS
    public void LoadBanner()
    {
        if(showAds == true)
        {
            BannerLoadOptions options = new BannerLoadOptions
            {
                loadCallback = OnBannerLoaded,
                errorCallback = OnBannerError
            };

            Advertisement.Banner.Load(_banner_adUnitId, options);
        }
    }

    // Implement code to execute when the loadCallback event triggers:
    void OnBannerLoaded()
    {
        //Show the banner when it is loaded
        if(showAds)
            ShowBannerAd();
        //Debug.Log("Banner loaded");
    }

    // Implement code to execute when the load errorCallback event triggers:
    void OnBannerError(string message)
    {
        //Debug.Log($"Banner Error: {message}");
        // Optionally execute additional code, such as attempting to load another ad.
    }

    // Implement a method to call when the Show Banner button is clicked:
    void ShowBannerAd()
    {
        // Set up options to notify the SDK of show events:
        if (showAds)
        {
            BannerOptions options = new BannerOptions
            {
                clickCallback = OnBannerClicked,
                hideCallback = OnBannerHidden,
                showCallback = OnBannerShown
            };

            // Show the loaded Banner Ad Unit:
            Advertisement.Banner.Show(_banner_adUnitId, options);
        }
    }

    // Implement a method to call when the Hide Banner button is clicked:
    void HideBannerAd()
    {
        // Hide the banner:
        Advertisement.Banner.Hide();
    }

    void OnBannerClicked() {
        //Debug.Log("Banner Clicked");
    }
    void OnBannerShown() {
        //Debug.Log("Banner Shown");
    }
    void OnBannerHidden() {
        //Debug.Log("Banner Hidden");
    }
}