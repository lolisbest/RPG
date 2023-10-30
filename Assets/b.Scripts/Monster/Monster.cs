using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Common;
using UnityEngine.AI;
using UnityEditor.Animations;

namespace RPG.Monster
{
    public enum MonsterState
    {
        Idle = 0,
        Rotate,
        Walk,
        Patrol,
        Attack,
        Skill,
        Run,

    }

    [RequireComponent(typeof(NavMeshAgent))]
    public partial class Monster : DamageableStatusMonoBehaviour
    {
        #region Animation String Hashs
        private readonly int _animState = Animator.StringToHash("State");
        private readonly int _animIdle = Animator.StringToHash("Idle");
        private readonly int _animWalk = Animator.StringToHash("Walk");
        private readonly int _animRotate = Animator.StringToHash("Rotate");
        private readonly int _animAttack = Animator.StringToHash("Attack");
        private readonly int _animAttackType = Animator.StringToHash("AttackType");
        private readonly int _animRun = Animator.StringToHash("Run");

        private readonly int _animStateIdle = Animator.StringToHash("Base Layer.Idle");
        private readonly int _animStateRotate = Animator.StringToHash("Base Layer.Rotate");
        private readonly int _animStateWalk = Animator.StringToHash("Base Layer.Walk");
        private readonly int _animStateRun = Animator.StringToHash("Base Layer.Run");
        private readonly int _animStateAttack = Animator.StringToHash("Base Layer.Attack");

        #endregion

        [SerializeField] private Transform _dropStartPosition;

        #region Monster Info
        public InGameMonsterUI MonsterUI;
        [SerializeField] private MonsterState _state;
        /// <summary>
        /// 1) Clear Animator Parameters. 2) Set State Parameter
        /// </summary>
        public MonsterState State
        {
            get => _state;
            private set
            {
                Debug.Log($"{name} {_state} -> {value}");

                _state = value;
                if (_anim == null)
                    return;

                ClearAnimatorBools();

                if (_state == MonsterState.Idle)
                {
                    _anim.SetBool(_animIdle, true);
                }
                else if (_state == MonsterState.Rotate)
                {
                    _accumulatedRotation = 0f;
                    _anim.SetBool(_animRotate, true);
                }
                else if (_state == MonsterState.Walk)
                {
                    _anim.SetBool(_animWalk, true);
                }
                else if (_state == MonsterState.Run)
                {
                    _anim.SetBool(_animRun, true);
                }
                else if (_state == MonsterState.Attack)
                {
                    // AttackIdle is exclusive
                    int attackType = Random.Range(0, AttackStateStrings.Length - 1);
                    _anim.SetBool(_animAttack, true);
                    _anim.SetInteger(_animAttackType, attackType);
                }
            }
        }

        private float _accumulatedRotation;

        public int Id;
        // 쫓아가는 최대 거리
        private float _followMaxDistance;
        // 제자리 상태 혹은 Walk, Idle, Attack 공격 조건으로 사용
        public float _attackRange = 5.5f;
        // Run 상태 일때 공격 조건으로 사용
        public float _moveStopDistance = 5f;
        public float _toleranceAngleToTargetDir = 5f;
        public float _dynamicRotationSpeed = 5f;
        public float _staticRotationSpeed = 8f;
        public float _walkSpeed = 2f;
        public float _runSpeed = 15f;
        public float _runDistance = 8.8f;

        public float SpawnInterval { get; set; }
        public float LeftTimeToSpawn { get; set; }
        #endregion

        [SerializeField] private QuestManager _questManager;
        [SerializeField] private ItemDropper _itemDropper;

        [SerializeField] private NavMeshAgent _naviMeshAgent;
        [SerializeField] private Animator _anim;
        public AnimatorStateInfo CurrentStateInfo { get => _anim.GetCurrentAnimatorStateInfo(0); }

        [SerializeField] private Transform _toAttackTarget;
        public Transform ToAttackTarget
        {
            get => _toAttackTarget;
            private set
            {
                _toAttackTarget = value;

                if (!_minimapIconRenderer) return;

                if (_toAttackTarget)
                {
                    _minimapIconRenderer.color = Color.red;
                }
                else
                {
                    _minimapIconRenderer.color = Color.green;
                }
            }
        }

        public AttackCollider[] _attackColliders;

        [SerializeField] public Collider _hitCollider;

        [SerializeField] private CharacterController _characterController;

        [SerializeField] private SpriteRenderer _minimapIconRenderer;

        [SerializeField] private string[] AttackStateStrings;
        [SerializeField] private int[] AttackStateHashes;

