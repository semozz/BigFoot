using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Item {
	public static int limitReinforceStep = 6;
	public static int limitCompositionStep = 5;
	
	public static float reinforceGoldRate = 1.5f;
	
	public static List<float> itemGradeRates = new List<float>();
	public static List<float> itemSellRates = new List<float>();
	public static float GetGradeRate(int grade)
	{
		float rateValue = 1.0f;
		int nCount = itemGradeRates.Count;
		if (grade >= 0 && grade < nCount)
			rateValue = itemGradeRates[grade];
		
		return rateValue;
	}
	
	public static float GetSellRate(int grade)
	{
		float rateValue = 1.0f;
		int nCount = itemSellRates.Count;
		if (grade >= 0 && grade < nCount)
			rateValue = itemSellRates[grade];
		
		return rateValue;
	}
	
	//아이템 그레이드에 따른 능력치 비율.
	public float itemGradeRate = 0.03f;
	public int itemGrade = 0;
	public int reinforceStep = 0;
	public int itemCount = 1;
	
	public string uID = "";
	
	public Dictionary<AttributeValue.eAttributeType, AttributeValue> attributeList = new Dictionary<AttributeValue.eAttributeType, AttributeValue>();
	
	public ItemInfo itemInfo = null;
	
	//아이템 능력치?(하급/중급...)
	public int itemRateStep = -1;
	public float itemRateValue = 1.0f;
	
	public int setItemID = -1;
	
	public bool isTemplateItem = false;
	
	public uint itemExp = 0;
	public void AddExp(uint addExp)
	{
		uint totalExp = itemExp + addExp;
		
		SetExp(totalExp);
	}
	
	public void SetExp(uint totalExp)
	{
		itemExp = totalExp;
		
		ItemReinforceInfo reinforceInfo = null;
		if (itemInfo != null)
			reinforceInfo = GetReinforceInfo(itemInfo.baseExp, itemExp, itemRateValue);
		
		int newReinforceStep = reinforceStep;
		if (reinforceInfo != null)
			newReinforceStep = reinforceInfo.step;
		
		SetGradeInfo(this.itemGrade, newReinforceStep);
	}
	
	public static ItemReinforceInfo GetReinforceInfo(uint baseExp, uint totalExp, float itemRate)
	{
		TableManager tableManager = TableManager.Instance;
		ItemReinforceInfoTable reinforceInfoTable = tableManager != null ? tableManager.itemReinforceInfoTable : null;
		
		ItemReinforceInfo reinforceInfo = null;
		if (reinforceInfoTable != null)
			reinforceInfo = reinforceInfoTable.GetItemReinforceInfo(baseExp, totalExp, itemRate);
		
		return reinforceInfo;
	}
	
	public static ItemReinforceInfo GetMaxReinforceInfo(uint baseExp, float itemRate)
	{
		TableManager tableManager = TableManager.Instance;
		ItemReinforceInfoTable reinforceInfoTable = tableManager != null ? tableManager.itemReinforceInfoTable : null;
		
		ItemReinforceInfo reinforceInfo = null;
		if (reinforceInfoTable != null)
			reinforceInfo = reinforceInfoTable.GetMaxReinforceInfo(baseExp, itemRate);
		
		return reinforceInfo;
	}
	
	private bool isNewItem = false;
	public bool IsNewItem
	{
		get { return isNewItem; }
		set { isNewItem = value; }
	}
	
	public Item()
	{
		
	}
	
	public static Item CreateItem(Item oldItem)
	{
		Item newItem = null;
		
		if (oldItem != null && oldItem.itemInfo != null)
		{
			newItem = new Item();
			newItem.SetItem(oldItem.itemInfo.itemID);
			
			//newItem.SetGradeInfo(oldItem.itemGrade, oldItem.reinforceStep);
			newItem.itemGrade = oldItem.itemGrade;
			
			newItem.itemCount = oldItem.itemCount;
			
			newItem.uID = oldItem.uID;
			
			newItem.itemRateStep = oldItem.itemRateStep;
			newItem.itemRateValue = oldItem.itemRateValue;
			
			newItem.SetExp(oldItem.itemExp);
		}
		
		return newItem;
	}
	
	public static Item CreateItem(EquipItemDBInfo dbInfo)
	{
		return CreateItem(dbInfo.ID,  dbInfo.UID, dbInfo.Grade, dbInfo.Reinforce, dbInfo.Count, dbInfo.Rate, dbInfo.Exp);
	}
	
	public static Item CreateItem(ItemDBInfo dbInfo)
	{
		return CreateItem(dbInfo.ID,  dbInfo.UID, dbInfo.Grade, dbInfo.Reinforce, dbInfo.Count, dbInfo.Rate, dbInfo.Exp);
	}
	
	public static Item CreateItem(MaterialItemDBInfo dbInfo)
	{
		return CreateItem(dbInfo.ID, dbInfo.UID, 0, 0, dbInfo.Count, -1, 0);
	}
	
	public static Item CreateItem(int itemID, string uID, int itemGrade, int reinforceStep, int itemCount)
	{
		return CreateItem(itemID, uID, itemGrade, reinforceStep, itemCount, -1, 0);
	}
	
	public static Item CreateItem(int itemID, string uID, int itemGrade, int reinforceStep, int itemCount, int itemRateStep, int Exp)
	{
		Item newItem = null;
		
		newItem = new Item();
		newItem.SetItem(itemID);
		
		newItem.SetItemRate(itemRateStep);
		
		newItem.itemGrade = itemGrade;
		//newItem.SetGradeInfo(itemGrade, reinforceStep);
		newItem.SetExp((uint)Exp);
		
		newItem.itemCount = itemCount;
		newItem.uID = uID;
		
		return newItem;
	}
	
	public void SetItemInfo(int itemID, string uID, int itemGrade, int reinforceStep, int itemCount, int itemRateStep, int Exp)
	{
		SetItem(itemID);
		this.uID = uID;
		this.itemCount = itemCount;
		
		SetItemRate(itemRateStep);
		
		//SetGradeInfo(itemGrade, reinforceStep);
		this.itemGrade = itemGrade;
		SetExp((uint)Exp);
	}
	
	public void SetItemRate(int itemRateStep)
	{
		if (this.itemInfo != null)
		{
			switch(this.itemInfo.itemType)
			{
			case ItemInfo.eItemType.Material:
			case ItemInfo.eItemType.Material_Compose:
				itemRateStep = -1;
				break;
			}
		}
		
		this.itemRateStep = itemRateStep;
		
		string stringValueTableName = "";
		switch(itemRateStep)
		{
		case 0://최하급.
			stringValueTableName = "ItemRate1";
			break;
		case 1://하급.
			stringValueTableName = "ItemRate2";
			break;
		case 2://중급.
			stringValueTableName = "ItemRate3";
			break;
		case 3://상급.
			stringValueTableName = "ItemRate4";
			break;
		case 4://최상급.
			stringValueTableName = "ItemRate5";
			break;
		default:
			stringValueTableName = "ItemRate3";
			break;
		}
		
		TableManager tableMgr = TableManager.Instance;
		StringValueTable stringValueTable = tableMgr != null ? tableMgr.stringValueTable : null;
		if (stringValueTable != null)
			itemRateValue = stringValueTable.GetData(stringValueTableName) * 0.01f;
		else
			itemRateValue = 1.0f;
	}
	
	public void SetItem(int itemID)
	{
		TableManager tableMgr = TableManager.Instance;
		AttributeInitTable attributeIncTable = null;
		ItemTable itemTable = null;
		if (tableMgr != null)
		{
			itemTable = tableMgr.itemTable;
			attributeIncTable = tableMgr.attributeIncTable;
		}
		
		if (itemTable != null)
			itemInfo = itemTable.GetData(itemID);
		
		if (itemInfo != null)
			this.setItemID = itemInfo.setItemID;
		
		AttributeInitData incData = null;
		if (attributeIncTable != null)
			incData = attributeIncTable.GetData((int)(Game.Instance.playerClass) + 1);
		
		if (itemInfo != null)
		{
			if (itemInfo.baseItemInfo.attackDamage != 0.0f)
				AddAttributeValue(AttributeValue.eAttributeType.AttackDamage, itemInfo.baseItemInfo.attackDamage, incData != null ? incData.attackDamage : 0.0f);
			if (itemInfo.baseItemInfo.abilityPower != 0.0f)
				AddAttributeValue(AttributeValue.eAttributeType.AbilityPower, itemInfo.baseItemInfo.abilityPower, incData != null ? incData.abilityPower : 0.0f);
			
			if (itemInfo.baseItemInfo.criticalHitRate != 0.0f)
				AddAttributeValue(AttributeValue.eAttributeType.CriticalHitRate, itemInfo.baseItemInfo.criticalHitRate, incData != null ? incData.criticalHitRate : 0.0f);
			
			if (itemInfo.baseItemInfo.health != 0.0f)
				AddAttributeValue(AttributeValue.eAttributeType.HealthMax, itemInfo.baseItemInfo.health, incData != null ? incData.healthMax : 0.0f);
			if (itemInfo.baseItemInfo.healthRegen != 0.0f)
				AddAttributeValue(AttributeValue.eAttributeType.HealthRegen, itemInfo.baseItemInfo.healthRegen, incData != null ? incData.healthRegen : 0.0f);
			
			if (itemInfo.baseItemInfo.manaRegen != 0.0f)
				AddAttributeValue(AttributeValue.eAttributeType.ManaRegen, itemInfo.baseItemInfo.manaRegen, incData != null ? incData.manaRegen : 0.0f);
			
			if (itemInfo.baseItemInfo.armor != 0.0f)
				AddAttributeValue(AttributeValue.eAttributeType.Armor, itemInfo.baseItemInfo.armor, incData != null ? incData.armor : 0.0f);
			if (itemInfo.baseItemInfo.magicResist != 0.0f)
				AddAttributeValue(AttributeValue.eAttributeType.MagicResist, itemInfo.baseItemInfo.magicResist, incData != null ? incData.magicResist : 0.0f);
			
			if (itemInfo.baseItemInfo.armorPenetration != 0.0f)
				AddAttributeValue(AttributeValue.eAttributeType.ArmorPenetration, itemInfo.baseItemInfo.armorPenetration, incData != null ? incData.armorPenetration : 0.0f);
			if (itemInfo.baseItemInfo.magicPenetration != 0.0f)
				AddAttributeValue(AttributeValue.eAttributeType.MagicPenetration, itemInfo.baseItemInfo.magicPenetration, incData != null ? incData.magicPenetration : 0.0f);
			
			if (itemInfo.baseItemInfo.lifeSteal != 0.0f)
				AddAttributeValue(AttributeValue.eAttributeType.LifeSteal, itemInfo.baseItemInfo.lifeSteal, 0.0f);
			
		}
	}
	
	public AttributeValue AddAttributeValue(AttributeValue.eAttributeType type, float _value, float incValue)
	{
		AttributeValue attValue = null;
		if (attributeList.ContainsKey(type) == false)
		{
			attValue = new AttributeValue(type, _value, incValue, 1.0f);
			
			bool isCostume = false;
			if (itemInfo != null)
			{
				switch(itemInfo.itemType)
				{
				case ItemInfo.eItemType.Costume_Back:
				case ItemInfo.eItemType.Costume_Body:
				case ItemInfo.eItemType.Costume_Head:
					isCostume = true;
					break;
				}
			}
			
			if (isCostume == true)
				attValue.calcType = AttributeValue.eAttributeCalcType.CostumeAttribute;
			else
				attValue.calcType = AttributeValue.eAttributeCalcType.ItemAttribute;
			
			attributeList.Add(type, attValue);
		}
		else
		{
			attValue = attributeList[type];
			attValue.baseValue = _value;
		}
		
		return attValue;
	}
	
	public float GetItemAttribute(AttributeValue.eAttributeType type)
	{
		AttributeValue val = null;
		if (attributeList.ContainsKey(type) == true)
			val = attributeList[type];
		
		float result = 0.0f;
		if (val != null)
			result = val.Value;
		
		return result;
	}
	
	public void SetGradeInfo(int grade, int reinforce)
	{
		this.itemGrade = grade;
		this.reinforceStep = reinforce;
		
		this.itemGradeRate = GetGradeRate(grade);
		
		UpdateAttributeValue();
	}
	
	public void SetArenaItemRate(float arenaRate)
	{
		AttributeValue attValue = null;
		foreach(var temp in attributeList)
		{
			attValue = temp.Value;
			
			attValue.itemRateValue = this.itemRateValue;
			attValue.itemGradeRateValue = this.itemGradeRate;
			attValue.level_limitRate = arenaRate;
			
			attValue.SetItemGrade(itemGrade, reinforceStep);
		}
	}
	
	public void ResetArenaItemRate()
	{
		SetArenaItemRate(1.0f);
	}
	
	public void UpdateAttributeValue()
	{
		float limitRate = 1.0f;
		if (CheckArenaItem() == true)
		{
			
		}
		
		AttributeValue attValue = null;
		foreach(var temp in attributeList)
		{
			attValue = temp.Value;
			
			attValue.itemRateValue = this.itemRateValue;
			attValue.itemGradeRateValue = this.itemGradeRate;
			attValue.level_limitRate = limitRate;
			
			attValue.SetItemGrade(itemGrade, reinforceStep);
		}
	}
	
	public string GetAttributeInfo()
	{
		return GetAttributeInfo(Game.Instance.itemAttPlusColor, Game.Instance.itemAttMinusColor);
	}
	
	public string GetAttributeInfo(Color plusColor, Color minusColor)
	{
		if (isTemplateItem == false)
			return GetAttributeInfos(plusColor, minusColor);
		else
			return GetTemplateItemInfos();
	}
	
	public string GetAttributeInfos(Color plusColor, Color minusColor)
	{
		string msg = "";
		
		//int nCount = attributeList.Count;
		int index = 0;
		AttributeValue attValue = null;
		
		foreach(var temp in attributeList)
		{
			attValue = temp.Value;
			
			if (index > 0)
				msg += "\n";
			
			msg += attValue.GetItemInfoStr(plusColor, minusColor);
			
			++index;
		}
		
		return msg;
	}
	
	public string GetAttributeInfos()
	{
		return GetAttributeInfos(Game.Instance.itemAttPlusColor, Game.Instance.itemAttMinusColor);
	}
	
	public string GetTemplateItemInfos()
	{
		string msg = "";
		int index = 0;
		AttributeValue attValue = null;
		
		foreach(var temp in attributeList)
		{
			attValue = temp.Value;
			
			if (index > 0)
				msg += "\n";
			
			msg += attValue.GetTemplateItemInfoStr();
			
			++index;
		}
		
		return msg;
	}
	
	public string GetReinforceAttributeInfo(uint addExp, Color origPlusColor, Color origMinusColor, Color plusColor, Color minusColor)
	{
		string msg = "";
		
		int curReinforceStep = Mathf.Min(limitReinforceStep, reinforceStep);
		
		ItemReinforceInfo nextReinfoceInfo = Item.GetReinforceInfo(this.itemInfo.baseExp, this.itemExp + addExp, this.itemRateValue);
		int nextLevel = Mathf.Min(limitReinforceStep, nextReinfoceInfo.step);
		
		if (curReinforceStep == nextLevel)
			nextLevel = -1;
		
		AttributeValue attValue = null;
		int index = 0;
		foreach(var temp in attributeList)
		{
			attValue = temp.Value;
			
			if (index > 0)
				msg += "\n";
			
			msg += attValue.GetInfoStrAddNextLevel(nextLevel, origPlusColor, origMinusColor, plusColor, minusColor);
			
			++index;
		}
		
		return msg;
	}
	
	public static Vector3 NeedGoldForReinforce(uint addExp)
	{
		/*
		Vector3 needGoldValue = itemInfo != null ? itemInfo.sellPrice * 5.0f : Vector3.zero;
		needGoldValue *= (0.1f * ((float)reinforceStep + 1.0f));
		*/
		
		Vector3 needGoldValue = Vector3.zero;
		float goldValue = (float)addExp * Item.reinforceGoldRate;
		string goldValueStr = string.Format("{0}", (int)goldValue);
		needGoldValue.x = (float)int.Parse(goldValueStr);
		
		return needGoldValue;
	}
	
	public bool CheckReinforce(int ownGold, int ownJewel, int ownMedal)
	{
		int errorCount = 0;
		
		if (itemInfo == null)
			errorCount++;
		else
		{
			switch(itemInfo.itemType)
			{
			case ItemInfo.eItemType.Common:
			case ItemInfo.eItemType.Material:
			case ItemInfo.eItemType.Material_Compose:
			case ItemInfo.eItemType.Potion_1:
			case ItemInfo.eItemType.Potion_2:
			case ItemInfo.eItemType.Costume_Body:
			case ItemInfo.eItemType.Costume_Head:
			case ItemInfo.eItemType.Costume_Back:
				errorCount++;
				break;
			}
		}
		
		if (reinforceStep >= limitReinforceStep)
			errorCount++;
		
		return (errorCount == 0);
	}
	
	
	public Vector3 NeedGoldForComposition()
	{
		Vector3 needGoldValue = itemInfo != null ? itemInfo.sellPrice * 5.0f : Vector3.zero;
		float goldRate = 1.0f;
		
		TableManager tableManager = TableManager.Instance;
		StringValueTable stringValueTable = tableManager != null ? tableManager.stringValueTable : null;
		
		string fieldName = string.Format("ItemCompositionGold{0}", this.itemGrade);
		int goldRateValue = 100;
		if (stringValueTable != null)
			goldRateValue = stringValueTable.GetData(fieldName);
		
		goldRate = goldRateValue * 0.01f;
		
		needGoldValue *= goldRate * this.itemRateValue;
		
		return needGoldValue;
	}
	
	public bool CheckComposition(int ownGold, int ownJewel, int ownMedal)
	{
		int errorCount = 0;
		
		if (itemGrade >= Item.limitCompositionStep)
			errorCount++;
		
		if (itemInfo == null)
			errorCount++;
		else
		{
			switch(itemInfo.itemType)
			{
			case ItemInfo.eItemType.Common:
			case ItemInfo.eItemType.Material:
			case ItemInfo.eItemType.Material_Compose:
			case ItemInfo.eItemType.Potion_1:
			case ItemInfo.eItemType.Potion_2:
			case ItemInfo.eItemType.Costume_Body:
			case ItemInfo.eItemType.Costume_Head:
			case ItemInfo.eItemType.Costume_Back:
				errorCount++;
				break;
			}
		}
		
		return (errorCount == 0);
	}
	
	public int GetMaterialID()
	{
		int materialID = -1;
		
		int itemGradeIndex = (int)this.itemGrade;
		if (itemInfo != null)
		{
			int nCount = itemInfo.materialIDs.Count;
			if (itemGradeIndex >= 0 && itemGradeIndex < nCount)
				materialID = itemInfo.materialIDs[itemGradeIndex];
		}
		
		return materialID;
	}
	
	public bool CheckArenaItem()
	{
		bool isArena = false;
		
		/*
		if (this.itemInfo != null &&
			this.itemInfo.buyPrice.z > 0.0f)
			isArena = true;
		*/
		
		if (this.itemInfo != null &&
			this.itemInfo.isArena == 1)
				isArena = true;
		
		return isArena;
	}
	
	public Vector3 GetSellPrice()
	{
		TableManager tableManager = TableManager.Instance;
		ItemReinforceInfoTable reinforceInfoTable = tableManager != null ? tableManager.itemReinforceInfoTable : null;
		
		ItemReinforceInfo reinforceInfo = null;
		if (reinforceInfoTable != null)
			reinforceInfo = reinforceInfoTable.GetItemReinforceInfo(this.itemInfo.baseExp, itemExp, itemRateValue);
		
		float sellReinforceRate = 1.0f;
		if (reinforceInfo != null)
			sellReinforceRate = reinforceInfo.sellPriceRate;
		
		float sellGradeRate = GetSellRate(this.itemGrade);
		
		float rate = itemRateValue * (sellReinforceRate + sellGradeRate);
		Vector3 sellPrice = Vector3.zero;
		if (itemInfo != null)
			sellPrice = itemInfo.sellPrice * rate;
		
		return sellPrice;
	}
	
	public uint GetItemExp()
	{
		uint expValue = 0;
		
		if (itemInfo != null)
		{
			switch(itemInfo.itemType)
			{
			case ItemInfo.eItemType.Material:
				expValue = itemInfo.baseExp;
				break;
			case ItemInfo.eItemType.Material_Compose:
				expValue = 0;
				break;
			default:
				ItemReinforceInfo info = GetReinforceInfo(itemInfo.baseExp, this.itemExp, itemRateValue);
				if (info != null && itemInfo.baseExp != 0.0f)
				{
					float rateValue = (info.sellPriceRate + GetSellRate(this.itemGrade)) * itemRateValue;
					float resultValue = itemInfo.baseExp * rateValue;
					string floatStr = string.Format("{0:F0}", resultValue);
					
					int intValue = int.Parse(floatStr);
					
					expValue = (uint)intValue;
				}
				break;
			}
		}
		
		return expValue;
	}
	
	private Color itemRate1Color = Color.white;
	private Color itemRate2Color = new Color(0.470588f, 1.0f, 0.313725f);
	private Color itemRate3Color = new Color(0.0f, 0.70588f, 1.0f);
	private Color itemRate4Color = new Color(0.862745f, 0.35294f, 1.0f);
	private Color itemRate5Color = new Color(1.0f, 0.7843f, 0.2745f);
	
	private int itemRate1StringID = 169;
	private int itemRate2StringID = 170;
	private int itemRate3StringID = 171;
	private int itemRate4StringID = 172;
	private int itemRate5StringID = 173;
	
	private int itemGradeStringID = 1000;
	
	public string GetItemName()
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
		
		if (itemInfo.itemName != "")
		{
			if (postfixString != "")
				itemNameStr = string.Format("{0}{1}({2})[-]", GameDef.RGBToHex(itemRateColor), itemInfo.itemName, postfixString);
			else
				itemNameStr = string.Format("{0}{1}[-]", GameDef.RGBToHex(itemRateColor), itemInfo.itemName);
		}
		else
			itemNameStr = itemInfo.itemName;
		
		return itemNameStr;
	}
	
	public uint GetItemMaxExp()
	{
		uint expValue = 0;
		
		if (itemInfo != null)
		{
			switch(itemInfo.itemType)
			{
			case ItemInfo.eItemType.Material:
			case ItemInfo.eItemType.Material_Compose:
				expValue = 0;
				break;
			default:
				ItemReinforceInfo info = Item.GetMaxReinforceInfo(itemInfo.baseExp, itemRateValue);
				if (info != null)
					expValue = info.limitExp;
				break;
			}
		}
		
		return expValue;
	}
	
	//리턴이 false이면 해당 캐릭터 사용 못함.
	public bool CheckLimitClass(int charIndex)
	{
		bool isLimit = true;
		if (itemInfo != null)
		{
			switch(itemInfo.limitClass)
			{
			case ItemInfo.eClass.Warrior:
			case ItemInfo.eClass.Assassin:
			case ItemInfo.eClass.Wizard:
				isLimit = charIndex == (int)itemInfo.limitClass;
				break;
			default:
				isLimit = true;
				break;
			}
		}
		
		return isLimit;
	}
}
