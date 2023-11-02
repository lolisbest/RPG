using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Common;
using TMPro;
using RPG.Item;
using RPG.UI;
using System.Linq;
using UnityEngine.Networking;

namespace RPG.UI
{
    public partial class UIManager : Singleton<UIManager>
    {
        [Space(10f)]
        [Header("------- 인게임 씬 -------")]
        [SerializeField] private QuestManager _questManager;
        public Player @Player { get; private set; }

        public TextMeshProUGUI FpsText;
        private readonly string _baseFpsString = "FPS : {0}";
        public bool displayFps;
        private double _fps;

        [Header("상호 작용 키 입력 메시지 팝업 창")]
        [SerializeField] private GameObject _interactionKeyMessagePanel;
        [SerializeField] private TextMeshProUGUI _interactionKeyMessageTxt;
        [SerializeField] private InteractionType _lastInteractionType;

        public bool IsInteractingWithPlayer
        {
            get
            {
                return IsOpenItemBoxWindow ||
                IsOpenNpcServiceSelectionWindow || IsOpenNpcQuestDetailWindow || IsOpenQuestSelectionWindow ||
                IsOpenNpcQuestDetailWindow || IsOpenDialogWindow ||
                IsOpenShopWindow || IsOpenTransactionWindow || IsOpenEscWindow;
            }
        }

        public static readonly string KeyPressMessage = "Press [F] to {0}";

        [Header("ItemBox 창")]
        [SerializeField] private ItemBoxWindow _itemBoxWindow;
        public bool IsOpenItemBoxWindow { get => _itemBoxWindow.gameObject.activeSelf; }
        public FieldItemBox CurrentBeingOpenItemBox { get; private set; }


        [Header("인벤토리 창")]
        [SerializeField] private InventoryWindow _inventoryWindow;
        public bool IsOpenInventoryWindow { get => _inventoryWindow.gameObject.activeSelf; }

        [Header("아이템 정보 창")]
        [SerializeField] private InventoryItemInfoWindow _inventoryItemInfoWindow;
        public bool IsOpenInventoryItemInfoWindow { get => _inventoryItemInfoWindow.gameObject.activeSelf; }

        public int CurrentSelectedSlotIndex;

        [Header("NPC 서비스 선택 창")]
        [SerializeField] private NpcServiceSelectionWindow _npcServiceSelectionWindow;
        public bool IsOpenNpcServiceSelectionWindow { get => _npcServiceSelectionWindow.gameObject.activeSelf; }
        public Npc CurrentNpc;

        [Header("퀘스트 선택 창")]
        [SerializeField] private QuestSelectionWindow _questSelectionWindow;
        public bool IsOpenQuestSelectionWindow { get => _questSelectionWindow.gameObject.activeSelf; }

        [Header("Npc 퀘스트 창")]
        [SerializeField] private NpcQuestDetailWindow _npcQuestDetailWindow;
        public bool IsOpenNpcQuestDetailWindow { get => _npcQuestDetailWindow.gameObject.activeSelf; }

        [Header("현재 진행 중인 퀘스트 목록 창")]
        [SerializeField] private CurrentQuestsWindow _currentQuestsWindow;
        public bool IsOpenCurrentQuestsWindow { get => _currentQuestsWindow.gameObject.activeSelf; }

        [Header("선택된 진행 중인 퀘스트 창")]
        [SerializeField] private CurrentQuestDetailWindow _currentQuestDetailWindow;
        public bool IsOpenCurrentQuestDetailWindow { get => _currentQuestDetailWindow.gameObject.activeSelf; }

        [Header("대화 창")]
        [SerializeField] private DialogWindow _dialogWindow;
        public bool IsOpenDialogWindow { get => _dialogWindow.gameObject.activeSelf; }

        public bool IsPresentDialogQuitButton { get => _dialogWindow.QuitButton.gameObject.activeSelf; }
        public bool IsPresentDialogNextButton
        {
            get => _dialogWindow.NextButton.gameObject.activeSelf;
        }

