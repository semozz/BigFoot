using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class EquipInfo
{
	public GameDef.eSlotType slotType = GameDef.eSlotType.Common;
	public Item item = null;
	
	public int slotIndex = -1;
	
	public void SetSlotType(int slotIndex)
	{
		switch(slotIndex)
		{
		case 0:
			slotType = GameDef.eSlotType.Head;
			break;
		case 1:
			slotType = GameDef.eSlotType.Armor;
			break;
		case 2:
			slotType = GameDef.eSlotType.Pants;
			break;
		case 3:
			slotType = GameDef.eSlotType.Boots;
			break;
		case 4:
			slotType = GameDef.eSlotType.Hand;
			break;
		case 5:
			slotType = GameDef.eSlotType.Ring;
			break;
		case 6:
			slotType = GameDef.eSlotType.Ring;
			break;
		case 7:
			slotType = GameDef.eSlotType.Accessories;
			break;
		case 8:
			slotType = GameDef.eSlotType.Weapon;
			break;
		case 9:
			slotType = GameDef.eSlotType.Costume_Body;
			break;
		case 10:
			slotType = GameDef.eSlotType.Costume_Back;
			break;
		case 11:
			slotType = GameDef.eSlotType.Costume_Head;
			break;	
		default:
			slotType = GameDef.eSlotType.Common;
			break;
		}
	}
	
	public static int SlotTypeToEquipIndex(CharPrivateData privateData, GameDef.eSlotType slotType)
	{
		int equipSlotIndex = -1;
		
		if (privateData == null)
		{
			switch(slotType)
			{
			case GameDef.eSlotType.Head:
				equipSlotIndex = 0;
				break;
			case GameDef.eSlotType.Hand:
				equipSlotIndex = 4;
				break;
			case GameDef.eSlotType.Pants:
				equipSlotIndex = 2;
				break;
			case GameDef.eSlotType.Boots:
				equipSlotIndex = 3;
				break;
			case GameDef.eSlotType.Armor:
				equipSlotIndex = 1;
				break;
			case GameDef.eSlotType.Weapon:
				equipSlotIndex = 8;
				break;
			case GameDef.eSlotType.Accessories:
				equipSlotIndex = 7;
				break;
			case GameDef.eSlotType.Ring:
				equipSlotIndex = 5;
				break;
			case GameDef.eSlotType.Costume_Body:
				equipSlotIndex = 9;
				break;
			case GameDef.eSlotType.Costume_Back:
				equipSlotIndex = 10;
				break;
			case GameDef.eSlotType.Costume_Head:
				equipSlotIndex = 11;
				break;
			default:
				equipSlotIndex = 12;
				break;
			}
		}
		else
		{
			List<int> equipInfoList = new List<int>();
			List<int> availableIndexList = new List<int>();
			
			int nCount = privateData.equipData.Count;
			for (int index = 0; index < nCount; ++index)
			{
				EquipInfo temp = privateData.equipData[index];
				if (temp != null && temp.slotType == slotType)
				{
					//빈 슬롯 인덱스 리스트..
					if (temp.item == null)
						availableIndexList.Add(index);
					
					//슬롯 타입이 같은 리스트..
					equipInfoList.Add(index);
				}
			}
			
			//빈 슬롯이 없으면 첫번째꺼..
			if (availableIndexList.Count == 0)
			{
				if (equipInfoList.Count > 0)
					equipSlotIndex = equipInfoList[0];
			}
			else
			{
				equipSlotIndex = availableIndexList[0];
			}
		}
		
		return equipSlotIndex;
	}
	
	public static GameDef.eSlotType ItemTypeToSlotType(ItemInfo.eItemType itemType)
	{
		GameDef.eSlotType slotType = GameDef.eSlotType.Common;
		switch(itemType)
		{
		case ItemInfo.eItemType.Head:
			slotType = GameDef.eSlotType.Head;
			break;
		case ItemInfo.eItemType.Hand:
			slotType = GameDef.eSlotType.Hand;
			break;
		case ItemInfo.eItemType.Pants:
			slotType = GameDef.eSlotType.Pants;
			break;
		case ItemInfo.eItemType.Boots:
			slotType = GameDef.eSlotType.Boots;
			break;
		case ItemInfo.eItemType.Armor:
			slotType = GameDef.eSlotType.Armor;
			break;
		case ItemInfo.eItemType.Weapon:
			slotType = GameDef.eSlotType.Weapon;
			break;
		case ItemInfo.eItemType.Accessories:
			slotType = GameDef.eSlotType.Accessories;
			break;
		case ItemInfo.eItemType.Ring:
			slotType = GameDef.eSlotType.Ring;
			break;
		case ItemInfo.eItemType.Costume_Body:
			slotType = GameDef.eSlotType.Costume_Body;
			break;
		case ItemInfo.eItemType.Costume_Back:
			slotType = GameDef.eSlotType.Costume_Back;
			break;
		case ItemInfo.eItemType.Costume_Head:
			slotType = GameDef.eSlotType.Costume_Head;
			break;
		default:
			slotType = GameDef.eSlotType.Common;
			break;
		}
		
		return slotType;
	}
	
	public static int ItemTypeToEquipSlotIndex(CharPrivateData privateData, ItemInfo.eItemType type)
	{
		GameDef.eSlotType slotType = ItemTypeToSlotType(type);
		int equipSlotIndex = SlotTypeToEquipIndex(privateData, slotType);
		return equipSlotIndex;
	}
}

