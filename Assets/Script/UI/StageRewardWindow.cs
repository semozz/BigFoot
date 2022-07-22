using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class EventBoxInfo
{
	public int eventBoxID = 0;
	public string eventBoxSprite = "";
}

public class StageRewardWindow : MonoBehaviour {
	public string stageName = "TownTest";
	
	public bool isFirstClear = false;
	public int stageType = 0;
	public int stageIndex = -1;
	
	private bool bAnimationFinished = false;
	
	public UISlider expProgressBar = null;
	
	public List<GameObject> itemSlots = new List<GameObject>();
	private List<int> itemSlotIndex = new List<int>();
	public List<RewardItemSlot> rewardItemSlots = new List<RewardItemSlot>();
	private List<int> getSlots = new List<int>();
	private List<string> itemNames = new List<string>();
	
	private bool expUpdateComplete = false;
	
	public UILabel gemLabel = null;
	public UILabel gainBasicGoldLabel = null;
	public UILabel gainBuffGoldLabel = null;
	public UILabel gainTotalGoldLabel = null;
	public UILabel gainBasicExpLabel = null;
	public UILabel gainBuffExpLabel = null;
	public UILabel gainTotalExpLabel = null;
	public UILabel gainJewelLabel = null;
	public UILabel gainItemLabel = null;
	public UILabel getItemLabel = null;
	public UILabel retryLabel = null;
	public UILabel rewardAgainLabel = null;
	public UILabel nextLabel = null;
	
	public UIButton retryBtn = null;
	public UIButton nextBtn = null;
	public UIButton getRewardAgainBtn = null;
	public UIButton okBtn = null;
	
	public UIPanel resultPanel = null;
	public UIPanel rewardPanel = null;
	public UISprite buffGoldSprite = null;
	public UISprite buffExpSprite = null;
	private string buffIconOff = "Reward_Buff_Icon01";
	private string buffIconOn = "Reward_Buff_Icon02";
	
	private float selectAniTime = 0.0f;
	public float selectWaitTime = 0.0f;
	private bool isSelectPlaying = false;
	private int currentRewardSlotIndex = 0;
	private float selectAniStart = 2.0f;
	
	private StringTable stringTable = null;
	private bool isRewardStage = false;
	private int gemNum = 0;
	private int selectIndex = -1;
	
	public AudioClip battleWinClip = null;
	public AudioClip rewardPageClip = null;
	public AudioClip getItemClip = null;
	public AudioClip selectItemClip = null;
	
	private float rewardOpenTime = -1.0f;
	
	public enum eExpMode
	{
		NormalMode,
		AwakeningMode
	}
	
	public GameObject eventRoot = null;
	public UILabel eventBoxAmount = null;
	public UISprite eventBoxSprite = null;
	public List<EventBoxInfo> eventBoxInfos = new List<EventBoxInfo>();
	public EventBoxInfo GetEventBoxInfo(int boxID)
	{
		EventBoxInfo info = null;
		foreach(EventBoxInfo temp in eventBoxInfos)
		{
			if (temp.eventBoxID == boxID)
			{
				info = temp;
				break;
			}
		}
		
		return info;
	}
	
	private void UpdateExp()
	{
		float targetValue = 1.0f;
		if (curLevel < endLevel)
		{
			targetValue = 1.0f;
		}
		else
			targetValue = endExpRate;
		
		
		float curValue = 0.0f;
		if (expProgressBar != null)
			curValue = expProgressBar.sliderValue;
		
		if (Mathf.Abs(curValue - targetValue) <= 0.01f)
		{
			if (curLevel == endLevel)
			{
				expUpdateComplete = true;
				if (gainTotalExpLabel != null)
					gainTotalExpLabel.text = string.Format("{0:0.#}%", targetValue * 100.0f);
			}
			else
			{
				curLevel++;
				curValue = 0.0f;
				
				if (expProgressBar != null)
					expProgressBar.sliderValue = curValue;
				if (gainTotalExpLabel != null)
					gainTotalExpLabel.text = string.Format("{0:0.#}%", curValue * 100.0f);
			}
		}
		else
		{
			curValue = Mathf.Lerp(curValue, targetValue, 0.3f);
			
			if (expProgressBar != null)
				expProgressBar.sliderValue = curValue;
			if (gainTotalExpLabel != null)
				gainTotalExpLabel.text = string.Format("{0:0.#}%", curValue * 100.0f);
		}
	}
	
