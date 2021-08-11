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


       


        //Admob
        MobileAds.Initialize(_appIdAdmob);
        this.RequestBanner();
        


        //Unity consent
        MetaData gdprMetaData = new MetaData("gdpr");
        string _consentToUnity = (_consentUserToGdpr) ? "true" : "false";
        gdprMetaData.Set("consent", _consentToUnity);
        Advertisement.SetMetaData(gdprMetaData);

        

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

    
    public void SendIntersitialAds()
    {
        //1) UNity Ads
        if (Advertisement.IsReady("video"))
        {
            Advertisement.Show("video");
            return;
        }

    }


    public void SendRewaredAds()
    {
        //1) Unity Rewared
        if (_testMode)
        {
            _sendAfterFinishedRewared();
            return;
        }

        if (Advertisement.IsReady("rewardedVideo"))
        {
            Advertisement.Show("rewardedVideo", _optionsRevared);
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
          

    
}
