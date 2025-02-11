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

        [Header("Jump and Falling")]
        public float fallingVelocity = 0;
        public float gravity = 15f;
        public float stepHeight = 0.4f;
        public float jumpVelocity = 7f;
        Vector3 wallNormal;

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
        float currentRunningSpeed = 0;

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
            ignoreForGroundCheck = ~(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("CharacterColliderBlocker"));

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

            if(playerManager.isInAir)
            {
                if(moveDirection.magnitude > 0.1f)
                {
                    float rs = rotationSpeed;
                    Quaternion tr = Quaternion.LookRotation(moveDirection.normalized);
                    Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);
                    myTransform.rotation = targetRotation;
                }
            }

            if(playerManager.isGrounded)
            {
                float rs = rotationSpeed;
                Quaternion tr = Quaternion.Euler(0, myTransform.rotation.eulerAngles.y, 0);
                Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);
                myTransform.rotation = targetRotation;
            }
        }

        public void HandleMovement(float delta)
        {
            if (playerManager.usingAnimationMove)
                return;

            if (inputHandle.rollFlag)
                return;

            if (playerManager.isInteracting)
                return;

            moveDirection = cameraObject.forward * inputHandle.vertical;
            moveDirection += cameraObject.right * inputHandle.horizontal;
            moveDirection.Normalize();
            moveDirection.y = 0;

            currentRunningSpeed = movementSpeed;

            if (inputHandle.sprintFlag && inputHandle.moveAmount > 0.5)
            {
                currentRunningSpeed = sprintSpeed;
                playerManager.isSprinting = true;
                playerStats.ReduceStamina(sprintStaminaCost);
            }
            else
            {
                if (inputHandle.moveAmount < 0.5f)
                {
                    currentRunningSpeed = walkingSpeed;
                }
                else
                {
                    currentRunningSpeed = movementSpeed;
                }
                playerManager.isSprinting = false;
            }

            moveDirection *= CalculateRunningSpeed(currentRunningSpeed);

            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            SetRigidbodyVelocity(projectedVelocity);



            if (inputHandle.lockOnFlag && inputHandle.sprintFlag == false)
            {
                animatorHandle.UpdateAnimatorValues(inputHandle.vertical, inputHandle.horizontal, playerManager.isSprinting);
            }
            else
            {
                animatorHandle.UpdateAnimatorValues(inputHandle.moveAmount, 0, playerManager.isSprinting);
            }
        }

        public void HandleStepping(float delta)
        {
            if (playerManager.usingAnimationMove)
                return;

            if (inputHandle.rollFlag)
                return;

            if (playerManager.isInteracting)
                return;

            if (playerManager.isInAir || playerManager.isJumping)
                return;

            RaycastHit hitLower45;
            Vector3 startTestStepPosition = transform.position;
            float maxDistance = 0.3f;

            Vector3 foward = moveDirection.normalized;
            if (Physics.Raycast(startTestStepPosition, foward, out hitLower45, maxDistance + 0.3f, cameraHandle.ignoreLayers))
            {
                RaycastHit hitUpper45;
                if (!Physics.Raycast(startTestStepPosition + new Vector3(0, stepHeight, 0), foward, out hitUpper45, maxDistance + 0.3f, cameraHandle.ignoreLayers))
                {
                    Vector3 move = moveDirection.normalized + Vector3.up;
                    SetRigidbodyVelocity(CalculateRunningSpeed(currentRunningSpeed) * move.normalized);
                    return;
                }
            }

            Vector3 side = Vector3.Cross(transform.up, moveDirection.normalized);
            Vector3 left = (foward + side).normalized;
            if (Physics.Raycast(startTestStepPosition, left, out hitLower45, maxDistance, cameraHandle.ignoreLayers))
            {
                RaycastHit hitUpper45;
                if (!Physics.Raycast(startTestStepPosition + new Vector3(0, stepHeight, 0), left, out hitUpper45, maxDistance, cameraHandle.ignoreLayers))
                {
                    Vector3 move = moveDirection.normalized + Vector3.up;
                    SetRigidbodyVelocity(CalculateRunningSpeed(currentRunningSpeed) * move.normalized);
                    return;
                }
            }

            Vector3 right = (foward - side).normalized;
            if (Physics.Raycast(startTestStepPosition, right, out hitLower45, maxDistance, cameraHandle.ignoreLayers))
            {
                RaycastHit hitUpper45;
                if (!Physics.Raycast(startTestStepPosition + new Vector3(0, stepHeight, 0), right, out hitUpper45, maxDistance, cameraHandle.ignoreLayers))
                {

                    Vector3 move = moveDirection.normalized + Vector3.up;
                    SetRigidbodyVelocity(CalculateRunningSpeed(currentRunningSpeed) * move.normalized);
                    return;
                }
            }
        }

        public void HandleRollingAndSprinting(float delta)
        {
            if (playerManager.usingAnimationMove)
                return;

            if (playerManager.isInteracting)
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
                }
                else
                {
                    animatorHandle.PlayTargetAnimation("Backstep", true);
                    animatorHandle.anim.SetBool("usingAnimationMove", true);
                    playerStats.ReduceStamina(backStepStaminaCost);
                }
            }
        }

        public void HandleFalling(float delta, Vector3 moveDirection)
        {
            if (playerManager.usingAnimationMove)
                return;

            if (playerManager.isOnWall)
                return;

            playerManager.isGrounded = false;
            RaycastHit hit;
            Vector3 origin = myTransform.position;
            origin.y += groundDetectionRayStartPoint;

            LayerMask ignoreRaycastMask = (LayerMask.GetMask("Environment"));
            if (Physics.SphereCast(origin, groundDetectionRayStartPoint, moveDirection.normalized, out hit, 0.4f, ignoreRaycastMask))
            {
                moveDirection = Vector3.zero;

                bool climbable = (hit.collider.gameObject.layer == cameraHandle.environmentLayer);

                if (playerManager.isJumping && playerManager.isInAir && climbable)
                {
                    //start climp wall
                    animatorHandle.StartClimbing();
                    wallNormal = hit.normal;
                }
            }
            //change velocity of rigidbody
            Vector3 velocity = moveDirection.normalized * CalculateRunningSpeed(currentRunningSpeed) - fallingVelocity * Vector3.up;

            if (playerManager.isJumping)
            {
                //velocity += moveDirection.normalized * currentRunningSpeed;
                velocity += Vector3.up * jumpVelocity;
            }

            SetRigidbodyVelocity(velocity);

            Vector3 dir = moveDirection;
            dir.Normalize();
            origin = origin + Vector3.up * groundDirectionRayDistance;
            targetPosition = myTransform.position;

            //start falling
            if (velocity.y < 0.1f)
            {
                if (Physics.SphereCast(origin, 0.1f, -Vector3.up, out hit, minimumDistanceNeededToBeginFall, ignoreForGroundCheck))
                {
                    normalVector = hit.normal;
                    Vector3 tp = hit.point;
                    targetPosition.y = tp.y;

                    playerManager.isGrounded = true;

                    if (playerManager.isInAir)
                    {
                        if (inAirTimer > 0.1f)
                        {
                            Debug.Log("You were in the air for " + inAirTimer);
                            animatorHandle.PlayTargetAnimation("Land", true);
                        }
                        else
                        {
                            animatorHandle.PlayTargetAnimation("Empty", false);
                        }

                        inAirTimer = 0;
                        playerManager.isInAir = false;
                        this.moveDirection = Vector3.zero;
                        animatorHandle.StopJumping();
                        animatorHandle.StopDoubleJump();
                    }
                }
                else
                {
                    if (playerManager.isGrounded)
                    {
                        playerManager.isGrounded = false;
                    }

                    if (playerManager.isInAir == false)
                    {

                        if (playerManager.isInteracting == false)
                        {
                            animatorHandle.PlayTargetAnimation("Falling", true);
                        }

                        playerManager.isInAir = true;
                    }
                }
            }


            if (playerManager.isGrounded)
            {
                if (playerManager.isInteracting || inputHandle.moveAmount > 0)
                {
                    myTransform.position = Vector3.Lerp(myTransform.position, targetPosition, Time.deltaTime / 0.1f);
                }
                else
                {
                    myTransform.position = targetPosition;
                }

                if(!playerManager.isJumping) 
                    fallingVelocity = 0;
            }
            else
            {
                fallingVelocity += gravity * Time.deltaTime;
            }

        }

        public void HandleOnWall(float delta)
        {
            if(playerManager.isOnWall)
            {
                if(inAirTimer > 0.6f)
                {
                    animatorHandle.StopClimbing();
                }
            }
        }

        public void HandleJumping(float delta)
        {
            if (playerManager.usingAnimationMove)
                return;

            if (playerManager.isInteracting && !playerManager.isOnWall && !playerManager.canDoubleJump)
                return;

            if (playerStats.currentStamina <= 0)
                return;

            if (inputHandle.jump_input)
            {
                if(playerManager.isOnWall)
                {
                    animatorHandle.StopClimbing();
                    animatorHandle.PlayTargetAnimation("Jump", true);
                    moveDirection = Vector3.Reflect(moveDirection, wallNormal);
                    animatorHandle.StartJumping();
                    fallingVelocity = 0;
                    return;
                }

                if(playerManager.canDoubleJump)
                {
                    animatorHandle.PlayTargetAnimation("Rolling", true);
                    animatorHandle.StartJumping();
                    fallingVelocity = 0;
                    return;
                }

                if (inputHandle.moveAmount > 0)
                {
                    moveDirection = cameraObject.forward * inputHandle.vertical;
                    moveDirection += cameraObject.right * inputHandle.horizontal;
                    animatorHandle.PlayTargetAnimation("Jump", true);

                    moveDirection.y = 0;
                    moveDirection.Normalize();
                    Quaternion jumpRotation = Quaternion.LookRotation(moveDirection);
                    myTransform.rotation = jumpRotation;

                    animatorHandle.StartJumping();
                }
                else
                {
                    animatorHandle.PlayTargetAnimation("Jump", true);
                    moveDirection = Vector3.zero;
                    animatorHandle.StartJumping();
                    
                }
            }
        }

        #endregion

        public float CalculateRunningSpeed(float speed)
        {
            return speed * playerStats.speedMultiplier;
        }

        public void SetRigidbodyVelocity(Vector3 velocity)
        {
            if (!rigidbody.isKinematic)
                rigidbody.velocity = velocity;
        }
    }

}
