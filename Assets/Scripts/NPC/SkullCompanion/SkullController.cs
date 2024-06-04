using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SkullController : MonoBehaviour
{
    private Rigidbody _rb;
    private Transform _dropPosition;
    [SerializeField] private GameObject fireFX;

    //NOTE: 
    //WHEN PICKING UP THE SKULL IS FULLY SET UP, ADD THE RIGIDBODY COMPONENT->bool isKinematic TO A NEW 
    //INTERACTION EVENT, AND DISABLE THE CHECK BOX!!
    //END NOTE
    
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true;
        _dropPosition = GameObject.Find("ObjectDropPosition").transform;
    }

    private void Update()
    {
        if (InGameSettingsManager.Instance.enableDroppingItems &&
            Input.GetKeyDown(InGameSettingsManager.Instance.dropCurrentItem) &&
            fireFX.activeSelf)
        {
            DropSkull();
            ToggleFire();
        }
    }

    private void ToggleFire()
    {
        fireFX.SetActive(!fireFX.activeSelf);
    }

    public void DropSkull()
    {
        ToggleFire();
        
        transform.SetParent(null);

        transform.rotation = _dropPosition.rotation;

        _rb.isKinematic = false;
    }
}
