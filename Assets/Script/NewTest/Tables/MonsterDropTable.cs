using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterDropItemInfo
{
	public enum eDropType
	{
		ItemDrop,
		PotionDrop,
		GoldDrop,
		JewelDrop,
		MaterialItemDrop,
		EventItemDrop,
	}
	public eDropType dropType = eDropType.GoldDrop;
	
	public int itemID = -1;
	public int itemCount = 0;
	public float dropRate = 0;
}

public class EventDropItemInfo
{
	
}

public class MonsterDropInfo
{
	public List<MonsterDropItemInfo> dropItems = new List<MonsterDropItemInfo>();
	
	public MonsterDropItemInfo potionInfo = new MonsterDropItemInfo();
	public MonsterDropItemInfo goldInfo = new MonsterDropItemInfo();
	public MonsterDropItemInfo jewelInfo = new MonsterDropItemInfo();
	
	public float eventDropRate = 0.0f;
	public MonsterDropItemInfo eventDropInfo = new MonsterDropItemInfo();
	
	public MonsterDropItemInfo GetRandDropInfo(int eventID)
	{
		int randValue = Random.Range(0, 1000);
		
		bool isEventDrop = false;
		if (eventID != -1)
		{
			int eventRate = Mathf.RoundToInt(eventDropRate * 1000.0f);
			if (randValue < eventRate)
				isEventDrop = true;
		}
		
		if (isEventDrop == true)
		{
			return this.eventDropInfo;
		}
		else
		{
			randValue = Random.Range(0, 1000);
			
			int startRate = 0;
			int limitRate = Mathf.RoundToInt(goldInfo.dropRate * 1000.0f);
			if (randValue >= startRate && randValue < limitRate)
				return goldInfo;
			
			startRate = limitRate;
			limitRate += Mathf.RoundToInt(jewelInfo.dropRate * 1000.0f);
			if (randValue >= startRate && randValue < limitRate)
				return jewelInfo;
			
			startRate = limitRate;
			limitRate += Mathf.RoundToInt(potionInfo.dropRate * 1000.0f);
			if (randValue >= startRate && randValue < limitRate)
				return potionInfo;
			
			foreach(MonsterDropItemInfo info in dropItems)
			{
				startRate = limitRate;
				limitRate += Mathf.RoundToInt(info.dropRate * 1000.0f);
				if (randValue >= startRate && randValue < limitRate)
					return info;
			}
			
			return goldInfo;
		}
	}
}

public class MonsterDropTable : BaseTable {

	public Dictionary<int, MonsterDropInfo> dataList = new Dictionary<int, MonsterDropInfo>();
	
	public MonsterDropInfo GetData(int id)
	{
		MonsterDropInfo data = null;
		if (dataList != null &&
			dataList.ContainsKey(id) == true)
			data = dataList[id];
		
		return data;
	}
	
