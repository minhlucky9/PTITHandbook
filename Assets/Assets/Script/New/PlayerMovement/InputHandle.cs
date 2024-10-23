
using PlayerStatsController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerController
{
    public class InputHandle : MonoBehaviour
    {
        public float horizontal;
        public float vertical;
        public float moveAmount;
        public float mouseX;
        public float mouseY;

        public bool b_input;
        public bool jump_input;
        public bool inventory_input;
        public bool pick_up;
        public bool lock_on_input;

        public bool rollFlag;
        public bool lockOnFlag;
        public bool sprintFlag;
        public float rollInputTimer;
        

        PlayerControls inputActions;
        CameraHandle cameraHandle;
        PlayerManager playerManager;
        PlayerStats playerStats;
        PlayerAnimatorHandle animatorHandle;

        Vector2 movementInput;
        Vector2 cameraInput;

        public Transform criticalAttackRayCastStartPoint;

        private void Start()
        {
            playerManager = GetComponent<PlayerManager>();
            cameraHandle = FindObjectOfType<CameraHandle>();
            animatorHandle = GetComponentInChildren<PlayerAnimatorHandle>();
            playerStats = GetComponent<PlayerStats>();
        }

        public void OnEnable()
        {
            if(inputActions == null)
            {
                inputActions = new PlayerControls();
                //bind input values to parameters
                inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
                inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();

                inputActions.PlayerAction.PickUp.performed += i => pick_up = true;
                inputActions.PlayerAction.Jump.performed += i => jump_input = true;
                inputActions.PlayerAction.Inventory.performed += i => inventory_input = true;

                inputActions.PlayerAction.Roll.performed += i => b_input = true;
                inputActions.PlayerAction.Roll.canceled += i => b_input = false;

                inputActions.PlayerAction.LockOn.performed += i => lock_on_input = true;

            }

            inputActions.Enable();
        }

        public void OnDisable()
        {
            inputActions.Disable();
        }

        public void TickInput(float delta)
        {
            HandleMoveInput(delta);
            HandleRollInput(delta);
            HandleLockOnInput();

        }

        private void HandleMoveInput(float delta)
        {
            horizontal = movementInput.x;
            vertical = movementInput.y;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            mouseX = cameraInput.x;
            mouseY = cameraInput.y;

        }

        private void HandleRollInput(float delta)
        {
            if (b_input)
            {
                rollInputTimer += delta;
                
                if(playerStats.currentStamina <= 0)
                {
                    b_input = false;
                    sprintFlag = false;
                }
                
                if(moveAmount > 0.5f && playerStats.currentStamina > 0)
                {
                    sprintFlag = true;
                }
            } else
            {
                sprintFlag = false;

                if (rollInputTimer > 0 && rollInputTimer < 0.5f)
                {
                    sprintFlag = false;
                    rollFlag = true;
                }

                rollInputTimer = 0;
            }
        }

        private void HandleLockOnInput()
        {
            if(lock_on_input && lockOnFlag == false)
            {
                lock_on_input = false;
                cameraHandle.HandleLockOn();

                if(cameraHandle.nearestLockOnTarget != null)
                {
                    cameraHandle.currentLockOnTarget = cameraHandle.nearestLockOnTarget;
                    lockOnFlag = true;
                }

            } else if(lock_on_input && lockOnFlag)
            {
                lock_on_input = false;
                lockOnFlag = false;
                //Clear lock on target
                cameraHandle.ClearLockOnTarget();
            }

            cameraHandle.SetCameraHeight();
        }

    }
}


