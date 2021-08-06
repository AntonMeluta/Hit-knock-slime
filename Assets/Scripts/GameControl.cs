using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//GameManager (управляет состоянием игры, сценами/менюшками, их переключением)

//отслеживание актуальное состояние геймплея, условия победы/поражения
public class GameControl : MonoBehaviour
{
    
    [HideInInspector]public Material _materialForBalls;
    LvlControl _getLvlInfo;

    int _bestScore;
    [SerializeField] Text[] _uiBestScore;
    int _currentScore;
    [SerializeField] Text[] _uiCurrentScore;
    public Text _countLastBallsUi;
    public int _countLastBallsInt;


    //нужно знать сколько банок до победы должно упасть вниз
    int _countBankToWin;
    //количество упавших банок на текущий момент
    int _countBankToDownNow;
    //нужно знать сколько шаров до поражения должно упасть
    int _countBallsToLoss;
    //количество упавших шаров на текущий момент
    int _countBallsToDownNow;


    int _indexCurrentLevel;
    public GameObject[] _packLevels;
    public GameObject[] _allBalls;

    //transitions
    public GameObject[] _shopWindow;
    public GameObject[] _gameWindow;

    //GDPR check
    int _gdprState;
    public GameObject _gdprPanelText;

    public GameObject _panelQuestionsInGame;
    public GameObject _greatTextBeforeNextLvl;

    public string _refMoreGames;
    public string _refPrivacyPolicy;


    private void Awake()
    {
        
        _bestScore = PlayerPrefs.GetInt("hittBestScore", 0);
        foreach (var item in _uiBestScore)
            item.text = _bestScore.ToString();
        
    }

    private void Start()
    {
        _gdprState = PlayerPrefs.GetInt("_gdprState", 0);
        if (_gdprState > 0 || Application.systemLanguage == SystemLanguage.Russian)
            GDPRAgreeI();
        else
            _gdprPanelText.SetActive(true);
    }


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


