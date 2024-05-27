﻿using System.Collections;
using UnityEngine;
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

		[Header("Added Parameters")]
		public GameObject cameraRoot;
		public bool canMove = true;
		public bool isSprinting = false;

		[SerializeField] private PlayerDeathController deathController;
		[SerializeField] private PauseManager pauseManager;
		
		
		[Space(10)]
		
		
		[Header("View Bobbing Parameters")]
		[SerializeField] private float walkBobSpeed = 14f;
		[SerializeField] private float walkBobAmount = 0.05f;
		[SerializeField] private float sprintBobSpeed = 18f;
		[SerializeField] private float sprintBobAmount = 0.11f;
		[SerializeField] private float crouchBobSpeed = 8f;
		[SerializeField] private float crouchBobAmount = 0.025f;
		private float _defaultYPos = 0;
		private float _timer;
		
		
		[Space(10)]
		
		
		[Header("Crouching Parameters")]
		//private bool ShouldCrouch = Input.GetKeyDown(KeyCode.LeftControl) && !
		public bool isCrouching = false;
		private bool _inCrouchAnimation;
		
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

			if (InGameSettingsManager.Instance.enableViewBobbing)
			{
				HandleHeadBob();
			}

			// Toggle crouch state on key press
			if (Input.GetKeyDown(KeyCode.LeftControl) && Grounded && !_inCrouchAnimation)
			{
				HandleCrouching();
			}
		
			// Set booleans for sprinting
			isSprinting = Input.GetKey(KeyCode.LeftShift) && Grounded;
		}



		private void LateUpdate()
		{
			if (pauseManager.GameIsPaused) return;
			
			if (DialogueController.Instance.dialogueEnabled)
			{
				canMove = false;
				//Debug.Log("disabling movement because dialogue is enabled");
			}
			else
			{
				canMove = true;
				//Debug.Log("enabling movement because dialogue is disabled");
			}
			
			if(canMove && !deathController.isDead)
				CameraRotation();
		}

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
			// set target speed based on move speed, sprint speed and if sprint is pressed
			//float targetSpeed = input.sprint ? SprintSpeed : MoveSpeed;
			float targetSpeed;
			
			if (input.sprint)
			{
				targetSpeed = SprintSpeed;
			}
			else if (isCrouching)
			{
				targetSpeed = CrouchSpeed;
			}
			else
			{
				targetSpeed = MoveSpeed;
			}

			// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

			// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is no input, set the target speed to 0
			if (input.move == Vector2.zero) targetSpeed = 0.0f;

			// a reference to the players current horizontal velocity
			float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

			float speedOffset = 0.1f;
			float inputMagnitude = input.analogMovement ? input.move.magnitude : 1f;

			// accelerate or decelerate to target speed
			if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
			{
				// creates curved result rather than a linear one giving a more organic speed change
				// note T in Lerp is clamped, so we don't need to clamp our speed
				_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

				// round speed to 3 decimal places
				_speed = Mathf.Round(_speed * 1000f) / 1000f;
			}
			else
			{
				_speed = targetSpeed;
			}

			// normalise input direction
			Vector3 inputDirection = new Vector3(input.move.x, 0.0f, input.move.y).normalized;

			// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is a move input rotate player when the player is moving
			if (input.move != Vector2.zero)
			{
				// move
				inputDirection = transform.right * input.move.x + transform.forward * input.move.y;
			}

			if (!deathController.isDead)
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
				
				// cameraRoot.transform.localPosition = new Vector3(
				// 	cameraRoot.transform.localPosition.x,
				// 	_defaultYPos + Mathf.Sin(_timer) * (isCrouching ? crouchBobAmount : isSprinting ? sprintBobAmount : walkBobAmount),
				// 	cameraRoot.transform.localPosition.z
				// );
			}
		}

		private void HandleCrouching()
		{
			StartCoroutine(CrouchAndStandRoutine());
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