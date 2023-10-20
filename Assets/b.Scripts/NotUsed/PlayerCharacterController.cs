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
//                        // ���� ���� �ִϸ��̼�
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

//            // �߷�
//            characterController
//        }
//        /// <summary>
//        /// �ӵ� ũ�⿡ ���� Idle, Walk, Run ����.
//        /// �����̴� ���⿡ ���� ȸ���� ����
//        /// </summary>
//        void SetIdleWalkRunDir()
//        {
//            // �Է� ������ �ӵ� ���� ���
//            normalizedVelocity = new Vector3(inputManager.X, 0, inputManager.Z);

//            // ũ���� ���������� ���� ���� �׳� ���
//            // �ӵ� ũ��� Idle, Walk, Run ����
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
//            // ������ �ٵ� ����
//            rb.velocity = normalizedVelocity * moveMultiply;

//            // ȸ��
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
//        /// �ִϸ��̼� �̺�Ʈ�� ���̴� �뵵
//        /// </summary>
//        void SetIdle()
//        {
//            animator.SetInteger(PlayerAnimatorStringHash.ParmState, (int)PlayerState.Idle);
//            inputManager.InitXZ();
//        }

//    }
//}