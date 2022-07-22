using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[System.Serializable]
public class MasteryInfo_New
{
	public enum eMethodType
	{
		None,
		
		Warrior_01,
		Warrior_02,
		Warrior_03,
		Warrior_04,
		Warrior_05,
		Warrior_06,
		Warrior_07,
		Warrior_08,
		Warrior_09,
		Warrior_10,
		Warrior_11,
		Warrior_12,
		Warrior_13,
		Warrior_14,
		Warrior_15,
		Warrior_16,
		Warrior_17,
		Warrior_18,
		Warrior_19,
		Warrior_20,
		Warrior_21,
		Warrior_22,
		Warrior_23,
		Warrior_24,
		Warrior_25,
		Warrior_26,
		Warrior_27,
		Warrior_28,
		Warrior_29,
		Warrior_30,
		
				
		Assassin_01,
		Assassin_02,
		Assassin_03,
		Assassin_04,
		Assassin_05,
		Assassin_06,
		Assassin_07,
		Assassin_08,
		Assassin_09,
		Assassin_10,
		Assassin_11,
		Assassin_12,
		Assassin_13,
		Assassin_14,
		Assassin_15,
		Assassin_16,
		Assassin_17,
		Assassin_18,
		Assassin_19,
		Assassin_20,
		Assassin_21,
		Assassin_22,
		Assassin_23,
		Assassin_24,
		Assassin_25,
		Assassin_26,
		Assassin_27,
		Assassin_28,
		Assassin_29,
		Assassin_30,
		
				
		Wizard_01,
		Wizard_02,
		Wizard_03,
		Wizard_04,
		Wizard_05,
		Wizard_06,
		Wizard_07,
		Wizard_08,
		Wizard_09,
		Wizard_10,
		Wizard_11,
		Wizard_12,
		Wizard_13,
		Wizard_14,
		Wizard_15,
		Wizard_16,
		Wizard_17,
		Wizard_18,
		Wizard_19,
		Wizard_20,
		Wizard_21,
		Wizard_22,
		Wizard_23,
		Wizard_24,
		Wizard_25,
		Wizard_26,
		Wizard_27,
		Wizard_28,
		Wizard_29,
		Wizard_30,
	}
	public eMethodType method = eMethodType.None;
	public string methodArg = "";
	
	public enum eMasteryActiveType
	{
		None,
		Active,
		Passive,
	}
	public eMasteryActiveType activeType = eMasteryActiveType.Passive;
	
	public int id = -1;
	
	public int groupID = -1;
	
	public int maxPoint = 0;
	public int curPoint = 0;
	public int addPoint = 0;
	public int Point
	{
		get { return curPoint + addPoint; }
	}
	
	public int needGroupID = 0;
	public int needGroupPoint = 0;
	public List<int> needMasteryIDs = new List<int>();
	
	public string iconName = "";
	public string name = "";
	public string desc = "";
	
	public string formatString = "";
	public string unitString = "";
	
	public float incValue = 0.0f;
	
	public string GetCurInfo()
	{
		string infos = "";
		infos = string.Format("{0:#,###,##0.##}", (incValue * curPoint));
		return infos;
	}
	
	public string GetNextInfo()
	{
		string infos = "";
		if (manager != null && curPoint < manager.maxLevel)
		{
			infos = string.Format("{0:#,###,##0.##}", (incValue * (curPoint + 1)));
		}
		return infos;
	}
	
	public bool CheckEnable()
	{
		int errorCount = 0;
		if (needGroupID != 0 && needGroupPoint != 0)
		{
			int curGroupPoint = this.manager.GetGroupPoint(needGroupID);
			if (curGroupPoint < needGroupPoint)
				errorCount++;
		}
		
		foreach(int needMasteryID in this.needMasteryIDs)
		{
			MasteryInfo_New needMastery = this.manager.GetMastery(needMasteryID);
			if (needMastery == null || needMastery.Point < needMastery.maxPoint)
			{
				errorCount++;
				break;
			}
		}
		
		if (activeType == eMasteryActiveType.Active)
		{
			if (manager.activeMastery != null && manager.activeMastery.id != 0 &&
				manager.activeMastery.id != this.id)
				errorCount++;
		}
		
		return (errorCount == 0);
	}
	
