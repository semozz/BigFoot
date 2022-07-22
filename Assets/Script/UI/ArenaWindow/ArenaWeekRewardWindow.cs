using UnityEngine;
using System.Collections;

public class ArenaWeekRewardWindow : BaseWeekRewardWindow {
	public int rewardMedal = 0;
	
	public override void SetReward(int amount)
	{
		base.SetReward(amount);
		rewardMedal = amount;
	}
	
	public void SetMyRankInfo(int rankType, int ranking)
	{
		TableManager tableManager = TableManager.Instance;
		ArenaRewardInfoTable arenaRewardInfoTable = tableManager != null ? tableManager.arenaRewardInfo : null;
		
		ArenaRewardData rankData = arenaRewardInfoTable.GetRewardInfoData(rankType);
		if (rankTypeLabel != null && rankData != null)
			rankTypeLabel.text = rankData.rankName;
		
		if (rankingLabel != null)
			rankingLabel.text = string.Format("{0:#,###,###}", ranking);
	}
	
	public override void OnOK()
	{
		CharInfoData charData = Game.Instance.charInfoData;
		if (charData != null)
			charData.medal_Value += rewardMedal;
		
		base.OnOK();
	}
}
