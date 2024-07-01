using System.Collections;
using StarterAssets;
using TMPro;
using UnityEngine;

public class NoteController : MonoBehaviour
{
    public SO_Note note;
    [SerializeField] private TMP_Text noteText;
    [SerializeField] private GameObject notePanel; 

    private bool _isPickedUp = false;

    private AudioSource _audio;
    [SerializeField] private AudioClip pickupClip, paperFallingClip;

    private FirstPersonController _firstPersonController;
    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _firstPersonController = FindObjectOfType<FirstPersonController>();
    }

    public void ToggleUI()
    {
        if (!_isPickedUp)
        {
            PickupNote();
        }
        else
        {
            DropNote();
        }
        _isPickedUp = !_isPickedUp;
    }

    private void PickupNote()
    {
        notePanel.SetActive(true);
        noteText.text = note.noteText;
        if (_audio && !_audio.isPlaying)
        {
            _audio.PlayOneShot(pickupClip);
        }
        
        //GlobalCursorManager.Instance.EnableCursor();
        _firstPersonController.canMove = false;
        _firstPersonController.canRotate = false;
    }

    private void DropNote()
    {
        notePanel.SetActive(false);
        noteText.text = "";
        if (_audio && !_audio.isPlaying)
        {
            _audio.PlayOneShot(paperFallingClip);
        }
        
        //GlobalCursorManager.Instance.DisableCursor();
        _firstPersonController.canMove = true;
        _firstPersonController.canRotate = true;
    }
}
