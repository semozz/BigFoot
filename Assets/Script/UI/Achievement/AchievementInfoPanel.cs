using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AchievementInfoPanel : MonoBehaviour {
	public BaseAchievementListWindow parentWindow = null;
	
	public UILabel titleLabel = null;
	public UILabel descLabel = null;
	
	public UIButtonMessage buttonMessage = null;
	
	public UILabel rewardButtonLabel = null;
	public UILabel progressInfoLabel = null;
	public int completeStringID = -1;
	public int rewardButtonStringID = -1;
	public int rewardItemStringID = -1;
	
	public UISprite rewardSprite = null;
	public UILabel rewardInfoLabel = null;
	public string defaultSpriteName = "Shop_Money01";
	public List<string> rewardSpriteNames = new List<string>();
	
	public GameObject charFaceRoot = null;
	public UISprite charFace = null;
	public List<string> charFaceSprites = new List<string>();
		
	public GameObject rewardButtonObj = null;
	
	public GameObject completePanelObj = null;
	
	public Achievement achieve = null;
	public void SetAchievement(Achievement achieve)
	{
		this.achieve = achieve;
		
		UpdateCharFace();
		
		UpdateInfo();
		
	}
	
	public void UpdateCharFace()
	{
		string faceSpriteName = "";
		if (this.achieve != null)
		{
			if (this.achieve.charIndex != -1)
				faceSpriteName = GetCharFaceSprite(this.achieve.charIndex);
		}
		
		if (charFaceRoot != null)
		{
			charFaceRoot.SetActive(faceSpriteName != "");
			if (charFace != null)
				charFace.spriteName = faceSpriteName;
		}
	}
	
	private string GetCharFaceSprite(int charIndex)
	{
		string spriteName = "";
		int nCount = charFaceSprites.Count;
		if (charIndex >= 0 && charIndex < nCount)
			spriteName = charFaceSprites[charIndex];
		
		return spriteName;
	}
	
	public void UpdateInfo()
	{
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		if (rewardButtonLabel != null)
			rewardButtonLabel.text = stringTable.GetData(rewardButtonStringID);
		
		string titleStr = "";
		string descStr = "";
		string progressInfoStr = "";
		
		if (this.achieve != null)
		{
			titleStr = this.achieve.title;
			//descStr = this.achieve.desc;
			
			if (completePanelObj != null)
				completePanelObj.SetActive(this.achieve.isComplete);
			
			int curStep = 0;
			int maxStep = 0;
			
			AchievementReward reward = null;
			
			if (this.achieve.isComplete == true)
			{
				maxStep = curStep = this.achieve.achievementRewards.Count;
				switch(this.achieve.type)
				{
				case Achievement.eAchievementType.eLevelUp:
				case Achievement.eAchievementType.eStageClear:
					maxStep = 0;
					break;
				}
				
				UpdateStepInfo(curStep, maxStep);
				
				if (rewardButtonObj != null)
					rewardButtonObj.SetActive(false);
				
				progressInfoStr = stringTable.GetData(completeStringID);
				
				if (this.parentWindow.grid.sorted == true)
					this.parentWindow.SetSortName(this);
				
				reward = this.achieve.GetLastReward();
				
				if (reward != null)
					descStr = reward.desc;
				
				if (rewardSprite != null)
					rewardSprite.gameObject.SetActive(false);
				if (rewardInfoLabel != null)
					rewardInfoLabel.gameObject.SetActive(false);
			}
			else
			{
				reward = this.achieve.GetCurReward();
				if (reward != null)
				{
					descStr = reward.desc;
					
					curStep = reward.stepID - 1;
					maxStep = this.achieve.achievementRewards.Count;
					switch(this.achieve.type)
					{
					case Achievement.eAchievementType.eLevelUp:
					case Achievement.eAchievementType.eStageClear:
						maxStep = 0;
						break;
					}
					
					UpdateStepInfo(curStep, maxStep);
				
					int achieveCount = 0;
					switch(achieve.type)
					{
					case Achievement.eAchievementType.eLevelUp:
						achieveCount = this.achieve.curCount + this.achieve.addCount;
						break;
					case Achievement.eAchievementType.eArenaStraightVic:
						achieveCount = this.achieve.curCount + this.achieve.addCount;
						break;
					default:
						achieveCount = this.achieve.curCount + this.achieve.addCount - reward.prevLimitCount;
						if (achieveCount < 0)
							achieveCount = 0;
						break;
					}
					
					if (achieveCount < reward.limitCount)
					{
						if (rewardButtonObj != null)
							rewardButtonObj.SetActive(false);
						
						if (achieve.type == Achievement.eAchievementType.eLevelUp)
							progressInfoStr = string.Format("({0}/{1})", 0, 1);
						else
							progressInfoStr = string.Format("({0}/{1})", achieveCount, reward.limitCount);
					}
					else
					{
						if (rewardButtonObj != null)
							rewardButtonObj.SetActive(true);
					}
					
					string rewardInfoStr = "";
					string rewardSpriteName = defaultSpriteName;
					int spriteIndex = 0;
					if (reward.rewardGold.x > 0)
					{
						rewardInfoStr = string.Format("{0:#,###,##0}", reward.rewardGold.x);
						spriteIndex = 0;
					}
					else if (reward.rewardGold.y > 0)
					{
						rewardInfoStr = string.Format("{0:#,###,##0}", reward.rewardGold.y);
						spriteIndex = 1;
					}
					else if (reward.rewardItemIDs.Count > 0)
					{
						rewardInfoStr = stringTable.GetData(rewardItemStringID);
						
						spriteIndex = 2;
					}
					else if (reward.awaken > 0)
					{
						rewardInfoStr = string.Format("{0:#,###,##0}", reward.awaken);
						spriteIndex = 3;
					}
					
					rewardSpriteName = GetSpriteName(spriteIndex);
					
					if (rewardSprite != null)
					{
						rewardSprite.gameObject.SetActive(true);
						rewardSprite.spriteName = rewardSpriteName;
					}
					if (rewardInfoLabel != null)
					{
						rewardInfoLabel.gameObject.SetActive(true);
						rewardInfoLabel.text = rewardInfoStr;
					}
				}
			}
		}
		
		if (titleLabel != null)
			titleLabel.text = titleStr;
		if (descLabel != null)
			descLabel.text = descStr;
		
		if (progressInfoLabel != null)
			progressInfoLabel.text = progressInfoStr;
	}
	
	public string GetSpriteName(int index)
	{
		int nCount = this.rewardSpriteNames.Count;
		string spriteName = "";
		if (index >= 0 && index < nCount)
			spriteName = this.rewardSpriteNames[index];
		
		return spriteName;
	}
	
	public string activeRewardSpritePath = "";
	public string deActiveRewardSpritePath = "";
	public List<GameObject> rewardStepSprites = new List<GameObject>();
	public List<Transform> posInfos = new List<Transform>();
	public Transform spriteRoot = null;
	
	public void UpdateStepInfo(int curStep, int maxStep)
	{
		foreach(GameObject obj in rewardStepSprites)
			DestroyObject(obj, 0.0f);
		rewardStepSprites.Clear();
		
		string prefabPath = "";
		for (int index = 0; index < maxStep; ++index)
		{
			Transform trans = GetPosInfo(index);
			
			if (index < curStep)
				prefabPath = activeRewardSpritePath;
			else
				prefabPath = deActiveRewardSpritePath;
			
			GameObject newObj = ResourceManager.CreatePrefab(prefabPath, spriteRoot, trans.localPosition);
			rewardStepSprites.Add(newObj);
		}
	}
	
	public Transform GetPosInfo(int index)
	{
		Transform trans = null;
		int nCount = posInfos.Count;
		
		index = Mathf.Min(index, nCount-1);
		
		if (index >= 0 && index < nCount)
			trans = posInfos[index];
		
		return trans;
	}
}
