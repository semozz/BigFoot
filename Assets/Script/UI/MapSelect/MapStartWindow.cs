using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ModeTypeInfo
{
	public int stringID;
	public string spriteName;
	public Color modeColor;
}

public class MapStartWindow : PopupBaseWindow {
	public UILabel titleLabel = null;
	public int titleStringID = -1;
	
	public List<GameObject> kingdomObjs = new List<GameObject>();
	
	public UILabel modeTypeLabel = null;
	public List<ModeTypeInfo> modeTypeInfos = new List<ModeTypeInfo>();
	
	public UILabel kingdomNameLabel = null;
	public UILabel stageNameLabel = null;
	
	public UILabel stageTitleLabel = null;
	public int stageTitleStringID = -1;
	
	public UILabel stageRewardTitleLabel = null;
	public int stageRewardTitleStringID = -1;
	
	public string itemPrefabPath = "UI/Item/ItemSlot";
	public List<Transform> itemInfos = new List<Transform>();
	public Transform rewardItemNode = null;
	public List<ItemSlot> rewardItems = new List<ItemSlot>();
	public List<UILabel> itemInfoLabels = new List<UILabel>();
	public List<UILabel> itemNameLabels = new List<UILabel>();
	
	public UILabel mobTitleLabel = null;
	public int mobTitleStringID = -1;
	
	public List<Transform> mobInfos = new List<Transform>();
	public Transform mobFaceNode = null;
	public List<GameObject> mobFaces = new List<GameObject>();
	
	public UILabel reinforceTitleLabel = null;
	public int reinforceTitleStringID = -1;
	
	public string reinforceItemButtonPrefabPath = "UI/ReinforceItem";
	public Transform reinforceItemNode = null;
	public List<Transform> reinforceButtonPosInfos = new List<Transform>();
	public List<ReinforceItem> reinforceButtons = new List<ReinforceItem>();
	
	public UILabel stageStartStaminaLabel = null;
	public UIButton startButton = null;
	public UILabel startButtonLabel = null;
	public int startButtonStringID = -1;
	
	public UISlider resultStaminaValue = null;
	public UISlider currentStaminaValue = null;
	public UILabel staminaInfoLabel = null;
	public UILabel timerLabel = null;
	
	public System.TimeSpan addTime = new System.TimeSpan(0, 0, 25);
	public int addStaminaValue = 0;
	
	public Vector3 needGold = Vector3.zero;
	public float needStamina = 0.0f;
	
	public StageTable stageTable = null;
	public StageRewardTable stageRewardTable = null;
	public WaveStartInfo waveStartInfo = null;
	public StringTable stringTable = null;
	public CharExpTable expTable = null;
	
	public Vector3 staminaRecoveryValue = Vector3.zero;
	public static int staminaRate = 0;
	
	public int buyPotion1 = 0;
	public int buyPotion2 = 0;
	
	public int potion1Gold = 0;
	public int potion2Gold = 0;
	
	public int limitPotionCount = 0;
	public int stageCountPerTheme = 20;
	
	public UILabel properLevelInfoLabel = null;
	
	public override void Awake()
	{
		TableManager tableManager = TableManager.Instance;
		if (tableManager != null)
		{
			stageTable = tableManager.stageTable;
			stageRewardTable = tableManager.stageRewardTable;
			waveStartInfo = tableManager.waveStartInfo;
			stringTable = tableManager.stringTable;
			expTable = tableManager.charExpTable;
			
			staminaRecoveryValue = waveStartInfo.staminaRecoverPrice;
		}
		
		StringValueTable stringValueTabel = tableManager != null ? tableManager.stringValueTable : null;
		int addSec = 30;
		if (stringValueTabel != null)
		{
			addSec = stringValueTabel.GetData(StringValueKey.StaminaRefreshTimeSec);
			addStaminaValue = stringValueTabel.GetData(StringValueKey.StaminaAddValue);
			
			potion1Gold = stringValueTabel.GetData("Potion1Price");
			potion2Gold = stringValueTabel.GetData("Potion2Price");
			
			limitPotionCount = stringValueTabel.GetData("LimitPotionCount");
			
			stageCountPerTheme = stringValueTabel.GetData("StageCountPerTheme");
		}
		
		addTime = Game.ToTimeSpan(addSec);
		
		if (stringTable != null)
		{
			SetLabelString(titleLabel, titleStringID, stringTable);
			SetLabelString(stageRewardTitleLabel, stageRewardTitleStringID, stringTable);
			SetLabelString(mobTitleLabel, mobTitleStringID, stringTable);
			SetLabelString(reinforceTitleLabel, reinforceTitleStringID, stringTable);
			
			SetLabelString(stageTitleLabel, stageTitleStringID, stringTable);
			
			SetLabelString(startButtonLabel, startButtonStringID, stringTable);
		}
		
		MakeReinforceButton();
	}
	
