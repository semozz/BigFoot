using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GotoMapInfo
{
	public enum eGotoType
	{
		eGoto_Stage = 0,
		eGoto_Gamble = 1,
		eGoto_Shop = 2,
		eGoto_Defence = 3
	}
	public eGotoType gotoType = eGotoType.eGoto_Stage;
	
	public Dictionary<int, int> gotoStageInfos = new Dictionary<int, int>();
	
	public int tabID;
}

public class BaseItemInfo
{
	public float attackDamage = 0.0f;
	public float abilityPower = 0.0f;
	
	public float criticalHitRate = 0.0f;
	
	public float health = 0.0f;
	public float healthRegen = 0.0f;
	
	public float manaRegen = 0.0f;
	
	public float armor = 0.0f;
	public float magicResist = 0.0f;
	public float armorPenetration = 0.0f;
	public float magicPenetration = 0.0f;
	
	public float lifeSteal = 0.0f;
}
public class ItemInfo
{
	public int itemID = -1;
	public int setItemID = -1;
	
	public string itemName = "";
	public string itemIcon = "";
	public string strDesc = "";
	
	public enum eClass
	{
		Warrior,
		Assassin,
		Wizard,
		Common,
	}
	public eClass limitClass = eClass.Common;
	
	public enum eItemType
	{
		Common,
		Head,
		Hand,
		Pants,
		Boots,
		Armor,
		Weapon,
		Accessories,
		Ring,
		Potion_1,
		Potion_2,
		Material,
		Costume_Body,
		Costume_Back,
		Costume_Head,
		Arena,
		Material_Compose,
	}
	public eItemType itemType = eItemType.Common;
	
	public int costumeSetItemID = -1;
	
	public int equipLevel = 0;
	public int maxStackCount = 1;
	//public int materialID = -1;
	public List<int> materialIDs = new List<int>();
	
	public Vector3 buyPrice = Vector3.zero;
	public Vector3 sellPrice = Vector3.zero;
	
	public int addComposeRate = 0;
	
	public uint baseExp = 0;
	public int isArena = 0;
	
	public int itemLevel = 0;
	
	public Dictionary<GotoMapInfo.eGotoType, GotoMapInfo> stageIDs = new Dictionary<GotoMapInfo.eGotoType, GotoMapInfo>(); //0:1;1:1;2:1;Gamble;Storage:2
	
	public BaseItemInfo baseItemInfo = new BaseItemInfo();
}

public class ItemTable : BaseTable
{
	public Dictionary<int, ItemInfo> dataList = new Dictionary<int, ItemInfo>();
	
	public List<ItemInfo> accessoriesItems = new List<ItemInfo>();
	public List<ItemInfo> costumeItems = new List<ItemInfo>();
	public List<ItemInfo> arenaItems = new List<ItemInfo>();
	
	public ItemInfo GetData(int id)
	{
		ItemInfo data = null;
		if (dataList != null &&
			dataList.ContainsKey(id) == true)
			data = dataList[id];
		
		return data;
	}
	
	public int GetItemIDByIndex(int reqIndex)
	{
		int itemID = -1;
		
		//int nCount = dataList.Count;
		int index = 0;
		foreach(var temp in dataList)
		{
			if (index == reqIndex)
			{
				itemID = temp.Key;
				break;
			}
			
			++index;
		}
		
		return itemID;
	}
	
	public int GetRandItemID()
	{
		int itemID = -1;
		
		int nCount = dataList.Count;
		int randIndex = -1;
		
		if (nCount > 0)
			randIndex = Random.Range(0, nCount);
		
		int index = 0;
		foreach(var temp in dataList)
		{
			if (index == randIndex)
			{
				itemID = temp.Key;
				break;
			}
			
			++index;
		}
		
		return itemID;
	}
	
