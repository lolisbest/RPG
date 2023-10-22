using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Common;
using TMPro;
using RPG.Item;
using RPG.UI;
using System.Linq;
using UnityEngine.Networking;

public class InGameUIManager : Singleton<InGameUIManager>
{
    public QuestManager @QuestManager;
    public Player @Player { get; private set; }

    public TextMeshProUGUI FpsText;
    private readonly string _baseFpsString = "FPS : {0}";
    public bool displayFps;
    private double _fps;


    [Header("상호 작용 키 입력 메시지 팝업 창")]
    public GameObject InteractionKeyMessagePanel;
    public TextMeshProUGUI InteractionKeyMessageTxt;
    public InteractionType LastInteractionType;
    public bool IsInteractingWithPlayer
    {
        get
        {
            return IsOpenItemBoxWindow || IsOpenInventoryWindow ||
            IsOpenNpcServiceSelectionWindow || IsOpenNpcQuestDetailWindow || IsOpenQuestSelectionWindow ||
            IsOpenNpcQuestDetailWindow || IsOpenCurrentQuestDetailWindow || IsOpenDialogWindow ||
            IsOpenShopWindow || IsOpenTransactionWindow || IsOpenEscWindow || IsOpenSkillsWindow;
        }
    }

    public static readonly string KeyPressMessage = "Press [F] to {0}";

    [Header("ItemBox 창")]
    public ItemBoxWindow @ItemBoxWindow;
    public FieldItemBox CurrentBeingOpenItemBox;

    public bool IsOpenItemBoxWindow { get => @ItemBoxWindow.gameObject.activeSelf; }


    [Header("인벤토리 창")]
    public InventoryWindow @InventoryWindow;
    public bool IsOpenInventoryWindow { get => @InventoryWindow.gameObject.activeSelf; }

    [Header("아이템 정보 창")]
    public InventoryItemInfoWindow @InventoryItemInfoWindow;

    public int CurrentSelectedSlotIndex;

    [Header("NPC 서비스 선택 창")]
    public NpcServiceSelectionWindow @NpcServiceSelectionWindow;
    public bool IsOpenNpcServiceSelectionWindow { get => NpcServiceSelectionWindow.gameObject.activeSelf; }
    public Npc CurrentNpc;

    [Header("퀘스트 선택 창")]
    public QuestSelectionWindow QuestSelectionWindow;
    public bool IsOpenQuestSelectionWindow { get => QuestSelectionWindow.gameObject.activeSelf; }

    [Header("Npc 퀘스트 창")]
    public NpcQuestDetailWindow @NpcQuestDetailWindow;
    public bool IsOpenNpcQuestDetailWindow { get => @NpcQuestDetailWindow.gameObject.activeSelf; }

    [Header("현재 진행 중인 퀘스트 목록 창")]
    public CurrentQuestsWindow @CurrentQuestsWindow;

    [Header("선택된 진행 중인 퀘스트 창")]
    public CurrentQuestDetailWindow @CurrentQuestDetailWindow;
    public bool IsOpenCurrentQuestDetailWindow { get => @CurrentQuestDetailWindow.gameObject.activeSelf; }

    [Header("대화 창")]
    public DialogWindow @DialogWindow;
    public bool IsOpenDialogWindow { get => @DialogWindow.gameObject.activeSelf; }

    public bool IsPresentDialogQuitButton { get => @DialogWindow.QuitButton.gameObject.activeSelf; }
    public bool IsPresentDialogNextButton 
    { 
        get => @DialogWindow.NextButton.gameObject.activeSelf; 
    }

    [Header("Hp Gauge")]
    public Gauge HpGauge;

    [Header("Mp Gauge")]
    public Gauge MpGauge;

    [Header("스텟 창")]
    public StatusWindow @StatusWindow;
    public bool IsOpenStatusWindow { get => @StatusWindow.gameObject.activeSelf; }

    [Header("퀵슬롯")]
    public QuickSlotManager @QuickSlotManager;

    [Header("대미지 텍스트")]
    public DamageTextDrawer @DamageTextDrawer;

    [Header("상점")]
    public ShopWindow @ShopWindow;
    public bool IsOpenShopWindow { get => @ShopWindow.gameObject.activeSelf; }

    [Header("거래 조율 창")]
    public TransactionConfirmWindow TransactionWindow;
    public bool IsOpenTransactionWindow { get => TransactionWindow.gameObject.activeSelf; }

    [Header("미니맵")]
    [SerializeField] private MinimapController _minimapController;

    [Header("Esc 메뉴 창")]
    [SerializeField] private GameObject _escWindow;
    public bool IsOpenEscWindow { get => _escWindow.activeSelf; }