	public virtual void Start()
	{
		GameUI.Instance.mapStartWindow = this;
		
		this.windowType = TownUI.eTOWN_UI_TYPE.MAPSELECT;
	}
	
	public virtual void OnDestroy()
	{
		GameUI.Instance.mapStartWindow = null;
	}
	
	public void InitSelectInfo()
	{
		foreach(ReinforceItem item in reinforceButtons)
		{
			item.checkBox.isChecked = false;
		}
	}
	
	
	public int limitLevel = 0;
	public int properLevel = 0;
	public virtual void InitWindow(int id, int stageType)
	{
		needGold = Vector3.zero;
		buyPotion1 = buyPotion2 = 0;
		
		Game.Instance.selectedReinforceInfos.Clear();
		InitSelectInfo();
		
		int themeID = ((id - 1) / stageCountPerTheme) + 1;
		string stagePostfixName = "";
		switch(stageType)
		{
		case 0:
			stagePostfixName = "Normal";
			break;
		case 1:
			stagePostfixName = "Hard";
			break;
		case 2:
			stagePostfixName = "Hell";
			break;
		}
		
		int idx = 0;
		string buffGoldName = "";
		
		TableManager tableManager = TableManager.Instance;
		StringValueTable stringValueTabel = tableManager != null ? tableManager.stringValueTable : null;
		foreach(ReinforceItem button in reinforceButtons)
		{
			ReinforceInfo oldInfo = waveStartInfo.GetReinforceInfo(idx);
			
			ReinforceInfo info = new ReinforceInfo(oldInfo);
			
			buffGoldName = string.Format("act{0}_{1}", themeID, stagePostfixName);
			if (stringValueTabel != null)
				info.needGold.x = (float)stringValueTabel.GetData(buffGoldName);
			
			button.SetInfo(info);
			
			idx++;
		}
		
		CharInfoData charData = Game.Instance.charInfoData;
		if (charData != null)
		{
			int charIndex = Game.Instance.connector.charIndex;
			CharPrivateData privateData = charData.GetPrivateData(charIndex);
			
			if (privateData != null)
				refreshExpireTime = privateData.refreshStaminaExpireTime;
		}
		
		if (stageTable != null)
		{
			StageTableInfo stageTableInfo = stageTable.GetData(id);
			
			List<string> mobFacesInfos = new List<string>();
			string kingdomName = "";
			string stageName = "";
			string stageModeName = "";
			
			int stageRewardTableID = -1;
			
			Color modeColor = Color.white;
			if (stageTableInfo != null)
			{
				BasicStageInfo basicStageInfo = stageTableInfo.GetBasicStageInfo(stageType);
				
				stageRewardTableID = id;
				needStamina = (float)basicStageInfo.needStamina;
				if (MapStartWindow.staminaRate != 0)
				{
					float rateValue = Mathf.Clamp((float)MapStartWindow.staminaRate * 0.01f, 0.0f, 1.0f);
					int minusValue = (int)(needStamina * rateValue);
					needStamina = needStamina - minusValue;
				}
				
				mobFacesInfos = basicStageInfo.mobFaceList;
				limitLevel = basicStageInfo.limitLevel;
				properLevel = basicStageInfo.properLevel;
				
				ModeTypeInfo modeTypeInfo = GetModeTypeInfo(stageType);
				if (modeTypeInfo != null)
				{
					stageModeName = stringTable.GetData(modeTypeInfo.stringID);
					modeColor = modeTypeInfo.modeColor;
				}
				
				kingdomName = stageTableInfo.kingdom;
				stageName = stageTableInfo.stageNumber;
				//actName = stageTableInfo.actName;
				//chapterName = stageTableInfo.chapterName;
			}
			
			if (this.modeTypeLabel != null)
			{
				this.modeTypeLabel.color = modeColor;
				this.modeTypeLabel.text = stageModeName;
			}
			
			if (this.kingdomNameLabel != null)
				this.kingdomNameLabel.text = kingdomName;
			
			if (this.stageNameLabel != null)
				this.stageNameLabel.text = stageName;
			
			if (this.stageStartStaminaLabel != null)
				this.stageStartStaminaLabel.text = string.Format("{0}", (int)needStamina);
			
			if (this.properLevelInfoLabel != null)
				this.properLevelInfoLabel.text = string.Format("Lv.{0}", properLevel);
			
			selectedStageIndex = id;
			selectedStageType = stageType;
			
			MakeStageRewardItems(stageRewardTableID);
			
			MakeMobInfo(mobFacesInfos);
			
			string[] stageArgs = stageName.Split('-');
			int kingdomID = int.Parse(stageArgs[0]);
			ActiveKingdom(kingdomID - 1);
		}
		
		UpdateCoinInfo();
		
		UpdateStamina();
		
		UpdateBuyPotion1();
		UpdateBuyPotion2();
	}
	
