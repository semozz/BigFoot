using UnityEngine;
using System.Collections;

public class ArenaWindow : PopupBaseWindow {
	public TownUI townUI = null;
	
	public UILabel titleLabel = null;
	public int titleStringID = -1;
	
	public UILabel ticketInfoLabel = null;
	public int ticketInfoStringID = -1;
	
	public ScoreWindow scoreWindow = null;
	
	public TabButtonController tabButtonController = null;
	
	public UIPopupList_New rankPopupList = null;
	
	public ArenaRewardInfoTable arenaRewardInfoTable = null;
	int refreshTicketPrice = 0;
	public override void Awake()
	{
		this.windowType = TownUI.eTOWN_UI_TYPE.ARENA;
		
		base.Awake();
		
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = null;
		StringValueTable stringValueTable = null;
		if (tableManager != null)
		{
			arenaRewardInfoTable = tableManager.arenaRewardInfo;
			stringTable = tableManager.stringTable;
			stringValueTable = tableManager.stringValueTable;
		}
		
		if (stringValueTable != null)
			refreshTicketPrice = stringValueTable.GetData("ArenaTicketPrice");
		
		if (stringTable != null)
		{
			if (titleLabel != null && titleStringID != -1)
				titleLabel.text = stringTable.GetData(titleStringID);
		}
	}
	// Use this for initialization
	void Start () {
		GameUI.Instance.arenaWindow = this;
	}
	
	void OnDestory()
	{
		GameUI.Instance.arenaWindow = null;
	}
	
	public void Update()
	{
		
	}
	
	public int curRankType = -1;
	public int myRankType = -1;
	public bool bListEnd = false;
	
	public void InitWindow(ArenaInfo arenaInfo, ArenaRankingInfo[] rankingInfos, int leftRefreshTime, int isOpen)
	{
		ArenaMyInfo myInfo = tabButtonController != null ? tabButtonController.GetViewWindow<ArenaMyInfo>(0) : null;
		if (myInfo != null)
			myInfo.SetMyInfo(arenaInfo);
		
		myRankType = curRankType = arenaInfo.rankType;
		if (scoreWindow != null)
			scoreWindow.SetScoreInfos(rankingInfos, leftRefreshTime, isOpen);
		
		UpdateTicketInfo();
		
		if (rankPopupList != null)
		{
			rankPopupList.items.Clear();
			
			RankListItemData curData = null;
			foreach(ArenaRewardData data in arenaRewardInfoTable.rewardInfos)
			{
				RankListItemData rankData = new RankListItemData();
				rankData.rankTypeName = string.Format("Rank {0}", data.rankStep);
				rankData.rankName = data.rankName;
				
				rankData.rankType = data.rankStep;
				
				if (rankData.rankType == curRankType)
					curData = rankData;
				
				rankPopupList.items.Add(rankData);
			}
			
			//if (rankPopupList.itemUI != null)
			//	rankPopupList.itemUI.SetData(curData);
			
			rankPopupList.selection = curData;
		}
		
		bool isAdjustTime = leftRefreshTime < 0;
		SetAdjustTimeMode(isOpen == 1);
		
		UpdateCoinInfo();
		
		if (TownUI.notifyOpen)
		{
			NotifyOpenPopup();
			TownUI.notifyOpen = false;
		}
	}
	
	public GameObject adjustModeObj = null;
	public GameObject normalModeObj = null;
	public void SetAdjustTimeMode(bool isOpen)
	{
		if (adjustModeObj != null && normalModeObj != null)
		{
			adjustModeObj.SetActive(!isOpen);
			normalModeObj.SetActive(isOpen);
		}
	}
	
	public void CloseStartButton()
	{
		SetAdjustTimeMode(false);
	}
	
