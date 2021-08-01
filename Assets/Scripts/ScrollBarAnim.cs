using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollBarAnim : MonoBehaviour
{
    Scrollbar scrollbar;
    float valueScrollbar;

    public float speedIntrepolate = 10;

    private void Awake()
    {
        scrollbar = GetComponent<Scrollbar>();
    }

    private void OnEnable()
    {
        valueScrollbar = 1;
        scrollbar.value = valueScrollbar;

        StartCoroutine(LerpScrollbarDown());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }


    public void StopAllAnim()
    {
        StopAllCoroutines();
    }


    IEnumerator LerpScrollbarDown()
    {

        while (valueScrollbar >= 0)
        {
            valueScrollbar -= Time.deltaTime / speedIntrepolate;
            scrollbar.value = valueScrollbar;
            yield return null;
        }


        StartCoroutine(LerpScrollbarUp());

    }

    IEnumerator LerpScrollbarUp()
    {
        while (valueScrollbar <= 1)
        {
            valueScrollbar += Time.deltaTime / speedIntrepolate;
            scrollbar.value = valueScrollbar;
            yield return null;
        }

        StartCoroutine(LerpScrollbarDown());

    }


}
