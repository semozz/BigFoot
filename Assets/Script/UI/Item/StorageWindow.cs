using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StorageWindow : BaseItemWindow {
	public UIImageButton equipButton = null;
	public UIImageButton unEquipButton = null;
	public UIImageButton reinforceButton = null;
	public UIImageButton compositionButton = null;
	public UIImageButton sellButton = null;
	
	public CharacterInfos charInfos = null;
	
	public GameObject expandButtonPanel = null;
	
	public int maxPageCount = 10;
	
	/*
	public EquipManager equipManager = null;
	public InventoryManager inventoryManager = null;
	*/
	public override void Awake()
	{
		this.windowType = TownUI.eTOWN_UI_TYPE.STORAGE;
		
		TableManager tableManager = TableManager.Instance;
		StringValueTable stringValueTable = tableManager != null ? tableManager.stringValueTable : null;
		
		if (stringValueTable != null)
			expandSlotNeedGold.y = (float)stringValueTable.GetData("ExpandSlotGold");
		
		base.Awake();
		
		this.onChangeSelectedItem = new BaseItemWindow.OnChangeSelectedItem(ChangeSelectedItem);
		
		if (charInfos != null)
			charInfos.DeActivate();
		
		GameUI.Instance.storageWindow = this;
	}
	
	public void ChangeSelectedItem()
	{
		//PlayerController player = Game.Instance.player;
		//LifeManager lifeManager = player != null ? player.lifeManager : null;
		
		CharInfoData charData = Game.Instance.charInfoData;
		//Vector3 ownGoldValue = charData != null ? charData.goldInfo : Vector3.zero;
		int ownGold = 0;
		int ownJewel = 0;
		int ownMedal = 0;
		if (charData != null)
		{
			ownGold = charData.gold_Value;
			ownJewel = charData.jewel_Value;
			ownMedal = charData.medal_Value;
		}
		
		if (selectedSlotWindow == GameDef.eItemSlotWindow.Equip)
		{
			if (selectedItem != null || selectCostumeSetItem != null)
			{
				SetActivateButton(equipButton.gameObject, false);
				SetActivateButton(unEquipButton.gameObject, true);
				
				bool enableReinforce = false;
				bool enableComposition = false;
				if (selectedItem != null)
				{
					enableReinforce = selectedItem.CheckReinforce(ownGold, ownJewel, ownMedal);
					enableComposition = selectedItem.CheckComposition(ownGold, ownJewel, ownMedal);
				}
				
				SetActivateButton(reinforceButton.gameObject, enableReinforce);
				SetActivateButton(compositionButton.gameObject, enableComposition);
				
				SetActivateButton(sellButton.gameObject, true);
			}
			else
			{
				SetActivateButton(equipButton.gameObject, false);
				SetActivateButton(unEquipButton.gameObject, false);
				
				SetActivateButton(reinforceButton.gameObject, false);
				SetActivateButton(compositionButton.gameObject, false);
				
				SetActivateButton(sellButton.gameObject, false);
			}
		}
		else
		{
			SetActivateButton(unEquipButton.gameObject, false);
			
			bool bCheckEquip = CheckEquipItem(selectedItem, selectCostumeSetItem);
			SetActivateButton(equipButton.gameObject, bCheckEquip);
			
			bool bNullCheck = !(selectedItem == null && selectCostumeSetItem == null);
			
			bool enableReinforce = false;
			bool enableComposition = false;
			if (selectedItem != null)
			{
				enableReinforce = selectedItem.CheckReinforce(ownGold, ownJewel, ownMedal);
				enableComposition = selectedItem.CheckComposition(ownGold, ownJewel, ownMedal);
			}

			SetActivateButton(reinforceButton.gameObject, enableReinforce);
			SetActivateButton(compositionButton.gameObject, enableComposition);
			
			SetActivateButton(sellButton.gameObject, bNullCheck);
		}
		
		UpdateSelectedSlotFrame();
	}
	
	public bool CheckEquipItem(Item item, CostumeSetItem costumeSet)
	{
		bool bCheckEquipByItem = CheckEquipItem(item);
		bool bCheckEquipByCostumeSet = CheckEquipCostumeSet(costumeSet);
		
		if (bCheckEquipByItem == false && bCheckEquipByCostumeSet == false)
			return false;
		else
			return true;
	}
	
	public bool CheckEquipCostumeSet(CostumeSetItem costumeSet)
	{
		if (costumeSet == null || costumeSet.setItemInfo == null)
			return false;
		
		bool bCheck = false;
		GameDef.ePlayerClass playerClass = Game.Instance.playerClass;
		switch(costumeSet.setItemInfo.limitClass)
		{
		case ItemInfo.eClass.Warrior:
			bCheck = (playerClass == GameDef.ePlayerClass.CLASS_WARRIOR);
			break;
		case ItemInfo.eClass.Assassin:
			bCheck = (playerClass == GameDef.ePlayerClass.CLASS_ASSASSIN);
			break;
		case ItemInfo.eClass.Wizard:
			bCheck = (playerClass == GameDef.ePlayerClass.CLASS_WIZARD);
			break;
		default:
			bCheck = true;
			break;
		}
		
		if (bCheck == false)
			return false;
		
		return true;
	}
	
	public bool CheckEquipItem(Item item)
	{
		if (item == null || item.itemInfo == null)
			return false;
		
		bool bCheck = false;
		GameDef.ePlayerClass playerClass = Game.Instance.playerClass;
		switch(item.itemInfo.limitClass)
		{
		case ItemInfo.eClass.Warrior:
			bCheck = (playerClass == GameDef.ePlayerClass.CLASS_WARRIOR);
			break;
		case ItemInfo.eClass.Assassin:
			bCheck = (playerClass == GameDef.ePlayerClass.CLASS_ASSASSIN);
			break;
		case ItemInfo.eClass.Wizard:
			bCheck = (playerClass == GameDef.ePlayerClass.CLASS_WIZARD);
			break;
		default:
			bCheck = true;
			break;
		}
		
		if (bCheck == false)
			return false;
		
		bCheck = true;
		switch(item.itemInfo.itemType)
		{
		case ItemInfo.eItemType.Common:
		case ItemInfo.eItemType.Material:
		case ItemInfo.eItemType.Material_Compose:
			bCheck = false;
			break;
		}
		
		if (bCheck == false)
			return false;
		
		if (item.itemInfo.equipLevel != 0)
		{
			PlayerController player = Game.Instance.player;
			int charLevel = 0;
			if (player != null && player.lifeManager != null)
				charLevel = player.lifeManager.charLevel;
			
			if (charLevel < item.itemInfo.equipLevel)
				bCheck = false;
		}
		
		if (bCheck == false)
			return false;
		
		return true;
	}
	
	public void DoUnEquipItem()
	{
		if (StorageWindow.isTutorialMode == true)
			return;
		
		if (selectedItem == null && selectCostumeSetItem == null)
			return;
		
		if (selectedItem != null && selectedItem.itemInfo == null)
			return;
		
		IPacketSender packetSender = Game.Instance.packetSender;
		if (packetSender != null)
		{
			if (selectedItem != null)
			{
				ItemInfo.eItemType itemType = ItemInfo.eItemType.Common;
				if (selectedItem.itemInfo != null)
					itemType = selectedItem.itemInfo.itemType;
				switch(itemType)
				{
				case ItemInfo.eItemType.Costume_Back:
				case ItemInfo.eItemType.Costume_Body:
				case ItemInfo.eItemType.Costume_Head:
					packetSender.SendRequestDoUnEquipCostume(selectedItemIndex, selectedItem.uID, selectedItem.itemInfo.itemID);
					break;
				default:
					packetSender.SendRequestDoUnEquipItem(selectedItemIndex, selectedItem.uID, selectedItem.itemInfo.itemID);
					break;
				}
				
				requestCount++;
			}
			else if (selectCostumeSetItem != null)
			{
				packetSender.SendRequestDoUnEquipCostumeSetItem(selectCostumeSetItem.UID, selectCostumeSetItem.setItemInfo.setID);
				
				requestCount++;
				
				//packetSender.SendRequestDoUnEquipItem(
				/*
				int charIndex = Game.Instance.connector.charIndex;
				CharPrivateData privateData = Game.Instance.charInfoData.privateDatas[charIndex];
				
				List<EquipInfo> equipItems = null;
				CostumeSetItem costumeSetItem = null;
				if (privateData != null)
				{
					equipItems = privateData.equipData;
					privateData.costumeSetItem = null;
				}
				
				if (avatarCam != null)
				{
					avatarCam.playerClass = Game.Instance.playerClass;
					avatarCam.ChangeAvatar();
					
					avatar = avatarCam.avatar;
				}
				
				SetEquipItems(equipItems, costumeSetItem);
				
				CharInfoData charData = Game.Instance.charInfoData;
				if (charData != null)
					charData.inventoryCostumeSetData.Add(selectCostumeSetItem);
				
				BaseItemScrollView costumeSetView = tabViewControl.GetItemWindow(GameDef.eItemSlotWindow.CostumeSet);
				if (costumeSetView != null)
				{
					CostumeSetItemScrollView view = (CostumeSetItemScrollView)costumeSetView;
					int nCount = view.costumeSetItemPanels.Count;
					costumeSetView.SetItem(nCount, selectCostumeSetItem);
				}
				
				if (equipItemPage != null)
					equipItemPage.ActiveCostumeSlot(true);
				
				selectCostumeSetItem = null;
				*/
			}
		}
	}
	
	public void OnUnEquipResult(int slotIndex, NetErrorCode errorCode)
	{
		requestCount--;
		
		if (errorCode != NetErrorCode.OK)
		{
			OnErrorMessage(errorCode, this);
			return;
		}
		
		PlayerController player = Game.Instance.player;
		EquipManager equipManager = null;
		InventoryManager invenManager = null;
		
		if (player != null && player.lifeManager != null)
		{
			equipManager = player.lifeManager.equipManager;
			invenManager = player.lifeManager.inventoryManager;
		}
		
		BaseItemScrollView invenWindow = null;
		BaseItemScrollView costumeWindow = null;
		BaseItemScrollView costumeSetWindow = null;
		if (tabViewControl != null)
		{
			invenWindow = tabViewControl.GetItemWindow(GameDef.eItemSlotWindow.Inventory);
			costumeWindow = tabViewControl.GetItemWindow(GameDef.eItemSlotWindow.Costume);
			costumeSetWindow = tabViewControl.GetItemWindow(GameDef.eItemSlotWindow.CostumeSet);
		}
		
		//List<EquipInfo> equipItems = null;
		if (equipItemPage != null && equipManager != null)
		{
			//equipItemPage.SetEquipItems(equipManager.equipItems);
			this.SetEquipItems(equipManager.equipItems, equipManager.costumeSetItem);
			
			//equipItems = equipManager.equipItems;
		}
		
		if (invenManager != null)
		{
			if (invenWindow != null)
				SetWindowItems(invenWindow, invenManager.itemList);
			
			if (costumeWindow != null)
				SetWindowItems(costumeWindow, invenManager.costumeList);
			
			if (costumeSetWindow != null)
			{
				costumeSetWindow.InitData();
				SetWindowItems(costumeSetWindow, invenManager.costumeSetList);
			}
		}
		
		OnSelectStorageItem(GameDef.eItemSlotWindow.Inventory);
		CloseCharInfos();
		
		if (player != null)
			player.Post_UpdateAbilityData();
	}

	
	public void DoEquipItem()
	{
		if (CheckTutorialModeStep(TownTutorialInfo.eTownTutorialStep.ActiveEquipButton) == true)
			return;
		
		if (selectedItem == null && selectCostumeSetItem == null)
			return;
		
		if (selectedItem != null && selectedItem.itemInfo == null)
			return;
		
		IPacketSender packetSender = Game.Instance.packetSender;
		if (packetSender != null)
		{
			int charIndex = Game.Instance.connector.charIndex;
			CharPrivateData privateData = Game.Instance.charInfoData.privateDatas[charIndex];
				
			if (selectedItem != null)
			{
				ItemInfo.eItemType itemType = ItemInfo.eItemType.Common;
				if (selectedItem.itemInfo != null)
					itemType = selectedItem.itemInfo.itemType;
				
				int equipSlotIndex = EquipInfo.ItemTypeToEquipSlotIndex(privateData, selectedItem.itemInfo.itemType);
				
				switch(itemType)
				{
				case ItemInfo.eItemType.Costume_Back:
				case ItemInfo.eItemType.Costume_Body:
				case ItemInfo.eItemType.Costume_Head:
					packetSender.SendRequestDoEquipCostumeItem(equipSlotIndex, selectedItemIndex, selectedItem.itemInfo.itemID, selectedItem.uID);
					break;
				default:
					packetSender.SendRequestDoEquipItem(equipSlotIndex, selectedItemIndex, selectedItem.itemInfo.itemID, selectedItem.uID, selectedSlotWindow);
					break;
				}
				
				requestCount++;
			}
			else if (selectCostumeSetItem != null)
			{
				packetSender.SendRequestDoEquipCostumeSetItem(selectedItemIndex, selectCostumeSetItem.setItemInfo.setID, selectCostumeSetItem.UID, selectedSlotWindow);
				
				requestCount++;
			}
		}
	}
	
	public void OnEquipResult(int slotIndex, GameDef.eItemSlotWindow windowType, NetErrorCode errorCode)
	{
		requestCount--;
		
		if (errorCode != NetErrorCode.OK)
		{
			OnErrorMessage(errorCode, this);
			return;
		}
		
		PlayerController player = Game.Instance.player;
		EquipManager equipManager = null;
		InventoryManager invenManager = null;
		
		if (player != null && player.lifeManager != null)
		{
			equipManager = player.lifeManager.equipManager;
			invenManager = player.lifeManager.inventoryManager;
		}
		
		BaseItemScrollView invenWindow = null;
		BaseItemScrollView costumeWindow = null;
		BaseItemScrollView costumeSetWindow = null;
		
		if (tabViewControl != null)
		{
			invenWindow = tabViewControl.GetItemWindow(GameDef.eItemSlotWindow.Inventory);
			costumeWindow = tabViewControl.GetItemWindow(GameDef.eItemSlotWindow.Costume);
			
			costumeSetWindow = tabViewControl.GetItemWindow(GameDef.eItemSlotWindow.CostumeSet);
		}
		
		//List<EquipInfo> equipItems = null;
		if (equipItemPage != null && equipManager != null)
		{
			//equipItemPage.SetEquipItems(equipManager.equipItems);
			this.SetEquipItems(equipManager.equipItems, equipManager.costumeSetItem);
			
			//equipItems = equipManager.equipItems;
		}
		
		if (invenManager != null)
		{
			if (invenWindow != null)
				SetWindowItems(invenWindow, invenManager.itemList);
			
			if (costumeWindow != null)
				SetWindowItems(costumeWindow, invenManager.costumeList);
			
			if (costumeSetWindow != null)
			{
				costumeSetWindow.InitData();
				SetWindowItems(costumeSetWindow, invenManager.costumeSetList);
			}
		}
		
		
		selectedItemIndex = slotIndex;
		OnSelectStorageItem(windowType);
		CloseCharInfos();
		
		if (StorageWindow.isTutorialMode == true)
		{
			TownUI townUI = GameUI.Instance.townUI;
			if (townUI != null && townUI.tutorialController != null)
				townUI.tutorialController.NextStep();
		}
		
		if (player != null)
			player.Post_UpdateAbilityData();
	}
	
	public void UpdateWindow()
	{
		CharInfoData charInfoData = Game.Instance.charInfoData;
		
		int nCount = -1;
		if (charInfoData != null)
			nCount = charInfoData.stageRewardItems.Count;
		
		Item item = null;
		int addIndex = -1;
		for (int index = 0; index < nCount; ++index)
		{
			item = charInfoData.stageRewardItems[index];
			
			addIndex = GetEmptyIndex(charInfoData.inventoryNormalData);
			if (addIndex == -1)
				charInfoData.inventoryNormalData.Add(item);
			else
				charInfoData.inventoryNormalData[addIndex] = item;
		}
		
		charInfoData.stageRewardItems.Clear();
		
		PlayerController player = Game.Instance.player;
		InventoryManager invenManager = null;
		//EquipManager equipManager = null;
		if (player != null && player.lifeManager != null)
		{
			invenManager = player.lifeManager.inventoryManager;
			//equipManager = player.lifeManager.equipManager;
		}
		
		if (invenManager != null)
		{
			//Game.RerangeItemList(charInfoData.inventoryNormalData);
			//Game.RerangeItemList(charInfoData.inventoryCostumeData);
			
			invenManager.SetInvenItemData(charInfoData.inventoryNormalData);
			invenManager.SetInvenCostumeData(charInfoData.inventoryCostumeData);
			invenManager.SetMaterialItemData(charInfoData.inventoryMaterialData);
		}
		
		base.InitWindow();
		
		
		BaseItemScrollView invenWindow = null;
		BaseItemScrollView costumeWindow = null;
		BaseItemScrollView materialWindow = null;
		if (tabViewControl != null)
		{
			invenWindow = tabViewControl.GetItemWindow(GameDef.eItemSlotWindow.Inventory);
			costumeWindow = tabViewControl.GetItemWindow(GameDef.eItemSlotWindow.Costume);
			materialWindow = tabViewControl.GetItemWindow(GameDef.eItemSlotWindow.MaterialItem);
		}
		
		if (invenManager != null)
		{
			if (invenWindow != null)
			{
				SetWindowItems(invenWindow, invenManager.itemList);
			
				//invenWindow.Invoke("InitPage", 0.2f);
			}
			
			if (costumeWindow != null)
			{
				SetWindowItems(costumeWindow, invenManager.costumeList);
			
				//costumeWindow.Invoke("InitPage", 0.2f);
			}
			
			if (materialWindow != null)
			{
				SetWindowItems(materialWindow, invenManager.materialItemList);
			}
		}
		
		OnSelectItem(null);
	}
	
	public override void InitWindow()
	{
		CloseCharInfos();
		
		CharInfoData charInfoData = Game.Instance.charInfoData;
		
		int nCount = -1;
		if (charInfoData != null)
			nCount = charInfoData.stageRewardItems.Count;
		
		Item item = null;
		for (int index = 0; index < nCount; ++index)
		{
			item = charInfoData.stageRewardItems[index];
			
			charInfoData.AddItemOnEmptySlotOrEnd(item);
		}
		
		charInfoData.stageRewardItems.Clear();
		
		PlayerController player = Game.Instance.player;
		InventoryManager invenManager = null;
		//EquipManager equipManager = null;
		if (player != null && player.lifeManager != null)
		{
			invenManager = player.lifeManager.inventoryManager;
			//equipManager = player.lifeManager.equipManager;
		}
		
		if (invenManager != null)
		{
			Game.RerangeItemList(charInfoData.inventoryNormalData);
			Game.RerangeItemList(charInfoData.inventoryCostumeData);
			Game.RerangeItemList(charInfoData.inventoryMaterialData);
			
			invenManager.SetInvenItemData(charInfoData.inventoryNormalData);
			invenManager.SetInvenCostumeData(charInfoData.inventoryCostumeData);
			invenManager.SetMaterialItemData(charInfoData.inventoryMaterialData);
			invenManager.SetCostumeSetItems(charInfoData.inventoryCostumeSetData);
		}
		
		base.InitWindow();
		
		
		BaseItemScrollView invenWindow = null;
		BaseItemScrollView costumeWindow = null;
		BaseItemScrollView costumeSetWindow = null;
		BaseItemScrollView materialWindow = null;
		
		if (tabViewControl != null)
		{
			invenWindow = tabViewControl.GetItemWindow(GameDef.eItemSlotWindow.Inventory);
			costumeWindow = tabViewControl.GetItemWindow(GameDef.eItemSlotWindow.Costume);
			costumeSetWindow = tabViewControl.GetItemWindow(GameDef.eItemSlotWindow.CostumeSet);
			materialWindow = tabViewControl.GetItemWindow(GameDef.eItemSlotWindow.MaterialItem);
		}
		
		if (invenManager != null)
		{
			if (invenWindow != null)
			{
				ItemPageScrollView normalItemScrollView = (ItemPageScrollView)invenWindow;
				
				int maxCount = charInfoData.baseItemSlotCount + charInfoData.expandNormalItemSlotCount;
				if (normalItemScrollView != null)
				{
					normalItemScrollView.maxItems = maxCount;
					normalItemScrollView.maxPageCount = maxPageCount;
				}
				
				SetWindowItems(invenWindow, invenManager.itemList, maxCount);
			
				invenWindow.Invoke("InitPage", 0.2f);
			}
			
			if (materialWindow != null)
			{
				ItemPageScrollView materialItemScrollView = (ItemPageScrollView)materialWindow;
				
				int maxCount = charInfoData.baseItemSlotCount + charInfoData.expandMaterialItemSlotCount;
				if (materialItemScrollView != null)
				{
					materialItemScrollView.maxItems = maxCount;
					materialItemScrollView.maxPageCount = maxPageCount;
				}
				
				SetWindowItems(materialWindow, invenManager.materialItemList, maxCount);
				
				materialWindow.Invoke("InitPage", 0.2f);
			}
			
			
			if (costumeWindow != null)
			{
				SetWindowItems(costumeWindow, invenManager.costumeList);
			
				costumeWindow.Invoke("InitPage", 0.2f);
			}
			
			
			if (costumeSetWindow != null)
			{
				SetWindowItems(costumeSetWindow, invenManager.costumeSetList);
				costumeSetWindow.Invoke("InitPage", 0.2f);
			}
		}
		
		OnSelectItem(null);
		
		bInitWindow = true;
		
		if (expandButtonPanel != null)
			expandButtonPanel.SetActive(false);
	}
	
	public string reinforceWindowPrefabPath = "";
	public ReinforceWindow reinforceWindow = null;
	public void OnReinforceItem()
	{
		if (CheckTutorialModeStep(TownTutorialInfo.eTownTutorialStep.ActiveReinforceButton) == true)
			return;
		
		TownUI townUI = GameUI.Instance.townUI;
		if (townUI != null)
			townUI.toWindowtype = TownUI.eTOWN_UI_TYPE.NONE;
		
		if (reinforceWindow == null)
		{
			reinforceWindow = ResourceManager.CreatePrefab<ReinforceWindow>(reinforceWindowPrefabPath, popupNode, Vector3.zero);
			if (reinforceWindow != null)
				reinforceWindow.parentWindow = this;
			
		}
		else
		{
			reinforceWindow.gameObject.SetActive(true);
			//reinforceWindow.InitMap();
		}
		
		reinforceWindow.SetReinforceItem(selectedItem, selectedItemIndex, selectedSlotWindow);
		reinforceWindow.SetMode(ReinforceWindow.eReinforceStep.Wait);
		
		OnChildWindow(true);
		
		if (StorageWindow.isTutorialMode == true)
		{
			//TownUI townUI = GameUI.Instance.townUI;
			if (townUI != null && townUI.tutorialController != null)
				townUI.tutorialController.ClearAlret();
		}
	}
	
	public string compositionWindowPrefabPath = "";
	public CompositionWindow compositionWindow = null;
	public void OnCompositionItem()
	{
		if (CheckTutorialModeStep(TownTutorialInfo.eTownTutorialStep.ActiveComposeButton) == true)
			return;
		
		TownUI townUI = GameUI.Instance.townUI;
		if (townUI != null)
			townUI.toWindowtype = TownUI.eTOWN_UI_TYPE.NONE;
			
		if (compositionWindow == null)
		{
			compositionWindow = ResourceManager.CreatePrefab<CompositionWindow>(compositionWindowPrefabPath, popupNode, Vector3.zero);
			if (compositionWindow != null)
				compositionWindow.parentWindow = this;
		}
		else
		{
			compositionWindow.gameObject.SetActive(true);
			//reinforceWindow.InitMap();
		}
		
		compositionWindow.SetCompositionItem(selectedItem, selectedItemIndex, selectedSlotWindow);
		compositionWindow.SetCompositionMaterials();
		compositionWindow.SetMode(CompositionWindow.eReinforceStep.Wait);
		
		OnChildWindow(true);
		
		if (StorageWindow.isTutorialMode == true)
		{
			//TownUI townUI = GameUI.Instance.townUI;
			if (townUI != null && townUI.tutorialController != null)
				townUI.tutorialController.ClearAlret();
		}
	}
	
	public void OnSellItem(GameObject obj)
	{
		if (StorageWindow.isTutorialMode == true)
			return;
		
		if (selectedItem != null)
			OnSellConfirmPopup(selectedItem);
		else if (selectCostumeSetItem != null)
			OnSellConfirmPopup(selectCostumeSetItem);
	}
	
	public void OnConfirmPopupCancel(GameObject obj)
	{
		ClosePopup();
	}
	
	public void OnSellItemOK(GameObject obj)
	{
		ClosePopup();
		
		switch(selectedSlotWindow)
		{
		case GameDef.eItemSlotWindow.Inventory:
			OnSellItemFromInventory();
			break;
		case GameDef.eItemSlotWindow.MaterialItem:
			OnSellItemFromMaterial();
			break;
		case GameDef.eItemSlotWindow.Equip:
			OnSellItemFromEquip();
			break;
		case GameDef.eItemSlotWindow.Costume:
			OnSellItemFromCostume();
			break;
		case GameDef.eItemSlotWindow.CostumeSet:
			OnSellItemFromCostumeSet();
			break;
		}
	}
	
	public string sellConfirmPopupPrefabPath = "UI/Item/SellConfirmPopup";
	public void OnSellConfirmPopup(Item item)
	{
		BaseConfirmPopup sellConfirmPopup = ResourceManager.CreatePrefab<BaseConfirmPopup>(sellConfirmPopupPrefabPath, popupNode, Vector3.zero);
		if (sellConfirmPopup != null)
		{
			sellConfirmPopup.cancelButtonMessage.target = this.gameObject;
			sellConfirmPopup.cancelButtonMessage.functionName = "OnConfirmPopupCancel";
			
			sellConfirmPopup.okButtonMessage.target = this.gameObject;
			sellConfirmPopup.okButtonMessage.functionName = "OnSellItemOK";
			
			sellConfirmPopup.SetGoldInfo(item, false);
			
			popupList.Add(sellConfirmPopup);
		}
	}
	
	public void OnSellConfirmPopup(CostumeSetItem item)
	{
		BaseConfirmPopup sellConfirmPopup = ResourceManager.CreatePrefab<BaseConfirmPopup>(sellConfirmPopupPrefabPath, popupNode, Vector3.zero);
		if (sellConfirmPopup != null)
		{
			sellConfirmPopup.cancelButtonMessage.target = this.gameObject;
			sellConfirmPopup.cancelButtonMessage.functionName = "OnConfirmPopupCancel";
			
			sellConfirmPopup.okButtonMessage.target = this.gameObject;
			sellConfirmPopup.okButtonMessage.functionName = "OnSellItemOK";
			
			sellConfirmPopup.SetGoldInfo(item, false);
			
			popupList.Add(sellConfirmPopup);
		}
	}
	
	public void OnSellItemFromMaterial()
	{
		if (selectedItem != null)
		{
			IPacketSender packetSender = Game.Instance.packetSender;
			if (packetSender != null)
			{
				// todo.semo
				int itemID = selectedItem.itemInfo.itemID;
				string uID = selectedItem.uID;
				packetSender.SendRequestSellMaterialItemFromStorage(packetSender.Connector.charIndex, selectedItemIndex, itemID, uID);
				
				requestCount++;
			}
		}
	}
	
	public void OnSellMaterialItemResult(int slotIndex, NetErrorCode errorCode)
	{
		requestCount--;
		
		if (errorCode != NetErrorCode.OK)
		{
			OnErrorMessage(errorCode, this);
			return;
		}
		
		PlayerController player = Game.Instance.player;
		LifeManager lifeManager = player != null ? player.lifeManager : null;
		InventoryManager invenManager = lifeManager != null ? lifeManager.inventoryManager : null;
		
		BaseItemScrollView invenWindow = null;
		if (tabViewControl != null)
			invenWindow = tabViewControl.GetItemWindow(GameDef.eItemSlotWindow.MaterialItem);
		
		if (invenWindow != null && invenManager != null)
			SetWindowItems(invenWindow, invenManager.materialItemList);
		
		UpdateCoinInfo(true);

		OnSelectStorageItem(GameDef.eItemSlotWindow.MaterialItem);
	}
	
	public void OnSellItemFromInventory()
	{
		if (selectedItem != null)
		{
			IPacketSender packetSender = Game.Instance.packetSender;
			if (packetSender != null)
			{
				// todo.semo
				int itemID = selectedItem.itemInfo.itemID;
				string uID = selectedItem.uID;
				packetSender.SendRequestSellNormalItemFromStorage(packetSender.Connector.charIndex, selectedItemIndex, itemID, uID);
				
				requestCount++;
			}
		}
	}
	
	public void OnSellNormalItemResult(int slotIndex, NetErrorCode errorCode)
	{
		requestCount--;
		
		if (errorCode != NetErrorCode.OK)
		{
			OnErrorMessage(errorCode, this);
			return;
		}
		
		PlayerController player = Game.Instance.player;
		LifeManager lifeManager = player != null ? player.lifeManager : null;
		InventoryManager invenManager = lifeManager != null ? lifeManager.inventoryManager : null;
		
		BaseItemScrollView invenWindow = null;
		if (tabViewControl != null)
			invenWindow = tabViewControl.GetItemWindow(GameDef.eItemSlotWindow.Inventory);
		
		if (invenWindow != null && invenManager != null)
			SetWindowItems(invenWindow, invenManager.itemList);
		
		UpdateCoinInfo(true);

		OnSelectStorageItem(GameDef.eItemSlotWindow.Inventory);
	}
	
		
	public void OnSelectStorageItem(GameDef.eItemSlotWindow window)
	{
		PlayerController player = Game.Instance.player;
		LifeManager lifeManager = player != null ? player.lifeManager : null;
		InventoryManager invenManager = lifeManager != null ? lifeManager.inventoryManager : null;
		
		switch(window)
		{
		case GameDef.eItemSlotWindow.Equip:
			if (selectedItemIndex == 100)
			{
				//if (equipItemPage.costumeSetPanel != null)
					
			}
			else
			{
				Item equipItem = equipItemPage.GetItem(selectedItemIndex);
				if (equipItem == null)
					OnSelectItem(null);
				else
					OnSelectItem(equipItem, selectedItemIndex, window);
			}
			break;
		case GameDef.eItemSlotWindow.Inventory:
		case GameDef.eItemSlotWindow.MaterialItem:
		case GameDef.eItemSlotWindow.Costume:
			Item item = invenManager.GetItem(selectedItemIndex, window);
			if (item == null)
				OnSelectItem(null);
			else
				OnSelectItem(item, selectedItemIndex, window);
			break;
		case GameDef.eItemSlotWindow.CostumeSet:
			CostumeSetItem costumeSet = invenManager.GetCostumeSetItem(selectedItemIndex, window);
			if (costumeSet == null)
				OnSelectItem(null);
			else
				OnSelectItem(costumeSet, selectedItemIndex, window);
			break;
		}
		
		if (equipItemPage != null)
		{
			bool bShowCostume = equipItemPage.isShowCostume;
			equipItemPage.ActiveCostumeSlot(bShowCostume);
		}
	}
	
	public void OnSellItemFromCostumeSet()
	{
		if (selectCostumeSetItem != null)
		{
			IPacketSender packetSender = Game.Instance.packetSender;
			if (packetSender != null)
			{
				// todo.semo
				int itemID = selectCostumeSetItem.setItemInfo.setID;
				string uID = selectCostumeSetItem.UID;
				packetSender.SendRequestSellCostumeSetItemFromStorage(packetSender.Connector.charIndex, selectedItemIndex, itemID, uID);
				
				requestCount++;
			}
		}
	}
	
	public void OnSellCostumeSetItemResult(int slotIndex, NetErrorCode errorCode)
	{
		requestCount--;
		
		if (errorCode != NetErrorCode.OK)
		{
			OnErrorMessage(errorCode, this);
			return;
		}

		PlayerController player = Game.Instance.player;
		LifeManager lifeManager = player != null ? player.lifeManager : null;
		InventoryManager invenManager = lifeManager != null ? lifeManager.inventoryManager : null;
		
		BaseItemScrollView costumeWindow = null;
		if (tabViewControl != null)
			costumeWindow = tabViewControl.GetItemWindow(GameDef.eItemSlotWindow.CostumeSet);
		
		if (costumeWindow != null && invenManager != null)
		{
			costumeWindow.InitData();
			SetWindowItems(costumeWindow, invenManager.costumeSetList);
		}
		
		UpdateCoinInfo(true);

		OnSelectStorageItem(GameDef.eItemSlotWindow.CostumeSet);
	}
	
	public void OnSellItemFromCostume()
	{
		if (selectedItem != null)
		{
			IPacketSender packetSender = Game.Instance.packetSender;
			if (packetSender != null)
			{
				// todo.semo
				int itemID = selectedItem.itemInfo.itemID;
				string uID = selectedItem.uID;
				packetSender.SendRequestSellCostumeItemFromStorage(packetSender.Connector.charIndex, selectedItemIndex, itemID, uID);
				
				requestCount++;
			}
		}
	}
	
	public void OnSellCostumeItemResult(int slotIndex, NetErrorCode errorCode)
	{
		requestCount--;
		
		if (errorCode != NetErrorCode.OK)
		{
			OnErrorMessage(errorCode, this);
			return;
		}

		PlayerController player = Game.Instance.player;
		LifeManager lifeManager = player != null ? player.lifeManager : null;
		InventoryManager invenManager = lifeManager != null ? lifeManager.inventoryManager : null;
		
		BaseItemScrollView costumeWindow = null;
		if (tabViewControl != null)
			costumeWindow = tabViewControl.GetItemWindow(GameDef.eItemSlotWindow.Costume);
		
		if (costumeWindow != null && invenManager != null)
			SetWindowItems(costumeWindow, invenManager.costumeList);
		
		UpdateCoinInfo(true);

		OnSelectStorageItem(GameDef.eItemSlotWindow.Costume);
	}
	
	public void OnSellItemFromEquip()
	{
		int itemID = -1;
		string UID = "";
		
		if (selectedItem != null)
		{
			itemID = selectedItem.itemInfo.itemID;
			UID = selectedItem.uID;
		}
		else if (selectCostumeSetItem != null)
		{
			itemID = selectCostumeSetItem.setItemInfo.setID;
			UID = selectCostumeSetItem.UID;
		}
		
		if (itemID != -1 && UID != "")
		{
			IPacketSender packetSender = Game.Instance.packetSender;
			if (packetSender != null)
			{
				packetSender.SendRequestSellEquipItemFromStorage(packetSender.Connector.charIndex, selectedItemIndex, itemID, UID);
				
				requestCount++;
			}
		}
		
	}
	
	public void OnSellEquipItemResult(int slotIndex, NetErrorCode errorCode)
	{
		requestCount--;
		
		if (errorCode != NetErrorCode.OK)
		{
			OnErrorMessage(errorCode, this);
			return;
		}

		PlayerController player = Game.Instance.player;
		LifeManager lifeManager = player != null ? player.lifeManager : null;
		EquipManager equipManager = lifeManager != null ? lifeManager.equipManager : null;
		
		if (equipManager != null)
			SetEquipItems(equipManager.equipItems, equipManager.costumeSetItem);
		
		UpdateCoinInfo(true);

		OnSelectItem(null);
	}
	
	public override void OnCostumeTabActive(bool bActive)
	{
		if (equipItemPage != null)
		{
			equipItemPage.ActiveCostumeSlot(bActive);
		}
		
		UpdateExpandButtonPanel();
	}
	
	public void OpenCharInfos(GameObject obj)
	{
		if (StorageWindow.isTutorialMode == true)
			return;
		
		if (charInfos != null)
		{
			PlayerController player = Game.Instance.player;
			charInfos.SetCharInfo(player);
			
			charInfos.OnToggle(true);
		}
	}
	
	public void CloseCharInfos()
	{
		if (charInfos != null)
		{
			charInfos.OnToggle(false);
		}
	}
	
	public void UpdateExpandButtonPanel()
	{
		TabViewInfo activeTabInfo = null;
		
		if (tabViewControl != null)
		{
			foreach(TabViewInfo info in tabViewControl.tabViewInfos)
			{
				if (info == null)
					continue;
				
				if (info.tabView == null)
					continue;
				
				if (info.tabView.activeInHierarchy == true)
				{
					activeTabInfo = info;
					break;
				}
			}
		}
		
		ItemPageScrollView itemPageScrollView = null;
		GameDef.eItemSlotWindow activeSlotWindow = GameDef.eItemSlotWindow.Inventory;
		
		if (activeTabInfo != null)
		{
			switch(activeTabInfo.slotWindowType)
			{
			case GameDef.eItemSlotWindow.Inventory:
			case GameDef.eItemSlotWindow.MaterialItem:
				itemPageScrollView = activeTabInfo.tabView.GetComponent<ItemPageScrollView>();
				activeSlotWindow = activeTabInfo.slotWindowType;
				break;
			}
		}
		
		bool isLastPageActive = false;
		if (itemPageScrollView != null)
			isLastPageActive = itemPageScrollView.CheckLastPageActive();
		
		OnLastPageOn(isLastPageActive, activeSlotWindow);
	}
	
	private GameDef.eItemSlotWindow expandSlotWindow = GameDef.eItemSlotWindow.Inventory;
	public void OnLastPageOn(bool isLastPage, GameDef.eItemSlotWindow slotWindow)
	{
		if (isLastPage == true)
			expandSlotWindow = slotWindow;
		
		if (expandButtonPanel != null)
			expandButtonPanel.SetActive(isLastPage);
	}
	
	public Vector3 expandSlotNeedGold = Vector3.zero;
	public void OnRequestExpandSlot(GameObject obj)
	{
		CashItemType cashCheck = CheckNeedGold(expandSlotNeedGold);
		if (cashCheck != CashItemType.None)
		{
			OnNeedMoneyPopup(cashCheck, this);
			return;
		}
		
		OnExpandSlotConfirmPopup();
	}
	
	public string expandSlotsConfirmPopupPrefab = "UI/Item/ExpandSlotPopup";
	public void OnExpandSlotConfirmPopup()
	{
		BaseConfirmPopup confirmPopup = ResourceManager.CreatePrefab<BaseConfirmPopup>(expandSlotsConfirmPopupPrefab, popupNode, Vector3.zero);
		if (confirmPopup != null)
		{
			confirmPopup.cancelButtonMessage.target = this.gameObject;
			confirmPopup.cancelButtonMessage.functionName = "OnConfirmPopupCancel";
			
			confirmPopup.okButtonMessage.target = this.gameObject;
			confirmPopup.okButtonMessage.functionName = "OnExpandSlotOK";
			
			confirmPopup.SetGoldInfo("", expandSlotNeedGold);
			
			popupList.Add(confirmPopup);
		}
	}
	
	public void OnExpandSlotOK(GameObject obj)
	{
		ClosePopup();
		
		if (expandButtonPanel != null)
		{
			if (expandButtonPanel.activeInHierarchy == false)
				return;
		
			IPacketSender sender = Game.Instance.packetSender;
			if (sender != null)
				sender.SendRequestExpandSlots(expandSlotWindow);
		}
	}
	
	public GameDef.eItemSlotWindow activeTabType = GameDef.eItemSlotWindow.Inventory;
	public void TabButtonActive()
	{
		if (tabViewControl != null)
		{
			bool isActive = false;
			UICheckbox checkBox = null;
			
			foreach(TabViewInfo info in tabViewControl.tabViewInfos)
			{
				if (info == null)
					continue;
				
				isActive = info.slotWindowType == activeTabType;
				
				if (info.tabButton != null)
					checkBox = info.tabButton.GetComponent<UICheckbox>();
				
				if (checkBox != null)
					checkBox.isChecked = isActive;
			}
		}
	}
	
	public void InitTabWindow(GameDef.eItemSlotWindow windowType)
	{
		CharInfoData charInfoData = Game.Instance.charInfoData;
		
		PlayerController player = Game.Instance.player;
		InventoryManager invenManager = null;
		
		if (player != null && player.lifeManager != null)
		{
			invenManager = player.lifeManager.inventoryManager;
		}
		
		if (invenManager != null)
		{
			switch(windowType)
			{
			case GameDef.eItemSlotWindow.Inventory:
				Game.RerangeItemList(charInfoData.inventoryNormalData);
				invenManager.SetInvenItemData(charInfoData.inventoryNormalData);
				
				BaseItemScrollView invenWindow = null;
				if (tabViewControl != null)
					invenWindow = tabViewControl.GetItemWindow(GameDef.eItemSlotWindow.Inventory);
				
				if (invenWindow != null)
				{
					ItemPageScrollView normalItemScrollView = (ItemPageScrollView)invenWindow;
					
					int maxCount = charInfoData.baseItemSlotCount + charInfoData.expandNormalItemSlotCount;
					if (normalItemScrollView != null)
					{
						normalItemScrollView.maxItems = maxCount;
						normalItemScrollView.maxPageCount = maxPageCount;
					}
					
					SetWindowItems(invenWindow, invenManager.itemList, maxCount);
				
					invenWindow.Invoke("InitPage", 0.2f);
				}
				break;
			case GameDef.eItemSlotWindow.MaterialItem:
				Game.RerangeItemList(charInfoData.inventoryMaterialData);
				invenManager.SetMaterialItemData(charInfoData.inventoryMaterialData);
				
				BaseItemScrollView mateiralWindow = null;
				if (tabViewControl != null)
					mateiralWindow = tabViewControl.GetItemWindow(GameDef.eItemSlotWindow.MaterialItem);
				
				if (mateiralWindow != null)
				{
					ItemPageScrollView materialItemScrollView = (ItemPageScrollView)mateiralWindow;
					
					int maxCount = charInfoData.baseItemSlotCount + charInfoData.expandMaterialItemSlotCount;
					if (materialItemScrollView != null)
					{
						materialItemScrollView.maxItems = maxCount;
						materialItemScrollView.maxPageCount = maxPageCount;
					}
					
					SetWindowItems(mateiralWindow, invenManager.materialItemList, maxCount);
				
					mateiralWindow.Invoke("InitPage", 0.2f);
				}
				break;
			}
		}
		
		OnSelectItem(null);
		
		if (expandButtonPanel != null)
			expandButtonPanel.SetActive(false);
	}
	
	public override void OnBack()
	{
		TownTutorialController townTutorial = null;
		
		if (StorageWindow.isTutorialMode == true)
		{
			TownUI townUI = GameUI.Instance.townUI;
			TownTutorialInfo currentStep = null;
			
			if (townUI != null)
				townTutorial = townUI.tutorialController;
			
			if (townTutorial != null)
				currentStep = townTutorial.currentInfo;
			
			if (currentStep != null && currentStep.step != TownTutorialInfo.eTownTutorialStep.ActiveStorageCloseButton)
				return;
		}
		
		base.OnBack();
		
		CharInfoData charData = Game.Instance.charInfoData;
		if (charData != null)
			charData.ResetNewItems();
		
		if (townTutorial != null)
			townTutorial.NextStep();
	}
	
	
	public static bool isTutorialMode = false;
	public void SetTutorialMode(bool isTutorialMode)
	{
		if (StorageWindow.isTutorialMode == isTutorialMode)
			return;
		
		StorageWindow.isTutorialMode = isTutorialMode;
		
		Item firstItem = null;
		
		if (this.tabViewControl != null)
		{
			foreach(TabViewInfo info in this.tabViewControl.tabViewInfos)
			{
				if (info != null && info.tabButton != null)
				{
					BoxCollider buttonCollider = (BoxCollider)info.tabButton.collider;
					if (buttonCollider != null)
						buttonCollider.enabled = !StorageWindow.isTutorialMode;
				}
			}
			
			BaseItemScrollView invenWindow = tabViewControl.GetItemWindow(GameDef.eItemSlotWindow.Inventory);
			ItemPageScrollView normalItemScrollView = null;
			if (invenWindow != null)
				normalItemScrollView = (ItemPageScrollView)invenWindow;
			
			if (normalItemScrollView != null)
			{
				firstItem = normalItemScrollView.GetItem(0);
				
				normalItemScrollView.SetEnableButton(!StorageWindow.isTutorialMode);
				
				if (normalItemScrollView.uiDraggablePanel != null)
					normalItemScrollView.uiDraggablePanel.enabled = !StorageWindow.isTutorialMode;
			}
			
			
			if (this.equipItemPage != null && this.equipItemPage.itemSlots != null)
			{
				foreach(ItemSlot slot in this.equipItemPage.itemSlots)
				{
					if (slot != null && slot.buttonMsg != null)
					{
						Collider buttonCollider = slot.buttonMsg.gameObject.collider;
						if (buttonCollider != null)
							buttonCollider.enabled = !StorageWindow.isTutorialMode;
					}
				}
			}
		}
		
		if (isTutorialMode == true)
			OnSelectItem(firstItem, 0, GameDef.eItemSlotWindow.Inventory); 
	}
	
	public override void OnChildWindow (bool isOpen)
	{
		base.OnChildWindow (isOpen);
		
		if (isOpen == false)
		{
			if (StorageWindow.isTutorialMode == true)
			{
				TownUI townUI = GameUI.Instance.townUI;
				if (townUI != null && townUI.tutorialController != null)
					townUI.tutorialController.NextStep();
			}
		}
	}
	
	
	public bool CheckTutorialModeStep(TownTutorialInfo.eTownTutorialStep checkStep)
	{
		bool bCheck = false;
		
		if (StorageWindow.isTutorialMode == true)
		{
			TownTutorialController townTutorial = null;
			if (townUI != null)
				townTutorial = townUI.tutorialController;
			
			TownTutorialInfo currentInfo = null;
			if (townTutorial != null)
				currentInfo = townTutorial.currentInfo;
			
			if (currentInfo != null && currentInfo.step != checkStep)
				bCheck = true;
		}
		
		return bCheck;
	}
}