	public override void LoadTable(CSVDB db)
	{
		if (db != null)
		{
			int id = 0;
			ItemInfo itemData = null;
			
			foreach(var data in db.data)
			{
				id = int.Parse(data.Key);
				
				itemData = new ItemInfo();
				
				itemData.itemID = id;
				itemData.itemName = data.Value.GetValue("Name").ToText();
				itemData.itemIcon = id.ToString();
				
				ValueData setItemIDValue = data.Value.GetValue("SetNo.");
				itemData.setItemID = setItemIDValue != null ? setItemIDValue.ToInt() : -1;
				
				ValueData costumeSetItemIDValue = data.Value.GetValue("CostumeSetID");
				itemData.costumeSetItemID = costumeSetItemIDValue != null ? costumeSetItemIDValue.ToInt() : -1;
					
				ValueData descValue = data.Value.GetValue("Desc");
				itemData.strDesc = descValue != null ? descValue.ToText() : "";
				
				string limitClass = data.Value.GetValue("Class").ToText();
				if (limitClass == "Common")
					itemData.limitClass = ItemInfo.eClass.Common;
				else if (limitClass == "Warrior")
					itemData.limitClass = ItemInfo.eClass.Warrior;
				else if (limitClass == "Assassin")
					itemData.limitClass = ItemInfo.eClass.Assassin;
				else if (limitClass == "Wizard")
					itemData.limitClass = ItemInfo.eClass.Wizard;
				
				string itemType = data.Value.GetValue("Part").ToText();
				if (itemType == "Head")
					itemData.itemType = ItemInfo.eItemType.Head;
				else if (itemType == "Hand")
					itemData.itemType = ItemInfo.eItemType.Hand;
				else if (itemType == "Pants")
					itemData.itemType = ItemInfo.eItemType.Pants;
				else if (itemType == "Foot")
					itemData.itemType = ItemInfo.eItemType.Boots;
				else if (itemType == "Body")
					itemData.itemType = ItemInfo.eItemType.Armor;
				else if (itemType == "Weapon")
					itemData.itemType = ItemInfo.eItemType.Weapon;
				else if (itemType == "Accessories")
					itemData.itemType = ItemInfo.eItemType.Accessories;
				else if (itemType == "Ring")
					itemData.itemType = ItemInfo.eItemType.Ring;
				else if (itemType == "Material")
					itemData.itemType = ItemInfo.eItemType.Material;
				else if (itemType == "Potion_1")
					itemData.itemType = ItemInfo.eItemType.Potion_1;
				else if (itemType == "Potion_2")
					itemData.itemType = ItemInfo.eItemType.Potion_2;
				else if (itemType == "C_Body")
					itemData.itemType = ItemInfo.eItemType.Costume_Body;
				else if (itemType == "C_Back")
					itemData.itemType = ItemInfo.eItemType.Costume_Back;
				else if (itemType == "C_Head")
					itemData.itemType = ItemInfo.eItemType.Costume_Head;
				else if (itemType == "Material_Compose")
					itemData.itemType = ItemInfo.eItemType.Material_Compose;
				else
					itemData.itemType = ItemInfo.eItemType.Common;
				
				itemData.equipLevel = data.Value.GetValue("EquipLV").ToInt();
				
				itemData.buyPrice.x = data.Value.GetValue("Price_Gold").ToFloat();
				itemData.buyPrice.y = data.Value.GetValue("Price_Jewel").ToFloat();
				itemData.buyPrice.z = data.Value.GetValue("Price_Medal").ToFloat();
				
				if (itemData.itemType == ItemInfo.eItemType.Material_Compose)
				{
					itemData.addComposeRate = data.Value.GetValue("Type").ToInt();
				}
				
				ValueData sellPrice = data.Value.GetValue("SellPrice");
				if (sellPrice == null)
					itemData.sellPrice = Vector3.zero;
				else
					itemData.sellPrice.x = sellPrice.ToFloat();
				
				itemData.maxStackCount = 1;
				ValueData maxCount = data.Value.GetValue("MaxStackCount");
				if (maxCount != null)
					itemData.maxStackCount = maxCount.ToInt();
				
				//if (itemData.itemType == ItemInfo.eItemType.Potion)
				//	itemData.maxStackCount = 9;
				
				int index = 0;
				for(; ; ++index)
				{
					int materialID = 0;
					ValueData materialIDValue = data.Value.GetValue("Grade" + index + "_TableID");
					if (materialIDValue != null)
						materialID = materialIDValue.ToInt();
					else
						break;
					if (materialID != 0)
						itemData.materialIDs.Add(materialID);
				}
				
				itemData.baseExp = (uint)data.Value.GetValue("EXP").ToInt();
				itemData.isArena = data.Value.GetValue("Arena").ToInt();
				itemData.itemLevel = data.Value.GetValue("ItemLv").ToInt();
				
				ValueData stageData = data.Value.GetValue("StageType");
				if (stageData != null)
				{
					string stageDataStr = stageData.ToText();
					string[] splits = stageDataStr.Split(';');
					foreach(string temp in splits)
					{
						GotoMapInfo newInfo = null;
						
						string[] tempSplits = temp.Split(':');
						string typeStr = tempSplits[0];
						if (typeStr == "0" || typeStr == "1" || typeStr == "2")
						{
							if (itemData.stageIDs.ContainsKey(GotoMapInfo.eGotoType.eGoto_Stage) == false)
							{
								newInfo = new GotoMapInfo();
								newInfo.gotoType = GotoMapInfo.eGotoType.eGoto_Stage;
								
								itemData.stageIDs.Add(GotoMapInfo.eGotoType.eGoto_Stage, newInfo);
							}
							else
								newInfo = itemData.stageIDs[GotoMapInfo.eGotoType.eGoto_Stage];
							
							int stageType = int.Parse(typeStr);
							int stageID = int.Parse(tempSplits[1]);
							
							if (newInfo != null)
								newInfo.gotoStageInfos.Add(stageType, stageID);
						}
						else if (typeStr == "Gamble")
						{
							newInfo = new GotoMapInfo();
							
							newInfo.gotoType = GotoMapInfo.eGotoType.eGoto_Gamble;
						}
						else if (typeStr == "Shop")
						{
							newInfo = new GotoMapInfo();
							
							newInfo.gotoType = GotoMapInfo.eGotoType.eGoto_Shop;
							newInfo.tabID = 0;//int.Parse(tempSplits[1]);
						}
						else if (typeStr == "Defence")
						{
							newInfo = new GotoMapInfo();
							
							newInfo.gotoType = GotoMapInfo.eGotoType.eGoto_Defence;
						}
						
						if (newInfo != null && itemData.stageIDs.ContainsKey(newInfo.gotoType) == false)
							itemData.stageIDs.Add(newInfo.gotoType, newInfo);
					}
				}
								
				BaseItemInfo baseItemInfo = itemData.baseItemInfo;
				
				baseItemInfo.attackDamage = data.Value.GetValue("AttackDamage").ToFloat();
				baseItemInfo.abilityPower = data.Value.GetValue("AbilityPower").ToFloat();
				
				baseItemInfo.criticalHitRate = data.Value.GetValue("CriticalHitRate").ToFloat() * 0.01f;
				
				baseItemInfo.health = data.Value.GetValue("HealthMax").ToFloat();
				baseItemInfo.healthRegen = data.Value.GetValue("HealthRegen").ToFloat();
				
				baseItemInfo.manaRegen = data.Value.GetValue("ManaRegen").ToFloat();
				
				baseItemInfo.armor = data.Value.GetValue("Armor").ToFloat();
				baseItemInfo.magicResist = data.Value.GetValue("MagicResist").ToFloat();
				baseItemInfo.armorPenetration = data.Value.GetValue("ArmorPenetration").ToFloat();
				baseItemInfo.magicPenetration = data.Value.GetValue("MagicPenetration").ToFloat();
				
				baseItemInfo.lifeSteal = data.Value.GetValue("LifeSteal").ToFloat();
				
				this.dataList.Add(id, itemData);
				
				switch(itemData.itemType)
				{
				case ItemInfo.eItemType.Accessories:
					accessoriesItems.Add(itemData);
					break;
				case ItemInfo.eItemType.Costume_Body:
				case ItemInfo.eItemType.Costume_Head:
				case ItemInfo.eItemType.Costume_Back:
					costumeItems.Add(itemData);
					break;
				case ItemInfo.eItemType.Head:
				case ItemInfo.eItemType.Hand:
				case ItemInfo.eItemType.Pants:
				case ItemInfo.eItemType.Boots:
				case ItemInfo.eItemType.Armor:
				case ItemInfo.eItemType.Weapon:
				case ItemInfo.eItemType.Ring:
					{
						// 장착가능하고 메달로 사는아이템이면 arenaItems이다.
						if (itemData.isArena == 1)
							arenaItems.Add(itemData);
					}
					break;
				}
			}
		}
	}
}
