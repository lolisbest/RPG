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
        private int _animIDSkill;
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

        public InGameUIManager @UIManager;

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
        private CustomStarterAssetsInputs _input;
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
        }

        public void SetDeath()
        {
            //Debug.Log("SetDeath");
            _animator.SetTrigger(_animIDDeath);
            _controller.enabled = false;
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
            _input = GetComponent<CustomStarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM 
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

            @UIManager = InGameUIManager.Instance;
            @Player = Player.Instance;
        }

        private void FixedUpdate()
        {
            RaycastFromCamera();
        }

        private void Update()
        {
            if (@UIManager.IsInteractingWithPlayer || @Player.IsDie)
            {
                //Debug.Log("InteractingWithPlayer ");
                _input.ClearInputsOnIntraction();
            }

            if (@Player.IsDie)
            {
                // 죽었다면 return;
                //Debug.Log("Player Is Die");
                _input.ClearBoolInputs(); 
                return;
            }

            if (_input.noMouseRotation)
            {
                //Debug.Log("noMouseRotation");
                _input.look = Vector2.zero;
                _input.attack = false;
            }

            // 점프 후, 그라운드 체크
            JumpAndGravity();
            GroundedCheck();


            // Animation

            if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                Slash();
            }

            // 현재 막기 중이면 Move 호출하지 않음.
            if(!IsBlocking)
            {
                Move();
            }
       
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

            ToggleEscMenu();

            _input.ClearBoolInputs();
        }

        private void LateUpdate()
        {
            CameraRotation();
            ClearSkillInput();
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
            _animIDSkill = Animator.StringToHash("Skill");
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
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
                deltaTimeMultiplier *= CameraRotationSensitivity;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
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
            if(IsIdleBlend && _input.attack)
            {
                _animator.SetTrigger(_animIDSlash);
            }
        }

        public void Skill(int skillId)
        {
            if (IsIdleBlend && skillId > 0)
            {
                _animator.SetInteger(_animIDSkill, skillId);
            }
        }

        public void ClearSkillInput()
        {
            if (!IsIdleBlend)
            {
                // Idle이 아니라면 Skill Parameter를 0으로 매 프레임마다 초기화
                // 1) Idle 이 아니고 스킬 사용 중이라면 다음번 상태를 위한 셋팅.
                // 2) Idle 이 아니고 다른 행동 중이라면 스킬 입력을 버림
                _animator.SetInteger(_animIDSkill, 0);
            }
        }

        private void Block()
        {
            _animator.SetBool(_animIDBlock, _input.block);
        }

        private void Move()
        {
            Vector2 move;
            bool sprint;

            if (!IsIdleBlend)
            {
                move = Vector2.zero;
                sprint = false;
            }
            else
            {
                move = _input.move;
                sprint = _input.sprint;
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
            float inputMagnitude = _input.analogMovement ? move.magnitude : 1f;

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

            //Debug.Log("_speed " + _speed);
            // 점점 빨라지거나 점점 느려지는 효과
            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(move.x, 0.0f, move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            //Debug.Log("Move " + targetDirection.normalized * (_speed * Time.deltaTime) +
            //                 new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        public void Move(Vector3 delta)
        {
            _controller.Move(delta);

            //Debug.Log("delta " + delta);
            //Debug.Log("_controller.velocity " + _controller.velocity);

            // 제자리 이동이면 속도가 0으로 됨. -> 막기를 푼 직후 Move()에서 currentHorizontalSpeed에 이상한 값이 들어가는 것을 방지
            _controller.Move(Vector3.zero);
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
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
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
                _input.jump = false;
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
            if (ObjectToInteractWith != null && !@UIManager.IsInteractingWithPlayer)
            {
                //Debug.Log("TryDisplayInteractionKeyMessage");
                @UIManager.ActivateInteractionKeyMessage(ObjectToInteractWith.Type);
                ObjectToInteractWith.ActivateDetectedOutline();
                return;
            }

            @UIManager.DeactivateInteractionKeyMessage();
        }

        private void Interact()
        {
            if(_input.interaction && ObjectToInteractWith != null)
            {
                ObjectToInteractWith.Interact();
            }
        }

        private void LootAll()
        {
            if(_input.lootAll && @UIManager.IsOpenItemBoxWindow)
            {
                StructIdCount[] droppedItems = @UIManager.CurrentBeingOpenItemBox.Items.ToArray();
                Debug.Log($"PutAwayAll : {droppedItems.Length}");

                for (int itemIndex = 0; itemIndex < droppedItems.Length; itemIndex++)
                {
                    StructItemData itemData = DataBase.Items[droppedItems[itemIndex].Id];
                    if(itemData.ItemType == EnumItemType.Currency)
                    {
                        @Player.AddMoney(droppedItems[itemIndex].Count);
                        @UIManager.CurrentBeingOpenItemBox.RemoveItem(droppedItems[itemIndex].Id);
                    }
                    else if (@Player.AddItem(droppedItems[itemIndex].Id, droppedItems[itemIndex].Count) == ResultType.Success)
                    {
                        @UIManager.CurrentBeingOpenItemBox.RemoveItem(droppedItems[itemIndex].Id);
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
            if(@UIManager.IsOpenNpcServiceSelectionWindow)
            {
                if (_input.quit)
                {
                    @UIManager.CloseNpcServiceSelectionWindow();
                }

            }
        }

        private void CloseItemBoxWindow()
        {
            if (@UIManager.IsOpenItemBoxWindow)
            {
                if (_input.quit)
                {
                    @UIManager.CloseItemBoxWindow();
                }
                else if (@UIManager.CurrentBeingOpenItemBox != null && Vector3.Distance(transform.position, InGameUIManager.Instance.CurrentBeingOpenItemBox.transform.position) > MaxDistanceOnInteraction)
                {
                    @UIManager.CloseItemBoxWindow();
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
            if (@UIManager.IsInteractingWithPlayer)
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

                int exceptLayers = ~InteractableObject.InitialLayer;
                Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * CameraRayCastMaxDistance);

                // 카메라 방향으로 상호 작용 가능한 오브젝트 검사
                if (Physics.SphereCast(Camera.main.transform.position, Radius, Camera.main.transform.forward, out RaycastHit hit, CameraRayCastMaxDistance, exceptLayers, QueryTriggerInteraction.UseGlobal))
                {
                    ObjectToInteractWith = hit.transform.gameObject.GetComponent<InteractableObject>();
                }
            }
        }

        private void ToggleInventoryWindow()
        {
            if (_input.inventory)
            {
                //Debug.Log("@UIManager.IsOpenInventoryWindow: " + @UIManager.IsOpenInventoryWindow);
                //Debug.Log("@UIManager.IsInteractingWithPlayer: " + @UIManager.IsInteractingWithPlayer);
                if (!@UIManager.IsOpenInventoryWindow && !@UIManager.IsInteractingWithPlayer)
                {
                    @UIManager.OpenInventoryWindow();
                }
                else if(@UIManager.IsOpenInventoryWindow)
                {
                    @UIManager.CloseInventoryWindow();
                }
            }
        }

        private void ToggleSkillsWindow()
        {
            if (_input.skill)
            {
                //Debug.Log("@UIManager.IsOpenInventoryWindow: " + @UIManager.IsOpenInventoryWindow);
                //Debug.Log("@UIManager.IsInteractingWithPlayer: " + @UIManager.IsInteractingWithPlayer);
                if (!@UIManager.IsOpenSkillsWindow && !@UIManager.IsInteractingWithPlayer)
                {
                    @UIManager.OpenSkillsWindow();
                }
                else if (@UIManager.IsOpenSkillsWindow)
                {
                    @UIManager.CloseSkillsWindow();
                }
            }
        }

        private void TalkWithNpc()
        {
            if(@UIManager.IsOpenNpcServiceSelectionWindow)
            {
                if (_input.npcTalk)
                {
                    @UIManager.OpenDialogWindow();
                }
            }
        }

        private void NextNpcDialog()
        {
            if (@UIManager.IsOpenDialogWindow && @UIManager.IsPresentDialogNextButton)
            {
                if (_input.npcTalkNext)
                {
                    @UIManager.NextDialog();
                }
            }
        }

        private void CloseNpcDialogWindow()
        {
            if(@UIManager.IsOpenDialogWindow && @UIManager.IsPresentDialogQuitButton)
            {
                if(_input.quit)
                {
                    @UIManager.CloseDialogWindow();
                }
            }
        }

        private void OpenQuestSelectionWindow()
        {
            if (@UIManager.IsOpenNpcServiceSelectionWindow)
            {
                if (_input.npcQuest)
                {
                    @UIManager.OpenQuestSelectionWindow();
                }
            }
        }

        private void CloseQuestSelectionWindow()
        {
            if (@UIManager.IsOpenQuestSelectionWindow)
            {
                if (_input.quit)
                {
                    @UIManager.CloseQuestSelectionWindow();
                }
            }
        }

        private void AcceptQuest()
        {
            if (@UIManager.IsOpenNpcQuestDetailWindow)
            {
                if (_input.npcQuestAccept)
                {
                    @UIManager.AcceptNpcQuest();
                }
            }
        }

        private void CloseNpcQuestDetailWindow()
        {
            if (@UIManager.IsOpenNpcQuestDetailWindow)
            {
                if (_input.quit)
                {
                    @UIManager.CloseNpcQuestDetailWindow();
                }
            }
        }

        private void OpenNpcShopWindow()
        {
            if (@UIManager.IsOpenNpcServiceSelectionWindow)
            {
                if (_input.npcShop)
                {
                    InGameUIManager.Instance.OpenShop();
                }
            }
        }

        private void CloseNpcShopWindow()
        {
            if (@UIManager.IsOpenShopWindow)
            {
                if (_input.quit)
                {
                    InGameUIManager.Instance.CloseShop();
                }
            }
        }

        private void RequestNpcRest()
        {
            if (@UIManager.IsOpenNpcServiceSelectionWindow)
            {
                if (_input.npcRest)
                {
                    throw new System.NotImplementedException("not implemented npc rest");
                }
            }
        }

        private void PressQuickSlot()
        {
            if(_input.slot1)
            {
                @UIManager.UseQuickSlot(1);
            } 

            if (_input.slot2)
            {
                @UIManager.UseQuickSlot(2);
            }

            if (_input.slot3)
            {
                @UIManager.UseQuickSlot(3);
            }

            if (_input.slot4)
            {
                @UIManager.UseQuickSlot(4);
            }

            if (_input.slot5)
            {
                @UIManager.UseQuickSlot(5);
            }

            if (_input.slot6)
            {
                //Debug.Log("Quick 6");
                @UIManager.UseQuickSlot(6);
            }

            if (_input.slot7)
            {
                @UIManager.UseQuickSlot(7);
            }

            if (_input.slot8)
            {
                @UIManager.UseQuickSlot(8);
            }

            if (_input.slot9)
            {
                @UIManager.UseQuickSlot(9);
            }

            if (_input.slot0)
            {
                @UIManager.UseQuickSlot(0);
            }
        }

        private void ToggleEscMenu()
        {
            if (_input.esc)
            {
                //Debug.Log("Player.ToggleEscMenu()");
                InGameUIManager.Instance.ToggleEscWindow();
            }
        }
    }
}