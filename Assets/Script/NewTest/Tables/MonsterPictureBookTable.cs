using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterPictureBookData
{
	public int id = 0;
	public int act = 0;
	public bool isHardMode = false;
	
	public string name = "";
	public string desc = "";
}

public class MonsterPictureBookTable : BaseTable {
	public Dictionary<int, MonsterPictureBookData> dataList = new Dictionary<int, MonsterPictureBookData>();
	
	public MonsterPictureBookData GetData(int id)
	{
		MonsterPictureBookData data = null;
		if (dataList.ContainsKey(id) == true)
			data = dataList[id];
		
		return data;
	}
	
	public override void LoadTable(CSVDB db)
	{
		if (db != null)
		{
			int id = 0;
			
			foreach(var data in db.data)
			{
				id = int.Parse(data.Key);
				
				MonsterPictureBookData newInfo = new MonsterPictureBookData();
				newInfo.id = id;
				newInfo.name = data.Value.GetValue("Name").ToText();
				newInfo.desc = data.Value.GetValue("Desc").ToText();
				
				newInfo.act = data.Value.GetValue("Act").ToInt();
				newInfo.isHardMode = data.Value.GetValue("HardMode").ToText() == "true";
				
				dataList.Add(id, newInfo);
			}
		}
		
	}
}
