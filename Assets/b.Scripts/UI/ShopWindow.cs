using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RPG.UI;
using RPG.Common;
using System.Linq;


public class ShopWindow : MonoBehaviour
{
    public GameObject SlotRef;

    private int _defaultSlotCount = 5;

    public Transform ShopSlotsRoot;
    public Transform PlayerSlotsRoot;

    public List<ItemSlotInShop> ShopSlots;
    public List<ItemSlotInShop> PlayerSlots;
    public Player @Player;

    [Header("Player Slots에 쓰일 크기 참조")]
    public RectTransform PlayerSlotsViewArea;
    public ItemSlotInShop CurrentSelectedPlayerSlot;
    public int SellCount;

    [Header("Shop Slots에 쓰일 크기 참조")]
    public RectTransform ShopSlotsViewArea;
    public ItemSlotInShop CurrentSelectedShopSlot;
    public int BuyCount;

    public TextMeshProUGUI NpcName;
    public TextMeshProUGUI PlayerName;
    public TextMeshProUGUI HoldingGold;
    private readonly string _baseHoldingGoldString = "{0}";

    public void Initialize()
    {
        CreateShopSlots(_defaultSlotCount);
        CreatePlayerSlots(_defaultSlotCount);
    }

    public void SetPlayerInstance(Player player)
    {
        @Player = player;
    }

    private ItemSlotInShop CreateSlot()
    {
        GameObject slotObj = Instantiate(SlotRef);
        ItemSlotInShop slot = slotObj.GetComponent<ItemSlotInShop>();
        slot.Off();
        return slot;
    }

    private void CreateShopSlots(int count)
    {
        for (int i = 0; i < count; i++)
        {
            ItemSlotInShop slot = CreateSlot();
            slot.name = $"ShopSlot[{ShopSlots.Count}]";
            slot.transform.SetParent(ShopSlotsRoot);
            slot.SetMode(TransactionMode.ShopKeeper);
            AspectRatioKeeper ratioKeeper = slot.GetComponent<AspectRatioKeeper>();
            if(ratioKeeper != null)
            {
                ratioKeeper.SetOtherReference(ShopSlotsViewArea);
            }
            ShopSlots.Add(slot);
        }
    }

    private void CreatePlayerSlots(int count)
    {
        for (int i = 0; i < count; i++)
        {
            ItemSlotInShop slot = CreateSlot();
            slot.name = $"PlayerSlot[{PlayerSlots.Count}]";
            slot.transform.SetParent(PlayerSlotsRoot);
            slot.SetMode(TransactionMode.Player);
            AspectRatioKeeper ratioKeeper = slot.GetComponent<AspectRatioKeeper>();
            if (ratioKeeper != null)
            {
                ratioKeeper.SetOtherReference(PlayerSlotsViewArea);
            }
            PlayerSlots.Add(slot);
        }
    }

    //private void UpdateShopSlots(StructItemData[] itemDatas)
    //{
    //    if (ShopSlots.Count < itemDatas.Length)
    //    {
    //        int addNumber = itemDatas.Length - ShopSlots.Count;
    //        CreateShopSlots(addNumber);
    //    }

    //    int[] itemIds = itemDatas.Where(x => x.Id > 0).Select(x => x.Id).ToArray();
    //    UpdateSlots(itemIds, ShopSlots);
    //}

    private void UpdateShopSlots(int[] itemIds)
    {
        if (ShopSlots.Count < itemIds.Length)
        {
            int addNumber = itemIds.Length - ShopSlots.Count;
            CreateShopSlots(addNumber);
        }

        int[] validItemIds = itemIds.Where(x => x > 0).Select(x => x).ToArray();
        UpdateSlots(validItemIds, ShopSlots);
    }

    //private void UpdatePlayerSlots(StructItemData[] itemDatas)
    //{
    //    if (PlayerSlots.Count < itemDatas.Length)
    //    {
    //        int addNumber = itemDatas.Length - PlayerSlots.Count;
    //        CreatePlayerSlots(addNumber);
    //    }

    //    int[] itemIds = itemDatas.Where(x => x.Id > 0).Select(x => x.Id).ToArray();
    //    UpdateSlots(itemIds, PlayerSlots);
    //}

    private void UpdatePlayerSlots()
    {
        // 장착 중인 아이템은 보이지 않음
        StructInventorySlot[] playerInventorySlots = @Player.Items.Where(x => x.ItemId > 0 && !x.IsOnEquip).Select(x => x).ToArray();

        Debug.Log($"ShopWindow playerInventorySlots : {string.Join("|", playerInventorySlots)}");
        
        HoldingGold.text = string.Format(_baseHoldingGoldString, @Player.Money);
        if (PlayerSlots.Count < playerInventorySlots.Length)
        {
            int addNumber = playerInventorySlots.Length - PlayerSlots.Count;
            CreatePlayerSlots(addNumber);
        }

        UpdateSlots(playerInventorySlots, PlayerSlots);
    }

    private void UpdateSlots(int[] itemIds, List<ItemSlotInShop> slots)
    {
        foreach (var slot in slots)
        {
            slot.Off();
        }

        for (int i = 0; i < itemIds.Length; i++)
        {
            int itemId = itemIds[i];
            ItemSlotInShop slot = slots[i];
            slot.On(itemId);
        }
    }

    private void UpdateSlots(StructInventorySlot[] inventorySlots, List<ItemSlotInShop> slots)
    {
        foreach(var slot in slots)
        {
            slot.Off();
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            StructInventorySlot inventorySlot = inventorySlots[i];
            ItemSlotInShop slot = slots[i];
            slot.On(inventorySlot);
        }
    }

    public void Close()
    {
        foreach(var slot in ShopSlots)
        {
            slot.Off();
        }

        foreach (var slot in PlayerSlots)
        {
            slot.Off();
        }

        gameObject.SetActive(false);
    }

    void Update()
    {
        if(Player.Instance.IsChangedInventory)
        {
            Debug.Log("Update Update");
            UpdatePlayerSlots();
            Player.Instance.IsChangedInventory = false;
        }
    }

    public void Open(int npcId)
    {
        NpcName.text = DataBase.Npcs[npcId].Name;
        PlayerName.text = Player.Instance.Status.Name;
        int[] shopItemIds = DataBase.Npcs[npcId].SellingItems.Select(x => x).ToArray();
        UpdateShopSlots(shopItemIds);
        UpdatePlayerSlots();
        gameObject.SetActive(true);
    }

    public void SelectPlayerSlot(ItemSlotInShop playerSlot)
    {
        CurrentSelectedPlayerSlot = playerSlot;
    }

    public void SelectShopSlot(ItemSlotInShop shopSlot)
    {
        CurrentSelectedShopSlot = shopSlot;
    }
}
