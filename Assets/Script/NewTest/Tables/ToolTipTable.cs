using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ToolTipTable : BaseTable {
	public Dictionary<int, string> dataList = new Dictionary<int, string>();
	
	public string GetData(int id)
	{
		string msg = "";
		if (dataList != null && dataList.ContainsKey(id) == true)
			msg = dataList[id];
		else
			msg = id + " not Found!";
		
		return msg;
	}
	
	public override void LoadTable(CSVDB db)
	{
		if (db != null)
		{
			int id = 0;
			string msg = "";
			
			foreach(var data in db.data)
			{
				id = int.Parse(data.Key);
				msg = data.Value.GetValue("ToolTip").ToText();
				
				dataList.Add(id, msg);
			}
		}
		
	}
	
	public string GetRandTooltip()
	{
		int nCount = dataList.Count;
		
		string tooltip = "";
		int index = -1;
		if (nCount > 0)
			index = Random.Range(1, nCount + 1);
		
		tooltip = GetData(index);
		return tooltip;
	}
	
}
