using UnityEngine;
using System.Collections;

public class WaveRecord : MonoBehaviour {

	public UILabel title = null;
	
	public UILabel curWave = null;
	public UILabel maxWave = null;
	
	public UILabel timePrefixLabel = null;
	public UILabel timeLabel = null;
	
	public int titleStringID = -1;
	public int maxWaveStringID = -1;
	public int maxWaveStep = 64;
	public int timePrefixStringID = -1;
	
	void Awake()
	{
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		if (title != null)
			title.text = stringTable.GetData(titleStringID);
		
		if (timePrefixLabel != null)
			timePrefixLabel.text = stringTable.GetData(timePrefixStringID);
		
		if (curWave != null)
			curWave.text = string.Format("{0:00}", 0);
		
		if (maxWave != null)
			maxWave.text = string.Format("/{0}{1}", maxWaveStep, stringTable.GetData(maxWaveStringID));
		
		if (timeLabel != null)
		{
			System.TimeSpan time = new System.TimeSpan(0, 0, 0, 0);
			string timeText = string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds);
			timeLabel.text = timeText;
		}
	}
	
	public void SetWaveTime(int waveStep, int timeSec)
	{
		System.TimeSpan time = Game.ToTimeSpan(timeSec);
		
		if (curWave != null)
			curWave.text = string.Format("{0:00}", waveStep);
		
		if (timeLabel != null)
		{
			string timeText = string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds);
			timeLabel.text = timeText;
		}
	}
}
