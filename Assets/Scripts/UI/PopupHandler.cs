using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupHandler : MonoBehaviour
{
    public GameObject popupImage; // Assign the pop-up image GameObject in the Inspector
    public TMP_Text popupText; // Assign the pop-up text GameObject in the Inspector (or TextMeshProUGUI if using TextMeshPro)

    // This method will be called when the mouse enters the area of the background image
    public void OnMouseEnter()
    {
        popupImage.SetActive(true);
        //popupText.text = ""; // Customize this text as needed
    }

    // This method will be called when the mouse exits the area of the background image
    public void OnMouseExit()
    {
        popupImage.SetActive(false);
    }
}