    [Header("스킬 창")]
    [SerializeField] private SkillsWindow _skillsWindow;
    public bool IsOpenSkillsWindow { get => _skillsWindow.gameObject.activeSelf; }

    [Header("사망 창")]
    [SerializeField] private GameObject _onDeathWindow;
    public bool IsOpenOnDeathWindow { get => _onDeathWindow.gameObject.activeSelf; }


    public override void Initialize()
    {
        LastInteractionType = InteractionType.Open;
        if (InteractionKeyMessageTxt != null)
            InteractionKeyMessageTxt.text = string.Format(KeyPressMessage, LastInteractionType.ToString());

        @NpcServiceSelectionWindow.Initialize();
        @QuestSelectionWindow.Initialize();
        @QuestManager = QuestManager.Instance;
        @NpcQuestDetailWindow.Initialize();
        @DamageTextDrawer.Initialize();

        @InventoryWindow.Initialize();
        @CurrentQuestsWindow.Initialize();
        @DialogWindow.Initialize();
        @CurrentQuestDetailWindow.Initialize();
        @ShopWindow.Initialize();

        return;
        //DroppedBox.Initialize();
        //@Player = Player.Instance;
    }

    protected override void Awake()
    {
        base.Awake();
        Initialize();
    }

    void Start()
    {
        //return;

        ToggleFpsText(displayFps);
    }

    void Update()
    {
        if (displayFps)
            UpdateFps();
    }

    public void SetPlayerInstance(Player player)
    {
        @Player = player;
        @InventoryWindow.SetPlayerInstance(player);
        @ShopWindow.SetPlayerInstance(player);
        _minimapController.Intit();
    }

    public void ActivateInteractionKeyMessage(InteractionType type)
    {
        if (InteractionKeyMessagePanel == null)
        {
            //Debug.Log("InteractionKeyMessagePanel is null");
            return;
        }

        if (LastInteractionType != type)
        {
            LastInteractionType = type;
            //string formerText = InteractionKeyMessageTxt.text;
            InteractionKeyMessageTxt.text = string.Format(KeyPressMessage, LastInteractionType.ToString());
            //Debug.Log($"{formerText} => {InteractionKeyMessageTxt.text}");
        }

        if (!InteractionKeyMessagePanel.activeSelf)
        {
            InteractionKeyMessagePanel.SetActive(true);
        }
    }

    public void DeactivateInteractionKeyMessage()
    {
        if (InteractionKeyMessagePanel == null)
        {
            //Debug.Log("InteractionKeyMessagePanel is null");
            return;
        }

        if (!InteractionKeyMessagePanel.activeSelf)
            return;

        InteractionKeyMessagePanel.SetActive(false);
    }

    public bool TryOpenItemBoxWindow(FieldItemBox itemBox)
    {
        if (itemBox == null)
            return false;

        if (IsInteractingWithPlayer)
            return false;

        if (@ItemBoxWindow == null)
            return false;

        Debug.Log($"TryOpenItemBoxWindow Items:{itemBox.Items.Count}");
        CurrentBeingOpenItemBox = itemBox;
        @ItemBoxWindow.Link(CurrentBeingOpenItemBox);
        @ItemBoxWindow.Open();

        return true;
    }

    public void CloseItemBoxWindow()
    {
        if (CurrentBeingOpenItemBox != null)
        {
            CurrentBeingOpenItemBox.StopInteraction();
            CurrentBeingOpenItemBox = null;
        }

        @ItemBoxWindow.Close();
    }

    public void OpenInventoryWindow()
    {
        if (@InventoryWindow) @InventoryWindow.Open();
        if (@StatusWindow) @StatusWindow.Open();
    }

    public void CloseInventoryWindow()
    {
        if (@InventoryWindow) @InventoryWindow.Close();
        if (@InventoryItemInfoWindow) @InventoryItemInfoWindow.Close();
        if (@StatusWindow) @StatusWindow.Close();
    }

    public void CLoseInventoryItemInfoWindow()
    {
        InventoryItemInfoWindow.Close();
    }

    public bool TryOpenNpcServiceSelectionWindow(Npc npc)
    {
        CurrentNpc = npc;

        @NpcServiceSelectionWindow.SetNpcName(npc.Data.Name);
        @NpcServiceSelectionWindow.SetServices(npc.Data.Services);
        @NpcServiceSelectionWindow.Open();
        return true;
    }

    /// <summary>
    /// CurrentNpc = null;
    /// @NpcServiceSelectionWindow.Close();
    /// </summary>
    public void CloseNpcServiceSelectionWindow()
    {
        CurrentNpc = null;
        @NpcServiceSelectionWindow.Close();
    }