	/*
	void Awake()
	{
		SetExp(0, 10000); 
	}
	*/
	public void Update()
	{	
		if (bAnimationFinished == true)
		{
			if (expUpdateComplete == false)
				UpdateExp();
		}
		
		if( selectAniTime > 0.0f )
		{
			if( selectWaitTime > 0.0f )
			{
				selectWaitTime -= Time.deltaTime;
			}
			else
			{
				selectAniTime -= Time.deltaTime;
				int index = getSelectSlotIndex(selectAniTime);
				if (selectIndex != index)
					rewardSelectedActive(index, false);
				isSelectPlaying = true;
			}
		}
		else if( isSelectPlaying )
		{
			isSelectPlaying = false;
			int index = itemSlotIndex[currentRewardIndex];
			rewardSelectedActive(index, true);
			rewardItemSlots[index].SetSelectedActive(false);
			rewardGetActive(currentRewardIndex);
			rewardItemSlots[index].PlayFXLight();
			SetEnableButtons(true);
		}
		
		if (rewardOpenTime > 0.0f)
		{
			rewardOpenTime -= Time.deltaTime;
			if (rewardOpenTime < 1.0f)
			{
				rewardOpenTime = -1.0f;
				SelectRewardItem();
			}
		}
	}
	
	private int getSelectSlotIndex(float aniTime)
	{
		int index = getSlots.Count-1-(int)(aniTime*aniTime*4)%getSlots.Count;
		index += currentRewardSlotIndex;
		return getSlots[index%getSlots.Count];
	}
	
	public string CheckBossClear(string stageName)
	{
		string bgName = "";
		if (stageName.Contains("act") == true)
		{
			string numberStr = stageName.Substring(3);
			
			int stageNumber = 0;
			if (numberStr.Length > 0)
				stageNumber = int.Parse(numberStr);
			
			int index = -1;
			if (stageNumber % 10 == 0)
				index = (stageNumber / 10) - 1;
			
			if (index != -1)
				bgName = "bossClear";
		}
		else if (stageName == "Wave")
		{
			bgName = "Clear_Defence";
		}
		else if (stageName == "BossRaid")
		{
			bgName = "Clear_BossRaid";
		}
		
		return bgName;
	}
	
	public string bossClearPrefabPath = "";
	public Vector3 bossPopupPos = new Vector3(0.0f, 0.0f, -210.0f);
	public void CreateBossClear(string stageName, int stageType, bool isFirstClear)
	{
		BossClearWindow bossClear = ResourceManager.CreatePrefab<BossClearWindow>(bossClearPrefabPath, popupNode, bossPopupPos);
		if (bossClear != null)
		{
			bossClear.SetStageName(stageName, stageType, isFirstClear);
			
			bossClear.parentObj = this.gameObject;
		}
	}
	
	public void OnFinishBossClear()
	{
		OnLevelUpWindow();
	}
	
	public void OnLevelUpWindow()
	{
		if (isLevelUped == true)
		{
			switch(this.expMode)
			{
			case eExpMode.NormalMode:
				CreateLevelUpWindow(levelUpWindowPrefabPath);
				break;
			case eExpMode.AwakeningMode:
				CreateLevelUpWindow(awakeningLevelUpWindowPrefab);
				break;
			}
		}
		else
			OnLevelUpFinished();
	}
	
	public Transform popupNode = null;
	public string levelUpWindowPrefabPath = "";
	public string awakeningLevelUpWindowPrefab = "";
	public int warriorCharNameStringID = -1;
	public int assassinCharNameStringID = -1;
	public int wizardCharNameStringID = -1;
	
