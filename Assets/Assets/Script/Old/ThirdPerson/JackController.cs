using UnityEngine;
using System.Collections;
using static Models;
using TMPro;
using System.Collections.Generic;



public class JackController : MonoBehaviour
{
    CharacterController characterController;
    Animator characterAnimator;
    PlayerInputActions playerInputActions;
    //[HideInInspector]
    public Vector2 input_Movement;
    [HideInInspector]
    public Vector2 input_View;

    public Vector3 playerMovement;

    [Header("Settings")]
    public PlayerSettingModel settings;
    public bool isTargetMode;

    [Header("Camera")]
    public Transform cameraTarget;
    public CameraController cameraController;

    [Header("Movement")]
    public float movementSpeedOffset = 1;
    public float movementSmoothdamp = 0.3f;
    public bool isWalking;
    public bool isSprinting;
    public bool isMoving;

    private float verticalSpeed;
    private float targetVerticalSpeed;
    private float verticalSpeedVelocity;

    private float horizontalSpeed;
    private float targetHorizontalSpeed;
    private float horizontalSpeedVelocity;


    [Header("Grid Movement")]
    [SerializeField]
    public float moveSpeed = 0.25f;
    [SerializeField]
    public float rayLength = 1.4f;
    [SerializeField]
    public float rayOffsetX = 0.5f;
    [SerializeField]
    public float rayOffsetY = 0.5f;
    [SerializeField]
    public float rayOffsetZ = 0.5f;

    [SerializeField]
    Vector3 targetPosition;
    [SerializeField]
    Vector3 startPosition;
    bool moving;

    Vector3 direc;
    Vector3 xOffset;
    Vector3 yOffset;
    Vector3 zOffset;
    Vector3 zAxisOriginA;
    Vector3 zAxisOriginB;
    Vector3 xAxisOriginA;
    Vector3 xAxisOriginB;

    [SerializeField]
    Transform cameraRotator = null;

    [SerializeField]
    LayerMask walkableMask = 0;

    [SerializeField]
    LayerMask collidableMask = 0;

    [SerializeField]
    float maxFallCastDistance = 100f;
    [SerializeField]
    float fallSpeed = 30f;
    bool falling;
    float targetFallHeight;

    // Khai báo biến để lưu trữ vị trí mục tiêu khi di chuyển
    Vector3 targetPosition2;

    // Khai báo biến để lưu trữ khoảng cách di chuyển tối đa
    public float maxMoveDistance = 1.5f;
    public float movementSpeed = 5f;

    [Header("Audio")]
    public AudioSource MovementSound;
    public AudioSource CollisionSound;
    public float soundCooldown = 0.5f; // Thời gian cooldown giữa các lần phát âm thanh
    private float soundCooldownTimer = 0f;

