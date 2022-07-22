using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShopInfo
{
	public int itemID = -1;
	public int itemRate = -1;
}

public class ShopTable : BaseTable {
	public int itemRateStep = -1;
	public Dictionary<int, ShopInfo> dataList = new Dictionary<int, ShopInfo>();
	
	public ShopInfo GetData(int id)
	{
		ShopInfo data = null;
		if (dataList != null &&
			dataList.ContainsKey(id) == true)
			data = dataList[id];
		
		return data;
	}
	
	public override void LoadTable(CSVDB db)
	{
		if (db != null)
		{
			int id = 0;
			ShopInfo shopInfo = null;
			int itemID = -1;
			
			ValueData valueData = null;
			
			foreach(var data in db.data)
			{
				if (data.Key == "")
					break;
				
				id = int.Parse(data.Key);
				
				itemID = -1;
				valueData = data.Value.GetValue("ItemID");
				if (valueData != null)
					itemID = valueData.ToInt();
				
				if (itemID != -1)
				{
					shopInfo = new ShopInfo();
					shopInfo.itemID = itemID;
					shopInfo.itemRate = itemRateStep;
					
					dataList.Add(id, shopInfo);
				}
			}
		}
	}
}
