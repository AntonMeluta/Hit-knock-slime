using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectBalls : MonoBehaviour {


    [SerializeField]MeshRenderer _rendererBall;
    int _indexBallSkin;
    GameControl _gc;
    bool _isPermissionToTransitionToGame;

    public Material[] _packMaterials;
    public GameObject _lockObject;
    public GameObject _questionsToPlayer;
    public GameObject _3dVisual;


    private void Awake()
    {
        

        _gc = GameObject.FindObjectOfType<GameControl>();
        //_rendererBall = GetComponentInChildren<MeshRenderer>();

        _indexBallSkin = 0;
        PlayerPrefs.SetInt("ballCheckSki1nn1" + _indexBallSkin, 1);
        _rendererBall.material = _packMaterials[_indexBallSkin];

        _lockObject.SetActive(false);
        _isPermissionToTransitionToGame = true;
    }


    public void SwithToRight()
    {
        _indexBallSkin++;
        if (_indexBallSkin == _packMaterials.Length)
            _indexBallSkin = 0;
        _rendererBall.material = _packMaterials[_indexBallSkin];

        int _checkStatusSkin = PlayerPrefs.GetInt("ballCheckSki1nn1" + _indexBallSkin, 0);
        if (_checkStatusSkin == 0)
        {
            _lockObject.SetActive(true);
            _isPermissionToTransitionToGame = false;
        }
        else
        {
            _lockObject.SetActive(false);
            _isPermissionToTransitionToGame = true;
            
        }
    }


    public void SwithToLeft()
    {
        _indexBallSkin--;
        if (_indexBallSkin < 0)
            _indexBallSkin = _packMaterials.Length - 1;
        _rendererBall.material = _packMaterials[_indexBallSkin];

        int _checkStatusSkin = PlayerPrefs.GetInt("ballCheckSki1nn1" + _indexBallSkin, 0);
        if (_checkStatusSkin == 0)
        {
            _lockObject.SetActive(true);
            _isPermissionToTransitionToGame = false;
        }
        else
        {
            _lockObject.SetActive(false);
            _isPermissionToTransitionToGame = true;
        }
    }


    public void PlayerWantSToGame()
    {
        if (_isPermissionToTransitionToGame)
        {
            _gc._materialForBalls = _packMaterials[_indexBallSkin];
            _gc.ToGameWindow();
        }
        else
        {
            //ОТКРЫВАЕМ ОКНО С ВОПРОСОМ ОБ ОТКРЫТИИ ЭЛЕМЕНТА
            _questionsToPlayer.SetActive(true);
            _3dVisual.SetActive(false);
        }
    }

    public void ToStartRewared()
    {
        AdvertisingHand._advertisingHand._sendAfterFinishedRewared = OpeningBall;
        AdvertisingHand._advertisingHand.SendRewaredAds();
    }


    void OpeningBall()
    {
        _questionsToPlayer.SetActive(false);
        _3dVisual.SetActive(true);

        PlayerPrefs.SetInt("ballCheckSki1nn1" + _indexBallSkin, 1);

        _lockObject.SetActive(false);
        _isPermissionToTransitionToGame = true;


    }


}