public class EquipManager : MonoBehaviour
{
	public List<EquipInfo> applyEquipItems = new List<EquipInfo>();
	
	public List<EquipInfo> equipItems = new List<EquipInfo>();
	public CostumeSetItem costumeSetItem = null;
	
	public delegate void OnEquipChanged();
	public OnEquipChanged onChangeEquip = null;
	
	private LifeManager lifeManager = null;
	private PlayerController player = null;
	
	
	public SetItemManager setItemManager = null;
	
	void Awake()
	{
		lifeManager = this.gameObject.GetComponent<LifeManager>();
		player = this.gameObject.GetComponent<PlayerController>();
		
		setItemManager = new SetItemManager();
	}
	
	public void SetEquipItemData(List<EquipInfo> equipList, CostumeSetItem costumeSet)
	{
		this.equipItems.Clear();
		this.equipItems.AddRange(equipList);
		this.costumeSetItem = costumeSet;
		
		List<EquipInfo> newEquipList = CharPrivateData.GetEquipItemInfos(equipList, costumeSet);
		SetEquipItemData(newEquipList);
	}
	
	public void SetEquipItemData(List<EquipInfo> list)
	{
		bool isCostumeItem = false;
		
		int charLevel = 1;
		float arenaItemRate = 1.0f;
		TableManager tableManager = TableManager.Instance;
		ArenaItemRateTable arenaItemRateTable = tableManager != null ? tableManager.arenaItemRateTable : null;
		if (lifeManager != null)
			charLevel = lifeManager.charLevel;
		if (arenaItemRateTable != null)
			arenaItemRate = arenaItemRateTable.GetItemRate(charLevel);
		
		foreach(EquipInfo old in applyEquipItems)
		{
			isCostumeItem = false;
			if (old != null && old.item != null)
			{
				if (old.item.itemInfo != null)
				{
					switch(old.item.itemInfo.itemType)
					{
					case ItemInfo.eItemType.Costume_Back:
					case ItemInfo.eItemType.Costume_Body:
					case ItemInfo.eItemType.Costume_Head:
						isCostumeItem = true;
						break;
					}
				}
				
				bool isArenaItem = old.item.CheckArenaItem();
				if (isArenaItem == true)
					old.item.SetArenaItemRate(arenaItemRate);
				
				if (isCostumeItem == true)
				{
					foreach(var delValue in old.item.attributeList)
					{
						AttributeValue att = delValue.Value;
						lifeManager.attributeManager.SubValueRate(att.valueType, att.Value);
					}
				}
				else
				{
					foreach(var delValue in old.item.attributeList)
						lifeManager.attributeManager.SubValue(delValue.Value);
				}				
				setItemManager.RemoveSetItem(old.item, lifeManager.attributeManager);
			}
		}
		applyEquipItems.Clear();
		
		
		int weaponID = -1;
		int bodyID = -1;
		int headID = -1;
		int backID = -1;
		
		foreach(EquipInfo temp in list)
		{
			applyEquipItems.Add(temp);
			
			isCostumeItem = false;
			if (temp != null && temp.item != null)
			{
				if (temp.item.itemInfo != null)
				{
					switch(temp.item.itemInfo.itemType)
					{
					case ItemInfo.eItemType.Costume_Back:
					case ItemInfo.eItemType.Costume_Body:
					case ItemInfo.eItemType.Costume_Head:
						isCostumeItem = true;
						break;
					}
				}
				
				bool isArenaItem = temp.item.CheckArenaItem();
				if (isArenaItem == true)
					temp.item.SetArenaItemRate(arenaItemRate);
				
				if (isCostumeItem == true)
				{
					foreach(var addValue in temp.item.attributeList)
					{
						AttributeValue att = addValue.Value;
						lifeManager.attributeManager.AddValueRate(att.valueType, att.Value);
					}
				}
				else
				{
					foreach(var addValue in temp.item.attributeList)
						lifeManager.attributeManager.AddValue(addValue.Value);
				}
				
				switch(temp.slotType)
				{
				case GameDef.eSlotType.Weapon:
					if (temp.item.itemInfo != null)
						weaponID = temp.item.itemInfo.itemID;
					break;
				case GameDef.eSlotType.Costume_Body:
					if (temp.item.itemInfo != null)
						bodyID = temp.item.itemInfo.itemID;
					break;
				case GameDef.eSlotType.Costume_Back:
					if (temp.item.itemInfo != null)
						backID = temp.item.itemInfo.itemID;
					break;
				case GameDef.eSlotType.Costume_Head:
					if (temp.item.itemInfo != null)
						headID = temp.item.itemInfo.itemID;
					break;
				}
				
				setItemManager.ApplySetItem(temp.item, lifeManager.attributeManager);
			}
		}
		
		if (lifeManager != null)
		{
			lifeManager.ChangeCostume(bodyID, headID, backID);
			lifeManager.ChangeWeapon(weaponID);
		}
	}
	
