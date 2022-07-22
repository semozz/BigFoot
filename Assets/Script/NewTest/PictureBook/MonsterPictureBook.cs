using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterPictureBookInfo
{
	public int id = 0;
	public string name = "";
	public string desc = "";
	public int act = 0;
	public bool isHardMode = false;
	
	public Dictionary<AttributeValue.eAttributeType, AttributeValue> attributeList = new Dictionary<AttributeValue.eAttributeType, AttributeValue>();
	
	public bool isOpen = false;
	
	public string GetAttributeInfo()
	{
		string msg = "";
		
		//int nCount = attributeList.Count;
		int index = 0;
		AttributeValue attValue = null;
		
		foreach(var temp in attributeList)
		{
			attValue = temp.Value;
			
			bool skipValue = false;
			switch(attValue.valueType)
			{
			case AttributeValue.eAttributeType.Health:
			case AttributeValue.eAttributeType.CriticalHitRate:
			case AttributeValue.eAttributeType.CriticalDamageRate:
				skipValue = true;
				break;
			}
			
			if (attValue.Value == 0.0f || skipValue == true)
				continue;
			
			if (index > 0)
				msg += "\n";
			
			msg += attValue.GetItemInfoStr(Game.Instance.itemAttPlusColor, Game.Instance.itemAttMinusColor);
			
			++index;
		}
		
		return msg;
	}
	
	public static MonsterPictureBookInfo CreateInfo(MonsterPictureBookData data)
	{
		MonsterPictureBookInfo newInfo = new MonsterPictureBookInfo();
		
		newInfo.id = data.id;
		newInfo.name = data.name;
		newInfo.desc = data.desc;
		newInfo.act = data.act;
		newInfo.isHardMode = data.isHardMode;
		newInfo.isOpen = false;
		
		TableManager tableManager = TableManager.Instance;
		AttributeInitTable attributeTable = null;
		
		if (tableManager != null)
			attributeTable = tableManager.attributeInitTable;
		
		AttributeInitData initData = null;
		if (attributeTable != null)
			initData = attributeTable.GetData(data.id);
		
		if (initData != null)
		{
			AttributeValue[] attributes = {
				new AttributeValue(AttributeValue.eAttributeType.AbilityPower, initData.abilityPower),
				new AttributeValue(AttributeValue.eAttributeType.AttackDamage, initData.attackDamage),
				new AttributeValue(AttributeValue.eAttributeType.CriticalHitRate, initData.criticalHitRate),
				new AttributeValue(AttributeValue.eAttributeType.CriticalDamageRate, initData.criticalDamageRate),
				new AttributeValue(AttributeValue.eAttributeType.Health, initData.healthMax),
				new AttributeValue(AttributeValue.eAttributeType.HealthMax, initData.healthMax),
				new AttributeValue(AttributeValue.eAttributeType.HealthRegen, initData.healthRegen),
				
				new AttributeValue(AttributeValue.eAttributeType.Armor, initData.armor),
				new AttributeValue(AttributeValue.eAttributeType.MagicResist, initData.magicResist),
				new AttributeValue(AttributeValue.eAttributeType.ArmorPenetration, initData.armorPenetration),
				new AttributeValue(AttributeValue.eAttributeType.MagicPenetration, initData.magicPenetration),

			};
			
			foreach(AttributeValue initValue in attributes)
			{
				newInfo.attributeList.Add(initValue.valueType, initValue);
			}
		}
		
		return newInfo;
	}
}

public class MonsterPictureBook 
{

	public Dictionary<int, MonsterPictureBookInfo> normalMonsterPictureBookList = new Dictionary<int, MonsterPictureBookInfo>();
	public Dictionary<int, MonsterPictureBookInfo> hardMonsterPictureBookList = new Dictionary<int, MonsterPictureBookInfo>();
	
	public void AddNormalMonsterPictureBook(int id, MonsterPictureBookInfo info)
	{
		if (normalMonsterPictureBookList.ContainsKey(id) == false)
			normalMonsterPictureBookList.Add(id, info);
	}
	
	public void AddHardMonsterPictureBook(int id, MonsterPictureBookInfo info)
	{
		if (hardMonsterPictureBookList.ContainsKey(id) == false)
			hardMonsterPictureBookList.Add(id, info);
	}
	
