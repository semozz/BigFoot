using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseItemWindow : PopupBaseWindow {
	
	public TownUI townUI = null;
	
	public ItemInfoPage equipItemInfoPage = null;
	public ItemInfoPage itemInfoPage = null;
	public ItemSlotPanel equipItemPage = null;
	
	public AvatarCam avatarCam = null;
	
	public TabViewControl tabViewControl = null;
	
	public int selectedItemIndex = -1;
	public GameDef.eItemSlotWindow selectedSlotWindow = GameDef.eItemSlotWindow.Inventory;
	public Item selectedItem = null;
	public CostumeSetItem selectCostumeSetItem = null;
	
	public delegate void OnChangeSelectedItem();
	public OnChangeSelectedItem onChangeSelectedItem = null;
	
	
	//public CharInfos charInfo = null;
	
	public AvatarController avatar = null;
	
	public GameObject addAvatarTexture = null;
	
	public int requestCount = 0;
	
	public override void Awake()
	{
		//if (equipItemInfoPage != null)
		//	equipItemInfoPage.OnToggle(false);
	}
	
	public virtual void OnChildWindow(bool isOpen)
	{
		if (addAvatarTexture != null)
			addAvatarTexture.SetActive(!isOpen);
	}
	
	public virtual void UpdateSelectedSlotFrame()
	{
		equipItemPage.InitSelectedSlot();
		if (tabViewControl != null)
			tabViewControl.InitSelectedSlot();
		
		if (selectedItemIndex != -1)
		{
			if (selectedSlotWindow == GameDef.eItemSlotWindow.Equip)
			{
				if (equipItemPage != null)
					equipItemPage.SetSelectedSlot(selectedItemIndex, true);
			}
			else
			{
				BaseItemScrollView window =  tabViewControl.GetItemWindow(selectedSlotWindow);
				if (window != null)
					window.SetSelectedSlot(selectedItemIndex, true);
			}
		}
	}
	
	public void SetActivateButton(GameObject buttonObj, bool bActive)
	{
		if (buttonObj != null)
		{
			buttonObj.SetActive(bActive);
		}
	}
	
	public virtual void OnSelectItem(GameObject button)
	{
		GameObject parent = null;
		ItemSlot itemSlot = null;
		CostumeSetItemPanel costumeSetItemPanel = null;
		
		int slotIndex = -1;
		GameDef.eItemSlotWindow slotWindow = GameDef.eItemSlotWindow.Inventory;
		
		if (button != null)
			costumeSetItemPanel =  button.GetComponent<CostumeSetItemPanel>();
		
		if (costumeSetItemPanel != null)
		{
			CostumeSetItem setItem = costumeSetItemPanel.setItem;
			slotIndex = costumeSetItemPanel.slotIndex;
			slotWindow = costumeSetItemPanel.slotWindowType;
			
			OnSelectItem(setItem, slotIndex, slotWindow);
		}
		else
		{
			if (button != null)
				parent = button.transform.parent.gameObject;
			
			if (parent != null)
				itemSlot = parent.GetComponent<ItemSlot>();
			
			Item item = null;
			
			if (itemSlot != null)
			{
				if (itemSlot.itemIcon != null)
					item = itemSlot.itemIcon.item;
				
				slotIndex = itemSlot.slotIndex;
				slotWindow = itemSlot.slotWindowType;
			}
			
			OnSelectItem(item, slotIndex, slotWindow);
		}
		
		if (equipItemPage != null)
		{
			bool bShowCostume = equipItemPage.isShowCostume;
			equipItemPage.ActiveCostumeSlot(bShowCostume);
		}
	}
	
	public void OnCostumeSetItem(GameObject obj)
	{
		//GameObject parent = null;
		//ItemSlot itemSlot = null;
		CostumeSetItemPanel costumeSetItemPanel = null;
		
		int slotIndex = -1;
		GameDef.eItemSlotWindow slotWindow = GameDef.eItemSlotWindow.Inventory;
		
		if (obj != null)
			costumeSetItemPanel =  obj.GetComponent<CostumeSetItemPanel>();
		
		if (costumeSetItemPanel != null)
		{
			CostumeSetItem setItem = costumeSetItemPanel.setItem;
			slotIndex = costumeSetItemPanel.slotIndex;
			slotWindow = costumeSetItemPanel.slotWindowType;
			
			OnSelectItem(setItem, slotIndex, slotWindow);
		}
	}
	
	public void InitSelectItem()
	{
		Item item = null;
		int slotIndex = -1;
		GameDef.eItemSlotWindow slotWindow = GameDef.eItemSlotWindow.Inventory;
		
		OnSelectItem(item, slotIndex, slotWindow);
	}
	
	public virtual void OnSelectItem(Item item, int slotIndex, GameDef.eItemSlotWindow slotWindow)
	{
		if (selectCostumeSetItem != null)
		{
			if (slotWindow == GameDef.eItemSlotWindow.Shop_Costume)
			{
				if (avatar != null)
					avatar.ChangeCostume(-1, -1, -1);
			}
		}
		selectCostumeSetItem = null;
		
		selectedItem = item;
		selectedItemIndex = slotIndex;
		selectedSlotWindow = slotWindow;
		
		if (selectedSlotWindow == GameDef.eItemSlotWindow.Equip)
		{
			if (itemInfoPage != null)
				itemInfoPage.SetItem(selectedItem);
			
			if (equipItemInfoPage != null)
			{
				equipItemInfoPage.OnToggle(false);
				equipItemInfoPage.SetItem(null);
			}
		}
		else
		{
			Item equipItem = null;
			if (equipItemPage != null)
			{
				if (selectedItem != null && selectedItem.itemInfo != null)
					equipItem = equipItemPage.FindItemByType(selectedItem);
				
				if (equipItem != null && equipItem.itemInfo != null)
				{
					switch(equipItem.itemInfo.itemType)
					{
					case ItemInfo.eItemType.Potion_1:
					case ItemInfo.eItemType.Potion_2:
						equipItem = null;
						break;
					}
				}
			}
			
			if (equipItemInfoPage != null)
			{
				equipItemInfoPage.OnToggle(equipItem != null);
				equipItemInfoPage.SetItem(equipItem);
			}
			
			if (itemInfoPage != null)
				itemInfoPage.SetItem(selectedItem);
		}
		
		if (onChangeSelectedItem != null)
			onChangeSelectedItem();
	}
	
	public virtual void OnSelectItem(CostumeSetItem setItem, int slotIndex, GameDef.eItemSlotWindow slotWindow)
	{
		selectedItem = null;
		
		selectCostumeSetItem = setItem;
		selectedItemIndex = slotIndex;
		selectedSlotWindow = slotWindow;
		
		if (selectedSlotWindow == GameDef.eItemSlotWindow.Equip)
		{
			if (itemInfoPage != null)
				itemInfoPage.SetCostumeSetItem(setItem);
			
			if (equipItemInfoPage != null)
			{
				equipItemInfoPage.OnToggle(false);
				equipItemInfoPage.SetCostumeSetItem(null);
			}
		}
		else
		{
			if (itemInfoPage != null)
				itemInfoPage.SetCostumeSetItem(setItem);
			
			CostumeSetItem equipCostumeSetItem = null;
			if (selectCostumeSetItem != null)
				equipCostumeSetItem = equipItemPage.costumeSetPanel.setItem;
			
			if (equipItemInfoPage != null)
			{
				equipItemInfoPage.OnToggle(equipCostumeSetItem != null);
				equipItemInfoPage.SetCostumeSetItem(equipCostumeSetItem);
			}
		}
		
		if (onChangeSelectedItem != null)
			onChangeSelectedItem();
	}
	
	/*
	public void SetEquipItems(List<Item> equipItems)
	{
		int index = 0;
		foreach(Item item in equipItems)
		{
			if (equipItemPage != null)
				equipItemPage.SetItem(index, item);
			
			++index;
		}
	}
	*/
	
	public void SetEquipItems(List<EquipInfo> equipItems, CostumeSetItem costumeSetItem)
	{
		ResetItems(equipItemPage);
		
		if (equipItems == null)
			return;
		
		int weaponID = -1;
		int costumeBackID = -1;
		int costumeHeadID = -1;
		int costumeBodyID = -1;
		
		int index = 0;
		foreach(EquipInfo info in equipItems)
		{
			if (equipItemPage != null)
				equipItemPage.SetItem(index, info.item);
			
			switch(info.slotType)
			{
			case GameDef.eSlotType.Weapon:
				if (info.item != null)
					weaponID = info.item.itemInfo.itemID;
				break;
			case GameDef.eSlotType.Costume_Body:
				if (info.item != null)
					costumeBodyID = info.item.itemInfo.itemID;
				break;
			case GameDef.eSlotType.Costume_Back:
				if (info.item != null)
					costumeBackID = info.item.itemInfo.itemID;
				break;
			case GameDef.eSlotType.Costume_Head:
				if (info.item != null)
					costumeHeadID = info.item.itemInfo.itemID;
				break;
			}
			
			++index;
		}
		
		if (equipItemPage != null && equipItemPage.costumeSetPanel != null)
			equipItemPage.costumeSetPanel.SetCostumeItem(costumeSetItem);
		
		if (costumeSetItem != null)
		{
			foreach(Item item in costumeSetItem.items)
			{
				if (item == null || item.itemInfo == null)
					continue;
				
				switch(item.itemInfo.itemType)
				{
				case ItemInfo.eItemType.Costume_Back:
					costumeBackID = item.itemInfo.itemID;
					break;
				case ItemInfo.eItemType.Costume_Body:
					costumeBodyID = item.itemInfo.itemID;
					break;
				case ItemInfo.eItemType.Costume_Head:
					costumeHeadID = item.itemInfo.itemID;
					break;
				}
			}
		}
		
		if (avatar != null)
		{
			avatar.ChangeCostume(costumeBodyID, costumeHeadID, costumeBackID);
			avatar.ChangeWeapon(weaponID);
		}
		
		PlayerController player = Game.Instance.player;
		if (player != null && player.lifeManager != null)
		{
			player.lifeManager.ChangeCostume(costumeBodyID, costumeHeadID, costumeBackID);
			
			player.lifeManager.ChangeWeapon(weaponID);
		}
	}
	
	public void ResetItems(ItemSlotPanel itemSlotPanel)
	{
		int slotCount = 0;
		if (itemSlotPanel != null)
			slotCount = itemSlotPanel.itemSlots.Count;
		
		for (int index = 0; index < slotCount; ++index)
		{
			itemSlotPanel.SetItem(index, null);
		}
		
		if (itemSlotPanel != null && itemSlotPanel.costumeSetPanel != null)
			itemSlotPanel.costumeSetPanel.SetCostumeItem(null);
	}
	
	public override void OnBack()
	{
		requestCount = 0;
		
		base.OnBack();
	}
	
	public int GetEmptyIndex(List<Item> list)
	{
		int index = -1;
		int nCount = list.Count;
		
		Item item = null;
		for (int i = 0; i < nCount; ++i)
		{
			item = list[i];
			if (item == null)
			{
				index = i;
				break;
			}
		}
		
		return index;
	}
	
	public void SetWindowItems(BaseItemScrollView window, List<Item> itemList)
	{
		int nCount = 0;
		if (itemList != null)
			nCount = itemList.Count;
		
		for (int index = 0; index < nCount; ++index)
		{
			Item item = itemList[index];
			window.SetItem(index, item);
		}
	}
	
	public void SetWindowItems(BaseItemScrollView window, List<Item> itemList, int maxCount)
	{
		int nCount = 0;
		if (itemList != null)
			nCount = itemList.Count;
		
		window.SetLockRestSlots(0, false);
		
		int index = 0;
		Item item = null;
		for (index = 0; index < nCount; ++index)
		{
			item = itemList[index];
			window.SetItem(index, item);
		}
		
		for (; index < maxCount; ++index)
		{
			item = null;
			window.SetItem(index, item);
		}
		
		window.SetLockRestSlots(index, true);
	}
	
	public void SetWindowItems(BaseItemScrollView window, List<CostumeSetItem> itemList)
	{
		int nCount = 0;
		if (itemList != null)
			nCount = itemList.Count;
		
		for (int index = 0; index < nCount; ++index)
		{
			CostumeSetItem item = itemList[index];
			window.SetItem(index, item);
		}
	}
	
	protected bool bInitWindow = false;
	public virtual void InitWindow()
	{
		if (tabViewControl == null)
			return;
		
		tabViewControl.InitDefaultTab();
		
		int charIndex = -1;
		List<EquipInfo> equipItems = null;
		CostumeSetItem costumeSetItem = null;
		
		CharInfoData charData = Game.Instance.charInfoData;
		if (Game.Instance.connector != null)
			charIndex = Game.Instance.connector.charIndex;
		
		CharPrivateData privateData = null;
		if (charData != null)
			privateData = charData.GetPrivateData(charIndex);
		
		if (privateData != null)
		{
			equipItems = privateData.equipData;
			costumeSetItem = privateData.costumeSetItem;
		}
		
		if (avatarCam != null)
		{
			avatarCam.playerClass = Game.Instance.playerClass;
			avatarCam.ChangeAvatar();
			
			avatar = avatarCam.avatar;
		}
		
		SetEquipItems(equipItems, costumeSetItem);
		
		UpdateCoinInfo();
		
		requestCount = 0;
	}
	
	public virtual void InitWindow(int tabIndex)
	{
		
	}

	public void UpdateInventoryWindow(bool scroll)
	{
		CharInfoData charInfoData = Game.Instance.charInfoData;
		int charIndex = -1;
		if (Game.Instance.connector != null)
			charIndex = Game.Instance.connector.charIndex;
		CharPrivateData privateData = charInfoData.GetPrivateData(charIndex);
		
		PlayerController player = Game.Instance.player;
		InventoryManager invenManager = null;
		
		if (player != null && player.lifeManager != null)
			invenManager = player.lifeManager.inventoryManager;
		
		UpdateCoinInfo();
	
		BaseItemScrollView invenWindow = null;
		BaseItemScrollView costumeWindow = null;
		if (tabViewControl != null)
		{
			invenWindow = tabViewControl.GetItemWindow(GameDef.eItemSlotWindow.Inventory);
			costumeWindow = tabViewControl.GetItemWindow(GameDef.eItemSlotWindow.Costume);
		}
	
		if (invenManager != null)
		{
			Game.RerangeItemList(charInfoData.inventoryNormalData);
			Game.RerangeItemList(charInfoData.inventoryCostumeData);
			
			this.selectedItemIndex = charInfoData.FindSlotIndex(selectedItem, privateData, selectedSlotWindow);
			
			invenManager.SetInvenItemData(charInfoData.inventoryNormalData);
			invenManager.SetInvenCostumeData(charInfoData.inventoryCostumeData);
			
			if (invenWindow != null)
				SetWindowItems(invenWindow, invenManager.itemList);
			
			if (costumeWindow != null)
				SetWindowItems(costumeWindow, invenManager.costumeList);
			
			BaseItemScrollView scrollView = null;
			switch(selectedSlotWindow)
			{
			case GameDef.eItemSlotWindow.Inventory:
				scrollView = invenWindow;
				break;
			case GameDef.eItemSlotWindow.Costume:
				scrollView = costumeWindow;
				break;
			}
			
			if (scroll && scrollView != null)
			{
				//int newPage = (this.selectedItemIndex / scrollView.itemPerPage) + 1;
				scrollView.ChangePage(this.selectedItemIndex);
			}
			
			OnSelectItem(selectedItem, selectedItemIndex, selectedSlotWindow);
		}
	}
	
	public void SetReinforceResult(Item reinforceItem, int slotIndex, GameDef.eItemSlotWindow slotWindow)
	{
		OnSelectItem(reinforceItem, slotIndex, slotWindow);
		
		PlayerController player = Game.Instance.player;
		LifeManager lifeManager = player != null ? player.lifeManager : null;
		
		UpdateCoinInfo(true);
		
		if (slotWindow == GameDef.eItemSlotWindow.Equip)
		{
			EquipManager equipManager = null;
			
			if (lifeManager != null)
				equipManager = lifeManager.equipManager;
			
			if (equipManager != null)
			{
				equipManager.SetEquipItem(slotIndex, reinforceItem);
				
				SetEquipItems(equipManager.equipItems, equipManager.costumeSetItem);
			}
		}
	}
	
	public virtual void OnCostumeTabActive(bool bActive)
	{
		
	}
	
	public override void OnErrorMessage(NetErrorCode errorCode, PopupBaseWindow popupBase)
	{
		requestCount--;
		
		base.OnErrorMessage(errorCode, popupBase);
		
		this.OnChildWindow(false);
	}
	
	public Item GetItem(GameDef.eItemSlotWindow windowType, int slotIndex)
	{
		Item item = null;
		switch(windowType)
		{
		case GameDef.eItemSlotWindow.Equip:
			item = equipItemPage.GetItem(slotIndex);
			break;
		case GameDef.eItemSlotWindow.Inventory:
		case GameDef.eItemSlotWindow.Costume:
			BaseItemScrollView window = tabViewControl.GetItemWindow(windowType);
			if (window != null)
				item = window.GetItem(slotIndex);
			break;
		}
		
		return item;
	}
}