	public override void LoadTable(CSVDB db)
	{
		TableManager tableManager = TableManager.Instance;
		ItemTable itemTable = tableManager != null ? tableManager.itemTable : null;
		
		if (db != null)
		{
			int id = 0;
			MonsterDropInfo dropInfo = null;
			
			int itemID = 0;
			float dropRate = 0.0f;
			
			ValueData valueData1 = null;
			ValueData valueData2 = null;
			
			foreach(var data in db.data)
			{
				if (data.Key == "")
					break;
				
				id = int.Parse(data.Key);
				
				dropInfo = new MonsterDropInfo();
				
				int index = 1;
				for(; ; ++index)
				{
					itemID = 0;
					dropRate = 0.0f;
					
					valueData1 = data.Value.GetValue("Material" + index);
					valueData2 = data.Value.GetValue("Material" + index + "_drop rate");
					if (valueData1 != null && valueData2 != null)
					{
						itemID = valueData1.ToInt();
						dropRate = valueData2.ToFloat();
						
						if (itemID != 0 && dropRate != 0.0f)
						{
							MonsterDropItemInfo dropItemInfo = new MonsterDropItemInfo();
							dropItemInfo.itemID = itemID;
							
							ItemInfo itemInfo = itemTable != null ? itemTable.GetData(itemID) : null;
							
							MonsterDropItemInfo.eDropType dropType = MonsterDropItemInfo.eDropType.ItemDrop;
							if (itemInfo != null)
							{
								switch(itemInfo.itemType)
								{
								case ItemInfo.eItemType.Material:
								case ItemInfo.eItemType.Material_Compose:
									dropType = MonsterDropItemInfo.eDropType.MaterialItemDrop;
									break;
								default:
									dropType = MonsterDropItemInfo.eDropType.ItemDrop;
									break;
								}
							}
							
							dropItemInfo.dropType = dropType;
							
							dropItemInfo.dropRate = dropRate;
							dropItemInfo.itemCount = 1;
							
							dropInfo.dropItems.Add(dropItemInfo);
						}
					}
					else
					{
						break;
					}
				}
				
				valueData1 = data.Value.GetValue("Potion");
				valueData2 = data.Value.GetValue("Potion_drop rate");
				if (valueData1 != null && valueData2 != null)
				{
					itemID = valueData1.ToInt();
					dropRate = valueData2.ToFloat();
					
					if (itemID != 0 && dropRate != 0.0f)
					{
						MonsterDropItemInfo dropPotionInfo = new MonsterDropItemInfo();
						dropPotionInfo.itemID = itemID;
						dropPotionInfo.dropRate = dropRate;
						dropPotionInfo.itemCount = 1;
						dropPotionInfo.dropType = MonsterDropItemInfo.eDropType.PotionDrop;
						
						dropInfo.potionInfo = dropPotionInfo;
					}
				}
				
				valueData1 = data.Value.GetValue("gem");
				valueData2 = data.Value.GetValue("gem_drop rate");
				if (valueData1 != null && valueData2 != null)
				{
					itemID = valueData1.ToInt();
					dropRate = valueData2.ToFloat();
					
					if (itemID != 0 && dropRate != 0.0f)
					{
						MonsterDropItemInfo dropJewelInfo = new MonsterDropItemInfo();
						dropJewelInfo.itemID = itemID;
						dropJewelInfo.dropRate = dropRate;
						dropJewelInfo.itemCount = 1;
						dropJewelInfo.dropType = MonsterDropItemInfo.eDropType.JewelDrop;
						
						dropInfo.jewelInfo = dropJewelInfo;

					}
				}
				
				valueData1 = data.Value.GetValue("gold");
				valueData2 = data.Value.GetValue("gold_drop rate");
				if (valueData1 != null && valueData2 != null)
				{
					itemID = valueData1.ToInt();
					dropRate = valueData2.ToFloat();
					
					if (itemID != 0 && dropRate != 0.0f)
					{
						MonsterDropItemInfo dropGoldInfo = new MonsterDropItemInfo();
						dropGoldInfo.itemID = itemID;
						dropGoldInfo.dropRate = dropRate;
						dropGoldInfo.itemCount = 1;
						dropGoldInfo.dropType = MonsterDropItemInfo.eDropType.GoldDrop;
						
						dropInfo.goldInfo = dropGoldInfo;
					}
				}
				
				valueData1 = data.Value.GetValue("EventRate");
				valueData2 = data.Value.GetValue("EventDropID");
				if (valueData1 != null && valueData2 != null)
				{
					dropRate = valueData1.ToFloat();
					itemID = valueData2.ToInt();
					if (itemID != 0 && dropRate != 0.0f)
					{
						MonsterDropItemInfo eventDropInfo = new MonsterDropItemInfo();
						eventDropInfo.itemID = itemID;
						eventDropInfo.dropRate = dropRate;
						eventDropInfo.itemCount = 1;
						eventDropInfo.dropType = MonsterDropItemInfo.eDropType.EventItemDrop;
						
						dropInfo.eventDropInfo = eventDropInfo;
						dropInfo.eventDropRate = dropRate;
					}
				}
								
				this.dataList.Add(id, dropInfo);
			}
		}
	}
}