	public bool CanUpgrade()
	{
		int errorCount = 0;
		
		if (Point >= maxPoint)
			errorCount++;
		
		return (errorCount == 0);
	}
	
	public MasteryInfo_New(MasteryTableInfo_New info)
	{
		this.method = info.methodType;
		this.methodArg = info.methodArg;
		
		this.activeType = info.activeType;
		
		this.id = info.id;
		this.groupID = info.groupID;
		
		this.maxPoint = info.maxPoint;
		this.curPoint = 0;
		this.addPoint = 0;
		
		this.needGroupID = info.needGroupID;
		this.needGroupPoint = info.needGroupPoint;
		this.needMasteryIDs.AddRange(info.needMasteryIDs);
		
		this.iconName = info.iconName;
		this.name = info.name;
		this.desc = info.desc;
		
		this.formatString = info.formatString;
		this.unitString = info.unitString;
		
		this.incValue = info.incValue;
	}
	
	/*
	public float _value = 0.0f;
	public float _incValue = 0.0f;
	
	public int _level = 0;
	public int Level
	{
		get { return _level; }
		set
		{
			_level = value;
			
			if (manager != null && manager.onUpdateMastery != null)
				manager.onUpdateMastery();
		}
	}
	*/
	
	public MasteryManager_New manager = null;
	
	public static MasteryInfo_New.eMasteryActiveType ToMasteryActiveType(string typeStr)
	{
		MasteryInfo_New.eMasteryActiveType activeType = eMasteryActiveType.None;
		if (typeStr == "Active")
			activeType = MasteryInfo_New.eMasteryActiveType.Active;
		else if (typeStr == "Passive")
			activeType = MasteryInfo_New.eMasteryActiveType.Passive;
		
		return activeType;
	}
	