	public void Update()
	{
		RefreshStaminaValue();
	}
	
	public System.DateTime refreshExpireTime = System.DateTime.Now;
	public void RefreshStaminaValue()
	{
		System.TimeSpan restTime = new System.TimeSpan(0, 0, 0);
		UpdateRefreshTime(out restTime);
		
		string timeText = "";
		if (restTime.TotalSeconds < 0)
			timeText = "--:--";
		else
			timeText = string.Format("{0:D2}:{1:D2}", restTime.Minutes, restTime.Seconds);
		
		timerLabel.text = timeText;
		
		tempStaminaRate = Mathf.Lerp(tempStaminaRate, resultStaminaRate, 0.3f);
		if (resultStaminaValue != null)
			resultStaminaValue.sliderValue = tempStaminaRate;
	}
	
	public void UpdateRefreshTime(out System.TimeSpan restTime)
	{
		System.DateTime nowTime = System.DateTime.Now;
		restTime = refreshExpireTime - nowTime;
		
		double totalSeconds = restTime.TotalSeconds;
		if (totalSeconds <= 0)
		{
			double restTotalSecs = -restTime.TotalSeconds;
			double addTotalSecs = addTime.TotalSeconds;
			int nCount = (int)(restTotalSecs / addTotalSecs);
			
			if (((int)restTotalSecs % (int)addTotalSecs) != 0)
				nCount++;
			
			int addCount = Mathf.Max(1, nCount);
			int newStamina = 0;
			
			int charIndex = Game.Instance.connector.charIndex;
			CharPrivateData privateData = Game.Instance.charInfoData.GetPrivateData(charIndex);
			
			System.TimeSpan newAddTime = Game.ToTimeSpan(addTotalSecs * addCount);
			refreshExpireTime += newAddTime;
			
			UpdateStamina();
		}
	}
	
	
	public float currentStaminaRate = 0.0f;
	public float resultStaminaRate = 0.0f;
	public float tempStaminaRate = 0.0f;
	public void UpdateStamina()
	{
		int charIndex = Game.Instance.connector.charIndex;
		float curStamina = 0.0f;
		float maxStamina = 0.0f;
		
		CharPrivateData privateData = Game.Instance.charInfoData.GetPrivateData(charIndex);
		if (privateData != null)
		{
			curStamina = (float)(privateData.baseInfo.StaminaCur + privateData.baseInfo.StaminaPresent);
			maxStamina = (float)privateData.baseInfo.StaminaMax;
		}
		
		
		//float rate = 0.0f;
		if (maxStamina > 0.0f)
			tempStaminaRate = currentStaminaRate = curStamina / maxStamina;
		
		if (currentStaminaValue != null)
			currentStaminaValue.sliderValue = currentStaminaRate;
		
		bool isLowerStamina = false;
		curStamina -= needStamina;
		if (curStamina < 0)
		{
			isLowerStamina = true;
			curStamina = 0;
		}
		
		//rate = 0.0f;
		if (maxStamina > 0.0f)
			resultStaminaRate = curStamina / maxStamina;
		
		if (resultStaminaValue != null)
			resultStaminaValue.sliderValue = currentStaminaRate;
		
		Color stanimaColor = Color.white;
		if (isLowerStamina == true)
			stanimaColor = Color.red;
		
		if (staminaInfoLabel != null)
		{
			staminaInfoLabel.color = stanimaColor;
			staminaInfoLabel.text = string.Format("{0}/{1}", (int)curStamina, (int)maxStamina);
		}
	}
	
