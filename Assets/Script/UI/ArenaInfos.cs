using UnityEngine;
using System.Collections;

public class ArenaInfos : MonoBehaviour {
	public UILabel titleLabel = null;
	public int titleStringID = -1;
	
	public UILabel arenaInfoLabel = null;
	
	public int rankTitleStringID = -1;
	
	public int orderTitleStringID = -1;
	public int orderUnitStringID = -1;
	
	public int straitVictoryTitleStringID = -1;
	public int straightUnitStringID = -1;
	
	public int totalVictoryTitleStringID = -1;
	public int victoryUnitStringID = -1;
	
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
	
	public void SetInfo(ArenaInfo info)
	{
		string infoStr = "";
		
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = null;
		ArenaRewardInfoTable arenaRewardInfoTable = null;
		if (tableManager != null)
		{
			stringTable = tableManager.stringTable;
			arenaRewardInfoTable = tableManager.arenaRewardInfo;
		}
		
		string rankTypeStr = "";
		ArenaRewardData rewardData = arenaRewardInfoTable.GetRewardInfoData(info.rankType);
		if (rewardData != null)
			rankTypeStr = string.Format("{0} : {1}", stringTable.GetData(rankTitleStringID), rewardData.rankName);
		
		string curRanking = string.Format("{0} : {1:#,###,##0}{2}", stringTable.GetData(orderTitleStringID), info.groupRanking, stringTable.GetData(orderUnitStringID));
		string straightVictory = string.Format("{0} : {1:#,###,##0}{2}", stringTable.GetData(straitVictoryTitleStringID), info.winningStreakCount, stringTable.GetData(straightUnitStringID));
		string totalWinningCount = string.Format("{0} : {1:#,###,##0}{2}", stringTable.GetData(totalVictoryTitleStringID), info.totalWinningCount, stringTable.GetData(victoryUnitStringID));
		
		infoStr = string.Format("{0}\n{1}\n{2}\n{3}", rankTypeStr, curRanking, straightVictory, totalWinningCount);
		if (arenaInfoLabel != null)
			arenaInfoLabel.text = infoStr;
	}
}