    IEnumerator PlaySoundWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        MovementSound.Play();
    }



    private Dictionary<KeyCode, bool> canPressKey = new Dictionary<KeyCode, bool>
    {
        { KeyCode.W, true },
        { KeyCode.A, true },
        { KeyCode.S, true },
        { KeyCode.D, true }
    };



    IEnumerator CheckKeyPress()
    {
        while (true)
        {
            foreach (KeyCode key in new KeyCode[] { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D })
            {
                if (Input.GetKeyDown(key) && canPressKey[key])
                {
                    // Thực hiện hành động khi nhấn phím
                    Debug.Log($"Key {key} pressed");

                    // Khóa tất cả các phím trong bộ W, A, S, D
                    LockKeys();
                    //  OnDisable();  // Invoke("OnDisable", 1f); 
                    // Bắt đầu coroutine để mở khóa các phím sau 2 giây
                    StartCoroutine(UnlockKeysAfterDelay(2f));
                }
            }

            // Chờ đến frame tiếp theo
            yield return null;
        }
    }

    void LockKeys()
    {
        foreach (KeyCode key in new KeyCode[] { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D })
        {
            canPressKey[key] = false;
        }
    }

    IEnumerator UnlockKeysAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // OnEnable();
        UnlockKeys();
    }

    void UnlockKeys()
    {
        foreach (KeyCode key in new KeyCode[] { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D })
        {
            canPressKey[key] = true;
        }
    }

    [Header("Stats")]
    public PlayerStatsModels playerStats;

    #region - Awake -

    private void Awake()
    {

        characterController = GetComponent<CharacterController>();
        characterAnimator = GetComponent<Animator>();
        cameraController = FindObjectOfType<CameraController>();

        playerInputActions = new PlayerInputActions();

        playerInputActions.Movement.Movement.performed += x => input_Movement = x.ReadValue<Vector2>();
        playerInputActions.Movement.View.performed += x => input_View = x.ReadValue<Vector2>();

        playerInputActions.Actions.Jump.performed += x => Jump();

        playerInputActions.Actions.WalkingToggle.performed += x => ToggleWalking();
        playerInputActions.Actions.Sprint.performed += x => Sprint();
    }

    #endregion

    #region - Jumping -

    private void Jump()
    {
        Debug.Log("I'm Jumping");
    }

    #endregion

    #region - Sprinting -

    private void Sprint()
    {
        if (!CanSprint())
        {
            return;
        }

        if (playerStats.Stamina > (playerStats.MaxStamina / 4))
        {
            isSprinting = true;
        }
    }

    private bool CanSprint()
    {
        if (isTargetMode)
        {
            return false;
        }

        var sprintFalloff = 0.8f;

        if ((input_Movement.y < 0 ? input_Movement.y * -1 : input_Movement.y) < sprintFalloff && (input_Movement.x < 0 ? input_Movement.x * -1 : input_Movement.x) < sprintFalloff)
        {
            return false;
        }

        return true;
    }

    private void CalculateSprint()
    {
        if (!CanSprint())
        {
            isSprinting = false;
        }

        if (isSprinting)
        {
            if (playerStats.Stamina > 0)
            {
                playerStats.Stamina -= playerStats.StaminaDrain * Time.deltaTime;
            }
            else
            {
                isSprinting = false;
            }

            playerStats.StaminaCurrentDelay = playerStats.StaminaDelay;
        }
        else
        {
            if (playerStats.StaminaCurrentDelay <= 0)
            {
                if (playerStats.Stamina < playerStats.MaxStamina)
                {
                    playerStats.Stamina += playerStats.StaminaRestore * Time.deltaTime;
                }
                else
                {
                    playerStats.Stamina = playerStats.MaxStamina;
                }
            }
            else
            {
                playerStats.StaminaCurrentDelay -= Time.deltaTime;
            }
        }
    }

    #endregion

    #region - Movement -

    private void ToggleWalking()
    {
        isWalking = !isWalking;
    }








    private void Movement()
    {
        characterAnimator.SetBool("IsTargetMode", isTargetMode);

        if (isTargetMode)
        {
            if (input_Movement.y > 0)
            {
                targetVerticalSpeed = (isWalking ? settings.WalkingSpeed : settings.RunningSpeed);
            }
            else
            {
                targetVerticalSpeed = (isWalking ? settings.WalkingBackwardSpeed : settings.RunningBackwardSpeed);
            }

            targetHorizontalSpeed = (isWalking ? settings.WalkingStrafingSpeed : settings.RunningStrafingSpeed);
        }
        else
        {
            var orginalRotation = transform.rotation;
            transform.LookAt(playerMovement + transform.position, Vector3.up);
            var newRotation = transform.rotation;

            transform.rotation = Quaternion.Lerp(orginalRotation, newRotation, settings.CharaterRotationSmoothDamp);

            float playerSpeed = 0;

            if (isSprinting)
            {
                playerSpeed = settings.SprintingSpeed;
            }
            else
            {
                playerSpeed = (isWalking ? settings.WalkingSpeed : settings.RunningSpeed);
            }

            targetVerticalSpeed = playerSpeed;
            targetHorizontalSpeed = playerSpeed;
        }


        targetVerticalSpeed = (targetVerticalSpeed * movementSpeedOffset) * input_Movement.y;
        targetHorizontalSpeed = (targetHorizontalSpeed * movementSpeedOffset) * input_Movement.x;

        verticalSpeed = Mathf.SmoothDamp(verticalSpeed, targetVerticalSpeed, ref verticalSpeedVelocity, movementSmoothdamp);
        horizontalSpeed = Mathf.SmoothDamp(horizontalSpeed, targetHorizontalSpeed, ref horizontalSpeedVelocity, movementSmoothdamp);

        if (isTargetMode)
        {
            characterAnimator.SetFloat("Vertical", verticalSpeed);
            characterAnimator.SetFloat("Horizontal", horizontalSpeed);
        }
        else
        {
            float verticalActualSpeed = verticalSpeed < 0 ? verticalSpeed * -1 : verticalSpeed;
            float horizontalActualSpeed = horizontalSpeed < 0 ? horizontalSpeed * -1 : horizontalSpeed;

            float animatorVertical = verticalActualSpeed > horizontalActualSpeed ? verticalActualSpeed : horizontalActualSpeed;

            characterAnimator.SetFloat("Vertical", animatorVertical);
        }




        playerMovement = cameraController.transform.forward * verticalSpeed * Time.deltaTime;
        playerMovement += cameraController.transform.right * horizontalSpeed * Time.deltaTime;

        isMoving = playerMovement != Vector3.zero;

        if (isMoving)
        {
            if (!MovementSound.isPlaying && soundCooldownTimer <= 0f)
            {
                StartCoroutine(PlaySoundWithDelay(0f)); // Phát âm thanh với độ trễ 1 giây
                soundCooldownTimer = soundCooldown; // Thiết lập thời gian cooldown
            }
        }
        else
        {
            MovementSound.Stop();

        }

        // Giảm thời gian cooldown sau mỗi frame
        if (soundCooldownTimer > 0f)
        {
            soundCooldownTimer -= Time.deltaTime;
        }



        //   characterController.Move(playerMovement);



    }

    #endregion

    #region - Update -

    private void Update()
    {
    
       // Grid();
        Movement();
        // StartCoroutine(CheckKeyPress());
        //CalculateSprint();

    }
    private void LateUpdate()
    {
        cameraController.FollowCameraTarget();
    }
    #endregion

    #region - Enable/Disable -

    private void OnEnable()
    {
        playerInputActions.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Disable();
    }

    #endregion


    #region  - Grid Movement -
    private void Grid()
    {
        // Set the ray positions every frame

        yOffset = transform.position + Vector3.up * rayOffsetY;
        zOffset = Vector3.forward * rayOffsetZ;
        xOffset = Vector3.right * rayOffsetX;

        zAxisOriginA = yOffset + xOffset;
        zAxisOriginB = yOffset - xOffset;

        xAxisOriginA = yOffset + zOffset;
        xAxisOriginB = yOffset - zOffset;

        // Draw Debug Rays

        Debug.DrawLine(
                zAxisOriginA,
                zAxisOriginA + Vector3.forward * rayLength,
                Color.red,
                Time.deltaTime);
        Debug.DrawLine(
                zAxisOriginB,
                zAxisOriginB + Vector3.forward * rayLength,
                Color.red,
                Time.deltaTime);

        Debug.DrawLine(
                zAxisOriginA,
                zAxisOriginA + Vector3.back * rayLength,
                Color.red,
                Time.deltaTime);
        Debug.DrawLine(
                zAxisOriginB,
                zAxisOriginB + Vector3.back * rayLength,
                Color.red,
                Time.deltaTime);

        Debug.DrawLine(
                xAxisOriginA,
                xAxisOriginA + Vector3.left * rayLength,
                Color.red,
                Time.deltaTime);
        Debug.DrawLine(
                xAxisOriginB,
                xAxisOriginB + Vector3.left * rayLength,
                Color.red,
                Time.deltaTime);

        Debug.DrawLine(
                xAxisOriginA,
                xAxisOriginA + Vector3.right * rayLength,
                Color.red,
                Time.deltaTime);
        Debug.DrawLine(
                xAxisOriginB,
                xAxisOriginB + Vector3.right * rayLength,
                Color.red,
                Time.deltaTime);

        if (falling)
        {
            if (transform.position.y <= targetFallHeight)
            {
                float x = Mathf.Round(transform.position.x);
                float y = Mathf.Round(targetFallHeight);
                float z = Mathf.Round(transform.position.z);

                transform.position = new Vector3(x, y, z);

                falling = false;

                return;
            }

            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            return;
        }
        else if (moving)
        {
            if (Vector3.Distance(startPosition, transform.position) > 1f)
            {
                float x = Mathf.Round(targetPosition.x);
                float y = Mathf.Round(targetPosition.y);
                float z = Mathf.Round(targetPosition.z);

                transform.position = new Vector3(x, y, z);

                moving = false;

                return;
            }

            transform.position += (targetPosition - startPosition) * moveSpeed * Time.deltaTime;
            return;
        }
        else
        {
            RaycastHit[] hits = Physics.RaycastAll(
                    transform.position + Vector3.up * 0.5f,
                    Vector3.down,
                    maxFallCastDistance,
                    walkableMask
            );

            if (hits.Length > 0)
            {
                int topCollider = 0;
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[topCollider].collider.bounds.max.y < hits[i].collider.bounds.max.y)
                        topCollider = i;
                }
                if (hits[topCollider].distance > 1f)
                {
                    targetFallHeight = transform.position.y - hits[topCollider].distance + 0.5f;
                    falling = true;
                }
            }
            else
            {
                targetFallHeight = -Mathf.Infinity;
                falling = true;
            }
        }

        // Handle player input
        // Also handle moving up 1 level

        if (Input.GetKeyDown(KeyCode.D))
        {
            if (CanMove(Vector3.forward))
            {
                targetPosition = transform.position + cameraRotator.transform.forward;
                startPosition = transform.position;
                moving = true;
            }
            /*
            else if (CanMoveUp(Vector3.forward))
            {
                targetPosition = transform.position + cameraRotator.transform.forward + Vector3.up;
                startPosition = transform.position;
                moving = true;
            }
            */
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            if (CanMove(Vector3.back))
            {
                targetPosition = transform.position - cameraRotator.transform.forward;
                startPosition = transform.position;
                moving = true;
            }
            /*
            else if (CanMoveUp(Vector3.back))
            {
                targetPosition = transform.position - cameraRotator.transform.forward + Vector3.up;
                startPosition = transform.position;
                moving = true;
            }
            */
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            if (CanMove(Vector3.left))
            {
                targetPosition = transform.position - cameraRotator.transform.right;
                startPosition = transform.position;
                moving = true;
            }
            /*
            else if (CanMoveUp(Vector3.left))
            {
                targetPosition = transform.position - cameraRotator.transform.right + Vector3.up;
                startPosition = transform.position;
                moving = true;
            }
            */
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            if (CanMove(Vector3.right))
            {
                targetPosition = transform.position + cameraRotator.transform.right;
                startPosition = transform.position;
                moving = true;
            }
            /*
            else if (CanMoveUp(Vector3.right))
            {
                targetPosition = transform.position + cameraRotator.transform.right + Vector3.up;
                startPosition = transform.position;
                moving = true;
            }
            */
        }
    }

    // Check if the player can move

    bool CanMove(Vector3 direction)
    {
        if (direction.z != 0)
        {
            if (Physics.Raycast(zAxisOriginA, direction, rayLength)) return false;
            if (Physics.Raycast(zAxisOriginB, direction, rayLength)) return false;
        }
        else if (direction.x != 0)
        {
            if (Physics.Raycast(xAxisOriginA, direction, rayLength)) return false;
            if (Physics.Raycast(xAxisOriginB, direction, rayLength)) return false;
        }
        return true;
    }




    // Check if the player can step-up
    /*
    bool CanMoveUp(Vector3 direction)
    {
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.up, 1f, collidableMask))
            return false;
        if (Physics.Raycast(transform.position + Vector3.up * 1.5f, direction, 1f, collidableMask))
            return false;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, direction, 1f, walkableMask))
            return true;
        return false;
    }

    */


    /*
    void OnCollisionEnter(Collision other)
    {
        if (falling && (1 << other.gameObject.layer & walkableMask) == 0)
        {
            // Find a nearby vacant square to push us on to
            Vector3 direction = Vector3.zero;
            Vector3[] directions = { Vector3.forward, Vector3.right, Vector3.back, Vector3.left };
            for (int i = 0; i < 4; i++)
            {
                if (Physics.OverlapSphere(transform.position + directions[i], 0.1f).Length == 0)
                {
                    direction = directions[i];
                    break;
                }
            }
            transform.position += direction;
        }
    }
    */

    #endregion
}

