using System.Collections;
using Microsoft.Unity.VisualStudio.Editor;
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
		[Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 4.0f;
		[Tooltip("Sprint speed of the character in m/s")]
		public float SprintSpeed = 6.0f;
		[Tooltip("Crouch speed of the character in m/s")]
		public float CrouchSpeed = 2.0f; 
		[Tooltip("Rotation speed of the character")]
		public float RotationSpeed = 1.0f;
		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;

		[Space(10)]
		[Tooltip("The height the player can jump")]
		public float JumpHeight = 1.2f;
		[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
		public float Gravity = -15.0f;

		[Space(10)]
		[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
		public float JumpTimeout = 0.1f;
		[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
		public float FallTimeout = 0.15f;

		[Header("Player Grounded")]
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		public bool Grounded = true;
		[Tooltip("Useful for rough ground")]
		public float GroundedOffset = -0.14f;
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.5f;
		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;

		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 90.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -90.0f;

		// cinemachine
		private float _cinemachineTargetPitch;

		// player
		private float _speed;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;

		// timeout deltatime
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;

	
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
		
		//disabled due to conflicts with crouching and head bob.
		//[SerializeField] private float crouchBobSpeed = 8f;
		//[SerializeField] private float crouchBobAmount = 0.025f;
		private float _defaultYPos = 0;
		private float _timer;
		
		
		[Space(10)]
		
		
		[Header("Crouching Parameters")]
		public bool isCrouching = false;
		private bool _inCrouchAnimation;
		[SerializeField] private Image standImage;
		[SerializeField] private Sprite crouchingSprite;
		[SerializeField] private Sprite standingSprite;
		
		[SerializeField] private float crouchingHeight = .5f;
		[SerializeField] private float standingHeight = 2.0f;
		[SerializeField] private float timeToEnterCrouch = 0.25f;
		[SerializeField] private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
		[SerializeField] private Vector3 standingCenter = new Vector3(0, .93f, 0);
		private float _originalCameraRootPosition;
		
		private bool IsCurrentDeviceMouse
		{
			get
			{
				#if ENABLE_INPUT_SYSTEM
				return _playerInput.currentControlScheme == "KeyboardMouse";
				#else
				return false;
				#endif
			}
		}

		private void Awake()
		{
			// get a reference to our main camera
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}

			_defaultYPos = _mainCamera.transform.localPosition.y;

			_originalCameraRootPosition = cameraRoot.transform.localPosition.y;
		}

		private void Start()
		{
			controller = GetComponent<CharacterController>();
			input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
			_playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

			// reset our timeouts on start
			_jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;
		}

		private void Update()
		{
			if (pauseManager.GameIsPaused) return;
			if (deathController.isDead) return;
			
			JumpAndGravity();
			GroundedCheck();
			Move();

			if (InGameSettingsManager.Instance.enableViewBobbing && controller.enabled)
			{
				HandleHeadBob();
			}

			// Toggle crouch state on key press
			if (Input.GetKeyDown(InGameSettingsManager.Instance.crouchKey) && Grounded && !_inCrouchAnimation)
			{
				HandleCrouching();
			}
		
			// Set booleans for sprinting
			isSprinting = Input.GetKey(InGameSettingsManager.Instance.sprintKey) && !isCrouching && Grounded;

			if (Input.GetKeyDown(KeyCode.Escape) && !pauseManager.GameIsPaused)
			{
				canMove = true;
				canRotate = true;
			}
		}



		private void LateUpdate()
		{
			if (pauseManager.GameIsPaused) return;
			
			if (DialogueController.Instance != null && DialogueController.Instance.dialogueEnabled)
			{
				canMove = false;
				//Debug.Log("disabling movement because dialogue is enabled");
			}
			else
			{
				canMove = true;
				//Debug.Log("enabling movement because dialogue is disabled");
			}
			
			if(canMove && !deathController.isDead && controller.enabled)
				CameraRotation();
		}

		public void ToggleCanMove()
		{
			canMove = !canMove;
			controller.enabled = !controller.enabled;
		}

		private Coroutine fadeOutCoroutine;

		private void HandleCrouchingIcon()
		{
			standImage.sprite = !isCrouching ? crouchingSprite : standingSprite;

			//set to highest alpha value
			standImage.color = new Color(standImage.color.r, standImage.color.g, standImage.color.b, 1.0f);

			//prevent the icon from bugging out when spamming crouch/stand
			if (fadeOutCoroutine != null)
			{
				StopCoroutine(fadeOutCoroutine);
			}
            
			//icon will stay for 2 seconds
			fadeOutCoroutine = StartCoroutine(FadeOutIcon(standImage, 2.0f));
		}

		private IEnumerator FadeOutIcon(Image image, float displayTime)
		{
			yield return new WaitForSeconds(displayTime);

			//how long it will take for the fade to finish
			float fadeDuration = 1.0f;
			float elapsedTime = 0.0f;
            
			Color initialColor = image.color;

			//gradually increase the alpha value
			while (elapsedTime < fadeDuration)
			{
				float newAlpha = Mathf.Lerp(initialColor.a, 0, elapsedTime / fadeDuration);
                
				image.color = new Color(initialColor.r, initialColor.g, initialColor.b, newAlpha);

				elapsedTime += Time.deltaTime;

				yield return null;
			}

			//reset the image to transparent after fading out
			image.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0);

			fadeOutCoroutine = null;
		}

		#region Default Functions
		private void GroundedCheck()
		{
			// set sphere position, with offset
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
		}

		private void CameraRotation()
		{
			// if there is an input
			if (input.look.sqrMagnitude >= _threshold)
			{
				//Don't multiply mouse input by Time.deltaTime
				float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
				
				_cinemachineTargetPitch += input.look.y * RotationSpeed * deltaTimeMultiplier;
				_rotationVelocity = input.look.x * RotationSpeed * deltaTimeMultiplier;

				// clamp our pitch rotation
				_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

				// Update Cinemachine camera target pitch
				CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

				// rotate the player left and right
				transform.Rotate(Vector3.up * _rotationVelocity);
			}
		}

		private void Move()
		{
			if (!canMove && !controller.enabled) return;

			// Set target speed based on move speed, sprint speed, and crouch speed
			float targetSpeed;

			// Determine the correct speed based on the player's current state
			if (isCrouching)
			{
				targetSpeed = CrouchSpeed;
			}
			else if (input.sprint)
			{
				targetSpeed = SprintSpeed;
			}
			else
			{
				targetSpeed = MoveSpeed;
			}

			// If there is no input, set the target speed to 0
			if (input.move == Vector2.zero) targetSpeed = 0.0f;

			// A reference to the player's current horizontal velocity
			float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

			float speedOffset = 0.1f;
			float inputMagnitude = input.analogMovement ? input.move.magnitude : 1f;

			// Accelerate or decelerate to target speed
			if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
			{
				// Creates a curved result rather than a linear one giving a more organic speed change
				_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

				// Round speed to 3 decimal places
				_speed = Mathf.Round(_speed * 1000f) / 1000f;
			}
			else
			{
				_speed = targetSpeed;
			}

			// Normalize input direction
			Vector3 inputDirection = new Vector3(input.move.x, 0.0f, input.move.y).normalized;

			// If there is move input, rotate the player when the player is moving
			if (input.move != Vector2.zero)
			{
				// Move
				inputDirection = transform.right * input.move.x + transform.forward * input.move.y;
			}

			if (!deathController.isDead && controller.enabled == true)
			{
				controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
			}
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

		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		private void OnDrawGizmosSelected()
		{
			Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			if (Grounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;

			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
		}
		#endregion
		
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
	}
}