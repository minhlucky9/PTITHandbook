using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotifCollider : MonoBehaviour
{
    public GameObject Collider;

    [Header("MovementFreezee")]
    public scr_CameraController CameraToggle;
    public scr_CameraController CameraToggle2;
    public scr_PlayerController MovementToggle;
    public scr_PlayerController MovementToggle2;
    public Character3D_Manager_Ingame character;
    public MouseManager MouseManager;
    public ButtonActivator ButtonActivator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(DelayedTrigger());
        }
    }

    private IEnumerator DelayedTrigger()
    {
        yield return new WaitForSeconds(1f); // Wait for 1 second
        Collider.SetActive(true);
        MouseManager.ShowCursor();
        ButtonActivator.IsUIShow = true;
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
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Collider.SetActive(false);
        }
    }
}
