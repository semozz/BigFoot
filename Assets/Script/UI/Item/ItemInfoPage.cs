using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemInfoPage : MonoBehaviour {
	
	public UILabel itemGradeLabel = null;
	public UILabel itemName = null;
	public UILabel itemInfos = null;
	public UILabel itemDesc = null;
	
	public UILabel classLimit = null;
	public UISprite goldType = null;
	public UILabel priceLabel = null;
	public UILabel levelLimit = null;
	
	public UILabel setItemInfos = null;
	public int setLimitTitleStringID = 144;
	
	public Item item = null;
	
	public BaseItemWindow parent = null;
	
	public Color itemRate1Color = Color.white;
	public Color itemRate2Color = Color.green;
	public Color itemRate3Color = Color.blue;
	public Color itemRate4Color = Color.magenta;
	public Color itemRate5Color = Color.yellow;
	
	public int itemRate1StringID = 169;
	public int itemRate2StringID = 170;
	public int itemRate3StringID = 171;
	public int itemRate4StringID = 172;
	public int itemRate5StringID = 173;
	
	public int itemGradeStringID = 1000;
	
	public Color setItemDefaultColor = Color.green;
	public Color setItemDisableColor = Color.gray;
	
	
	public PlayerController player = null;
	
	public bool bShowSetItemInfo = true;
	
	public ItemSlot itemSlot = null;
	public string itemSlotPrefab = "";
	public Transform itemSlotNode = null;
	
	public string costumeItemSlotPrefab = "UI/Item/ItemSlot";
	public List<ItemSlotInfo> costumeSetItemSlotInfos = new List<ItemSlotInfo>();
	public List<ItemSlot> costumeSetItems = new List<ItemSlot>();
	public List<UILabel> costumeSetItemInfoLables = new List<UILabel>();
	
	public Transform costumeSetItemSlotNode = null;
	public GameObject costumeInfoRoot = null;
	
	public Color itemInfoMinusColor = Color.red;
	public Color itemInfoPlusColor = Color.green;
	
	protected bool isStarted = false;
	void Start()
	{
		if (isStarted == true)
			return;
		
		if (itemSlotPrefab != "")
		{
			itemSlot = ResourceManager.CreatePrefab<ItemSlot>(itemSlotPrefab, itemSlotNode, Vector3.zero);
			if (itemSlot != null)
			{
				itemSlot.slotWindowType = GameDef.eItemSlotWindow.Gamble;
				
				itemSlot.SetItem(null);
			}
		}
		
		if (costumeSetItemSlotNode != null)
		{
			ItemSlot costumeSetItemSlot = null;
			foreach(ItemSlotInfo info in costumeSetItemSlotInfos)
			{
				costumeSetItemSlot = ResourceManager.CreatePrefab<ItemSlot>(costumeItemSlotPrefab, costumeSetItemSlotNode, info.trans.localPosition);
				
				costumeSetItemSlot.slotWindowType = GameDef.eItemSlotWindow.Equip;
				costumeSetItemSlot.SetItem(null);
				
				costumeSetItems.Add(costumeSetItemSlot);
			}
			
			//costumeSetItemSlotNode.gameObject.SetActive(false);
		}
		
		if (costumeInfoRoot != null)
			costumeInfoRoot.SetActive(false);
		
		isStarted = true;
	}
	
	public void SetCostumeSetItem(CostumeSetItem costumeSet)
	{
		if (isStarted == false)
			Start();
		
		if (costumeInfoRoot != null)
			costumeInfoRoot.SetActive(true);
		
		if (itemSlot != null)
			itemSlot.gameObject.SetActive(false);
		
		if (costumeSet != null)
		{
			if (itemInfos != null)
				itemInfos.text = "";
			if (itemDesc != null)
				itemDesc.text = "";
			
			int nCount = costumeSetItems.Count;
			for (int index = 0; index < nCount; ++index)
			{
				ItemSlot slot = costumeSetItems[index];
				Item item = costumeSet.GetItemByIndex(index);
				
				if (slot != null)
					slot.SetItem(item);
				
				SetCostumeSetItemInfo(index, item);
			}
			
			string itemName = costumeSet.setItemInfo.setName;
			SetItemName(-1, -1, itemName);
			
			ItemInfo.eClass limitClass = costumeSet.setItemInfo.limitClass;
			SetItemLimitClass(limitClass);
			
			Vector3 buyPrice = costumeSet.setItemInfo.buyPrice;
			Vector3 sellPrice = costumeSet.setItemInfo.sellPrice;
			SetPriceInfo(buyPrice, sellPrice);
			
		}
		else
		{
			foreach(ItemSlot slot in costumeSetItems)
				slot.SetItem(null);
		}
	}
	
	public void SetCostumeSetItemInfo(int index, Item item)
	{
		int nCount = this.costumeSetItemInfoLables.Count;
		UILabel label = null;
		if (index >= 0 && index < nCount)
			label = this.costumeSetItemInfoLables[index];
		
		if (label != null && item != null)
			label.text = item.GetAttributeInfos();
	}
	
	public void SetItemName(int itemGrade, int itemRateStep, string nameStr)
	{
		if (itemName != null)
			itemName.text = GetItemName(itemRateStep, nameStr);
		SetItemGradeLabel(itemGrade, itemRateStep, nameStr);
	}
	
	public string GetItemName(int itemRateStep, string nameStr)
	{
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		string itemNameStr = "";
		int itemNamePostfixStringID = -1;
		string postfixString = "";
		
		Color itemRateColor = itemRate1Color;
		switch(itemRateStep)
		{
		case 0:
			itemRateColor = itemRate1Color;
			itemNamePostfixStringID = itemRate1StringID;
			break;
		case 1:
			itemRateColor = itemRate2Color;
			itemNamePostfixStringID = itemRate2StringID;
			break;
		case 2:
			itemRateColor = itemRate3Color;
			itemNamePostfixStringID = itemRate3StringID;
			break;
		case 3:
			itemRateColor = itemRate4Color;
			itemNamePostfixStringID = itemRate4StringID;
			break;
		case 4:
			itemRateColor = itemRate5Color;
			itemNamePostfixStringID = itemRate5StringID;
			break;
		}
		
		if (stringTable != null && itemNamePostfixStringID != -1)
			postfixString = stringTable.GetData(itemNamePostfixStringID);
		
		if (nameStr != "")
		{
			if (postfixString != "")
				itemNameStr = string.Format("{0}{1}({2})[-]", GameDef.RGBToHex(itemRateColor), nameStr, postfixString);
			else
				itemNameStr = string.Format("{0}{1}[-]", GameDef.RGBToHex(itemRateColor), nameStr);
		}
		else
			itemNameStr = nameStr;
		
		return itemNameStr;
	}
	
	public void SetItemGradeLabel(int itemGrade, int itemRateStep, string nameStr)
	{		
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		string prefixString = "";
		int itemNamePostfixStringID = -1;
		int itemNamePrefixStringID = -1;
		
		if (itemRateStep >= 0 && itemGrade >= 0)
			itemNamePrefixStringID = itemGradeStringID + itemGrade;
		switch(itemRateStep)
		{
		case 0:
			itemNamePostfixStringID = itemRate1StringID;
			break;
		case 1:
			itemNamePostfixStringID = itemRate2StringID;
			break;
		case 2:
			itemNamePostfixStringID = itemRate3StringID;
			break;
		case 3:
			itemNamePostfixStringID = itemRate4StringID;
			break;
		case 4:
			itemNamePostfixStringID = itemRate5StringID;
			break;
		}
		
		if (nameStr != "" && stringTable != null && itemNamePrefixStringID != -1)
			prefixString = stringTable.GetData(itemNamePrefixStringID);
		
		if (itemGradeLabel != null)
			itemGradeLabel.text = prefixString;
	}
	
	public void SetItemLimitClass(ItemInfo.eClass limitClass)
	{
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		if (classLimit != null)
		{
			bool bCheckClass = true;
			GameDef.ePlayerClass playerClass = Game.Instance.playerClass;
			
			if (limitClass == ItemInfo.eClass.Common)
			{
				bCheckClass = true;
			}
			else
			{
				if ((int)playerClass != (int)limitClass)
					bCheckClass = false;
				else
					bCheckClass = true;
			}
			
			int limitClassStringID = -1;
			switch(limitClass)
			{
			case ItemInfo.eClass.Warrior:
				limitClassStringID = 1;
				break;
			case ItemInfo.eClass.Assassin:
				limitClassStringID = 2;
				break;
			case ItemInfo.eClass.Wizard:
				limitClassStringID = 3;
				break;
			default:
				limitClassStringID = 4;
				break;
			}
			
			classLimit.text = stringTable != null ? stringTable.GetData(limitClassStringID) : "";
			
			if (bCheckClass == false)
				classLimit.color = Color.red;
			else
				classLimit.color = Color.white;
		}
	}
	
	public void SetPriceInfo(Vector3 buyPrice, Vector3 sellPrice)
	{
		Vector3 itemPrice = Vector3.zero;
		if (priceLabel != null)
		{
			if (parent != null)
			{
				GameDef.eItemSlotWindow slotWindowType = parent.selectedSlotWindow;
				switch(slotWindowType)
				{
				case GameDef.eItemSlotWindow.Shop_Normal:
				case GameDef.eItemSlotWindow.Shop_Costume:
				case GameDef.eItemSlotWindow.Shop_Cash:
				case GameDef.eItemSlotWindow.Shop_CostumeSet:
					itemPrice = buyPrice;
					break;
				case GameDef.eItemSlotWindow.Equip:
				case GameDef.eItemSlotWindow.Inventory:
				case GameDef.eItemSlotWindow.MaterialItem:
				case GameDef.eItemSlotWindow.Costume:
				case GameDef.eItemSlotWindow.CostumeSet:
					itemPrice = sellPrice;
					break;
				default:
					break;
				}
			}
			
			float price = 0.0f;
			if (itemPrice.x > 0.0f)
				price = itemPrice.x;
			else if (itemPrice.y > 0.0f)
				price = itemPrice.y;
			else if (itemPrice.z > 0.0)
				price = itemPrice.z;
			
			priceLabel.text = string.Format("{0:#,###,###}", price);
		}
		
		if (goldType != null)
		{
			if (itemPrice == Vector3.zero)
				goldType.gameObject.SetActive(false);
			else
			{
				goldType.gameObject.SetActive(true);
				
				string spriteName = "Shop_Money01";
				if (itemPrice.x > 0.0f)
				{
					spriteName = "Shop_Money01";
				}
				else if (itemPrice.y > 0.0f)
				{
					spriteName = "Shop_Money02";
				}
				else if (itemPrice.z > 0.0)
				{
					spriteName = "Shop_Money03";
				}
				goldType.spriteName = spriteName;
			}
		}
	}
	
	public void SetLimitLevel(int limitLevel)
	{
		if (levelLimit != null)
		{
			int charLevel = 0;
			if (player != null)
			{
				charLevel = player.lifeManager.charLevel;
			}
			else
			{
				PlayerController myPlayer = Game.Instance.player;
				if (myPlayer != null)
				{
					charLevel = myPlayer.lifeManager.charLevel;
				}
			}
			
			Color limitLevelColor = Color.white;
			
			string infoStr = "";
			if (limitLevel > 0)
			{
				if (limitLevel > charLevel)
					limitLevelColor = Color.red;
				
				infoStr = string.Format("Lv.{0}", limitLevel);
			}
			else
				infoStr = "";
			
			levelLimit.text = infoStr;
			levelLimit.color = limitLevelColor;
		}
	}
	
	public void SetItem(Item item)
	{
		if (isStarted == false)
			Start();
		
		if (costumeInfoRoot != null)
			costumeInfoRoot.SetActive(false);
		
		if (itemSlot != null)
		{
			itemSlot.gameObject.SetActive(item != null);
			itemSlot.SetItem(item);
		}
		
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		ArenaItemRateTable arenaItemRateTable = tableManager != null ? tableManager.arenaItemRateTable : null;
		
		SetItemManager setItemManager = null;
		
		int charLevel = 1;
		float arenaItemRate = 1.0f;
		
		if (player != null)
		{
			setItemManager = player.lifeManager.equipManager.setItemManager;
			
			charLevel = player.lifeManager.charLevel;
		}
		else
		{
			PlayerController myPlayer = Game.Instance.player;
			if (myPlayer != null)
			{
				setItemManager = myPlayer.lifeManager.equipManager.setItemManager;
				
				charLevel = myPlayer.lifeManager.charLevel;
			}
		}
		
		arenaItemRate = arenaItemRateTable != null ? arenaItemRateTable.GetItemRate(charLevel) : 1.0f;
		
		this.item = item;
		
		int itemRateStep = 0;
		int itemGrade = 0;
		string itemNameStr = "";
		if (item != null && item.itemInfo != null)
		{
			switch(item.itemInfo.itemType)
			{
			case ItemInfo.eItemType.Potion_1:
			case ItemInfo.eItemType.Potion_2:
				itemNameStr = item.itemInfo.itemName;
				itemGrade = -1;
				itemRateStep = -1;
				break;
			default:
				itemGrade = (int)item.itemGrade;
				itemRateStep = item.itemRateStep;
				itemNameStr = item.itemInfo.itemName;
				break;
			}
		}
		SetItemName(itemGrade, itemRateStep, itemNameStr);

		
		if (itemInfos != null)
		{
			string itemInfoStr = "";
			
			if (item != null)
			{
				bool isArenaItem = item.CheckArenaItem();
				if (isArenaItem == true)
					item.SetArenaItemRate(arenaItemRate);
				
				itemInfoStr = item.GetAttributeInfo(this.itemInfoPlusColor, this.itemInfoMinusColor);
				
				if (isArenaItem == true)
					item.ResetArenaItemRate();
			}
			
			itemInfos.text = itemInfoStr;
		}
		
		if (itemDesc != null)
		{
			if (item != null && item.itemInfo != null)
				itemDesc.text = item.itemInfo.strDesc;
			else
				itemDesc.text = "";
		}
		
		if (classLimit != null)
		{
			if (item == null || item.itemInfo == null)
				classLimit.gameObject.SetActive(false);
			else
				classLimit.gameObject.SetActive(true);
			
			if (item != null && item.itemInfo != null)
			{
				SetItemLimitClass(item.itemInfo.limitClass);
			}
		}
		
		Vector3 buyPrice = Vector3.zero;
		Vector3 sellPrice = Vector3.zero;
		if (item != null && item.itemInfo != null)
		{
			buyPrice = item.itemInfo.buyPrice;
			sellPrice = item.GetSellPrice();
		}
		SetPriceInfo(buyPrice, sellPrice);
		
		int limitLevel = 0;
		if (item != null && item.itemInfo != null)
			limitLevel = item.itemInfo.equipLevel;
		SetLimitLevel(limitLevel);
		
		string setItemInfoStr = "";
		if (this.item != null && this.item.setItemID != -1)
		{
			SetItemInfo setItemInfo = setItemManager != null ? setItemManager.GetSetItemInfo(this.item.setItemID) : null;
			if (setItemInfo != null)
			{
				List<string> setInfoStrs = new List<string>();
				
				string setName = string.Format("{0}{1}[-]", GameDef.RGBToHex(setItemDefaultColor), setItemInfo.setItemName);
				
				setInfoStrs.Add(setName);
				
				if (bShowSetItemInfo == true)
				{
					Color setColor = setItemDefaultColor;
					foreach(SetAttributeInfo setInfo in setItemInfo.setAttributeList)
					{
						string setStepInfo = "";
						
						if (setInfo.isAcive)
							setColor = setItemDefaultColor;
						else
							setColor = setItemDisableColor;
						
						string setLimitTitle = string.Format("{0}{1}", setInfo.limitCount, stringTable.GetData(setLimitTitleStringID));
						string setAttributeStr = "";
						foreach(AttributeValue _value in setInfo.attributeList)
						{
							if (setAttributeStr != "")
								setAttributeStr += string.Format(", {1}", _value.GetInfoStr());
							else
								setAttributeStr = _value.GetInfoStr();
						}
						
						setStepInfo = string.Format("{0}{1} : {2}[-]", GameDef.RGBToHex(setColor), setLimitTitle, setAttributeStr);
						
						setInfoStrs.Add(setStepInfo);
					}
				}
				
				foreach(string infoStr in setInfoStrs)
				{
					if (setItemInfoStr == "")
						setItemInfoStr = infoStr;
					else
						setItemInfoStr += string.Format("\n{0}", infoStr);
				}
			}
		}
		if (setItemInfos != null)
			setItemInfos.text = setItemInfoStr;
	}
	
	public void OnToggle(bool bActive)
	{
		this.gameObject.SetActive(bActive);
	}
	
	public void DeActivate()
	{
		OnToggle(false);
	}
	
	public void Activate()
	{
		OnToggle(true);
	}
}