	protected void ActiveKingdom(int index)
	{
		GameObject obj = null;
		int nCount = kingdomObjs.Count;
		for (int idx = 0; idx < nCount; ++idx)
		{
			obj = kingdomObjs[idx];
			
			obj.SetActive(idx == index);
		}
	}
	
	protected void MakeMobInfo(List<string> mobFacesInfos)
	{
		foreach(GameObject temp in mobFaces)
		{
			DestroyObject(temp, 0.2f);
		}
		this.mobFaces.Clear();
		
		int nCount = mobFacesInfos.Count;
		string path = "UI/BossBattleFace/";
		for (int index = 0; index < nCount; ++index)
		{
			string fileName = mobFacesInfos[index];
			
			string prefabPath = path + fileName;
			
			Transform trans = this.mobInfos[index];
			GameObject faceObj = ResourceManager.CreatePrefab(prefabPath, mobFaceNode, trans.localPosition);
			this.mobFaces.Add(faceObj);
		}
	}
	
	public List<ItemSlot> stageRewardItemList = new List<ItemSlot>();
	protected void MakeStageRewardItems(int stageID)
	{
		if (stageRewardTable == null)
			return;
		
		foreach(UILabel label in itemInfoLabels)
			label.text = "";
		
		foreach(UILabel label in itemNameLabels)
			label.text = "";
		
		foreach(ItemSlot temp in stageRewardItemList)
		{
			DestroyObject(temp.gameObject, 0.2f);
		}
		stageRewardItemList.Clear();
		
		StageRewardInfo stageRewardInfo = stageRewardTable.GetData(stageID);
		StageReward rewardInfo = stageRewardInfo != null ? stageRewardInfo.GetStageReward(selectedStageType) : null;
		
		if (rewardInfo != null)
		{
			int itemCount = rewardInfo.rewardItemIDs.Count;
			
			for (int index = 0; index < itemCount; ++index)
			{
				int itemID = rewardInfo.rewardItemIDs[index];
				
				Transform slotPos = itemInfos[index];
				UILabel itemInfoLabel = itemInfoLabels[index];
				UILabel itemNameLabel = itemNameLabels[index];
				
				ItemSlot itemSlot = ResourceManager.CreatePrefab<ItemSlot>(itemPrefabPath, rewardItemNode, slotPos.localPosition);
				if (itemSlot != null)
				{
					stageRewardItemList.Add(itemSlot);
					
					itemSlot.slotWindowType = GameDef.eItemSlotWindow.StageReward_List;
					
					Item rewardItem = Item.CreateItem(itemID, "", 0, 0, 1);
					itemSlot.SetItem(rewardItem);
					
					itemInfoLabel.text = rewardItem.GetAttributeInfo();
					
					if (rewardItem != null && rewardItem.itemInfo != null)
						itemNameLabel.text = rewardItem.itemInfo.itemName;
				}
			}
		}
	}
	
	private void MakeReinforceButton()
	{
		reinforceButtons.Clear();
		
		foreach(Transform trans in reinforceButtonPosInfos)
		{
			ReinforceItem reinforceButton = ResourceManager.CreatePrefab<ReinforceItem>(reinforceItemButtonPrefabPath, reinforceItemNode, trans.localPosition);
			if (reinforceButton != null)
			{
				if (reinforceButton.checkBox != null)
				{
					//reinforceButton.checkBox.eventReceiver = this.gameObject;
					//reinforceButton.checkBox.functionName = "OnSelectReinforce";
					reinforceButton.checkBox.onStageChangeArg2 = new UICheckbox.OnStateChangeArg2(OnReinforceChange);
				}
				
				reinforceButtons.Add(reinforceButton);
			}
		}
	}
	
