using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AchievementWindow : PopupBaseWindow {
	public TownUI townUI = null;
	
	public TabButtonController tabButtonController = null;
	
	void Start()
	{
		this.windowType = TownUI.eTOWN_UI_TYPE.ACHIVEMENT;
		
		GameUI.Instance.achievementWindow = this;
	}
	
	void OnDestroy()
	{
		GameUI.Instance.achievementWindow = null;
	}
	
	public BaseAchievementListWindow GetTabWindow(BaseAchievementListWindow.eAchievementListType type)
	{
		BaseAchievementListWindow listWindow = null;
		if (tabButtonController != null)
		{
			foreach(TabButtonInfo info in tabButtonController.tabInfos)
			{
				BaseAchievementListWindow tempWindow = info.viewWindow != null ? info.viewWindow.GetComponent<BaseAchievementListWindow>() : null;
				if (tempWindow != null && tempWindow.listType == type)
				{
					listWindow = tempWindow;
					
					if (listWindow != null)
						listWindow.parentWindow = this.gameObject;
					
					break;
				}
			}
		}
		
		return listWindow;
	}
	
	public void InitWindow()
	{
		AchievementManager achievementManager = null;
		int charIndex = -1;
		if (Game.Instance.connector != null)
			charIndex = Game.Instance.connector.charIndex;
		
		CharInfoData charData = Game.Instance.charInfoData;
		if (charData != null)
			achievementManager = charData.achievementManager;
		
		if (achievementManager != null)
			achievementManager.UpdateDailyMissionTime();
		
		List<Achievement> activeList = achievementManager.GetAchievementList(charIndex, false);
		List<Achievement> completeList = achievementManager.GetCompleteList(charIndex);
		
		List<Achievement> specialActiveList = achievementManager.GetSpecialMissionList(charIndex);
		List<Achievement> specialCompleteList = achievementManager.GetCompleteSpecialMissionList();
		
		BaseAchievementListWindow achieveListWindow = GetTabWindow(BaseAchievementListWindow.eAchievementListType.Achievement);
		if (achieveListWindow != null)
			achieveListWindow.SetInfos(activeList, completeList);
		
		List<Achievement> dailyAchieveList = achievementManager.GetDailyAchievementList();
		achieveListWindow = GetTabWindow(BaseAchievementListWindow.eAchievementListType.DailyAchievement);
		if (achieveListWindow != null)
			achieveListWindow.SetInfos(dailyAchieveList, null);
		
		SpecialAchievementListWindow specialAchieveListWindow = (SpecialAchievementListWindow)GetTabWindow(BaseAchievementListWindow.eAchievementListType.SpecialAchievement);
		if (specialAchieveListWindow != null)
		{
			specialAchieveListWindow.SetInfos(specialActiveList, specialCompleteList);
			
			if (specialAchieveListWindow.banner != null && charData != null && charData.specialEventInfo != null)
				specialAchieveListWindow.banner.SetURL(charData.specialEventInfo.eventBannerURL);
		}
	}
	
	public void OnAcceptReward(GameObject obj)
	{
		if (requestCount > 0)
			return;
		
		AchievementInfoPanel infoPanel = null;
		GameObject parentObj = obj != null ? obj.transform.parent.gameObject : null;
		
		if (parentObj != null)
			infoPanel = parentObj.GetComponent<AchievementInfoPanel>();
		
		if (infoPanel != null)
		{
			//int targetUserIndexID = -1;
			//int targetCharIndex = -1;
			Achievement achieve = infoPanel.achieve;
			
			int groupID = -1;
			int stepID = -1;
			
			int achieveCharIndex = -1;
			
			IPacketSender sender = Game.Instance.packetSender;
			if (sender != null)
				achieveCharIndex = sender.Connector.charIndex;
			
			if (achieve != null)
			{
				groupID = achieve.id;
				AchievementReward reward = achieve.GetCurReward();
				if (reward != null)
					stepID = reward.stepID;
				
				if (achieve.isShare == 1)
					achieveCharIndex = -1;
			}
			
			switch(infoPanel.parentWindow.listType)
			{
			case BaseAchievementListWindow.eAchievementListType.Achievement:
				if (sender != null)
				{
					sender.SendRequestAcceptAchieveReward(groupID, stepID, achieveCharIndex);
					requestCount++;
				}
				break;
			case BaseAchievementListWindow.eAchievementListType.DailyAchievement:
				if (sender != null)
				{
					sender.SendRequestAcceptDailyAchieveReward(groupID, stepID);
					requestCount++;
				}
				break;
			case BaseAchievementListWindow.eAchievementListType.SpecialAchievement:
				if (sender != null)
				{
					sender.SendRequestAcceptSpecialAchieveReward(groupID, stepID, achieveCharIndex);
					requestCount++;
				}
				break;
			}
		}
	}
	
	public int requestCount = 0;
	public override void OnBack()
	{
		requestCount = 0;
		base.OnBack();
		
		foreach(TabButtonInfo info in tabButtonController.tabInfos)
		{
			BaseAchievementListWindow tempWindow = info.viewWindow != null ? info.viewWindow.GetComponent<BaseAchievementListWindow>() : null;
			if (tempWindow != null)
				tempWindow.InitList();
		}
	}
	
	
	public override void OnErrorMessage (NetErrorCode errorCode, PopupBaseWindow popupBase)
	{
		base.OnErrorMessage (errorCode, popupBase);
	}
	
	public void OnChildWindow(GameObject obj)
	{
		ClosePopup();
	}
	
	public void UpdateInfo()
	{
		BaseAchievementListWindow achieveListWindow = GetTabWindow(BaseAchievementListWindow.eAchievementListType.Achievement);
		if (achieveListWindow != null)
			achieveListWindow.UpdateInfo();
		
		achieveListWindow = GetTabWindow(BaseAchievementListWindow.eAchievementListType.DailyAchievement);
		if (achieveListWindow != null)
			achieveListWindow.UpdateInfo();
		
		achieveListWindow = GetTabWindow(BaseAchievementListWindow.eAchievementListType.SpecialAchievement);
		if (achieveListWindow != null)
			achieveListWindow.UpdateInfo();
	}
}
