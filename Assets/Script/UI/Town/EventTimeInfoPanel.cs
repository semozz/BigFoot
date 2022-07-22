using UnityEngine;
using System.Collections;

public class EventTimeInfoPanel : MonoBehaviour {

	private Game.EventInfo eventInfo = null;
	
	public UILabel timeInfoLabel = null;
	
	public int dayTimeInfoStringID = 237;
	public int hourTimeInfoStringID = 238;
	public int minTimeInfoStringID = 239;
	
	private string dayTimeFormatString = "{0}:{1}";
	private string hourTimeFormatString = "{0}:{1}";
	private string minTimeFormatString = "{0}:{1}";
	
	void Start()
	{
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		if (stringTable != null)
		{
			dayTimeFormatString = stringTable.GetData(dayTimeInfoStringID);
			hourTimeFormatString = stringTable.GetData(hourTimeInfoStringID);
			minTimeFormatString = stringTable.GetData(minTimeInfoStringID);
		}
	}
	
	public void SetEventInfo(Game.EventInfo eventInfo)
	{
		this.eventInfo = eventInfo;
		
		UpdateEventTime();
	}
	
	public void UpdateEventTime()
	{
		System.TimeSpan timeSpan = System.TimeSpan.MinValue;
		if (eventInfo != null)
			timeSpan = eventInfo.endTime - System.DateTime.Now;
		
		string timeFormatStr = "";
		if (timeSpan.TotalSeconds <= 0)
		{
			timeFormatStr = "--:--:--";
		}
		else if (timeSpan.TotalDays >= 1)
		{
			timeFormatStr = string.Format(dayTimeFormatString, timeSpan.Days, timeSpan.Hours);
		}
		else if (timeSpan.TotalHours >= 1)
		{
			timeFormatStr = string.Format(hourTimeFormatString, timeSpan.Hours, timeSpan.Minutes);
		}
		else
		{
			timeFormatStr = string.Format(minTimeFormatString, timeSpan.Hours, timeSpan.Minutes);
		}
		
		if (timeInfoLabel != null)
			timeInfoLabel.text = timeFormatStr;
		
		if (timeSpan.TotalSeconds > 0)
			Invoke("UpdateEventTime", 1.0f);
	}
}