	public void UpdateTicketInfo()
	{
		int charIndex = -1;
		CharInfoData charData = Game.Instance.charInfoData;
		CharPrivateData privateData = null;
		if (Game.Instance.connector != null)
			charIndex = Game.Instance.connector.charIndex;
		if (charData != null)
			privateData = charData.GetPrivateData(charIndex);
		
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = null;
		StringValueTable stringValueTable = null;
		
		if (tableManager != null)
		{
			stringValueTable = tableManager.stringValueTable;
			stringTable = tableManager.stringTable;
		}
		
		string ticketInfoStr = "";
		if (privateData != null)
		{
			int maxTicket = 5;
			if (stringValueTable != null)
				maxTicket = stringValueTable.GetData("MaxTicketCount");
			
			string prefixStr = "";
			if (stringTable != null)
				prefixStr = stringTable.GetData(ticketInfoStringID);

            ticketInfoStr = string.Format("{0} {1} / {2}", prefixStr, charData.ticket, maxTicket);
		}
		
		if (ticketInfoLabel != null)
			ticketInfoLabel.text = ticketInfoStr;
	}
	
	public override void OnBack()
	{
		requestCount = 0;
		base.OnBack();
		
		if (scoreWindow != null)
			scoreWindow.ResetScrollViewData();
	}
	
	public int requestCount = 0;
	public void OnGoArenaShop()
	{
		Debug.Log("Go ArenaShop....");
		requestCount = 0;
		
		OnBack();
		
		TownUI townUI = GameUI.Instance.townUI;
		if (townUI != null)
		{
			townUI.toWindowtype = this.windowType;
			townUI.OnShop();
		}
	}
	
	public string alertArenaPopupPrefabPath = "";
	public void DoAlertArenaPopup()
	{
		if (requestCount > 0)
			return;
		
		BaseConfirmPopup resetConfirmPopup = null;
		
		resetConfirmPopup = ResourceManager.CreatePrefab<BaseConfirmPopup>(alertArenaPopupPrefabPath, popupNode, Vector3.zero);
		
		if (resetConfirmPopup != null)
		{
			resetConfirmPopup.cancelButtonMessage.target = this.gameObject;
			resetConfirmPopup.cancelButtonMessage.functionName = "OnCancelPopup";
			
			resetConfirmPopup.okButtonMessage.target = this.gameObject;
			resetConfirmPopup.okButtonMessage.functionName = "OnEnterArenaByCheckTicket";
			
			popupList.Add(resetConfirmPopup);
		}
	}
	
	public void OnEnterArena(GameObject obj)
	{
		if (requestCount > 0)
			return;
		
		Debug.Log("Enter Arena....");
		
		if (scoreWindow != null)
		{
			System.TimeSpan restTime = new System.TimeSpan(0, 0, 0);
			scoreWindow.UpdateRefreshTime(out restTime);
			double totalSeconds = restTime.TotalSeconds;
			System.TimeSpan limitTime = new System.TimeSpan(0, 30, 0);
			if (totalSeconds >= 0 && totalSeconds <=limitTime.TotalSeconds)
			{
				DoAlertArenaPopup();
				return;
			}
			else
				OnEnterArenaByCheckTicket(null);
		}
	}
	
	public void OnEnterArenaByCheckTicket(GameObject obj)
	{
		//TableManager tableManager = TableManager.Instance;
		//StringValueTable stringValueTable = tableManager != null ? tableManager.stringValueTable : null;
		
		CharInfoData charData = Game.Instance.charInfoData;
		CharPrivateData privateData = null;
		int charIndex = -1;
		if (Game.Instance.connector != null)
			charIndex = Game.Instance.connector.charIndex;
		
		if (charData.ticket <= 0)
		{
			ClosePopup();
			
			OnNeedTicketPopup(refreshTicketPrice);
		}
		else
		{
			OnRequestEnterArena();
		}
	}
	
	public string needMedalPopupPrefabPath = "";
	public void OnNeedTicketPopup(int refreshTicketPrice)
	{
		if (requestCount > 0)
			return;
		
		BaseConfirmPopup resetConfirmPopup = null;
		
		resetConfirmPopup = ResourceManager.CreatePrefab<BaseConfirmPopup>(needMedalPopupPrefabPath, popupNode, Vector3.zero);
		
		if (resetConfirmPopup != null)
		{
			resetConfirmPopup.cancelButtonMessage.target = this.gameObject;
			resetConfirmPopup.cancelButtonMessage.functionName = "OnCancelPopup";
			
			resetConfirmPopup.okButtonMessage.target = this.gameObject;
			resetConfirmPopup.okButtonMessage.functionName = "OnRefreshTicket";
			
			if (resetConfirmPopup.priceValueLabel != null)
				resetConfirmPopup.priceValueLabel.text = string.Format("{0:#,###,##0}", refreshTicketPrice);
			
			popupList.Add(resetConfirmPopup);
		}
	}
	
