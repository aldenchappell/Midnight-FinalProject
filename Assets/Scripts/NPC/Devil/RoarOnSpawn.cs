using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoarOnSpawn : MonoBehaviour
{
    [SerializeField] AudioClip roar;
    public bool isSpawn;
    // Start is called before the first frame update
    void Start()
    {
        if(isSpawn)
        {
            Invoke("Roar", 3f);
        }
        
        //
    }

    private void Roar()
    {
        GetComponent<AudioSource>().PlayOneShot(roar);
    }

    
}
