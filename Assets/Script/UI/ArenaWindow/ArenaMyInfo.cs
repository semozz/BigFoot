using UnityEngine;
using System.Collections;

/*
public class ArenaMyInfoData
{
	public int curRank = 0;
	public int curRankOrder = 0;
	public int straightVictoryCount = 0;
	public int accumulationVictoryCount = 0;
	public int highestRank = 0;
}
*/

public class ArenaMyInfo : MonoBehaviour {
	public UILabel curRankTitle = null;
	public int curRankTiltleStringID = -1;
	public UILabel curRankName = null;
	public UILabel curRankType = null;
	
	public UILabel rankOrderTitle = null;
	public int rankOrderTitleStringID = -1;
	public UILabel rankOrder = null;
	
	public UILabel straightVictoryTitle = null;
	public int straightVictoryTitleStringID = -1;
	public UILabel straightVictoryInfo = null;
	
	public UILabel accumulationVictoryTitle = null;
	public int accumulationVictoryTitileStringID = -1;
	public UILabel accumulationInfo = null;
	
	public UILabel highestTiltle = null;
	public int highestTitleStringID = -1;
	public UILabel highestRankName = null;
	public UILabel highestRankType = null;
	
	
	public int orderUnitStringID = -1;
	public int straightUnitStringID = -1;
	public int victoryUnitStringID = -1;
	
	public ArenaInfo myInfo = null;
	
	public void Awake()
	{
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		if (stringTable != null)
		{
			if (curRankTitle != null && curRankTiltleStringID != -1)
				curRankTitle.text = stringTable.GetData(curRankTiltleStringID);
			
			if (rankOrderTitle != null && rankOrderTitleStringID != -1)
				rankOrderTitle.text = stringTable.GetData(rankOrderTitleStringID);
			
			if (straightVictoryTitle != null && straightVictoryTitleStringID != -1)
				straightVictoryTitle.text = stringTable.GetData(straightVictoryTitleStringID);
			
			if (accumulationVictoryTitle != null && accumulationVictoryTitileStringID != -1)
				accumulationVictoryTitle.text = stringTable.GetData(accumulationVictoryTitileStringID);
			
			if (highestTiltle != null && highestTitleStringID != -1)
				highestTiltle.text = stringTable.GetData(highestTitleStringID);
		}
	}
	
	public void SetMyInfo(ArenaInfo info)
	{
		myInfo = info;
		
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = null;
		ArenaRewardInfoTable arenaRewardInfoTable = null;
		if (tableManager != null)
		{
			stringTable = tableManager.stringTable;
			arenaRewardInfoTable = tableManager.arenaRewardInfo;
		}
		
		string rankType = "";
		string rankName = "";
		string curRank = "";
		string straightVictory = "";
		string totalWinningCount = "";
		string highRankType = "";
		string highRankName = "";
		
		if (info != null)
		{
			ArenaRewardData rewardData = arenaRewardInfoTable.GetRewardInfoData(info.rankType);
			if (rewardData != null)
			{
				rankType = string.Format("Rank {0}", rewardData.rankStep);
				rankName = rewardData.rankName;
			}
			
			curRank = string.Format("{0:#,###,##0}{1}", info.groupRanking, stringTable.GetData(orderUnitStringID));
			straightVictory = string.Format("{0:#,###,##0}{1}", info.winningStreakCount, stringTable.GetData(straightUnitStringID));
			totalWinningCount = string.Format("{0:#,###,##0}{1}", info.totalWinningCount, stringTable.GetData(victoryUnitStringID));
			
			rewardData = arenaRewardInfoTable.GetRewardInfoData(info.seasonBestRank);
			if (rewardData != null)
			{
				highRankType = string.Format("Rank {0}", rewardData.rankStep);
				highRankName = rewardData.rankName;
			}
		}
		
		if (curRankType != null)
			curRankType.text = rankType;
		if (curRankName != null)
			curRankName.text = rankName;
		if (rankOrder != null)
			rankOrder.text = curRank;
		if (straightVictoryInfo != null)
			straightVictoryInfo.text = straightVictory;
		if (accumulationInfo != null)
			accumulationInfo.text = totalWinningCount;
		
		if (highestRankType != null)
			highestRankType.text = highRankType;
		if (highestRankName != null)
			highestRankName.text = highRankName;
	}
}
