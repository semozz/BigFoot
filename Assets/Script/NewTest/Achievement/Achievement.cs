using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StepLimitInfo
{
	public string argValue = "";
	public int prevLimitCount = 0;
	public int targetCount = 0;
}

public class Achievement
{
	public enum eAchievementType
	{
		None,
		eStageClear,				//스테이지 클리어.
		eStageFailed,				//스테이지 실패.
		eRevival,					//부활 사용.
		eRescurePrisoner,		//포로 구출.
		eLevelUp,					//레벨업.
		eUseSkillPoint,			//특성 포인트 사용.
		eActiveSkill,				//액티브스킬 활성화.
		eResetSkillPoint,			//특성 초기화.
		eKillMonster,			//몬스터 잡기.
		eReinforceItem,			//아이템 강화.
		eCompositionSuccess,		//합성 성공.
		eCompositionFail,			//합성 실패.
		eGetBestItem,			//최상급 아이템 획득
		eUpgradeToLimit,		//최고 단계 아이템 강화/합성.
		eUseGamble,				//겜블 이용.
		eUpdateGamble,			//겜블 즉시 갱신.
		eBuyItem,					//아이템 구입.
		eSellItem,					//아이템 판매.
		eExpandItemSlot,		//창고 확장.
		eArenaWin,				//투기장 승리.
		eArenaFail,				//투기장 패배.
		eArenaStraightVic,		//투기장 연승.
		eArenaMedal,				//투기장 훈장.
		eBuyArenaItem,			//투기장 아이템 구입.
		eGetRank1,				//랭크 1 달성.
		eStageEnter,				//스테이지 진입.
		eUsePotion,				//포션 사용.
		eDefenceEnter,			//디펜스 시작.
		eArenaEnter,				//투기장 시작.
		eDailyAchieveComplete,	//일일 미션 완료.
		eDefenceComplete,		//디펜스 클리어.
		eSpecialEventItemGathering,	//특수 임무 아이템 모으기.
		eDailyAwakenPoint,		//일일 각성 포인트 얻기.
	}
	public eAchievementType type = eAchievementType.None;
	
	public int id = 0;
	
	public int curCount = 0;		//서버에서 받은 기본 값.
	public int addCount = 0;	//클라이언트에서 추가 되는 값 카운트.
	
	public string title = "";
	public string desc = "";
	
	public int charIndex = -1;
	
	public string argValues = "";
	
	public bool isStepLimit = false;
	public List<StepLimitInfo> stepArgValues = new List<StepLimitInfo>();
	public void AddStepLimitInfo(int prevLimitCount, int targetCount, string args)
	{
		StepLimitInfo info = new StepLimitInfo();
		info.prevLimitCount = prevLimitCount;
		info.targetCount = targetCount;
		info.argValue = args;
		
		stepArgValues.Add(info);
	}
	
	public List<AchievementReward> achievementRewards = new List<AchievementReward>();
	
	//public int curRewardStep = -1;
	
	public bool isComplete = false;
	public int isShare = 0;
	public int repeatType = 0;
	
	public static eAchievementType ToType(string typeStr)
	{
		eAchievementType type = eAchievementType.None;
		if (typeStr == "eStageClear")
			type = eAchievementType.eStageClear;
		else if (typeStr == "eStageFailed")
			type = eAchievementType.eStageFailed;
		else if (typeStr == "eRevival")
			type = eAchievementType.eRevival;
		else if (typeStr == "eRescurePrisoner")
			type = eAchievementType.eRescurePrisoner;
		else if (typeStr == "eLevelUp")
			type = eAchievementType.eLevelUp;
		else if (typeStr == "eUseSkillPoint")
			type = eAchievementType.eUseSkillPoint;
		else if (typeStr == "eActiveSkill")
			type = eAchievementType.eActiveSkill;
		else if (typeStr == "eResetSkillPoint")
			type = eAchievementType.eResetSkillPoint;
		else if (typeStr == "eKillMonster")
			type = eAchievementType.eKillMonster;
		else if (typeStr == "eReinforceItem")
			type = eAchievementType.eReinforceItem;
		else if (typeStr == "eCompositionSuccess")
			type = eAchievementType.eCompositionSuccess;
		else if (typeStr == "eCompositionFail")
			type = eAchievementType.eCompositionFail;
		else if (typeStr == "eGetBestItem")
			type = eAchievementType.eGetBestItem;
		else if (typeStr == "eUpgradeToLimit")
			type = eAchievementType.eUpgradeToLimit;
		else if (typeStr == "eUseGamble")
			type = eAchievementType.eUseGamble;
		else if (typeStr == "eUpdateGamble")
			type = eAchievementType.eUpdateGamble;
		else if (typeStr == "eBuyItem")
			type = eAchievementType.eBuyItem;
		else if (typeStr == "eSellItem")
			type = eAchievementType.eSellItem;
		else if (typeStr == "eExpandItemSlot")
			type = eAchievementType.eExpandItemSlot;
		else if (typeStr == "eArenaWin")
			type = eAchievementType.eArenaWin;
		else if (typeStr == "eArenaFail")
			type = eAchievementType.eArenaFail;
		else if (typeStr == "eArenaStraightVic")
			type = eAchievementType.eArenaStraightVic;
		else if (typeStr == "eArenaMedal")
			type = eAchievementType.eArenaMedal;
		else if (typeStr == "eBuyArenaItem")
			type = eAchievementType.eBuyArenaItem;
		else if (typeStr == "eGetRank1")
			type = eAchievementType.eGetRank1;
		else if (typeStr == "eStageEnter")
			type = eAchievementType.eStageEnter;
		else if (typeStr == "eUsePotion")
			type = eAchievementType.eUsePotion;
		else if (typeStr == "eDefenceEnter")
			type = eAchievementType.eDefenceEnter;
		else if (typeStr == "eArenaEnter")
			type = eAchievementType.eArenaEnter;
		else if (typeStr == "eDailyAchieveComplete")
			type = eAchievementType.eDailyAchieveComplete;
		else if (typeStr == "eDefenceComplete")
			type = eAchievementType.eDefenceComplete;
		else if (typeStr == "eSpecialEventItemGathering")
			type = eAchievementType.eSpecialEventItemGathering;
		else if (typeStr == "eDailyAwakenPoint")
			type = eAchievementType.eDailyAwakenPoint;
		return type;
	}
	