        private Vector3 _initialPosition;
        private Quaternion _initialRotation;

        /// <summary>
        /// 스폰 할 때 사용할 위치와 회전
        /// </summary>
        /// <param name="initialPosition"></param>
        /// <param name="initialRotation"></param>
        public void SetIntialPoseRot(Vector3 initialPosition, Quaternion initialRotation)
        {
            _initialPosition = initialPosition;
            _initialRotation = initialRotation;
        }

        void Awake()
        {
            Utils.CheckNull(_naviMeshAgent, "_naviMeshAgent is null");
            Utils.CheckNull(_anim, "_anim is null");
            Utils.CheckNull(_attackColliders, "_attackCollider is null");

            Utils.CheckNull(_dropStartPosition, "_dropStartPosition is null");

            SetAttackStateHashes();
            TakenHits = new();
        }

        void Start()
        {
            _naviMeshAgent.enabled = true;
            _naviMeshAgent.isStopped = true;

            _questManager = QuestManager.Instance;
            _itemDropper = ItemDropper.Instance;

            Spawn();
        }

        private void SetAttackStateHashes()
        {
            AttackStateHashes = new int[AttackStateStrings.Length];

            for(int i = 0; i < AttackStateStrings.Length; i++)
            {
                //Animator.StringToHash("Base Layer.Attack.BearAttackIdle");
                AttackStateHashes[i] = Animator.StringToHash(AttackStateStrings[i]);
            }
        }


        public void Spawn()
        {
            IsDie = false;
            State = MonsterState.Idle;
            Hp = RealStatus.MaxHp;
            LeftTimeToSpawn = SpawnInterval;
            Debug.Log("SpwanInterval " + SpawnInterval);
            gameObject.SetActive(true);
            ToAttackTarget = null;
            _naviMeshAgent.isStopped = true;
            transform.position = _initialPosition;
            transform.rotation = _initialRotation;
        }

        //private void OnTriggerEnter(Collider other)
        //{
        //    Debug.Log($"{name} OnTriggerEnter. ohter:{other.GetType()} + {other.gameObject.name}");
        //    //Debug.Log($"{name} collision with {other.transform.parent?.name}");
        //    if (other.CompareTag(StringStatic.PlayerAttackEffectTag))
        //    {
        //        AttackCollider attackCollider = other.gameObject.GetComponent<AttackCollider>();

        //        if (attackCollider != null)
        //        {
        //            StructAttackHit attackHit = new StructAttackHit
        //            {
        //                AttackCollider = attackCollider,
        //                AttackScriptId = attackCollider.GetHashCode(),
        //                IsBlocked = false,
        //                IsApplied = false,
        //                RawDamage = attackCollider.Damage,
        //                HitPosition = _dropStartPosition.position,
        //                Attacker = attackCollider.Attacker
        //            };

        //            OnDamage(attackHit);
        //        }
        //    }
        //}

        private void Update()
        {
            if (ToAttackTarget)
            {
                // 대상이 죽었다면 표적 풀기
                IDamageable target = ToAttackTarget.GetComponent<IDamageable>();
                //Debug.Log($"{ToAttackTarget} IDamageable {target != null}");
                if (target != null)
                {
                    if (target.IsDie) ToAttackTarget = null;
                    //Debug.Log("Target IsDie " + target.IsDie);
                }
            }

            switch (State)
            {
                case MonsterState.Idle:
                    Idle();
                    break;
                case MonsterState.Rotate:
                    Rotate();
                    break;
                case MonsterState.Walk:
                    Walk();
                    break;
                case MonsterState.Patrol:
                    Patrol();
                    break;
                case MonsterState.Attack:
                    Attack();
                    break;
                case MonsterState.Skill:
                    Skill();
                    break;
                case MonsterState.Run:
                    Run();
                    break;
                default:
                    break;
            }
        }
        
        void LateUpdate()
        {
            ApplyDamage();
        }

        private float? GetDistanceFromTarget(Transform target)
        {
            if (target == null)
                return null;

            return Vector3.Distance(transform.position, target.position);
        }

        private float? GetAngleToTarget(Transform target)
        {
            if (target == null)
                return null;

            Vector3 targetDir = target.position - transform.position;
            targetDir.y = 0f;
            return Vector3.Angle(targetDir, transform.forward);
        }

