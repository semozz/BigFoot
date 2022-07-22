using UnityEngine;
using System.Collections;

public class RescureTimer : MonoBehaviour {
	public GameObject warningPanel = null;
	
	public UILabel titleLabel = null;
	public UILabel rescureCountLabel = null;
	public UILabel timerInfo = null;
	
	public int titleStringID = -1;
	
	public Timer timer = null;
	public EventCondition rescureCondition = null;
	
	void Start()
	{
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		if (stringTable != null)
		{
			if (titleLabel != null && titleStringID != -1)
				titleLabel.text = stringTable.GetData(titleStringID);
		}
	}
	
	void Update()
	{
		System.TimeSpan timeSpan = new System.TimeSpan(0);
		bool isTimerActivate = false;
		if (timer != null)
		{
			isTimerActivate = timer.IsActivate;
			timeSpan = timer.GetLeftTime();
		}
		
		if (timeSpan.TotalSeconds < 0)
			timeSpan = new System.TimeSpan(0);
		
		int milliseconds = timeSpan.Milliseconds / 10;
		string timerInfoStr = string.Format("{0:D2}:{1:D2}.{2:D2}", timeSpan.Minutes, timeSpan.Seconds, milliseconds);
		
		bool isWarning = false;
		if (isTimerActivate == true)
		{
			if (timeSpan.TotalSeconds <= 10.0f && timeSpan.TotalSeconds > 0)
				isWarning = true;
			else
				isWarning = false;
		}
		
		if (warningPanel != null)
			warningPanel.SetActive(isWarning);
			
		if (timerInfo != null)
			timerInfo.text = timerInfoStr;
		
		string rescureCountInfostr = "0/0";
		if (rescureCondition != null)
			rescureCountInfostr = string.Format("{0} / {1}", rescureCondition.ConditionValue, rescureCondition.TargetConditionValue);
		
		if (rescureCountLabel != null)
			rescureCountLabel.text = rescureCountInfostr;
	}
}
