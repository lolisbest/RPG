//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace RPG.Player
//{
//    public enum PlayerState
//    {
//        Idle = 0,
//        Walk,
//        Run,
//        Slash,
//        SlashThree,
//        OneHandSpellCasting,
//        SpellCasting,
//    }

//    public static class PlayerAnimatorStringHash
//    {
//        public readonly static int ParmX = Animator.StringToHash("X");
//        public readonly static int ParmZ = Animator.StringToHash("Z");
//        public readonly static int ParmState = Animator.StringToHash("State");
//        public readonly static int StateIdle = Animator.StringToHash("great sword idle");
//        public readonly static int StateWalk = Animator.StringToHash("great sword walk");
//        public readonly static int StateRun = Animator.StringToHash("great sword run");
//        public readonly static int StateSlash = Animator.StringToHash("great sword slash");

//    }

//    public class PlayerCharacterController : MonoBehaviour
//    {
//        private PlayerInputManager inputManager;
//        private CharacterController characterController;
//        public float gravityScale;
//        public float moveMultiply;
//        public Vector3 normalizedVelocity;
//        public Animator animator;

//#if UNITY_EDITOR
//        [SerializeField]
//        private bool isGrounded = false;
//        public bool IsGrounded 
//        { 
//            get
//            {
//                return isGrounded;
//            }
//            private set
//            {
//                isGrounded = value;
//            }
//        }
//#else
//        public bool IsGrounded { get; private set; }
//#endif

//        public LayerMask GroundLayers;
//        public float GroundedRadius;
//        public float GroundedOffset;

//        private void Awake()
//        {
//            Common.Singleton.Init();
//            characterController = GetComponent<CharacterController>();
//        }

//        void Start()
//        {
//            inputManager = PlayerInputManager.Instance;
//            Debug.Log("inputManager: " + PlayerInputManager.Instance.GetHashCode());

//        }

//        void SetZeroSpeed()
//        {

//        }

//        private void FixedUpdate()
//        {
//            GroundedCheck();

//            PlayerState currentState = (PlayerState)animator.GetInteger(PlayerAnimatorStringHash.ParmState);
//            bool slashButton = inputManager.Button0;
//            bool oneHandSpellButton = inputManager.Button1;
//            bool swordSpellButton = inputManager.Button2;

//            switch (currentState)
//            {
//                case PlayerState.Idle: case PlayerState.Walk: case PlayerState.Run:
//                    if (slashButton)
//                    {
//                        animator.SetInteger(PlayerAnimatorStringHash.ParmState, (int)PlayerState.Slash);
//                        SetZeroSpeed();
//                    }
//                    else if (oneHandSpellButton)
//                    {
//                        animator.SetInteger(PlayerAnimatorStringHash.ParmState, (int)PlayerState.OneHandSpellCasting);
//                        SetZeroSpeed();
//                    }
//                    else if (swordSpellButton)
//                    {
//                        animator.SetInteger(PlayerAnimatorStringHash.ParmState, (int)PlayerState.SpellCasting);
//                        SetZeroSpeed();
//                    }
//                    else
//                    {
//                        SetIdleWalkRunDir();
//                    }
//                    break;
//                case PlayerState.Slash:
//                    if (slashButton)
//                    {
//                        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
//                        // 연속 공격 애니메이션
//                        if (stateInfo.normalizedTime > 0.37f && stateInfo.normalizedTime < 0.57f)
//                        {
//                            animator.SetInteger(PlayerAnimatorStringHash.ParmState, (int)PlayerState.SlashThree);
//                        }
//                    }
//                    break;
//                case PlayerState.SlashThree:
//                    break;
//                case PlayerState.OneHandSpellCasting:
//                    break;
//                case PlayerState.SpellCasting:
//                    break;
//                default:
//                    return;
//            }

//            // 중력
//            characterController
//        }
//        /// <summary>
//        /// 속도 크기에 따라 Idle, Walk, Run 설정.
//        /// 움직이는 방향에 따라 회전도 포함
//        /// </summary>
//        void SetIdleWalkRunDir()
//        {
//            // 입력 값으로 속도 벡터 얻고
//            normalizedVelocity = new Vector3(inputManager.X, 0, inputManager.Z);

//            // 크기의 제곱이지만 제곱 값을 그냥 사용
//            // 속도 크기로 Idle, Walk, Run 설정
//            if (normalizedVelocity.sqrMagnitude > 0.7f)
//            {
//                animator.SetInteger(PlayerAnimatorStringHash.ParmState, (int)PlayerState.Run);
//            }
//            else if (normalizedVelocity.sqrMagnitude > 0f)
//            {
//                animator.SetInteger(PlayerAnimatorStringHash.ParmState, (int)PlayerState.Walk);
//            }
//            else
//            {
//                animator.SetInteger(PlayerAnimatorStringHash.ParmState, (int)PlayerState.Idle);
//            }

//            animator.SetFloat(PlayerAnimatorStringHash.ParmX, inputManager.X);
//            animator.SetFloat(PlayerAnimatorStringHash.ParmZ, inputManager.Z);
//            // 리지드 바디에 설정
//            rb.velocity = normalizedVelocity * moveMultiply;

//            // 회전
//            if (normalizedVelocity.sqrMagnitude > 0f)
//            {
//                Quaternion oldQuat = Quaternion.LookRotation(normalizedVelocity);
//                Quaternion newQuat = Quaternion.Slerp(rb.rotation, oldQuat, 0.3f);
//                rb.MoveRotation(newQuat);
//            }
//        }
//        private void GroundedCheck()
//        {
//            // set sphere position, with offset
//            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y + GroundedOffset,
//                transform.position.z);

//            IsGrounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
//                QueryTriggerInteraction.Ignore);

//            // update animator if using character
//            //if (_hasAnimator)
//            //{
//            //    _animator.SetBool(_animIDGrounded, Grounded);
//            //}
//        }

//#if UNITY_EDITOR
//        private void OnDrawGizmosSelected()
//        {
//            Gizmos.color = Color.green;
//            Vector3 center = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
//            Gizmos.DrawWireSphere(center, GroundedRadius);
//        }
//#endif
//        /// <summary>
//        /// 애니메이션 이벤트에 붙이는 용도
//        /// </summary>
//        void SetIdle()
//        {
//            animator.SetInteger(PlayerAnimatorStringHash.ParmState, (int)PlayerState.Idle);
//            inputManager.InitXZ();
//        }

//    }
//}