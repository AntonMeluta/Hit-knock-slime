using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//отслеживает актуальное состояние геймплея, условие победы/поражения
public class GameControl : MonoBehaviour
{
    readonly int numberLevelHard = 13;
    readonly int numberLevelMiddle = 6;
    readonly int multiplyScore = 3;

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

    

    public GameObject _panelQuestionsInGame;
    public GameObject _greatTextBeforeNextLvl;

    
    private void Awake()
    {
        
        _bestScore = PlayerPrefs.GetInt("hittBestScore", 0);
        foreach (var item in _uiBestScore)
            item.text = _bestScore.ToString();
        
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
        

        //обнуление текущих значений упавших банок и шаров
        _countBankToDownNow = 0;
        _countBallsToDownNow = 0;

        //включаем следующий уровень
        _indexCurrentLevel++;
        if (_indexCurrentLevel == _packLevels.Length)
            _indexCurrentLevel = numberLevelHard;

        

        //регулировка частоты показа межстраничной рекламы
        if (_indexCurrentLevel > numberLevelHard + 1)
        {
            if (_indexCurrentLevel % 2 == 1)
                AdvertisingHand.Instance.SendIntersitialAds();
        }
        else if (_indexCurrentLevel >= numberLevelMiddle)
        {
            if (_indexCurrentLevel % numberLevelMiddle == 0)
                AdvertisingHand.Instance.SendIntersitialAds();
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
        AdvertisingHand.Instance._sendAfterFinishedRewared = ExtraChance;
        AdvertisingHand.Instance.SendRewaredAds();
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
        _currentScore += multiplyScore;
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
        int delayToLoss = 3;
        _countBallsToDownNow++;

        if (_countBallsToDownNow == _countBallsToLoss)
        {
            Invoke("SendLoss", delayToLoss);
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


    

}


public enum CountBalls
{
    TwoBalls = 2,
    FiveBalls = 5,
    NineBalls = 9
}