    public void ToGameWindow()
    {        
        foreach (var item in _shopWindow)
            item.SetActive(false);        
        foreach (var item in _gameWindow)
            item.SetActive(true);
        StartLvlFirst();
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


    public void StartLvlFirst()
    {

        //обнуление текущих значений упавших юанов и шаров
        _countBankToDownNow = 0;
        _countBallsToDownNow = 0;


        //включаем первый уровень
        _indexCurrentLevel = 0;
        for (int i = 0; i < _packLevels.Length; i++)
            _packLevels[i].SetActive(false);
        _packLevels[_indexCurrentLevel].SetActive(true);

        _getLvlInfo = _packLevels[_indexCurrentLevel].GetComponent<LvlControl>();
        //записываем количество банок, которые надо уронить
        _countBankToWin = _getLvlInfo._countBank;

        //включаем необходимое количество шаров
        _countBallsToLoss = (int)_getLvlInfo._countBalls;
        _countLastBallsInt = _countBallsToLoss;
        _countLastBallsUi.text = _countLastBallsInt.ToString();
        for (int i = 0; i < _allBalls.Length; i++)
            _allBalls[i].SetActive(false);
        for (int i = 0; i < _countBallsToLoss; i++)
            _allBalls[i].SetActive(true);

        _currentScore = 0;
        foreach (var item in _uiCurrentScore)
            item.text = _currentScore.ToString();

        
    }

   

    public void ToNextLevel()
    {
        _greatTextBeforeNextLvl.SetActive(false);
        

        //обнуление текущих значений упавших юанов и шаров
        _countBankToDownNow = 0;
        _countBallsToDownNow = 0;

        //включаем следующий уровень
        _indexCurrentLevel++;
        if (_indexCurrentLevel == _packLevels.Length)
            _indexCurrentLevel = 13;
        print("_indexCurrentLevel = " + _indexCurrentLevel);

        if (_indexCurrentLevel > 14)
        {
            if (_indexCurrentLevel % 2 == 1)
                AdvertisingHand._advertisingHand.SendIntersitialAds();
        }
        else if (_indexCurrentLevel >= 6)
        {
            if (_indexCurrentLevel % 6 == 0)
                AdvertisingHand._advertisingHand.SendIntersitialAds();
        }
        
        

        for (int i = 0; i < _packLevels.Length; i++)
            _packLevels[i].SetActive(false);
        _packLevels[_indexCurrentLevel].SetActive(true);
        //Получаем скрипт уровня, для доступа к соответствующей инфе
        _getLvlInfo = _packLevels[_indexCurrentLevel].GetComponent<LvlControl>();
        //записываем количество банок, которые надо уронить
        _countBankToWin = _getLvlInfo._countBank;

        //включаем необходимое количество шаров
        _countBallsToLoss = (int)_getLvlInfo._countBalls;
        _countLastBallsInt = _countBallsToLoss;
        _countLastBallsUi.text = _countLastBallsInt.ToString();
        for (int i = 0; i < _allBalls.Length; i++)
            _allBalls[i].SetActive(false);
        for (int i = 0; i < _countBallsToLoss; i++)
            _allBalls[i].SetActive(true);

        CancelInvoke("SendLoss");
    }



    public void SenderExtraChance()
    {
        //ads rewared
        AdvertisingHand._advertisingHand._sendAfterFinishedRewared = ExtraChance;
        AdvertisingHand._advertisingHand.SendRewaredAds();
    }


    //Доп 2 удара после просмотра рекламы
    public void ExtraChance()
    {
        //включаем необходимое количество шаров
        _countBallsToLoss = (int)CountBalls.TwoBalls;
        _countLastBallsInt = _countBallsToLoss;
        _countLastBallsUi.text = _countLastBallsInt.ToString();
        for (int i = 0; i < _allBalls.Length; i++)
            _allBalls[i].SetActive(false);
        for (int i = 0; i < (int)CountBalls.TwoBalls; i++)
            _allBalls[i].SetActive(true);

        _countBallsToDownNow = 0;

        _panelQuestionsInGame.SetActive(false);
    }




    //считаем упавшие банки до победы и очки
    public void UpdateScore()
    {
        _currentScore += 3;
        foreach (var item in _uiCurrentScore)
            item.text = _currentScore.ToString();
        if (_currentScore > _bestScore)
        {
            _bestScore = _currentScore;
            PlayerPrefs.SetInt("hittBestScore", _bestScore);

            foreach (var item in _uiBestScore)
                item.text = _bestScore.ToString();
        }

        //Подсчёт банок
        _countBankToDownNow++;
        //Если все банки упали
        if (_countBankToDownNow == _countBankToWin)
        {
            CancelInvoke("SendLoss");
            print("WIN!!!");
            //если какие-то шары остались нетронутыми, начисляем бонусные балы
            //if () GREAT SEND!!!
            _greatTextBeforeNextLvl.SetActive(true);
            Invoke("ToNextLevel", 1f);
        }
            

    }

    void ExtraIncrementScore(int _plusScore)
    {
        _currentScore += _plusScore;
        foreach (var item in _uiCurrentScore)
            item.text = _currentScore.ToString();
        if (_currentScore > _bestScore)
        {
            _bestScore = _currentScore;
            PlayerPrefs.SetInt("hittBestScore", _bestScore);

            foreach (var item in _uiBestScore)
                item.text = _bestScore.ToString();
        }
    }




    //считаем упавшие мячи до поражения
    public void CountingFallenBalls()
    {
        _countBallsToDownNow++;
        if (_countBallsToDownNow == _countBallsToLoss)
        {
            Invoke("SendLoss", 3);
        }
    }

    void SendLoss()
    {
        //Вызов панели поражения, далее либо "ExtraIncrementScore", либо "StartLvlFirst"
        print("LOOOOOSER!!");
        _panelQuestionsInGame.SetActive(true);
    }

    //ПОказываем сколько мячей осталось
    public void DecrementJobsBalls()
    {
        _countLastBallsInt--;
        _countLastBallsUi.text = _countLastBallsInt.ToString();
    }


    public void TransitionMoreGames()
    {
        Application.OpenURL(_refMoreGames);
    }

    public void TransitionPrivacyPolicy()
    {
        Application.OpenURL(_refPrivacyPolicy);
    }

}


public enum CountBalls
{
    TwoBalls = 2,
    FiveBalls = 5,
    NineBalls = 9
}
