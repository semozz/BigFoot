using UnityEngine;
using System.Collections;

public class LimitTimeBuffIcon : MonoBehaviour {
	public UILabel timeInfoLabel = null;
	
	public int dayInfoStringID = 236;
	public int dayTimeInfoStringID = 237;
	public int hourTimeInfoStringID = 238;
	public int minTimeInfoStringID = 239;
	
	private string dayFormatString = "{0}";
	private string dayTimeFormatString = "{0}:{1}";
	private string hourTimeFormatString = "{0}:{1}";
	private string minTimeFormatString = "{0}:{1}";
	
	private System.DateTime endTime;
	
	public GameObject buffRoot = null;
	
	void Start()
	{
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		if (stringTable != null)
		{
			dayFormatString = stringTable.GetData(dayInfoStringID);
			dayTimeFormatString = stringTable.GetData(dayTimeInfoStringID);
			hourTimeFormatString = stringTable.GetData(hourTimeInfoStringID);
			minTimeFormatString = stringTable.GetData(minTimeInfoStringID);
		}
	}
	
	public void SetTimeInfo(System.DateTime endTime)
	{
		this.endTime = endTime;
	}
	
	public void Update()
	{
		System.TimeSpan timeSpan = new System.TimeSpan();
		if (this.endTime != System.DateTime.MinValue)
			timeSpan = this.endTime - System.DateTime.Now;
		
		if (timeSpan.TotalSeconds <= 0)
		{
			this.buffRoot.SetActive(false);
			return;
		}
		else
			this.buffRoot.SetActive(true);
		
		int totalDay = (int)timeSpan.TotalDays;
		
		string timeFormatStr = "";
		if ( totalDay >= 30)
		{
			timeFormatStr = string.Format(dayFormatString, timeSpan.Days);
		}
		else if (totalDay >= 1)
		{
			timeFormatStr = string.Format(dayTimeFormatString, timeSpan.Days, timeSpan.Hours);
		}
		else if (timeSpan.TotalHours >= 1)
		{
			timeFormatStr = string.Format(hourTimeFormatString, timeSpan.Hours, timeSpan.Minutes);
		}
		else
		{
			timeFormatStr = string.Format(minTimeFormatString, timeSpan.Minutes, timeSpan.Seconds);
		}
		
		if (timeInfoLabel != null)
			timeInfoLabel.text = timeFormatStr;		
	}
}