	public void CreateLevelUpWindow(string prefabPath)
	{
		LevelUpWindow levelUpWindow = ResourceManager.CreatePrefab<LevelUpWindow>(prefabPath, popupNode, Vector3.zero);
		
		CharInfoData charData = Game.Instance.charInfoData;
		CharPrivateData privateData = null;
		int charIndex = -1;
		if (Game.Instance.connector != null)
			charIndex = Game.Instance.connector.charIndex;
		
		if (charData != null)
			privateData = charData.GetPrivateData(charIndex);
		
		string charNameStr = "";
		string userIDStr = "";
		List<EquipInfo> equipItems = null;
		CostumeSetItem costumeSetItem = null;
		
		if (privateData != null)
		{
			equipItems = privateData.equipData;
			costumeSetItem = privateData.costumeSetItem;
			
			int charNameStringID = -1;
			switch(privateData.baseInfo.CharacterIndex)
			{
			case 0:
				charNameStringID = warriorCharNameStringID;
				break;
			case 1:
				charNameStringID = assassinCharNameStringID;
				break;
			case 2:
				charNameStringID = wizardCharNameStringID;
				break;
			}
			
			TableManager tableManager = TableManager.Instance;
			StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
			
			if (stringTable != null)
				charNameStr = stringTable.GetData(charNameStringID);
			
			userIDStr = privateData.NickName;
		}
		
		if (levelUpWindow != null)
		{
			levelUpWindow.parentObj = this.gameObject;
			levelUpWindow.SetCharInfo(curLevel, charNameStr, userIDStr);
			levelUpWindow.SetNextLevel(endLevel);
			
			levelUpWindow.SetAvatar((GameDef.ePlayerClass)charIndex, equipItems, costumeSetItem);
		}
	}
	
	public void OnLevelUpFinished()
	{
		CreateLoadingPanel(stageName);
		MonsterGenerator.isSurrendMode = false;
		
		DestroyObject(this.gameObject, 0.0f);
	}
	
	public string loadingPanelPrefabPath = "";
	public LoadingPanel loadingPanel = null;
	public void CreateLoadingPanel(string stageName)
	{
		StageManager stageManger = Game.Instance.stageManager;
		if (stageManger != null && stageManger.StageType == StageManager.eStageType.ST_TUTORIAL)
			TownUI.firstWindowType = TownUI.eTOWN_UI_TYPE.NONE;
		else
			TownUI.firstWindowType = TownUI.eTOWN_UI_TYPE.MAPSELECT;
		
		TownUI.notifyOpen = isNotifyOpenMode();
		if (TownUI.notifyOpen)
		{
			if (endLevel==arenaOpenLevel)
				TownUI.firstWindowType = TownUI.eTOWN_UI_TYPE.ARENA;
			else if (endLevel==waveOpenLevel)
				TownUI.firstWindowType = TownUI.eTOWN_UI_TYPE.WAVE;
			else
				Game.Instance.lastSelectStageType = endLevel == hardOpenLevel? 1 : 2;
		}
		
		if (loadingPanel == null)
		{
			Transform uiRoot = this.transform.parent;
			loadingPanel = ResourceManager.CreatePrefab<LoadingPanel>(loadingPanelPrefabPath, uiRoot, Vector3.zero);
		}
		else
		{
			loadingPanel.gameObject.SetActive(true);
			//reinforceWindow.InitMap();
		}
		
		if (loadingPanel != null)
			loadingPanel.LoadScene(stageName, null);
	}
	
	private void playAgain()
	{
		TownUI.stageIndex = stageIndex;
		townTestLoading();
	}
	
	private void townTestLoading()
	{			
		resultPanel.gameObject.SetActive(false);
		string stageName = Game.Instance.stageName;
		if (stageName.StartsWith("act") == true)
		{
			string numberStr = stageName.Substring(3);
			int hardModeMask = 1000;
			int actID = 0;
			if (numberStr.Length > 0)
				actID = int.Parse(numberStr);
			
			actID = actID % hardModeMask;
			stageName = string.Format("act{0}", actID);
		}
		
		string bossClear = CheckBossClear(stageName);
		if (bossClear != "")
		{
			CreateBossClear(stageName, stageType, isFirstClear);
		}
		else
		{
			OnLevelUpWindow();
		}
	}
	
