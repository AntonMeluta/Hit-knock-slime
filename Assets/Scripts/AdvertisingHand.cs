using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdvertisingHand : Singleton<AdvertisingHand>
{

     int _fixDoubleSend = 0;
    AudioSource _music;

    //For Admob Ads
    AdPosition _posTopOrBottom;
    private BannerView _bannerView;

    //---------------------------------------------------------------------------------------------------------

    public delegate void VideoFinished();
    public VideoFinished _sendAfterFinishedRewared;
    ShowOptions _optionsRevared;

    public bool _topBanner = false;
    public bool _testMode = false;
    //private string _testModeForApplovin;
    public bool _consentUserToGdpr = false;

    //For admob
    public string _appIdAdmob;
    public string _bannerIdAdmob;

    //vungle variables
    public string appIdVungle;
    public string _intersitialVungleAutomaticCache;
    public string _rewaredVungleNonAutomaticCache;


    //For Aplovin
    public string SDK_KEY = "mTanzsuvkpTB-Ln8_xJ7I3-ZhOUzDO3-ydlvUdpDRHOPcATb5siNvhyb4RTIrcGnF0gldUy3mLSMV0ybsf-mLw";
    // Controlled State
    private bool IsPreloadingRewardedVideo = false;





    protected override void Awake()
    {
        //Singletone realise
        base.Awake();        

        _optionsRevared = new ShowOptions { resultCallback = HandleShowResult };
        _posTopOrBottom = (_topBanner) ? AdPosition.Top : AdPosition.Bottom;
        if (_testMode)
        {
            ///_appId = "ca-app-pub-3940256099942544~3347511713";
            _bannerIdAdmob = "ca-app-pub-3940256099942544/2934735716";
        }


        //test
        //UserSelectedConsent();

    }


    private void Start()
    {
        _music = GetComponent<AudioSource>();
    }


    private void OnDestroy()
    {
        _bannerView.Destroy();
    }

    
    public void UserSelectedConsent()
    {


        if (_fixDoubleSend > 0)
            return;
        _fixDoubleSend++;


        //AppLovin
        if ("devFromMordor".Equals(SDK_KEY))
        {
            Debug.Log("ERROR: PLEASE UPDATE YOUR SDK KEY IN Assets/MainMenu.cs");
        }
        else
        {
            string _consentForApplovin = (_consentUserToGdpr) ? "true" : "false";
            AppLovin.SetHasUserConsent(_consentForApplovin);

            AppLovin.SetSdkKey(SDK_KEY);
            AppLovin.InitializeSdk();

            string _testModeForApplovin = (_testMode) ? "true" : "false";
            AppLovin.SetTestAdsEnabled(_testModeForApplovin);

            AppLovin.SetUnityAdListener(gameObject.name);
            // Set SDK key and initialize SDK


            AppLovin.PreloadInterstitial();
            AppLovin.LoadRewardedInterstitial();

            //}
            Debug.Log("ПРИВЕТ из мордора");
        }


        //Admob
        MobileAds.Initialize(_appIdAdmob);
        this.RequestBanner();
        


        //Unity consent
        MetaData gdprMetaData = new MetaData("gdpr");
        string _consentToUnity = (_consentUserToGdpr) ? "true" : "false";
        gdprMetaData.Set("consent", _consentToUnity);
        Advertisement.SetMetaData(gdprMetaData);


        //Vungle
        //vungle
        /*тестовые ключи
         * app id - "591236625b2480ac40000028"
         * intersitial DEFAULT - "DEFAULT18080"
         * rewared NOT DEFAULT - "PLMT03R02739" 
         */
        Vungle.init(appIdVungle);
        initializeEventHandlers();
        //GDPR Vungle
        Vungle.Consent consentVungle;
        if (_consentUserToGdpr)
            consentVungle = Vungle.Consent.Accepted;
        else
            consentVungle = Vungle.Consent.Denied;
        Vungle.updateConsentStatus(consentVungle);



    }

    #region Reqest AdmobAds
    private void RequestBanner()
    {

        // Create a 320x50 banner at the top of the screen.
        _bannerView = new BannerView(_bannerIdAdmob, AdSize.Banner, _posTopOrBottom);


        // Called when an ad request has successfully loaded.
        _bannerView.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        _bannerView.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is clicked.
        _bannerView.OnAdOpening += HandleOnAdOpened;
        // Called when the user returned from the app after an ad click.
        _bannerView.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        _bannerView.OnAdLeavingApplication += HandleOnAdLeavingApplication;


        // Create an empty ad request.
        AdRequest request;
        if (_consentUserToGdpr)
            request = new AdRequest.Builder().Build();
        else
            request = new AdRequest.Builder().AddExtra("npa", "1").Build();
        // Load the banner with the request.
        _bannerView.LoadAd(request);

        _bannerView.Show();

    }

    

    #endregion


    public void ShowBanner()
    {
        //this.RequestBanner();
    }


    public void HideBanner()
    {
        //_bannerView.Destroy();
    }

    // Setup EventHandlers for all available Vungle events
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            Vungle.onPause();
        }
        else
        {
            Vungle.onResume();
        }
    }


    public void SendIntersitialAds()
    {
        //1) UNity Ads
        if (Advertisement.IsReady("video"))
        {
            Advertisement.Show("video");
            return;
        }


        
        /*//3) Vungle Ads
        if (Vungle.isAdvertAvailable(_intersitialVungleAutomaticCache))
        {
            Vungle.playAd( _intersitialVungleAutomaticCache);
            return;
        }


        //2) Applovin
        if (AppLovin.HasPreloadedInterstitial())
        {
            // An ad is currently available, so show the interstitial.
            AppLovin.ShowInterstitial();
            return;
        }*/


    }


    public void SendRewaredAds()
    {
        //1) Unity Rewared
        if (Advertisement.IsReady("rewardedVideo"))
        {
            Advertisement.Show("rewardedVideo", _optionsRevared);
            return;
        }


       

        /*//3) Vungle Rewared
        if (Vungle.isAdvertAvailable(_rewaredVungleNonAutomaticCache))
        {
            

            Vungle.playAd(_rewaredVungleNonAutomaticCache);
            return;
        }



        //2)Applovin rewared
        if (AppLovin.IsIncentInterstitialReady())
        {
            AppLovin.ShowRewardedInterstitial();
            return;
        }*/



    }


    public void SendIntersitialVungle()
    {

        if (Vungle.isAdvertAvailable(_intersitialVungleAutomaticCache))
        {
            Vungle.playAd(/*options,*/ _intersitialVungleAutomaticCache);
            return;
        }

    }

    public void SendRewaredVungle()
    {
        if (Vungle.isAdvertAvailable(_rewaredVungleNonAutomaticCache))
        {
            /* Dictionary<string, object> options = new Dictionary<string, object>();
             //options["userTag"] = "test_user_id";
             options["alertTitle"] = "Careful!";
             options["alertText"] = "If the video isn't completed you won't get your reward! Are you sure you want to close early?";
             options["closeText"] = "Close";
             options["continueText"] = "Keep Watching";*/

            Vungle.playAd(/*options, */_rewaredVungleNonAutomaticCache);
            return;
        }
    }





    #region Rewared callback handlers UnityAds
    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                _sendAfterFinishedRewared();
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                break;
        }
    }
    #endregion





    #region Callbacks Banners
    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
                            + args.Message);
        //this.RequestBanner();
        /*if (_topBanner)
            AppLovin.ShowAd(AppLovin.AD_POSITION_CENTER, AppLovin.AD_POSITION_TOP);
        else
            AppLovin.ShowAd(AppLovin.AD_POSITION_CENTER, AppLovin.AD_POSITION_BOTTOM);*/
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeavingApplication event received");
    }
    #endregion


    

    #region Callback Applovin
    private void onAppLovinEventReceived(string ev)
    {
        //Подтверждение от сервера Applovin (т.е. Реклама просмотрена).
        if (ev.Contains("REWARDAPPROVEDINFO"))
        {
            _sendAfterFinishedRewared();
        }
        else if (ev.Contains("LOADREWARDEDFAILED"))
        {
            // A rewarded video failed to load.
        }
        //Закрытие Rewared
        else if (ev.Contains("HIDDENREWARDED"))
        {
            // A rewarded video has been closed.  Preload the next rewarded video.
            AppLovin.LoadRewardedInterstitial();
            Time.timeScale = 1;
            _music.mute = false;
        }
        //закрытие Intersitial
        else if (ev.Contains("HIDDENINTER"))
        {
            // Ad ad was closed.  Resume the game.
            // If you're using PreloadInterstitial/HasPreloadedInterstitial, make a preload call here.
            AppLovin.PreloadInterstitial();
            Time.timeScale = 1;
            _music.mute = false;
        }
        //Старт видео
        else if (ev.Contains("VIDEOBEGAN"))
        {
            Time.timeScale = 0;
            _music.mute = true;
        }
        //Видео завершено
        else if (ev.Contains("VIDEOSTOPPED"))
        {
            /*Time.timeScale = 1;
            _music.mute = false;*/
        }


    }
    #endregion


    #region Callback Vungle
    void initializeEventHandlers()
    {

        // Event triggered during when an ad is about to be played
        Vungle.onAdStartedEvent += (placementID) => {
            //DebugLog("Ad " + placementID + " is starting!  Pause your game  animation or sound here.");
            Time.timeScale = 0;
            _music.mute = true;
        };

        // Event is triggered when a Vungle ad finished and provides the entire information about this event
        // These can be used to determine how much of the video the user viewed, if they skipped the ad early, etc.
        Vungle.onAdFinishedEvent += (placementID, args) => {
            /*DebugLog("Ad finished - placementID " + placementID + ", was call to action clicked:" + args.WasCallToActionClicked + ", is completed view:"
                + args.IsCompletedView);*/
            if (placementID == _rewaredVungleNonAutomaticCache)
            {
                if (args.IsCompletedView)
                {
                    _sendAfterFinishedRewared();
                }
                Vungle.loadAd(_rewaredVungleNonAutomaticCache);
            }

            Time.timeScale = 1;
            _music.mute = false;


        };


        //Вызывается когда объявления закэшированы, для неавтоматического ещё и в случае failed
        Vungle.adPlayableEvent += (placementID, adPlayable) => {
            //DebugLog("Ad's playable state has been changed! placementID " + placementID + ". Now: " + adPlayable);
            //_delText.text = "Ad's playable state has been changed! placementID " + placementID + ". Now: " + adPlayable;
            //placements[placementID] = adPlayable;
        };

        //Fired initialize event from sdk
        Vungle.onInitializeEvent += () => {
            //adInited = true;
            /*DebugLog("SDK initialized");
            _delText.text = "SDK initialized";*/
            Vungle.loadAd(_rewaredVungleNonAutomaticCache);
        };

        // Other events
        /*
		//Vungle.onLogEvent += (log) => {
			DebugLog ("Log: " + log);
		};

		Vungle.onPlacementPreparedEvent += (placementID, bidToken) => {
		    DebugLog ("<onPlacementPreparedEvent> Placement Ad is prepared with bidToken! " + placementID + " " + bidToken);
		};
		 
		Vungle.onVungleCreativeEvent += (placementID, creativeID) => {
		    DebugLog ("<onVungleCreativeEvent> Placement Ad is about to play with creative ID " + placementID + " " + creativeID);
		};
		*/
    }
    #endregion
}
