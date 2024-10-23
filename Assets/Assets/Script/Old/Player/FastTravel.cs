using System.Collections;
using UnityEngine;

public class FastTravel : MonoBehaviour
{
    public GameObject fasttravelUI;    // UI element to display for fast travel options
    public Transform[] travelPos;      // Array of positions to travel to
    public GameObject player;          // Player character GameObject
    public GameObject player2;
    CharacterController characterController;
    private scr_PlayerController charControl;
    CharacterController characterController2;
    private scr_PlayerController charControl2;

    public Character3D_Manager_Ingame Character3D;

    private void Start()
    {
        fasttravelUI.SetActive(false);
        charControl = player.GetComponent<scr_PlayerController>();
        characterController = player.GetComponent<CharacterController>();

        charControl2 = player2.GetComponent<scr_PlayerController>();
        characterController2 = player2.GetComponent<CharacterController>();
    }

    // Method to activate the fast travel UI
    public void ShowFastTravelUI()
    {
       // Cursor.visible = true;
       // Cursor.lockState = CursorLockMode.None;
        fasttravelUI.SetActive(true);
    }

    // Method to hide the fast travel UI
    public void HideFastTravelUI()
    {
        fasttravelUI.SetActive(false);
    }

    // Method to move the player to the selected position
    public void TravelTo(int posIndex)
    {
        if (Character3D.index == 0)
        {
            charControl2.enabled = false; // Vô hi?u hóa t?m th?i
            characterController2.enabled = false; // T?t CharacterController
            fasttravelUI.SetActive(true);

            // ??t v? trí chính xác
            player2.transform.position = travelPos[posIndex].position;

            // Reset tr?ng thái t??ng tác
            Interact interactScript = player2.GetComponent<Interact>();
            if (interactScript != null && interactScript.currentInteractable != null)
            {
                interactScript.currentInteractable = null; // ??t l?i tr?ng thái t??ng tác
                if (interactScript.interactionUI != null)
                {
                    interactScript.interactionUI.SetActive(false); // T?t UI khi không còn trong trigger
                }
            }

            characterController2.enabled = true; // Kích ho?t l?i CharacterController
            charControl2.enabled = true; // Kích ho?t l?i ?i?u khi?n nhân v?t
            StartCoroutine(Loading());
        }
        else
        {
            charControl.enabled = false; // Vô hi?u hóa t?m th?i
            characterController.enabled = false; // T?t CharacterController
            fasttravelUI.SetActive(true);

            // ??t v? trí chính xác
            player.transform.position = travelPos[posIndex].position;

            // Reset tr?ng thái t??ng tác
            Interact interactScript = player.GetComponent<Interact>();
            if (interactScript != null && interactScript.currentInteractable != null)
            {
                interactScript.currentInteractable = null; // ??t l?i tr?ng thái t??ng tác
                if (interactScript.interactionUI != null)
                {
                    interactScript.interactionUI.SetActive(false); // T?t UI khi không còn trong trigger
                }
            }

            characterController.enabled = true; // Kích ho?t l?i CharacterController
            charControl.enabled = true; // Kích ho?t l?i ?i?u khi?n nhân v?t
            StartCoroutine(Loading());
        }
    }


    // Coroutine to re-enable player control after a delay
    IEnumerator Loading()
    {
        yield return new WaitForSeconds(3);
      //  charControl.enabled = true;
        fasttravelUI.SetActive(false);
    }
}
