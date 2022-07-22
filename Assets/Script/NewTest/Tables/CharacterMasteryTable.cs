using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MasteryIDs
{
	public List<int>	 masteryIDs = new List<int>();
	
	public void AddMasteryID(int id)
	{
		masteryIDs.Add(id);
	}
}

public class CharacterMasteryTable : BaseTable {
	public Dictionary<GameDef.ePlayerClass, MasteryIDs> dataList = new Dictionary<GameDef.ePlayerClass, MasteryIDs>();
	
	public MasteryIDs GetData(GameDef.ePlayerClass classType)
	{
		MasteryIDs info = null;
		if (dataList != null && dataList.ContainsKey(classType) == true)
			info = dataList[classType];
		
		return info;
	}
	
	public override void LoadTable(CSVDB db)
	{
		if (db != null)
		{
			MasteryIDs info = null;
			string infoStr = "";
			int masteryID = 0;
			
			//int id = 0;
			foreach(var data in db.data)
			{
				//id = int.Parse(data.Key);
				
				GameDef.ePlayerClass classType = GameDef.ePlayerClass.CLASS_WARRIOR;
				infoStr = data.Value.GetValue("Class_Type").ToText();
				if (infoStr == "Warrior")
					classType = GameDef.ePlayerClass.CLASS_WARRIOR;
				else if (infoStr == "Assassin")
					classType = GameDef.ePlayerClass.CLASS_ASSASSIN;
				else if (infoStr == "Wizzard")
					classType = GameDef.ePlayerClass.CLASS_WIZARD;
				
				if (dataList.ContainsKey(classType) == false)
				{
					info = new MasteryIDs();
					dataList.Add(classType, info);
				}
				else
					info = dataList[classType];
				
				infoStr = data.Value.GetValue("MasteryID").ToText();
				masteryID = int.Parse(infoStr);
				
				if (info != null)
					info.AddMasteryID(masteryID);
			}
		}
		
	}
}
