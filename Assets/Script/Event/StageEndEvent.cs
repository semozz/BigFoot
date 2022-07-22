using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StageEndEvent : EventStep {	
	public string StartTile = "";
	public string StartMsg = "";
	
	public EventCondition eventCondition = null;
	
	public ScrollCamera mainCamera = null;
	
	public bool rewardWindowCalled = false;
	
	public delegate void OnEvent();
	public OnEvent onComplete = null;
	public OnEvent onStageFailed = null;
	
	private bool isSuccess = false;
	public bool IsSuccess
	{
		get { return isSuccess; }
		set { isSuccess = value; }
	}
	
	// Use this for initialization
	public override void Start () {
		mainCamera = GameObject.FindObjectOfType(typeof(ScrollCamera)) as ScrollCamera;
		
		base.Start();
		
		if (this.isAutoActivate == true)
		{
			this.isActivate = true;
			
			ChangeState(EventStep.eEventStepState.Begin);
		}
	}
		
	public override void OnActivate()
	{
		base.OnActivate();
		Game.Instance.InputPause = true;
	}
	
	public void OnEventAreaEnter()
	{
		if (eventCondition != null)
			eventCondition.AddCondtionValue(1);
	}
	
	
	private bool isAlreadyStageEnd = false;
	public override void OnCompleteStep()
	{
		if (isAlreadyStageEnd == true)
			return;
		
		isAlreadyStageEnd = true;
		
		MonsterGenerator.isSurrendMode = true;
		
		base.OnCompleteStep();
		
		ActorManager actorManager = ActorManager.Instance;
		PlayerController player = null;
		if (actorManager != null)
		{
			if (actorManager.playerInfo != null)
				player = actorManager.playerInfo.gameObject.GetComponent<PlayerController>();
		}
		
		if (player != null && player.stateController != null)
		{
			player.stateController.ChangeState(BaseState.eState.Stage_clear1);
		}
		
		if (onComplete != null)
			onComplete();
	}
			
	public override void Update()
	{
		base.Update();
		if (rewardWindowCalled == true)
		{
			RewardWindowOpen();
			rewardWindowCalled = false;
		}
	}
	
	public string rewardWindowPrefabPath = "UI/Area/EndBlack";
	private StageRewardWindow window = null;
	public void RewardWindowOpen()
	{
		Transform uiRoot = null;
		if (mainCamera.Stage.uiRootPanel != null)
			uiRoot = mainCamera.Stage.uiRootPanel.transform;
		
		window = ResourceManager.CreatePrefab<StageRewardWindow>(rewardWindowPrefabPath, uiRoot);
		if (window != null)
		{
			window.isFirstClear = this.isFirstClear;
			window.stageType = this.stageType;
			window.stageIndex = Game.Instance.stageIndex + 1;
			
			window.SetEnableButtons(false);
			window.SetStageRewardItems(stageRewardItems, stageRewardItem, stageRewardMeat,
				stageRewardGold, stageRewardMaterialItemID, stageRewardIndex, stageRewardPrices);
			
			CharInfoData charData = Game.Instance.charInfoData;
			if (charData != null)
			{
				int gold = (int)charData.dropGold.x;
				int addGold = (int)charData.dropAddGold.x;
				int jewel = (int)charData.dropGold.y;
				int potion = 0;
				foreach(GainItemInfo info in charData.dropItems)
					potion += info.Count;
				
				int item = 0;
				foreach(GainItemInfo info in charData.dropMaterialItems)
					item += info.Count;
				
				int buffGold = 0;
				long buffExp = 0;
				if(charData.timeLimitBuffList != null )
				{
					foreach(TimeLimitBuffInfo info in charData.timeLimitBuffList)
					{
						if( info.buffType == AttributeValue.eAttributeType.IncGainGold )
							buffGold = (int)((float)(gold+addGold)/(1.0f + info.buffValue)*info.buffValue);
						else if( info.buffType == AttributeValue.eAttributeType.IncGainExp )
							buffExp = (long)((float)stageClearExp/(1.0f + info.buffValue)*info.buffValue);
					}
				}
				
				window.GainInfos(gold+addGold, buffGold, jewel, potion, item);
				window.SetExp(curExp, stageClearExp, buffExp, this.expTable, this.expMode);
				
				window.SetEventDropItems(charData.dropEventItems);
				window.setGemLabel(charData.jewel_Value);
			}
		}
	}
	
	public void RewardAgain(int index, int cash)
	{
		if( window != null )
		{
			window.GetAgainRewardItem(index, cash);
		}
	}
	
	public Item stageRewardItem = null;
	public List<Item> stageRewardItems = new List<Item>();
	public int stageRewardMeat = 0;
	public int stageRewardGold = 0;
	public int stageRewardMaterialItemID = 0;
	public int stageRewardIndex = -1;
	public int[] stageRewardPrices;
	public long stageClearExp = 0L;
	public long curExp = 0L;
	public bool isFirstClear = false;
	public CharExpTable expTable = null;
	public StageRewardWindow.eExpMode expMode = StageRewardWindow.eExpMode.NormalMode;
	public int stageType = 0;
	public void MakeRewardItems(int stageRewardID)
	{
		CharInfoData charInfo = Game.Instance.charInfoData;
		int charIndex = Game.Instance.connector.charIndex;
		int curLevel = 0;
		
		int usedPotion1 = 0;
		int usedPotion2 = 0;
		if (charInfo != null)
		{
			usedPotion1 = charInfo.usedPotion1;
			usedPotion2 = charInfo.usedPotion2;
		}
		
		IPacketSender packetSender = Game.Instance.packetSender;
		
		if (stageRewardID == -1)
		{
		
			stageRewardItem = null;
			stageClearExp = 0L;
		
			rewardWindowCalled = true;
			
			packetSender.SendStageEndFailed(charIndex, usedPotion1, usedPotion2);
			return;
		}
		
		stageRewardItem = null;
		stageRewardItems.Clear();
		stageClearExp = 0L;
		
		TableManager tableManager = TableManager.Instance;
		ItemTable itemTable = null;
		StageRewardTable stageRewardTable = null;
		CharExpTable expTable = null;
		CharExpTable awakeningExpTable = null;
		
		//PlayerController player = Game.Instance.player;
		//LifeManager lifeManager = player != null ? player.lifeManager : null;
		
		if (tableManager != null)
		{
			itemTable = tableManager.itemTable;
			stageRewardTable = tableManager.stageRewardTable;
			expTable = tableManager.charExpTable;
			awakeningExpTable = tableManager.awakenExpTable;
		}
		
		if (itemTable != null && stageRewardTable != null && expTable != null && awakeningExpTable != null)
		{
			int hardModeMask = 1000;
			int stageID = stageRewardID % hardModeMask;
			int stageType = (stageRewardID - stageID) / hardModeMask;
			
			StageRewardInfo info = stageRewardTable.GetData(stageID);
			StageReward rewardInfo = info != null ? info.GetStageReward(stageType) : null;
			
			if (rewardInfo != null)
			{
				Item tempItem = null;
				foreach(int itemID in rewardInfo.rewardItemIDs)
				{
					if (itemID <= 0)
						continue;
					
					tempItem = new Item();
					tempItem.SetItem(itemID);
					
					stageRewardItems.Add(tempItem);
				}
				
				PlayerController player = Game.Instance.player;
				LifeManager lifeManager = player != null ? player.lifeManager: null;
				AttributeManager attributeManager = lifeManager != null ? lifeManager.attributeManager : null;
				
				long tempExp = rewardInfo.stageExp;
				float incExpRate = attributeManager != null ? attributeManager.GetAttributeValue(AttributeValue.eAttributeType.IncGainExp) : 0.0f;
				long addExp = (long)((float)tempExp * incExpRate);
				
				stageClearExp = tempExp + addExp;
				
				charInfo.stageRewardItemID = -1;
				charInfo.stageClearExp = stageClearExp;
			}
			
			CharPrivateData privateData = charInfo.GetPrivateData(charIndex);
			if (privateData != null)
			{
				
				curExp = privateData.baseInfo.ExpValue;
				curLevel = expTable.GetLevel(curExp);
				
				int nextLevel = curLevel;
				
				if (curLevel >= expTable.maxLevel)
				{
					this.expTable = awakeningExpTable;
					this.expMode = StageRewardWindow.eExpMode.AwakeningMode;
					
					curExp = privateData.baseInfo.AExp;
					curLevel = awakeningExpTable.GetLevel(curExp);
				}
				else
				{
					this.expTable = expTable;
					this.expMode = StageRewardWindow.eExpMode.NormalMode;
					
					nextLevel = expTable.GetLevel(curExp + stageClearExp);
					
					if (curLevel < nextLevel)
						Game.Instance.ApplyAchievement(Achievement.eAchievementType.eLevelUp, nextLevel);
				}
			}
			
			GainItemInfo[] eventDropItems = charInfo.dropEventItems.ToArray();
			Game gameInstance = Game.Instance;
			if (gameInstance != null && eventDropItems != null)
			{
				foreach(GainItemInfo gainInfo in eventDropItems)
					gameInstance.ApplyAchievement(Achievement.eAchievementType.eSpecialEventItemGathering, gainInfo.ID, gainInfo.Count);
			}
			
			AchievementManager achiveMgr = null;
			if (charInfo != null)
				achiveMgr = charInfo.achievementManager;
			
			if (achiveMgr != null)
				achiveMgr.SendUpdateAchievementInfo(charIndex);
		}
		
		
		bool tutorialCheck = false;
		StageManager stageManager = null;
		if (this.mainCamera != null)
			stageManager = this.mainCamera.Stage;
		
		if (stageManager != null && stageManager.StageType == StageManager.eStageType.ST_TUTORIAL)
			tutorialCheck = true;
		
		if (packetSender != null)
		{
			if (tutorialCheck == true)
			{
				packetSender.SendTutorialEnd(charIndex);
			}
			else
			{
				int gainGold = (int)charInfo.dropGold.x;
				int gainCash = (int)charInfo.dropGold.y;
				int gainAddGold = (int)charInfo.dropAddGold.x;
				
				GainItemInfo[] gainItems = charInfo.dropItems.ToArray();
				GainItemInfo[] gainMaterialItems = charInfo.dropMaterialItems.ToArray();
				
				//GainItemInfo[] useItems = charInfo.useItems.ToArray();
				
				packetSender.SendStageEnd(charIndex, curLevel, gainGold + gainAddGold, gainCash, gainItems, gainMaterialItems, usedPotion1, usedPotion2);
			}
		}
	}
	
	public void EndArena(bool bWin, int targetUserIndex, int targetCharIndex)
	{
		IPacketSender packetSender = Game.Instance.packetSender;
		if (packetSender != null)
		{
			packetSender.SendRequestArenaEnd(bWin, targetUserIndex, targetCharIndex);
		}
	}
	
	public void OnStageClear()
	{
		//SlowEventStart();
	}
	
	public void OnStageFailed()
	{
		if (isAlreadyStageEnd == true)
			return;
		
		isAlreadyStageEnd = true;
		
		if (onStageFailed != null)
			onStageFailed();
	}
	
	public void OnStageEndResult(NetErrorCode errorCode, int charIndex, 
								int clearStageIndex, int stageType, ItemDBInfo dbInfo, long rewardExp, long totalExp,
								int rewardIndex, List<Item> rewardItemList, int rewardMeat, int rewardGold,
								int rewardMaterialItemID, int[] rewardPrices)
	{
		if (errorCode == NetErrorCode.OK)
		{
			if( dbInfo != null )
				stageRewardItem = Item.CreateItem(dbInfo);
			stageClearExp = rewardExp;
			
			stageRewardItems.Clear();
			if(rewardItemList != null)
				stageRewardItems = rewardItemList;
			stageRewardMeat = rewardMeat;
			stageRewardGold = rewardGold;
			stageRewardIndex = rewardIndex;
			stageRewardMaterialItemID = rewardMaterialItemID;
			stageRewardPrices = rewardPrices;
			
			this.stageType = stageType;
			
			CharInfoData charInfo = Game.Instance.charInfoData;
			CharPrivateData privateData = null;
			if (charInfo != null)
				privateData = charInfo.GetPrivateData(charIndex);
			
			if (privateData != null)
				isFirstClear = privateData.SetModeStageClear(stageType, clearStageIndex - 1, true);
		}
		else
		{
			stageRewardItem = null;
			stageClearExp = 0L;
			isFirstClear = false;
			
			stageType = 0;
		}
		
		rewardWindowCalled = true;
	}
	
	public string arenaEndPrefabPath = "";
	protected bool isArenaWin = false;
	public void OnArenaEnd(bool bWin)
	{
		isArenaWin = bWin;
		
		IPacketSender packetSender = Game.Instance.packetSender;
		if (packetSender != null)
		{
			CharPrivateData arenaTargetData = Game.Instance.arenaTargetInfo;
			long targetUserIndexID = Game.Instance.arenaTargetUserIndexID;
			int targetCharIndex = -1;
			if (arenaTargetData != null)
				targetCharIndex = arenaTargetData.baseInfo.CharacterIndex;
			
			packetSender.SendRequestArenaEnd(bWin, targetUserIndexID, targetCharIndex, arenaTargetData.platform);
		}
	}
	
	public void OnArenaResult(int charIndex, int RewardLeftTimeSec, ArenaInfo myNewArenaInfo)
	{
        if (myNewArenaInfo == null)
            return;

		CharPrivateData myPrivateData = null;
		CharPrivateData enemyPrivateData = Game.Instance.arenaTargetInfo;
		
		CharInfoData charData = Game.Instance.charInfoData;
		myPrivateData = charData != null ? charData.GetPrivateData(charIndex) : null;
		
		PlayerController arenaPlayer = Game.Instance.arenaPlayer;
		if (arenaPlayer != null)
			arenaPlayer.stateController.ChangeState(BaseState.eState.Stage_clear1);
		
		ArenaInfo myArenaInfo = null;
		ArenaInfo enemyArenaInfo = null;
		if (myPrivateData != null)
			myArenaInfo = myPrivateData.arenaInfo;
		if (enemyPrivateData != null)
			enemyArenaInfo = enemyPrivateData.arenaInfo;
		
		int curRankType = myArenaInfo.rankType;
		int nextRankType = myNewArenaInfo.rankType;
		
		if (myPrivateData != null)
		{
			myPrivateData.SetArenaInfo(myNewArenaInfo);
			myArenaInfo = myPrivateData.arenaInfo;
		}
		
		ArenaResultWindow arenaResult = ResourceManager.CreatePrefab<ArenaResultWindow>(arenaEndPrefabPath, mainCamera.Stage.uiRootPanel.transform, Vector3.zero);
		if (arenaResult != null)
		{
			arenaResult.InitWindow(myPrivateData, myArenaInfo, enemyPrivateData, enemyArenaInfo, isArenaWin);
			
			arenaResult.SetRankUp(curRankType, nextRankType);
		}
	}
	
	public void OnBossRaidEnd()
	{
		IPacketSender packetSender = Game.Instance.packetSender;
		if (packetSender != null)
		{
			float damageValue = Game.Instance.damageBossRaid;
			//int bossID = Game.Instance.bossID;
			long bossIndex = Game.Instance.bossIndex;
			
			bool isPhase2 = false;
			int curHP = 0;
			BaseMonster bossRaidMonster = Game.Instance.bossRaidMonster;
			if (bossRaidMonster != null)
			{
				isPhase2 = bossRaidMonster.lifeManager.isPhase2;
				curHP = (int)bossRaidMonster.lifeManager.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.Health);
			}

            packetSender.SendBossRaidEnd(bossIndex, damageValue, isPhase2, curHP, Game.Instance.ownerPlatform, Game.Instance.ownerPlatformUserID, Game.Instance.bossID);
		}
	}

	public string loadingPanelPrefabPath = "UI/LoadingPanel";
	public LoadingPanel loadingPanel = null;
	public string townStageName = "TownTest";
	public void CreateLoadingPanel()
	{
		if (loadingPanel == null)
		{
			Transform uiRoot = GameUI.Instance.uiRootPanel.transform;
			
			loadingPanel = ResourceManager.CreatePrefab<LoadingPanel>(loadingPanelPrefabPath, uiRoot, Vector3.zero);
		}
		else
		{
			loadingPanel.gameObject.SetActive(true);
			//reinforceWindow.InitMap();
		}
		
		if (loadingPanel != null)
			loadingPanel.LoadScene(townStageName, null);
	}
	
	
	public string failPopupPrefab = "UI/StageFailPopup";
	public StageFailPopup failPopup = null;
	public void OnStageFaileUI()
	{
		Time.timeScale = 1.0f;
		
		Transform uiRoot = GameUI.Instance.uiRootPanel.transform;
		
		if (failPopup == null)
			failPopup = ResourceManager.CreatePrefab<StageFailPopup>(failPopupPrefab, uiRoot, Vector3.zero);
		
		if (failPopup != null)
		{
			Game.Instance.Pause = true;
			failPopup.GambleButtonClicked += OnGoGamble;
			failPopup.ShopButtonClicked += OnGoShop;
			failPopup.TownButtonClicked += OnGoTownWhenFailed;
		}
	}
	
	public void OnGoGamble()
	{
		TownUI.firstWindowType = TownUI.eTOWN_UI_TYPE.GAMBLE;
		OnGoTown();
	}
	
	public void OnGoShop()
	{
		TownUI.firstWindowType = TownUI.eTOWN_UI_TYPE.SHOP;
		OnGoTown();
	}
	
	public void OnGoTownWhenFailed()
	{
		TownUI.openInducePopup = true;
		OnGoTown();
	}
	
	public void OnGoTown()
	{
		if (failPopup != null)
		{
			deleteEvent();
			DestroyObject(failPopup.gameObject, 0.0f);
			failPopup = null;
		}
		
		int charIndex = 0;
		if (Game.Instance.connector != null)
			charIndex = Game.Instance.connector.charIndex;
		
		int usedPotion1 = 0;
		int usedPotion2 = 0;
		CharInfoData charData = Game.Instance.charInfoData;
		if (charData != null)
		{
			usedPotion1 = charData.usedPotion1;
			usedPotion2 = charData.usedPotion2;
		}
		
		IPacketSender packetSender = Game.Instance.packetSender;
		if (packetSender != null)
			packetSender.SendStageEndFailed(charIndex, usedPotion1, usedPotion2);
		
		CreateLoadingPanel();
	}
	
	private void deleteEvent()
	{
		if (failPopup != null)
		{
			failPopup.GambleButtonClicked -= OnGoGamble;
			failPopup.ShopButtonClicked -= OnGoShop;
			failPopup.TownButtonClicked -= OnGoTown;
		}
	}
}
