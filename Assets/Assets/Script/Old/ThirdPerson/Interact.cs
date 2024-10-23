using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class Interact : MonoBehaviour
{
    public scr_CameraController CameraToggle;
    public scr_CameraController CameraToggle2;
    public scr_PlayerController MovementToggle;
    public scr_PlayerController MovementToggle2;
    public Character3D_Manager_Ingame character;

    public GameObject interactionUI; // UI element for interaction prompt

    private PlayerInput playerInput;
    private InputAction interactAction;
    public IIInteractable currentInteractable; // The object in trigger range

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        // Get the "Interact" action from Action Map "Actions"
        interactAction = playerInput.actions.FindAction("Interact", true);
    }

    private void OnEnable()
    {
        // Register the interaction handler
        interactAction.performed += OnInteract;
    }

    private void OnDisable()
    {
        // Unregister the interaction handler
        interactAction.performed -= OnInteract;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IIInteractable interactable))
        {
            currentInteractable = interactable;
            if (interactionUI != null)
            {
                interactionUI.SetActive(true); // Show UI when in range
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IIInteractable interactable) && interactable == currentInteractable)
        {
            currentInteractable = null;
            if (interactionUI != null)
            {
                interactionUI.SetActive(false); // Hide UI when out of range
            }
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (currentInteractable != null)
        {
            currentInteractable.Interact();

            if (character.index == 0)
            {
                MovementToggle.isCheck = false;
                CameraToggle.isCheck = false;
            }
            else
            {
                MovementToggle2.isCheck = false;
                CameraToggle2.isCheck = false;
            }

            print("Interacting with object");
        }
    }
    // Place this at the top of the script or in a separate file
    public interface IIInteractable
    {
        void Interact();
    }


}
