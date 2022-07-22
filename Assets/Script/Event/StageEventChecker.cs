using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class StageEventInfo
{
	public StageEventChecker.eStageEventType type = StageEventChecker.eStageEventType.eRescure;
	public string uiTimerPrefabPath = "";
	public string uiAlertPrefabPath = "";
}

public class StageEventChecker : MonoBehaviour {
	public enum eStageEventType
	{
		eRescure,
		eGuard,
		eAssult,
		eSeige,
	}
	public eStageEventType type = eStageEventType.eRescure;
	
	public List<StageEventInfo> stageEventInfos = new List<StageEventInfo>();
	
	public Timer timer = null;
	
	public List<EventCondition> eventCheckers = new List<EventCondition>();
	
	public StageManager stageManager = null;
	
	void Start()
	{
		stageManager = GameObject.FindObjectOfType(typeof(StageManager)) as StageManager;
		
		StageEventInfo info = GetStageEventInfo(this.type);
		if (info != null)
		{
			Transform uiRoot = GameUI.Instance.uiRootPanel.transform;
			
			switch(info.type)
			{
			case StageEventChecker.eStageEventType.eRescure:
				RescureTimer rescureTime = ResourceManager.CreatePrefab<RescureTimer>(info.uiTimerPrefabPath, uiRoot, Vector3.zero);
				if (rescureTime != null)
				{
					rescureTime.timer = this.timer;
					rescureTime.rescureCondition = eventCheckers[0];
				}
				break;
			case StageEventChecker.eStageEventType.eSeige:
				RescureTimer siegeTime = ResourceManager.CreatePrefab<RescureTimer>(info.uiTimerPrefabPath, uiRoot, Vector3.zero);
				if (siegeTime != null)
				{
					siegeTime.timer = this.timer;
					//siegeTime.rescureCondition = eventCheckers[0];
				}
				break;
			}
			
			if (info.uiAlertPrefabPath != "")
			{
				GameObject alertObj = ResourceManager.CreatePrefab(info.uiAlertPrefabPath, uiRoot, Vector3.zero);
			}
		}
	}
	
	public StageEventInfo GetStageEventInfo(StageEventChecker.eStageEventType type)
	{
		StageEventInfo info = null;
		foreach(StageEventInfo temp in stageEventInfos)
		{
			if (temp.type == type)
			{
				info = temp;
				break;
			}
		}
		
		return info;
	}
	
	public bool isChecked = false;
	void Update()
	{
		if (isChecked == true)
			return;
		
		//bool isTimeCheck = false;
		if (timer != null)
		{
			if (timer.IsActivate == true)
			{
				System.TimeSpan leftTime = timer.GetLeftTime();
				
				bool conditionCheck = false;
				int conditionCount = eventCheckers.Count;
				
				foreach(EventCondition condition in eventCheckers)
				{
					if (condition.IsComplete == true)
					{
						conditionCheck = true;
						break;
					}
				}
				
				if (leftTime.TotalSeconds < 0)
				{
					if (conditionCount > 0 && conditionCheck == false)
					{
						if (stageManager != null)
							stageManager.OnStageFailed();
						
						isChecked = true;
					}
					else
					{
						isChecked = true;
					}
				}
				else
				{
					if (conditionCheck == true)
					{
						isChecked = true;
					}
				}
			}
		}
		
	}
}