	public static MasteryInfo_New.eMethodType ToMasteryMethodType(string typeStr)
	{
		MasteryInfo_New.eMethodType type = MasteryInfo_New.eMethodType.None;
		if (typeStr == "Warrior_01")
			type = eMethodType.Warrior_01;
		else if (typeStr == "Warrior_02")
			type = eMethodType.Warrior_02;
		else if (typeStr == "Warrior_03")
			type = eMethodType.Warrior_03;
		else if (typeStr == "Warrior_04")
			type = eMethodType.Warrior_04;
		else if (typeStr == "Warrior_05")
			type = eMethodType.Warrior_05;
		else if (typeStr == "Warrior_06")
			type = eMethodType.Warrior_06;
		else if (typeStr == "Warrior_07")
			type = eMethodType.Warrior_07;
		else if (typeStr == "Warrior_08")
			type = eMethodType.Warrior_08;
		else if (typeStr == "Warrior_09")
			type = eMethodType.Warrior_09;
		else if (typeStr == "Warrior_10")
			type = eMethodType.Warrior_10;
		else if (typeStr == "Warrior_11")
			type = eMethodType.Warrior_11;
		else if (typeStr == "Warrior_12")
			type = eMethodType.Warrior_12;
		else if (typeStr == "Warrior_13")
			type = eMethodType.Warrior_13;
		else if (typeStr == "Warrior_14")
			type = eMethodType.Warrior_14;
		else if (typeStr == "Warrior_15")
			type = eMethodType.Warrior_15;
		else if (typeStr == "Warrior_16")
			type = eMethodType.Warrior_16;
		else if (typeStr == "Warrior_17")
			type = eMethodType.Warrior_17;
		else if (typeStr == "Warrior_18")
			type = eMethodType.Warrior_18;
		else if (typeStr == "Warrior_19")
			type = eMethodType.Warrior_19;
		else if (typeStr == "Warrior_20")
			type = eMethodType.Warrior_20;
		else if (typeStr == "Warrior_21")
			type = eMethodType.Warrior_21;
		else if (typeStr == "Warrior_22")
			type = eMethodType.Warrior_22;
		else if (typeStr == "Warrior_23")
			type = eMethodType.Warrior_23;
		else if (typeStr == "Warrior_24")
			type = eMethodType.Warrior_24;
		else if (typeStr == "Warrior_25")
			type = eMethodType.Warrior_25;
		else if (typeStr == "Warrior_26")
			type = eMethodType.Warrior_26;
		else if (typeStr == "Warrior_27")
			type = eMethodType.Warrior_27;
		else if (typeStr == "Warrior_28")
			type = eMethodType.Warrior_28;
		else if (typeStr == "Warrior_29")
			type = eMethodType.Warrior_29;
		else if (typeStr == "Warrior_30")
			type = eMethodType.Warrior_30;
		else if (typeStr == "Assassin_01")
			type = eMethodType.Assassin_01;
		else if (typeStr == "Assassin_02")
			type = eMethodType.Assassin_02;
		else if (typeStr == "Assassin_03")
			type = eMethodType.Assassin_03;
		else if (typeStr == "Assassin_04")
			type = eMethodType.Assassin_04;
		else if (typeStr == "Assassin_05")
			type = eMethodType.Assassin_05;
		else if (typeStr == "Assassin_06")
			type = eMethodType.Assassin_06;
		else if (typeStr == "Assassin_07")
			type = eMethodType.Assassin_07;
		else if (typeStr == "Assassin_08")
			type = eMethodType.Assassin_08;
		else if (typeStr == "Assassin_09")
			type = eMethodType.Assassin_09;
		else if (typeStr == "Assassin_10")
			type = eMethodType.Assassin_10;
		else if (typeStr == "Assassin_11")
			type = eMethodType.Assassin_11;
		else if (typeStr == "Assassin_12")
			type = eMethodType.Assassin_12;
		else if (typeStr == "Assassin_13")
			type = eMethodType.Assassin_13;
		else if (typeStr == "Assassin_14")
			type = eMethodType.Assassin_14;
		else if (typeStr == "Assassin_15")
			type = eMethodType.Assassin_15;
		else if (typeStr == "Assassin_16")
			type = eMethodType.Assassin_16;
		else if (typeStr == "Assassin_17")
			type = eMethodType.Assassin_17;
		else if (typeStr == "Assassin_18")
			type = eMethodType.Assassin_18;
		else if (typeStr == "Assassin_19")
			type = eMethodType.Assassin_19;
		else if (typeStr == "Assassin_20")
			type = eMethodType.Assassin_20;
		else if (typeStr == "Assassin_21")
			type = eMethodType.Assassin_21;
		else if (typeStr == "Assassin_22")
			type = eMethodType.Assassin_22;
		else if (typeStr == "Assassin_23")
			type = eMethodType.Assassin_23;
		else if (typeStr == "Assassin_24")
			type = eMethodType.Assassin_24;
		else if (typeStr == "Assassin_25")
			type = eMethodType.Assassin_25;
		else if (typeStr == "Assassin_26")
			type = eMethodType.Assassin_26;
		else if (typeStr == "Assassin_27")
			type = eMethodType.Assassin_27;
		else if (typeStr == "Assassin_28")
			type = eMethodType.Assassin_28;
		else if (typeStr == "Assassin_29")
			type = eMethodType.Assassin_29;
		else if (typeStr == "Assassin_30")
			type = eMethodType.Assassin_30;
		else if (typeStr == "Wizard_01")
			type = eMethodType.Wizard_01;
		else if (typeStr == "Wizard_02")
			type = eMethodType.Wizard_02;
		else if (typeStr == "Wizard_03")
			type = eMethodType.Wizard_03;
		else if (typeStr == "Wizard_04")
			type = eMethodType.Wizard_04;
		else if (typeStr == "Wizard_05")
			type = eMethodType.Wizard_05;
		else if (typeStr == "Wizard_06")
			type = eMethodType.Wizard_06;
		else if (typeStr == "Wizard_07")
			type = eMethodType.Wizard_07;
		else if (typeStr == "Wizard_08")
			type = eMethodType.Wizard_08;
		else if (typeStr == "Wizard_09")
			type = eMethodType.Wizard_09;
		else if (typeStr == "Wizard_10")
			type = eMethodType.Wizard_10;
		else if (typeStr == "Wizard_11")
			type = eMethodType.Wizard_11;
		else if (typeStr == "Wizard_12")
			type = eMethodType.Wizard_12;
		else if (typeStr == "Wizard_13")
			type = eMethodType.Wizard_13;
		else if (typeStr == "Wizard_14")
			type = eMethodType.Wizard_14;
		else if (typeStr == "Wizard_15")
			type = eMethodType.Wizard_15;
		else if (typeStr == "Wizard_16")
			type = eMethodType.Wizard_16;
		else if (typeStr == "Wizard_17")
			type = eMethodType.Wizard_17;
		else if (typeStr == "Wizard_18")
			type = eMethodType.Wizard_18;
		else if (typeStr == "Wizard_19")
			type = eMethodType.Wizard_19;
		else if (typeStr == "Wizard_20")
			type = eMethodType.Wizard_20;
		else if (typeStr == "Wizard_21")
			type = eMethodType.Wizard_21;
		else if (typeStr == "Wizard_22")
			type = eMethodType.Wizard_22;
		else if (typeStr == "Wizard_23")
			type = eMethodType.Wizard_23;
		else if (typeStr == "Wizard_24")
			type = eMethodType.Wizard_24;
		else if (typeStr == "Wizard_25")
			type = eMethodType.Wizard_25;
		else if (typeStr == "Wizard_26")
			type = eMethodType.Wizard_26;
		else if (typeStr == "Wizard_27")
			type = eMethodType.Wizard_27;
		else if (typeStr == "Wizard_28")
			type = eMethodType.Wizard_28;
		else if (typeStr == "Wizard_29")
			type = eMethodType.Wizard_29;
		else if (typeStr == "Wizard_30")
			type = eMethodType.Wizard_30;
		
		

		return type;
	}
}

