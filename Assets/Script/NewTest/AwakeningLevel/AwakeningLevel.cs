using UnityEngine;
using System.Collections;

public class AwakeningLevel
{
	public AwakeningLevelManager skillManager = null;
	
	public int skillID = -1;
	
	public AttributeValue.eAttributeType type;
	public int Point
	{
		get { return curPoint + addPoint; }
	}
	public int curPoint = 0;
	public int addPoint = 0;
	
	public int maxPoint = 1000;
	
	public float needGold = 50000.0f;
	
	public string skillName = "";
	public string skillIconName = "";
	
	public AwakeningLevel()
	{
		
	}
	
	public AwakeningLevel(AwakeningLevelInfo info)
	{
		if (info != null)
		{
			this.type = info.valueType;
			this.skillID = info.id;
		
			this.skillName = info.skillName;
			this.skillIconName = info.iconName;
			
			this.maxPoint = info.maxCount;
			
			this.needGold = info.startGold;
			//this.maxGold = info.maxGold;
			
			attributeValue = new AttributeValue(this.type, 0.0f, info.incValue, 0.0f);
		}
	}
	
	public float NeedGold()
	{
		float needGoldValue = addPoint * needGold;
		return needGoldValue;
	}
	
	public float LearnGold()
	{
		return needGold;
	}
	
	private AttributeValue attributeValue = null;
	public void SetPoint(int level)
	{
		curPoint = level;
		addPoint = 0;
		
		UpdateAttributeValue();
	}
	
	public void AddPoint()
	{
		addPoint += 1;
		
		UpdateAttributeValue();
	}
	public void ResetPoint()
	{
		addPoint = 0;
		
		UpdateAttributeValue();
	}
	
	public void InitPoint()
	{
		curPoint = addPoint = 0;
		
		UpdateAttributeValue();
	}
	
	public void UpdateAttributeValue()
	{
		float newLevel = (float)(curPoint + addPoint);
		
		if (attributeValue != null)
		{
			float oldLevel = attributeValue.level;
			if (oldLevel != newLevel)
			{
				if (skillManager != null)
					skillManager.OnResetAwakeningLevelValue(attributeValue);
				
				attributeValue.SetLevel(newLevel);
				
				if (skillManager != null)
					skillManager.OnUpdateAwakeningLevelValue(attributeValue);
			}
		}
	}
	
	public string GetCurSkillInfo()
	{
		string infoStr = "";
		if (attributeValue != null)
		{
			infoStr = attributeValue.GetInfoStrTempLevel(Point);
		}
		
		return infoStr;
	}
	
	public string GetNextSkillInfo()
	{
		string infoStr = "";
		if (attributeValue != null)
		{
			if (Point < maxPoint)
			{
				infoStr = attributeValue.GetInfoStrTempLevel(Point + 1);
			}
		}
		
		return infoStr;
	}
}
