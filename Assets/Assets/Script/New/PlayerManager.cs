using Core;
using ItemController;
using PlayerStatsController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerController
{
    
    public class PlayerManager : CharacterManager
    {
        InputHandle inputHandle;
        Animator anim;
        CameraHandle cameraHandle;
        PlayerAnimatorHandle animatorHandle;
        PlayerStats playerStats;
        PlayerLocomotion playerLocomotion;
        public GameObject interactionPopup;
        public GameObject itemPopup;

        public bool isInteracting;

        [Header("Player Flags")]
        public bool isSprinting;
        public bool isInAir;
        public bool isGrounded;

        private void Awake()
        {
            inputHandle = GetComponent<InputHandle>();
            anim = GetComponentInChildren<Animator>();
            cameraHandle = FindObjectOfType<CameraHandle>();
            playerLocomotion = GetComponent<PlayerLocomotion>();
            playerStats = GetComponent<PlayerStats>();
            animatorHandle = GetComponentInChildren<PlayerAnimatorHandle>();
        }

        private void Update()
        {
            float delta = Time.deltaTime;
            isInteracting = anim.GetBool("isInteracting");
            animatorHandle.canRotate = anim.GetBool("canRotate");
            anim.SetBool("isInAir", isInAir);
            anim.SetBool("isDead", playerStats.isDead);

            inputHandle.TickInput(delta);
            playerLocomotion.HandleJumping(delta);
            playerLocomotion.HandleRollingAndSprinting(delta);
            playerStats.RegenerateStamina();

            CheckForInteractableObject();
        }

        private void FixedUpdate()
        {
            float delta = Time.deltaTime;

            playerLocomotion.HandleMovement(delta);
            playerLocomotion.HandleFalling(delta, playerLocomotion.moveDirection);
            playerLocomotion.HandleRotation(delta);
        }

        private void LateUpdate()
        {
            inputHandle.rollFlag = false;
            inputHandle.pick_up = false;
            inputHandle.jump_input = false;
            inputHandle.inventory_input = false;

            float delta = Time.deltaTime;

            if (cameraHandle != null)
            {
                cameraHandle.FollowTarget(delta);
                cameraHandle.HandleCameraRotation(delta, inputHandle.mouseX, inputHandle.mouseY);
            }

            if (isInAir)
            {
                playerLocomotion.inAirTimer += Time.deltaTime;
            }
        }

        #region Player Interaction

        public void CheckForInteractableObject()
        {
            RaycastHit hit;

            if(Physics.SphereCast(transform.position, 0.3f, transform.forward, out hit, 1f, cameraHandle.ignoreLayers))
            {
                
                if (hit.collider.tag == "Interactable")
                {
               
                    Interactable interactableObject = hit.collider.GetComponent<Interactable>();

                    if(interactableObject != null)
                    {
                        string interactableText = interactableObject.interactableText;
                        //
                        //interactableUI.interactableText.text = interactableText;
                        interactionPopup.SetActive(true);

                        if(inputHandle.pick_up)
                        {
                            interactableObject.Interact(this);
                        }
                    }
                }
            } else
            {
                if(interactionPopup != null)
                {
                    interactionPopup.SetActive(false);
                }

                if(itemPopup != null && inputHandle.pick_up)
                {
                    itemPopup.SetActive(false);
                }

            }
        }

        public void OpenChestInteraction(Transform playerStandingPosition)
        {
            //playerLocomotion.rigidbody.velocity = Vector3.zero;
            transform.position = playerStandingPosition.position;
            //

        }
        #endregion
    }

}
