using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BossFaceInfo
{
	public int id = 0;
	public string spriteName = "";
}

[System.Serializable]
public class BossRaidStageTypeInfo
{
	public int stringID = 0;
	public string spriteName = "";
	public Color labelColor = Color.white;
}

public class BossRaidInfo
{
	public bool isCleared = false;
	
	public long index = 0;
	public int bossID = 0;
	public int leftSec = 0;
	
	public bool isPhase2 = false;
    public string ownerPlatform;
    public string ownerPlatformUserID;
	public string finderName = "";
	
	public int myDamage = 0;
	
	public string topCharName = "";
	public int topCharDamage = 0;
	
	public string lastAttackerName = "";
	
	public int curHP = 0;
}

public class BossRaidInfoPanel : MonoBehaviour {
	public BossRaidListWindow parentWindow = null;
	
	public List<BossFaceInfo> bossFaceInfos = new List<BossFaceInfo>();
	public List<BossRaidStageTypeInfo> stageTypeInfos = new List<BossRaidStageTypeInfo>();
	
	public UISprite bossFace = null;
	
	public BossRaidBasicInfoPanel normalInfo = null;
	public BossRaidBasicInfoPanel clearInfo = null;
	
	public UILabel stageTypeNameLabel = null;
	public UISprite stageTypeSprite = null;
	public string defaultStageSpriteName = "Boss_Icon_normal";
	
	public UISlider hpSlider = null;
	public UILabel hpInfoLabel  = null;
	
	public BossRaidStageTypeInfo GetStageTypeInfo(int stageType)
	{
		BossRaidStageTypeInfo info = null;
		int nCount = stageTypeInfos.Count;
		if (stageType >= 0 && stageType < nCount)
			info = stageTypeInfos[stageType];
		
		return info;
	}
	
	public BossFaceInfo GetBossFaceInfo(int id)
	{
		BossFaceInfo info = null;
		foreach(BossFaceInfo temp in bossFaceInfos)
		{
			if (temp.id == id)
			{
				info = temp;
				break;
			}
		}
		
		return info;
	}
	
	public BossRaidInfo bossRaidInfo = null;
	public void SetBossInfo(BossRaidInfo info)
	{
		bossRaidInfo = info;
		
		if (normalInfo != null)
		{
			normalInfo.SetBossInfo(info);
			normalInfo.gameObject.SetActive(!bossRaidInfo.isCleared);
		}
		if (clearInfo != null)
		{
			clearInfo.SetBossInfo(info);
			clearInfo.gameObject.SetActive(bossRaidInfo.isCleared);
		}
		
		string hpRateInfo = "";
		
		float hpRate = 1.0f;
		float maxHPValue = 1000.0f;
		
		TableManager tableManager = TableManager.Instance;
		BossRaidTable bossRaidTable = tableManager != null ? tableManager.bossRaidTable : null;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		string stageTypeSpriteName = defaultStageSpriteName;
		string stageTypeName = "";
		Color stageTypeColor = Color.white;
		
		if (bossRaidInfo != null)
		{
			BossRaidData bossRaidData = bossRaidTable != null ? bossRaidTable.GetData(bossRaidInfo.bossID) : null;
			BossFaceInfo faceInfo = GetBossFaceInfo(bossRaidInfo.bossID);
		
			if (faceInfo != null)
			{
				if (bossFace != null)
					bossFace.spriteName = faceInfo.spriteName;
			}
		
			if (bossRaidData != null)
				maxHPValue = (float)bossRaidData.maxHP;
			
			if (maxHPValue > 0.0f)
				hpRate = (float)bossRaidInfo.curHP / maxHPValue;
			
			hpRateInfo = string.Format("{0:##0}%", hpRate * 100.0f);
			
			BossRaidStageTypeInfo tempInfo = GetStageTypeInfo(bossRaidData.stageType);
			if (tempInfo != null)
			{
				stageTypeSpriteName = tempInfo.spriteName;
				stageTypeName = stringTable.GetData(tempInfo.stringID);
				stageTypeColor = tempInfo.labelColor;
			}
		}
		
		if (hpInfoLabel != null)
			hpInfoLabel.text = hpRateInfo;
		if (hpSlider != null)
			hpSlider.sliderValue = hpRate;
		
		if (stageTypeSprite != null)
			stageTypeSprite.spriteName = stageTypeSpriteName;
		if (stageTypeNameLabel != null)
		{
			stageTypeNameLabel.text = stageTypeName;
			stageTypeNameLabel.color = stageTypeColor;
		}
	}
}
