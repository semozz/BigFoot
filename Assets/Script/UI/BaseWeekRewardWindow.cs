using UnityEngine;
using System.Collections;

public class BaseWeekRewardWindow : MonoBehaviour {
	public TownUI townUI = null;
	public Transform popupNode = null;
	
	public UILabel titleLabel = null;
	public int titleStringID = -1;
	
	public UILabel rewardName = null;
	public int rewardNameStringID = -1;
	
	public UILabel rewardAmountLabel = null;
	public int rewardAmountStringID = -1;
	
	public UILabel lastWeekMyInfoLabel = null;
	public int lastWeekMyInfoStringID = -1;
	
	public UILabel rankTypeTiltleLabel = null;
	public int rankTypeTitleStringID = -1;
	public UILabel rankTypeLabel = null;
	
	public UILabel rankingTitleLabel = null;
	public int rankingTitleStringID = -1;
	public UILabel rankingLabel = null;
	
	public UILabel lastWeekFirstLabel = null;
	public int lastWeekFirstStringID = -1;
	public ScoreInfoPanel lastWeekFirst = null;
	
	public void Awake()
	{
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		SetLabel(titleLabel, titleStringID, stringTable);
		SetLabel(rewardName, rewardNameStringID, stringTable);
		SetLabel(lastWeekMyInfoLabel, lastWeekMyInfoStringID, stringTable);
		SetLabel(rankTypeTiltleLabel, rankTypeTitleStringID, stringTable);
		SetLabel(rankingTitleLabel, rankingTitleStringID, stringTable);
		SetLabel(lastWeekFirstLabel, lastWeekFirstStringID, stringTable);
		
		if (lastWeekFirst != null && lastWeekFirst.buttonMessage != null)
		{
			lastWeekFirst.buttonMessage.target = this.gameObject;
			lastWeekFirst.buttonMessage.functionName = "OnDetailView";
		}
	}
	
	public void SetLabel(UILabel label, int stringID, StringTable stringTable)
	{
		if (stringTable != null && label != null && stringID != -1)
		{
			label.text = stringTable.GetData(stringID);
		}
	}
	
	public virtual void SetReward(int amount)
	{
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		string postfixStr = "";
		if (stringTable != null && rewardAmountStringID != -1)
			postfixStr = stringTable.GetData(rewardAmountStringID);
		
		string infoStr = string.Format("{0} {1}", amount, postfixStr);
		
		if (rewardAmountLabel != null)
			rewardAmountLabel.text = infoStr;
	}
	
	public void SetLastWeekFirstInfo(ScoreInfo info)
	{
		if (lastWeekFirst != null)
			lastWeekFirst.SetInfo(info);
	}
	
	public void SetLastWeekFirstInfo(ArenaRankingInfo info)
	{
		if (lastWeekFirst != null)
			lastWeekFirst.SetInfo(info);
	}
	
	public void SetLastWeekFirstInfo(WaveRankingInfo info)
	{
		if (lastWeekFirst != null)
			lastWeekFirst.SetInfo(info);
	}
	
	public virtual void OnOK()
	{
		DestroyObject(this.gameObject, 0.0f);
		if (townUI != null)
		{
			if (GameUI.Instance.myCharInfos != null)
				GameUI.Instance.myCharInfos.UpdateValue();
			
			townUI.RewardInfoProcess();
		}
	}
	
	public void OnDetailView(GameObject obj)
	{
		ScoreInfoPanel scorePanel = null;
		GameObject parentObj = obj != null ? obj.transform.parent.gameObject : null;
		
		if (parentObj != null)
			scorePanel = parentObj.GetComponent<ScoreInfoPanel>();
		
		if (scorePanel != null)
		{
			long targetUserIndexID = -1;
			int targetCharIndex = -1;
            string targetPlatform = "kakao";
			switch(scorePanel.dataType)
			{
			case ScoreInfoPanel.eScoreType.eScoreInfo:
				break;
			case ScoreInfoPanel.eScoreType.eArenaRankingInfo:
				targetUserIndexID = scorePanel.arenaRankingInfo.UserIndexID;
				targetCharIndex = scorePanel.arenaRankingInfo.CharacterIndex;
                targetPlatform =  scorePanel.arenaRankingInfo.platform;
				break;
			case ScoreInfoPanel.eScoreType.eWaveRankingInfo:
				targetUserIndexID = scorePanel.waveRankingInfo.UserIndexID;
				targetCharIndex = scorePanel.waveRankingInfo.CharacterIndex;
                targetPlatform = scorePanel.waveRankingInfo.Platform;
				break;
			}
			
			IPacketSender sender = Game.Instance.packetSender;
			if (sender != null && targetUserIndexID != -1 && targetCharIndex != -1)
			{
				if (TownUI.detailRequestCount > 0)
					return;
				
				TownUI.detailRequestCount++;
				TownUI.detailWindowRoot = this.popupNode;

                sender.SendRequestTargetEquipItem(targetUserIndexID, targetCharIndex, targetPlatform);
			}
		}
	}
}
