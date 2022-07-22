using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StringTable : BaseTable {

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
				msg = data.Value.GetValue("msg").ToText();
				
				dataList.Add(id, msg);
			}
		}
		
	}
}
