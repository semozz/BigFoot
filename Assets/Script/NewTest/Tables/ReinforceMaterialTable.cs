using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CompositionMaterialInfo
{
	public int slotIndex = -1;
	public Item item = null;
	
	public static int SortByDefault (CompositionMaterialInfo a, CompositionMaterialInfo b)
	{
		int aValue = (a != null) ? (int)a.item.reinforceStep : -1;
		int bValue = (b != null) ? (int)b.item.reinforceStep : -1;
		
		return aValue - bValue;
	}
}

public class ReinforceMaterials
{
	public List<ReinforceMaterialInfo> materials = new List<ReinforceMaterialInfo>();
	public int stageActNo = 1;
	
	public ReinforceMaterialInfo GetMaterial(int index)
	{
		ReinforceMaterialInfo info = null;
		int nCount = materials.Count;
		if (index >= 0 && index < nCount)
			info = materials[index];
		
		return info;
	}
	
	public void AddMaterial(ReinforceMaterialInfo info)
	{
		if (info == null)
			return;
		
		materials.Add(info);
	}
}

public class ReinforceMaterialInfo
{
	public int nItemID = -1;
	public int nItemCount = 0;
}

public class ReinforceMaterial
{
	public Item item = null;
	public bool bCheck = false;
}

public class ReinforceMaterialTable : BaseTable {
	public Dictionary<int, ReinforceMaterials> dataList = new Dictionary<int, ReinforceMaterials>();
	
	public ReinforceMaterials GetData(int id)
	{
		ReinforceMaterials data = null;
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
			int itemID = -1;
			int itemCount = -1;
			
			ValueData valueData1 = null;
			ValueData valueData2 = null;
			
			foreach(var data in db.data)
			{
				if (data.Key == "")
					break;
				
				id = int.Parse(data.Key);
				
				ReinforceMaterials materials = new ReinforceMaterials();
				
				int index = 1;
				for(; ; ++index)
				{
					itemID = -1;
					itemCount = -1;
					
					valueData1 = data.Value.GetValue("ItemID" + index);
					valueData2 = data.Value.GetValue("ItemCount" + index);
					if (valueData1 != null && valueData2 != null)
					{
						itemID = valueData1.ToInt();
						itemCount = valueData2.ToInt();
						
						if (itemID != 0 && itemCount != 0)
						{
							ReinforceMaterialInfo info = new ReinforceMaterialInfo();
							info.nItemID = itemID;
							info.nItemCount = itemCount;
							
							materials.AddMaterial(info);
						}
					}
					else
					{
						break;
					}
				}
				
				valueData1 = data.Value.GetValue("StageAct");
				if (valueData1 != null)
					materials.stageActNo = valueData1.ToInt();
				else
					materials.stageActNo = 0;
				
				dataList.Add(id, materials);
			}
		}
	}
	
}