	public void OpenNormalMonsterPictureBook(int id)
	{
		foreach(var temp in normalMonsterPictureBookList)
		{
			MonsterPictureBookInfo info = temp.Value;
			
			if (info.id == id)
				info.isOpen = true;
		}
	}
	
	public void OpenHardMonsterPictureBook(int id)
	{
		foreach(var temp in hardMonsterPictureBookList)
		{
			MonsterPictureBookInfo info = temp.Value;
			
			if (info.id == id)
				info.isOpen = true;
		}
	}
	
	public void InitData ()
	{
		normalMonsterPictureBookList.Clear();
		hardMonsterPictureBookList.Clear();
		
		TableManager tableManager = TableManager.Instance;
		MonsterPictureBookTable monsterPictureBook = tableManager != null ? tableManager.monsterPictureBookTable : null;
		
		if (monsterPictureBook != null)
		{
			foreach(var temp in monsterPictureBook.dataList)
			{
				MonsterPictureBookData data = temp.Value;
				
				MonsterPictureBookInfo newInfo = MonsterPictureBookInfo.CreateInfo(data);
				
				if (data.isHardMode == true)
					AddHardMonsterPictureBook(data.id, newInfo);
				else
					AddNormalMonsterPictureBook(data.id, newInfo);
			}
		}
	}
	
	public void UpdateMonsterPictureBook(CharInfoData charData)
	{
		foreach(var temp in normalMonsterPictureBookList)
			temp.Value.isOpen = false;
		foreach(var temp in hardMonsterPictureBookList)
			temp.Value.isOpen = false;
		
		TableManager tableManager = TableManager.Instance;
		StageTable stageTable = tableManager != null ? tableManager.stageTable : null;
		
		Dictionary<int, int> normalMonsterIDs = new Dictionary<int, int>();
		Dictionary<int, int> harMonsterIDs = new Dictionary<int, int>();
			
		if (charData != null)
		{
			int privateCount = charData.privateDatas.Length;
			for (int charIndex = 0; charIndex < privateCount; ++charIndex)
			{
				CharPrivateData privateData = charData.GetPrivateData(charIndex);
				if (privateData == null)
					continue;
				
				List<StageInfo> modeStageList = null;
				
				modeStageList = privateData.stageInfos[0];
				
				int nCount = modeStageList != null ? modeStageList.Count : 0;
				int monsterID = 0;
				StageInfo stageInfo = null;
				StageTableInfo stageTableInfo = null;
				
				for (int stageIndex = 0; stageIndex < nCount; ++stageIndex)
				{
					stageInfo = modeStageList[stageIndex];
					if (stageInfo.stageInfo == StageButton.eStageButton.Clear)
					{
						stageTableInfo = stageTable.GetData(stageIndex + 1);
						
						BasicStageInfo basicInfo = stageTableInfo.GetBasicStageInfo(0);
						foreach(string idStr in basicInfo.mobFaceList)
						{
							monsterID = int.Parse(idStr);
							if (normalMonsterIDs.ContainsKey(monsterID) == false)
								normalMonsterIDs.Add(monsterID, monsterID);
						}
					}
				}
				
				modeStageList = privateData.stageInfos[1];
				
				nCount = modeStageList != null ? modeStageList.Count : 0;
				for (int stageIndex = 0; stageIndex < nCount; ++stageIndex)
				{
					stageInfo = modeStageList[stageIndex];
					if (stageInfo.stageInfo == StageButton.eStageButton.Clear)
					{
						stageTableInfo = stageTable.GetData(stageIndex + 1);
						
						BasicStageInfo basicInfo = stageTableInfo.GetBasicStageInfo(1);
						foreach(string idStr in basicInfo.mobFaceList)
						{
							monsterID = int.Parse(idStr);
							if (harMonsterIDs.ContainsKey(monsterID) == false)
								harMonsterIDs.Add(monsterID, monsterID);
						}
					}
				}
			}
		}
		
		foreach(var temp in normalMonsterIDs)
			OpenNormalMonsterPictureBook(temp.Value);
		
		foreach(var temp in harMonsterIDs)
			OpenHardMonsterPictureBook(temp.Value);
	}
}
