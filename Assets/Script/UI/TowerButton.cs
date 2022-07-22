using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TowerLevelLimitInfo
{
	public int limitLevel;
	public GameObject obj;
	public bool isActive;
}

public class TowerButton : MonoBehaviour {
	public TowerInfo.eTowerType towerType = TowerInfo.eTowerType.Wood;
	
	public UISprite towerSprite = null;
	
	public UILabel towerNameLabel = null;
	public UILabel attackInfoLabel = null;
	public UILabel healthInfoLabel = null;
	public UILabel needInofLabel = null;
	
	public UIButtonMessage buttonMessage = null;
	public UICheckbox checkBox = null;
	public UISprite jewelSprite = null;
	
	public int attackLabelStringID = -1;
	public int healthLabelStringID = -1;
	public int basicStringID = -1;
	public int equipStringID = -1;
	
	public List<TowerLevelLimitInfo> levelLimitInfo = new List<TowerLevelLimitInfo>();
	public UIButton button = null;
	public void SetActiveLimitInfo(int index, bool bActive)
	{
		int nCount = levelLimitInfo.Count;
		TowerLevelLimitInfo info = null;
		if (index >= 0 && index < nCount)
			info = levelLimitInfo[index];
		
		if (info != null)
			info.isActive = bActive;
	}
	
	public void SetLimitLevel(int charLevel)
	{
		foreach(TowerLevelLimitInfo info in levelLimitInfo)
		{
			if (info.obj != null)
				info.obj.SetActive(false);
		}
		
		bool isLevelLimit = false;
		GameObject limitObj = null;
		foreach(TowerLevelLimitInfo info in levelLimitInfo)
		{
			if (info.isActive == true &&
				charLevel < info.limitLevel)
			{
				limitObj = info.obj;
				isLevelLimit = true;
				break;
			}
		}
		
		if (limitObj != null)
			limitObj.SetActive(true);
		
		if (button != null)
			button.isEnabled = !isLevelLimit;
	}
	
	public TowerInfo towerInfo = null;
	public TowerInfo GetInfo()
	{
		return towerInfo;
	}
	
	public void SetInfo(TowerInfo info)
	{
		towerInfo = info;
		
		TowerInfo.eTowerType type = TowerInfo.eTowerType.None;
		string name = "";
		float attack = 0.0f;
		float health = 0.0f;
		float jewel = 0.0f;
		
		if (towerInfo != null)
		{
			type = info.type;
			attack = info.attack;
			health = info.health;
			jewel = info.needJewel;
			name =  info.name;
		}
		
		SetInfo(type, name, attack, health, jewel);
	}
	
	public void SetInfo(TowerInfo.eTowerType type, string name, float attack, float health, float needJewel)
	{
		SetTowerType(type);
		
		if (towerNameLabel != null)
			towerNameLabel.text = name;
		
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		string msg = "";
		if (attackInfoLabel != null)
		{
			if (stringTable != null && attackLabelStringID != -1)
				msg = stringTable.GetData(attackLabelStringID);
			
			attackInfoLabel.text = string.Format("{0}\n{1:#,###,###}", msg, attack);
		}
		
		if (healthInfoLabel != null)
		{
			if (stringTable != null && healthLabelStringID != -1)
				msg = stringTable.GetData(healthLabelStringID);
			
			healthInfoLabel.text = string.Format("{0}\n{1:#,###,###}", msg, health);
		}
		
		if (needInofLabel != null)
		{
			if (stringTable != null && attackLabelStringID != -1)
			{
				string jewelText = stringTable.GetData(basicStringID);
				if(needJewel > 0.0f)
				{
					jewelSprite.enabled = true;
					jewelText = string.Format("{0:#,###}", needJewel);
				}
				else
					jewelSprite.enabled = false;
				
				needInofLabel.text = jewelText + " " + stringTable.GetData(equipStringID);
			}
		}
	}
	
	public void SetTowerType(TowerInfo.eTowerType type)
	{
		towerType = type;
		
		string spriteName = "Tower_Wood";
		switch(towerType)
		{
		case TowerInfo.eTowerType.None:
			spriteName = "Black_Frame";
			break;
		case TowerInfo.eTowerType.Wood:
			spriteName = "Tower_Wood";
			break;
		case TowerInfo.eTowerType.Stone:
			spriteName = "Tower_Stone";
			break;
		case TowerInfo.eTowerType.Iron:
			spriteName = "Tower_Iron";
			break;
		}
		
		if (towerSprite != null)
			towerSprite.spriteName = spriteName;
	}
}
