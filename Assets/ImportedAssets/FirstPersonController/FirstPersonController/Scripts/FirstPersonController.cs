using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class FirstPersonController : MonoBehaviour
    {
        #region Default Parameters
        [Header("Player")]
        public float MoveSpeed = 4.0f;
        public float SprintSpeed = 6.0f;
        public float CrouchSpeed = 2.0f; 
        public float RotationSpeed = 1.0f;
        public float SpeedChangeRate = 10.0f;

        [Space(10)]
        public float JumpHeight = 1.2f;
        public float Gravity = -15.0f;

        [Space(10)]
        public float JumpTimeout = 0.1f;
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        public bool Grounded = true;
        public float GroundedOffset = -0.14f;
        public float GroundedRadius = 0.5f;
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        public GameObject CinemachineCameraTarget;
        public float TopClamp = 90.0f;
        public float BottomClamp = -90.0f;

        private float _cinemachineTargetPitch;
        private float _speed;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        private float _currentSpeed;
        public float GetCurrentSpeed => _currentSpeed;
	
#if ENABLE_INPUT_SYSTEM
        public PlayerInput _playerInput;
#endif
        public CharacterController controller;
        public StarterAssetsInputs input;
        private GameObject _mainCamera;
        private const float _threshold = 0.01f;

        #endregion

        [Header("Added Parameters")]
        public GameObject cameraRoot;
        public bool canMove = true;
        public bool isSprinting = false;
        public bool canRotate = true;

        [SerializeField] private PlayerDeathController deathController;
        [SerializeField] private PauseManager pauseManager;

        [Space(10)]

        [Header("View Bobbing Parameters")]
        [SerializeField] private float walkBobSpeed = 14f;
        [SerializeField] private float walkBobAmount = 0.05f;
        [SerializeField] private float sprintBobSpeed = 18f;
        [SerializeField] private float sprintBobAmount = 0.11f;

        private float _defaultYPos = 0;
        private float _timer;

        [Space(10)]

        [Header("Crouching Parameters")]
        public bool isCrouching = false;
        private bool _inCrouchAnimation;
        [SerializeField] private Image standImage;
        [SerializeField] private Sprite crouchingSprite;
        [SerializeField] private Sprite standingSprite;
        [SerializeField] private Sprite sprintingSprite;

        [SerializeField] private Slider sprintStaminaSlider;
        private const float MaxSprintStamina = 5.0f;
        private float _currentSprintStamina = MaxSprintStamina;
        [SerializeField] private float sprintStaminaDecreaseRate = 1.0f;
        [SerializeField] private float sprintStaminaRecoveryRate = 0.5f;
        [SerializeField] private float staminaRecoveryDelay = 2.0f;
        private float _staminaRecoveryTimer = 0.0f;

        [SerializeField] private float crouchingHeight = .5f;
        [SerializeField] private float standingHeight = 2.0f;
        [SerializeField] private float timeToEnterCrouch = 0.25f;
        [SerializeField] private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
        [SerializeField] private Vector3 standingCenter = new Vector3(0, .93f, 0);
        private float _originalCameraRootPosition;

        private Coroutine _fadeSliderCoroutine;
        private Coroutine _fadeOutCoroutine;

        private PlayerDualHandInventory _playerInventory;
        private PlayerArmsAnimationController _playerArms;

        private Color _originalSliderColor;
        private Color _originalBackgroundColor;
        private Color _originalFillColor;

        private bool IsCurrentDeviceMouse => 
#if ENABLE_INPUT_SYSTEM
            _playerInput.currentControlScheme == "KeyboardMouse";
#else
            false;
#endif

        private void Awake()
        {
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }

            _defaultYPos = _mainCamera.transform.localPosition.y;
            _originalCameraRootPosition = cameraRoot.transform.localPosition.y;
            _playerArms = FindObjectOfType<PlayerArmsAnimationController>();
            _playerInventory = GetComponent<PlayerDualHandInventory>();
            
            _originalSliderColor = sprintStaminaSlider.fillRect.GetComponent<Image>().color;
            _originalBackgroundColor = sprintStaminaSlider.transform.Find("Background").GetComponent<Image>().color;
            _originalFillColor = sprintStaminaSlider.transform.Find("Image").GetComponent<Image>().color;
        }

        private void Start()
        {
            controller = GetComponent<CharacterController>();
            input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#else
            Debug.LogError("Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
            _currentSprintStamina = MaxSprintStamina;
            sprintStaminaSlider.value = _currentSprintStamina;
        }

        private void Update()
        {
            if (pauseManager.GameIsPaused) return;
            if (deathController.isDead) return;

            JumpAndGravity();
            GroundedCheck();
            Move();

            if (InGameSettingsManager.Instance.enableHeadBobbing && controller.enabled)
            {
                HandleHeadBob();
            }

            if (Input.GetKeyDown(InGameSettingsManager.Instance.crouchKey) && Grounded && !_inCrouchAnimation)
            {
                HandleCrouching();
            }

            bool wasSprinting = isSprinting;
            isSprinting = Input.GetKey(InGameSettingsManager.Instance.sprintKey) && !isCrouching && Grounded && _currentSprintStamina > 0;

            if (wasSprinting != isSprinting)
            {
                HandleSprintingIcon();
            }

            if (Input.GetKeyDown(KeyCode.Escape) && !pauseManager.GameIsPaused)
            {
                canMove = true;
                canRotate = true;
            }

            UpdateSprintStamina();
            
            UpdateAnimator();
            UpdateArmAnimations();
        }
        
        private void UpdateAnimator()
        {
            if (_playerArms != null)
            {
                _playerArms.animator.SetBool("isWalking", input.move != Vector2.zero && !isSprinting && !isCrouching);
                _playerArms.animator.SetBool("isRunning", isSprinting);
                _playerArms.animator.SetBool("isCrouching", isCrouching);
            }
        }

        private void UpdateArmAnimations()
        {
            if (_playerArms != null)
            {
                switch (isSprinting)
                {
                    case true when _playerInventory.GetCurrentHandItem != null && input.move != Vector2.zero:
                        _playerArms.SetIsRunningWithItem(true);
                        _playerArms.SetRunning(false);
                        _playerArms.SetWalking(false);
                        _playerArms.SetPickingUp(false);
                        _playerArms.SetIdle(false);
                        _playerArms.SetCrouching(false);
                        break;
                    case true when _playerInventory.GetCurrentHandItem == null && input.move != Vector2.zero:
                        _playerArms.SetIsRunningWithItem(false);
                        _playerArms.SetRunning(true);
                        _playerArms.SetWalking(false);
                        _playerArms.SetPickingUp(false);
                        _playerArms.SetIdle(false);
                        _playerArms.SetCrouching(false);
                        break;
                    default:
                    {
                        if (input.move != Vector2.zero && !isCrouching)
                        {
                            _playerArms.SetRunning(false);
                            _playerArms.SetWalking(true);
                            _playerArms.SetPickingUp(false);
                            _playerArms.SetIdle(false);
                            _playerArms.SetCrouching(false);
                        }
                        else if (isCrouching)
                        {
                            _playerArms.SetRunning(false);
                            _playerArms.SetWalking(false);
                            _playerArms.SetPickingUp(false);
                            _playerArms.SetIdle(false);
                            _playerArms.SetCrouching(true);
                        }
                        else
                        {
                            _playerArms.SetRunning(false);
                            _playerArms.SetWalking(false);
                            _playerArms.SetPickingUp(false);
                            _playerArms.SetIdle(true);
                            _playerArms.SetCrouching(false);
                        }

                        break;
                    }
                }
            }
        }

        
        
        public void ToggleCanMove()
        {
            canMove = !canMove;
        }

        private void UpdateSprintStamina()
        {
            if (isSprinting && input.move != Vector2.zero)
            {
                _currentSprintStamina -= Time.deltaTime * sprintStaminaDecreaseRate;
                if (_currentSprintStamina <= 0)
                {
                    _currentSprintStamina = 0;
                    isSprinting = false;
                    _staminaRecoveryTimer = staminaRecoveryDelay;
                }

                // Ensure the slider is fully visible when sprinting
                if (_fadeSliderCoroutine != null)
                {
                    StopCoroutine(_fadeSliderCoroutine);
                    _fadeSliderCoroutine = null;
                }
                SetSliderAlpha(1.0f);
            }
            else
            {
                if (_staminaRecoveryTimer > 0)
                {
                    _staminaRecoveryTimer -= Time.deltaTime;
                }
                else
                {
                    _currentSprintStamina += Time.deltaTime * sprintStaminaRecoveryRate;
                    if (_currentSprintStamina > MaxSprintStamina)
                    {
                        _currentSprintStamina = MaxSprintStamina;

                        _fadeSliderCoroutine ??= StartCoroutine(FadeOutSlider(sprintStaminaSlider, .75f));
                    }
                }
            }

            sprintStaminaSlider.value = _currentSprintStamina / MaxSprintStamina;
        }
		
        private void SetSliderAlpha(float alpha)
        {
            Color sliderColor = _originalSliderColor;
            sliderColor.a = alpha;
            sprintStaminaSlider.fillRect.GetComponent<Image>().color = sliderColor;

            Image backgroundImage = sprintStaminaSlider.transform.Find("Background").GetComponent<Image>();
            Color backgroundColor = _originalBackgroundColor;
            backgroundColor.a = alpha;
            backgroundImage.color = backgroundColor;

            Image sprintSliderFillImage = sprintStaminaSlider.transform.Find("Image").GetComponent<Image>();
            Color fillColor = _originalFillColor;
            fillColor.a = alpha;
            sprintSliderFillImage.color = fillColor;
        }

        private IEnumerator FadeOutSlider(Slider slider, float delay)
        {
            yield return new WaitForSeconds(delay);

            float fadeDuration = .6f;
            float elapsedTime = 0.0f;

            Color initialColor = slider.fillRect.GetComponent<Image>().color;

            while (elapsedTime < fadeDuration)
            {
                float newAlpha = Mathf.Lerp(initialColor.a, 0, elapsedTime / fadeDuration);
                SetSliderAlpha(newAlpha);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            SetSliderAlpha(0);
            _fadeSliderCoroutine = null;
        }


        private void LateUpdate()
        {
            if (pauseManager.GameIsPaused) return;
            if (deathController.isDead) return;

            CameraRotation();
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        }

        private void CameraRotation()
        {
            if (!canRotate) return;

            if (input.look.sqrMagnitude >= _threshold)
            {
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetPitch += input.look.y * RotationSpeed * deltaTimeMultiplier;
                _rotationVelocity = input.look.x * RotationSpeed * deltaTimeMultiplier;

                _cinemachineTargetPitch = Mathf.Clamp(_cinemachineTargetPitch, BottomClamp, TopClamp);

                CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
                transform.Rotate(Vector3.up * _rotationVelocity);
            }
        }

        private void Move()
        {
            if (!canMove) return;

            float targetSpeed = isSprinting ? SprintSpeed : isCrouching ? CrouchSpeed : MoveSpeed;
            if (input.move == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = input.analogMovement ? input.move.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }
            _currentSpeed = _speed;

            Vector3 inputDirection = new Vector3(input.move.x, 0.0f, input.move.y).normalized;
            if (input.move != Vector2.zero)
            {
                inputDirection = transform.right * input.move.x + transform.forward * input.move.y;
            }

            controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    if(InGameSettingsManager.Instance.enableJumping)
                        _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }

                // if we are not grounded, do not jump
                input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private void HandleHeadBob()
        {
            if (!Grounded) return;
			
            //Head bob while crouching is currently disabled.
            
            if (Mathf.Abs(input.move.x) != 0.0f && !isCrouching || Mathf.Abs(input.move.y) != 0.0f && !isCrouching)
            {
                //Debug.Log("Moving");
                _timer += Time.deltaTime * (isSprinting ? sprintBobSpeed : walkBobSpeed);

                cameraRoot.transform.localPosition = new Vector3(
                    cameraRoot.transform.localPosition.x,
                    _defaultYPos + Mathf.Sin(_timer) * (isSprinting ? sprintBobAmount : walkBobAmount),
                    cameraRoot.transform.localPosition.z
                );
            }
        }

        private void HandleCrouching()
        {
            StartCoroutine(CrouchAndStandRoutine());

            HandleCrouchingIcon();
        }


        private IEnumerator CrouchAndStandRoutine()
        {
            _inCrouchAnimation = true;

            float timeElapsed = 0;
            float targetHeight = isCrouching ? standingHeight : crouchingHeight;
            float currentHeight = controller.height;

            Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
            Vector3 currentCenter = controller.center;

            // Calculate target and current positions for cameraRoot
            Vector3 targetCameraRootPosition =
                isCrouching ? new Vector3(0, _defaultYPos, 0)
                    : new Vector3(0, crouchingHeight, 0);
			
            Vector3 currentCameraRootPosition = cameraRoot.transform.localPosition;

            while (timeElapsed < timeToEnterCrouch)
            {
                float t = timeElapsed / timeToEnterCrouch;

                controller.height = Mathf.Lerp(currentHeight, targetHeight, t);
                controller.center = Vector3.Lerp(currentCenter, targetCenter, t);

                // Interpolate the cameraRoot position
                cameraRoot.transform.localPosition = Vector3.Lerp(currentCameraRootPosition, targetCameraRootPosition, t);

                timeElapsed += Time.deltaTime;
                yield return null;
            }

            controller.height = targetHeight;
            controller.center = targetCenter;
            cameraRoot.transform.localPosition = targetCameraRootPosition;

            // Toggle crouch state
            isCrouching = !isCrouching;
            _inCrouchAnimation = false;
        }
    

        private void HandleSprintingIcon()
        {
            if (isSprinting && input.move != Vector2.zero)
            {
                standImage.sprite = sprintingSprite;
                standImage.color = Color.white;
                if (_fadeOutCoroutine != null) StopCoroutine(_fadeOutCoroutine);
                _fadeOutCoroutine = StartCoroutine(FadeOutIcon(standImage, 1.5f));
            }
            else
            {
                standImage.sprite = isCrouching ? crouchingSprite : standingSprite;
            }
        }

        private void HandleCrouchingIcon()
        {
            if (isCrouching)
            {
                standImage.sprite = crouchingSprite;
                standImage.color = Color.white;
                if (_fadeOutCoroutine != null) StopCoroutine(_fadeOutCoroutine);
                _fadeOutCoroutine = StartCoroutine(FadeOutIcon(standImage, 1.5f));
            }
            else
            {
                standImage.sprite = standingSprite;
            }
        }

        private IEnumerator FadeOutIcon(Image image, float delay)
        {
            yield return new WaitForSeconds(delay);
            while (image.color.a > 0)
            {
                Color newColor = image.color;
                newColor.a -= Time.deltaTime * 2;
                image.color = newColor;
                yield return null;
            }
        }
    }
}
