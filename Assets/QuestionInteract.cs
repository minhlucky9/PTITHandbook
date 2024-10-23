using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static Interact;

public class QuestionInteract : MonoBehaviour, IIInteractable
{
    public GameObject interactionUI;
    public GameObject UI;
    public GameObject targetGameObject;
    public GameObject targetGameObject2;
    public Character3D_Manager_Ingame character;
    public scr_CameraController CameraToggle;
    public scr_CameraController CameraToggle2;
    public scr_PlayerController MovementToggle;
    public scr_PlayerController MovementToggle2;

    public ButtonActivator ButtonActivator;
    public MouseManager MouseManager;
    Interact interactor;

    public void Interact()
    {
        if (interactionUI != null)
        {
            interactionUI.SetActive(true);
            UI.SetActive(false);
            MouseManager.ShowCursor();
            ButtonActivator.IsUIShow = true;
        }
        else
        {
            Debug.LogWarning("Interaction UI not set on " + gameObject.name);
        }

        // Gọi Coroutine để trì hoãn việc tắt script
      //  StartCoroutine(DisableScriptAfterDelay(1.25f));
    }

    public void EndInteraction()
    {
        scr_PlayerController script = targetGameObject.GetComponent<scr_PlayerController>();
        scr_PlayerController script2 = targetGameObject2.GetComponent<scr_PlayerController>();
        if (script != null)
        {
            if (character.index == 0)
            {
                MovementToggle.isCheck = true;
                CameraToggle.isCheck = true;
            }
            else
            {
                MovementToggle2.isCheck = true;
                CameraToggle2.isCheck = true;
            }
            MouseManager.HideCursor();
            ButtonActivator.IsUIShow = false;
            Debug.Log("Script đã được bật.");
        }
        else
        {
            Debug.LogWarning("Không tìm thấy script trên game object.");
        }
    }
    public void EnTerInteraction()
    {
        scr_PlayerController script = targetGameObject.GetComponent<scr_PlayerController>();
        scr_PlayerController script2 = targetGameObject2.GetComponent<scr_PlayerController>();
        if (script != null)
        {
            if (character.index == 0)
            {
                MovementToggle.isCheck =false;
                CameraToggle.isCheck = false;
            }
            else
            {
                MovementToggle2.isCheck = false;
                CameraToggle2.isCheck = false;
            }
            MouseManager.ShowCursor();
            ButtonActivator.IsUIShow = true;
        }
        else
        {
            Debug.LogWarning("Không tìm thấy script trên game object.");
        }
    }

    private IEnumerator DisableScriptAfterDelay(float delay)
    {
        // Đợi trong 1.25 giây
        yield return new WaitForSeconds(delay);

        // Thực hiện việc tắt script
        scr_PlayerController script = targetGameObject.GetComponent<scr_PlayerController>();
        scr_PlayerController script2 = targetGameObject2.GetComponent<scr_PlayerController>();
        if (script != null)
        {
            if( character.index == 0 )
            {
                script2.enabled = false;
            }
            else
            {
                script.enabled = false;
            }
          
            Debug.Log("Script đã được tắt.");
        }
        else
        {
            Debug.LogWarning("Không tìm thấy script trên game object.");
        }
    }
}
