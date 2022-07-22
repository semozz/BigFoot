using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MasteryTableInfo_New
{
	public int groupID = 0;
	public int id = 0;
	public MasteryInfo_New.eMasteryActiveType activeType = MasteryInfo_New.eMasteryActiveType.Passive;
	public MasteryInfo_New.eMethodType methodType = MasteryInfo_New.eMethodType.None;
	public string methodArg = "";
	
	public int maxPoint = 0;
	
	public int needGroupID = 0;
	public int needGroupPoint = 0;
	public List<int> needMasteryIDs = new List<int>();
	
	//public BaseState.eState attackState = BaseState.eState.None;
	
	public string name = "";
	public string desc = "";
	public string iconName = "";
	
	public string formatString = "";
	public string unitString = "";
	
	public float incValue = 0.0f;
}

public class MasteryInfoTable : BaseTable {
	public Dictionary<int, MasteryTableInfo_New> dataList = new Dictionary<int, MasteryTableInfo_New>();
	
	public MasteryTableInfo_New GetData(int id)
	{
		MasteryTableInfo_New info = null;
		if (dataList != null && dataList.ContainsKey(id) == true)
			info = dataList[id];
		
		return info;
	}
	
	public override void LoadTable(CSVDB db)
	{
		if (db != null)
		{
			int id = 0;
			MasteryTableInfo_New info = null;
			
			string infoStr = "";
			
			foreach(var data in db.data)
			{
				id = int.Parse(data.Key);
				
				info = new MasteryTableInfo_New();
				info.id = id;
				
				info.name = data.Value.GetValue("Name").ToText();
				info.groupID = data.Value.GetValue("GroupID").ToInt();
				info.iconName = data.Value.GetValue("IconName").ToText();
				
				infoStr= data.Value.GetValue("Type_1").ToText();
				info.activeType = MasteryInfo_New.ToMasteryActiveType(infoStr);

				infoStr = data.Value.GetValue("Type_2").ToText();
				info.methodType = MasteryInfo_New.ToMasteryMethodType(infoStr);
				info.methodArg = data.Value.GetValue("MethodArg").ToText();
				
				info.maxPoint = data.Value.GetValue("MaxPoint").ToInt();
				
				info.needGroupID = data.Value.GetValue("Need_GroupID").ToInt();
				info.needGroupPoint = data.Value.GetValue("Need_Group_Point").ToInt();
				
				infoStr = data.Value.GetValue("NeedIDs").ToText();
				string[] needIDs = infoStr.Split(';');
				foreach(string needIDStr in needIDs)
				{
					if (needIDStr != "")
					{
						int needID = int.Parse(needIDStr);
						if (needID != 0)
							info.needMasteryIDs.Add(needID);
					}
				}
				
				/*
				infoStr = data.Value.GetValue("AttackState").ToText();
				info.attackState = BaseState.ToState(infoStr);
				*/
				
				info.formatString = data.Value.GetValue("FormatString").ToText();
				info.incValue = data.Value.GetValue("IncValue").ToFloat();
				info.unitString = data.Value.GetValue("UnitString").ToText();
				
				info.desc = data.Value.GetValue("Desc").ToText();
				
				infoStr = data.Value.GetValue("IncValue").ToText();
				if (infoStr != "")
					info.incValue = float.Parse(infoStr);
				
				dataList.Add(id, info);
			}
		}
		
	}
}