        [Header("Hp Gauge")]
        [SerializeField] private Gauge _hpGauge;

        [Header("Mp Gauge")]
        [SerializeField] private Gauge _mpGauge;

        [Header("스텟 창")]
        [SerializeField] private StatusWindow _statusWindow;
        public bool IsOpenStatusWindow { get => _statusWindow.gameObject.activeSelf; }

        [Header("퀵슬롯")]
        [SerializeField] private QuickSlotManager _quickSlotManager;

        [Header("대미지 텍스트")]
        [SerializeField] private DamageTextDrawer _damageTextDrawer;

        [Header("상점")]
        [SerializeField] private ShopWindow _shopWindow;
        public bool IsOpenShopWindow { get => _shopWindow.gameObject.activeSelf; }

        [Header("거래 조율 창")]
        [SerializeField] private TransactionConfirmWindow _transactionWindow;
        public bool IsOpenTransactionWindow { get => _transactionWindow.gameObject.activeSelf; }

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

        public void InitializeInGame()
        {
            _lastInteractionType = InteractionType.Open;
            if (_interactionKeyMessageTxt != null)
                _interactionKeyMessageTxt.text = string.Format(KeyPressMessage, _lastInteractionType.ToString());

            _npcServiceSelectionWindow.Initialize();
            _questSelectionWindow.Initialize();
            _questManager = QuestManager.Instance;
            _npcQuestDetailWindow.Initialize();
            _damageTextDrawer.Initialize();

            _inventoryWindow.Initialize();
            _currentQuestsWindow.Initialize();
            _dialogWindow.Initialize();
            _currentQuestDetailWindow.Initialize();
            _shopWindow.Initialize();
        }

        public void SetPlayerInstance(Player player)
        {
            @Player = player;
            _inventoryWindow.SetPlayerInstance(player);
            _shopWindow.SetPlayerInstance(player);
            _minimapController.Intit();
        }

        public void ActivateInteractionKeyMessage(InteractionType type)
        {
            if (_interactionKeyMessagePanel == null)
            {
                //Debug.Log("InteractionKeyMessagePanel is null");
                return;
            }

            if (_lastInteractionType != type)
            {
                _lastInteractionType = type;
                //string formerText = InteractionKeyMessageTxt.text;
                _interactionKeyMessageTxt.text = string.Format(KeyPressMessage, _lastInteractionType.ToString());
                //Debug.Log($"{formerText} => {InteractionKeyMessageTxt.text}");
            }

            if (!_interactionKeyMessagePanel.activeSelf)
            {
                _interactionKeyMessagePanel.SetActive(true);
            }
        }

        public void DeactivateInteractionKeyMessage()
        {
            if (_interactionKeyMessagePanel == null)
            {
                //Debug.Log("InteractionKeyMessagePanel is null");
                return;
            }

            if (!_interactionKeyMessagePanel.activeSelf)
                return;

            _interactionKeyMessagePanel.SetActive(false);
        }

        public bool TryOpenItemBoxWindow(FieldItemBox itemBox)
        {
            if (itemBox == null)
                return false;

            if (IsInteractingWithPlayer)
                return false;

            if (_itemBoxWindow == null)
                return false;

            //Debug.Log($"TryOpenItemBoxWindow Items:{itemBox.Items.Count}");
            CurrentBeingOpenItemBox = itemBox;
            _itemBoxWindow.Link(CurrentBeingOpenItemBox);
            _itemBoxWindow.Open();

            return true;
        }

        public void CloseItemBoxWindow()
        {
            if (CurrentBeingOpenItemBox != null)
            {
                CurrentBeingOpenItemBox.StopInteraction();
                CurrentBeingOpenItemBox = null;
            }

            _itemBoxWindow.Close();
        }

