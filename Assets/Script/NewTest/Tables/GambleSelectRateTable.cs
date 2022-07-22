using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GambleSelectRate
{
	public int selectRow1 = 0;
	public int selectRow2 = 0;
	public int selectRow3 = 0;
}

public class GambleSelectRateTable : BaseTable {
	public Dictionary<int, GambleSelectRate> dataList = new Dictionary<int, GambleSelectRate>();
	
	public GambleSelectRate GetData(int id)
	{
		GambleSelectRate data = null;
		
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
			GambleSelectRate gambleSelectRate = null;
			
			foreach(var data in db.data)
			{
				if (data.Key == "")
					break;
				
				id = int.Parse(data.Key);
				
				gambleSelectRate = new GambleSelectRate();
				
				gambleSelectRate.selectRow1 = data.Value.GetValue("Row1").ToInt();
				gambleSelectRate.selectRow2 = data.Value.GetValue("Row2").ToInt();
				gambleSelectRate.selectRow3 = data.Value.GetValue("Row3").ToInt();
				
				dataList.Add(id, gambleSelectRate);
			}
		}
	}
}