        /// <summary>
        /// Idle -> Patrol, Attack, Rotate, Walk
        /// </summary>
        private void Idle()
        {
            if (ToAttackTarget)
            {
                float? distance = GetDistanceFromTarget(ToAttackTarget);
                float? angle = GetAngleToTarget(ToAttackTarget);
                if (distance <= _attackRange && angle <= _toleranceAngleToTargetDir)
                {
                    State = MonsterState.Attack;
                }
                else
                {
                    // distance > _attackRange || angle > _toleranceAngleToTargetDir

                    if (angle > _toleranceAngleToTargetDir)
                    {
                        State = MonsterState.Rotate;
                    }
                    else
                    {
                        State = MonsterState.Walk;
                    }
                }
            }
            else
            {
                bool isTimeToPatrol = false;
                if (isTimeToPatrol)
                {
                    //State = MonsterState.Patrol;
                    throw new System.NotImplementedException("MonsterState.Patrol");
                }
                else
                {
                    ;
                }
            }
        }

        /// <summary>
        /// Rotate -> Walk, Idle
        /// </summary>
        private void Rotate()
        {
            if (ToAttackTarget)
            {
                float? distance = GetDistanceFromTarget(ToAttackTarget);
                float? angle = GetAngleToTarget(ToAttackTarget);

                float? arriveTime = distance / _runSpeed;
                float? focusTime = angle / _dynamicRotationSpeed;

                if (distance <= _moveStopDistance && angle <= _toleranceAngleToTargetDir)
                {
                    State = MonsterState.Attack;
                }
                if (focusTime > arriveTime)
                {
                    //Debug.Log($"distance : {distance}");
                    //Debug.Log($"angle : {angle}");
                    //Debug.Log($"arriveTime : {arriveTime}");
                    //Debug.Log($"focusTime : {focusTime}");
                    //Debug.Log($"_accumulatedRotation : {_accumulatedRotation}");
                    // 대상을 바라보는 시간이 도착하는 시간 보다 길면
                    RotateTo(ToAttackTarget.position, _staticRotationSpeed);
                }
                else
                {
                    State = MonsterState.Walk;
                }
            }
            else
            {
                State = MonsterState.Idle;
            }
        }