    public void OpenQuestSelectionWindow()
    {
        int[] availableQuestIds = (from questId in CurrentNpc.Data.QuestIds
                                       // 진행 중이지 않은 Quuest Ids
                                   where @QuestManager.IsInProgress(questId) == false && !DataBase.Quests[questId].IsClear
                                   select questId).ToArray();

        //Debug.Log($"Npc:{CurrentNpc.Data.Name} Available QuestIds : {string.Join(",", availableQuestIds)}");
        QuestSelectionWindow.UpdateQuest(availableQuestIds);
        QuestSelectionWindow.Open();
        CloseNpcServiceSelectionWindow(); // CurrentNpc = null;
    }

    public void CloseQuestSelectionWindow()
    {
        @QuestSelectionWindow.Close();
    }

    public void OpenNpcQuestDetailWindow(int questId)
    {
        // Open을 먼저해서 CurrentNpc를 넘겨주고
        // CloseNpcServiceSelectionWindow 에서 CurrentNpc를 null로 셋팅
        @NpcQuestDetailWindow.OpenDetailWindow(questId);
        CloseQuestSelectionWindow();
    }

    public void AcceptNpcQuest()
    {
        @NpcQuestDetailWindow.Accept();
    }

    public void CloseNpcQuestDetailWindow()
    {
        @NpcQuestDetailWindow.Deny();
    }

    public void AddQuestToCurrentQuestsWindow(int questId)
    {
        if (@CurrentQuestsWindow) @CurrentQuestsWindow.AddQuest(questId);
        if (@QuestManager) @QuestManager.AddQuest(questId);
    }

    public void OpenCurrentQuestDetailWindow(int questId)
    {
        StructQuestData[] questDataArray = @QuestManager.CurrentInProgressQuests.Where(x => x.Id == questId).ToArray();
        if (questDataArray.Length == 1)
        {
            @CurrentQuestDetailWindow.OpenDetailWindow(questDataArray[0]);
        }
        else
        {
            throw new System.NotImplementedException($"{questDataArray.Length} of questId:{questId} in CurrentInProgressQuests");
        }
    }
    public void OpenDialogWindow()
    {
        //Debug.Log("OpenDialogWindow");
        //Debug.Log("CurrentNpc :" + CurrentNpc);
        //Debug.Log("CurrentNpc.Data :" + CurrentNpc.Data);

        List<int> availbleDialogIds = new();

        foreach (var dialogId in CurrentNpc.Data.StartDialogIds)
        {
            int conditionPassed = 0;
            int[] requiredNotClearedQuestIds = DataBase.Dialogs[dialogId].RequiredNotClearedQuestIds;

            if (requiredNotClearedQuestIds.Length == 0)
            {
                availbleDialogIds.Add(dialogId);
                continue;
            }

            foreach (var requiredNotClearedQuestId in requiredNotClearedQuestIds)
            {
                // 아직 완료 안 했는지
                if (!DataBase.Quests[requiredNotClearedQuestId].IsClear)
                {
                    // 완료 되지 않았다면 조건 만족
                    conditionPassed += 1;
                }
            }

            if (conditionPassed == requiredNotClearedQuestIds.Length)
            {
                availbleDialogIds.Add(dialogId);
            }
        }

        //Debug.Log($"availbleDialogIds : {string.Join(",", availbleDialogIds)}");

        int toOpenDialogId = -1;
        if (availbleDialogIds.Count != 0)
        {
            // 가능한 다이얼로그 중 첫 번째
            toOpenDialogId = availbleDialogIds[0];
        }

        //Debug.Log("toOpenDialogId " + toOpenDialogId);
        @QuestManager.CallbackQuestCondition(QuestConditionType.Talk, CurrentNpc.Id, 1);
        DialogWindow.Open(CurrentNpc.Data.Name, toOpenDialogId);
        // CurrentNpc를 null 만들기 때문에 마지막에 호출
        CloseNpcServiceSelectionWindow();
    }

    public void NextDialog()
    {
        @DialogWindow.Next();
    }

    public void CloseDialogWindow()
    {
        @DialogWindow.Quit();
    }

    public void FinishQuest(int questId)
    {
        if (@QuestManager.FinishQuest(questId))
        {
            // Successful Finishing Quest
            @CurrentQuestDetailWindow.Quit();
            @CurrentQuestsWindow.DeleteQuest(questId);
            StructIdCount[] RewardItems = DataBase.Quests[questId].RewardItems;
            foreach (var item in RewardItems)
            {
                Debug.Log($"Player Inventory Added : {item.Count} of {DataBase.Items[item.Id].Name}");
                @Player.AddItem(item.Id, item.Count);
            }

            @Player.AddExperience(DataBase.Quests[questId].RewardExp);
        }
        else
        {
            // Failed Finishing Quest
        }
    }

