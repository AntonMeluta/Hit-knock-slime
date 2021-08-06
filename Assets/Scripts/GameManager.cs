using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Реализовать синглтоны
//Гейм стейт в гейм менеджере
//Дописать гейм менеджер и проверить ссылки в инспекторе

//GameManager (управляет состоянием игры, сценами/менюшками, их переключением)
public class GameManager : Singleton<GameManager>
{
    GameControl gameControl;

    //transitions
    public GameObject[] _shopWindow;
    public GameObject[] _gameWindow;

    //GDPR check
    int _gdprState;
    public GameObject _gdprPanelText;

    public string _refMoreGames;
    public string _refPrivacyPolicy;

    private void Start()
    {
        gameControl = GameObject.FindObjectOfType<GameControl>();

        _gdprState = PlayerPrefs.GetInt("_gdprState", 0);
        if (_gdprState > 0 || Application.systemLanguage == SystemLanguage.Russian)
            GDPRAgreeI();
        else
            _gdprPanelText.SetActive(true);
    }

    #region Switching between scenes/menus   
    public void ToGameWindow()
    {
        foreach (var item in _shopWindow)
            item.SetActive(false);
        foreach (var item in _gameWindow)
            item.SetActive(true);

        gameControl.StartLvlFirst();
    }

    public void ToShopWindow()
    {
        foreach (var item in _shopWindow)
            item.SetActive(true);
        foreach (var item in _gameWindow)
            item.SetActive(false);

        CancelInvoke("SendLoss");
        AdvertisingHand._advertisingHand.SendIntersitialAds();
    }
    #endregion

    #region GDPR    
    public void GDPRAgreeI()
    {
        if (_gdprState == 0)
        {
            _gdprState = 1;
            PlayerPrefs.SetInt("_gdprState", _gdprState);
        }

        AdvertisingHand._advertisingHand._consentUserToGdpr = true;
        AdvertisingHand._advertisingHand.UserSelectedConsent();

        _gdprPanelText.SetActive(false);

    }

    public void GDPRNoThanks()
    {
        AdvertisingHand._advertisingHand._consentUserToGdpr = false;
        AdvertisingHand._advertisingHand.UserSelectedConsent();

        _gdprPanelText.SetActive(false);
    }
    #endregion




}