	public void OnFinished()
	{
		bAnimationFinished = true;
	}
	
	public void SetEnableButtons(bool bEnable)
	{
		retryBtn.isEnabled = bEnable && !isNotifyOpenMode();
		getRewardAgainBtn.isEnabled = bEnable && getSlots.Count != 0;
		nextBtn.isEnabled = bEnable;
	}
	
	public int arenaOpenLevel = 5;
	public int waveOpenLevel = 10;	
	public int hardOpenLevel = 7;
	public int hellOpenLevel = 40;	
	private bool isNotifyOpenMode()
	{
		return expMode == eExpMode.NormalMode && isLevelUped 
			&& (endLevel==hardOpenLevel || endLevel==hellOpenLevel
			|| endLevel==arenaOpenLevel || endLevel==waveOpenLevel);
	}
	
	public void SetActiveButtons(bool bActive)
	{
		retryBtn.gameObject.SetActive(bActive);
		getRewardAgainBtn.gameObject.SetActive(bActive);
		nextBtn.gameObject.SetActive(bActive);
	}
	
	public string itemSlotPrefabPath = "";
	public string rewardItemSpritePrefabPath = "";
	public string meatSpriteName = "";
	public string goldSpriteName = "";
	private int currentRewardIndex = 0;
	private int meatNum = 0;
	private int goldNum = 0;
	private int[] prices;
	public void SetStageRewardItems(List<Item> items, Item rewardItem, int rewardMeat, int rewardGold, int rewardMaterialItemID, int rewardIndex, int[] rewardPrices)
	{
		NGUITools.PlaySound(battleWinClip);
		isRewardStage = items.Count > 0;
		if(isRewardStage)
		{
			for (int i = 0; i < itemSlots.Count; i++)
			{
				itemSlotIndex.Add(i);
				getSlots.Add(i);
			}
			
			for (int i = 0; i < itemSlots.Count; i++) {
        		int temp = itemSlotIndex[i];
        		int randomIndex = Random.Range(i, itemSlotIndex.Count);
        		itemSlotIndex[i] = itemSlotIndex[randomIndex];
        		itemSlotIndex[randomIndex] = temp;
    		}
			
			for (int i = 0; i < items.Count; i++)
				setItemSlot(i, items[i]);
			
			TableManager tableManager = TableManager.Instance;
			stringTable = tableManager != null ? tableManager.stringTable : null;
			
			setRewardItemSprite(3, meatSpriteName, rewardMeat);
			itemNames.Add( stringTable.GetData(206) + " " + GetLabelNumber(rewardMeat).ToString() );
			setRewardItemSprite(4, goldSpriteName, rewardGold);
			itemNames.Add( stringTable.GetData(17) + " " + GetLabelNumber(rewardGold).ToString() );
			meatNum = rewardMeat;
			goldNum = rewardGold;
			prices = rewardPrices;
			Item materialItem = Item.CreateItem(rewardMaterialItemID, "", 0, 0, 1);
			setItemSlot(5, materialItem);
			
			currentRewardIndex = rewardIndex;
			rewardAgainLabel.text = string.Format( stringTable.GetData(278), prices[0] );
			retryLabel.text = stringTable.GetData(276);
			nextLabel.text = stringTable.GetData(271);
		}
		SetActiveButtons(false);
		SetEnableButtons(false);
		rewardPanel.gameObject.SetActive(false);
	}
	
	private void setItemSlot(int i, Item item)
	{
		itemNames.Add( item.GetItemName() );
		ItemSlot itemSlot = ResourceManager.CreatePrefab<ItemSlot>(itemSlotPrefabPath, itemSlots[itemSlotIndex[i]].transform, Vector3.zero);
		itemSlot.SetItem(item);
		itemSlot.SetButtonActive(false);
		rewardItemSlots[itemSlotIndex[i]].ResetSlot();
	}
	
	private void setRewardItemSprite(int i, string name, int num)
	{
		RewardItem rewardItem = ResourceManager.CreatePrefab<RewardItem>(rewardItemSpritePrefabPath,
			itemSlots[itemSlotIndex[i]].transform, Vector3.zero);
		rewardItem.SetItem(name, num);
		rewardItemSlots[itemSlotIndex[i]].ResetSlot();
	}
	
