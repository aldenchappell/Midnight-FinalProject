using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactivateDeathUI : MonoBehaviour
{
    private PlayerDeathController _deathController;

    private void Awake()
    {
        _deathController = GameObject.FindObjectOfType<PlayerDeathController>();
    }

    private void OnDisable()
    {
        print("Being Disabled");
        if(_deathController.isDying)
        {
            print("Reenabling");
            Reenable();
        }
    }

    private void Reenable()
    {
        gameObject.SetActive(true);
    }
}
