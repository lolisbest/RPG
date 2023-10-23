using RPG.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.UI;
using RPG.Utils;
using RPG.Input;
using UnityEngine.SceneManagement;
using System;

public partial class Player : Singleton<Player>, IDamageable, IStatus
{
    #region IStatus Implements
    [SerializeField]
    private StructStatus _status;

    public StructRealStatus RealStatus { get; private set; }

    public Transform CameraRoot;

    /// <summary>
    /// IsChangedStatus = true;
    /// </summary>
    public StructStatus Status
    {
        get => _status;
        private set
        {
            _status = value;

            int[] equipIds = CurrentEquips();

            IsChangedStatus = true;
            RealStatus = IStatus.UpdateRealStatus(_status, equipIds);
            _attackCollider.SetDamage(RealStatus.Atk);
        }
    }
    #endregion

    #region IDamageable Implements
    public bool IsDie { get; private set; }
    public void OnDamage(int damageAmount)
    {
        //Debug.Log("Player is Hit!!!");
        //Debug.Log($"Player Hp : {Hp} -> {Hp - damageAmount}");
        Hp -= damageAmount;

        if (Hp <= 0f)
        {
            OnDeath();
        }
    }

    public void OnDeath()
    {
        _controller.SetDeath();
        IsDie = true;
    }
    #endregion

    #region Hp, Mp
    public int _hp;
    public int Hp
    {
        get => _hp;
        private set
        {
            //Debug.Log($"_hp {_hp} -> {value}");
            _hp = value < 0 ? 0 : value;
            float rate = (float)_hp / RealStatus.MaxHp;
            InGameUIManager.Instance.UpdateHpGauge(rate);
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
            InGameUIManager.Instance.UpdateMpGauge(rate);
        }
    }
    #endregion

    public AttackCollider _attackCollider;
    public Animator _anim;

    public CustomThirdPersonController _controller;

    [SerializeField] private Defence _defence;

    public bool IsChangedStatus { get; set; }

    public Dictionary<int, StructAttackHit> TakenHits;

    [SerializeField] private AnimationCurve _knockbackMultifly;

    private void OnTriggerEnter(Collider other)
    {
        CheckAttack(other);
    }

    private void OnTriggerStay(Collider other)
    {
        CheckAttack(other);
    }

    private void CheckAttack(Collider other)
    {
        if (other.CompareTag(StringStatic.PlaceBoundaryTag))
        {
            PlaceBoundary boundary = other.GetComponent<PlaceBoundary>();
            InGameUIManager.Instance.SetPlaceName(boundary.EnteringPlaceName);
            return;
        }

        if (other.CompareTag(StringStatic.MonsterAttackEffectTag))
        {
            AttackCollider attackCollider = other.GetComponent<AttackCollider>();
            if (attackCollider != null)
            {
                //Debug.Log("Player.OnTriggerEnter");
                StructAttackHit attackhit = new StructAttackHit
                {
                    AttackCollider = attackCollider,
                    AttackScriptId = attackCollider.GetHashCode(),
                    IsBlocked = false,
                    IsApplied = false,
                    RawDamage = attackCollider.Damage
                };

                AddAttackHit(attackhit);
            }
        }
    }

    private void SetPlayerData(StructPlayerData playerData)
    {
        _structInventory = playerData.Inventory;
        _structInventory.Items = new StructInventorySlot[playerData.Inventory.InventorySlotNumber];

        for(int i = 0; i < _structInventory.Items.Length; i++)
        {
            _structInventory.Items[i] = new StructInventorySlot(-1, 0, i, false);
        }

        for (int i = 0; i < playerData.Inventory.Items.Length; i++)
        {
            StructInventorySlot loadedItem = playerData.Inventory.Items[i];

            if(loadedItem.SlotIndex >= _structInventory.Items.Length)
            {
                throw new System.Exception("loadedItem.SlotIndex >= _structInventory.Items.Length");
            }

            AddItem(loadedItem.SlotIndex, loadedItem.ItemId, loadedItem.ItemCount);
        }

        //Debug.Log("_structInventory:" + _structInventory);
        //Debug.Log("playerData.Status:" + playerData.Status);
        HumanEquipSlots = playerData.HumanEquipSlots;
        Status = playerData.Status;

        if (Status.AvailableSkillIds == null)
        {
            StructStatus status = Status;
            status.AvailableSkillIds = new int[0];
            Status = status;
        }
        //Debug.Log("LoadPlayerData");
        Debug.Log("LoadPlayerData " + Status.ToString());
    }

    public void AddExperience(int experienceAmount)
    {
        //Debug.Log($"Player AddExperience : Add {experienceAmount}");
        int currentExperience = Status.Experience;
        currentExperience += experienceAmount;
        // 프로퍼티로 구조체에 접근했으니 변경 결과를 받아서 저장해야 함
        Status = Status.SetExperience(currentExperience);
        IsChangedStatus = true;
    }