[System.Serializable]
public class MasteryManager_New  {
	public List<MasteryInfo_New> totalList = new List<MasteryInfo_New>();
	public Dictionary<int, List<MasteryInfo_New>> groupList = new Dictionary<int, List<MasteryInfo_New>>();
	
	public int maxLevel = 20;
	public LifeManager lifeManager = null;
	
	public MasteryInfo_New activeMastery = null;
	
	public delegate void OnUpdateMastery();
	public OnUpdateMastery onUpdateMastery = null;
	public OnUpdateMastery onResetMastery = null;
	
	public MasteryManager_New(LifeManager lifeManager)
	{
		this.lifeManager = lifeManager;
		
		activeMastery = null;
	}
	
	public MasteryInfo_New GetMastery(int id)
	{
		MasteryInfo_New selectedInfo = null;
		foreach(MasteryInfo_New temp in totalList)
		{
			if (temp != null && temp.id == id)
			{
				selectedInfo = temp;
				break;
			}
		}
		
		return selectedInfo;
	}
	
	public bool AddMastery(MasteryInfo_New newInfo)
	{
		if (newInfo == null)
			return false;
		
		MasteryInfo_New info = GetMastery(newInfo.id);
		if (info != null)
			return false;
		
		if (totalList == null)
			totalList = new List<MasteryInfo_New>();
		
		newInfo.manager = this;
		totalList.Add(newInfo);
		
		AddGroupList(newInfo);
		
		return true;
	}
			
	public void AddGroupList(MasteryInfo_New newInfo)
	{
		if (newInfo == null)
			return;
		
		int groupID = newInfo.groupID;
		List<MasteryInfo_New> tempList = null;
		if (groupList == null)
			groupList = new Dictionary<int, List<MasteryInfo_New>>();
		
		if (groupList.ContainsKey(groupID) == false)
		{
			tempList = new List<MasteryInfo_New>();
			groupList.Add(groupID, tempList);
		}
		else
			tempList = groupList[groupID];
		
		if (tempList != null)
			tempList.Add(newInfo);
	}
	