	public void OnRefreshTicket(GameObject obj)
	{
		CashItemType checkType = CheckNeedGold(0, refreshTicketPrice, 0);
		if (checkType != CashItemType.None)
		{
			ClosePopup();
			OnNeedMoneyPopup(checkType, this);
			return;
		}
		else
		{
			IPacketSender packetSender = Game.Instance.packetSender;
			if (packetSender != null)
				packetSender.SendRequestArenaStartBuyTicket();
		}
	}
	
	public void OnRequestEnterArena()
	{
		IPacketSender packetSender = Game.Instance.packetSender;
		if (packetSender != null)
		{
			packetSender.SendRequestArenaStart();
			
			requestCount++;
		}
	}
	
	public void OnSelectionChangeRank(ListItemData data)
	{
		Debug.Log("Change RankType....");
		
		if (data.GetType() == typeof(RankListItemData))
		{
			RankListItemData rankData = (RankListItemData)data;
			
			if (rankData != null)
			{
				if (curRankType == rankData.rankType)
					return;
				
				curRankType = rankData.rankType;
				
				if (scoreWindow != null)
					scoreWindow.ResetScrollViewData();
				
				IPacketSender packetSender = Game.Instance.packetSender;
				if (packetSender != null)
				{
					packetSender.SendRequestArenaRanking(rankData.rankType, 0, true);
					
					requestCount++;
				}
			}
		}
	}
	
	public void RefreshRankList(NetErrorCode errorCode, int rankType, ArenaRankingInfo[] rankingInfos, bool bDown)
	{
		requestCount--;
		
		if (errorCode == NetErrorCode.OK)
		{
			if (scoreWindow != null)
			{
				if (bDown == true)
					scoreWindow.AddDownList(rankingInfos);
				else
					scoreWindow.AddUpList(rankingInfos);
			}
		}
	}
	
	public void RequestUpperList(int ranking)
	{
		IPacketSender packetSender = Game.Instance.packetSender;
		if (packetSender != null)
		{
			packetSender.SendRequestArenaRanking(curRankType, ranking, false);
			
			requestCount++;
		}
	}
	
	public void RequestLowerList(int ranking)
	{
		IPacketSender packetSender = Game.Instance.packetSender;
		if (packetSender != null)
		{
			packetSender.SendRequestArenaRanking(curRankType, ranking, true);
			
			requestCount++;
		}
	}
	
	public string arenaStageName = "";
	public string loadingPanelPrefabPath = "";
	public void CreateLoadingPanel(string stageName)
	{
		LoadingPanel loadingPanel = null;
	
		loadingPanel = ResourceManager.CreatePrefab<LoadingPanel>(loadingPanelPrefabPath, popupNode, Vector3.zero);
		
		CharPrivateData myPrivateData = null;
		CharPrivateData arenaEnemyPrivateData = null;
		
		CharInfoData charData = Game.Instance.charInfoData;
		int charIndex = -1;
		if (Game.Instance.connector != null)
			charIndex = Game.Instance.connector.charIndex;
		if (charData != null)
		{
			myPrivateData = charData.GetPrivateData(charIndex);
			if (myPrivateData != null && myPrivateData.baseInfo != null)
				myPrivateData.NickName = charData.NickName;
		}
		
		arenaEnemyPrivateData = Game.Instance.arenaTargetInfo;
		
		int myRankType = -1;
		int myRanking = -1;
		int enemyRankType = -1;
		int enemyRanking = -1;
		
		if (myPrivateData != null && myPrivateData.arenaInfo != null)
		{
			myRankType = myPrivateData.arenaInfo.rankType;
			myRanking = myPrivateData.arenaInfo.groupRanking;
		}
		
		if (arenaEnemyPrivateData != null && arenaEnemyPrivateData.arenaInfo != null)
		{
			enemyRankType = arenaEnemyPrivateData.arenaInfo.rankType;
			enemyRanking = arenaEnemyPrivateData.arenaInfo.groupRanking;
		}
		
		ArenaLoadingWindow arenaLoading = loadingPanel.gameObject.GetComponent<ArenaLoadingWindow>();
		if (arenaLoading != null)
			arenaLoading.InitWindow(myRankType, myRanking, myPrivateData, enemyRankType, enemyRanking, arenaEnemyPrivateData);
		
		if (loadingPanel != null)
		{
			loadingPanel.ChangeLoadingBackgroundImage(stageName);
			loadingPanel.LoadScene(stageName, this);
		}
	}
	
