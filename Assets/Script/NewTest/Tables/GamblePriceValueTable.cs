using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GamblePrice
{
	public Vector3 gamblePrice = Vector3.zero;
	public Vector3 refreshPrice = Vector3.zero;
}

public class GamblePriceValueTable : BaseTable {
	public Dictionary<int, GamblePrice> dataList = new Dictionary<int, GamblePrice>();
	
	public GamblePrice GetData(int id)
	{
		GamblePrice data = null;
		
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
			GamblePrice gamblePrice = null;
			
			foreach(var data in db.data)
			{
				if (data.Key == "")
					break;
				
				id = int.Parse(data.Key);
				
				gamblePrice = new GamblePrice();
				
				gamblePrice.gamblePrice.x = data.Value.GetValue("Gamble_Gold").ToFloat();
				gamblePrice.gamblePrice.y = data.Value.GetValue("Gamble_Jewel").ToFloat();
				gamblePrice.gamblePrice.z = data.Value.GetValue("Gamble_Medal").ToFloat();
				
				gamblePrice.refreshPrice.x = data.Value.GetValue("Refresh_Gold").ToFloat();
				gamblePrice.refreshPrice.y = data.Value.GetValue("Refresh_Jewel").ToFloat();
				gamblePrice.refreshPrice.z = data.Value.GetValue("Refresh_Medal").ToFloat();
				
				dataList.Add(id, gamblePrice);
			}
		}
	}
}
