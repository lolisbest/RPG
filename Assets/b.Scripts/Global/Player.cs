using RPG.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.UI;
using RPG.Utils;
using RPG.Input;
using UnityEngine.SceneManagement;

public partial class Player : Singleton<Player>, IDamageable, IStatus
{
    #region IStatus Implements
    [SerializeField]
    private StructStatus _status;

    public StructRealStatus RealStatus { get; private set; }

    public Transform CameraRoot;

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
        throw new System.NotImplementedException();
    }
    #endregion

    #region Hp, Mp
    public int _hp;
    public int Hp
    {
        get => _hp;
        private set
        {
            Debug.Log($"_hp {_hp} -> {value}");
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

    public Dictionary<AttackCollider, bool> TakenDamageColliders;

    [SerializeField] private AnimationCurve _knockbackMultifly;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(StringStatic.PlaceBoundaryTag)) 
        {
            PlaceBoundary boundary = other.GetComponent<PlaceBoundary>();
            InGameUIManager.Instance.SetPlaceName(boundary.EnteringPlaceName);
            return;
        }

        if (other.CompareTag(StringStatic.MonsterAttackEffectTag))
        {
            AttackCollider monsterAttackCollider = other.GetComponent<AttackCollider>();
            if(monsterAttackCollider != null && !TakenDamageColliders.ContainsKey(monsterAttackCollider))
            {
                TakenDamageColliders.Add(monsterAttackCollider, false);

                //int realDamage = Calculate.RealDamage(monsterAttackCollider.Damage, RealStatus.Def);

                //if (realDamage > 0)
                //{
                //    //Debug.Log("Player take Damage : " + monsterAttackCollider.Damage);
                //    //Debug.Log("Playe Def : " + Def);
                //    //Debug.Log("Player take realDamage : " + realDamage);


                //    if (_controller.IsBlocking)
                //    {
                //        //Debug.Log("Block!!!");
                //        OnDamage(0);
                //    }
                //    else
                //    {
                //        OnDamage(realDamage);
                //    }
                //}
                //else
                //{
                //    OnDamage(0);
                //}
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

        Debug.Log("LoadPlayerData");
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

    void Awake()
    {
    }

    void Start()
    {
        SetPlayerData(GameManager.Instance.CurrentPlayerData);
        Debug.Log("Player Set Data " + GameManager.Instance.CurrentPlayerData);
        InGameUIManager.Instance.SetPlayerInstance(this);
        Debug.Log("Player Awake()");
        RecoveryAll();
        TakenDamageColliders = new();
    }

    void FixedUpdate()
    {

    }

    void Update()
    {
        if (!_defence) return;

        if(_defence.HasBlocked)
        {
            StartKnockback();
        }
    }

    void LateUpdate()
    {
        CheckDamage();
    }

    private void CheckDamage()
    {
        AttackCollider[] copyTakenDamageColliders = new AttackCollider[TakenDamageColliders.Count];
        int copyStartIndex = 0;
        TakenDamageColliders.Keys.CopyTo(copyTakenDamageColliders, copyStartIndex);

        for (int i = 0; i < copyTakenDamageColliders.Length; i++)
        {
            // 공격 콜라이더가 꺼지면 제거
            if (!copyTakenDamageColliders[i].gameObject.activeSelf)
            {
                TakenDamageColliders.Remove(copyTakenDamageColliders[i]);
            }
            else
            {
                if (TakenDamageColliders[copyTakenDamageColliders[i]] == false)
                {
                    // 아직 대미지 판정을 안 했다면

                    int realDamage = Calculate.RealDamage(copyTakenDamageColliders[i].Damage, RealStatus.Def);

                    if (copyTakenDamageColliders[i].IsBlocked)
                    {
                        // 막은 판정의 공격 콜라이더라면
                        // 데미지 1/5 수준으로
                        Debug.Log("막은 공격");
                        OnDamage(realDamage / 5);
                    }
                    else
                    {
                        Debug.Log("막지 못 한 공격");
                        OnDamage(realDamage);
                    }

                    // 판정 끝남을 체크
                    TakenDamageColliders[copyTakenDamageColliders[i]] = true;
                }
            }
        }
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
}
