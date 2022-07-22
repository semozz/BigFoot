using UnityEngine;
using System.Collections;

public class CashEventButton : MonoBehaviour {

	public UILabel timeInfo = null;
	public System.DateTime endTime;
	
	public int timeFormatStringID = 61;
	private string timeFormatString = "{0:D2}:{1:D2}:{2:D2}";
	
	public void Awake()
	{
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		if (stringTable != null)
			timeFormatString = stringTable.GetData(timeFormatStringID);
	}
	
	public void SetEndTime(System.DateTime endTime)
	{
		this.endTime = endTime;
		
		UpateTimeInfo();
	}
	
	public void UpateTimeInfo()
	{
		System.TimeSpan timeSpan = endTime - System.DateTime.Now;
		
		string timeInfoStr = string.Format(timeFormatString, timeSpan.Days, timeSpan.Hours, timeSpan.Minutes);
		PopupBaseWindow.SetLabelString(timeInfo, timeInfoStr);
		
		if (timeInfo != null)
			Invoke("UpateTimeInfo", 1.0f);
	}
}