	public int GetGroupPoint(int groupID)
	{
		int groupPoint = 0;
		
		List<MasteryInfo_New> groupData = null;
		if (groupList.ContainsKey(groupID) == true)
			groupData = groupList[groupID];
		
		if (groupData != null)
		{
			foreach(MasteryInfo_New info in groupData)
			{
				if (info != null)
					groupPoint += info.Point;
			}
		}
		
		return groupPoint;
	}
	
	public int CheckAddPoint()
	{
		int addPoint = 0;
		foreach(MasteryInfo_New info in totalList)
		{
			addPoint += info.addPoint;
		}
		
		return addPoint;
	}
	
	public int CheckPoint()
	{
		int addPoint = 0;
		foreach(MasteryInfo_New info in totalList)
		{
			addPoint += info.curPoint;
		}
		
		return addPoint;
	}
	
	public int ResetMastery()
	{
		if (onResetMastery != null)
			onResetMastery();
		
		int charIndex = 0;
		CharInfoData charData = Game.Instance.charInfoData;
		CharPrivateData privateData = null;
		if (Game.Instance.connector != null)
			charIndex = Game.Instance.connector.charIndex;
		if (charData != null)
			privateData = charData.GetPrivateData(charIndex);
		
		int recoveryPoint = 0;
		
		foreach(MasteryInfo_New info in totalList)
		{
			recoveryPoint += info.curPoint;
			recoveryPoint += info.addPoint;
			
			info.curPoint = 0;
			info.addPoint = 0;
			
			if (privateData != null)
				privateData.SetMasteryData(info.id, info.curPoint);
		}
		
		activeMastery = null;
		
		return recoveryPoint;
	}
	
	public int ApplyAddPoint()
	{
		int charIndex = 0;
		CharInfoData charData = Game.Instance.charInfoData;
		CharPrivateData privateData = null;
		if (Game.Instance.connector != null)
			charIndex = Game.Instance.connector.charIndex;
		if (charData != null)
			privateData = charData.GetPrivateData(charIndex);
		
		PlayerController player = Game.Instance.player;
		if (player != null)
			player.OnResetMastery_New();
		
		int applyPoint = 0;
		foreach(MasteryInfo_New info in totalList)
		{
			info.curPoint += info.addPoint;
			applyPoint += info.addPoint;
			
			if (privateData != null)
				privateData.SetMasteryData(info.id, info.curPoint);
			
			info.addPoint = 0;
			
			if (info.activeType == MasteryInfo_New.eMasteryActiveType.Active &&
				info.manager.activeMastery == info &&
				info.Point < 1)
				info.manager.activeMastery = null;
		}
		
		if (player != null)
			player.OnUpdateMastery_New();
		
		return applyPoint;
	}
	
	public int GetApplyPoint(SkillUpgradeDBInfo upgradeInfo)
	{
		int applyPoint = 0;
		bool isActiveMastery = false;
		
		List<int> IDs = new List<int>();
		List<int> Lvs = new List<int>();
		List<int> Adds = new List<int>();
		
		foreach(MasteryInfo_New info in totalList)
		{
			if (info.addPoint == 0)
				continue;
			
			IDs.Add(info.id);
			Lvs.Add(info.curPoint + info.addPoint);
			Adds.Add(info.addPoint);
			
			applyPoint += info.addPoint;
			
			if (info.activeType == MasteryInfo_New.eMasteryActiveType.Active)
				isActiveMastery = true;
			//info.addPoint = 0;
		}
		
		upgradeInfo.SkillIDs = IDs.ToArray();
		upgradeInfo.Levels = Lvs.ToArray();
		upgradeInfo.Adds = Adds.ToArray();
		
		if (isActiveMastery == true)
			Game.Instance.ApplyAchievement(Achievement.eAchievementType.eActiveSkill, 1);
		
		Game.Instance.SendUpdateAchievmentInfo();
		
		return applyPoint;
	}
	
