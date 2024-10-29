using Core;
using Interaction;
using PlayerController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkInteraction : Interactable
{
    float rotationSpeed = 5f;
    PlayerManager playerManager;
    [HideInInspector]public ConservationManager conservationManager;
    Quaternion originalRotation;

    public virtual void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("TalkableObject");
        gameObject.tag = "Talkable";
        interactableText = "Press E to talk to NPC";

        originalRotation = transform.rotation;
        playerManager = FindObjectOfType<PlayerManager>();
        conservationManager = FindObjectOfType<ConservationManager>();
    }

    public virtual void Update()
    {
        if (isInteracting)
        {
            Quaternion tr = Quaternion.LookRotation(playerManager.transform.position - transform.position);
            HandleRotation(tr);
        }
        else
        {
            HandleRotation(originalRotation);
        }
    }

    public override void Interact()
    {
        base.Interact();
        isInteracting = true;
        //start conservation
        StartCoroutine(StartConservation());
    }

    public override void StopInteract()
    {
        base.StopInteract();
        isInteracting = false;
        //Stop conservation
        StartCoroutine(StopConservation());
    }

    void HandleRotation(Quaternion quaternion)
    {
        if(Quaternion.Angle(quaternion, transform.rotation) > 0.01f)
        {
            float rs = rotationSpeed;
            Quaternion tr = quaternion;
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rs * Time.deltaTime);
            transform.rotation = targetRotation;
        } else
        {
            transform.rotation = quaternion;
        }
    }

    IEnumerator StartConservation()
    {
        playerManager.DeactivateController();
        yield return new WaitForSeconds(0.5f);
        yield return conservationManager.ActivateConservationDialog();
    }

    IEnumerator StopConservation()
    {
        yield return conservationManager.DeactivateConservationDialog();
        yield return new WaitForSeconds(0.7f);
        playerManager.ActivateController();

    }
}
