using RPG.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.UI;
using RPG.Input;
using UnityEngine.SceneManagement;
using System;

public partial class Player : DamageableStatusMonoBehaviour
{
    public Transform CameraRoot;

    [SerializeField] private Transform _actionSkillPosition;
    [SerializeField] private Transform _projectileSkillPosition;

    [SerializeField] private CustomThirdPersonController _inputController;
    [SerializeField] private CharacterController _characterController;

    [SerializeField] private Defence _defence;

    [SerializeField] private AnimationCurve _knockbackMultifly;

    [SerializeField] private Skill _loadedSkill;
    [SerializeField] private AttackCollider _baseSlashAttackCollider;

    //private Vector3 _formerPosition;

    //public bool IsHitFromMonster { get; private set; }



    //private void OnTriggerEnter(Collider other)
    //{
    //    CheckTrigger(other);
    //}

    //private void OnTriggerStay(Collider other)
    //{
    //    CheckTrigger(other);
    //}

    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log("OnCollision " + collision.gameObject.name);
    //    CheckTrigger(collision);
    //}

    //private void OnCollisionStay(Collision collision)
    //{
    //    Debug.Log("OnCollision " + collision.gameObject.name);
    //    CheckTrigger(collision);
    //}

    private void CheckTrigger(Collider other)
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
                Vector3 hitPosition = Vector3.zero;

                //Debug.Log("Player.OnTriggerEnter");
                StructAttackHit attackhit = new StructAttackHit
                {
                    AttackCollider = attackCollider,
                    AttackScriptId = attackCollider.GetHashCode(),
                    IsBlocked = false,
                    IsApplied = false,
                    RawDamage = attackCollider.Damage,
                    HitPosition = hitPosition,
                    Attacker = attackCollider.Attacker
                };

                AddAttackHit(attackhit);
            }
        }
    }

    //private void CheckTrigger(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag(StringStatic.PlaceBoundaryTag))
    //    {
    //        PlaceBoundary boundary = collision.gameObject.GetComponent<PlaceBoundary>();
    //        InGameUIManager.Instance.SetPlaceName(boundary.EnteringPlaceName);
    //        return;
    //    }

    //    if (collision.gameObject.CompareTag(StringStatic.MonsterAttackEffectTag))
    //    {
    //        AttackCollider attackCollider = collision.gameObject.GetComponent<AttackCollider>();
    //        if (attackCollider != null)
    //        {
    //            Debug.Log("OnCollision " + attackCollider.name);

    //            Vector3 sum = Vector3.zero;

    //            foreach (var contact in collision.contacts)
    //            {
    //                sum += contact.point;
    //            }

    //            Vector3 averagePosition = sum / collision.contacts.Length;

    //            StructAttackHit attackhit = new StructAttackHit
    //            {
    //                AttackCollider = attackCollider,
    //                AttackScriptId = attackCollider.GetHashCode(),
    //                IsBlocked = false,
    //                IsApplied = false,
    //                RawDamage = attackCollider.Damage,
    //                HitPosition = averagePosition,
    //                Body = transform
    //            };

    //            AddAttackHit(attackhit);

    //            //IsHitFromMonster = true;

    //            //_characterController.enabled = false;
    //            //transform.position = _formerPosition;
    //            //_characterController.enabled = true;

    //            Debug.Log("OnCollision character controller.velocity " + _characterController.velocity);
    //            //Debug.Log("OnCollision character controller.velocity " + _characterController);
    //        }
    //    }
    //}

    private  void Awake()
    {
        _defence.OnCollision += AddAttackHit;
        _baseSlashAttackCollider.SetAttacker(transform);
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
        //_formerPosition = transform.position;
        //IsHitFromMonster = false;
    }

    void Update()
    {
        //if (IsHitFromMonster)
        //{
        //    _characterController.enabled = false;
        //    transform.position = _formerPosition;
        //    _characterController.enabled = true;
        //    IsHitFromMonster = false;
        //}
    }

    void LateUpdate()
    {
        ApplyDamage();
    }

    private void SetPlayerData(StructPlayerData playerData)
    {
        _structInventory = playerData.Inventory;
        _structInventory.Items = new StructInventorySlot[playerData.Inventory.InventorySlotNumber];

        for (int i = 0; i < _structInventory.Items.Length; i++)
        {
            _structInventory.Items[i] = new StructInventorySlot(-1, 0, i, false);
        }

        for (int i = 0; i < playerData.Inventory.Items.Length; i++)
        {
            StructInventorySlot loadedItem = playerData.Inventory.Items[i];

            if (loadedItem.SlotIndex >= _structInventory.Items.Length)
            {
                throw new System.Exception("loadedItem.SlotIndex >= _structInventory.Items.Length");
            }

            AddItem(loadedItem.SlotIndex, loadedItem.ItemId, loadedItem.ItemCount, loadedItem.IsOnEquip);
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


    public ResultType LoadSkill(int skillId)
    {
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            StructSkillData skillData = DataBase.Skills[skillId];
            if (skillData.MpCost > Mp) return ResultType.SkillNotEnoughMP;

            if (!_inputController.Skill(skillData.Type)) return ResultType.SkillBeingDifferentAnimation;

            Mp -= skillData.MpCost;

            Debug.Log($"Player.ActSkill() {skillData.Name}");
            Debug.Log("Create skillObject");
            GameObject skillObject = Instantiate(skillData.Prefab);
            Debug.Log("Created skillObject.position0: " + skillObject.transform.position);
            Skill skill = skillObject.GetComponent<Skill>();
            skill.SetAttacker(transform);
            skill.SetDamage(RealStatus.Atk);

            if (skillData.Type == EnumSkillType.Action)
            {
                skill.SetTransformState(_actionSkillPosition);
            }
            else if (skillData.Type == EnumSkillType.Projectile)
            {
                _inputController.AlignPlayerDirectionWithCamera();
                skill.SetTransformState(_projectileSkillPosition);
                (skill as ProjectileSkill).SetDirection(Camera.main.transform.forward);
            }
            Debug.Log("Created skillObject.position1: " + skillObject.transform.position);
            _loadedSkill = skill;

            return ResultType.SkillSuccess;
        }

        return ResultType.MouseEventOnObject;
    }

    public void ActivateSkill()
    {
        _loadedSkill.On();
    }

    private void RecoveryAll()
    {
        Hp = RealStatus.MaxHp;
        Mp = RealStatus.MaxMp;
    }

    public void Spwan(Vector3 spwanPosition)
    {
        transform.position = spwanPosition;
        RecoveryAll();
        _inputController.SetRespawn();
        IsDie = false;
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
