using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    GameState currentGameState = GameState.PREGAME;



    private void Start()
    {
        gameControl = GameObject.FindObjectOfType<GameControl>();

        _gdprState = PlayerPrefs.GetInt("_gdprState", 0);
        if (_gdprState > 0 || Application.systemLanguage == SystemLanguage.Russian)
            GDPRAgreeI();
        else
            _gdprPanelText.SetActive(true);
    }

    #region Properties    
    public GameState CurrentGameState
    {
        get
        {
            return currentGameState;
        }
        set
        {
            //currentGameState = value;
        }
    }
    #endregion

    #region Switching between scenes/menus   
    public void ToGameWindow()
    {
        foreach (var item in _shopWindow)
            item.SetActive(false);
        foreach (var item in _gameWindow)
            item.SetActive(true);

        currentGameState = GameState.GAME;
        gameControl.StartLvlFirst();
    }

    public void ToShopWindow()
    {
        foreach (var item in _shopWindow)
            item.SetActive(true);
        foreach (var item in _gameWindow)
            item.SetActive(false);

        currentGameState = GameState.SHOP;
        AdvertisingHand.Instance.SendIntersitialAds();
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

        AdvertisingHand.Instance._consentUserToGdpr = true;
        AdvertisingHand.Instance.UserSelectedConsent();

        _gdprPanelText.SetActive(false);

    }

    public void GDPRNoThanks()
    {
        AdvertisingHand.Instance._consentUserToGdpr = false;
        AdvertisingHand.Instance.UserSelectedConsent();

        _gdprPanelText.SetActive(false);
    }
    #endregion

    #region Outer Ref
    public void TransitionMoreGames()
    {
        Application.OpenURL(_refMoreGames);
    }

    public void TransitionPrivacyPolicy()
    {
        Application.OpenURL(_refPrivacyPolicy);
    }
    #endregion

}


public enum GameState
{
    PREGAME,
    SHOP,
    GAME
}