	public int CancelAddPoint()
	{
		int recoveryPoint = 0;
		foreach(MasteryInfo_New info in totalList)
		{
			recoveryPoint += info.addPoint;
			info.addPoint = 0;
			
			if (info.activeType == MasteryInfo_New.eMasteryActiveType.Active &&
				info.manager.activeMastery == info &&
				info.Point < 1)
				info.manager.activeMastery = null;
		}
		
		return recoveryPoint;
	}
	
	public void ApplyMasteryLevelInfos(Dictionary<int, CharMasteryData> list)
	{
		if (onResetMastery != null)
			onResetMastery();
		
		foreach(var temp in list)
		{
			CharMasteryData data = temp.Value;
			MasteryInfo_New info = GetMastery(data.masteryID);
			if (info != null)
			{
				info.curPoint = Mathf.Clamp(data.masteryLevel, 0, info.maxPoint);
				
				if (info.activeType == MasteryInfo_New.eMasteryActiveType.Active &&
					(this.activeMastery == null || this.activeMastery.id == 0) &&
					info.Point > 0)
					this.activeMastery = info;
			}
		}
		
		if (onUpdateMastery != null)
			onUpdateMastery();
	}
	
	public void ApplyMasteryLevelInfos(int id, int level)
	{
		if (onResetMastery != null)
			onResetMastery();
		
		MasteryInfo_New info = GetMastery(id);
		if (info != null)
		{
			info.curPoint = level;
			
			if (info.activeType == MasteryInfo_New.eMasteryActiveType.Active &&
				(this.activeMastery == null || this.activeMastery.id == 0) &&
				info.Point > 0)
				this.activeMastery = info;
		}
		
		if (onUpdateMastery != null)
			onUpdateMastery();
	}
	
	public void ApplyUpgradeInfo(SkillUpgradeDBInfo info)
	{
		if (onResetMastery != null)
			onResetMastery();
		
		int skillID = 0;
		int skillLv = 0;
		
		int nCount = Mathf.Min(info.SkillIDs.Length, info.Levels.Length);
		for (int index = 0; index < nCount; ++index)
		{
			skillID = info.SkillIDs[index];
			skillLv = info.Levels[index];
			
			MasteryInfo_New mastery = GetMastery(skillID);
			if (mastery != null)
			{
				mastery.curPoint = Mathf.Clamp(skillLv, 0, mastery.maxPoint);
				mastery.addPoint = 0;
				
				if (mastery.activeType == MasteryInfo_New.eMasteryActiveType.Active &&
					(this.activeMastery == null || this.activeMastery.id == 0) &&
					mastery.Point > 0)
					this.activeMastery = mastery;
			}
		}
		
		if (onUpdateMastery != null)
			onUpdateMastery();
	}
	
	public bool CheckUsablePoint()
	{
		int errorCount = 0;
		
		int addPoint = this.CheckAddPoint();
		
		int charIndex = -1;
		CharInfoData charData = Game.Instance.charInfoData;
		if (charData != null)
			charIndex = Game.Instance.connector.charIndex;
		CharPrivateData privateData = null;
		if (charData != null)
			privateData = charData.GetPrivateData(charIndex);
		
		int curSkillPoint = 0;
		if (privateData != null)
			curSkillPoint = privateData.baseInfo.SkillPoint;
		
		if (curSkillPoint <= addPoint)
			errorCount++;
		
		return (errorCount == 0);
	}
	
	/*
	public float GetMasteryValue(MasteryInfo.eMasteries type)
	{
		float resultValue = 0.0f;
		MasteryInfo info = GetMastery(type);
		
		if (info != null)
			resultValue = info._incValue * info._level;
		
		return resultValue;
	}
	
	public void SetMasteryLevel(MasteryInfo.eMasteries type, int level)
	{
		MasteryInfo info = GetMastery(type);
		if (info != null)
		{
			if (onResetMastery != null)
				onResetMastery(null);
			
			info._level = Mathf.Clamp(level, 0, maxLevel);
			
			if (onUpdateMastery != null)
				onUpdateMastery(info);
		}
	}
	*/
}
