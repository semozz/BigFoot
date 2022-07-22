using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseAchievementListWindow : MonoBehaviour {
	public GameObject parentWindow = null;
	
	public enum eAchievementListType
	{
		Achievement,
		DailyAchievement,
		SpecialAchievement,
	}
	public eAchievementListType listType = eAchievementListType.Achievement;
	
	public string infoPanelPrefabPath = "";
	public UIGrid grid = null;
	public UIDraggablePanel dragablePanel = null;
	public UIPanel scrollPanel = null;
	
	public void SetButtonMessage(UIButtonMessage buttonMsg, GameObject target, string funcName)
	{
		if (buttonMsg != null)
		{
			buttonMsg.target = target;
			buttonMsg.functionName = funcName;//"OnTargetDetailWindow";
		}
	}
	
	private List<AchievementInfoPanel> achievementInfoPanels = new List<AchievementInfoPanel>();
	public void SetInfos2(Dictionary<int, Achievement> achievements, Dictionary<int, Achievement> completeList)
	{
		List<Achievement> tempNormalList = new List<Achievement>();
		if (achievements != null)
		{
			foreach(var temp1 in achievements)
				tempNormalList.Add(temp1.Value);
		}
		
		
		List<Achievement> tempCompleteList = new List<Achievement>();
		if (completeList != null)
		{
			foreach(var temp2 in completeList)
				tempCompleteList.Add(temp2.Value);
		}
		
		SetInfos(tempNormalList, tempCompleteList);
	}
	
	public void SetInfos(List<Achievement> achievements, List<Achievement> completeList)
	{
		if (achievements == null)
			return;
		
		Vector3 vPos = Vector3.zero;
		int nCount = achievements != null ? achievements.Count : 0;
		for (int index = 0; index < nCount; ++index)
		{
			Achievement info = achievements[index];
			
			AchievementInfoPanel infoPanel = ResourceManager.CreatePrefab<AchievementInfoPanel>(infoPanelPrefabPath, grid.transform, vPos);
			if (infoPanel != null)
			{
				infoPanel.parentWindow = this;
				
				infoPanel.SetAchievement(info);
				
				SetButtonMessage(infoPanel.buttonMessage, this.parentWindow, "OnAcceptReward");
				
				achievementInfoPanels.Add(infoPanel);
				
				if (this.grid.sorted == true)
					SetSortName(infoPanel);
				
				vPos.y += grid.cellHeight;
			}
		}
		
		nCount = completeList != null ? completeList.Count : 0;
		for (int index = 0; index < nCount; ++index)
		{
			Achievement info = completeList[index];
			
			AchievementInfoPanel infoPanel = ResourceManager.CreatePrefab<AchievementInfoPanel>(infoPanelPrefabPath, grid.transform, vPos);
			if (infoPanel != null)
			{
				infoPanel.parentWindow = this;
				
				infoPanel.SetAchievement(info);
				
				SetButtonMessage(infoPanel.buttonMessage, this.parentWindow, "OnAcceptReward");
				
				achievementInfoPanels.Add(infoPanel);
				
				if (this.grid.sorted == true)
					SetSortName(infoPanel);
				
				vPos.y += grid.cellHeight;
			}
		}
		
		//RefreshPanels();
		this.Invoke("RefreshPanels", 0.1f);
	}
	
	public void SetSortName(AchievementInfoPanel infoPanel)
	{
		if (infoPanel == null || infoPanel.achieve == null)
			return;
		
		string prefixStr = "";
		
		AchievementReward reward = infoPanel.achieve.GetCurReward();
		
		int achieveCount = -1;
		int prevLimitCount = 0;
		int limitCount = 0;
		
		if (reward != null)
		{
			limitCount = reward.limitCount;
			prevLimitCount = reward.prevLimitCount;
			
			switch(infoPanel.achieve.type)
			{
			case Achievement.eAchievementType.eLevelUp:
				achieveCount = infoPanel.achieve.curCount + infoPanel.achieve.addCount;
				break;
			case Achievement.eAchievementType.eArenaStraightVic:
				achieveCount = infoPanel.achieve.curCount + infoPanel.achieve.addCount;
				break;
			default:
				achieveCount = infoPanel.achieve.curCount + infoPanel.achieve.addCount - prevLimitCount;
				break;
			}
		}
		
		bool isRewardEnable = true;
		if (reward != null && achieveCount < limitCount)
			isRewardEnable = false;
		
		if (reward == null || infoPanel.achieve.isComplete == true)
			prefixStr = "ZZZ";
		else
		{
			if (isRewardEnable == true)
				prefixStr = "000";
			else
				prefixStr = "AAA";
		}
		
		string objName = string.Format("{0}{1:D8}", prefixStr, infoPanel.achieve.id);
		infoPanel.name = objName;
	}
	
	public void RefreshPanels()
	{
		if (grid != null)
			grid.Reposition();
		
		if (dragablePanel != null)
			dragablePanel.ResetPosition();
		
		if (scrollPanel != null)
			scrollPanel.Refresh();
	}
	
	public void RemoveInfo(AchievementInfoPanel infoPanel)
	{
		foreach(AchievementInfoPanel temp in achievementInfoPanels)
		{
			if (temp == infoPanel)
			{
				achievementInfoPanels.Remove(temp);
				DestroyObject(temp.gameObject, 0.0f);
				break;
			}
		}
		
		//RefreshPanels();
		this.Invoke("RefreshPanels", 0.1f);
	}
	
	bool isRefreshCalled = false;
	public void OnEnable()
	{
		if (isRefreshCalled == false && achievementInfoPanels.Count > 0)
		{
			//RefreshPanels();
			this.Invoke("RefreshPanels", 0.1f);
			isRefreshCalled = true;
		}
	}
	
	public void OnDisable()
	{
		isRefreshCalled = false;
	}
	
	public void InitList()
	{
		foreach(AchievementInfoPanel temp in achievementInfoPanels)
		{
			DestroyObject(temp.gameObject, 0.0f);
		}
		achievementInfoPanels.Clear();
		
		isRefreshCalled = false;
	}
	
	public void UpdateInfo()
	{
		foreach(AchievementInfoPanel temp in achievementInfoPanels)
		{
			temp.UpdateInfo();
			SetSortName(temp);
		}
		
		if (this.grid.sorted == true)
			this.Invoke("RefreshPanels", 0.1f);
	}
}