	public void OnReinforceChange(UICheckbox checkBox, bool bActive)
	{
		Debug.Log(checkBox);
		
		List<ReinforceInfo> reinforceList = Game.Instance.selectedReinforceInfos;
		
		ReinforceItem changedButton = checkBox.gameObject.GetComponent<ReinforceItem>();
		if (changedButton != null)
		{
			if (bActive == false)
			{
				foreach(ReinforceInfo info in reinforceList)
				{
					if (changedButton.info == info)
					{
						needGold -= changedButton.info.needGold;
						reinforceList.Remove(info);
						break;
					}
				}
			}
			else
			{
				Vector3 tempValue = needGold + changedButton.info.needGold;
				CashItemType check = CheckNeedGold(tempValue);
				if (check != CashItemType.None)
				{
					checkBox.isChecked = false;
					OnNeedMoneyPopup(check, this);
					return;
				}
				
				needGold += changedButton.info.needGold;
				reinforceList.Add(changedButton.info);
			}
		}
		
		UpdateCoinInfo();
		UpdateStartButton();
	}
	
	public override void UpdateCoinInfo()
	{
		//Vector3 ownGold = Game.Instance.charInfoData.goldInfo;
		int ownGold = 0;
		int ownJewel = 0;
		int ownMedal = 0;
		
		CharInfoData charData = Game.Instance.charInfoData;
		if (charData != null)
		{
			ownGold = charData.gold_Value;
			ownJewel = charData.jewel_Value;
			ownMedal = charData.medal_Value;
		}
		
		ownGold -= (int)needGold.x;
		ownJewel -= (int)needGold.y;
		ownMedal -= (int)needGold.z;
		
		ownGold -= (buyPotion1 * potion1Gold);
		ownGold -= (buyPotion2 * potion2Gold);
		
		SetCoinInfo(ownGold, ownJewel, ownMedal, false);
	}
	
	public void UpdateStartButton()
	{
		//PlayerController player = Game.Instance.player;
		//LifeManager lifeManager = player != null ? player.lifeManager : null;
		int ownGold = 0;
		int ownJewel = 0;
		int ownMedal = 0;
		
		CharInfoData charData = Game.Instance.charInfoData;
		int charIndex = -1;
		CharPrivateData privateData = null;
		if (charData != null)
		{
			ownGold = charData.gold_Value;
			ownJewel = charData.jewel_Value;
			ownMedal = charData.medal_Value;
		
			privateData = charData.GetPrivateData(charIndex);
		}
		
		
		Vector2 ownStamina = new Vector2(0.0f, 100.0f);
		if (privateData != null)
		{
			ownStamina.x = privateData.baseInfo.StaminaCur;
			ownStamina.y = privateData.baseInfo.StaminaMax;
		}
		
		int errorCount = 0;
		if ((needGold.x > 0.0f && (int)needGold.x > ownGold)  ||
			(needGold.y > 0.0f && (int)needGold.y > ownJewel) ||
			(needGold.z > 0.0f && (int)needGold.z > ownMedal))
			errorCount++;
		
		//if (needStamina > 0.0f && needStamina > ownStamina.x)
		//	errorCount++;
		
		bool isEnable = errorCount == 0;
		Color buttonColor = Color.white;
		
		if (startButton != null)
		{
			buttonColor = isEnable == true ? startButton.defaultColor : startButton.disabledColor;
			startButton.isEnabled = isEnable;
		}
		
		if (startButtonLabel != null)
			startButtonLabel.color = buttonColor;
	}
	
	public override void OnBack()
	{
		base.OnBack();
		
		this.gameObject.SetActive(false);
	}
	
	public void OnNeedStaminaOK(GameObject obj)
	{
		ClosePopup();
		
		CashItemType check = CheckNeedGold(staminaRecoveryValue);
		if (check != CashItemType.None)
		{
			OnNeedMoneyPopup(check, this);
			return;
		}
		
		//스테미나 회복 요청 패킷? 보내고 대기??...
		IPacketSender packetSender = Game.Instance.packetSender;
		if (packetSender != null)
		{
			List<int> reinforceIndexList = new List<int>();
			List<ReinforceInfo> reinforceInfos = Game.Instance.selectedReinforceInfos;
			foreach(ReinforceInfo info in reinforceInfos)
			{
				int selectedIndex = waveStartInfo.FindReinforceIndex(info);
				if (selectedIndex != -1)
					reinforceIndexList.Add(selectedIndex);
			}
			
			packetSender.SendRecoveryStaminaByStage(Game.Instance.connector.charIndex,  reinforceIndexList.ToArray(), selectedStageIndex, selectedStageType,
													buyPotion1, buyPotion2);
			
			requestCount++;
		}
	}
	
