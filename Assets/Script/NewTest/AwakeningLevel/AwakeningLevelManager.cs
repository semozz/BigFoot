using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AwakeningLevelManager : MonoBehaviour {

	public List<AwakeningLevel> skillList = new List<AwakeningLevel>();
	
	public long curExp = 0;
	public int curLevel = 0;
	
	public int skillPoint = 0;	//기본 사용가능 포인트.(레벨업, 레벨 10당, 보스 포함)
	public int giftPoint = 0;	//선물 받은 포인트.
	public int canBuyPoint = 0;	//레벨업당 3개 구입 가능.
	public int buyPoint = 0;	//구입한 캐시 스킬 포인트.
	
	public int usedPoint = 0;	//실제 사용한 포인트.
	
	public static int startJewel = 5;
	public static int incJewel = 1;
	public static int maxJewel = 50;
	
	public LifeManager lifeManager = null;
	
	public delegate void UpdateAwakenLevel(AwakeningLevelManager manager);
	public UpdateAwakenLevel updateAwakenLevel = null;
	
	private CharExpTable awakenExpTable = null;
	public void Awake()
	{
		TableManager tableManager = TableManager.Instance;
		awakenExpTable = tableManager != null ? tableManager.awakenExpTable : null;
	}
	
	public int GetAvailablePoint()
	{
		int addPoint = GetAddPoint();
		int usedPoint = GetUsedPoint();
		return Mathf.Max(0, (skillPoint + giftPoint + Mathf.Min(this.canBuyPoint, buyPoint)) - (addPoint + usedPoint));
	}
	
	public int GetAvailableBuyPoint()
	{
		return Mathf.Max(0, canBuyPoint - buyPoint);
	}
	
	public void SetSkillLevel(AwakeningLevelInfo info, int point)
	{
		if (info == null)
			return;
		
		AwakeningLevel skill = GetSkill(info.valueType);
		if (skill == null)
		{
			skill = CreateInitSkill(info);
			skillList.Add(skill);
		}
		
		if (skill != null)
			skill.SetPoint(point);
	}
	
	public AwakeningLevel CreateInitSkill(AwakeningLevelInfo info)
	{
		AwakeningLevel skill = null;
		
		skill = new AwakeningLevel(info);
		skill.skillManager = this;
		
		return skill;
	}
	
	public void SetInfo(long newExp, int level, int skillPoint, int giftPoint, int canBuyPoint, int buyPoint)
	{
		curExp = newExp;
		
		curLevel = level;
		if (awakenExpTable != null)
			curLevel = awakenExpTable.GetLevel(curExp);
		
		this.skillPoint = skillPoint;
		this.giftPoint = giftPoint;
		this.canBuyPoint = canBuyPoint;
		this.buyPoint = buyPoint;
		
		if (updateAwakenLevel != null)
			updateAwakenLevel(this);
	}
	
	public int GetNeedJewel(int addBuyPoint)
	{
		int needJewel = 0;
		
		int totalBuyCount = Mathf.Min(canBuyPoint, (buyPoint + addBuyPoint));
		
		if (addBuyPoint > 0)
		{
			/*
			for (int step = buyPoint + 1; step <= totalBuyCount; ++step)
				needJewel += GetNeedJewelStep(step);
			*/
			
			needJewel = GetNeedJewelStep(addBuyPoint);
		}
		
		return needJewel;
	}
	
	public int GetNeedJewelStep(int step)
	{
		/*
		int stepValue = Mathf.Max(0, step - 1);
		int needJewel = Mathf.Min(maxJewel, startJewel + (stepValue * incJewel));
		*/
		
		int stepValue = Mathf.Max(0, step);
		int needJewel = stepValue * startJewel;
		
		return needJewel;
	}
	
	public AwakeningLevel GetSkill(AttributeValue.eAttributeType type)
	{
		AwakeningLevel skill = null;
		foreach(AwakeningLevel tempSkill in skillList)
		{
			if (tempSkill != null && tempSkill.type == type)
			{
				skill = tempSkill;
				break;
			}
		}
		
		return skill;
	}
	
	public AwakeningLevel GetSkill(int skillID)
	{
		AwakeningLevel skill = null;
		foreach(AwakeningLevel tempSkill in skillList)
		{
			if (tempSkill != null && tempSkill.skillID == skillID)
			{
				skill = tempSkill;
				break;
			}
		}
		
		return skill;
	}
	
	public void ApplyLevelInfos(Dictionary<int, CharMasteryData> list)
	{
		TableManager tableManager = TableManager.Instance;
		AwakeningLevelInfoTable conquerorSkillTable = tableManager != null ? tableManager.awakeningLevelInfoTable : null;
		
		foreach(var temp in list)
		{
			CharMasteryData data = temp.Value;
			AwakeningLevelInfo info = conquerorSkillTable != null ? conquerorSkillTable.GetData(data.masteryID) : null;
			
			if (info != null)
				SetSkillLevel(info, data.masteryLevel);
		}
	}
	
	public void OnResetAwakeningLevelValue(AttributeValue attrValue)
	{
		if (attrValue != null)
		{
			if (this.lifeManager != null && this.lifeManager.attributeManager != null)
			{
				this.lifeManager.attributeManager.SubValue(attrValue);
			}
		}
	}
	
	public void OnUpdateAwakeningLevelValue(AttributeValue attrValue)
	{
		if (attrValue != null)
		{
			if (this.lifeManager != null && this.lifeManager.attributeManager != null)
			{
				this.lifeManager.attributeManager.AddValue(attrValue);
			}
		}
	}
	
	public void ResetSkills()
	{
		foreach(AwakeningLevel skill in skillList)
		{
			skill.ResetPoint();
		}
	}
	
	public void InitSkills()
	{
		foreach(AwakeningLevel skill in skillList)
		{
			skill.InitPoint();
		}
	}
	
	public int GetAddPoint()
	{
		int addPoint = 0;
		foreach(AwakeningLevel skill in skillList)
		{
			addPoint += skill.addPoint;
		}
		
		return addPoint;
	}
	
	public int GetUsedPoint()
	{
		int usedPoint = 0;
		foreach(AwakeningLevel skill in skillList)
		{
			usedPoint += skill.curPoint;
		}
		
		return usedPoint;
	}
	
	public void ApplyUpgradeInfo(SkillUpgradeDBInfo info)
	{
		int skillID = 0;
		int skillLv = 0;
		
		int nCount = Mathf.Min(info.SkillIDs.Length, info.Levels.Length);
		for (int index = 0; index < nCount; ++index)
		{
			skillID = info.SkillIDs[index];
			skillLv = info.Levels[index];
			
			AwakeningLevel skill = GetSkill(skillID);
			if (skill != null)
			{
				skillLv = Mathf.Clamp(skillLv, 0, skill.maxPoint);
				skill.SetPoint(skillLv);
			}
		}
	}
	
	public int GetApplyPoint(SkillUpgradeDBInfo upgradeInfo)
	{
		int applyPoint = 0;
		
		List<int> IDs = new List<int>();
		List<int> Lvs = new List<int>();
		List<int> Adds = new List<int>();
		
		foreach(AwakeningLevel info in this.skillList)
		{
			if (info.addPoint == 0)
				continue;
			
			IDs.Add(info.skillID);
			Lvs.Add(info.curPoint + info.addPoint);
			Adds.Add(info.addPoint);
			
			applyPoint += info.addPoint;
		}
		
		upgradeInfo.SkillIDs = IDs.ToArray();
		upgradeInfo.Levels = Lvs.ToArray();
		upgradeInfo.Adds = Adds.ToArray();
		
		return applyPoint;
	}
	
	public Vector2 GetApplyPoint()
	{
		Vector2 result = Vector2.zero;
		
		foreach(AwakeningLevel info in this.skillList)
		{
			result.x += (float)info.curPoint;
			result.y += (float)info.addPoint;
		}
		
		return result;
	}
	
	public float GetLearnNeedGold()
	{
		float needGold = 0.0f;
		foreach(AwakeningLevel info in this.skillList)
		{
			needGold += info.NeedGold();
		}
		
		return needGold;
	}
}
