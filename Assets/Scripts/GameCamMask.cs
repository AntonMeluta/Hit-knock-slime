using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamMask : MonoBehaviour {



    private void OnEnable()
    {
        Camera.main.cullingMask = 32;
    }

    private void OnDisable()
    {
        Camera.main.cullingMask = -1;
    }


   


}
