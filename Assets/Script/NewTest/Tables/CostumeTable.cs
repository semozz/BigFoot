using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CostumeInfo
{
	public string prefabFileName = "";
	public string option = "";
}

public class CostumeTable : BaseTable {
	public Dictionary<int, CostumeInfo> dataList = new Dictionary<int, CostumeInfo>();
	
	public CostumeInfo GetData(int id)
	{
		CostumeInfo info = null;
		if (dataList != null && dataList.ContainsKey(id) == true)
			info = dataList[id];
		
		return info;
	}
	
	public override void LoadTable(CSVDB db)
	{
		if (db != null)
		{
			int id = 0;
			CostumeInfo info = null;
			
			ValueData _valueData;
			
			string prefabNaem = "";
			string optionVaue = "";
			
			foreach(var data in db.data)
			{
				id = int.Parse(data.Key);
				
				info = new CostumeInfo();
				
				_valueData = data.Value.GetValue("Prefab");
				if (_valueData != null)
					info.prefabFileName = _valueData.ToText();
				
				_valueData = data.Value.GetValue("Option");
				if (_valueData != null)
					info.option = _valueData.ToText();
				
				dataList.Add(id, info);
			}
		}
		
	}
	
}