	public Achievement()
	{
		this.id = 0;
		this.curCount = this.addCount = 0;
		
		this.title = "";
		this.desc = "";
		
		this.charIndex = -1;
		
		//this.curRewardStep = 0;
		this.achievementRewards.Clear();
	}
	
	public Achievement(Achievement temp)
	{
		this.id = temp.id;
		this.curCount = this.addCount = 0;
		
		this.title = temp.title;
		this.desc = temp.desc;
		
		this.charIndex = temp.charIndex;
		
		//this.curRewardStep = 0;
		this.repeatType = temp.repeatType;
		
		this.type = temp.type;
		this.argValues = temp.argValues;
		this.isShare = temp.isShare;
		
		this.isStepLimit = temp.isStepLimit;
		if (temp.isStepLimit == true)
		{
			this.stepArgValues.Clear();
			
			foreach(StepLimitInfo info in temp.stepArgValues)
				this.AddStepLimitInfo(info.prevLimitCount, info.targetCount, info.argValue);
		}
		
		this.achievementRewards.Clear();
		foreach(AchievementReward tempReward in temp.achievementRewards)
		{
			AchievementReward reward = new AchievementReward(tempReward);
			this.achievementRewards.Add(reward);
		}
	}
	
	public void AddReward(AchievementReward reward)
	{
		this.achievementRewards.Add(reward);
	}
	
	public void ApplyAchievement(int charIndex, int id, int count)
	{
		switch(this.type)
		{
		case eAchievementType.eStageClear:
			ApplyStageClear(charIndex, count);
			break;
		case eAchievementType.eLevelUp:
			Debug.Log("LevelUp Acheivement... ");
			this.addCount = count;
			this.curCount = 0;
			break;
		case eAchievementType.eArenaStraightVic:
			ApplyArenaStraightVictory(charIndex);
			break;
		case eAchievementType.eSpecialEventItemGathering:
			ApplyAchievementGathering(id, count);
			break;
		case eAchievementType.eKillMonster:
			ApplyKillMonster(charIndex, id, count);
			break;
		default:
			this.addCount += 1;
			break;
		}
	}
	
	public void ApplyAchievement(int charIndex, int _value)
	{
		ApplyAchievement(charIndex, -1, _value);
	}
	
	public void ApplyAchievementGathering(int id, int count)
	{
		if (string.IsNullOrEmpty(this.argValues))
			return;
		
		string[] args = this.argValues.Split(',');
		int nCount = args.Length;
		bool bCheck = false;
		
		if (nCount > 0)
		{
			for (int index = 0; index < nCount; ++ index)
			{
				int checkID = int.Parse(args[index]);
				if (checkID != -1 && checkID == id)
				{
					bCheck = true;
					break;
				}
			}
		}
		
		if (bCheck == true)
			this.addCount += count;
	}
	
	public void ApplyArenaStraightVictory(int charIndex)
	{
		CharInfoData charData = Game.Instance.charInfoData;
		CharPrivateData privateData = null;
		if (charData != null)
			privateData = charData.GetPrivateData(charIndex);
		
		if (privateData != null && privateData.arenaInfo != null)
		{
			this.addCount = Mathf.Max(0, privateData.arenaInfo.winningStreakCount - this.curCount);
			//this.curCount = 0;
		}
	}
	
