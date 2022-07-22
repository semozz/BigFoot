using UnityEngine;
using System.Collections;

public class DefenceInfos : MonoBehaviour {
	public UILabel titleLabel = null;
	public int titleStringID = -1;
	
	public UILabel defenceInfo = null;
	
	public int orderTitleStringID = -1;
	public int orderUnitStringID = -1;
	
	public int waveTitleStringID = -1;
	
	public int waveTimeTitleStringID = -1;
	
	void Awake()
	{
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		if (stringTable != null)
		{
			if (titleLabel != null && titleStringID != -1)
				titleLabel.text = stringTable.GetData(titleStringID);
		}
	}
	
	public void SetInfo(WaveRankingInfo info)
	{
		string infoStr = "";
		
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = null;
		if (tableManager != null)
		{
			stringTable = tableManager.stringTable;
		}
		
		string curRanking = "";
		string waveStep = "";
		string waveTime = "";
		
		if (info.ranking >= 0)
			curRanking = string.Format("{0} : {1:#,###,##0}{2}", stringTable.GetData(orderTitleStringID), info.ranking, stringTable.GetData(orderUnitStringID));
		else
			curRanking = string.Format("{0} : --", stringTable.GetData(orderTitleStringID));
		
		if (info.RecordStep >= 0)
			waveStep = string.Format("{0} : {1:#,###,##0}", stringTable.GetData(waveTitleStringID), info.RecordStep);
		else
			waveStep = string.Format("{0} : --", stringTable.GetData(waveTitleStringID));
		
		if (info.RecordSec >= 0)
		{
			System.TimeSpan time = Game.ToTimeSpan(info.RecordSec);
			waveTime = string.Format("{0} : {1:D2}:{2:D2}:{3:D2}", stringTable.GetData(waveTimeTitleStringID), time.Hours, time.Minutes, time.Seconds);
		}
		else
			waveTime = string.Format("{0} : --:--:--", stringTable.GetData(waveTimeTitleStringID));
		
		infoStr = string.Format("{0}\n{1}\n{2}", curRanking, waveStep, waveTime);
		if (defenceInfo != null)
			defenceInfo.text = infoStr;
	}
}
