using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RPG.Common;
using RPG.UI;
using System.Linq;

public partial class Player
{
    [SerializeField]
    private StructInventory _structInventory;
    public StructInventorySlot[] Items { get => _structInventory.Items; }

    [SerializeField]
    private StructHumanEquipSlots _humanEquipSlots;

    /// <summary>
    /// set 할 시에 IsChangedStatus = true; IsChangedInventory = true;
    /// </summary>
    public StructHumanEquipSlots HumanEquipSlots
    {
        get => _humanEquipSlots;
        private set
        {
            _humanEquipSlots = value;
            IsChangedInventory = true;

            RealStatus = IStatus.UpdateRealStatus(Status, CurrentEquips());
            IsChangedStatus = true;
        }
    }

    private bool _isChangedInventory;
    /// <summary>
    /// true가 할당되면 UpdateStatus 호출
    /// </summary>
    public bool IsChangedInventory
    {
        get => _isChangedInventory;
        set
        {
            _isChangedInventory = value;
        }
    }

    public int Money { get => _structInventory.Money; }

    public static bool UseItem(int itemSlotIndex)
    {
        return false;
    }

    public bool Pay(int moneyAmount)
    {
        if (_structInventory.Money - moneyAmount >= 0)
        {
            _structInventory.Money -= moneyAmount;
            IsChangedInventory = true;
            return true;
        }

        return false;
    }

    /// <summary>
    /// 아이템의 유형에 따라 단일 혹은 묶음 형태로 저장
    /// </summary>
    /// <param name="itemId"></param>
    /// <param name="itemCount"></param>
    /// <returns></returns>
    public ResultType AddItem(int itemId, int itemCount)
    {
        // 빈 슬롯 찾기
        int addIndex = GetEmptySlotIndex();

        StructItemData itemData = DataBase.Items[itemId];
        // 화폐인지?
        if (itemData.ItemType == EnumItemType.Currency)
        {
            AddMoney(itemCount);
        }
        // 묶을 수 있는 아이템인지?
        else if (
            itemData.ItemType == EnumItemType.Consumable ||
            itemData.ItemType == EnumItemType.Etc
            )
        {
            // 묶음 아이템이면, 인벤토리에 존재하는 동일한 아이템 찾기
            int sameItemIndex = GetSameItemSlotIndex(itemId);
            if (sameItemIndex == -1)
            {
                if (addIndex == -1)
                {
                    // 동일한 묶음 아이템이 없어서 새로운 슬롯을 사용해야하고
                    // 빈 슬롯이 없을 떄
                    return ResultType.NoEmptySlot;
                }

                // 동일한 아이템이 없다면 빈 슬롯에 아이템 추가
                _structInventory.Items[addIndex].ItemId = itemId;
                _structInventory.Items[addIndex].ItemCount = itemCount;
            }
            else
            {
                // 동일한 아이템에 개수 추가
                _structInventory.Items[sameItemIndex].ItemCount += itemCount;
            }
        }
        else
        {
            if (addIndex == -1)
            {
                // 묶을 수 없는 아이템이고
                // 빈 슬롯이 없을 떄
                return ResultType.NoEmptySlot;
            }

            // 묶음 아이템이 아니므로 빈 슬롯에 추가
            _structInventory.Items[addIndex].ItemId = itemId;
            _structInventory.Items[addIndex].ItemCount = itemCount;
        }

        IsChangedInventory = true;
        return ResultType.Success;
    }

    /// <summary>
    /// 특정 슬롯에 아이템 추가, 슬롯이 사용 중이라면 빈 슬롯에 추가
    /// </summary>
    /// <param name="itemSlotIndex"></param>
    /// <param name="itemId"></param>
    /// <param name="itemCount"></param>
    /// <returns></returns>
    public ResultType AddItem(int itemSlotIndex, int itemId, int itemCount, bool isEquiped)
    {
        if (_structInventory.Items[itemSlotIndex].ItemId != -1)
        {
            // 이미 사용 중인 아이템 슬롯
            // 다른 임의의 슬롯에 저장
            return AddItem(itemId, itemCount);
        }

        // 사용 중인 슬롯이 아니므로 아이템 추가
        _structInventory.Items[itemSlotIndex].ItemId = itemId;
        _structInventory.Items[itemSlotIndex].ItemCount = itemCount;
        _structInventory.Items[itemSlotIndex].IsOnEquip = isEquiped;
        IsChangedInventory = true;
        return ResultType.Success;
    }

    public bool CreateSlots(int inventoryLength)
    {
        // 이 메서드를 인벤토리 공간을 축소시키는 용도로 사용할 경우 문제가 발생.
        // 그리고 게임 도중에 사용할 경우 기존의 아이템이 사라짐. 반드시 게임 실행 시작에만 사용 할 것.

        _structInventory.Items = new StructInventorySlot[inventoryLength];
        _structInventory.InventorySlotNumber = inventoryLength;

        for (int i = 0; i < inventoryLength; i++)
        {
            _structInventory.Items[i] = new StructInventorySlot() { ItemId = -1, ItemCount = 0, SlotIndex = i };
        }

        IsChangedInventory = true;
        return true;
    }

