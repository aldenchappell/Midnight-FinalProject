using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerfumeBottleSetActive : MonoBehaviour
{
    private GameObject _shadowBook;
    private bool isChecking;

    
    
    private void Start()
    {
        _shadowBook = GameObject.Find("BookShadow");
        isChecking = true;
    }

    private void Update()
    {
        if(_shadowBook == null && isChecking)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
            isChecking = false;
        }
    }

   
}
