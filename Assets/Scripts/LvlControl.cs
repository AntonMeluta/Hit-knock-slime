using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LvlControl : MonoBehaviour {

    BankaController[] _allBanksChildren;
    //GameObject[] _allBanksObjectsChildren;

    [HideInInspector]public int _countBank;
    public CountBalls _countBalls;


    private void Awake()
    {

        _allBanksChildren = GetComponentsInChildren<BankaController>();
        _countBank = _allBanksChildren.Length;
        /*_allBanksObjectsChildren = new GameObject[_allBanksChildren.Length];
        for (int i = 0; i < _allBanksChildren.Length; i++)        
            _allBanksObjectsChildren[i] = _allBanksChildren[i].gameObject;*/
        
    }

    private void OnEnable()
    {
        foreach (var item in _allBanksChildren)
            item.gameObject.SetActive(true);
        
    }

    
}
