using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamMask : MonoBehaviour {

    int maskCamGameplay = -1;
    int maskCamMenu = 32;

    private void OnEnable()
    {
        Camera.main.cullingMask = maskCamMenu;
    }

    private void OnDisable()
    {
        Camera.main.cullingMask = maskCamGameplay;
    }


   


}
