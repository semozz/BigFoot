using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StageReward
{
	public List<int> rewardItemIDs = new List<int>();
	public long stageExp = 0L;
	
	public void AddRewardItemID(int id)
	{
		rewardItemIDs.Add(id);
	}
	
	public int GetRandItem()
	{
		int nCount = rewardItemIDs.Count;
		int itemID = -1;
		
		if (nCount > 0)
		{
			int ranIndex = Random.Range(0, nCount);
			
			itemID = rewardItemIDs[ranIndex];
		}
		
		return itemID;
	}
}

public class StageRewardInfo
{
	public Dictionary<int, StageReward> stageRewardInfos = new Dictionary<int, StageReward>();
	
	public long GetExp(int stageType)
	{
		StageReward stageReward = GetStageReward(stageType);
		
		long expValue = 0L;
		if (stageReward != null)
			expValue = stageReward.stageExp;
		
		return expValue;
	}
	public void SetExp(int stageType, long exp)
	{
		StageReward stageReward = GetStageReward(stageType);
		if (stageReward == null)
		{
			stageReward = new StageReward();
			stageRewardInfos.Add(stageType, stageReward);
		}
		
		stageReward.stageExp = exp;
	}
	
	public StageReward GetStageReward(int stageType)
	{
		StageReward stageReward = null;
		if (stageRewardInfos.ContainsKey(stageType) == true)
			stageReward = stageRewardInfos[stageType];
		
		return stageReward;
	}
	
	public void AddRewardItemID(int stageType, int id)
	{
		StageReward stageReward = GetStageReward(stageType);
		
		if (stageReward != null)
			stageReward.AddRewardItemID(id);
	}
	
	public int GetRandItem(int stageType)
	{
		int itemID = -1;
		
		StageReward stageReward = GetStageReward(stageType);
		if (stageReward != null)
			itemID = stageReward.GetRandItem();
		
		return itemID;
	}
}

public class StageRewardTable : BaseTable
{
	public Dictionary<int, StageRewardInfo> dataList = new Dictionary<int, StageRewardInfo>();
	
	public StageRewardInfo GetData(int id)
	{
		StageRewardInfo data = null;
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
			StageRewardInfo rewardInfo = null;
			
			int itemID = -1;
			ValueData valueData = null;
			long expValue = 0L;
			
			string fieldName = "";
			
			foreach(var data in db.data)
			{
				if (data.Key == "")
					break;
				
				id = int.Parse(data.Key);
				
				rewardInfo = new StageRewardInfo();
				
				StageReward stageReward = new StageReward();
				
				itemID = -1;
				for (int index = 1; ; ++index)
				{
					fieldName = string.Format("Item{0}", index);
					valueData = data.Value.GetValue(fieldName);
					
					if (valueData == null)
						break;
					
					itemID = valueData.ToInt();
					if (itemID > 0)
						stageReward.AddRewardItemID(itemID);
				}
				rewardInfo.stageRewardInfos.Add(0, stageReward);
				
				stageReward = new StageReward();
				itemID = -1;
				for (int index = 1; ; ++index)
				{
					fieldName = string.Format("Item{0}", index);
					valueData = data.Value.GetValue(fieldName);
					
					if (valueData == null)
						break;
					
					itemID = valueData.ToInt();
					if (itemID > 0)
						stageReward.AddRewardItemID(itemID);
				}
				rewardInfo.stageRewardInfos.Add(1, stageReward);
				
				stageReward = new StageReward();
				itemID = -1;
				for (int index = 1; ; ++index)
				{
					fieldName = string.Format("HellItem{0}", index);
					valueData = data.Value.GetValue(fieldName);
					
					if (valueData == null)
						break;
					
					itemID = valueData.ToInt();
					if (itemID > 0)
						stageReward.AddRewardItemID(itemID);
				}
				rewardInfo.stageRewardInfos.Add(2, stageReward);
				
				expValue = 0L;
				
				for (int stageType = 0; ; ++stageType)
				{
					fieldName = string.Format("EXP_{0}", stageType);
					valueData = data.Value.GetValue(fieldName);
					
					if (valueData == null)
						break;
					
					expValue = valueData.ToLong();
					rewardInfo.SetExp(stageType, expValue);
				}
				
				this.dataList.Add(id, rewardInfo);
			}
		}
	}
}