    public bool AddMoney(int moneyAmount)
    {
        if (_structInventory.Money + moneyAmount > _structInventory.MaxMoney)
        {
            Debug.Log($"{_structInventory.Money + moneyAmount - _structInventory.MaxMoney} 골드를 소지하지 못 합니다.");
            // 다르면 변화가 있음 -> true
            IsChangedInventory = _structInventory.Money != _structInventory.MaxMoney;
            _structInventory.Money = _structInventory.MaxMoney;
            return false;
        }

        _structInventory.Money += moneyAmount;
        IsChangedInventory = true;
        return true;
    }

    private int GetEmptySlotIndex()
    {
        for (int i = 0; i < _structInventory.Items.Length; i++)
        {
            if (_structInventory.Items[i].ItemId == -1)
            {
                return i;
            }
        }

        return -1;
    }

    private int GetSameItemSlotIndex(int itemId)
    {
        for (int i = 0; i < _structInventory.Items.Length; i++)
        {
            if (itemId == _structInventory.Items[i].ItemId)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// 사용 실패는 -1, 사용 성공 시 소모용 아이템의 남은 개수 반환
    /// </summary>
    /// <param name="slotIndex"></param>
    /// <returns></returns>
    public int ConsumeItem(int slotIndex)
    {
        Debug.Log("ConsumeItem");

        int itemId = _structInventory.Items[slotIndex].ItemId;
        StructItemData itemData = DataBase.Items[itemId];

        if (itemData.SkillId > 0)
        {
            // 이미 습득한 스킬이면 사용 안 함
            if (Status.AvailableSkillIds.Contains(itemData.SkillId))
            {
                Debug.Log($"Already learned skill. {DataBase.Skills[itemData.SkillId].Name}");
                return _structInventory.Items[slotIndex].ItemCount;
            }
        }

        if (itemData.RecoveryHpAmount > 0 && Hp >= RealStatus.MaxHp)
        {
            // 사용 실패
            Debug.Log($"Fail to use `{itemData.Name}`. Already Hp is full.");
            return -1;
        }

        if (itemData.RecoveryMpAmount > 0 && Mp >= RealStatus.MaxMp)
        {
            // 사용 실패
            Debug.Log($"Fail to use `{itemData.Name}`. Already Mp is full.");
            return -1;
        }

        Hp += itemData.RecoveryHpAmount;
        Mp += itemData.RecoveryMpAmount;

        AddSkill(itemData.SkillId);

        _structInventory.Items[slotIndex].ItemCount -= 1;

        IsChangedInventory = true;

        if (_structInventory.Items[slotIndex].ItemCount == 0)
        {
            RemoveItem(slotIndex);
            InGameUIManager.Instance.CLoseInventoryItemInfoWindow();
        }
        else if (_structInventory.Items[slotIndex].ItemCount < 0)
        {
            throw new Exception($"InventorySlot#{slotIndex}'s count < 0");
        }

        return _structInventory.Items[slotIndex].ItemCount;
    }

    private void AddSkill(int skillId)
    {
        StructStatus newStatus = Status;
        int newSize = Status.AvailableSkillIds.Length + 1;
        Array.Resize(ref newStatus.AvailableSkillIds, newSize);
        newStatus.AvailableSkillIds[newSize-1] = skillId;
        Status = newStatus;
        Debug.Log($"New Player Available Skills {string.Join(",", newStatus.AvailableSkillIds)}");
    }

    private void RemoveItem(int slotIndex)
    {
        _structInventory.Items[slotIndex].ItemId = -1;
        _structInventory.Items[slotIndex].ItemCount = 0;
        IsChangedInventory = true;
    }

    public void EquipItem(int slotIndex)
    {
        int itemId = Items[slotIndex].ItemId;
        if (itemId == -1)
            return;

        StructItemData itemData = DataBase.Items[itemId];
        if (itemData.ItemType != EnumItemType.Equipment)
        {
            return;
            throw new Exception($"Not Equipment. slot : {slotIndex}, item Name : {itemData.Name}, Type : {itemData.ItemType}");
        }

        int previousEquipSlotIndex;
        StructHumanEquipSlots newHumanEquipSlots = HumanEquipSlots;

        switch (itemData.EquipType)
        {
            case EnumEquipType.Head:
                previousEquipSlotIndex = HumanEquipSlots.HeadIndex;
                newHumanEquipSlots.HeadIndex = slotIndex;
                break;
            case EnumEquipType.Chest:
                previousEquipSlotIndex = HumanEquipSlots.ChestIndex;
                newHumanEquipSlots.ChestIndex = slotIndex;
                break;
            case EnumEquipType.Hand0:
                previousEquipSlotIndex = HumanEquipSlots.HandIndex0;
                newHumanEquipSlots.HandIndex0 = slotIndex;
                break;
            case EnumEquipType.Hand1:
                previousEquipSlotIndex = HumanEquipSlots.HandIndex1;
                newHumanEquipSlots.HandIndex1 = slotIndex;
                break;
            case EnumEquipType.Foot:
                previousEquipSlotIndex = HumanEquipSlots.FootIndex;
                newHumanEquipSlots.FootIndex = slotIndex;
                break;
            default:
                throw new Exception($"Equip Fail. Unknown EquipType. slot : {slotIndex}, item Name : {itemData.Name}, EquipType : {itemData.EquipType}");
        }

        if (previousEquipSlotIndex != -1)
        {
            Items[previousEquipSlotIndex].IsOnEquip = false;
        }

        HumanEquipSlots = newHumanEquipSlots;
        Items[slotIndex].IsOnEquip = true;
    }

    public void UnequipItem(int slotIndex)
    {
        int itemId = Items[slotIndex].ItemId;
        if(itemId == -1)
        {
            return;
        }

        if (!Items[slotIndex].IsOnEquip)
        {
            return;
        }

        StructItemData itemData = DataBase.Items[itemId];
        if (itemData.ItemType != EnumItemType.Equipment)
        {
            return;
            throw new Exception($"Current Equipment is not equipment. slot : {slotIndex}, item Name : {itemData.Name}, Type : {itemData.ItemType}");
        }

        StructHumanEquipSlots newHumanEquipSlots = HumanEquipSlots;

        switch (itemData.EquipType)
        {
            case EnumEquipType.Head:
                newHumanEquipSlots.HeadIndex = -1;
                break;
            case EnumEquipType.Chest:
                newHumanEquipSlots.ChestIndex = -1;
                break;
            case EnumEquipType.Hand0:
                newHumanEquipSlots.HandIndex0 = -1;
                break;
            case EnumEquipType.Hand1:
                newHumanEquipSlots.HandIndex1 = -1;
                break;
            case EnumEquipType.Foot:
                newHumanEquipSlots.FootIndex = -1;
                break;
            default:
                throw new Exception($"Unquip Fail. Unknown EquipType. slot : {slotIndex}, item Name : {itemData.Name}, EquipType : {itemData.EquipType}");
        }

        HumanEquipSlots = newHumanEquipSlots;
        Items[slotIndex].IsOnEquip = false;
    }

    public void SwapItemSlot(int leftSlotIndex, int rightSlotIndex)
    {
        StructInventorySlot leftSlot = Items[leftSlotIndex];
        StructInventorySlot rightSlot = Items[rightSlotIndex];

        UnequipItem(leftSlotIndex);
        UnequipItem(rightSlotIndex);

        rightSlot.SlotIndex = leftSlotIndex;
        leftSlot.SlotIndex = rightSlotIndex;

        Items[leftSlotIndex] = rightSlot;
        Items[rightSlotIndex] = leftSlot;

        if (Items[leftSlotIndex].IsOnEquip)
        {
            EquipItem(leftSlotIndex);
        }
        
        if(Items[rightSlotIndex].IsOnEquip)
        {
            EquipItem(rightSlotIndex);
        }

        IsChangedInventory = true;
    }

    public int[] CurrentEquips()
    {
        int[] equipSlotIndexes = new int[] {
                HumanEquipSlots.HeadIndex, HumanEquipSlots.ChestIndex, HumanEquipSlots.HandIndex0,
                HumanEquipSlots.HandIndex1, HumanEquipSlots.FootIndex,
            };

        int[] equipIds = equipSlotIndexes.Where(x => x != -1).Select(x => Items[x].ItemId).ToArray();

        return equipIds;
    }

    public ResultType SellItem(int inventorySlotIndex, int sellCount)
    {
        if (inventorySlotIndex == -1)
        {
            return ResultType.WrongSlotIndex;
        }

        StructInventorySlot changedInventorySlot = Items[inventorySlotIndex];

        if(changedInventorySlot.ItemId > 0)
        {
            if(changedInventorySlot.ItemCount >= sellCount)
            {
                StructItemData itemData = DataBase.Items[changedInventorySlot.ItemId];
                changedInventorySlot.ItemCount -= sellCount;
                if(changedInventorySlot.ItemCount <= 0)
                {
                    //Debug.Log("Replace With Empty InventorySlot");
                    StructInventorySlot newInventorySlot = StructInventorySlot.GetEmpty();
                    newInventorySlot.SlotIndex = inventorySlotIndex;
                    Items[inventorySlotIndex] = newInventorySlot;
                }
                else
                {
                    Items[inventorySlotIndex] = changedInventorySlot;
                }
                
                int earnGold = itemData.SellPrice * sellCount;
                AddMoney(earnGold);

                IsChangedInventory = true;
                //Debug.Log("IsChangedInventory " + IsChangedInventory);
                return ResultType.Success;
            }
            else
            {
                return ResultType.SellFaillLackCount;
            }
        }
        else
        {
            return ResultType.WrongItemId;
        }
    }
}