        public void OpenInventoryWindow()
        {
            if (_inventoryWindow) { _inventoryWindow.Open(); _inventoryWindow.transform.SetAsLastSibling(); }
            if (_statusWindow) { _statusWindow.Open(); _statusWindow.transform.SetAsLastSibling(); }
        }

        public void CloseInventoryWindow()
        {
            Debug.Log("CloseInventoryWindow");

            if (_inventoryWindow) _inventoryWindow.Close();
            if (_inventoryItemInfoWindow) _inventoryItemInfoWindow.Close();
            if (_statusWindow) _statusWindow.Close();
        }

        public void CLoseInventoryItemInfoWindow()
        {
            _inventoryItemInfoWindow.Close();
        }

        public bool TryOpenNpcServiceSelectionWindow(Npc npc)
        {
            CurrentNpc = npc;

            _npcServiceSelectionWindow.SetNpcName(npc.Data.Name);
            _npcServiceSelectionWindow.SetServices(npc.Data.Services);
            _npcServiceSelectionWindow.Open();
            return true;
        }

        /// <summary>
        /// CurrentNpc = null;
        /// @NpcServiceSelectionWindow.Close();
        /// </summary>
        public void CloseNpcServiceSelectionWindow()
        {
            CurrentNpc = null;
            _npcServiceSelectionWindow.Close();
        }

        public void OpenQuestSelectionWindow()
        {
            int[] availableQuestIds = (from questId in CurrentNpc.Data.QuestIds
                                           // 진행 중이지 않은 Quuest Ids
                                       where _questManager.IsInProgress(questId) == false && !GameManager.Instance.Player.ClearedQuestIds.Contains(questId)
                                       select questId).ToArray();

            //Debug.Log($"Npc:{CurrentNpc.Data.Name} Available QuestIds : {string.Join(",", availableQuestIds)}");
            _questSelectionWindow.UpdateQuest(availableQuestIds);
            _questSelectionWindow.Open();
            CloseNpcServiceSelectionWindow(); // CurrentNpc = null;
        }

        public void CloseQuestSelectionWindow()
        {
            _questSelectionWindow.Close();
        }

        public void OpenNpcQuestDetailWindow(int questId)
        {
            // Open을 먼저해서 CurrentNpc를 넘겨주고
            // CloseNpcServiceSelectionWindow 에서 CurrentNpc를 null로 셋팅
            _npcQuestDetailWindow.OpenDetailWindow(questId);
            CloseQuestSelectionWindow();
        }

        public void AcceptNpcQuest()
        {
            _npcQuestDetailWindow.Accept();
        }

        public void CloseNpcQuestDetailWindow()
        {
            _npcQuestDetailWindow.Deny();
        }

        public void AddQuestToCurrentQuestsWindow(int questId)
        {
            if (_currentQuestsWindow) _currentQuestsWindow.AddQuest(questId);
            if (_questManager) _questManager.AddQuest(questId);
        }

