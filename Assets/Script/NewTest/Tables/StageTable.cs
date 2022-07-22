using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BasicStageInfo
{
	public string stageName;
	public int stageID;
	public int needStamina;
	
	public int ticket;
	public int limitLevel;
	public int properLevel;
	
	public List<string> mobFaceList = new List<string>();
}

public class StageTableInfo
{
	public Dictionary<int, BasicStageInfo> stageInfos = new Dictionary<int, BasicStageInfo>();
	
	public string kingdom = "";
	public string stageNumber = "";
	public string actName = "";
	public string chapterName = "";
	
	public void AddStageInfo(int stageType, BasicStageInfo info)
	{
		if (stageInfos.ContainsKey(stageType) == false)
			stageInfos.Add(stageType, info);
	}
	
	public BasicStageInfo GetBasicStageInfo(int stageType)
	{
		BasicStageInfo info = null;
		if (stageInfos.ContainsKey(stageType) == true)
			info = stageInfos[stageType];
	
		return info;
	}
}

public class StageTable : BaseTable {
	public Dictionary<int, StageTableInfo> dataList = new Dictionary<int, StageTableInfo>();
	
	public StageTableInfo GetData(int id)
	{
		StageTableInfo data = null;
		if (dataList != null && dataList.ContainsKey(id) == true)
			data = dataList[id];
		
		return data;
	}
	
	public override void LoadTable(CSVDB db)
	{
		if (db != null)
		{
			StageTableInfo stageInfo = null;
			
			BasicStageInfo basicStageInfo = null;
			int stageType = -1;
			int stageID = -1;
			
			foreach(var data in db.data)
			{
				if (data.Key == "")
					break;
				
				stageID = data.Value.GetValue("StageID").ToInt();
				if (dataList.ContainsKey(stageID) == false)
				{
					stageInfo = new StageTableInfo();
					dataList.Add(stageID, stageInfo);
					
					stageInfo.kingdom = data.Value.GetValue("Kingdom").ToText();
					stageInfo.stageNumber = data.Value.GetValue("Number").ToText();
					stageInfo.actName = data.Value.GetValue("Act").ToText();
					stageInfo.chapterName = data.Value.GetValue("Chapter").ToText();
				}
				else
					stageInfo = dataList[stageID];
				
				stageType = data.Value.GetValue("StageType").ToInt();
				
				basicStageInfo = new BasicStageInfo();
				
				basicStageInfo.stageID = data.Value.GetValue("Stage_No").ToInt();
				basicStageInfo.stageName = data.Value.GetValue("StageName").ToText();
				
				basicStageInfo.needStamina = data.Value.GetValue("Stamina").ToInt();
				basicStageInfo.ticket = data.Value.GetValue("Ticket").ToInt();
				basicStageInfo.limitLevel = data.Value.GetValue("Limit_LV").ToInt();
				basicStageInfo.properLevel = data.Value.GetValue("Suggest_LV").ToInt();
				
				int index = 1;
				string faceInfo = "";
				for(; ; ++index)
				{
					ValueData faceData = data.Value.GetValue("Mob_" + index);
					if (faceData != null)
					{
						faceInfo = faceData.ToText();
						if (faceInfo != "")
							basicStageInfo.mobFaceList.Add(faceInfo);
					}
					else
						break;
				}
				
				stageInfo.AddStageInfo(stageType, basicStageInfo);
			}
		}
		
	}
}
