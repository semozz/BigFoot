using UnityEngine;
using System.Collections;

public class WaveWeekRewardWindow : BaseWeekRewardWindow {
	public int rewardJewel = 0;
	public override void SetReward(int amount)
	{
		base.SetReward(amount);
		rewardJewel = amount;
	}
	
	public UILabel waveLabel = null;
	public int waveStringID = -1;
	public void SetMyRecord(int order, int waveStep, int time)
	{
		if (rankTypeLabel != null)
			rankTypeLabel.text = string.Format("{0:#,###,###}", order);
		
		
		System.TimeSpan timeSpan = Game.ToTimeSpan(time);
		if (rankingLabel != null)
			rankingLabel.text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
		
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		string waveStr = "";
		if (stringTable != null && waveStringID != -1)
			waveStr = stringTable.GetData(waveStringID);
		
		if (waveLabel != null)
			waveLabel.text = string.Format("{0:D2}{1}", waveStep, waveStr);
	}
	
	public override void OnOK()
	{
		CharInfoData charData = Game.Instance.charInfoData;
		if (charData != null)
			charData.jewel_Value += rewardJewel;
		
		base.OnOK();
	}
}
