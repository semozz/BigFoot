using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GambleItemInfo
{
	public int itemID = -1;
}

public class GambleItemInfos
{
	public Vector2 levelRange = Vector2.zero;
	public List<GambleItemInfo> gambleItems = new List<GambleItemInfo>();
	
	public GambleItemInfos(int startLevel, int endLevel)
	{
		levelRange.x = (float)startLevel;
		levelRange.y = (float)endLevel;
	}
	
	public void Add(GambleItemInfo info)
	{
		gambleItems.Add(info);
	}
}

public class GambleItemTable : BaseTable {

	public Dictionary<int, GambleItemInfos> dataList = new Dictionary<int, GambleItemInfos>();
	
	public GambleItemInfos GetData(int level)
	{
		GambleItemInfos data = null;
		
		foreach(var temp in dataList)
		{
			GambleItemInfos info = temp.Value;
			if (info.levelRange.x <= level && level <= info.levelRange.y)
			{
				data = info;
				break;
			}
		}
		
		return data;
	}
	
	public override void LoadTable(CSVDB db)
	{
		if (db != null)
		{
			int id = 0;
			GambleItemInfos gambleInfos = null;
			int itemID = -1;
			
			int startLevel = -1;
			int endLevel = -1;
			
			ValueData valueData1 = null;
			foreach(var data in db.data)
			{
				if (data.Key == "")
					break;
				
				id = int.Parse(data.Key);
				
				startLevel = data.Value.GetValue("LV_low").ToInt();
				endLevel = data.Value.GetValue("LV_High").ToInt();
				
				gambleInfos = new GambleItemInfos(startLevel, endLevel);
				
				int index = 1;
				for(; ; ++index)
				{
					itemID = -1;
					
					valueData1 = data.Value.GetValue("Item_" + index);
					if (valueData1 != null)
					{
						itemID = valueData1.ToInt();
						
						if (itemID != 0)
						{
							GambleItemInfo info = new GambleItemInfo();
							info.itemID = itemID;
							
							gambleInfos.Add(info);
						}
					}
					else
					{
						break;
					}
				}
				
				dataList.Add(id, gambleInfos);
			}
		}
	}
}