        /// <summary>
        /// 대상의 방향으로 조금씩 회전
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <param name="rotationSpeed"></param>
        private void RotateTo(Vector3 targetPosition, float rotationSpeed)
        {
            Vector3 targetDir = (targetPosition - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(targetDir, transform.up);

            transform.rotation = Quaternion.Slerp(Quaternion.LookRotation(targetDir), targetRotation, _accumulatedRotation);
            _accumulatedRotation = rotationSpeed * Time.deltaTime;
        }

        /// <summary>
        /// Move -> Attack, Idle, Run, Rotate
        /// </summary>
        private void Walk()
        {
            if (ToAttackTarget)
            {
                float? distance = GetDistanceFromTarget(ToAttackTarget);
                float? angle = GetAngleToTarget(ToAttackTarget);

                float? arriveTime = distance / _runSpeed;
                float? focusTime = angle / _dynamicRotationSpeed;

                if (distance <= _moveStopDistance && angle <= _toleranceAngleToTargetDir)
                {
                    State = MonsterState.Attack;
                }
                else if (focusTime > arriveTime)
                {
                    State = MonsterState.Rotate;
                }
                else if (distance > _runDistance)
                {
                    //Debug.Log($"Run : distance:{distance} > _runDistance:{_runDistance}");
                    State = MonsterState.Run;
                }
                else
                {
                    //if(_naviMeshAgent.isStopped)
                    //{
                    //    _naviMeshAgent.isStopped = false;
                    //}

                    GoTo(ToAttackTarget.position, _walkSpeed);
                    RotateTo(ToAttackTarget.position, _dynamicRotationSpeed);
                }
            }
            else
            {
                State = MonsterState.Idle;
            }
        }

        /// <summary>
        /// NaviMesh를 참조하여 대상 위치로 조금씩이동
        /// </summary>
        /// <param name="destPosition"></param>
        private void GoTo(Vector3 destPosition, float speed)
        {
            _naviMeshAgent.SetDestination(destPosition);

            if (_naviMeshAgent.path.corners.Length < 2)
                return;

            Vector3 nextCornerPosition = _naviMeshAgent.path.corners[1];

            _characterController.Move((nextCornerPosition - transform.position).normalized * speed * Time.deltaTime);
        }

        /// <summary>
        /// Attack -> Rotate, Walk, Idle.
        /// 공격 모션(상태)가 끝나기 전까지는 MonsterState와 상태 머신 변경 불가능
        /// </summary>
        private void Attack()
        {
            if (ToAttackTarget)
            {
                if (!IsInAttackStates() && _anim.GetBool(_animAttack))
                {
                    // 아직 전이 하지 않은 상태
                    return;
                }

                // Attack 으로 완전 전이됐다면
                if (IsInAttackStates())
                {
                    float attackClipExitTime = 0.9f;
                    if (CurrentStateInfo.normalizedTime < attackClipExitTime)
                    {
                        // 충분히 애니메이션이 재생되지 않았다면
                        //Debug.Log("CurrentStateInfo.normalizedTime < attackClipExitTime");
                        return;
                    }
                }

                State = MonsterState.Idle;
            }
            else
            {
                State = MonsterState.Idle;
            }
        }

        private void Patrol()
        {

        }

        private void Skill()
        {

        }

        /// <summary>
        /// Run -> Attack, Idle, Rotate
        /// </summary>
        private void Run()
        {
            if (ToAttackTarget)
            {
                float? distance = GetDistanceFromTarget(ToAttackTarget);
                float? angle = GetAngleToTarget(ToAttackTarget);

                float? arriveTime = distance / _runSpeed;
                float? focusTime = angle / _dynamicRotationSpeed;

                if (distance <= _moveStopDistance && angle <= _toleranceAngleToTargetDir)
                {
                    Debug.Log("_naviMeshAgent.isStopped " + _naviMeshAgent.isStopped);
                    State = MonsterState.Attack;
                }
                else if (focusTime > arriveTime)
                {
                    // 대상을 바라보는 시간이 도착하는 시간 보다 길면
                    State = MonsterState.Rotate;
                }
                else
                {
                    GoTo(ToAttackTarget.position, _runSpeed);
                    RotateTo(ToAttackTarget.position, _dynamicRotationSpeed);
                }
            }
            else
            {
                State = MonsterState.Idle;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _attackRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _runDistance);
        }

        private void DisplayAnimator(string message = "")
        {
            AnimatorStateInfo stateInfo = _anim.GetCurrentAnimatorStateInfo(0);
            string stateName;
            if (stateInfo.fullPathHash == _animStateIdle)
            {
                stateName = "Idle";
            }
            else if (stateInfo.fullPathHash == _animStateRotate)
            {
                stateName = "Rotate";
            }
            else if (stateInfo.fullPathHash == _animStateWalk)
            {
                stateName = "Walk";
            }
            else if (stateInfo.fullPathHash == _animStateRun)
            {
                stateName = "Run";
            }
            else if (stateInfo.fullPathHash == _animStateAttack)
            {
                stateName = "Attack";
            }
            else
            {
                stateName = "Unknown";
            }

            if (message == "")
            {
                Debug.Log($"MonsterState: {State}, _anim State Name: {stateName}, normalizedTime: {stateInfo.normalizedTime}\n" +
                    $"_animIdle: {_anim.GetBool(_animIdle)}\n" +
                    $"_animRotate: {_anim.GetBool(_animRotate)}\n" +
                    $"_animWalk: {_anim.GetBool(_animWalk)}\n" +
                    $"_animRun: {_anim.GetBool(_animRun)}\n" +
                    $"_animAttack: {_anim.GetBool(_animAttack)}\n" +
                    $"");
            }
            else
            {
                Debug.Log($"#{message} MonsterState: {State}, _anim State Name: {stateName}, normalizedTime: {stateInfo.normalizedTime}\n" +
                    $"_animIdle: {_anim.GetBool(_animIdle)}\n" +
                    $"_animRotate: {_anim.GetBool(_animRotate)}\n" +
                    $"_animWalk: {_anim.GetBool(_animWalk)}\n" +
                    $"_animRun: {_anim.GetBool(_animRun)}\n" +
                    $"_animAttack: {_anim.GetBool(_animAttack)}\n" +
                    $"");
            }
        }
        private void ClearAnimatorBools()
        {
            _anim.SetBool(_animIdle, false);
            _anim.SetBool(_animRotate, false);
            _anim.SetBool(_animWalk, false);
            _anim.SetBool(_animAttack, false);
            _anim.SetBool(_animRun, false);
        }

        private bool IsInAttackStates()
        {
            for (int i = 0; i < AttackStateHashes.Length; i++)
            {
                if (CurrentStateInfo.fullPathHash == AttackStateHashes[i])
                {
                    //Debug.Log(AttackStateStrings[i]);
                    return true;
                }
            }

            //Debug.Log("Not Attack State");
            return false;
        }

        public void SetMonsterDetails(int monsterId)
        {
            StructMonsterData monsterData = DataBase.Monsters[monsterId];
            Status = monsterData.Status;
            SpawnInterval = monsterData.SpawnInterval;
        }
    }
}