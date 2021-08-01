using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StvorkiAnim : MonoBehaviour {


    public GameObject[] _forEnabled;

    private void OnEnable()
    {
        foreach (var item in _forEnabled)
        {
            item.SetActive(false);
        }
    }

    public void ONENABLEShopObjects()
    {
        foreach (var item in _forEnabled)
        {
            item.SetActive(true);
        }
    }

}
