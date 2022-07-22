using UnityEngine;
using System.Collections;

public class WaveRewardInfo : MonoBehaviour {
	public UILabel rankName = null;
	public int rankUnitStringID = -1;
	public int lessRankUnitStringID = -1;
	
	public UILabel rewardAmount = null;
	
	public void SetInfo(WaveRewardData data)
	{
		string rankNameStr = "";
		string rewardMedal = "";
		
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		if (data != null)
		{
			if (data.minOrder == data.maxOrder)
			{
				rankNameStr = string.Format("{0}{1}", data.minOrder, stringTable.GetData(rankUnitStringID));
			}
			else if (data.maxOrder == -1)
			{
				rankNameStr = string.Format("{0}{1} {2}", data.minOrder, stringTable.GetData(rankUnitStringID), stringTable.GetData(lessRankUnitStringID));
			}
			else
			{
				rankNameStr = string.Format("{0}~{1}{2}", data.minOrder, data.maxOrder, stringTable.GetData(rankUnitStringID));
			}
			
			rewardMedal = string.Format("{0:#,###,##0}", data.rewardJewel);
		}
		
		if (rankName != null)
			rankName.text = rankNameStr;
		
		if (rewardAmount != null)
			rewardAmount.text = rewardMedal;
	}
}
