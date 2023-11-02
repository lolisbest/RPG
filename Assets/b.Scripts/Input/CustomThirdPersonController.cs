using UnityEngine;
using RPG.Common;
using RPG.UI;
using System.Collections.Generic;
using System.Collections;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using StarterAssets;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace RPG.Input
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class CustomThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        [Tooltip("Camera Rotation Sensitivity")]
        public float CameraRotationSensitivity = 2f;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

        private int _animIDSlash;
        private int _animIDActionSkill;
        private int _animIDProjectileSkill;

        private int _animIDBlock;
        private int _animIDHit;
        private int _animIDDeath;
        private int _animIDRespawn;

        [Header("Physics.Cast Radius")]
        public float Radius = 1f;

        [Header("상호작용 가능한 오브젝트 감지 거리")]
        public float CameraRayCastMaxDistance;

        [Header("감지된 후, 상호 작용 가능한 최대거리")]
        public float MaxDistanceOnInteraction;

        [Header("Raycast로 감지된 오브젝트")]
        public InteractableObject ObjectToInteractWith;

        private UIManager _uiManager;

        public Player @Player;

        public Transform CameraRoot;

        public bool IsBlocking
        {
            get
            {
                return _animator.GetCurrentAnimatorStateInfo(0).IsName("great sword blocking on");
            }
        }

