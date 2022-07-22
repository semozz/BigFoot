using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TownUI : MonoBehaviour 
{
	public enum eTOWN_UI_TYPE
	{
		NONE = -1,
		FIREND,
		MYINFO,
		CHANGE_CHAR,
		MASTERY,
		STORAGE ,
		SHOP,
		CASH_SHOP,
		GAMBLE,
		ACHIVEMENT,
		PICTUREBOOK,
		POST,
		BIGFOOT_NOTICE,
		MAPSELECT,
		WAVE,
		ARENA,
		BOSSRAID,
		OPTION,
		EVENTSHOP,
		PACKAGEITEMSHOP,
		AWAKENING,
		RANDOMBOXSHOP,
		SPECIAL_MAP_START,
		MAXCOUNT,
	}
	public eTOWN_UI_TYPE toWindowtype = eTOWN_UI_TYPE.NONE;
	public static eTOWN_UI_TYPE firstWindowType = eTOWN_UI_TYPE.NONE;
	public static bool openInducePopup = false;
	public static int stageIndex = -1;
	public static bool notifyOpen = false;
	
	public List<TownButton> townButtonList = new List<TownButton>();
	public TownButton GetTownButton(int index)
	{
		TownButton button = null;
		int nCount = townButtonList.Count;
		if (index >= 0 && index < nCount)
			button = townButtonList[index];
		
		return button;
	}
	
	public UITexture waveAvatarTexture = null;
	public UITexture arenaAvatarTexture = null;
	public GameObject waveLocked = null;
	public GameObject arenaLocked = null;
	
	public Transform uiRoot = null;
	
	//public GameObject mapSelectPrefab = null;
	public string mapSelectPrefabPath = "";
	public MapSelect mapSelect = null;
	
	public UIButton mapSelectButton = null;
	public int requestCount = 0;
	
	void Awake()
	{
		GameUI.Instance.townUI = this;
		
		TableManager tableManager = TableManager.Instance;
		StringValueTable stringValueTabel = tableManager != null ? tableManager.stringValueTable : null;
		int addSec = 21600;
		if (stringValueTabel != null)
			addSec = stringValueTabel.GetData(StringValueKey.GambleRefreshTimeSec);
		
		//addSec = 15;
		GambleWindow.addTime = Game.ToTimeSpan(addSec);
	}
	
	void OnDestroy()
	{
		GameUI.Instance.townUI = null;
		
		if (Game.Instance != null)
		{
			if (Game.Instance.noticeItems != null)
				Game.Instance.noticeItems.Clear();
			
			if (Game.Instance.eventList != null)
				Game.Instance.eventList.Clear();
			
			CharInfoData charData = Game.Instance.charInfoData;
			EventShopInfoData eventInfo = null;
			if (charData != null)
				eventInfo = charData.GetEventShopInfo(eCashEvent.CashBonus);
			
			if (eventInfo != null)
				eventInfo.expireTime = System.DateTime.Now;
		}
	}
	
	public void OnMap()
	{
		int stageType = 0;
		if (Game.Instance != null)
			stageType = Game.Instance.lastSelectStageType;
		
		OnMapSelect(stageType);
	}
	
	public void OnMapSelect(int stageType)
	{
		if (requestCount == 0)
		{
			requestCount++;
			
			if (mapSelect == null)
			{
				mapSelect = ResourceManager.CreatePrefab<MapSelect>(mapSelectPrefabPath, uiRoot);
				
				if (mapSelect != null)
					mapSelect.townUI = this;
			}
			else
			{
				mapSelect.gameObject.SetActive(true);
			}
			
			if (mapSelect != null)
				mapSelect.InitMap(stageType);
			
			GameUI.Instance.SetCurrentWindow(mapSelect);
			
			this.gameObject.SetActive(false);
		}
	}
	
	public void OnMapSelect(int stageType, int stageID)
	{
		OnMapSelect(stageType);
		
		if (mapSelect != null)
			mapSelect.OnMapStart(stageID);
	}
	
	//public GameObject storagePrefab = null;
	public string storagePrefabPath = "";
	public StorageWindow storageWindow = null;
	public void OnStorage()
	{
		if (requestCount == 0)
		{
			requestCount++;
			
			if (storageWindow == null)
			{
				storageWindow = ResourceManager.CreatePrefab<StorageWindow>(storagePrefabPath, uiRoot);
				if (storageWindow != null)
					storageWindow.townUI = this;
			}
			else
			{
				storageWindow.gameObject.SetActive(true);
			}
			
			if (storageWindow != null)
			{
				storageWindow.InitWindow();
				
				if (TownUI.isTutorialMode == true)
					Invoke("SetStorageTutorialMode", 0.2f);
			}
			
			GameUI.Instance.SetCurrentWindow(storageWindow);
			
			TownButton townButton = GetTownButton((int)eTOWN_UI_TYPE.STORAGE);
			if (townButton != null)
				townButton.ClearBadgeNotify();
			
			this.gameObject.SetActive(false);
		}
	}
	
	public void SetStorageTutorialMode()
	{
		if (storageWindow != null)
			storageWindow.SetTutorialMode(TownUI.isTutorialMode);
		
		if (TownUI.isTutorialMode == true && tutorialController != null)
			tutorialController.NextStep();
	}
	
	//public GameObject shopPrefab = null;
	public string shopPrefabPath = "";
	public ShopWindow shopWindow = null;
	public void OnShop()
	{
		OnShopByTab(-1);
	}
	
	public void OnShopByTab(int tabIndex)
	{
		if (requestCount > 0)
			return;
		
		requestCount++;
		
		if (shopWindow == null)
		{
			shopWindow = ResourceManager.CreatePrefab<ShopWindow>(shopPrefabPath, uiRoot);
			if (shopWindow != null)
				shopWindow.townUI = this;
		}
		else
		{
			shopWindow.gameObject.SetActive(true);
		}
		
		if (shopWindow != null)
		{	
			shopWindow.InitWindow(tabIndex);
		}
		
		GameUI.Instance.SetCurrentWindow(shopWindow);
		
		TownButton townButton = GetTownButton((int)eTOWN_UI_TYPE.SHOP);
		if (townButton != null)
			townButton.ClearBadgeNotify();
		
		this.gameObject.SetActive(false);
	}
	
	public string masteryPrefabPath = "";
	public MasteryWindow_New masteryWindow_New = null;
	public void OnMastery()
	{
		if (requestCount > 0)
			return;
		
		requestCount++;
		
		if (masteryWindow_New == null)
		{
			masteryWindow_New = ResourceManager.CreatePrefab<MasteryWindow_New>(masteryPrefabPath, uiRoot, Vector3.zero);
			if (masteryWindow_New != null)
				masteryWindow_New.townUI = this;
		}
		else
			masteryWindow_New.gameObject.SetActive(true);
		
		if (masteryWindow_New != null)
			masteryWindow_New.InitWindow();
		
		GameUI.Instance.SetCurrentWindow(masteryWindow_New);
		
		TownButton townButton = GetTownButton((int)eTOWN_UI_TYPE.MASTERY);
		if (townButton != null)
			townButton.ClearBadgeNotify();
		
		this.gameObject.SetActive(false);
	}
	
	public string dummyPrefabPath = "";
	public BaseItemWindow dummyWindow = null;
	public void OnDummyWindow()
	{
		if (dummyWindow == null)
		{
			dummyWindow = ResourceManager.CreatePrefab<BaseItemWindow>(dummyPrefabPath, uiRoot, Vector3.zero);
			if (dummyWindow != null)
				dummyWindow.townUI = this;
		}
		else
		{
			dummyWindow.gameObject.SetActive(true);
		}
		
		if (dummyWindow != null)
		{	
			dummyWindow.InitWindow();
		}
		
		this.gameObject.SetActive(false);
	}
	
	public string gamblePrefabPath = "";
	public GambleWindow gambleWindow = null;
	public void OnGambleWindow()
	{
		if (requestCount > 0)
			return;
		
		requestCount++;
		
		ClientConnector connector =  Game.Instance.Connector;
		CharInfoData charData = Game.Instance.charInfoData;
		CharPrivateData privateData = null;
		int charIndex = 0;
		if (connector != null)
			charIndex = connector.charIndex;
		if (charData != null)
			privateData = charData.GetPrivateData(charIndex);
		
		System.TimeSpan timeSpan = privateData.refreshGambleExpireTime - System.DateTime.Now;
		bool hasGambleItems = false;
		if (privateData != null)
			hasGambleItems = privateData.gambleItemList.Count > 0;
		
		if (timeSpan.TotalSeconds <= 0 || hasGambleItems == false)
		{
			IPacketSender sender = Game.Instance.PacketSender;
			if (sender != null)
				sender.SendRequestGambleInfo();
		}
		else
		{
			OnGambleWindowOpen();
		}
	}
	
	public void OnGambleWindowOpen()
	{
		if (gambleWindow == null)
		{
			gambleWindow = ResourceManager.CreatePrefab<GambleWindow>(gamblePrefabPath, uiRoot);
			if (gambleWindow != null)
				gambleWindow.townUI = this;
		}
		else
		{
			gambleWindow.gameObject.SetActive(true);
		}
		
		if (gambleWindow != null)
		{	
			gambleWindow.InitWindow();
		}
		
		GameUI.Instance.SetCurrentWindow(gambleWindow);
		
		TownButton townButton = GetTownButton((int)eTOWN_UI_TYPE.GAMBLE);
		if (townButton != null)
			townButton.ClearBadgeNotify();
		
		this.gameObject.SetActive(false);
	}
	
	public void OnRequestWaveInfo()
	{
		if (requestCount > 0)
			return;
		
		requestCount++;
		
		if (Game.Instance.packetSender != null)
			Game.Instance.packetSender.SendRequestWaveInfo();
	}
	
	public string artifactPopupPrefabPath = "UI/Induce_ArtifactPopup";
	public string gamblePopupPrefabPath = "UI/Induce_GambletPopup";
	public RevivalConfirmPopup revivalPopup = null;
	public void OnOpenInducePopup()
	{		
		openInducePopup = false;
		string prefabPath = "";
		int randValue = Random.Range(1,20);
		if (randValue <= 6)
		{
			if (randValue <= 3)
			{
				prefabPath = artifactPopupPrefabPath;
				firstWindowType = eTOWN_UI_TYPE.SHOP;
			}
			else
			{
				prefabPath = gamblePopupPrefabPath;
				firstWindowType = eTOWN_UI_TYPE.GAMBLE;
			}
		}
		else
			return;
		
		revivalPopup = ResourceManager.CreatePrefab<RevivalConfirmPopup>(prefabPath, popupNode);
		if (revivalPopup != null)
		{		
			revivalPopup.cancelButtonMessage.target = this.gameObject;
			revivalPopup.cancelButtonMessage.functionName = "openInduceCancel";
			
			revivalPopup.okButtonMessage.target = this.gameObject;
			revivalPopup.okButtonMessage.functionName = "openInduceOK";
		}
	}
	
	private void openInduceCancel(GameObject obj)
	{
		firstWindowType = eTOWN_UI_TYPE.NONE;
		DestroyObject(revivalPopup.gameObject, 0.0f);
		revivalPopup = null;
	}
	
	private void openInduceOK(GameObject obj)
	{
		DestroyObject(revivalPopup.gameObject, 0.0f);
		revivalPopup = null;
		OpenFirstWindow();
	}
	
	public string wavePrefabPath = "";
	public WaveWindow waveWindow = null;
	public void OnWaveWindow(WaveRankingInfo myWaveInfo, WaveRankingInfo[] rankingInfos, int leftRefreshTime, int isClear, int isOpen)
	{
		requestCount--;
		
		if (waveWindow == null)
		{
			waveWindow = ResourceManager.CreatePrefab<WaveWindow>(wavePrefabPath, uiRoot);
			if (waveWindow != null)
				waveWindow.townUI = this;
		}
		else
		{
			waveWindow.gameObject.SetActive(true);
		}
		
		if (waveWindow != null)
		{	
			waveWindow.InitWindow(myWaveInfo, rankingInfos, leftRefreshTime, isClear, isOpen);
		}
		
		GameUI.Instance.SetCurrentWindow(waveWindow);
		
		this.gameObject.SetActive(false);
	}
	
	public string cashShopPrefabPath = "UI/Item/CashShopWindow";
	public CashShopWindow cashShopWindow = null;
	public void OnCashShop()
	{
		OnCashShop(CashItemType.CashToJewel, null);
	}
	
	public void OnCashShop(CashItemType type, PopupBaseWindow popupBase)
	{
		if (popupBase == null)
		{
			if (requestCount > 0)
				return;
			
			requestCount++;
		}
		
		if (cashShopWindow == null)
		{
			cashShopWindow = ResourceManager.CreatePrefab<CashShopWindow>(cashShopPrefabPath, uiRoot);
			if (cashShopWindow != null)
				cashShopWindow.townUI = this;
		}
		else
		{
			cashShopWindow.gameObject.SetActive(true);
		}
		
		if (cashShopWindow != null)
		{	
			cashShopWindow.InitWindow(type, popupBase);
		}
		
		GameUI.Instance.SetCurrentWindow(cashShopWindow);
		
		TownButton townButton = GetTownButton((int)eTOWN_UI_TYPE.CASH_SHOP);
		if (townButton != null)
			townButton.ClearBadgeNotify();
		
		TownUI townUI = GameUI.Instance.townUI;
		if (townUI != null)
		{
			if (townUI.toWindowtype != eTOWN_UI_TYPE.NONE)
				townUI.toWindowtype = popupBase.windowType;
		}
		//this.gameObject.SetActive(false);
	}
	
	public string arenaPrefabPath = "";
	public ArenaWindow arenaWindow = null;
	public void OnRequestArenaInfo()
	{
		if (requestCount > 0)
			return;
		
		requestCount++;
		
		if (Game.Instance.packetSender != null)
			Game.Instance.packetSender.SendRequestArenaInfo();
	}
	
	public void OnArenaWindow(ArenaInfo arenaInfo, ArenaRankingInfo[] rankingInfos, int leftRefreshTime, int isOpen)
	{
		if (arenaWindow == null)
		{
			arenaWindow = ResourceManager.CreatePrefab<ArenaWindow>(arenaPrefabPath, uiRoot);
			if (arenaWindow != null)
				arenaWindow.townUI = this;
		}
		else
		{
			arenaWindow.gameObject.SetActive(true);
		}
		
		if (arenaWindow != null)
		{	
			arenaWindow.InitWindow(arenaInfo, rankingInfos, leftRefreshTime, isOpen);
		}
		
		GameUI.Instance.SetCurrentWindow(arenaWindow);
		
		this.gameObject.SetActive(false);
		
		requestCount--;
	}
	
	public LimitTimeBuffIcon limitTimeBuffIcon = null;
	public LimitTimeBuffIcon limitTimeAwakenIcon = null;
	public void UpdateLimitTimeBuff(CharInfoData charData)
	{
		if (charData != null)
		{
			bool buffOn = false;			
			System.DateTime buffEndTime = charData.GetBuffEndTime(TimeLimitBuffItemInfo.eTimeLimitBuffItemType.GoldAndExpBuffItem);
		
			if (buffEndTime != System.DateTime.MinValue )
			{
				buffOn = true;
				if (limitTimeBuffIcon != null)
					limitTimeBuffIcon.SetTimeInfo(buffEndTime);
			}
			
			buffEndTime = charData.GetBuffEndTime(TimeLimitBuffItemInfo.eTimeLimitBuffItemType.JewelBuffItem);
		
			if (buffEndTime != System.DateTime.MinValue)
			{
				if (limitTimeAwakenIcon != null)
				{
					limitTimeAwakenIcon.SetTimeInfo(buffEndTime);
					limitTimeAwakenIcon.transform.localPosition = buffOn?new Vector3(120.0f, 295.0f, 0.0f):new Vector3(-10.0f, 295.0f, 0.0f);
				}
			}
		}
	}
	
	public void OnEnterTown()
	{
		Game.Instance.ResetFriendUpdateTime();
		
		string gameReviewURL = "";
		int attandanceCheckDay = 0;
		int charIndex = 0;
		AchievementManager achivementManager = null;
		CharInfoData charData = Game.Instance.charInfoData;
		
		if (charData != null)
		{
			attandanceCheckDay = charData.attandanceCheck;
			achivementManager = charData.achievementManager;
			gameReviewURL = charData.gameReviewURL;
			UpdateLimitTimeBuff(charData);
		}
		
		UpdateEventInfo();
		
		if (Game.Instance.connector != null)
			charIndex = Game.Instance.connector.charIndex;
		
		CharPrivateData privateData = null;
		if (charData != null)
		{
			privateData = charData.GetPrivateData(charIndex);
			
			GambleWindow.refreshExpireTime = privateData.refreshGambleExpireTime;
		}
		
		TableManager tableManger = TableManager.Instance;
		CharExpTable expTable = tableManger != null ? tableManger.charExpTable : null;
		int charLevel = 1;
		if (expTable != null && privateData != null)
			charLevel = expTable.GetLevel(privateData.baseInfo.ExpValue);
		
		int levelUpEventCheck = 0;
		if (privateData != null)
			levelUpEventCheck = privateData.levelupRewardEventCheck;
		
		bool isWaveOpen = charLevel >= 10;
		bool isArenaOpen = charLevel >= 5;
		
		TownButton waveButton = GetTownButton((int)eTOWN_UI_TYPE.WAVE);
		TownButton arenaButton = GetTownButton((int)eTOWN_UI_TYPE.ARENA);
		waveButton.GetComponent<BoxCollider>().enabled = isWaveOpen;
		arenaButton.GetComponent<BoxCollider>().enabled = isArenaOpen;
		waveAvatarTexture.color = isWaveOpen? Color.white:new Color(0.0f, 0.0f, 0.0f, 1.0f);
		arenaAvatarTexture.color = isArenaOpen? Color.white:new Color(0.0588f, 0.1176f, 0.392f, 0.784f);
		waveLocked.SetActive(!isWaveOpen);
		arenaLocked.SetActive(!isArenaOpen);
		
		if (achivementManager != null)
		{
			achivementManager.ApplyAchievement(Achievement.eAchievementType.eLevelUp, charIndex, charLevel);
			achivementManager.ApplyAchievement(Achievement.eAchievementType.eStageClear, charIndex, 0);
			achivementManager.ApplyAchievement(Achievement.eAchievementType.eArenaStraightVic, charIndex, 0);
			
			achivementManager.SendUpdateAchievementInfo(charIndex);
		}
		
		
		int nNoticeCount = Game.Instance.noticeItems.Count;
		if (attandanceCheckDay > 0)
		{
			OnAttandanceCheck(attandanceCheckDay);
		}
		else if (levelUpEventCheck > 0)
		{
			OnLevelUpRewardEvent(levelUpEventCheck);
		}
		else if (gameReviewURL != "")
		{
			OnRequestGameReview(gameReviewURL);
		}
		else if (nNoticeCount > 0 )
		{
			OnNoticePopup();
		}
		else
		{
            if (Game.Instance.appearBossID != -1)
			{
                OnBossAppear(Game.Instance.appearBossID);

                Game.Instance.appearBossID = -1;
			}
			else
			{
				if (Game.Instance.bossRaidEnd != null)
				{
					if (Game.Instance.bossRaidEnd.bClear == 1)
						OnBossRaidResult();
					else
						Game.Instance.bossRaidEnd = null;
				}
				else
					RewardInfoProcess();
			}
		}
	}
	
	public string gameReviewPrefab = "UI/Notice/GameReviewPopup";
	public BaseConfirmPopup gameReviewPopup = null;
	public void OnRequestGameReview(string url)
	{
		if (gameReviewPopup == null)
			gameReviewPopup = ResourceManager.CreatePrefab<BaseConfirmPopup>(gameReviewPrefab, popupNode, Vector3.zero);
		
		if (gameReviewPopup != null)
		{
			gameReviewPopup.cancelButtonMessage.target = this.gameObject;
			gameReviewPopup.cancelButtonMessage.functionName = "OnReviewGameCancel";
			
			gameReviewPopup.okButtonMessage.target = this.gameObject;
			gameReviewPopup.okButtonMessage.functionName = "OnReviewGameOK";
		}
	}
	
	public void CloseGameReviewPopup()
	{
		if (gameReviewPopup != null)
		{
			DestroyObject(gameReviewPopup.gameObject, 0.2f);
			gameReviewPopup = null;
		}
	}
	
	public void OnReviewGameOK(GameObject obj)
	{
		CloseGameReviewPopup();
		
		CharInfoData charData = Game.Instance.charInfoData;
		if (charData != null)
		{
			IPacketSender sender = Game.Instance.PacketSender;
			if (sender != null)
				sender.SendRequestGameReview();
			
			//Application.OpenURL(charData.gameReviewURL);
			AndroidManager androidManager = Game.Instance.AndroidManager;
			if (androidManager != null)
				androidManager.CallUnityReview("");
			
			charData.gameReviewURL = "";
		}
	}
	
	public void OnReviewGameCancel(GameObject obj)
	{
		CloseGameReviewPopup();
		
		CharInfoData charData = Game.Instance.charInfoData;
		if (charData != null)
		{
			charData.gameReviewURL = "";
		}
	}
	
	public string attandanceCheckPrefab = "UI/Event/Event_Attendance";
	public void OnAttandanceCheck(int checkDay)
	{
		AttandanceEventWindow attandanceCheckWindow = ResourceManager.CreatePrefab<AttandanceEventWindow>(attandanceCheckPrefab, popupNode, Vector3.zero);
		if (attandanceCheckWindow != null)
			attandanceCheckWindow.SetAttandanceCheck(checkDay);
	}
	
	public string levelUpRewardCheckPrefab = "UI/Event/Event_Levelup";
	public void OnLevelUpRewardEvent(int checkCount)
	{
		LevelUpEventWindow levelUpEventWindow = ResourceManager.CreatePrefab<LevelUpEventWindow>(levelUpRewardCheckPrefab, popupNode, Vector3.zero);
		if (levelUpEventWindow != null)
			levelUpEventWindow.SetAttandanceCheck(checkCount);
	}
	
	public string noticePopupPrefab = "";
	public void OnNoticePopup()
	{
		int nCount = Game.Instance.noticeItems.Count;
		if (nCount > 0)
		{
			NoticeItem notice = Game.Instance.noticeItems[0];
			NoticePopupWindow noticeWindow = ResourceManager.CreatePrefabByResource<NoticePopupWindow>(noticePopupPrefab, popupNode, Vector3.zero);
			
			if (noticeWindow != null)
				noticeWindow.SetNotice(notice);
			
			Game.Instance.noticeItems.RemoveAt(0);
		}
	}
	
	public void RewardInfoProcess()
	{
		Header header = null;
		
		List<Header> list = Game.Instance.rewardPacketList;
		if (list != null && list.Count > 0)
		{
			header = list[0];
			list.RemoveAt(0);
			
			if (header == null)
				return;
			
			switch(header.MsgID)
			{
			case NetID.ArenaReward:
				PacketArenaReward arenaReward = (PacketArenaReward)header;
				ArenaReward(arenaReward);
				break;
			case NetID.WaveReward:
				PacketWaveReward waveReward = (PacketWaveReward)header;
				WaveReward(waveReward);
				break;
			}
		}
		else
		{
			OpenFirstWindow();
		}
	}
	
	
	public static bool isTutorialMode = false;
	public TownTutorialController tutorialController = null;
	public string townTutorialPrefab = "UI/Tutorial/TownTutorialController";
	public void DestroyTutorial()
	{
		if (tutorialController != null)
		{
			DestroyObject(tutorialController.gameObject, 0.1f);
			tutorialController = null;
		}
	}
	
	public void SetTutorialMode(bool bTutorial)
	{
		UIButton button = null;
		foreach(TownButton temp in townButtonList)
		{
			if (temp == null)
				continue;
			
			button = temp.GetComponent<UIButton>();
			if (button != null)
				button.GetComponent<BoxCollider>().enabled = !bTutorial;
		}
		
		TownButton waveButton = GetTownButton((int)eTOWN_UI_TYPE.WAVE);
		TownButton arenaButton = GetTownButton((int)eTOWN_UI_TYPE.ARENA);
		waveButton.GetComponent<BoxCollider>().enabled = false;
		arenaButton.GetComponent<BoxCollider>().enabled = false;
		
		TownButton storageButton = GetTownButton((int)eTOWN_UI_TYPE.STORAGE);
		if (storageButton != null)
			button = storageButton.GetComponent<UIButton>();
		
		if (button != null)
			button.isEnabled = true;
		
		TownUI.isTutorialMode = bTutorial;
		if (bTutorial == false && this.storageWindow != null)
			this.storageWindow.SetTutorialMode(false);
	}
	
	public void OpenFirstWindow()
	{
		CharInfoData charData = Game.Instance.charInfoData;
		CharPrivateData privateData = null;
		int charIndex = 0;
		if (Game.Instance.connector != null)
			charIndex = Game.Instance.connector.charIndex;
		
		bool townTutorialSkip = false;
		bool townTutorialCheck = false;
		
		//계정이 마을 튜토리얼을 이미 했으면 마지막 단계로 스킵.
		if (charData != null)
		{
			townTutorialSkip = charData.isTutorialComplete;
			privateData = charData.GetPrivateData(charIndex);
		}
		
		//캐릭터가 튜토리얼을 끝내지 않으면 마을 튜토리얼 시작.
		if (privateData != null)
			townTutorialCheck = !(privateData.baseInfo.tutorial != 0);
		
		TownUI.isTutorialMode = townTutorialCheck;
		
		if (TownUI.isTutorialMode == true)
		{
			if (tutorialController != null)
				DestroyTutorial();
			
			tutorialController = ResourceManager.CreatePrefab<TownTutorialController>(townTutorialPrefab, null);
			
			if (tutorialController != null)
			{
				tutorialController.popupNode = this.popupNode;
				
				if (townTutorialSkip == true)
				{
					tutorialController.GoLastStep();
				}
				else
				{
					tutorialController.NextStep();
					SetTutorialMode(true);
				}
				return;
			}
		}
		
		switch(firstWindowType)
		{
		case TownUI.eTOWN_UI_TYPE.ACHIVEMENT:
			OnAchieveWindow();
			break;
		case TownUI.eTOWN_UI_TYPE.CASH_SHOP:
			OnCashShop();
			break;
		case TownUI.eTOWN_UI_TYPE.FIREND:
			OnFriendsWindow();
			break;
		case TownUI.eTOWN_UI_TYPE.GAMBLE:
			OnGambleWindow();
			break;
		case TownUI.eTOWN_UI_TYPE.MAPSELECT:
			OnMap();
			break;
		case TownUI.eTOWN_UI_TYPE.PICTUREBOOK:
			OnPictureBookWindow();
			break;
		case TownUI.eTOWN_UI_TYPE.POST:
			OnPostWindow();
			break;
		case TownUI.eTOWN_UI_TYPE.SHOP:
			OnShop();
			break;
		case TownUI.eTOWN_UI_TYPE.STORAGE:
			OnStorage();
			break;
		case TownUI.eTOWN_UI_TYPE.ARENA:
			OnRequestArenaInfo();
			break;
		case TownUI.eTOWN_UI_TYPE.WAVE:
			OnRequestWaveInfo();
			break;
		default:
			this.gameObject.SetActive(true);
			break;
		}
		
		if (firstWindowType == TownUI.eTOWN_UI_TYPE.NONE && openInducePopup == true)
			OnOpenInducePopup();
		else
			firstWindowType = TownUI.eTOWN_UI_TYPE.NONE;
	}
	
	public string bossAppearPrefabPath = "";
	public void OnBossAppear(int id)
	{
		BossAppearWindow window = ResourceManager.CreatePrefab<BossAppearWindow>(bossAppearPrefabPath, popupNode, Vector3.zero);

		if (window != null)
			window.SetBoss(id);
	}
	
	public string bossEndPrefabPath = "";
	public void OnBossRaidResult()
	{
		BossRaidResultWindow bossRaidResult = ResourceManager.CreatePrefab<BossRaidResultWindow>(bossEndPrefabPath, popupNode, Vector3.zero);
		if (bossRaidResult != null)
		{
			bossRaidResult.InitWindow();
		}
	}
	
	public string arenaRewardWindowPrefabPath = "";
	public void ArenaReward(PacketArenaReward packet)
	{
		ArenaWeekRewardWindow window = ResourceManager.CreatePrefab<ArenaWeekRewardWindow>(arenaRewardWindowPrefabPath, popupNode, Vector3.zero);
		if (window != null)
		{
			window.townUI = this;
			
			window.SetReward(packet.RewardMedal);
			window.SetMyRankInfo(packet.RankType, packet.GroupRanking);
			window.SetLastWeekFirstInfo(packet.TopInfo);
			
			bool bApplied = false;
			
			if (packet.RewardMedal > 0)
			{
				Game.Instance.ApplyAchievement(Achievement.eAchievementType.eArenaMedal, 1);
				bApplied = true;
			}
			
			//랭크1 보상적용..
			if (packet.RankType == 1)
			{
				Game.Instance.ApplyAchievement(Achievement.eAchievementType.eGetRank1, 1);
				bApplied = true;
			}
						
			if (bApplied == true)
				Game.Instance.SendUpdateAchievmentInfo();
		}
	}
	
	public string waveRewardWindowPrefabPath = "";
	public void WaveReward(PacketWaveReward packet)
	{
		WaveWeekRewardWindow window = ResourceManager.CreatePrefab<WaveWeekRewardWindow>(waveRewardWindowPrefabPath, popupNode, Vector3.zero);
		if (window != null)
		{
			window.townUI = this;
			
			window.SetReward(packet.RewardJewel);
			window.SetMyRecord(packet.Ranking, packet.RecordStep, packet.RecordSec);
			window.SetLastWeekFirstInfo(packet.TopInfo);
		}
	}
	
	public static string targetDetailWindowPrefabPath = "UI/TargetDetailWindow";
	public static Transform detailWindowRoot = null;
	public static int detailRequestCount = 0;
	
	public static void TargetDetailWindow(PacketTargetEquipItem pd)
	{
		long myUserIndexID = -1;
		if (Game.Instance.connector != null)
			myUserIndexID = Game.Instance.connector.UserIndexID;
		
		TargetDetailWindow window = ResourceManager.CreatePrefab<TargetDetailWindow>(targetDetailWindowPrefabPath, detailWindowRoot, Vector3.zero);
		if (window != null)
		{
			if (pd.errorCode == NetErrorCode.OK)
				window.InitWindow(pd.TargetCharacterIndex, pd.Account, pd.Infos, myUserIndexID, pd.TargetUserPlatform, pd.TargetUserIndexID, pd.IsFriend);
			else
			{
				TableManager tableManager = TableManager.Instance;
				StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
				
				string errorMsg = "Error!!!";
				if (stringTable != null)
					errorMsg = stringTable.GetData((int)pd.errorCode);
				if (GameUI.Instance.MessageBox != null)
					GameUI.Instance.MessageBox.SetMessage(errorMsg);
			}
		}
		
		if (pd.TargetUserIndexID == myUserIndexID)
		{
			GameUI.Instance.SetCurrentWindow(window);
		
			if (GameUI.Instance.townUI != null)
				GameUI.Instance.townUI.requestCount--;
		}
	}
	
	
	public Transform popupNode = null;
	public void OnRequestMyInfo()
	{
		if (requestCount > 0)
			return;
		
		long myUserIndexID = -1;
		int myCharIndex = -1;
		
		if (Game.Instance.connector != null)
		{
			myUserIndexID = Game.Instance.connector.UserIndexID;
			myCharIndex = Game.Instance.connector.charIndex;
		}
		
		if (myUserIndexID != -1 && myCharIndex != -1)
		{
			if (TownUI.detailRequestCount > 0)
				return;
			
			requestCount++;
			
			TownUI.detailRequestCount++;
			TownUI.detailWindowRoot = this.popupNode;
	
			PacketTargetEquipItem myInfoPacket = GetMyEquipItemInfo(myUserIndexID, myCharIndex);
			
			TargetDetailWindow(myInfoPacket);
			
			TownButton townButton = GetTownButton((int)eTOWN_UI_TYPE.MYINFO);
			if (townButton != null)
				townButton.ClearBadgeNotify();
		}
	}
	
	private PacketTargetEquipItem GetMyEquipItemInfo(long userIndexID, int charIndex)
	{
		PacketTargetEquipItem packet = new PacketTargetEquipItem();
		
		packet.errorCode = NetErrorCode.OK;
		
		packet.TargetUserIndexID = userIndexID;
		packet.TargetCharacterIndex = charIndex;
		
		CharInfoData charData = Game.Instance.charInfoData;
		ClientConnector connector = Game.Instance.Connector;
		
		if (connector != null)
			packet.Account = connector.Nick;
		
		packet.IsFriend = 0;
		packet.Infos = new TargetInfoAll[] {new TargetInfoAll(), new TargetInfoAll(), new TargetInfoAll() };
		
		for (int index = 0; index < 3; ++index)
		{
			CharPrivateData privateData = charData.GetPrivateData(index);
			TargetInfoAll privateInfo = packet.Infos[index];
			
			if (privateInfo != null && privateData != null)
			{
				privateInfo.Exp = privateData.baseInfo.ExpValue;
				
				privateInfo.equips = privateData.ToEquipItemDBInfos();
				
				if (privateData.costumeSetItem != null && privateData.costumeSetItem.setItemInfo != null)
				{
					privateInfo.costumeSetItem = new CostumeItemDBInfo();
					
					privateInfo.costumeSetItem.ID = privateData.costumeSetItem.setItemInfo.setID;
					privateInfo.costumeSetItem.UID = privateData.costumeSetItem.UID;
				}
				
				privateInfo.skills = privateData.ToSkillDBInfoFromMastery();
				privateInfo.awakenSkills = privateData.ToSkillDBInfoFromAwakening();
			}
		}
			
		return packet;
	}
	
	public GameObject optionPanel = null;
	public void OnOption()
	{
		if (optionPanel != null)
		{
			bool isActive = optionPanel.activeInHierarchy;
			optionPanel.SetActive(!isActive);
		}
	}
	
	public string friendsWindowPrefabPath = "";
	public FriendWindow friendsWindow = null;
	public void OnFriendsWindow()
	{
		if (friendsWindow == null)
		{
			friendsWindow = ResourceManager.CreatePrefab<FriendWindow>(friendsWindowPrefabPath, popupNode, Vector3.zero);
			if (friendsWindow != null)
				friendsWindow.townUI = this;
		}
		else
		{
			friendsWindow.gameObject.SetActive(true);
		}
		
		if (friendsWindow != null)
		{	
			friendsWindow.InitWindow();
		}
		
		GameUI.Instance.SetCurrentWindow(friendsWindow);
		
		TownButton townButton = GetTownButton((int)eTOWN_UI_TYPE.FIREND);
		if (townButton != null)
			townButton.ClearBadgeNotify();
	}
	
	public string achieveWindowPrefabPath = "";
	public AchievementWindow achieveWindow = null;
	public void OnAchieveWindow()
	{
		if (achieveWindow == null)
		{
			achieveWindow = ResourceManager.CreatePrefab<AchievementWindow>(achieveWindowPrefabPath, popupNode, Vector3.zero);
			if (achieveWindow != null)
				achieveWindow.townUI = this;
		}
		
		if (achieveWindow != null)
		{
			
			TownButton townButton = GetTownButton((int)eTOWN_UI_TYPE.ACHIVEMENT);
			if (townButton != null)
				townButton.ClearBadgeNotify();
			
			achieveWindow.InitWindow();
			
			achieveWindow.gameObject.SetActive(true);
		}
		
		GameUI.Instance.SetCurrentWindow(achieveWindow);
	}
	
	public string pictureBookWindowPrefabPath = "";
	public PictureBookWindow pictureBookWindow = null;
	public void OnPictureBookWindow()
	{
		if (pictureBookWindow == null)
		{
			pictureBookWindow = ResourceManager.CreatePrefab<PictureBookWindow>(pictureBookWindowPrefabPath, popupNode, Vector3.zero);
			if (pictureBookWindow != null)
				pictureBookWindow.townUI = this;
		}
		else
		{
			pictureBookWindow.gameObject.SetActive(true);
		}
		
		if (pictureBookWindow != null)
		{
			TownButton townButton = GetTownButton((int)eTOWN_UI_TYPE.PICTUREBOOK);
			if (townButton != null)
				townButton.ClearBadgeNotify();
			
			pictureBookWindow.InitWindow();
		}
		
		GameUI.Instance.SetCurrentWindow(pictureBookWindow);
	}
	
	public string noticeWindowPrefabPath = "";
	public BaseItemWindow noticeWindow = null;
	public void OnNoticeWindow()
	{
		if (noticeWindow == null)
		{
			noticeWindow = ResourceManager.CreatePrefab<BaseItemWindow>(noticeWindowPrefabPath, popupNode, Vector3.zero);
			if (noticeWindow != null)
				noticeWindow.townUI = this;
		}
		else
		{
			noticeWindow.gameObject.SetActive(true);
		}
		
		if (noticeWindow != null)
		{
			TownButton townButton = GetTownButton((int)eTOWN_UI_TYPE.BIGFOOT_NOTICE);
			if (townButton != null)
				townButton.ClearBadgeNotify();
			
			noticeWindow.InitWindow();
		}
	}
	
	public string postWindowPrefabPath = "";
	public PostWindow postWindow = null;
	public void OnRequestPostInfo()
	{
		List<MailInfo> postItemsList = Game.Instance.postItemList;
		int postRefreshGapMinitues = 5;
		
		TableManager tableManager = TableManager.Instance;
		StringValueTable stringValueTable = tableManager != null ? tableManager.stringValueTable : null;
		if (stringValueTable != null)
			postRefreshGapMinitues = stringValueTable.GetData("PostDelayTime");
		
		bool overTime = false;
		System.TimeSpan delta = System.DateTime.Now - Game.Instance.postUpdateTime;
		if (delta.TotalMinutes >= postRefreshGapMinitues)
			overTime = true;
		
		if (overTime == true)
		{
			if (requestCount > 0)
				return;
			
			requestCount++;
			
			if (Game.Instance.packetSender != null)
				Game.Instance.packetSender.SendRequestPostInfo();
		}
		else
		{
			OnPostWindow();
		}
	}
	
	public void OnPostWindow()
	{
		if (postWindow == null)
		{
			postWindow = ResourceManager.CreatePrefab<PostWindow>(postWindowPrefabPath, popupNode, Vector3.zero);
			if (postWindow != null)
				postWindow.townUI = this;
		}
		else
		{
			postWindow.gameObject.SetActive(true);
		}
		
		if (postWindow != null)
		{
			TownButton townButton = GetTownButton((int)eTOWN_UI_TYPE.POST);
			if (townButton != null)
				townButton.ClearBadgeNotify();
			
			List<MailInfo> postList = Game.Instance.postItemList;
			postWindow.InitWindow(postList);
		}
		
		GameUI.Instance.SetCurrentWindow(postWindow);
		
		if (requestCount > 0)
			requestCount--;
	}
	
	public void OnBossRaidWindow()
	{
		if (requestCount > 0)
			return;
		
		requestCount++;
		
		if (Game.Instance.packetSender != null)
			Game.Instance.packetSender.SendRequestBossRaidEnter();
	}
	
	public string bossRaidPrefabPath = "";
	public BossRaidWindow bossRaidWindow = null;
	public void OnBossRaidWindow(List<BossRaidInfo> bossRaidInfos)
	{
		if (bossRaidWindow == null)
		{
			bossRaidWindow = ResourceManager.CreatePrefab<BossRaidWindow>(bossRaidPrefabPath, popupNode, Vector3.zero);
			if (bossRaidWindow != null)
				bossRaidWindow.townUI = this;
		}
		else
		{
			bossRaidWindow.gameObject.SetActive(true);
		}
		
		if (bossRaidWindow != null)
		{
			TownButton townButton = GetTownButton((int)eTOWN_UI_TYPE.BOSSRAID);
			if (townButton != null)
				townButton.ClearBadgeNotify();
			
			bossRaidWindow.InitWindow(bossRaidInfos);
		}
		
		GameUI.Instance.SetCurrentWindow(bossRaidWindow);
	}
	
	
	public string changeCharacterStage = "SelectCharacter_New";
	public void OnChangeCharacter(GameObject obj)
	{
		Application.LoadLevel(changeCharacterStage);
	}
	
	
	public void SetBudgeNotify(int windowType, int tabIndex)
	{
		TownButton button = GetTownButton(windowType);
		if (button != null)
			button.SetBadgeNotify(tabIndex);
		
		switch((TownUI.eTOWN_UI_TYPE)windowType)
		{
		case TownUI.eTOWN_UI_TYPE.POST:
			Game.Instance.ResetPostUpdateTime();
			break;
		case TownUI.eTOWN_UI_TYPE.FIREND:
			Game.Instance.ResetFriendUpdateTime();
			break;
		}
	}
	
	public void ResetBudgeNotify(int windowType, int tabIndex)
	{
		TownButton button = GetTownButton(windowType);
		if (button != null)
			button.ClearBadgeNotify();
	}
	
	private float updateDelay = 1.0f;
	private float delayTime = 0.0f;
	public void Update()
	{
		delayTime -= Time.deltaTime;
		if (delayTime <= 0.0)
		{
			UpdateGambleTimeInfo();
			
			UpdateAchivementInfo();
			
			UpdateStorage();
			
			UpdateMastery();
			
			UpdateAwakenPoint();
			
			UpdateEventShopTimeInfo();
			
			UpdatePackageItemShopWindow();
			
			delayTime = updateDelay;
		}
	}
	
	public UILabel gambleTimeInfoLabel = null;
	public void UpdateGambleTimeInfo()
	{
		System.TimeSpan restTime = new System.TimeSpan(0, 0, 0);
		bool needRefresh = GambleWindow.UpdateRefreshTime(out restTime);
		
		string timeText = string.Format("{0:D2}:{1:D2}:{2:D2}", restTime.Hours, restTime.Minutes, restTime.Seconds);
		if (gambleTimeInfoLabel != null)
			gambleTimeInfoLabel.text = timeText;
		
		if (needRefresh == true && GambleWindow.bSendRefresh == false)
		{
			int charIndex = -1;
			if (Game.Instance.connector != null)
				charIndex = Game.Instance.connector.charIndex;
			
			GambleWindow.MakeGambleItems(GambleWindow.gambleItemList);
			
			IPacketSender packetSender = Game.Instance.packetSender;
			packetSender.SendRefreshGambleItems(false, charIndex, ref GambleWindow.gambleItemList);
			
			GambleWindow.bSendRefresh = true;
		}
	}
	
	public void UpdateAchivementInfo()
	{
		int charIndex = -1;
		CharInfoData charData = Game.Instance.charInfoData;
		if (Game.Instance.connector != null)
			charIndex = Game.Instance.connector.charIndex;
		
		if (charData != null && charData.achievementManager != null)
		{
			List<Achievement> achivementList = charData.achievementManager.GetAchievementListIncludeSpecial(charIndex, true);
			
			if (charData.achievementManager.CheckRewardList(achivementList) == true)
				SetBudgeNotify((int)TownUI.eTOWN_UI_TYPE.ACHIVEMENT, -1);
			else
				ResetBudgeNotify((int)TownUI.eTOWN_UI_TYPE.ACHIVEMENT, -1);
		}
	}
	
	public void UpdateStorage()
	{
		CharInfoData charData = Game.Instance.charInfoData;
		if (charData != null)
		{
			if (charData.CheckNewItems() == true)
				SetBudgeNotify((int)TownUI.eTOWN_UI_TYPE.STORAGE, -1);
			else
				ResetBudgeNotify((int)TownUI.eTOWN_UI_TYPE.STORAGE, -1);
		}
	}
	
	public void UpdateMastery()
	{
		CharInfoData charData = Game.Instance.charInfoData;
		CharPrivateData privateData = null;
		int charIndex = -1;
		if (Game.Instance.connector != null)
			charIndex = Game.Instance.connector.charIndex;
		
		if (charData != null)
			privateData = charData.GetPrivateData(charIndex);
		
		
		int skillPoint = 0;
		
		if (privateData != null)
			skillPoint = privateData.baseInfo.SkillPoint;
		
		if (skillPoint > 0)
			SetBudgeNotify((int)TownUI.eTOWN_UI_TYPE.MASTERY, -1);
		else
			ResetBudgeNotify((int)TownUI.eTOWN_UI_TYPE.MASTERY, -1);
	}
	
	public void UpdateAwakenPoint()
	{
		CharInfoData charData = Game.Instance.charInfoData;
		CharPrivateData privateData = null;
		int charIndex = -1;
		if (Game.Instance.connector != null)
			charIndex = Game.Instance.connector.charIndex;
		
		if (charData != null)
			privateData = charData.GetPrivateData(charIndex);
		
		int skillPoint = 0;
		
		if (privateData != null)
			skillPoint = privateData.GetAvailableAwakenPoint();
		
		if (skillPoint > 0)
			SetBudgeNotify((int)TownUI.eTOWN_UI_TYPE.AWAKENING, -1);
		else
			ResetBudgeNotify((int)TownUI.eTOWN_UI_TYPE.AWAKENING, -1);
	}
	
	
	public string optionWindowPrefab = "UI/Option/Option_Window";
	public void OnOptionWindow(GameObject obj)
	{
		OptionWindow optionWindow = ResourceManager.CreatePrefab<OptionWindow>(optionWindowPrefab, popupNode, Vector3.zero);
		if (optionWindow != null)
			optionWindow.InitWindow();
		
		GameUI.Instance.SetCurrentWindow(optionWindow);
	}
	
	public string eventShopWindowPrefab = "UI/Event/Event_Shop";
	public EventShopWindow eventShopWindow = null;
	public void OnEventShop(GameObject obj)
	{
		if (eventShopWindow == null)
			eventShopWindow = ResourceManager.CreatePrefab<EventShopWindow>(eventShopWindowPrefab, popupNode, Vector3.zero);
		else
			eventShopWindow.gameObject.SetActive(true);
		
		CharInfoData charData = Game.Instance.charInfoData;
		int buyCount = 0;
		int limitCount = 0;
		
		EventShopInfoData eventInfo = charData != null ? charData.GetEventShopInfo(eCashEvent.CashBonus) : null;
		if (eventInfo != null)
		{
			buyCount = eventInfo.buyCount;
			limitCount = eventInfo.limitCount;
		}
		
		if (eventShopWindow != null)
			eventShopWindow.InitWindow(buyCount, limitCount);
		
		GameUI.Instance.SetCurrentWindow(eventShopWindow);
	}
	
	public UILabel eventShopTimeInfoLabel = null;
	public int eventShopTimeInfoStringID = 223;
	public void UpdateEventShopTimeInfo()
	{
		CharInfoData charData = Game.Instance.charInfoData;
		
		EventShopInfoData eventShopInfo = charData != null ? charData.GetEventShopInfo(eCashEvent.CashBonus) : null;
		
		System.DateTime nowTime = System.DateTime.Now;
		System.TimeSpan leftTime = System.TimeSpan.Zero;
		
		bool bEnableEventShop = false;
		if (eventShopInfo != null)
		{
			bool buyCountCheck = eventShopInfo.buyCount < eventShopInfo.limitCount;
			bool timeCheck = false;
			
			leftTime = eventShopInfo.expireTime - nowTime;
			if (leftTime.TotalSeconds > 0)
				timeCheck = true;
			else
				leftTime = System.TimeSpan.Zero;
			
			bEnableEventShop = (buyCountCheck == true) && (timeCheck == true);
		}
		
		TownButton eventShopButton = GetTownButton((int)TownUI.eTOWN_UI_TYPE.EVENTSHOP);
		if (eventShopButton != null)
			eventShopButton.gameObject.SetActive(bEnableEventShop);
		
		if (eventShopTimeInfoLabel != null)
		{
			TableManager tableManager = TableManager.Instance;
			StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
			string timeFormatStr = "{0:D2}:{1:D2}:{2:D2}";
			if (stringTable != null && eventShopTimeInfoStringID != -1)
				timeFormatStr = stringTable.GetData(eventShopTimeInfoStringID);
			
			string timeText = string.Format(timeFormatStr, leftTime.Days, leftTime.Hours, leftTime.Minutes);
			eventShopTimeInfoLabel.text = timeText;
		}
	}
	
	
	
	public string packageItemShopWindowPrefab = "UI/Event/Starter_Package";
	PackageItemShopWindow packageItemShopWindow = null;
	public void OnPackageItemShop(GameObject obj)
	{
		if (packageItemShopWindow == null)
			packageItemShopWindow = ResourceManager.CreatePrefab<PackageItemShopWindow>(packageItemShopWindowPrefab, popupNode, Vector3.zero);
		else
			packageItemShopWindow.gameObject.SetActive(true);
		
		CharInfoData charData = Game.Instance.charInfoData;
		List<int> buyPackageItems = null;
		
		EventShopInfoData eventShop = charData != null ? charData.GetEventShopInfo(eCashEvent.CashBonus) : null;
		if (eventShop != null)
			buyPackageItems = charData.packageItems;
		
		if (packageItemShopWindow != null)
			packageItemShopWindow.InitWindow(buyPackageItems);
		
		GameUI.Instance.SetCurrentWindow(packageItemShopWindow);
	}
	
	public void UpdatePackageItemShopWindow()
	{
		TownButton packageItemShopWindow = GetTownButton((int)TownUI.eTOWN_UI_TYPE.PACKAGEITEMSHOP);
		
		bool bEnablePackageItemShop = false;
		if (Game.Instance != null)
		{
			CharInfoData charData = Game.Instance.charInfoData;
			if (charData != null)
				bEnablePackageItemShop = charData.CheckPackageItem();
			else
				bEnablePackageItemShop = false;
		}
		else
		{
			bEnablePackageItemShop = false;
		}
		
		if (packageItemShopWindow != null)
			packageItemShopWindow.gameObject.SetActive(bEnablePackageItemShop);
	}
	
	
	public string awakeningLevelWindowPrefab = "UI/MasteryWindow/AwakeningLevelWindow";
	public AwakeningLevelWindow awakeningLevelWindow = null;
	public void OnAwakeningLevelWindow()
	{
		if (awakeningLevelWindow == null)
		{
			awakeningLevelWindow = ResourceManager.CreatePrefab<AwakeningLevelWindow>(awakeningLevelWindowPrefab, uiRoot, Vector3.zero);
			if (awakeningLevelWindow != null)
				awakeningLevelWindow.townUI = this;
		}
		else
			awakeningLevelWindow.gameObject.SetActive(true);
		
		if (awakeningLevelWindow != null)
		{
			PlayerController player = Game.Instance.player;
			AwakeningLevelManager conquerorSkillManager = null;
			if (player != null && player.lifeManager != null)
				conquerorSkillManager = player.lifeManager.awakeningLevelManager;
			
			awakeningLevelWindow.InitWindow(conquerorSkillManager);
		}
	}
	
	public void UpdateEventInfo()
	{
		//GamebleRate... StaminaRate..
		TownButton townButton = GetTownButton((int)TownUI.eTOWN_UI_TYPE.GAMBLE);
		Game.EventInfo eventInfo = Game.Instance.GetEventInfo(CMSEventType.GambleRate);
		if (townButton != null)
		{
			if (eventInfo != null)
				townButton.SetEventBadge(null);
			else
				townButton.ClearEventBadge();
		}
		
		
		townButton = GetTownButton((int)TownUI.eTOWN_UI_TYPE.MAPSELECT);
		eventInfo = Game.Instance.GetEventInfo(CMSEventType.StaminaRate);
		if (townButton != null)
		{
			int staminaRateValue = eventInfo != null ? eventInfo.eventValue : 0;
			if (staminaRateValue != 0)
				townButton.SetEventBadge(eventInfo);
			else
				townButton.ClearEventBadge();
			
			MapStartWindow.staminaRate = staminaRateValue;
		}
		
		UpdateEventShopButton();
	}
	
	public GameObject eventShopButton = null;
	private void UpdateEventShopButton()
	{
		CharInfoData charData = Game.Instance.charInfoData;
		bool hasCashShopEvent = false;
		
		if (charData != null)
		{
			System.DateTime nowTime = System.DateTime.Now;
			
			foreach(var temp in charData.eventShopInfos)
			{
				EventShopInfoData eventShopInfo = temp.Value;
				if (eventShopInfo != null)
				{
					bool timeCheck = false;
					bool buyCountCheck = false;
					if (eventShopInfo.limitCount > 0)
						buyCountCheck = eventShopInfo.buyCount < eventShopInfo.limitCount;
					else
						buyCountCheck = true;
					
					System.TimeSpan leftTime = eventShopInfo.expireTime - nowTime;
					if (leftTime.TotalSeconds > 0)
						timeCheck = true;
					
					hasCashShopEvent = (buyCountCheck == true) && (timeCheck == true);
				}
				
				if (hasCashShopEvent == true)
					break;
			}
		}
		
		if (eventShopButton != null)
			eventShopButton.SetActive(hasCashShopEvent);
	}
}
