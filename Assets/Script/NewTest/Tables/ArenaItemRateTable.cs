using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArenaItemRateTable : BaseTable {
	public Dictionary<int, float> arenaItemRateList = new Dictionary<int, float>();
	
	public float GetItemRate(int charLevel)
	{
		float rateValue = 1.0f;
		if (arenaItemRateList.ContainsKey(charLevel) == true)
			rateValue = arenaItemRateList[charLevel];
		
		return rateValue;
	}
	
	public override void LoadTable(CSVDB db)
	{
		if (db != null)
		{
			int level = 0;
			float rateValue = 1.0f;
			foreach(var data in db.data)
			{
				level = int.Parse(data.Key);
				
				rateValue = data.Value.GetValue("LimitRate").ToFloat();
				
				arenaItemRateList.Add(level, rateValue);
			}
		}
	}
}