#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private CustomStarterAssetsInputs _inputAsset;

        public StructInput Inputs { get; private set; }

        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

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

        private bool IsIdleBlend
        {
            get
            {
                if (_hasAnimator)
                    return _animator.GetCurrentAnimatorStateInfo(0).IsName("Idle Walk Run Blend");
                return false;
            }
        }

        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        public void SetHitAnimation()
        {
            _animator.SetTrigger(_animIDHit);
            // 막기 자세 중에 막지 못 한 공격으로 막기 자세가 풀림
            _inputAsset.block = false;
        }

        public void SetDeath()
        {
            //Debug.Log("SetDeath");
            _animator.SetTrigger(_animIDDeath);
            _controller.enabled = false;
            ResetAnimatorAndInput();
        }
        
        private void ResetAnimatorAndInput()
        {
            _inputAsset.block = false;
        }

        public void SetRespawn()
        {
            //Debug.Log("SetRespawn");
            _animator.SetTrigger(_animIDRespawn);
            _controller.enabled = true;
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _inputAsset = GetComponent<CustomStarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM 
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

            _uiManager = RPG.UI.UIManager.Instance;
            @Player = GameManager.Instance.Player;
        }

        private void FixedUpdate()
        {
            RaycastFromCamera();
        }

        private void SetInputs()
        {
            if (_uiManager.IsInteractingWithPlayer || @Player.IsDie)
            {
                _inputAsset.ClearInputsOnIntraction();
            }

            StructInput inputs = _inputAsset.GetInputs();

            // 막기 중이라면 이동 입력 버림
            if (IsBlocking) inputs.Move = Vector2.zero;

            // 커서를 사용해야한다면 회전 입력 버림
            if (_uiManager.ShouldUnlockMouse) inputs.Look = Vector2.zero;

            Inputs = inputs;
        }

        private void Update()
        {
            SetInputs();

            // 중력 적용 후, 그라운드 체크
            ApplyGravity();
            GroundedCheck();

            ToggleEscMenu();

            Move();
            Slash();
            Block();

            // Interaction
            TryDisplayInteractionKeyMessage();
            Interact();

            // npc dialog window
            TalkWithNpc();
            NextNpcDialog();
            CloseNpcDialogWindow();

            // npc quest selection window
            OpenQuestSelectionWindow();
            CloseQuestSelectionWindow();

            // npc quest detail window
            AcceptQuest();
            CloseNpcQuestDetailWindow();

            // npc shop
            OpenNpcShopWindow();
            CloseNpcShopWindow();

            // npc rest
            //RequestNpcRest();

            CloseNpcServiceSelectionWindow();

            // itembox
            LootAll();
            CloseItemBoxWindow();

            // inventory
            ToggleInventoryWindow();

            // skill
            ToggleSkillsWindow();

            // quickslot
            PressQuickSlot();

            ToggleCurrentQuestsWindow();

            _inputAsset.ClearBoolInputs();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        /// <summary>
        /// 애니메이터 string hash 저장
        /// </summary>
        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");

            _animIDSlash = Animator.StringToHash("Slash");
            _animIDActionSkill = Animator.StringToHash("ActionSkill");
            _animIDProjectileSkill = Animator.StringToHash("ProjectileSkill");
            _animIDBlock = Animator.StringToHash("Block");
            _animIDHit = Animator.StringToHash("Hit");
            _animIDDeath = Animator.StringToHash("Death");
            _animIDRespawn = Animator.StringToHash("Respawn");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);
            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (Inputs.Look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
                deltaTimeMultiplier *= CameraRotationSensitivity;

                _cinemachineTargetYaw += Inputs.Look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += Inputs.Look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Slash()
        {
            if(IsIdleBlend && Inputs.Attack)
            {
                _animator.SetTrigger(_animIDSlash);
            }
        }

        public bool Skill(EnumLocationType locationType)
        {
            Debug.Log($"ThirdPersonController.Skill() {locationType}");
            if (IsIdleBlend && locationType == EnumLocationType.FixedOnPlayer)
            {
                _animator.SetTrigger(_animIDActionSkill);
                return true;
            }
            else if (IsIdleBlend && locationType == EnumLocationType.Moveable)
            {
                _animator.SetTrigger(_animIDProjectileSkill);
                return true;
            }
            return false;
        }

        private void Block()
        {
            _animator.SetBool(_animIDBlock, Inputs.Block);
        }

        private void Move()
        {
            if (@Player.IsDie) return;

            Vector2 move;
            bool sprint;

            if (!IsIdleBlend)
            {
                move = Vector2.zero;
                sprint = false;
            }
            else
            {
                move = Inputs.Move;
                sprint = Inputs.Sprint;
            }

            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = sprint ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _inputAsset.analogMovement ? move.magnitude : 1f;

            //Debug.Log("targetSpeed " + targetSpeed);
            //Debug.Log("currentHorizontalSpeed " + currentHorizontalSpeed);

            // accelerate or decelerate to target speed
            if ((currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset))
            {
                //Debug.Log("1");
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                //Debug.Log("2");
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(move.x, 0.0f, move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving

            float deltaY = 0f;

            if (move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;

                deltaY = Mathf.Abs(NormalizeAngle(transform.eulerAngles.y - _targetRotation));
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // 캐릭터의 방향과 입력 키의 방향의 각도 차이에 대해서 각도 차이가 심하면 느리게 움직임. 각도 차이가 작으면 원래의 움직이는 속도.
            float power = 0f;
            float statMoveAngle = 90f;
            float maxSpeedMax = 10f;

            if (deltaY <= statMoveAngle)
            {
                // 90~60 Lerp: 0~1
                // 60 = 1
                power = Mathf.Clamp01((80 - deltaY) / maxSpeedMax);

                // 0.1 ~ 1
                //power = 0.99f * power + 0.01f;
                power *= power *= power *= power *= power *= power;
            }

            _controller.Move(targetDirection.normalized * (_speed * power * Time.deltaTime) +
                 new Vector3(0.0f, _verticalVelocity, 0.0f) * power * Time.deltaTime);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude * power);
            }
        }

        public float NormalizeAngle(float angle)
        {
            angle %= 360; // 각도를 360으로 나눈 나머지를 사용
            if (angle > 180)
            {
                angle -= 360;
            }
            else if (angle < -180)
            {
                angle += 360;
            }

            return angle;
        }

        public void Move(Vector3 delta)
        {
            _controller.Move(delta);

            //Debug.Log("delta " + delta);
            //Debug.Log("_controller.velocity " + _controller.velocity);

            // 제자리 이동이면 속도가 0으로 됨. -> 막기를 푼 직후 Move()에서 currentHorizontalSpeed에 이상한 값이 들어가는 것을 방지
            _controller.Move(Vector3.zero);
        }

        private void ApplyGravity()
        {
            if (Grounded)
            {
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_inputAsset.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
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
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                _inputAsset.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            // -360~+360 범위로 angle을 변환 후, lfMin, lfMax 범위로 Clamp
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            // 땅 위에 붙어 있다면 초록, 땅과 떨어져 있다면 빨강
            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);

            // 상호 작용이 끊어지는 최대 거리 표시용
            Color transparentBlue = new Color(0.0f, 0.0f, 1.0f, 0.35f);
            Gizmos.color = transparentBlue;

            Gizmos.DrawWireSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                MaxDistanceOnInteraction);


        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }

        private void TryDisplayInteractionKeyMessage()
        {
            //Debug.Log($"TryDisplayInteractionKeyMessage : {ObjectToInteractWith}, {InGameUIManager.Instance.IsInteractingWithPlayer}");
            if (ObjectToInteractWith != null && !_uiManager.IsInteractingWithPlayer)
            {
                //Debug.Log("TryDisplayInteractionKeyMessage");
                _uiManager.ActivateInteractionKeyMessage(ObjectToInteractWith.Type);
                ObjectToInteractWith.ActivateDetectedOutline();
                return;
            }

            _uiManager.DeactivateInteractionKeyMessage();
        }

        private void Interact()
        {
            if(Inputs.Interact && ObjectToInteractWith != null)
            {
                ObjectToInteractWith.Interact();
            }
        }

        private void LootAll()
        {
            if(Inputs.LootAll && _uiManager.IsOpenItemBoxWindow)
            {
                StructIdCount[] droppedItems = _uiManager.CurrentBeingOpenItemBox.Items.ToArray();
                Debug.Log($"PutAwayAll : {droppedItems.Length}");

                for (int itemIndex = 0; itemIndex < droppedItems.Length; itemIndex++)
                {
                    StructItemData itemData = DataBase.Items[droppedItems[itemIndex].Id];
                    if(itemData.ItemType == EnumItemType.Currency)
                    {
                        @Player.AddMoney(droppedItems[itemIndex].Count);
                        _uiManager.CurrentBeingOpenItemBox.RemoveItem(droppedItems[itemIndex].Id);
                    }
                    else if (@Player.AddItem(droppedItems[itemIndex].Id, droppedItems[itemIndex].Count) == ResultType.Success)
                    {
                        _uiManager.CurrentBeingOpenItemBox.RemoveItem(droppedItems[itemIndex].Id);
                    }
                    else
                    {
                        Debug.Log($"Fail Add Item To Inventory : {itemData.Name}");
                    }
                }
            }
        }

        private void CloseNpcServiceSelectionWindow()
        {
            if(_uiManager.IsOpenNpcServiceSelectionWindow)
            {
                if (Inputs.Quit)
                {
                    _uiManager.CloseNpcServiceSelectionWindow();
                }

            }
        }

        private void CloseItemBoxWindow()
        {
            if (_uiManager.IsOpenItemBoxWindow)
            {
                if (Inputs.Quit)
                {
                    _uiManager.CloseItemBoxWindow();
                }
                else if (_uiManager.CurrentBeingOpenItemBox != null && Vector3.Distance(transform.position, _uiManager.CurrentBeingOpenItemBox.transform.position) > MaxDistanceOnInteraction)
                {
                    _uiManager.CloseItemBoxWindow();
                }
            }
        }

        /// <summary>
        /// 먄약 현재 상호 작용 중인 오브젝트가 있다면 거리가 멀어지면 새로운 오브젝트를 탐색
        /// 거리가 가깝다면 현재 상태 유지
        /// 상호 작용 중인 오브젝트가 없고 이전 XXXUpdate에서 찾은 오브젝트가 있다면 버리고 다시 찾음
        /// </summary>
        private void RaycastFromCamera()
        {
            // 현재 UI가 상호 작용 중인지
            if (_uiManager.IsInteractingWithPlayer)
            {
                if (ObjectToInteractWith == null)
                    return;

                // UI 가 상호 작용 중일 때
                float currentDistance = Vector3.Distance(transform.position, ObjectToInteractWith.transform.position);
                bool isOverDistance = currentDistance > MaxDistanceOnInteraction;

                // 어느 정도 멀어졌는지
                if (isOverDistance)
                {
                    // 멀어 졌다면 상호 작용 끄기
                    //Debug.Log("상호 작용 중에 거리가 멀어졌을 때");
                    ObjectToInteractWith.DeactivateOutLine();
                    ObjectToInteractWith.StopInteraction();
                    ObjectToInteractWith = null;
                }
                else
                {
                    // 현상 유지
                }
            }
            else
            {
                // UI 가 상호 작용하고 있지 않을 때
                
                // 매 프레임 마다 초기화 - 매 프레임마다 감지 결과를 변경하기 위해
                if (ObjectToInteractWith != null)
                {
                    //Debug.Log("현재 감지된 오브젝트가 있는 경우");
                    ObjectToInteractWith.DeactivateOutLine();
                    ObjectToInteractWith.StopInteraction();
                    ObjectToInteractWith = null;
                }

                int targetLayerMask = 1 << InteractableObject.LayerIndex;

                Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * CameraRayCastMaxDistance);

                // 카메라 방향으로 상호 작용 가능한 오브젝트 검사
                if (Physics.SphereCast(Camera.main.transform.position, Radius, Camera.main.transform.forward, 
                    out RaycastHit hit, CameraRayCastMaxDistance, targetLayerMask))
                {
                    ObjectToInteractWith = hit.transform.gameObject.GetComponent<InteractableObject>();
                }
            }
        }

        private void ToggleInventoryWindow()
        {
            if (Inputs.Inventory)
            {
                //Debug.Log("@UIManager.IsOpenInventoryWindow: " + @UIManager.IsOpenInventoryWindow);
                //Debug.Log("@UIManager.IsInteractingWithPlayer: " + @UIManager.IsInteractingWithPlayer);
                if (!_uiManager.IsOpenInventoryWindow && !_uiManager.IsInteractingWithPlayer)
                {
                    _uiManager.OpenInventoryWindow();
                }
                else if(_uiManager.IsOpenInventoryWindow)
                {
                    _uiManager.CloseInventoryWindow();
                }
            }
        }

        private void ToggleSkillsWindow()
        {
            if (Inputs.Skill)
            {
                //Debug.Log("@UIManager.IsOpenInventoryWindow: " + @UIManager.IsOpenInventoryWindow);
                //Debug.Log("@UIManager.IsInteractingWithPlayer: " + @UIManager.IsInteractingWithPlayer);
                if (!_uiManager.IsOpenSkillsWindow && !_uiManager.IsInteractingWithPlayer)
                {
                    _uiManager.OpenSkillsWindow();
                }
                else if (_uiManager.IsOpenSkillsWindow)
                {
                    _uiManager.CloseSkillsWindow();
                }
            }
        }

        private void TalkWithNpc()
        {
            if(_uiManager.IsOpenNpcServiceSelectionWindow)
            {
                if (Inputs.NpcTalk)
                {
                    _uiManager.OpenDialogWindow();
                }
            }
        }

        private void NextNpcDialog()
        {
            if (_uiManager.IsOpenDialogWindow && _uiManager.IsPresentDialogNextButton)
            {
                if (Inputs.NpcTalkNext)
                {
                    _uiManager.NextDialog();
                }
            }
        }

        private void CloseNpcDialogWindow()
        {
            if(_uiManager.IsOpenDialogWindow && _uiManager.IsPresentDialogQuitButton)
            {
                if(Inputs.Quit)
                {
                    _uiManager.CloseDialogWindow();
                }
            }
        }

        private void OpenQuestSelectionWindow()
        {
            if (_uiManager.IsOpenNpcServiceSelectionWindow)
            {
                if (Inputs.Quest)
                {
                    _uiManager.OpenQuestSelectionWindow();
                }
            }
        }

        private void CloseQuestSelectionWindow()
        {
            if (_uiManager.IsOpenQuestSelectionWindow)
            {
                if (Inputs.Quit)
                {
                    _uiManager.CloseQuestSelectionWindow();
                }
            }
        }

        private void AcceptQuest()
        {
            if (_uiManager.IsOpenNpcQuestDetailWindow)
            {
                if (Inputs.NpcQuestAccept)
                {
                    _uiManager.AcceptNpcQuest();
                }
            }
        }

        private void CloseNpcQuestDetailWindow()
        {
            if (_uiManager.IsOpenNpcQuestDetailWindow)
            {
                if (Inputs.Quit)
                {
                    _uiManager.CloseNpcQuestDetailWindow();
                }
            }
        }

        private void OpenNpcShopWindow()
        {
            if (_uiManager.IsOpenNpcServiceSelectionWindow)
            {
                if (Inputs.NpcShop)
                {
                    _uiManager.OpenShop();
                }
            }
        }

        private void CloseNpcShopWindow()
        {
            if (_uiManager.IsOpenShopWindow)
            {
                if (Inputs.Quit)
                {
                    _uiManager.CloseShop();
                }
            }
        }

        private void RequestNpcRest()
        {
            if (_uiManager.IsOpenNpcServiceSelectionWindow)
            {
                if (Inputs.NpcRest)
                {
                    throw new System.NotImplementedException("not implemented npc rest");
                }
            }
        }

        private void PressQuickSlot()
        {
            if(Inputs.Slot1)
            {
                _uiManager.UseQuickSlot(1);
            } 

            if (Inputs.Slot2)
            {
                _uiManager.UseQuickSlot(2);
            }

            if (Inputs.Slot3)
            {
                _uiManager.UseQuickSlot(3);
            }

            if (Inputs.Slot4)
            {
                _uiManager.UseQuickSlot(4);
            }

            if (Inputs.Slot5)
            {
                _uiManager.UseQuickSlot(5);
            }

            if (Inputs.Slot6)
            {
                _uiManager.UseQuickSlot(6);
            }

            if (Inputs.Slot7)
            {
                _uiManager.UseQuickSlot(7);
            }

            if (Inputs.Slot8)
            {
                _uiManager.UseQuickSlot(8);
            }

            if (Inputs.Slot9)
            {
                _uiManager.UseQuickSlot(9);
            }

            if (Inputs.Slot0)
            {
                _uiManager.UseQuickSlot(0);
            }
        }

        private void ToggleEscMenu()
        {
            if (Inputs.Esc)
            {
                Debug.Log("Player.ToggleEscMenu()");
                _uiManager.ToggleEscWindow();
            }
        }

        public void AlignPlayerDirectionWithCamera()
        {
            Vector3 cameraForward = Camera.main.transform.forward;
            cameraForward.y = 0;
            Quaternion newRotation = Quaternion.LookRotation(cameraForward);
            transform.rotation = newRotation;
        }

        /// <summary>
        /// AnimationClip Event
        /// </summary>
        public void OpenOnDeathWindow()
        {
            _uiManager.OpenOnDeathWindow();
        }

        public void ToggleCurrentQuestsWindow()
        {
            if (Inputs.Quest && !_uiManager.IsInteractingWithPlayer)
            {
                _uiManager.ToggleCurrentQuestsWindow();
            }
        }
    }
}