	public int GetEmptySlotIndex(GameDef.eSlotType type)
	{
		int emptyIndex = -1;
		
		int index = 0;
		List<int> indexList = new List<int>();
		foreach(EquipInfo info in equipItems)
		{
			if (info.slotType == type && info.item == null)
			{
				indexList.Add(index);
			}
			
			++index;
		}
		
		if (indexList.Count > 0)
			emptyIndex = indexList[0];
		
		return emptyIndex;
	}
	
	public int GetEquipSlotIndex(GameDef.eSlotType type)
	{
		int equipIndex = -1;
		
		int index = 0;
		List<int> indexList = new List<int>();
		foreach(EquipInfo info in equipItems)
		{
			if (info.slotType == type && info.item != null && info.item.itemInfo != null)
			{
				indexList.Add(index);
			}
			
			++index;
		}
		
		if (indexList.Count > 0)
		{
			int itemCount = int.MaxValue;
			foreach(int idx in indexList)
			{
				EquipInfo info = equipItems[idx];
				if (itemCount > info.item.itemCount)
				{
					itemCount = info.item.itemCount;
					equipIndex = idx;
				}
			}
		}
		
		return equipIndex;
	}
	
	public Item GetEquipItem(int index)
	{
		Item equipItem = null;
		int nCount = equipItems.Count;
		
		if (index < 0 || index >= nCount)
			return equipItem;
		
		EquipInfo info = equipItems[index];
		if (info != null)
			equipItem = info.item;
		
		return equipItem;
	}
	
	public void SetEquipItem(int index, Item item)
	{
		int nCount = equipItems.Count;
		
		if (index < 0 || index >= nCount)
			return ;
		
		int charLevel = 1;
		float arenaItemRate = 1.0f;
		TableManager tableManager = TableManager.Instance;
		ArenaItemRateTable arenaItemRateTable = tableManager != null ? tableManager.arenaItemRateTable : null;
		if (lifeManager != null)
			charLevel = lifeManager.charLevel;
		if (arenaItemRateTable != null)
			arenaItemRate = arenaItemRateTable.GetItemRate(charLevel);
		
		EquipInfo info = equipItems[index];
		if (info != null)
		{
			if (info.item != null)
			{
				bool isArenaItem = info.item.CheckArenaItem();
				if (isArenaItem == true)
					info.item.SetArenaItemRate(arenaItemRate);
				
				foreach(var delValue in info.item.attributeList)
				{
					lifeManager.attributeManager.SubValue(delValue.Value);
				}
				
				setItemManager.RemoveSetItem(info.item, lifeManager.attributeManager);
			}
			
			info.item = item;
			
			if (info.item != null)
			{
				bool isArenaItem = info.item.CheckArenaItem();
				if (isArenaItem == true)
					info.item.SetArenaItemRate(arenaItemRate);
				
				foreach(var addValue in info.item.attributeList)
				{
					lifeManager.attributeManager.AddValue(addValue.Value);
				}
				
				setItemManager.ApplySetItem(info.item, lifeManager.attributeManager);
			}
		}
		
		if (onChangeEquip != null)
			onChangeEquip();
	}
	
	public bool HasEquped(int itemID)
	{
		bool hasEquiped = false;
		
		int testNum = itemID;
		int modValue = testNum % 10;
		int subMaskValue = 1;
		
		while(modValue == 0 && testNum > 10)
		{
			testNum = testNum / 10;
			modValue = testNum % 10;
			
			subMaskValue *= 10;
		}
		
		foreach(EquipInfo info in equipItems)
		{
			if (info.item == null || info.item.itemInfo== null)
				continue;
			
			int tempID = info.item.itemInfo.itemID;
			int restNum = tempID % subMaskValue;
			int flagID = tempID - restNum;
			if (flagID == itemID)
			{
				hasEquiped = true;
				break;
			}
		}
		
		return hasEquiped;
	}
}
