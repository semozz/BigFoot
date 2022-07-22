using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AwakeningLevelInfo
{
	public int id;
	public string skillName = "";
	public string iconName = "";
	
	public AttributeValue.eAttributeType valueType;
	public int maxCount;
	
	public int startGold;
	public int maxGold;
	
	public float incValue;
	
	public string desc = "";
}

public class AwakeningLevelInfoTable : BaseTable {
	public Dictionary<int, AwakeningLevelInfo> dataList = new Dictionary<int, AwakeningLevelInfo>();
	
	public AwakeningLevelInfo GetData(int id)
	{
		AwakeningLevelInfo info = null;
		if (dataList != null && dataList.ContainsKey(id) == true)
			info = dataList[id];
		
		return info;
	}
	
	public override void LoadTable(CSVDB db)
	{
		if (db != null)
		{
			int id = 0;
			AwakeningLevelInfo info = null;
			
			string infoStr = "";
			ValueData valueData = null;
			
			foreach(var data in db.data)
			{
				id = int.Parse(data.Key);
				
				info = new AwakeningLevelInfo();
				info.id = id;
				
				valueData = data.Value.GetValue("AttributeType");
				if (valueData != null)
					info.valueType = AttributeValue.GetAttributeType(valueData.ToText());
				else
					continue;
				
				info.skillName = data.Value.GetValue("Name").ToText();
				info.iconName = data.Value.GetValue("Icon").ToText();
				
				info.maxCount = data.Value.GetValue("Max").ToInt();
				
				infoStr = data.Value.GetValue("IncValue").ToText();
				if (infoStr != "")
					info.incValue = float.Parse(infoStr);
				
				info.startGold = data.Value.GetValue("StartMoney").ToInt();
				info.maxGold = data.Value.GetValue("MaxMoney").ToInt();
				
				valueData = data.Value.GetValue("Desc");
				if (valueData != null)
					info.desc = valueData.ToText();
				
				dataList.Add(id, info);
			}
		}
		
	}
}
