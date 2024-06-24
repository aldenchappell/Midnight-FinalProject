using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicBoxPuzzle : MonoBehaviour
{ 
    private Animator _animator;
    private void Awake()
    {
        _animator = transform.GetComponentInChildren<Animator>();
    }


    public void AnimateMusicBox()
    {
        gameObject.transform.GetChild(2).gameObject.SetActive(false);
        _animator.SetTrigger("PlayMusicBox");
        Invoke("LoopAnimation", 4.04f);
        Invoke("InitiateKeySpawn", 4.04f);

    }
    private void LoopAnimation()
    {
        _animator.SetTrigger("PlayMusicBox");
    }

    public void InitiateKeySpawn()
    {
        StartCoroutine(SpawnKey());
    }

    private IEnumerator SpawnKey()
    {
        print("Spawning key");
        yield return new WaitForSeconds(4.0f);
       
        GetComponent<Puzzle>().CompletePuzzle();
        GetComponent<Puzzle>().onPuzzleCompletion.Invoke();
        LevelCompletionManager.Instance.OnKeySpawn();
    }
}
