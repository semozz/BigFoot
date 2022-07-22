using UnityEngine;
using System.Collections;

public class WaveMyInfo : MonoBehaviour {
	public UILabel curRankTitle = null;
	public int curRankTiltleStringID = -1;
	public UILabel curRankName = null;
	public int curRankUnitStringID = -1;
	
	public UILabel curRecordTitleLabel = null;
	public int curRecordTitleStringID = -1;
	
	public UILabel waveStepLabel = null;
	
	public UILabel wavePostfixLabel = null;
	public int wavePostfixStringID = -1;
	
	public UILabel waveTimeLabel = null;
	
	public WaveRankingInfo myInfo = null;
	
	void Awake()
	{
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		if (stringTable != null)
		{
			if (curRankTitle != null && curRankTiltleStringID != -1)
				curRankTitle.text = stringTable.GetData(curRankTiltleStringID);
			
			if (curRecordTitleLabel != null && curRecordTitleStringID != -1)
				curRecordTitleLabel.text = stringTable.GetData(curRecordTitleStringID);
			
			if (wavePostfixLabel != null && wavePostfixStringID != -1)
				wavePostfixLabel.text = stringTable.GetData(wavePostfixStringID);
		}
	}
	
	public void SetMyInfo(WaveRankingInfo info)
	{
		myInfo = info;
		
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = null;
		//WaveRewardInfoTable waveRewardInfoTable = null;
		if (tableManager != null)
		{
			stringTable = tableManager.stringTable;
			//waveRewardInfoTable = tableManager.waveRewardInfo;
		}
		
		string curRankStr = "";
		string waveStepStr = "";
		string waveTimeStr = "";
		
		if (info != null)
		{
			if (info.ranking > 0)
				curRankStr = string.Format("{0}{1}", info.ranking, stringTable.GetData(curRankUnitStringID));
			else
				curRankStr = string.Format("--{0}", stringTable.GetData(curRankUnitStringID));
			
			if (info.RecordStep >= 0)
				waveStepStr = string.Format("{0}", info.RecordStep);
			else
				waveStepStr = "--";
			
			if (info.RecordSec > 0)
			{
				System.TimeSpan time = Game.ToTimeSpan(info.RecordSec);
				waveTimeStr = string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds);
			}
			else
				waveTimeStr = "--:--:--";
		}
		
		if (curRankName != null)
			curRankName.text = curRankStr;
		if (waveStepLabel != null)
			waveStepLabel.text = waveStepStr;
		if (waveTimeLabel != null)
			waveTimeLabel.text = waveTimeStr;
	}
}