	public int requestCount = 0;
	public int selectedStageIndex = -1;
	public int selectedStageType = 0;
	public virtual void OnStart()
	{
		if (requestCount > 0)
			return;
		
		int charIndex = Game.Instance.connector.charIndex;
		float curStamina = 0.0f;
		//float maxStamina = 0.0f;
		
		CharInfoData charData = Game.Instance.charInfoData;
		int emptyInventoryCount = -1;
		if (charData != null)
			emptyInventoryCount = charData.CheckEmptyInventory();
		
		int charLevel = 0;
		CharPrivateData privateData = charData != null ? charData.GetPrivateData(charIndex) : null;
		if (privateData != null)
		{
			curStamina = (float)(privateData.baseInfo.StaminaCur + privateData.baseInfo.StaminaPresent);
			if (expTable != null)
				charLevel = expTable.GetLevel(privateData.baseInfo.ExpValue);
			//maxStamina = (float)privateData.baseInfo.StaminaMax;
		}
		
		if (emptyInventoryCount == 0)
		{
			OnNeedInvenSlotPopup();
			return;
		}
		
		if (curStamina < this.needStamina)
		{
			OnNeedStaminaPopup(this.staminaRecoveryValue);
			return;
		}
		
		if (charLevel < limitLevel)
		{
			PopupLimitLevel(limitLevel);
			return;
		}
		
		IPacketSender packetSender = Game.Instance.packetSender;
		if (packetSender != null)
		{
			List<int> reinforceIndexList = new List<int>();
			List<ReinforceInfo> reinforceInfos = Game.Instance.selectedReinforceInfos;
			foreach(ReinforceInfo info in reinforceInfos)
			{
				int selectedIndex = waveStartInfo.FindReinforceIndex(info);
				if (selectedIndex != -1)
					reinforceIndexList.Add(selectedIndex);
			}
			
			packetSender.SendStageStart(charIndex, selectedStageType, selectedStageIndex, 
										reinforceIndexList.ToArray(), (int)curStamina, buyPotion1, buyPotion2);
			
			requestCount++;
		}
	}
	
	public void OnConfirmPopupCancel(GameObject obj)
	{
		ClosePopup();
	}
	
	public override void OnErrorMessage(NetErrorCode errorCode, PopupBaseWindow popupBase)
	{
		requestCount--;
		base.OnErrorMessage(errorCode, popupBase);
	}
	
	public string loadingPanelPrefabPath = "";
	public LoadingPanel loadingPanel = null;
	public void CreateLoadingPanel(string stageName)
	{
		if (loadingPanel == null)
			loadingPanel = ResourceManager.CreatePrefab<LoadingPanel>(loadingPanelPrefabPath, popupNode, Vector3.zero);
		else
			loadingPanel.gameObject.SetActive(true);
		
		if (loadingPanel != null)
		{
			loadingPanel.ChangeLoadingBackgroundImage(stageName);
			loadingPanel.LoadScene(stageName, this);
		}
	}
	
	public virtual void OnStart(int stageIndex, int stageType, int[] buffs)
	{
		requestCount--;
		
		Debug.Log("MapStartWindow Active !!!!!!!");
		this.gameObject.SetActive(true);
		
		Game.Instance.selectedReinforceInfos.Clear();
		Game.Instance.selectedTowerInfo = null;
		
		TableManager tableManager = TableManager.Instance;
		WaveStartInfo waveStartInfo = null;
		StageTable stageTable = null;
		if (tableManager != null)
		{
			waveStartInfo = tableManager.waveStartInfo;
			stageTable = tableManager.stageTable;
		}
		
		foreach(int buffIndex in buffs)
		{
			ReinforceInfo info = waveStartInfo != null ? waveStartInfo.reinforceInfoList[buffIndex] : null;
			if (info != null)
				Game.Instance.selectedReinforceInfos.Add(info);
		}
		
		
		StageTableInfo stageInfo = null;
		string stageName = "";
		if (stageTable != null)
		{
			stageInfo = stageTable.GetData(stageIndex);
			BasicStageInfo info = stageInfo != null ? stageInfo.GetBasicStageInfo(stageType) : null;
			if (info != null)
				stageName = info.stageName;
		}
		
		Game.Instance.stageName = stageName;
		Game.Instance.stageIndex = stageIndex - 1;
		Game.Instance.lastSelectStageType = stageType;
		
		CreateLoadingPanel(stageName);
	}
	
	
	public string limitLevelPopupPrefabPath = "UI/MapSelect/LimitLevelPopup";
	public void PopupLimitLevel(int limiLevel)
	{
		BaseConfirmPopup popup = ResourceManager.CreatePrefab<BaseConfirmPopup>(limitLevelPopupPrefabPath, popupNode, Vector3.zero);
		
		if (popup != null)
		{
			popup.okButtonMessage.target = this.gameObject;
			popup.okButtonMessage.functionName = "OnClosePopup";
			
			if (popup.messageLabel != null)
			{
				string formatString = stringTable.GetData(popup.messageStringID);
				popup.messageLabel.text = string.Format(formatString, limiLevel);
			}
			
			popupList.Add(popup);
		}
	}
	
