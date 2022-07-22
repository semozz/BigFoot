using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AchievementTable : BaseTable {
	public Dictionary<int, Achievement> achievements = new Dictionary<int, Achievement>();
	
	public Achievement GetData(int id)
	{
		Achievement achieve = null;
		if (achievements.ContainsKey(id) == true)
			achieve = achievements[id];
		
		return achieve;
	}
	
	public Achievement GetTempData(int id)
	{
		Achievement temp = GetData(id);
		Achievement info = null;
		if (temp != null)
			info = new Achievement(temp);
		
		return info;
	}
	
	public override void LoadTable(CSVDB db)
	{
		if (db != null)
		{
			int prevLimitCount = 0;
			int targetCount = 0;
			
			foreach(var data in db.data)
			{
				int id = 0;
				int step = 0;
				
				//int limitCount = 0;
				
				id = data.Value.GetValue("GroupID").ToInt();
				step = data.Value.GetValue("Step").ToInt();
				
				//charIndex = data.Value.GetValue("CharIndex").ToInt();
				
				string title = data.Value.GetValue("Title").ToText();
				string desc = data.Value.GetValue("Description").ToText();
				
				int isShare = data.Value.GetValue("Share").ToInt();
				
				targetCount = data.Value.GetValue("TargetCount").ToInt();
				
				string typeStr = "";
				Achievement.eAchievementType type = Achievement.eAchievementType.None;
				ValueData _value = data.Value.GetValue("AchieveType");
				
				if (_value != null)
				{
					typeStr = _value.ToText();
					type = Achievement.ToType(typeStr);
				}
				
				bool isStepLimit = false;
				_value = data.Value.GetValue("StepLimit");
				if (_value != null)
					isStepLimit = _value.ToInt() == 1;
				
				string typeArgs = "";
				_value = data.Value.GetValue("Args");
				if (_value != null)
					typeArgs = _value.ToText();
				
				_value = data.Value.GetValue("repeat");
				int repeatType = 0;
				if (_value != null)
					repeatType = _value.ToInt();
				else
					repeatType = 0;
				
				Achievement achieve = null;
				if (achievements.ContainsKey(id) == false)
				{
					prevLimitCount = 0;
					
					achieve = new Achievement();
					achievements.Add(id, achieve);
					
					achieve.id = id;
					achieve.title = title;
					achieve.desc = desc;
					
					achieve.isShare = isShare;
					
					achieve.isStepLimit = isStepLimit;
					
					achieve.type = type;
					achieve.isStepLimit = isStepLimit;
					
					if (isStepLimit == true)
						achieve.AddStepLimitInfo(prevLimitCount, targetCount, typeArgs);
					else
						achieve.argValues = typeArgs;
					
					achieve.repeatType = repeatType;
				}
				else
				{
					achieve = achievements[id];
					
					if (achieve.isStepLimit == true)
						achieve.AddStepLimitInfo(prevLimitCount, targetCount, typeArgs);
				}
				
				AchievementReward reward = new AchievementReward();
				
				string fieldName1 = "";
				string fieldName2 = "";
				
				//int _intValue = 0;
				for(int index = 0; ; ++index)
				{
					fieldName1 = string.Format("RewardType{0}", index);
					fieldName2 = string.Format("RewardID{0}", index);
					
					ValueData _tempValue1 = data.Value.GetValue(fieldName1);
					ValueData _tempValue2 = data.Value.GetValue(fieldName2);
					if (_tempValue1 == null || _tempValue2 == null)
						break;
					
					int rewardType = _tempValue1.ToInt();
					int intValue = _tempValue2.ToInt();
					switch(rewardType)
					{
					case 1:
						reward.rewardGold.x += (float)intValue;
						break;
					case 2:
						reward.rewardGold.y += (float)intValue;
						break;
					case 3:
						if (intValue != 0)
							reward.rewardItemIDs.Add(intValue);
						break;
					case 7:
						reward.awaken = intValue;
						break;
					}
				}
				
				reward.stepID = step;
				reward.limitCount = targetCount;
				
				switch(type)
				{
				case Achievement.eAchievementType.eLevelUp:
					reward.prevLimitCount = reward.limitCount - 1;
					break;
				case Achievement.eAchievementType.eArenaStraightVic:
					reward.prevLimitCount = reward.limitCount - 1;
					break;
				default:
					reward.prevLimitCount = prevLimitCount;
					prevLimitCount += reward.limitCount;
					break;
				}
				
				reward.desc = desc;
				
				if (achieve != null && reward != null)
					achieve.AddReward(reward);
			}
		}
	}
	
}