	public void GetAgainRewardItem(int index, int cash)
	{
		currentRewardIndex = index;
		setGemLabel(cash);
		SelectRewardItem();
	}
	
	public void RewardPanelOK()
	{
		if( isRewardStage )
		{
			NGUITools.PlaySound(rewardPageClip);
			for (int i = 0; i < rewardItemSlots.Count; i++)
				rewardItemSlots[i].SetSelectedActive(false);
			rewardPanel.gameObject.SetActive(true);
			okBtn.gameObject.SetActive(false);
			rewardOpenTime = 1.1f;
		}
		else
			townTestLoading();
	}
	
	public void SelectRewardItem()
	{
		okBtn.gameObject.SetActive(false);
		selectAniTime = getSlots.Count == 1? 0.02f : selectAniStart;
		for (int i = 0; i < getSlots.Count; i++)
		{
			if(itemSlotIndex[currentRewardIndex] == getSlots[i])
				currentRewardSlotIndex = i;
		}
		SetActiveButtons(true);
		SetEnableButtons(false);
	}
	
	private void rewardSelectedActive(int index, bool bSelected)
	{
		selectIndex = index;
		if (bSelected)
			NGUITools.PlaySound(getItemClip);
		else
			NGUITools.PlaySound(selectItemClip);
		
		for (int i = 0; i < rewardItemSlots.Count; i++)
			rewardItemSlots[i].SetSelectedActive(i==index);
		
		string description = "";
		if(bSelected)
			description = "    " + stringTable.GetData(279);
		for (int i = 0; i < itemSlotIndex.Count; i++)
			if(index == itemSlotIndex[i])
				getItemLabel.text = itemNames[i] + description;
	}
	
	private void rewardGetActive(int currentRewardIndex)
	{
		int index = itemSlotIndex[currentRewardIndex];
		getSlots.Remove(index);
		if( getSlots.Count > 0 )
			rewardAgainLabel.text = string.Format( stringTable.GetData(278), getCurrentPrice());
		if(currentRewardIndex == 3)
		{
			CharInfoData charData = Game.Instance.connector != null ? Game.Instance.connector.charInfo : null;
			if (charData != null)
				charData.potion2 += meatNum;
		}
		rewardItemSlots[index].SetGetActive(true);
	}
	
	private int getCurrentPrice()
	{
		return prices[rewardItemSlots.Count - getSlots.Count - 1];
	}
	
	public void setGemLabel(int num)
	{
		gemLabel.text = GetLabelNumber(num);
		gemNum = num;
	}
	
	public float startExpRate = 0.0f;
	public float endExpRate = 0.0f;
	
	public int curLevel = 0;
	public int endLevel = 0;
	
	public bool isLevelUped = false;
	private eExpMode expMode = eExpMode.NormalMode;
	public void SetExp(long curExp, long stageClearExp, long buffExp, CharExpTable expTable, eExpMode expMode)
	{
		this.expMode = expMode;
		
		if (expTable != null)
		{
			startExpRate = expTable.GetProgressRate(curExp);
			curLevel = expTable.GetLevel(curExp);
			
			endExpRate = expTable.GetProgressRate(curExp + stageClearExp);
			endLevel = expTable.GetLevel(curExp + stageClearExp);
			
			isLevelUped = endLevel > curLevel;
			
			if (expProgressBar != null)
				expProgressBar.sliderValue = startExpRate;
			if (gainTotalExpLabel != null)
				gainTotalExpLabel.text = string.Format("{0:0.#}%", startExpRate * 100.0f);
		}
		if (gainBasicExpLabel != null)
			gainBasicExpLabel.text = GetLabelPlusNumber(stageClearExp - buffExp);
		if (gainBuffExpLabel != null)
			gainBuffExpLabel.text = GetLabelPlusNumber(buffExp);
		
		if(buffExp > 0L)
			buffExpSprite.spriteName = buffIconOn;
		else
			buffExpSprite.spriteName = buffIconOff;			
		
		expUpdateComplete = false;
	}
	
