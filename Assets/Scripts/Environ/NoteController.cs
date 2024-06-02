using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class NoteController : MonoBehaviour
{
    public SO_Note note;
    private TMP_Text _noteText;

    //where the note will be placed when picked up and parented to.
    private Transform _pickupPosition;
    //rotation of the note when dropped
    private Transform _dropPosition;

    private Rigidbody _rigidBody;
    private bool _isPickedUp = false;

    // Speed at which the note moves to the pickup position and rotation
    [SerializeField] private float pickupSpeed = 5f;

    private AudioSource _audio;
    [SerializeField] private AudioClip pickupClip, paperFallingClip;
    private void Awake()
    {
        _noteText = GetComponentInChildren<TMP_Text>();
        _rigidBody = GetComponent<Rigidbody>();
        _audio = GetComponent<AudioSource>();

        _pickupPosition = GameObject.Find("NotePickupPosition").transform;
        _dropPosition = GameObject.Find("NoteDropPosition").transform;

        _rigidBody.isKinematic = true;
    }

    private void Update()
    {
        if (note == null) return;

        if (_noteText == null) return;

        // Check if the note is picked up and F is pressed to drop it
        if (_isPickedUp && Input.GetKeyDown(KeyCode.F))
        {
            DropNote();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        _rigidBody.isKinematic = false;

        _audio.enabled = false;
    }

    public void PickupNote()
    {
        if (_isPickedUp) return; 

        _rigidBody.isKinematic = true; 

        _noteText.text = note.noteText;

        // Lerping position and rotation
        StartCoroutine(LerpToPositionAndRotation(transform, _pickupPosition, pickupSpeed));

        _isPickedUp = true;

        if (_audio)
        {
            _audio.PlayOneShot(pickupClip);
        }
    }

    private void DropNote()
    {
        transform.SetParent(null);

        transform.rotation = _dropPosition.rotation;

        _rigidBody.isKinematic = false;

        _noteText.text = "";

        _isPickedUp = false; 
        
        if (_audio)
        {
            _audio.PlayOneShot(paperFallingClip);
        }
    }

    // Coroutine to lerp position and rotation
    private IEnumerator LerpToPositionAndRotation(Transform pos, Transform targetPos, float speed)
    {
        float lerpTime = 0f;
        Vector3 startPos = pos.position;
        Quaternion startRot = pos.rotation;

        while (lerpTime < 1f)
        {
            lerpTime += Time.deltaTime * speed;
            pos.position = Vector3.Lerp(startPos, targetPos.position, lerpTime);
            pos.rotation = Quaternion.Lerp(startRot, targetPos.rotation, lerpTime);
            pos.SetParent(targetPos);
            yield return null;
        }
    }
}
