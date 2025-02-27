using Core;
using GameManager;
using PlayerStatsController;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PlayerController
{
    
    public class PlayerManager : CharacterManager
    {
        public static PlayerManager instance;

        InputHandle inputHandle;
        UIManager uiManager;
        MouseManager mouseManager;
        Animator anim;
        CameraHandle cameraHandle;
        PlayerAnimatorHandle animatorHandle;
        PlayerStats playerStats;
        PlayerLocomotion playerLocomotion;
        public UIAnimationController interactionPopup;
        TMP_Text interactionText;
        
        public GameObject itemPopup;

        public bool isInteracting;
        public bool usingAnimationMove;

        [Header("Player Flags")]
        public bool isSprinting;
        public bool isInAir;
        public bool isJumping;
        public bool isGrounded;
        public bool isOnWall;
        public bool canDoubleJump;
        public bool isClimbable;
        LayerMask lootableMask;
        LayerMask talkableMask;

        private void Awake()
        {
            instance = this;

            inputHandle = GetComponent<InputHandle>();
            anim = GetComponentInChildren<Animator>();
            uiManager = FindObjectOfType<UIManager>();
            mouseManager = FindObjectOfType<MouseManager>();

            cameraHandle = FindObjectOfType<CameraHandle>();
            playerLocomotion = GetComponent<PlayerLocomotion>();
            playerStats = GetComponent<PlayerStats>();
            animatorHandle = GetComponentInChildren<PlayerAnimatorHandle>();

            //get component
            interactionText = interactionPopup.gameObject.GetComponentInChildren<TMP_Text>();

            //setup raycast mask
            lootableMask = 1 << LayerMask.NameToLayer("LootableObject");
            talkableMask = 1 << LayerMask.NameToLayer("TalkableObject");
        }

        private void Update()
        {
            float delta = Time.deltaTime;
            
            //get player flag from animator
            isInteracting = anim.GetBool("isInteracting");
            usingAnimationMove = anim.GetBool("usingAnimationMove"); 
            isJumping = anim.GetBool("isJumping");
            isOnWall = anim.GetBool("isOnWall");
            canDoubleJump = anim.GetBool("canDoubleJump");
            animatorHandle.canRotate = anim.GetBool("canRotate");

            //set player flag in animator 
            anim.SetBool("isInAir", isInAir);
            anim.SetBool("isDead", playerStats.isDead);

            //capture input press event
            inputHandle.TickInput(delta);

            //disable rigidbody when not moving
            playerLocomotion.rigidbody.isKinematic = !(inputHandle.isMoveInputsPressed() || isInAir || isJumping || isOnWall || usingAnimationMove);
            
            //handle input event
            playerLocomotion.HandleJumping(delta);
            playerLocomotion.HandleRollingAndSprinting(delta);
            playerStats.RegenerateStamina();
            CheckForInteractableObject();
            //
        }

        private void FixedUpdate()
        {
            float delta = Time.deltaTime;
            playerLocomotion.HandleMovement(delta);
            playerLocomotion.HandleFalling(delta, playerLocomotion.moveDirection);
            playerLocomotion.HandleStepping(delta);
            playerLocomotion.HandleOnWall(delta);
            playerLocomotion.HandleRotation(delta);
        }

        private void LateUpdate()
        {
            inputHandle.rollFlag = false;
            inputHandle.pick_up = false;
            inputHandle.talk_input = false;
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

        public void ActivateController()
        {
            inputHandle.enabled = true;
            uiManager.hudWindow.Activate();
            mouseManager.HideCursor();
        }

        public void DeactivateController()
        {
            inputHandle.enabled = false;
            uiManager.hudWindow.Deactivate();
            mouseManager.ShowCursor();
        }



        #region Player Interaction

        public void CheckForInteractableObject()
        {
            RaycastHit hit;
            //sphere cast cannot detect collider in overlap sphere
            if(Physics.SphereCast(transform.position - transform.forward * 0.5f, 0.5f, transform.forward, out hit, 1f, talkableMask))
            {
                if (hit.collider.tag == "Talkable")
                {
                    Interactable interactableObject = hit.collider.GetComponent<TalkInteraction>();

                    if(interactableObject != null)
                    {
                        string interactableText = interactableObject.interactableText;
                        //
                        interactionText.text = interactableText;
                        interactionPopup.Activate();

                        if(inputHandle.talk_input)
                        {
                            interactableObject.Interact();
                            
                        }
                    }
                }
            } else
            {
                if (interactionPopup != null)
                {
                    interactionPopup.Deactivate();
                }

                if (itemPopup != null && inputHandle.pick_up)
                {
                    //itemPopup.SetActive(false);
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
