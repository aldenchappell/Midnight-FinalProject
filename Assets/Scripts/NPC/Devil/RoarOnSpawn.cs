using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoarOnSpawn : MonoBehaviour
{
    [SerializeField] AudioClip roar;
    // Start is called before the first frame update
    void Start()
    {
        if(GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Summoning"))
        {
            
        }
        Invoke("Roar", 3f);
        //
    }

    private void Roar()
    {
        GetComponent<AudioSource>().PlayOneShot(roar);
    }

    
}