	public override void OnNeedInvenSlotPopupOK(GameObject obj)
	{
		TownUI townUI = GameUI.Instance.townUI;
		if (townUI != null)
		{
			if (townUI.mapSelect != null)
				townUI.mapSelect.OnBack();
		}
		
		base.OnNeedInvenSlotPopupOK(obj);
	}
	
	public ModeTypeInfo GetModeTypeInfo(int stageType)
	{
		ModeTypeInfo info = null;
		int nCount = modeTypeInfos.Count;
		if (stageType >= 0 && stageType < nCount)
			info = modeTypeInfos[stageType];
		
		return info;
	}
	
	public UIButton incPotion1Button = null;
	public UIButton decPotion1Button = null;
	public void IncPotion1BuyCount()
	{
		int curPotion = limitPotionCount;
		
		CharInfoData charData = Game.Instance.charInfoData;
		
		if (charData != null)
			curPotion = charData.potion1 + charData.potion1Present;
		
		int canBuyCount = Mathf.Max(0, limitPotionCount - curPotion);
		
		buyPotion1 = Mathf.Min(canBuyCount, buyPotion1 + 1);
		
		UpdateBuyPotion1();
		UpdateCoinInfo();
	}
	
	public void DecPotion1BuyCount()
	{
		buyPotion1 = Mathf.Max(0, buyPotion1 - 1);
		
		UpdateBuyPotion1();
		UpdateCoinInfo();
	}
	
	public UILabel buyPotion1Count = null;
	public void UpdateBuyPotion1()
	{
		int curPotion = 0;
		CharInfoData charData = Game.Instance.charInfoData;
		if (charData != null)
			curPotion = charData.potion1 + charData.potion1Present;
		
		if (buyPotion1Count != null)
			buyPotion1Count.text = string.Format("{0}", buyPotion1 + curPotion);
		
		if (decPotion1Button != null)
			decPotion1Button.isEnabled = (buyPotion1 > 0);
		if (incPotion1Button != null)
			incPotion1Button.isEnabled = !(buyPotion1 + curPotion >= limitPotionCount);
	}
	
	public UIButton incPotion2Button = null;
	public UIButton decPotion2Button = null;
	public void IncPotion2BuyCount()
	{
		int curPotion = limitPotionCount;
		
		CharInfoData charData = Game.Instance.charInfoData;
		if (charData != null)
			curPotion = charData.potion2 + charData.potion2Present;
		
		int canBuyCount = Mathf.Max(0, limitPotionCount - curPotion);
		
		buyPotion2 = Mathf.Min(canBuyCount, buyPotion2 + 1);
		
		UpdateBuyPotion2();
		UpdateCoinInfo();
	}
	
	public void DecPotion2BuyCount()
	{
		buyPotion2 = Mathf.Max(0, buyPotion2 - 1);
		
		UpdateBuyPotion2();
		UpdateCoinInfo();
	}
	
	public UILabel buyPotion2Count = null;
	public void UpdateBuyPotion2()
	{
		int curPotion = 0;
		CharInfoData charData = Game.Instance.charInfoData;
		if (charData != null)
			curPotion = charData.potion2 + charData.potion2Present;
		
		if (buyPotion2Count != null)
			buyPotion2Count.text = string.Format("{0}", buyPotion2 + curPotion);
		
		if (decPotion2Button != null)
			decPotion2Button.isEnabled = (buyPotion2 > 0);
		if (incPotion2Button != null)
			incPotion2Button.isEnabled = !(buyPotion2 + curPotion >= limitPotionCount);
	}
}
