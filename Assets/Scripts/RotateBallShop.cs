using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBallShop : MonoBehaviour {



    private void Update()
    {
        transform.Rotate(new Vector3(0, 20 * Time.deltaTime, 0));
    }


}