	public void ApplyStageClear(int charIndex, int _value)
	{
		CharInfoData charData = Game.Instance.charInfoData;
		CharPrivateData privateData = null;
		if (charData != null)
			privateData = charData.GetPrivateData(charIndex);
		
		string[] args = this.argValues.Split(',');
		int nCount = args.Length;
		if (nCount == 0)
			return;
		
		int stageType = 0;
		if (args[0] == "true")
			stageType = 0;
		else if (args[0] == "false")
			stageType = 1;
		else
			stageType = int.Parse(args[0]);
		
		List<StageInfo> stageInfoList = null;
		if (privateData != null && privateData.stageInfos.ContainsKey(stageType) == true)
			stageInfoList = privateData.stageInfos[stageType];
		
		int clearCount = 0;
		if (stageInfoList != null)
		{
			for (int index = 1; index < nCount; ++ index)
			{
				int stageIndex = int.Parse(args[index]);
				StageInfo info = privateData.GetStageInfo(stageInfoList, stageIndex -1 );
				
				if (info != null && info.stageInfo == StageButton.eStageButton.Clear)
					clearCount++;
			}
		}
		
		this.addCount = clearCount - this.curCount;
		if (this.addCount < 0)
			this.addCount = 0;
	}
	
	private void ApplyKillMonsterNormalMode(int charIndex, int type, int attributeID)
	{
		if (string.IsNullOrEmpty(this.argValues))
			return;
		
		string[] args = this.argValues.Split(',');
		int nCount = args.Length;
		bool bCheck = false;
		
		if (nCount > 0)
		{
			int stageType = int.Parse(args[0]);
			
			if (stageType != type)
				return;
			
			for (int index = 1; index < nCount; ++ index)
			{
				int monsterID = int.Parse(args[index]);
				if (monsterID == -1 || monsterID == attributeID)
				{
					bCheck = true;
					break;
				}
			}
		}
		
		if (bCheck == true)
			this.addCount += 1;
	}
	
	private void ApplyKillMonsterStepLimitMode(int charIndex, int type, int attributeID)
	{
		foreach(StepLimitInfo info in stepArgValues)
		{
			if (this.curCount < info.prevLimitCount || this.curCount >= (info.prevLimitCount + info.targetCount))
				continue;
			
			if (string.IsNullOrEmpty(info.argValue))
				return;
			
			string[] args = info.argValue.Split(',');
			int nCount = args.Length;
			bool bCheck = false;
			
			if (nCount > 0)
			{
				int stageType = int.Parse(args[0]);
				
				if (stageType != type)
					return;
				
				for (int index = 1; index < nCount; ++ index)
				{
					int monsterID = int.Parse(args[index]);
					if (monsterID == -1 || monsterID == attributeID)
					{
						bCheck = true;
						break;
					}
				}
			}
			
			if (bCheck == true)
				this.addCount += 1;
		}
		
	}
	
	public void ApplyKillMonster(int charIndex, int type, int attributeID)
	{
		if (isStepLimit == false)
			ApplyKillMonsterNormalMode(charIndex, type, attributeID);
		else
			ApplyKillMonsterStepLimitMode(charIndex, type, attributeID);
	}
	
	public void SetClearStep(int stepID)
	{
		foreach(AchievementReward temp in this.achievementRewards)
		{
			if (temp.stepID <= stepID)
				temp.isRewardComplete = true;
		}
		
		bool isComplete = true;
		foreach(AchievementReward temp in this.achievementRewards)
		{
			if (temp.isRewardComplete == false)
			{
				isComplete = false;
				break;
			}
		}
		
		this.isComplete = isComplete;
	}
	
	public void ResetClearStep()
	{
		foreach(AchievementReward temp in this.achievementRewards)
		{
			temp.isRewardComplete = false;
		}
		
		this.isComplete = false;
		
		this.curCount = 0;
	}
	
	public AchievementReward GetCurReward()
	{
		AchievementReward reward = null;
		
		foreach(AchievementReward temp in this.achievementRewards)
		{
			if (temp.isRewardComplete == false)
			{
				reward = temp;
				break;
			}
		}
		
		return reward;
	}
	
	public AchievementReward GetLastReward()
	{
		AchievementReward reward = null;
		int nCount = this.achievementRewards.Count;
		
		if (nCount > 0)
			reward = this.achievementRewards[nCount-1];
		
		return reward;
	}
	
	public bool CheckCondition()
	{
		bool isComplete = true;
		int totalCount = this.curCount + this.addCount;
		
		foreach(AchievementReward temp in this.achievementRewards)
		{
			if (temp.limitCount > totalCount)
			{
				isComplete = false;
				break;
			}
		}
		
		return isComplete;
	}
}

public class AchievementReward {
	public int prevLimitCount = 0;
	public int limitCount = 0;	//목표 값.
	
	public int stepID = 0;
	
	public Vector3 rewardGold = Vector3.zero;
	public int awaken = 0;
	public List<int> rewardItemIDs = new List<int>();
	
	public bool isRewardComplete = false;
	
	public string desc = "";
	
	public AchievementReward()
	{
		this.prevLimitCount = 0;
		this.limitCount = 0;
		this.rewardGold = Vector3.zero;
		this.rewardItemIDs.Clear();
		
		this.desc = "";
	}
	
	public AchievementReward(AchievementReward temp)
	{
		this.prevLimitCount = temp.prevLimitCount;
		
		this.limitCount = temp.limitCount;
		this.rewardGold = temp.rewardGold;
		this.awaken = temp.awaken;
		this.stepID = temp.stepID;
		
		foreach(int itemID in temp.rewardItemIDs)
			this.rewardItemIDs.Add(itemID);
		
		this.desc = temp.desc;
	}
}
