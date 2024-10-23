using PlayerStatsController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerController
{
    public class PlayerLocomotion : MonoBehaviour
    {
        CameraHandle cameraHandle;
        PlayerManager playerManager;
        PlayerStats playerStats;
        Transform cameraObject;
        InputHandle inputHandle;
        public Vector3 moveDirection;

        [HideInInspector]
        public Transform myTransform;
        [HideInInspector] 
        public PlayerAnimatorHandle animatorHandle;

        public new Rigidbody rigidbody;
        public GameObject normalCamera;

        public CapsuleCollider characterCollider;
        public CapsuleCollider characterCollisionBlockerCollider;

        [Header("Ground & Air Detection Stats")]
        [SerializeField]
        float groundDetectionRayStartPoint = 0.5f;
        [SerializeField]
        float minimumDistanceNeededToBeginFall = 1f;
        [SerializeField]
        float groundDirectionRayDistance = 0.2f;
        LayerMask ignoreForGroundCheck;
        public float inAirTimer;

        [Header("Stamina Costs")]
        [SerializeField]
        int rollStaminaCost = 15;
        int backStepStaminaCost = 12;
        int sprintStaminaCost = 1;

        [Header("Movement Stats")]
        [SerializeField]
        float walkingSpeed = 3;
        [SerializeField]
        float movementSpeed = 5;
        [SerializeField]
        float sprintSpeed = 7;
        [SerializeField]
        float rotationSpeed = 10;
        [SerializeField]
        float fallingSpeed = 45;

        private void Awake()
        {
            cameraHandle = FindObjectOfType<CameraHandle>();
            playerManager = GetComponent<PlayerManager>();
            rigidbody = GetComponent<Rigidbody>();
            inputHandle = GetComponent<InputHandle>();
            animatorHandle = GetComponentInChildren<PlayerAnimatorHandle>();
            playerStats = GetComponent<PlayerStats>();

        }

        void Start()
        {

            cameraObject = Camera.main.transform;
            myTransform = transform;
            animatorHandle.Initialized();

            playerManager.isGrounded = true;
            ignoreForGroundCheck = ~(1 << 8 | 1 << 11);

            Physics.IgnoreCollision(characterCollider, characterCollisionBlockerCollider, true);
        }

        #region Movement
        Vector3 normalVector = Vector3.up;
        Vector3 targetPosition;

        public void HandleRotation(float delta)
        {
            if (animatorHandle.canRotate)
            {
                if (inputHandle.lockOnFlag && inputHandle.sprintFlag == false)
                {
                    if (inputHandle.sprintFlag || inputHandle.rollFlag)
                    {
                        Vector3 targetDirection = Vector3.zero;
                        targetDirection = cameraHandle.cameraTransform.forward * inputHandle.vertical;
                        targetDirection += cameraHandle.cameraTransform.right * inputHandle.horizontal;
                        targetDirection.Normalize();
                        targetDirection.y = 0;

                        if (targetDirection == Vector3.zero)
                        {
                            targetDirection = transform.forward;
                        }

                        Quaternion tr = Quaternion.LookRotation(targetDirection);
                        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);

                        transform.rotation = targetRotation;
                    }
                    else
                    {
                        Vector3 rotationDirection = moveDirection;
                        rotationDirection = cameraHandle.currentLockOnTarget.position - transform.position;
                        rotationDirection.y = 0;

                        rotationDirection.Normalize();

                        Quaternion tr = Quaternion.LookRotation(rotationDirection);
                        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);
                        transform.rotation = targetRotation;
                    }

                }
                else
                {
                    Vector3 targetDir = Vector3.zero;
                    float moveOverride = inputHandle.moveAmount;

                    targetDir = cameraObject.forward * inputHandle.vertical;
                    targetDir += cameraObject.right * inputHandle.horizontal;

                    targetDir.Normalize();
                    targetDir.y = 0;

                    if (targetDir == Vector3.zero)
                        targetDir = myTransform.forward;

                    float rs = rotationSpeed;

                    Quaternion tr = Quaternion.LookRotation(targetDir);
                    Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);

                    myTransform.rotation = targetRotation;
                }
            }
        }

        public void HandleMovement(float delta)
        {
            
            if (inputHandle.rollFlag)
                return;

            if (playerManager.isInteracting)
                return;

            moveDirection = cameraObject.forward * inputHandle.vertical;
            moveDirection += cameraObject.right * inputHandle.horizontal;
            moveDirection.Normalize();
            moveDirection.y = 0;

            float speed = movementSpeed;

            if(inputHandle.sprintFlag && inputHandle.moveAmount > 0.5)
            {
                speed = sprintSpeed;
                playerManager.isSprinting = true;
                moveDirection *= speed;

                playerStats.ReduceStamina(sprintStaminaCost);
            } else
            {
                if (inputHandle.moveAmount < 0.5f)
                {
                    moveDirection *= walkingSpeed;
                } else
                {
                    moveDirection *= movementSpeed;
                }
                playerManager.isSprinting = false;
            }

            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);

            rigidbody.velocity = projectedVelocity;

            if(inputHandle.lockOnFlag && inputHandle.sprintFlag == false)
            {
                animatorHandle.UpdateAnimatorValues(inputHandle.vertical, inputHandle.horizontal, playerManager.isSprinting);
            } else
            {
                animatorHandle.UpdateAnimatorValues(inputHandle.moveAmount, 0, playerManager.isSprinting);
            }
        }

        public void HandleRollingAndSprinting(float delta)
        {

            if (animatorHandle.anim.GetBool("isInteracting"))
                return;

            if (playerStats.currentStamina <= 0)
                return;

            if (inputHandle.rollFlag)
            {
                moveDirection += cameraObject.forward * inputHandle.vertical;
                moveDirection += cameraObject.right * inputHandle.horizontal;

                
                if (inputHandle.moveAmount > 0)
                {
                    animatorHandle.PlayTargetAnimation("Rolling", true);
                    moveDirection.y = 0;
                    Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                    myTransform.rotation = rollRotation;

                    playerStats.ReduceStamina(rollStaminaCost);
                } else
                {
                    animatorHandle.PlayTargetAnimation("Backstep", true);
                    playerStats.ReduceStamina(backStepStaminaCost);
                }
            }
        }

        public void HandleFalling(float delta, Vector3 moveDirection)
        {
            playerManager.isGrounded = false;
            RaycastHit hit;
            Vector3 origin = myTransform.position;
            origin.y += groundDetectionRayStartPoint;

            if(Physics.Raycast(origin, myTransform.forward, out hit, 0.4f))
            {
                moveDirection = Vector3.zero;
            }

            if(playerManager.isInAir)
            {
                rigidbody.AddForce(-Vector3.up * fallingSpeed);
                rigidbody.AddForce(moveDirection * fallingSpeed / 10f);
            }

            Vector3 dir = moveDirection;
            dir.Normalize();
            origin = origin + dir * groundDirectionRayDistance;

            targetPosition = myTransform.position;

            Debug.DrawRay(origin, -Vector3.up * minimumDistanceNeededToBeginFall, Color.red);
            if(Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall, ignoreForGroundCheck))
            {
                normalVector = hit.normal;
                Vector3 tp = hit.point;
                playerManager.isGrounded = true;
                targetPosition.y = tp.y;

                if(playerManager.isInAir)
                {
                    if(inAirTimer > 0.5f)
                    {
                        Debug.Log("You were in the air for " + inAirTimer);
                        animatorHandle.PlayTargetAnimation("Land", true);
                    } else
                    {
                        animatorHandle.PlayTargetAnimation("Empty", false);
                    }

                    inAirTimer = 0;
                    playerManager.isInAir = false;
                }
            } else
            {
                if(playerManager.isGrounded)
                {
                    playerManager.isGrounded = false;
                }

                if(playerManager.isInAir == false)
                {
                    
                    if(playerManager.isInteracting == false)
                    {
                        animatorHandle.PlayTargetAnimation("Falling", true);
                    }

                    Vector3 vel = rigidbody.velocity;
                    vel.Normalize();
                    rigidbody.velocity = vel * (movementSpeed / 2);
                    playerManager.isInAir = true;
                }
            }

            if(playerManager.isGrounded)
            {
                if(playerManager.isInteracting || inputHandle.moveAmount > 0)
                {
                    myTransform.position = Vector3.Lerp(myTransform.position, targetPosition, Time.deltaTime / 0.1f);
                } else
                {
                    myTransform.position = targetPosition;
                }
            }


        }

        public void HandleJumping(float delta)
        {
            if (playerManager.isInteracting)
                return;

            if (playerStats.currentStamina <= 0)
                return;

            if (inputHandle.jump_input)
            {
                if (inputHandle.moveAmount > 0)
                {
                    moveDirection = cameraObject.forward * inputHandle.vertical;
                    moveDirection += cameraObject.right * inputHandle.horizontal;
                    animatorHandle.PlayTargetAnimation("Jump", true);


                    moveDirection.y = 0;
                    Quaternion jumpRotation = Quaternion.LookRotation(moveDirection);
                    myTransform.rotation = jumpRotation;
                } else
                {
                    animatorHandle.PlayTargetAnimation("Jump", true);
                }

                Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);

                rigidbody.velocity = projectedVelocity;
            }
        }
        #endregion


    }

}
