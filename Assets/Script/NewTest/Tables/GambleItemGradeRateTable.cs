using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GambleItemGradeRateInfo
{
	public List<int> rateList = new List<int>();
}

public class GambleItemGradeRateTable : BaseTable {

	public Dictionary<int, GambleItemGradeRateInfo> dataList = new Dictionary<int, GambleItemGradeRateInfo>();
	
	public GambleItemGradeRateInfo GetData(int id)
	{
		GambleItemGradeRateInfo data = null;
		
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
			GambleItemGradeRateInfo gambleRateInfo = null;
			int rateValue = 0;
			
			foreach(var data in db.data)
			{
				if (data.Key == "")
					break;
				
				id = int.Parse(data.Key);
				
				gambleRateInfo = new GambleItemGradeRateInfo();
				
				rateValue = data.Value.GetValue("Normal").ToInt();
				gambleRateInfo.rateList.Add(rateValue);
				
				rateValue = data.Value.GetValue("High").ToInt();
				gambleRateInfo.rateList.Add(rateValue);
				
				rateValue = data.Value.GetValue("MasterPiece").ToInt();
				gambleRateInfo.rateList.Add(rateValue);
				
				rateValue = data.Value.GetValue("Legendary").ToInt();
				gambleRateInfo.rateList.Add(rateValue);
				
				dataList.Add(id, gambleRateInfo);
			}
		}
	}
}

public class GambleItemRateTable : BaseTable {

	public Dictionary<int, GambleItemGradeRateInfo> dataList = new Dictionary<int, GambleItemGradeRateInfo>();
	
	public GambleItemGradeRateInfo GetData(int id)
	{
		GambleItemGradeRateInfo data = null;
		
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
			GambleItemGradeRateInfo gambleRateInfo = null;
			int rateValue = 0;
			
			string fieldName = "";
			int index = 0;
			ValueData valueData = null;
			
			foreach(var data in db.data)
			{
				if (data.Key == "")
					break;
				
				id = int.Parse(data.Key);
				
				gambleRateInfo = new GambleItemGradeRateInfo();
				
				index = 0;
				for (; ; ++index)
				{
					fieldName = string.Format("Rate_{0}", index);
					valueData = data.Value.GetValue(fieldName);
					
					if (valueData != null)
					{
						rateValue = valueData.ToInt();
						gambleRateInfo.rateList.Add(rateValue);
					}
					else
						break;
				}
				
				dataList.Add(id, gambleRateInfo);
			}
		}
	}
}
