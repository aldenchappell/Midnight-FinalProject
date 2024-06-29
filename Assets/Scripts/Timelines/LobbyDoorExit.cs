using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyDoorExit : MonoBehaviour
{
    [SerializeField] private GameObject outroCutscene;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayOutroCutscene()
    {
        
        outroCutscene.SetActive(true);

    }
}
