using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShopWindow : BaseItemWindow {
	
	public GameObject buyButton = null;
	public GameObject sellButton = null;
	
	public CharacterInfos charInfos = null;
	
	public override void Awake()
	{
		this.windowType = TownUI.eTOWN_UI_TYPE.SHOP;
		
		base.Awake();
		
		this.onChangeSelectedItem = new BaseItemWindow.OnChangeSelectedItem(ChangeSelectedItem);
		
		if (charInfos != null)
			charInfos.DeActivate();
		
		GameUI.Instance.shopWindow = this;
	}
	
	public void ChangeSelectedItem()
	{
		switch(selectedSlotWindow)
		{
		case GameDef.eItemSlotWindow.Equip:
			if (selectedItem != null)
			{
				SetActivateButton(buyButton.gameObject, false);
				SetActivateButton(sellButton.gameObject, true);
			}
			else
			{
				SetActivateButton(sellButton.gameObject, false);
				SetActivateButton(buyButton.gameObject, false);
			}
			
			if (equipItemInfoPage != null)
				equipItemInfoPage.OnToggle(false);
			break;
		case GameDef.eItemSlotWindow.Shop_Normal:
		case GameDef.eItemSlotWindow.Shop_Costume:
		case GameDef.eItemSlotWindow.Shop_CostumeSet:
		case GameDef.eItemSlotWindow.Shop_Cash:
			SetActivateButton(sellButton.gameObject, false);
			SetActivateButton(buyButton.gameObject, true);
			break;
		default:
			SetActivateButton(buyButton.gameObject, false);
			SetActivateButton(sellButton.gameObject, false);
			break;
		}
		
		UpdateSelectedSlotFrame();
		Debug.Log("ShopWindow ... " + selectedSlotWindow);
		
		switch(selectedSlotWindow)
		{
		case GameDef.eItemSlotWindow.Shop_Costume:
			ChangePreviewCostume(selectedItem);
			break;
		case GameDef.eItemSlotWindow.Shop_CostumeSet:
			ChangePreviewCostume(this.selectCostumeSetItem);
			break;
		}
	}
	
	public void ChangePreviewCostume(Item costumeItem)
	{
		if (avatarCam == null)
			return;
		
		int charIndex = -1;
		CharInfoData charData = Game.Instance.charInfoData;
		//CharPrivateData privateData = null;
		if (charData != null)
		{
			charIndex = Game.Instance.connector.charIndex;
			//privateData = charData.GetPrivateData(charIndex);
		}
		
		if (costumeItem != null && costumeItem.itemInfo != null)
		{
			GameDef.ePlayerClass curClass = avatarCam.playerClass;
			GameDef.ePlayerClass costumeClass = curClass;
			
			if (costumeItem.itemInfo.itemType == ItemInfo.eItemType.Costume_Body)
			{
				switch(costumeItem.itemInfo.limitClass)
				{
				case ItemInfo.eClass.Assassin:
					costumeClass = GameDef.ePlayerClass.CLASS_ASSASSIN;
					break;
				case ItemInfo.eClass.Warrior:
					costumeClass = GameDef.ePlayerClass.CLASS_WARRIOR;
					break;
				case ItemInfo.eClass.Wizard:
					costumeClass = GameDef.ePlayerClass.CLASS_WIZARD;
					break;
				}
			}
			
			if (avatarCam != null && avatarCam.playerClass != costumeClass)
			{
				avatarCam.ChangeAvatarByCostume(charIndex, costumeClass);
				
				avatar = avatarCam.avatar;
			}
			
			int bodyID = -1;//avatar.costumeBodyID;
			int headID = -1;//avatar.costumeHeadID;
			int backID = -1;//avatar.costumeBackID;
			switch(costumeItem.itemInfo.itemType)
			{
			case ItemInfo.eItemType.Costume_Body:
				bodyID = costumeItem.itemInfo.itemID;
				break;
			case ItemInfo.eItemType.Costume_Head:
				headID = costumeItem.itemInfo.itemID;
				break;
			case ItemInfo.eItemType.Costume_Back:
				backID = costumeItem.itemInfo.itemID;
				break;
			}
			
			avatar.ChangeCostume(bodyID, headID, backID);
		}
	}
	
	public void ChangePreviewCostume(CostumeSetItem setItem)
	{
		if (avatarCam == null)
			return;
		
		int charIndex = -1;
		CharInfoData charData = Game.Instance.charInfoData;
		//CharPrivateData privateData = null;
		if (charData != null)
		{
			charIndex = Game.Instance.connector.charIndex;
			//privateData = charData.GetPrivateData(charIndex);
		}
		
		if (setItem != null && setItem.setItemInfo != null)
		{
			GameDef.ePlayerClass curClass = avatarCam.playerClass;
			GameDef.ePlayerClass costumeClass = curClass;
			
			switch(setItem.setItemInfo.limitClass)
			{
			case ItemInfo.eClass.Assassin:
				costumeClass = GameDef.ePlayerClass.CLASS_ASSASSIN;
				break;
			case ItemInfo.eClass.Warrior:
				costumeClass = GameDef.ePlayerClass.CLASS_WARRIOR;
				break;
			case ItemInfo.eClass.Wizard:
				costumeClass = GameDef.ePlayerClass.CLASS_WIZARD;
				break;
			}
			
			if (avatarCam != null && avatarCam.playerClass != costumeClass)
			{
				avatarCam.ChangeAvatarByCostume(charIndex, costumeClass);
				
				avatar = avatarCam.avatar;
			}
			
			int bodyID = avatar.costumeBodyID;
			int headID = avatar.costumeHeadID;
			int backID = avatar.costumeBackID;
			foreach(Item item in setItem.items)
			{
				if (item == null)
					continue;
				
				switch(item.itemInfo.itemType)
				{
				case ItemInfo.eItemType.Costume_Body:
					bodyID = item.itemInfo.itemID;
					break;
				case ItemInfo.eItemType.Costume_Head:
					headID = item.itemInfo.itemID;
					break;
				case ItemInfo.eItemType.Costume_Back:
					backID = item.itemInfo.itemID;
					break;
				}
			}
			
			avatar.ChangeCostume(bodyID, headID, backID);
		}
	}
	
	public override void InitWindow(int tabIndex)
	{
		CloseCharInfos();
		
		//GameDef.ePlayerClass _class = Game.Instance.playerClass;
		CharInfoData charInfoData = Game.Instance.charInfoData;
		//CharPrivateData privateData = Game.Instance.GetCharPrivateData(_class);
		
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
		GameDef.ePlayerClass playerClass = GameDef.ePlayerClass.CLASS_WARRIOR;
		if (player != null && player.lifeManager != null)
		{
			playerClass = player.classType;
			
			invenManager = player.lifeManager.inventoryManager;
			//equipManager = player.lifeManager.equipManager;
		}
		
		if (invenManager != null)
		{
			invenManager.SetInvenItemData(charInfoData.inventoryNormalData);
		}
		
		base.InitWindow();
		
		BaseItemScrollView shopArtifactWindow = null;
		if (tabViewControl != null)
			shopArtifactWindow = tabViewControl.GetItemWindow(GameDef.eItemSlotWindow.Shop_Cash);
		
		if (shopArtifactWindow != null)
		{
			TableManager tableManager = TableManager.Instance;
			ShopTable shopNormalTable = tableManager.shopArtifactTable;
			StringValueTable stringValueTable = tableManager != null ? tableManager.stringValueTable : null;
			
			int maxGrade = stringValueTable != null ? stringValueTable.GetData("CompositionMax") : 4;
			int maxReinforce = stringValueTable != null ? stringValueTable.GetData("ReinforceMax") : 30;
		
			//유물 아이템 캐릭터 클래스별 필터링..
			int charIndex = -1;
			if (Game.Instance.Connector != null)
				charIndex = Game.Instance.Connector.charIndex;
			
			List<Item> shopNormalItemList = new List<Item>();
			if (shopNormalTable != null)
			{
				foreach(var temp in shopNormalTable.dataList)
				{
					ShopInfo info = temp.Value;
					
					if (info == null)
						continue;
					
					Item newItem = Item.CreateItem(info.itemID, "", maxGrade, maxReinforce, 1, info.itemRate, 0);
					if (newItem != null && newItem.itemInfo != null && newItem.CheckLimitClass(charIndex) == true )
					{
						uint itemMaxExp = newItem.GetItemMaxExp();
						newItem.SetExp(itemMaxExp);
						
						shopNormalItemList.Add(newItem);
					}
				}
				
				SetWindowItems(shopArtifactWindow, shopNormalItemList);
				
				//if (bInitWindow == false)
					shopArtifactWindow.Invoke("InitPage", 0.1f);
			}
		}
		
		BaseItemScrollView shopNormalWindow = null;
		if (tabViewControl != null)
			shopNormalWindow = tabViewControl.GetItemWindow(GameDef.eItemSlotWindow.Shop_Normal);
		
		if (shopNormalWindow != null)
		{
			TableManager tableManager = TableManager.Instance;
			ShopTable shopNormalTable = tableManager.shopNormalTable;
			
			List<Item> shopNormalItemList = new List<Item>();
			if (shopNormalTable != null)
			{
				foreach(var temp in shopNormalTable.dataList)
				{
					ShopInfo info = temp.Value;
					
					if (info == null)
						continue;
					
					Item newItem = Item.CreateItem(info.itemID, "", 0, 0, 1, info.itemRate, 0);
					if (newItem != null && newItem.itemInfo != null)
						shopNormalItemList.Add(newItem);
				}
				
				SetWindowItems(shopNormalWindow, shopNormalItemList);
				
				//if (bInitWindow == false)
					shopNormalWindow.Invoke("InitPage", 0.1f);
			}
		}
		
		BaseItemScrollView shopCostumeWindow = null;
		if (tabViewControl != null)
			shopCostumeWindow = tabViewControl.GetItemWindow(GameDef.eItemSlotWindow.Shop_Costume);
		
		if (shopCostumeWindow != null)
		{
			TableManager tableManager = TableManager.Instance;
			ShopTable shopCostumeTable = tableManager.shopCostumeTable;
			
			List<Item> shopCostumeItemList = new List<Item>();
			if (shopCostumeTable != null)
			{
				foreach(var temp in shopCostumeTable.dataList)
				{
					ShopInfo info = temp.Value;
					if (info == null)
						continue;
					
					Item newItem = Item.CreateItem(info.itemID, "", 0, 0, 1, info.itemRate, 0);
					if (newItem != null && newItem.itemInfo != null)
						shopCostumeItemList.Add(newItem);
				}
				
				SetWindowItems(shopCostumeWindow, shopCostumeItemList);
				
				//if (bInitWindow == false)
					shopCostumeWindow.Invoke("InitPage", 0.1f);
			}
		}
		
		BaseItemScrollView shopCostumeSetWindow = null;
		if (tabViewControl != null)
			shopCostumeSetWindow = tabViewControl.GetItemWindow(GameDef.eItemSlotWindow.Shop_CostumeSet);
		
		if (shopCostumeSetWindow != null)
		{
			TableManager tableManager = TableManager.Instance;
			ShopCostumeSetTable shopCostumeSetTable = tableManager.shopCostumeSetTable;
			
			List<CostumeSetItem> shopCostumeSetItemList = new List<CostumeSetItem>();
			if (shopCostumeSetTable != null)
			{
				foreach(var temp in shopCostumeSetTable.dataList)
				{
					int itemID = temp.Value;
					CostumeSetItem newItem = null;
					if (itemID > 0)
						newItem = CostumeSetItem.Create(itemID, "");
					
					if (newItem == null)
						continue;
					
					if (Game.CheckClass(newItem.setItemInfo.limitClass, playerClass) == true)
						shopCostumeSetItemList.Add(newItem);
					
				}
				
				SetWindowItems(shopCostumeSetWindow, shopCostumeSetItemList);
				
				//shopCostumeSetWindow.Invoke("InitPage", 0.1f);
			}
		}
		
		if (tabViewControl != null)
			tabViewControl.InitByTabIndex(tabIndex);
		
		OnSelectItem(null);
		
		bInitWindow = true;
	}
	
	public void OnBuyItem(GameObject obj)
	{
		if (requestCount > 0)
			return;
		
		Vector3 itemPrice = Vector3.zero;
		if (selectedItem != null)
		{
			itemPrice = selectedItem.itemInfo.buyPrice;
		
			CashItemType cashCheck = CheckNeedGold(itemPrice);
			if (cashCheck != CashItemType.None)
			{
				OnNeedMoneyPopup(cashCheck, this);
				return;
			}
			
			OnBuyConfirmPopup(selectedItem);
		}
		else if (selectCostumeSetItem != null)
		{
			itemPrice = selectCostumeSetItem.setItemInfo.buyPrice;
		
			OnBuyConfirmPopup(selectCostumeSetItem);
		}
	}
	
	public string buyConfirmPopupPrefabPath = "UI/Item/BuyConfirmPopup";
	public string buyCountConfirmPopupPrefab = "UI/Item/BuyCountConfirmPopup";
	public void OnBuyConfirmPopup(CostumeSetItem item)
	{
		BaseConfirmPopup buyConfirmPopup = ResourceManager.CreatePrefab<BaseConfirmPopup>(buyConfirmPopupPrefabPath, popupNode, Vector3.zero);
		if (buyConfirmPopup != null)
		{
			buyConfirmPopup.cancelButtonMessage.target = this.gameObject;
			buyConfirmPopup.cancelButtonMessage.functionName = "OnConfirmPopupCancel";
			
			buyConfirmPopup.okButtonMessage.target = this.gameObject;
			buyConfirmPopup.okButtonMessage.functionName = "OnBuyItemOK";
			
			buyConfirmPopup.SetGoldInfo(item, true);
			
			popupList.Add(buyConfirmPopup);
		}
	}
	
	public void OnBuyConfirmPopup(Item item)
	{
		int maxStackCount = 1;
		if (item != null)
		{
			if (item.itemInfo != null)
				maxStackCount = item.itemInfo.maxStackCount;
		}
		
		if (maxStackCount == 1)
		{
			BaseConfirmPopup buyConfirmPopup = ResourceManager.CreatePrefab<BaseConfirmPopup>(buyConfirmPopupPrefabPath, popupNode, Vector3.zero);
			if (buyConfirmPopup != null)
			{
				buyConfirmPopup.cancelButtonMessage.target = this.gameObject;
				buyConfirmPopup.cancelButtonMessage.functionName = "OnConfirmPopupCancel";
				
				buyConfirmPopup.okButtonMessage.target = this.gameObject;
				buyConfirmPopup.okButtonMessage.functionName = "OnBuyItemOK";
				
				buyConfirmPopup.SetGoldInfo(item, true);
				
				popupList.Add(buyConfirmPopup);
			}
		}
		else
		{
			ItemCountConfirmPopup buyCountConfirmPopup = ResourceManager.CreatePrefab<ItemCountConfirmPopup>(buyCountConfirmPopupPrefab, popupNode, Vector3.zero);
			if (buyCountConfirmPopup != null)
			{
				buyCountConfirmPopup.cancelButtonMessage.target = this.gameObject;
				buyCountConfirmPopup.cancelButtonMessage.functionName = "OnConfirmPopupCancel";
				
				buyCountConfirmPopup.okButtonMessage.target = this.gameObject;
				buyCountConfirmPopup.okButtonMessage.functionName = "OnBuyItemOK";
				
				CharInfoData charData = Game.Instance.charInfoData;
				int ownGold = 0;
				int ownJewel = 0;
				int ownMedal = 0;
				
				if (charData != null)
				{
					ownGold = charData.gold_Value;
					ownJewel = charData.jewel_Value;
					ownMedal = charData.medal_Value;
				}
				
				buyCountConfirmPopup.SetBuyInfo(item, ownGold, ownJewel, ownMedal);
				
				popupList.Add(buyCountConfirmPopup);
			}
		}
	}
	
	public void OnConfirmPopupCancel(GameObject obj)
	{
		ClosePopup();
	}
	
	public void OnBuyItemOK(GameObject obj)
	{
		switch(selectedSlotWindow)
		{
		case GameDef.eItemSlotWindow.Shop_Normal:
			OnBuyNormalItem();
			break;
		case GameDef.eItemSlotWindow.Shop_Costume:
			OnBuyCostumeItem();
			break;
		case GameDef.eItemSlotWindow.Shop_CostumeSet:
			OnBuyCostumeSetItem();
			break;
		case GameDef.eItemSlotWindow.Shop_Cash:
			OnBuyArtifactItem();
			break;
		}
		
		ClosePopup();
	}
	
	public void OnBuyNormalItem()
	{
		if (selectedItem != null && selectedItem.itemInfo != null)
		{
			bool bCanBuy = true;
			switch(selectedItem.itemInfo.itemType)
			{
			case ItemInfo.eItemType.Costume_Body:
			case ItemInfo.eItemType.Costume_Head:
			case ItemInfo.eItemType.Costume_Back:
				bCanBuy = false;
				break;
			}
			
			if (bCanBuy == false)
				return;
			
			ItemCountConfirmPopup buyCountConfirmPopup = GetPopup<ItemCountConfirmPopup>();
			int buyCount = 1;
			if (buyCountConfirmPopup != null)
				buyCount = buyCountConfirmPopup.BuyCount;
			
			IPacketSender packetSender = Game.Instance.packetSender;
			if (packetSender != null)
			{
				packetSender.SendRequestBuyNormalItem(selectedItem.itemInfo.itemID, buyCount, selectedItemIndex, selectedSlotWindow);
				
				requestCount++;
			}
		}
		
		//OnSelectItem(null);
	}
	
	public void OnBuyArtifactItem()
	{
		if (selectedItem != null && selectedItem.itemInfo != null)
		{
			bool bCanBuy = true;
			switch(selectedItem.itemInfo.itemType)
			{
			case ItemInfo.eItemType.Costume_Body:
			case ItemInfo.eItemType.Costume_Head:
			case ItemInfo.eItemType.Costume_Back:
				bCanBuy = false;
				break;
			}
			
			if (bCanBuy == false)
				return;
			
			ItemCountConfirmPopup buyCountConfirmPopup = GetPopup<ItemCountConfirmPopup>();
			int buyCount = 1;
			if (buyCountConfirmPopup != null)
				buyCount = buyCountConfirmPopup.BuyCount;
			
			IPacketSender packetSender = Game.Instance.packetSender;
			if (packetSender != null)
			{
				packetSender.SendRequestBuyArtifactItem(selectedItem.itemInfo.itemID, buyCount, selectedItemIndex, selectedSlotWindow);
				
				requestCount++;
			}
		}
		
		//OnSelectItem(null);
	}
	
	public void OnBuyNormalItemResult(int slotIndex, NetErrorCode errorCode)
	{
		requestCount--;
		
		if (errorCode != NetErrorCode.OK)
		{
			OnErrorMessage(errorCode, this);
			return;
		}
		
		UpdateCoinInfo(true);

		//OnSelectItem(null);
	}
	
	public void OnBuyCostumeItem()
	{
		if (selectedItem != null && selectedItem.itemInfo != null)
		{
			bool bCanBuy = false;
			switch(selectedItem.itemInfo.itemType)
			{
			case ItemInfo.eItemType.Costume_Body:
			case ItemInfo.eItemType.Costume_Head:
			case ItemInfo.eItemType.Costume_Back:
				bCanBuy = true;
				break;
			}
			
			if (bCanBuy == false)
				return;
			
			IPacketSender packetSender = Game.Instance.packetSender;
			if (packetSender != null)
			{
				packetSender.SendRequestBuyCostumeItem(selectedItem.itemInfo.itemID, selectedItemIndex);
				
				requestCount++;
			}
		}
		
		//OnSelectItem(null);
	}
	
	public void OnBuyCostumeItemResult(int slotIndex, NetErrorCode errorCode)
	{
		requestCount--;
		
		if (errorCode != NetErrorCode.OK)
		{
			OnErrorMessage(errorCode, this);
			return;
		}
		
		UpdateCoinInfo(true);

		OnSelectItem(null);
	}
	
	public void OnBuyCostumeSetItem()
	{
		if (selectCostumeSetItem != null && selectCostumeSetItem.setItemInfo != null)
		{
			IPacketSender packetSender = Game.Instance.packetSender;
			if (packetSender != null)
			{
				packetSender.SendRequestBuyCostumeSetItem(selectCostumeSetItem.setItemInfo.setID, selectedItemIndex);
				
				requestCount++;
			}
		}
	}
	
	public void OnBuyCostumeSetItemResult(int slotIndex, NetErrorCode errorCode)
	{
		requestCount--;
		
		if (errorCode != NetErrorCode.OK)
		{
			OnErrorMessage(errorCode, this);
			return;
		}
		
		UpdateCoinInfo(true);

		OnSelectItem(null);
	}
	
	public void OnSellItem(GameObject obj)
	{
		if (requestCount > 0)
			return;
		
		OnSellConfirmPopup(selectedItem);
	}
	
	public void OnSellItemOK(GameObject obj)
	{
		ClosePopup();
		
		switch(selectedSlotWindow)
		{
		case GameDef.eItemSlotWindow.Equip:
			OnSellItemFromEquip();
			break;
		case GameDef.eItemSlotWindow.Costume:
			OnSellItemFromCostume();
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
	
	
	public void OnSellItemFromCostume()
	{
		if (selectedItem != null && selectedItem.itemInfo != null)
		{
			
			IPacketSender packetSender = Game.Instance.packetSender;
			if (packetSender != null)
			{
				 // todo.semo
				int itemID = selectedItem.itemInfo.itemID;
				string uID = selectedItem.uID;
				packetSender.SendRequestSellCostumeItem(packetSender.Connector.charIndex, selectedItemIndex, itemID, uID);
				
				requestCount++;
			}
		}
		
		//OnSelectItem(null);
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
		EquipManager equipManager = lifeManager != null ? lifeManager.equipManager : null;
		
		if (equipManager != null)
			SetEquipItems(equipManager.equipItems, equipManager.costumeSetItem);
		
		UpdateCoinInfo(true);

		OnSelectItem(null);
	}
	
	public void OnSellItemFromEquip()
	{
		if (selectedItem != null && selectedItem.itemInfo != null)
		{
			IPacketSender packetSender = Game.Instance.packetSender;
			if (packetSender != null)
			{
				// todo.semo
				int itemID = selectedItem.itemInfo.itemID;
				string uID = selectedItem.uID;
				packetSender.SendRequestSellEquipItem(packetSender.Connector.charIndex, selectedItemIndex, itemID, uID);
				
				requestCount++;
			}
		}
		
		//OnSelectItem(null);
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
		CloseCharInfos();
	}
	
	public override void OnCostumeTabActive(bool bActive)
	{
		if (equipItemPage != null)
		{
			equipItemPage.ActiveCostumeSlot(bActive);
		}
		
		//if (bActive == false)
		{
			DefaultPreviewCostume();
		}
	}
	
	public void DefaultPreviewCostume()
	{
		if (avatarCam == null)
			return;
		
		int charIndex = -1;
		CharInfoData charData = Game.Instance.charInfoData;
		//CharPrivateData privateData = null;
		if (charData != null)
		{
			charIndex = Game.Instance.connector.charIndex;
			//privateData = charData.GetPrivateData(charIndex);
		}
		
		GameDef.ePlayerClass origClass = Game.Instance.playerClass;
		if (avatarCam.playerClass != origClass)
		{
			avatarCam.ChangeAvatarByCostume(charIndex, origClass);
			
			avatar = avatarCam.avatar;
		}
		else
		{
			avatarCam.ResetDefaultCostume(charIndex, origClass);
		}
	}
	
	public void OpenCharInfos(GameObject obj)
	{
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
}
