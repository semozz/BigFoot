using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossRaidData
{
	public int bossID = 0;
	
	public int kingdomID = 0;
	
	public string bossName = "";
	public string desc = "";
	public int maxHP = 0;
	
	public string bossPrefabPath = "";
	
	public int stageType = 0;
}

public class BossRaidTable : BaseTable {
	public Dictionary<int, BossRaidData> dataList = new Dictionary<int, BossRaidData>();
	
	public BossRaidData GetData(int id)
	{
		BossRaidData data = null;
		if (dataList.ContainsKey(id) == true)
			data = dataList[id];
		
		return data;
	}
	
	public override void LoadTable(CSVDB db)
	{
		if (db != null)
		{
			int id = 0;
			ValueData valueData = null;
			foreach(var data in db.data)
			{
				if (data.Key == "")
					break;
				
				id = int.Parse(data.Key);
				
				BossRaidData newData = new BossRaidData();
				
				newData.bossID = id;
				newData.bossName = data.Value.GetValue("Name").ToText();
				newData.desc = data.Value.GetValue("Desc").ToText();
				newData.maxHP = data.Value.GetValue("HP").ToInt();
				
				newData.kingdomID = data.Value.GetValue("Kingdom").ToInt();
				
				newData.bossPrefabPath = data.Value.GetValue("BossPrefabPath").ToText();
				
				valueData = data.Value.GetValue("StageType");
				if (valueData != null)
					newData.stageType = valueData.ToInt();
				
				dataList.Add(id, newData);
			}
		}
	}
}