	public void GainInfos(int addGold, int buffGold, int jewel, int potion, int item)
	{
		if(buffGold > 0L)
			buffGoldSprite.spriteName = buffIconOn;
		else
			buffGoldSprite.spriteName = buffIconOff;	
		
		if (gainBasicGoldLabel != null)
			gainBasicGoldLabel.text = GetLabelPlusNumber(addGold - buffGold);
		if (gainBuffGoldLabel != null)
			gainBuffGoldLabel.text = GetLabelPlusNumber(buffGold);
		if (gainTotalGoldLabel != null)
			gainTotalGoldLabel.text = GetLabelPlusNumber(addGold);
		if (gainJewelLabel != null)
			gainJewelLabel.text = GetLabelNumber(jewel);
		if (gainItemLabel != null)
			gainItemLabel.text = GetLabelNumber(item);
		
	}
	
	public string GetLabelPlusNumber(int val)
	{
		if( val > 0 )
			return string.Format("+{0:#,###,###}", val);
		else
			return "0";
	}
	
	public string GetLabelPlusNumber(long val)
	{
		if( val > 0 )
			return string.Format("+{0:#,###,###}", val);
		else
			return "0";
	}
	
	public string GetLabelNumber(int val)
	{
		return string.Format("{0:#,###,##0}", val);
	}
	
	public void SetEventDropItems(List<GainItemInfo> dropEventItems)
	{
		int eventBoxID = -1;
		int eventBoxCount = 0;
		
		int eventID = Game.Instance.GetEventID();
		if (eventID != -1)
		{
			Dictionary<int, int> eventDrops = new Dictionary<int, int>();
			foreach(GainItemInfo info in dropEventItems)
			{
				if (eventDrops.ContainsKey(info.ID) == false)
				{
					eventDrops.Add(info.ID, info.Count);
				}
				else
				{
					int newCount = eventDrops[info.ID] + info.Count;
					eventDrops[info.ID] = newCount;
				}
			}
			
			foreach(var temp in eventDrops)
			{
				if (eventBoxCount < temp.Value)
				{
					eventBoxID = temp.Key;
					eventBoxCount = temp.Value;
				}
			}
		}
		
		if (eventRoot != null)
			eventRoot.SetActive(eventBoxCount > 0);
		
		if (eventBoxCount > 0)
		{
			EventBoxInfo info = GetEventBoxInfo(eventBoxID);
			if (info != null && eventBoxSprite != null)
				eventBoxSprite.spriteName = info.eventBoxSprite;
			
			if (eventBoxAmount != null)
				eventBoxAmount.text = string.Format("X{0}", eventBoxCount);
		}
	}
	
	private void requestRewardItem()
	{
		if(getCurrentPrice() > gemNum)
		{
			OpenCashShop(CashItemType.CashToJewel);
		}
		else
		{
			int charIndex = -1;
			if (Game.Instance.connector != null)
				charIndex = Game.Instance.connector.charIndex;
			IPacketSender packetSender = Game.Instance.packetSender;
			packetSender.SendStageReward(charIndex, stageType, stageIndex, getCurrentPrice());
		}
	}
	
	public void UpdateCash()
	{
		int ownJewel = 0;
		
		if (Game.Instance != null && Game.Instance.charInfoData != null)
			ownJewel = Game.Instance.charInfoData.jewel_Value;
		
		setGemLabel(ownJewel);
	}
	
	string cashShopPrefabPath = "UI/Item/CashShopWindow";
	CashShopWindow cashShopWindow = null;
	public void OpenCashShop(CashItemType checkType)
	{
		if (cashShopWindow == null)
			cashShopWindow = ResourceManager.CreatePrefab<CashShopWindow>(cashShopPrefabPath, this.popupNode);
		
		if (cashShopWindow != null)
		{
			cashShopWindow.onCashShopClose = new BaseCashShopWindow.OnCashShopClose(UpdateCash);
			
			cashShopWindow.gameObject.SetActive(true);
			cashShopWindow.InitWindow(checkType, null);
		}
	}
}
