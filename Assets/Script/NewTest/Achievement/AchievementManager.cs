using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AchievementManager {

	public Dictionary<int, Achievement> commonAchievements = new Dictionary<int, Achievement>();
	public Dictionary<int, Achievement>[] privateAchievements = new Dictionary<int, Achievement>[3];
	
	public System.DateTime dailyAchievementExpireTime = System.DateTime.Now;
	public Dictionary<int, Achievement> dailyAchivements = new Dictionary<int, Achievement>();
	public Achievement dailyCheckAchievement = null;
	
	public Dictionary<int, Achievement> completeList = new Dictionary<int, Achievement>();
	
	public Dictionary<int, Achievement> commonSpecialAchievements = new Dictionary<int, Achievement>();
	public Dictionary<int, Achievement>[] privateSpecialAchievements = new Dictionary<int, Achievement>[3];
	public Dictionary<int, Achievement> completeSpecialAchievements = new Dictionary<int, Achievement>();
	
	public AchievementManager()
	{
		privateAchievements[0] = new Dictionary<int, Achievement>();
		privateAchievements[1] = new Dictionary<int, Achievement>();
		privateAchievements[2] = new Dictionary<int, Achievement>();
		
		privateSpecialAchievements[0] = new Dictionary<int, Achievement>();
		privateSpecialAchievements[1] = new Dictionary<int, Achievement>();
		privateSpecialAchievements[2] = new Dictionary<int, Achievement>();
	}
	
	public void AddAchievement(int id, Achievement info)
	{
		if (commonAchievements.ContainsKey(id) == false)
			commonAchievements.Add(id, info);
	}
	
	public Achievement GetAchievement(int id, int charIndex)
	{
		Achievement achievement = null;
		Dictionary<int, Achievement> list = null;
		if (charIndex == -1)
			list = commonAchievements;
		else
		{
			list = privateAchievements[charIndex];
		}
		
		if (list.ContainsKey(id) == true)
			achievement = list[id];
		
		/*
		if (commonAchievements.ContainsKey(id) == true)
			achievement = commonAchievements[id];
		else
		{
			if (privateAchievements[charIndex].ContainsKey(id) == true)
				achievement = privateAchievements[charIndex][id];
		}
		*/
		
		return achievement;
	}
	
	public void AddDailyAchievement(int id, Achievement achieve)
	{
		if (achieve == null)
			return;
		
		if (achieve.type == Achievement.eAchievementType.eDailyAchieveComplete)
		{
			dailyCheckAchievement = achieve;
		}
		else
		{
			if (dailyAchivements.ContainsKey(id) == false)
				dailyAchivements.Add(id, achieve);
		}
	}
	
	public Achievement GetDailyAchievement(int id)
	{
		Achievement achievement = null;
		
		if (dailyCheckAchievement != null && dailyCheckAchievement.id == id)
			achievement = dailyCheckAchievement;
		else
		{
			if (dailyAchivements.ContainsKey(id) == true)
				achievement = dailyAchivements[id];
		}
		
		return achievement;
	}
	
	public List<Achievement> GetDailyAchievementList()
	{
		List<Achievement> list = new List<Achievement>();
		int completeCount = 0;
		foreach(var temp in dailyAchivements)
		{
			Achievement achieve = temp.Value;
			if (achieve != null && achieve.isComplete && achieve.type != Achievement.eAchievementType.eDailyAwakenPoint)
				completeCount++;
			
			list.Add(achieve);
		}
		
		if (dailyCheckAchievement != null)
		{
			dailyCheckAchievement.addCount = Mathf.Max(completeCount - dailyCheckAchievement.curCount, 0);
			list.Add(dailyCheckAchievement);
		}
		
		return list;
	}
	
	public void ApplyAchievement(Achievement.eAchievementType type, int charIndex, int id, int _value)
	{
		//공유 업적..
		foreach(var temp in commonAchievements)
		{
			Achievement achieve = temp.Value;
			if (achieve.type == type && (achieve.charIndex == -1 || achieve.charIndex == charIndex))
				ApplyAchievement(achieve, charIndex, id, _value); 
		}
		
		//개인 업적..
		int nCount = privateAchievements.Length;
		if (charIndex >= 0 && charIndex < nCount)
		{
			foreach(var temp in privateAchievements[charIndex])
			{
				Achievement achieve = temp.Value;
				if (achieve.type == type && (achieve.charIndex == -1 || achieve.charIndex == charIndex))
					ApplyAchievement(achieve, charIndex, id, _value); 
			}
		}
		
		UpdateDailyMissionTime();
		foreach(var temp1 in dailyAchivements)
		{
			Achievement dailyAchieve = temp1.Value;
			if (dailyAchieve.type == type && (dailyAchieve.charIndex == -1 || dailyAchieve.charIndex == charIndex))
			{
				bool prevCheck = dailyAchieve.CheckCondition();
				
				ApplyAchievement(dailyAchieve, charIndex, id, _value);
				
				bool curCheck = dailyAchieve.CheckCondition();
				
				if (dailyCheckAchievement != null &&
					prevCheck == false && curCheck == true)
					ApplyAchievement(dailyCheckAchievement, charIndex,id, 0);
			}
		}
		
		//스페셜 이벤트 업적.
		foreach(var temp in commonSpecialAchievements)
		{
			Achievement achieve = temp.Value;
			if (achieve.type == type && (achieve.charIndex == -1 || achieve.charIndex == charIndex))
				ApplyAchievement(achieve, charIndex, id, _value); 
		}
		
		nCount = privateSpecialAchievements.Length;
		if (charIndex >= 0 && charIndex < nCount)
		{
			foreach(var temp in privateSpecialAchievements[charIndex])
			{
				Achievement achieve = temp.Value;
				if (achieve.type == type && (achieve.charIndex == -1 || achieve.charIndex == charIndex))
					ApplyAchievement(achieve, charIndex, id, _value); 
			}
		}
	}
	
	public void ApplyAchievement(Achievement.eAchievementType type, int charIndex, int _value)
	{
		ApplyAchievement(type, charIndex, -1, _value);
	}
	
	public void SendUpdateAchievementInfo(int charIndex)
	{
		List<Achievement> updateList = new List<Achievement>();
		foreach(var temp in commonAchievements)
		{
			Achievement achieve = temp.Value;
			if (achieve == null || achieve.addCount == 0)
				continue;
			
			updateList.Add(achieve);
		}
		
		foreach(var temp in privateAchievements[charIndex])
		{
			Achievement achieve = temp.Value;
			if (achieve == null || achieve.addCount == 0)
				continue;
			
			updateList.Add(achieve);
		}
		
		IPacketSender sender = Game.Instance.packetSender;
		
		if (updateList.Count > 0)
		{
			if (sender != null)
				sender.SendAchievementProcess(updateList);
			
			UpdateAchievementCount(updateList);
		}
		
		
		updateList.Clear();
		foreach(var temp in dailyAchivements)
		{
			Achievement achieve = temp.Value;
			if (achieve == null || achieve.addCount == 0)
				continue;
			
			updateList.Add(achieve);
		}
		
		if (dailyCheckAchievement != null && dailyCheckAchievement.addCount > 0)
			updateList.Add(dailyCheckAchievement);
		
		if (updateList.Count > 0)
		{
			if (sender != null)
				sender.SendDailyAchievementProcess(updateList);
			
			UpdateAchievementCount(updateList);
		}
		
		//스페셜 이벤트 업적..
		updateList.Clear();
		foreach(var temp in commonSpecialAchievements)
		{
			Achievement achieve = temp.Value;
			if (achieve == null || achieve.addCount == 0)
				continue;
			
			updateList.Add(achieve);
		}
		
		foreach(var temp in privateSpecialAchievements[charIndex])
		{
			Achievement achieve = temp.Value;
			if (achieve == null || achieve.addCount == 0)
				continue;
			
			updateList.Add(achieve);
		}
		
		if (updateList.Count > 0)
		{
			if (sender != null)
				sender.SendSpecialAchievementProcess(updateList);
			
			UpdateAchievementCount(updateList);
		}
	}
	
	public void UpdateAchievementCount(List<Achievement> updateList)
	{
		foreach(Achievement achieve in updateList)
		{
			achieve.curCount += achieve.addCount;
			achieve.addCount = 0;
		}
	}
	
	public void ApplyAchievement(Achievement achieve, int charIndex, int id, int _value)
	{
		if (achieve == null)
			return;
		
		achieve.ApplyAchievement(charIndex, id, _value);
	}
	
	public void InitData()
	{
		dailyAchivements.Clear();
		commonAchievements.Clear();
		completeList.Clear();
		
		TableManager tableManager = TableManager.Instance;
		AchievementTable achievementTable = tableManager != null ? tableManager.normalAchievementTable : null;
		
		if (achievementTable != null)
		{
			foreach(var temp in achievementTable.achievements)
			{
				Achievement newAchieve = null;
				
				if (temp.Value.isShare == 1)
				{
					newAchieve = new Achievement(temp.Value);
					newAchieve.charIndex = -1;
					
					commonAchievements.Add(temp.Key, newAchieve);
				}
				else
				{
					int nCount = privateAchievements.Length;
					
					Dictionary<int, Achievement> tempList = null;
					
					for (int index = 0; index < nCount; ++index)
					{
						newAchieve = new Achievement(temp.Value);
						newAchieve.charIndex = index;
						
						//privateAchievements[index].Add(temp.Key, newAchieve);
						tempList = privateAchievements[index];//.Add(temp.Key, newAchieve);\
						
						if (tempList != null)
						{
							if (tempList.ContainsKey(temp.Key) == false)
								tempList.Add(temp.Key, newAchieve);
							else
								tempList[temp.Key] = newAchieve;
						}
						
					}
				}
			}
		}
	}
	
	public List<Achievement> GetAchievementListIncludeSpecial(int charIndex, bool bDailAchieve)
	{
		List<Achievement> achieveList = GetAchievementList(charIndex, bDailAchieve);
		
		Achievement achieve = null;
		if (commonSpecialAchievements != null && commonSpecialAchievements.Count > 0)
		{
			if (achieveList == null)
				achieveList = new List<Achievement>();
			
			foreach(var temp in commonSpecialAchievements)
			{
				achieve =  temp.Value;
				achieveList.Add(achieve);
			}
		}
		
		int nCount = 0;
		if (privateSpecialAchievements != null)
			nCount = privateSpecialAchievements.Length;
		
		if (charIndex >= 0 && charIndex < nCount)
		{
			Dictionary<int, Achievement> privateList = privateSpecialAchievements[charIndex];
			
			if (privateList != null && privateList.Count > 0)
			{
				if (achieveList == null)
					achieveList = new List<Achievement>();
				
				//개인 업적..
				foreach(var temp in privateList)
				{
					achieve =  temp.Value;
					achieveList.Add(achieve);
				}
			}
		}
		
		return achieveList;
	}
	
	public List<Achievement> GetAchievementList(int charIndex, bool bDailyAchieve)
	{
		List<Achievement> achieveList = new List<Achievement>();
		
		Achievement achieve = null;
		foreach(var temp in commonAchievements)
		{
			achieve =  temp.Value;
			achieveList.Add(achieve);
		}
		
		int nCount = privateAchievements.Length;
		if (charIndex >= 0 && charIndex < nCount)
		{
			//개인 업적..
			foreach(var temp in privateAchievements[charIndex])
			{
				achieve =  temp.Value;
				achieveList.Add(achieve);
			}
		}
		
		if (bDailyAchieve == true)
		{
			foreach(var temp in dailyAchivements)
			{
				achieve = temp.Value;
				achieveList.Add(achieve);
			}
		}
		
		return achieveList;
	}
	
	public List<Achievement> GetCompleteList(int charIndex)
	{
		List<Achievement> achieveList = new List<Achievement>();
		
		Achievement achieve = null;
		foreach(var temp in completeList)
		{
			achieve =  temp.Value;
			
			if (achieve.charIndex != -1 && achieve.charIndex != charIndex)
				continue;
			
			achieveList.Add(achieve);
		}
		
		return achieveList;
	}

    public void SetClearInfo(int characterIndex, int clearStep, int groupID)
    {
        Achievement achieve = GetAchievement(groupID, characterIndex);

        if (achieve == null)
            return;

        if (achieve.charIndex != -1 && achieve.charIndex != characterIndex)
            return;

        int rewardCount = achieve.achievementRewards.Count;

        achieve.SetClearStep(clearStep);

        //achieve.curRewardStep = Mathf.Min(clearStep, rewardCount - 1);

        achieve.isComplete = (rewardCount == clearStep);
        //전부 보상 받은 업적은 리스트 변경..
        if (achieve.isComplete == true)
        {
            if (completeList.ContainsKey(groupID) == false)
                completeList.Add(groupID, achieve);
            else
                completeList[groupID] = achieve;

            if (achieve.charIndex == -1)
                commonAchievements.Remove(groupID);
            else
            {
                int tempCount = privateAchievements.Length;

                if (achieve.charIndex >= 0 && achieve.charIndex < tempCount)
                    privateAchievements[achieve.charIndex].Remove(groupID);
            }
        }
    }

	
	public void SetClearInfo(AchievementClearInfo Info)
	{
		int nCount = Mathf.Min(Info.groupIDs.Length, Info.stepIDs.Length);
		for (int index = 0; index < nCount; ++index)
		{
			int groupID = Info.groupIDs[index];
			int clearStep = Info.stepIDs[index];

            SetClearInfo(Info.characterIndex, clearStep, groupID);
		}
	}
	
	public void SetAchieveInfo(int charIndex, int[] groupIDs, int[] counts)
	{
		int nCount = Mathf.Min(groupIDs.Length, counts.Length);
		for (int index = 0; index < nCount; ++index)
		{
			int groupID = groupIDs[index];
			int achieveCount = counts[index];
			
			Achievement achieve = GetAchievement(groupID, charIndex);
			
			if (achieve == null)
				continue;
			
			if (achieve.charIndex != -1 && achieve.charIndex != charIndex)
				continue;
			
			achieve.curCount = achieveCount;
			achieve.addCount = 0;
		}
	}

    public void SetAchieveInfo(int charIndex, int groupID, int achieveCount)
    {
        Achievement achieve = GetAchievement(groupID, charIndex);

        if (achieve == null)
            return;

        if (achieve.charIndex != -1 && achieve.charIndex != charIndex)
            return;

        achieve.curCount = achieveCount;
        achieve.addCount = 0;
    }
	
	public void SetAchieveInfo(AchievementDBInfo Info)
	{
		int nCount = Mathf.Min(Info.groupIDs.Length, Info.counts.Length);
		for (int index = 0; index < nCount; ++index)
		{
			int groupID = Info.groupIDs[index];
			int achieveCount = Info.counts[index];
			
			Achievement achieve = GetAchievement(groupID, Info.characterIndex);
			
			if (achieve == null)
				continue;
			
			if (achieve.charIndex != -1 && achieve.charIndex != Info.characterIndex)
				continue;
			
			achieve.curCount = achieveCount;
			achieve.addCount = 0;
		}
	}
	
	public void SetCompleteReward(int charIndex, int groupID, int stepID)
	{
		Achievement achieve = GetAchievement(groupID, charIndex);
			
		if (achieve == null)
			return;
		
		if (achieve.charIndex != -1 && achieve.charIndex != charIndex)
			return;
		
		if (achieve.repeatType == 1)
			achieve.ResetClearStep();
		else
			achieve.SetClearStep(stepID);
	}
	
	public void SetCompleteDailyAchieve(int groupID)
	{
		Achievement achieve = GetDailyAchievement(groupID);
		
		if (achieve == null)
			return;
		
		achieve.SetClearStep(1);
		//achieve.curRewardStep += 1;
	}
	
	public void SetDailyAchieve(int[] groupIDs, int[] counts)
	{
		int nCount = Mathf.Min(groupIDs.Length, counts.Length);
		for (int index = 0; index < nCount; ++index)
		{
			int groupID = groupIDs[index];
			int achieveCount = counts[index];
			
			Achievement achieve = GetDailyAchievement(groupID);
			
			if (achieve == null)
				continue;
			
			achieve.curCount = achieveCount;
			achieve.addCount = 0;
		}
	}
	
	public void UpdateDailyMissionTime()
	{
		System.DateTime nowTime = System.DateTime.Now;
		System.TimeSpan timeSpan = this.dailyAchievementExpireTime - nowTime;
		if (timeSpan.TotalSeconds <= 0)
			dailyAchivements.Clear();
	}
	
	public bool CheckRewardList(List<Achievement> achievementList)
	{
		bool isRewardEnable = false;
		
		foreach(Achievement achievement in achievementList)
		{
			if (achievement.isComplete == true)
				continue;
			
			AchievementReward reward = achievement.GetCurReward();
		
			int achieveCount = -1;
			int prevLimitCount = 0;
			int limitCount = 0;
			
			if (reward != null)
			{
				limitCount = reward.limitCount;
				prevLimitCount = reward.prevLimitCount;
				
				switch(achievement.type)
				{
				case Achievement.eAchievementType.eLevelUp:
					achieveCount = achievement.curCount + achievement.addCount;
					break;
				case Achievement.eAchievementType.eArenaStraightVic:
					achieveCount = achievement.curCount + achievement.addCount;
					break;
				default:
					achieveCount = achievement.curCount + achievement.addCount - prevLimitCount;
					break;
				}
			}
			
			if (reward != null && achieveCount >= limitCount)
			{
				isRewardEnable = true;
				break;
			}
		}
		
		return isRewardEnable;
	}
	
	public void ClearSpecialAchievements()
	{
		this.commonSpecialAchievements.Clear();
		privateSpecialAchievements[0].Clear();
		privateSpecialAchievements[1].Clear();
		privateSpecialAchievements[2].Clear();
		this.completeSpecialAchievements.Clear();
	}
	
	public void SetSpeicalAchieveInfo(SpecialMissionDBInfo Info)
	{
		int nCount = Mathf.Min(Info.groupIDs.Length, Info.counts.Length);
		for (int index = 0; index < nCount; ++index)
		{
			int groupID = Info.groupIDs[index];
			int achieveCount = Info.counts[index];
			int charIndex = Info.characterIndexs[index];
			
			Achievement achieve = GetSpecialAchievement(charIndex, groupID);
			
			if (achieve == null)
				continue;
			
			if (achieve.charIndex != -1 && achieve.charIndex != charIndex)
				continue;
			
			achieve.curCount = achieveCount;
			achieve.addCount = 0;
		}
	}
	
	public Achievement GetSpecialAchievement(int charIndex, int groupID)
	{
		Achievement achievement = null;
		Dictionary<int, Achievement> list = null;
		if (charIndex == -1)
			list = commonSpecialAchievements;
		else
		{
			list = privateSpecialAchievements[charIndex];
		}
		
		if (list.ContainsKey(groupID) == true)
			achievement = list[groupID];
		
		return achievement;
	}
	
	public void InitSpecialAchievements()
	{
		TableManager tableManager = TableManager.Instance;
		AchievementTable specialAchievementTable = tableManager != null ? tableManager.specialEventAchievementTable : null;
		
		if (specialAchievementTable != null)
		{
			foreach(var temp in specialAchievementTable.achievements)
			{
				Achievement newAchieve = null;
				
				if (temp.Value.isShare == 1)
				{
					newAchieve = new Achievement(temp.Value);
					newAchieve.charIndex = -1;
					
					commonSpecialAchievements.Add(temp.Key, newAchieve);
				}
				else
				{
					int nCount = privateSpecialAchievements.Length;
					
					Dictionary<int, Achievement> tempList = null;
					
					for (int index = 0; index < nCount; ++index)
					{
						newAchieve = new Achievement(temp.Value);
						newAchieve.charIndex = index;
						
						tempList = privateSpecialAchievements[index];//.Add(temp.Key, newAchieve);\
						
						if (tempList != null)
						{
							if (tempList.ContainsKey(temp.Key) == false)
								tempList.Add(temp.Key, newAchieve);
							else
								tempList[temp.Key] = newAchieve;
						}
					}
				}
			}
		}
	}
	
	public void SpecialAchievementComplete(int charIndex, int groupID, int clearStep)
	{
		Achievement achieve = GetSpecialAchievement(charIndex, groupID);
			
		if (achieve == null)
			return;
		
		if (achieve.charIndex != -1 && achieve.charIndex != charIndex)
			return;
		
		int rewardCount = achieve.achievementRewards.Count;
		
		achieve.SetClearStep(clearStep);
		achieve.isComplete = (rewardCount == clearStep);
		
		//전부 보상 받은 업적은 리스트 변경..
		if (achieve.isComplete == true)
		{
			if (completeSpecialAchievements.ContainsKey(groupID) == false)
				completeSpecialAchievements.Add(groupID, achieve);
			else
				completeSpecialAchievements[groupID] = achieve;
			
			if (achieve.charIndex == -1)
				this.commonSpecialAchievements.Remove(groupID);
			else
			{
				int tempCount = this.privateSpecialAchievements.Length;
				
				if (achieve.charIndex >= 0 && achieve.charIndex < tempCount)
					privateSpecialAchievements[achieve.charIndex].Remove(groupID);
			}
		}
	}
	
	public void SpecialAchievementUpdate(int charIndex, int groupID, int count)
	{
		Achievement achieve = GetSpecialAchievement(charIndex, groupID);
			
		if (achieve == null)
			return;
		
		achieve.curCount = count;
		achieve.addCount = 0;
	}
	
	public void SetCompleteSpecialAchive(int charIndex, int groupID, int clearStep)
	{
		Achievement achieve = GetSpecialAchievement(charIndex, groupID);
			
		if (achieve == null)
			return;
		
		if (achieve.repeatType == 1)
			achieve.ResetClearStep();
		else
			achieve.SetClearStep(clearStep);
	}
	
	public List<Achievement> GetSpecialMissionList(int charIndex)
	{
		List<Achievement> list = new List<Achievement>();
		foreach(var temp in commonSpecialAchievements)
			list.Add(temp.Value);
		
		int nCount = this.privateSpecialAchievements.Length;
		if (charIndex >= 0 && charIndex < nCount)
		{
			//개인 업적..
			foreach(var temp in privateSpecialAchievements[charIndex])
				list.Add(temp.Value);
		}
		
		return list;
	}
	
	public List<Achievement> GetCompleteSpecialMissionList()
	{
		List<Achievement> list = new List<Achievement>();
		foreach(var temp in completeSpecialAchievements)
			list.Add(temp.Value);
		
		return list;
	}
}