    public override void Initialize()
    {
        throw new System.NotImplementedException();
    }

    protected override void Awake()
    {
        base.Awake();
        _defence.OnCollision += AddAttackHit;
    }

    // 공격 피해를 담아 줌. 아직 적용된 피해가 아니고 중복되는 경우 막은 여부 취합.
    private void AddAttackHit(StructAttackHit attackHit)
    {
        //Debug.Log($"Add AttackHit {attackHit.AttackCollider.name} {attackHit.AttackCollider.gameObject.activeSelf}");
        if (TakenHits.ContainsKey(attackHit.AttackScriptId))
        {
            // 이미 존재하는 AttackCollider Id 라면
            StructAttackHit oldAttackHit = TakenHits[attackHit.AttackScriptId];

            // 이미 적용된 피해라면 변경 없이 메서드 종료
            if (oldAttackHit.IsApplied) return;

            // 막은 여부를 OR 연산 -> Player Collider와 Defence Collider가 동시에 닿았을 경우 방어 성공 판정
            oldAttackHit.IsBlocked |= attackHit.IsBlocked;

            TakenHits[oldAttackHit.AttackScriptId] = oldAttackHit;
        }
        else
        {
            // 새로운 공격 피해라면 추가
            TakenHits.Add(attackHit.AttackScriptId, attackHit);
        }
    }

    void Start()
    {
        SetPlayerData(GameManager.Instance.CurrentPlayerData);
        Debug.Log("Player Set Data " + GameManager.Instance.CurrentPlayerData);
        InGameUIManager.Instance.SetPlayerInstance(this);
        Debug.Log("Player Awake()");
        RecoveryAll();
        TakenHits = new();
        IsDie = false;
    }

    void FixedUpdate()
    {

    }

    void Update()
    {

    }

    void LateUpdate()
    {
        ApplyDamage();
    }

    public ResultType ActSkill(int skillId)
    {
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            StructSkillData skillData = DataBase.Skills[skillId];
            if (skillData.MpCost > Mp) return ResultType.SkillNotEnoughMP;

            Mp -= skillData.MpCost;
            _controller.Skill(skillId);

            return ResultType.SkillSuccess;
        }

        return ResultType.MouseEventOnObject;
    }

    private void ApplyDamage()
    {
        int[] attackHitIds = new int[TakenHits.Count];
        int copyStartIndex = 0;
        TakenHits.Keys.CopyTo(attackHitIds, copyStartIndex);

        for (int i = 0; i < attackHitIds.Length; i++)
        {
            int attackScriptId = attackHitIds[i];
            StructAttackHit attackHit = TakenHits[attackScriptId];

            //Debug.Log($"Attack " + attackHit.AttackCollider.name);

            if (!attackHit.AttackCollider.gameObject.activeSelf)
            {
                // 공격 콜라이더 오브젝트가 비활성화 되었다면 제거
                //Debug.Log("WasApplied Damage " + TakenHits.Count);
                TakenHits.Remove(attackScriptId);
                //Debug.Log("Removed Damage " + TakenHits.Count);
            }
            else
            {
                if (!attackHit.IsApplied)
                {
                    // 아직 대미지 판정을 안 했다면
                    //Debug.Log("Now Apply Damage");

                    int realDamage = Calculate.RealDamage(attackHit.RawDamage, RealStatus.Def);

                    if (attackHit.IsBlocked)
                    {
                        // 막은 판정의 공격 콜라이더라면
                        // 데미지 1/5 수준으로
                        _defence.OnBlockSuccess();
                        OnDamage(realDamage / 5);
                        StartKnockback();
                    }
                    else
                    {
                        //Debug.Log("Hit Attack");
                        OnDamage(realDamage);
                        Hit();
                    }

                    // 판정 끝남을 체크
                    attackHit.IsApplied = true;
                    TakenHits[attackScriptId] = attackHit;
                }
            }
        }
    }

    private void Hit()
    {
        _controller.SetHitAnimation();
    }

    private void RecoveryAll()
    {
        Hp = RealStatus.MaxHp;
        Mp = RealStatus.MaxMp;
    }

    public void SetStatus(StructStatus status)
    {
        Status = status;
        IsChangedStatus = true;
    }

    public void Spwan(Vector3 spwanPosition)
    {
        transform.position = spwanPosition;
        RecoveryAll();
        _controller.SetRespawn();
        IsDie = false;
    }

    public void OpenOnDeatthWindow()
    {
        InGameUIManager.Instance.OpenOnDeathWindow();
    }

    public StructPlayerData GetPlayerData()
    {
        StructPlayerData playerData = new();

        playerData.Status = _status;
        playerData.Inventory = _structInventory;
        playerData.HumanEquipSlots = _humanEquipSlots;
        
        /// todo:
        //playerData.DataId = -1
        //playerData.SpawnPlaceId = -1

        return playerData;
    }
}
