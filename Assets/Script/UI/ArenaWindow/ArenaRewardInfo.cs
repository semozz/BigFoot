using UnityEngine;
using System.Collections;

public class ArenaRewardInfo : MonoBehaviour {
	public UILabel rankName = null;
	public UILabel limitPercent = null;
	public UILabel rewardMedalAmount = null;
	
	public void SetInfo(ArenaRewardData data)
	{
		string rankNameStr = "";
		string limitPercentStr = "";
		string rewardMedal = "";
		
		if (data != null)
		{
			rankNameStr = data.rankName;
			limitPercentStr = data.desc;
			rewardMedal = string.Format("{0:#,###,##0}", data.rewardMedal);
		}
		
		if (rankName != null)
			rankName.text = rankNameStr;
		if (limitPercent != null)
			limitPercent.text = limitPercentStr;
		if (rewardMedalAmount != null)
			rewardMedalAmount.text = rewardMedal;
	}
}