	public void LoadArena(PacketArenaMatchingTarget matchingInfo)
	{
		Debug.Log("Arena Window Active !!!!!!!");
		this.gameObject.SetActive(true);
		
		if (matchingInfo.errorCode == NetErrorCode.OK)
		{
			ClosePopup();
			
			Game.Instance.SetArenaPlayerInfo(matchingInfo);
			CreateLoadingPanel(arenaStageName);
			
			Game.Instance.ApplyAchievement(Achievement.eAchievementType.eArenaEnter, 0);
		}
		else
		{
			requestCount--;
		
			OnErrorMessage(matchingInfo.errorCode, this);
		}
	}
	
	public void OnTargetDetailWindow(GameObject obj)
	{
		ScoreInfoPanel scorePanel = null;
		GameObject parentObj = obj != null ? obj.transform.parent.gameObject : null;
		
		if (parentObj != null)
			scorePanel = parentObj.GetComponent<ScoreInfoPanel>();
		
		if (scorePanel != null)
		{
			long targetUserIndexID = -1;
			int targetCharIndex = -1;
            string platform = "kakao";

			switch(scorePanel.dataType)
			{
			case ScoreInfoPanel.eScoreType.eScoreInfo:
				break;
			case ScoreInfoPanel.eScoreType.eArenaRankingInfo:
				targetUserIndexID = scorePanel.arenaRankingInfo.PlatformUserId != 0 ? scorePanel.arenaRankingInfo.PlatformUserId : (long)scorePanel.arenaRankingInfo.UserIndexID;
				targetCharIndex = scorePanel.arenaRankingInfo.CharacterIndex;
                platform = scorePanel.arenaRankingInfo.platform;

				break;
			case ScoreInfoPanel.eScoreType.eWaveRankingInfo:
				targetUserIndexID = scorePanel.waveRankingInfo.PlatformUserId != 0 ? scorePanel.waveRankingInfo.PlatformUserId : (long)scorePanel.waveRankingInfo.UserIndexID;
				targetCharIndex = scorePanel.waveRankingInfo.CharacterIndex;
                platform = scorePanel.waveRankingInfo.Platform;
				break;
			}
			
			IPacketSender sender = Game.Instance.packetSender;
			if (sender != null && targetUserIndexID != -1 && targetCharIndex != -1)
			{
				if (TownUI.detailRequestCount > 0)
					return;
				
				TownUI.detailRequestCount++;
				TownUI.detailWindowRoot = this.popupNode;
			
				sender.SendRequestTargetEquipItem(targetUserIndexID, targetCharIndex, platform);
			}
		}
	}
	
	public string arenaOpenPrefabPath = "UI/ArenaOpenPopup";
	public void NotifyOpenPopup()
	{		
		BaseConfirmPopup notifyOpenPopup = null;
		notifyOpenPopup = ResourceManager.CreatePrefab<BaseConfirmPopup>(arenaOpenPrefabPath, popupNode, Vector3.one);
		if (notifyOpenPopup != null)
		{
			notifyOpenPopup.okButtonMessage.target = this.gameObject;
			notifyOpenPopup.okButtonMessage.functionName = "OnCancelPopup";
			popupList.Add(notifyOpenPopup);
		}
	}
	
	public void OnCancelPopup(GameObject obj)
	{
		ClosePopup();
	}
}
