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
        public bool isInteract;
        public bool usingAnimationMove;
        public bool isTalkingWithNPC = false;

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

        [Header("Assign ở Inspector")]
        [SerializeField] private GameObject maleModel;
        [SerializeField] private GameObject femaleModel;

        private void Awake()
        {
            instance = this;

            // Đọc lựa chọn (mặc định là 1 nếu chưa có)
            int sel = PlayerPrefs.GetInt("SelectedCharacter", 1);

            // Nếu sel == 1 → female, sel == 2 → male
            femaleModel.SetActive(sel == 1);
            maleModel.SetActive(sel == 2);

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

        private bool returnRequested = false;
        private Vector3 returnPosition;
        private Vector3 returnCameraOffset;

       

        public void RequestReturnToSafeZone(Vector3 playerPos, Vector3 camOffset)
        {
            returnRequested = true;
            returnPosition = playerPos;
            returnCameraOffset = camOffset;
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
            if (!inputHandle.enabled)
            {
             //   playerLocomotion.rigidbody.velocity = Vector3.zero;
                anim.enabled = false; 
                return;
            }
            anim.enabled = true;
            float delta = Time.deltaTime;
            playerLocomotion.HandleMovement(delta);
            playerLocomotion.HandleFalling(delta, playerLocomotion.moveDirection);
            playerLocomotion.HandleStepping(delta);
         //   playerLocomotion.HandleOnWall(delta);
            playerLocomotion.HandleRotation(delta);
        }



        private void LateUpdate()
        {
            float delta = Time.deltaTime;

            // Xử lý teleport
            if (returnRequested && cameraHandle != null)
            {
                // Teleport player
                transform.position = returnPosition;

                // Tính toán vị trí camera mới
                Vector3 newCameraPosition = returnPosition + returnCameraOffset;

                // FORCE reset camera position và velocity
                cameraHandle.ForceResetCamera(newCameraPosition);

                // Reset camera components
                cameraHandle.cameraPivotTransform.localPosition = new Vector3(0f, cameraHandle.unlockPivotPosition, 0f);
                cameraHandle.cameraPivotTransform.localRotation = Quaternion.identity;
                cameraHandle.cameraTransform.localPosition = new Vector3(0f, 0f, cameraHandle.defaultPosition);

                returnRequested = false;

                // SKIP camera update trong frame này
                goto SkipCameraUpdate;
            }

            // Camera logic bình thường
            if (cameraHandle != null)
            {
                cameraHandle.FollowTarget(delta);

                if (!isInteracting && !isInteract)
                {
                    cameraHandle.HandleCameraRotation(delta, inputHandle.mouseX, inputHandle.mouseY);
                }
            }

        SkipCameraUpdate:

            // Reset input flags
            inputHandle.rollFlag = false;
            inputHandle.pick_up = false;
            inputHandle.talk_input = false;
            inputHandle.jump_input = false;
            inputHandle.inventory_input = false;

            if (isInAir)
            {
                playerLocomotion.inAirTimer += Time.deltaTime;
            }
        }

        public void ActivateController()
        {
            inputHandle.enabled = true;
            playerLocomotion.enabled = true;
            cameraHandle.enabled = true;
            uiManager.hudWindow.Activate();
            mouseManager.HideCursor();
            isTalkingWithNPC = false;
        }

        public void DeactivateController()
        {
            inputHandle.enabled = false;
            playerLocomotion.enabled = false;  // KHÔNG gọi HandleMovement nữa
            cameraHandle.enabled = false;
            playerLocomotion.rigidbody.velocity = Vector3.zero;
            uiManager.hudWindow.Deactivate();
            mouseManager.ShowCursor();
            isTalkingWithNPC = true;
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
