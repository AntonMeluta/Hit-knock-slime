using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBallShop : MonoBehaviour {

    [SerializeField]int speedRotate = 20;

    private void Update()
    {
        transform.Rotate(new Vector3(0, speedRotate * Time.deltaTime, 0));
    }


}
