using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShopCostumeSetTable : BaseTable {
	public Dictionary<int, int> dataList = new Dictionary<int, int>();

	public override void LoadTable(CSVDB db)
	{
		if (db != null)
		{
			int id = 0;
			//CostumeSetItem costumeSet = null;
			int itemID = -1;
			
			ValueData valueData = null;
			foreach(var data in db.data)
			{
				if (data.Key == "")
					break;
				
				id = int.Parse(data.Key);
				
				itemID = -1;
				valueData = data.Value.GetValue("CostumeSetID");
				if (valueData != null)
					itemID = valueData.ToInt();
				
				if (itemID != -1)
				{
					//CostumeSetItem setItem = CostumeSetItem.Create(itemID, "");
					dataList.Add(id, itemID);
				}
			}
		}
	}
}
