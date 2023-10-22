using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Common;
using UnityEngine.AI;
using RPG.Utils;

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
    public class Monster : MonoBehaviour, IStatus, IDamageable
    {
        #region Animation String Hashs
        private readonly int _animState = Animator.StringToHash("State");
        private readonly int _animIdle = Animator.StringToHash("Idle");
        private readonly int _animWalk = Animator.StringToHash("Walk");
        private readonly int _animRotate = Animator.StringToHash("Rotate");
        private readonly int _animAttack = Animator.StringToHash("Attack");
        private readonly int _animSkill = Animator.StringToHash("Skill");
        private readonly int _animRun = Animator.StringToHash("Run");

        private readonly int _animStateIdle = Animator.StringToHash("Base Layer.Idle");
        private readonly int _animStateRotate = Animator.StringToHash("Base Layer.Rotate");
        private readonly int _animStateWalk = Animator.StringToHash("Base Layer.Walk");
        private readonly int _animStateRun = Animator.StringToHash("Base Layer.Run");
        private readonly int _animStateAttack = Animator.StringToHash("Base Layer.Attack");

        #endregion

        #region IDamageable Property Implements
        public bool IsDie { get; protected set; }
        public void OnDeath()
        {
            //Debug.Log($"{name} die");
            @QuestManager.CallbackQuestCondition(QuestConditionType.Kill, Id, 1);
            IsDie = true;
            gameObject.SetActive(false);

            @ItemDropper.DropItemBox(Id, DropStartPosition.position);
        }

        public void OnDamage(int damageAmount)
        {
            //Debug.Log($"{name} : {Hp} -> {Hp - damageAmount}");

            //Debug.Log("DropStartPosition.position " + DropStartPosition.position);
            InGameUIManager.Instance.ShowDamageText(damageAmount, DropStartPosition.position);

            Hp -= damageAmount;
            if (Hp <= 0f)
            {
                OnDeath();
            }
        }
        #endregion

        private StructStatus _status;
        #region IStatus Implements
        public StructRealStatus RealStatus { get; private set; }
        public StructStatus Status
        {
            get => _status;
            private set
            {
                _status = value;

                RealStatus = IStatus.UpdateRealStatus(_status);
            }
        }

        #endregion

        #region Hp, Mp
        public int _hp;
        public int Hp
        {
            get => _hp;
            private set
            {
                _hp = value < 0 ? 0 : value;
                float rate = (float)_hp / RealStatus.MaxHp;
                MonsterUI.UpadteHp(rate);
                //Debug.Log("Hp Rate " + rate);
            }
        }

        public int _mp;
        public int Mp
        {
            get => _mp;
            private set
            {
                _mp = value < 0 ? 0 : value;
                float rate = (float)_mp / RealStatus.MaxMp;
            }
        }
        #endregion

        public Vector3 RespawnPosition { get; private set; }

        public Transform DropStartPosition;

        #region Monster Info
        public InGameMonsterUI MonsterUI;
        [SerializeField]
        private MonsterState _state;
        public MonsterState State 
        { 
            get => _state; 
            private set
            {
                _state = value;
                if (_anim == null)
                    return;

                ClearAnimatorBools();

                if(_state == MonsterState.Idle)
                {
                    _anim.SetBool(_animIdle, true);
                }
                else if (_state == MonsterState.Rotate)
                {
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
                    _anim.SetBool(_animAttack, true);
                }
            }
        }
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

        public float RespawnInterval { get; set; } = 2f;
        public float LeftTimeToSpawn { get; set; }
        #endregion

        public QuestManager @QuestManager;
        public ItemDropper @ItemDropper;

        private NavMeshAgent _naviMeshAgent;
        private Animator _anim;
        public AnimatorStateInfo CurrentStateInfo { get => _anim.GetCurrentAnimatorStateInfo(0); }

        [SerializeField] private Transform _toAttackTarget;
        public Transform ToAttackTarget
        {
            get => _toAttackTarget;
            private set
            {
                _toAttackTarget = value;

                if (!_minimapIconRenderer) return;

                if(_toAttackTarget)
                {
                    _minimapIconRenderer.color = Color.red;
                }
                else
                {
                    _minimapIconRenderer.color = Color.green;
                }
            }
        }

        public AttackCollider _attackCollider;

        public bool IsStopped;

        public Collider HitCollider;

        [SerializeField] private CharacterController _controller;

        [SerializeField] private SpriteRenderer _minimapIconRenderer;

        #region Public Methods
        #endregion

        #region Private Methods
        #endregion

        void Start()
        {
            MapManager.Instance.AddMonster(this);

            HitCollider.isTrigger = true;

            @QuestManager = QuestManager.Instance;
            @ItemDropper = ItemDropper.Instance;

            Status = DataBase.Monsters[Id].Status;
            name = $"{Status.Name}:{gameObject.GetHashCode()}";
            MonsterUI.SetName(Status.Name);

            Hp = RealStatus.MaxHp;


            if (TryGetComponent(out _naviMeshAgent))
            {
                _naviMeshAgent.isStopped = true;
                //Debug.Log("_naviMeshAgent.isStopped : " + _naviMeshAgent.isStopped);
            }
            
            _attackCollider.SetDamage(RealStatus.Atk);

            if(!TryGetComponent(out _anim))
            {
                throw new System.Exception("_anim is null");
            }

            IsDie = false;
            State = MonsterState.Idle;
            RespawnPosition = transform.position;
            LeftTimeToSpawn = RespawnInterval;
            ToAttackTarget = ToAttackTarget;
        }

        public void Spawn()
        {
            IsDie = false;
            State = MonsterState.Idle;
            transform.position = RespawnPosition;
            Hp = RealStatus.MaxHp;
            LeftTimeToSpawn = RespawnInterval;
            gameObject.SetActive(true);
            ToAttackTarget = null;
            _naviMeshAgent.isStopped = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            //Debug.Log($"{name} collision with {other.transform.parent?.name}");
            if (other.CompareTag(StringStatic.PlayerAttackEffectTag))
            {
                AttackCollider attackCollider = other.gameObject.GetComponent<AttackCollider>();
                if(attackCollider != null)
                {
                    //Debug.Log("playeryAttackCollider.Damage: " + playeryAttackCollider.Damage);
                    int realDamage = Calculate.RealDamage(attackCollider.Damage, RealStatus.Def);
                    if (realDamage != 0)
                    {
                        OnDamage(realDamage);

                        if (!ToAttackTarget)
                        {
                            ToAttackTarget = attackCollider.Body;
                        }
                    }
                    else
                    {
                        OnDamage(0);
                    }
                }
            }
        }

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
                float? angleToTarget = GetAngleToTarget(ToAttackTarget);
                if (angleToTarget < _toleranceAngleToTargetDir)
                {
                    State = MonsterState.Walk;
                }
                else
                {
                    RotateTo(ToAttackTarget.position, _staticRotationSpeed);
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
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
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
                if (distance <= _attackRange && angle <= _toleranceAngleToTargetDir)
                {
                    State = MonsterState.Attack;
                }
                else if(angle > _toleranceAngleToTargetDir)
                {
                    State = MonsterState.Rotate;
                }
                else if(distance > _runDistance)
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

            _controller.Move((nextCornerPosition - transform.position).normalized * speed * Time.deltaTime);
        }

        /// <summary>
        /// Attack -> Rotate, Walk, Idle.
        /// 공격 모션(상태)가 끝나기 전까지는 MonsterState와 상태 머신 변경 불가능
        /// </summary>
        private void Attack()
        {
            if (ToAttackTarget)
            {
                if (CurrentStateInfo.fullPathHash != _animStateAttack && _anim.GetBool(_animAttack))
                {
                    // 아직 전이 하지 않은 상태
                    return;
                }

                if (CurrentStateInfo.fullPathHash == _animStateAttack)
                {
                    float attackClipExitTime = 0.9f;
                    if (CurrentStateInfo.normalizedTime < attackClipExitTime)
                    {
                        return;
                    }
                    else
                    {
                        //Debug.Log("Attack.normalizedTime >= 0.75");
                    }
                }

                float? distance = GetDistanceFromTarget(ToAttackTarget);
                float? angle = GetAngleToTarget(ToAttackTarget);
                //Debug.Log($"Attack : distance: {distance}, angle: {angle}");
                if (distance <= _attackRange && angle <= _toleranceAngleToTargetDir)
                {
                    //Debug.Log($"Attack Stay");
                }
                else
                {
                    if(angle > _toleranceAngleToTargetDir)
                    {
                        State = MonsterState.Rotate;
                        Debug.Log($"Attack => {State}");
                    }
                    else
                    {
                        State = MonsterState.Walk;
                        Debug.Log($"Attack => {State}");
                    }
                }
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
                //else if (distance <= _moveStopDistance * 1.1f)
                //{
                //    //Debug.Log($"Run : distance:{distance} <= _moveStopDistance * 1.2f:{_moveStopDistance * 1.2f}");
                //    State = MonsterState.Walk;
                //}
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
            if(stateInfo.fullPathHash == _animStateIdle)
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

            if(message == "")
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
    }
}