        public void OpenCurrentQuestDetailWindow(int questId)
        {
            StructQuestData[] questDataArray = _questManager.CurrentInProgressQuests.Where(x => x.Id == questId).ToArray();
            if (questDataArray.Length == 1)
            {
                _currentQuestDetailWindow.OpenDetailWindow(questDataArray[0]);
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
                    if (!GameManager.Instance.Player.ClearedQuestIds.Contains(requiredNotClearedQuestId))
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
            _questManager.CallbackQuestCondition(QuestConditionType.Talk, CurrentNpc.Id, 1);
            _dialogWindow.Open(CurrentNpc.Data.Name, toOpenDialogId);
            // CurrentNpc를 null 만들기 때문에 마지막에 호출
            CloseNpcServiceSelectionWindow();
        }

        public void NextDialog()
        {
            _dialogWindow.Next();
        }

        public void CloseDialogWindow()
        {
            _dialogWindow.Quit();
        }

        public void FinishQuest(int questId)
        {
            if (_questManager.FinishQuest(questId))
            {
                // Successful Finishing Quest
                _currentQuestDetailWindow.Quit();
                _currentQuestsWindow.DeleteQuest(questId);
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
            _hpGauge.SetCurretRate(rate);
        }

        public void UpdateMpGauge(float rate)
        {
            _mpGauge.SetCurretRate(rate);
        }

        public void SelectInventorySlot(int slotIndex)
        {
            CurrentSelectedSlotIndex = slotIndex;
            if (slotIndex == -1)
            {
                //Debug.Log("SelectInventorySlot slotIndex == -1");
                return;
            }

            IconItemSlot slot = _inventoryWindow.Slots[slotIndex];
            if (slot.ItemId == -1)
                return;

            //Debug.Log("slot.ItemId: " + slot.ItemId);
            _inventoryItemInfoWindow.SetItemInfo(slot.ItemId, slot.IsEquipped);
            _inventoryItemInfoWindow.Open();
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

            IconItemSlot slot = _inventoryWindow.Slots[CurrentSelectedSlotIndex];

            if (slot.ItemId == -1)
                return;

            //Debug.Log("slot.ItemId: " + slot.ItemId);
            _inventoryItemInfoWindow.SetItemInfo(slot.ItemId, slot.IsEquipped);
        }

        public void OpenStatusWindow()
        {
            _statusWindow.Open();
        }

        public void CloseStatusWindow()
        {
            _statusWindow.Close();
        }

        public void UpdateQuickSlots()
        {
            _quickSlotManager.UpdateSlots();
        }

        public void UseQuickSlot(int quickSlotNumber)
        {
            //Debug.Log("Try UseQuickSlost quickSlotNumber " + quickSlotNumber);
            foreach (var slot in _quickSlotManager.QuickSlots)
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
            _damageTextDrawer.ShowDamageText(damage, worldPosition);
        }

        public void OpenShop()
        {
            _shopWindow.Open(CurrentNpc.Id);
            CloseNpcServiceSelectionWindow();
        }

        public void CloseShop()
        {
            _shopWindow.Close();
            _transactionWindow.Close();
        }

        public void OpenItemPlayerModeTransactionWindow(int playerInventorySlotIndex)
        {
            //Debug.Log("OpenItemPlayerModeTransactionWindow");
            _transactionWindow.OpenPlayerMode(playerInventorySlotIndex);
        }

        public void OpenItemShopKeeperModeTransactionWindow(StructItemData itemData)
        {
            //Debug.Log("OpenItemShopKeeperModeTransactionWindow");
            _transactionWindow.OpenShopKeeperMode(itemData);
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
            _escWindow.SetActive(!_escWindow.activeSelf);
        }

        public void CloseEscWindow()
        {
            _escWindow.SetActive(false);
        }

        public void OpenSkillsWindow()
        {
            if (_skillsWindow) 
            {
                StructSkillData[] skills = @Player.Status.AvailableSkillIds.Select(x => DataBase.Skills[x]).ToArray();
                _skillsWindow.LoadDataIntoSlots(skills);
                _skillsWindow.Open(); 
                _skillsWindow.transform.SetAsLastSibling(); 
            }
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
            MapManager.Instance.RespawnPlayerAtStartPosition();
        }

        private void UploadPlayerData()
        {
            StructPlayerData toUpLoadPlayerData = @Player.GetPlayerData();
            toUpLoadPlayerData.IsNewCharacter = false;
            Debug.Log(toUpLoadPlayerData);
            GameManager.Instance.SetCurrentPlayerData(toUpLoadPlayerData);
            GameManager.Instance.SavePlayerData();
        }

        public void LoadIntroScene()
        {
            UploadPlayerData();
            LoadingSceneController.Load("IntroScene");
        }

        public (string[], int[]) GetQuickSlotLinkes()
        {
            return _quickSlotManager.GetQuickSlotLinkes();
        }

        public void LoadQuickSlots()
        {
            //Debug.Log("UIManager.LoadQuickSlots()");

            string[] quickSlotTypes = GameManager.Instance.CurrentPlayerData.QuickSlotTypes;
            int[] quickSlotLinkes = GameManager.Instance.CurrentPlayerData.QuickSlotLinkes;

            //Debug.Log($"quickSlotTypes : {string.Join(",", quickSlotTypes)}");
            //Debug.Log($"quickSlotLinkes : {string.Join(",", quickSlotLinkes)}");

            _quickSlotManager.SetQuickSlots(quickSlotTypes, quickSlotLinkes);
        }

        public void CloseInGameWindows()
        {
            _inGameUIRoot.gameObject.SetActive(false);
            if (IsOpenItemBoxWindow) _itemBoxWindow.Close();
            
            if (IsOpenNpcServiceSelectionWindow) _npcServiceSelectionWindow.Close();
            if (IsOpenDialogWindow) _dialogWindow.Quit();
            if (IsOpenQuestSelectionWindow) _questSelectionWindow.Close();
            if (IsOpenNpcQuestDetailWindow) _npcQuestDetailWindow.Close();
            if (IsOpenCurrentQuestDetailWindow) _currentQuestDetailWindow.Close();

            if (IsOpenCurrentQuestsWindow) _currentQuestsWindow.Close();

            if (IsOpenShopWindow) _shopWindow.Close();
            if (IsOpenTransactionWindow) _transactionWindow.Close();

            if (IsOpenInventoryWindow) _inventoryWindow.Close();
            if (IsOpenInventoryWindow) _inventoryItemInfoWindow.Close();
            if (IsOpenSkillsWindow) _skillsWindow.Close();

            if (IsOpenEscWindow) _escWindow.gameObject.SetActive(false);

            if (IsOpenOnDeathWindow) _onDeathWindow.gameObject.SetActive(false);
        }

        public void OpenInGameWindow()
        {
            _inGameUIRoot.gameObject.SetActive(true);
            if (IsOpenItemBoxWindow) _itemBoxWindow.Close();
            
            if (IsOpenNpcServiceSelectionWindow) _npcServiceSelectionWindow.Close();
            if (IsOpenDialogWindow) _dialogWindow.Quit();
            if (IsOpenQuestSelectionWindow) _questSelectionWindow.Close();
            if (IsOpenNpcQuestDetailWindow) _npcQuestDetailWindow.Close();
            if (IsOpenCurrentQuestDetailWindow) _currentQuestDetailWindow.Close();

            if (IsOpenCurrentQuestsWindow) _currentQuestsWindow.Close();

            if (IsOpenShopWindow) _shopWindow.Close();
            if (IsOpenTransactionWindow) _transactionWindow.Close();

            if (IsOpenInventoryWindow) _inventoryWindow.Close();
            if (IsOpenInventoryWindow) _inventoryItemInfoWindow.Close();
            if (IsOpenSkillsWindow) _skillsWindow.Close();

            if (IsOpenEscWindow) _escWindow.gameObject.SetActive(false);

            if (IsOpenOnDeathWindow) _onDeathWindow.gameObject.SetActive(false);
        }

        public void ToggleCurrentQuestsWindow()
        {
            if (_currentQuestsWindow)
            {
                if (IsOpenCurrentQuestsWindow) _currentQuestsWindow.Close();
                else _currentQuestsWindow.Open();
            }
        }

        public void HighlightQuest(int questIndex)
        {
            _currentQuestsWindow.HighlightSlot(questIndex);
        }

        public void UnhighlightQuest(int questIndex)
        {
            _currentQuestsWindow.UnhighlightSlot(questIndex);
        }

        public void UpdateQuickSlotItemLink(int oldIndex, int newIndex)
        {
            _quickSlotManager.UpdateSlotItemLink(oldIndex, newIndex);
        }
    }
}