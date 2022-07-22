using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CostumeSetItem
{
	public string UID = "";
	public CostumeSetItemInfo setItemInfo = null;
	public List<Item> items = new List<Item>();
	
	public void SetItems(List<Item> itemList)
	{
		items.Clear();
		
		int setID = -1;
		if  (setItemInfo != null)
			setID = setItemInfo.setID;
		
		foreach(Item item in itemList)
		{
			if (item != null && item.itemInfo != null)
			{
				item.itemInfo.costumeSetItemID = setID;
				
				items.Add(item);
			}
		}
	}
	
	public void SetInfo(CostumeSetItemInfo info)
	{
		setItemInfo = info;
		
		if (setItemInfo != null)
		{
			foreach(int itemID in setItemInfo.itemIDs)
			{
				Item item = Item.CreateItem(itemID, "", 0, 0, 1, -1, 0);
				if (item != null && item.itemInfo != null)
					item.itemInfo.costumeSetItemID = setItemInfo.setID;
				
				items.Add(item);
			}
		}
	}
	
	public Item GetItemByIndex(int index)
	{
		Item item = null;
		int nCount = items.Count;
		if (index >= 0 && index < nCount)
			item = items[index];
		
		return item;
	}
	
	public static CostumeSetItem Create(int id, string UID)
	{
		TableManager tableManager = TableManager.Instance;
		CostumeSetItemTable costumeSetTable = tableManager != null ? tableManager.costumeSetItemTable : null;
		
		CostumeSetItem newItem = new CostumeSetItem();
		CostumeSetItemInfo info = null;
		if (costumeSetTable != null)
			info = costumeSetTable.GetCostumeSetInfo(id);
		
		newItem.UID = UID;
		newItem.SetInfo(info);
		
		return newItem;
	}
}

public class CostumeSetItemInfo
{
	public int setID = -1;
	public ItemInfo.eClass limitClass = ItemInfo.eClass.Warrior;
	
	public Vector3 sellPrice = Vector3.zero;
	public Vector3 buyPrice = Vector3.zero;
	public string setName = "";
	public string setSpriteName = "";
	
	public List<int> itemIDs = new List<int>();
}

public class CostumeSetItemTable : BaseTable {
	public Dictionary<int, CostumeSetItemInfo> dataList = new Dictionary<int, CostumeSetItemInfo>();
	
	public CostumeSetItemInfo GetCostumeSetInfo(int id)
	{
		CostumeSetItemInfo info = null;
		if (dataList.ContainsKey(id) == true)
			info = dataList[id];
		
		return info;
	}
	
	public override void LoadTable(CSVDB db)
	{
		if (db != null)
		{
			int id = 0;
			//CostumeSetItemInfo costumeSetInfo = null;
			int setID = -1;
			int itemID = -1;
			int sellPrice = 0;
			int buyPrice = 0;
			string setName = "";
			string spriteName = "";
			ItemInfo.eClass limitClass = ItemInfo.eClass.Common;
			
			List<int> itemList = new List<int>();
			
			ValueData valueData = null;
			foreach(var data in db.data)
			{
				if (data.Key == "")
					break;
				
				id = int.Parse(data.Key);
				
				setID = id;
				setName = data.Value.GetValue("Name").ToText();
				spriteName = data.Value.GetValue("SpriteName").ToText();
				sellPrice = data.Value.GetValue("SellPrice").ToInt();
				buyPrice = data.Value.GetValue("BuyPrice").ToInt();
				
				string limitClassStr = data.Value.GetValue("Class").ToText();
				if (limitClassStr == "Common")
					limitClass = ItemInfo.eClass.Common;
				else if (limitClassStr == "Warrior")
					limitClass = ItemInfo.eClass.Warrior;
				else if (limitClassStr == "Assassin")
					limitClass = ItemInfo.eClass.Assassin;
				else if (limitClassStr == "Wizard")
					limitClass = ItemInfo.eClass.Wizard;
				
				itemList.Clear();
				for (int index = 0;  ; ++index)
				{
					string fieldName = string.Format("ItemID_{0}", index);
					valueData = data.Value.GetValue(fieldName);
					
					if (valueData != null)
					{
						itemID = valueData.ToInt();
						itemList.Add(itemID);
					}
					else
					{
						break;
					}
				}
				
				CostumeSetItemInfo newInfo = new CostumeSetItemInfo();
				newInfo.setID = setID;
				newInfo.limitClass = limitClass;
				
				newInfo.setName = setName;
				newInfo.setSpriteName = spriteName;
				newInfo.sellPrice = new Vector3((float)sellPrice, 0.0f, 0.0f);
				newInfo.buyPrice = new Vector3(0.0f, (float)buyPrice, 0.0f);
				newInfo.itemIDs.AddRange(itemList);
				
				dataList.Add(id, newInfo);
			}
		}
	}
}