    public void UpdateHpGauge(float rate)
    {
        //Debug.Log("UpdateHpGauge " + rate);
        HpGauge.SetCurretRate(rate);
    }

    public void UpdateMpGauge(float rate)
    {
        MpGauge.SetCurretRate(rate);
    }

    public void SelectInventorySlot(int slotIndex)
    {
        CurrentSelectedSlotIndex = slotIndex;
        if (slotIndex == -1)
        {
            //Debug.Log("SelectInventorySlot slotIndex == -1");
            return;
        }

        IconItemSlot slot = @InventoryWindow.Slots[slotIndex];
        if (slot.ItemId == -1)
            return;

        //Debug.Log("slot.ItemId: " + slot.ItemId);
        @InventoryItemInfoWindow.SetItemInfo(slot.ItemId, slot.IsEquipped);
        @InventoryItemInfoWindow.Open();
    }

    /// <summary>
    /// InventoryWindow.Slots를 참고하여 InventoryItemInfoWindow를 갱신
    /// </summary>
    public void UpdateInventoryItemInfoWindow()
    {
        if (CurrentSelectedSlotIndex == -1)
        {
            //Debug.Log("UpdateInventoryItemInfoWindow CurrentSelectedSlotIndex == -1");
            return;
        }

        IconItemSlot slot = @InventoryWindow.Slots[CurrentSelectedSlotIndex];

        if (slot.ItemId == -1)
            return;

        //Debug.Log("slot.ItemId: " + slot.ItemId);
        @InventoryItemInfoWindow.SetItemInfo(slot.ItemId, slot.IsEquipped);
    }

    public void OpenStatusWindow()
    {
        @StatusWindow.Open();
    }

    public void CloseStatusWindow()
    {
        @StatusWindow.Close();
    }

    public void UpdateQuickSlots()
    {
        @QuickSlotManager.UpdateSlots();
    }

    public void UseQuickSlot(int quickSlotNumber)
    {
        //Debug.Log("Try UseQuickSlost quickSlotNumber " + quickSlotNumber);
        foreach (var slot in @QuickSlotManager.QuickSlots)
        {
            if (slot.Key == quickSlotNumber)
            {
                //Debug.Log("Try UseQuickSlost Key" + slot.Key);
                slot.Use();
            }
        }
    }

    public void ShowDamageText(int damage, Vector3 worldPosition)
    {
        @DamageTextDrawer.ShowDamageText(damage, worldPosition);
    }

    public void OpenShop()
    {
        @ShopWindow.Open(CurrentNpc.Id);
        CloseNpcServiceSelectionWindow();
    }

    public void CloseShop()
    {
        @ShopWindow.Close();
        TransactionWindow.Close();
    }

    public void OpenItemPlayerModeTransactionWindow(int playerInventorySlotIndex)
    {
        //Debug.Log("OpenItemPlayerModeTransactionWindow");
        TransactionWindow.OpenPlayerMode(playerInventorySlotIndex);
    }

    public void OpenItemShopKeeperModeTransactionWindow(StructItemData itemData)
    {
        //Debug.Log("OpenItemShopKeeperModeTransactionWindow");
        TransactionWindow.OpenShopKeeperMode(itemData);
    }

    public void ToggleFpsText(bool open)
    {
        FpsText.gameObject.SetActive(open);
    }

    public void UpdateFps()
    {
        _fps = System.Math.Round(1 / Time.deltaTime, 1);
        FpsText.text = string.Format(_baseFpsString, _fps);
    }

    public void SetPlaceName(string placeName)
    {
        _minimapController.SetPlaceName(placeName);
    }

    public void ToggleEscWindow()
    {
        //Debug.Log("InGameUiManager.ToggleEscWindow");
        if (!IsOpenSkillsWindow) _skillsWindow.Open();
        else _skillsWindow.Close();
    }

    public void CloseEscWindow()
    {
        _escWindow.SetActive(false);
    }

    public void OpenSkillsWindow()
    {
        StructSkillData[] skills = { DataBase.Skills[1] };
        _skillsWindow.LoadDataIntoSlots(skills);
        _skillsWindow.Open();
    }

    public void CloseSkillsWindow()
    {
        _skillsWindow.Close();
    }

    public void OpenOnDeathWindow()
    {
        _onDeathWindow.SetActive(true);
    }

    public void CloseOnDeathWindow()
    {
        _onDeathWindow.SetActive(false);
    }
    public void RespawnPlayer()
    {
        CloseOnDeathWindow();
        MapManager.Instance.RespwanPlayer();
    }
}