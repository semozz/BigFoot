using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GamblePriceInfo
{
	public int minLevel = 0;
	public int maxLevel = 0;
	
	public int needGold = 0;
}

public class GamblePriceTable : BaseTable {
	public Dictionary<int, GamblePriceInfo> dataList = new Dictionary<int, GamblePriceInfo>();
	
	public Vector3 GetGamblePrice(int charLevel)
	{
		Vector3 price = Vector3.zero;
		foreach(var temp in dataList)
		{
			GamblePriceInfo info = temp.Value;
			if (info.minLevel <= charLevel && charLevel <= info.maxLevel)
			{
				price.x = info.needGold;
				break;
			}
		}
		
		return price;
	}
	
	public override void LoadTable(CSVDB db)
	{
		if (db != null)
		{
			int id = 0;
			GamblePriceInfo priceInfo = null;
			int minLevel = 0;
			int maxLevel = 0;
			int needGold = 0;
			
			//ValueData valueData = null;
			
			foreach(var data in db.data)
			{
				if (data.Key == "")
					break;
				
				id = int.Parse(data.Key);
				
				minLevel = data.Value.GetValue("LV_low").ToInt();
				maxLevel = data.Value.GetValue("LV_High").ToInt();
				needGold = data.Value.GetValue("Gold").ToInt();
				
				priceInfo = new GamblePriceInfo();
				priceInfo.minLevel = minLevel;
				priceInfo.maxLevel = maxLevel;
				priceInfo.needGold = needGold;
				
				dataList.Add(id, priceInfo);
			}
		}
	}
}
