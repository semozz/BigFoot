using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GambleSGradeList : BaseTable {
	public List<int> dataList = new List<int>();
	
	public override void LoadTable(CSVDB db)
	{
		if (db != null)
		{
			int id = 0;
			
			int itemID = -1;
			
			ValueData valueData = null;
			foreach(var data in db.data)
			{
				if (data.Key == "")
					break;
				
				id = int.Parse(data.Key);
				
				int index = 1;
				for(; ; ++index)
				{
					itemID = -1;
					
					valueData = data.Value.GetValue("Item_" + index);
					if (valueData != null)
					{
						itemID = valueData.ToInt();
						
						if (itemID != 0)
							dataList.Add(itemID);
					}
					else
					{
						break;
					}
				}
			}
		}
	}
}
