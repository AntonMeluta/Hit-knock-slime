using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour {

    GameControl _gameControl;

    private void Start()
    {
        _gameControl = GameObject.FindObjectOfType<GameControl>();   
    }


    private void OnTriggerEnter(Collider other)
    {

        
        if (other.tag == "Banka")
        {
            _gameControl.UpdateScore();
            other.gameObject.SetActive(false);
        }

        if (other.tag == "Ball")
        {
            _gameControl.CountingFallenBalls();
            other.gameObject.SetActive(false);
        }
    }

}


