using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveStartWindow : PopupBaseWindow
{
	public TownUI townUI = null;
	
	public string towerButtonPrefabPath = "UI/TowerButton";
	public Transform towerButtonNode = null;
	public List<Transform> towerButtonPosInfos = new List<Transform>();
	public List<TowerButton> towerButtons = new List<TowerButton>();
	
	public string reinforceItemButtonPrefabPath = "UI/ReinforceItem";
	public Transform reinforceItemNode = null;
	public List<Transform> reinforceButtonPosInfos = new List<Transform>();
	public List<ReinforceItem> reinforceButtons = new List<ReinforceItem>();
	
	public Vector3 needGold = Vector3.zero;
	public float needStamina = 0.0f;
	
	public UILabel waveStartStaminaLabel = null;
	public UIButton startButton = null;
	public UILabel startButtonLabel = null;
	public int startButtonStringID = -1;
	
	public UILabel titleLabel = null;
	public int titleStringID = -1;
	public UILabel towerAreaLabel = null;
	public int towerAreaStringID = -1;
	public UILabel reinforceAreaLabel = null;
	public int reinforceAreaStringID = -1;
	
	public UISlider resultStaminaValue = null;
	public UISlider currentStaminaValue = null;
	public UILabel staminaInfoLabel = null;
	public UILabel timerLabel = null;
	
	public System.TimeSpan addTime = new System.TimeSpan(0, 0, 25);
	public int addStaminaValue = 0;
	
	public WaveWindow waveWindow = null;
	
	public int buyPotion1 = 0;
	public int buyPotion2 = 0;
	
	public int potion1Gold = 0;
	public int potion2Gold = 0;
	
	public int limitPotionCount = 0;
	
	public override void Awake()
	{
		this.windowType = TownUI.eTOWN_UI_TYPE.WAVE;
		
		base.Awake();
		
		if (startButton != null)
			startButton.defaultColor = Color.white;
		
		MakeTowerButton();
		MakeReinforceButton();
		
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		if (stringTable != null)
		{
			if (titleLabel != null)
				titleLabel.text = stringTable.GetData(titleStringID);
		
			if (towerAreaLabel != null)
				towerAreaLabel.text = stringTable.GetData(towerAreaStringID);
			
			if (reinforceAreaLabel != null)
				reinforceAreaLabel.text = stringTable.GetData(reinforceAreaStringID);
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
		}
		
		addTime = Game.ToTimeSpan(addSec);
		
		//InitWindow();
		
		GameUI.Instance.waveStartWindow = this;
	}
	
	private void MakeTowerButton()
	{
		towerButtons.Clear();
		
		GameObject prefab = ResourceManager.LoadPrefab(towerButtonPrefabPath);
		
		int index = 0;
		
		foreach(Transform trans in towerButtonPosInfos)
		{
			TowerButton towerButton = ResourceManager.CreatePrefab<TowerButton>(towerButtonPrefabPath, towerButtonNode, trans.localPosition);
			if (towerButton != null)
			{
				switch(index)
				{
				case 0:
					towerButton.SetActiveLimitInfo(0, false);
					towerButton.SetActiveLimitInfo(1, false);
					break;
				case 1:
					towerButton.SetActiveLimitInfo(1, false);
					break;
				case 2:
					towerButton.SetActiveLimitInfo(0, false);
					break;
				}
				
				if (towerButton.checkBox != null)
				{
					towerButton.checkBox.startsChecked = index == 0;
					
					towerButton.checkBox.eventReceiver = this.gameObject;
					towerButton.checkBox.functionName = "OnActivateTowerButton";
					
					towerButton.checkBox.radioButtonRoot = towerButtonNode;
				}
				
				towerButtons.Add(towerButton);
			}
			
			++index;
		}
	}
	
	private void MakeReinforceButton()
	{
		reinforceButtons.Clear();
		
		GameObject prefab = ResourceManager.LoadPrefab(reinforceItemButtonPrefabPath);
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
	
	public WaveStartInfo waveStartInfo = null;
	public void InitWindow()
	{
		InitSelectInfo();
		
		Game.Instance.selectedTowerInfo = null;
		Game.Instance.selectedReinforceInfos.Clear();
		
		TableManager tableManager = TableManager.Instance;
		waveStartInfo = tableManager != null ? tableManager.waveStartInfo : null;
		
		if (waveStartInfo == null)
			return;
		
		PlayerController player = Game.Instance.player;
		int charLevel = 0;
		if (player != null)
			charLevel = player.lifeManager.charLevel;
		
		//int nCount = waveStartInfo.towerInfoList.Count;
		int index = 0;
		foreach(TowerButton button in towerButtons)
		{
			TowerInfo towerInfo = waveStartInfo.GetTowerInfo(index);
			
			button.SetInfo(towerInfo);
			
			button.SetLimitLevel(charLevel);
			
			index++;
		}
		
		//nCount = waveStartInfo.reinforceInfoList.Count;
		index = 0;
		foreach(ReinforceItem button in reinforceButtons)
		{
			ReinforceInfo info = waveStartInfo.GetReinforceInfo(index);
			
			button.SetInfo(info);
			
			index++;
		}
		
		CharInfoData charData = Game.Instance.charInfoData;
		if (charData != null)
		{
			int charIndex = Game.Instance.connector.charIndex;
			CharPrivateData privateData = charData.GetPrivateData(charIndex);
			
			if (privateData != null)
				refreshExpireTime = privateData.refreshStaminaExpireTime;
		}
		
		UpdateCoinInfo();
		
		System.TimeSpan restTime = new System.TimeSpan(0, 0, 0);
		UpdateRefreshTime(out restTime);
		
		UpdateStamina();
		
		UpdateStartButton();
		
		UpdateBuyPotion1();
		UpdateBuyPotion2();
	}
	
	public void UpdateStartButton()
	{
		//PlayerController player = Game.Instance.player;
		//LifeManager lifeManager = player != null ? player.lifeManager : null;
		
		CharInfoData charData = Game.Instance.charInfoData;
		int charIndex = -1;
		CharPrivateData privateData = null;
		if (charData != null)
			privateData = charData.GetPrivateData(charIndex);
		
		int ownGold = 0;
		int ownJewel = 0;
		int ownMedal = 0;
		if (charData != null)
		{
			ownGold = charData.gold_Value;
			ownJewel = charData.jewel_Value;
			ownMedal = charData.medal_Value;
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
	
	public override void UpdateCoinInfo()
	{
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
		
		if (waveStartStaminaLabel != null)
			waveStartStaminaLabel.text = string.Format("{0}", (int)needStamina);
	}
	
	public void OnActivateTowerButton(bool bActive)
	{
		if (bActive == false)
			return;
		
		needStamina = 0.0f;
		
		TowerInfo selectedInfo = null;
		foreach(TowerButton button in towerButtons)
		{
			if (button.checkBox != null && button.checkBox.isChecked)
			{
				selectedInfo = button.GetInfo();
				break;
			}
		}
		
		if (selectedInfo != null)
			needStamina = selectedInfo.needStamina;
		
		Game.Instance.selectedTowerInfo = selectedInfo;
		UpdateStamina();
		
		UpdateStartButton();
		Debug.Log(selectedInfo != null ? selectedInfo.ToString() : "Not Selected!!");
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
	
	public void OnSelectReinforce(bool bActive)
	{
		Game.Instance.selectedReinforceInfos.Clear();
		
		needGold = Vector3.zero;
		
		foreach(ReinforceItem button in reinforceButtons)
		{
			if (button.checkBox != null && button.checkBox.isChecked)
			{
				if (button.info != null)
				{
					needGold += button.info.needGold;
					
					Game.Instance.selectedReinforceInfos.Add(button.info);
				}
			}
		}
		
		UpdateCoinInfo(true);
		UpdateStartButton();
		
		Debug.Log("Need Gold : " + needGold);
	}
	
	public string waveStageName = "";
	public int requestCount = 0;
	public void OnStart()
	{
		if (requestCount > 0)
			return;
		
		int charIndex = Game.Instance.connector.charIndex;
		float curStamina = 0.0f;
		//float maxStamina = 0.0f;
		
		CharPrivateData privateData = Game.Instance.charInfoData.GetPrivateData(charIndex);
		if (privateData != null)
		{
			curStamina = (float)(privateData.baseInfo.StaminaCur + privateData.baseInfo.StaminaPresent);
			//maxStamina = (float)privateData.baseInfo.StaminaMax;
		}
		
		float needStaminaValue = needStamina;
		
		if (curStamina < needStaminaValue)
		{
			OnNeedStaminaPopup(waveStartInfo.staminaRecoverPrice);
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
			
			int towerIndex = waveStartInfo.FindTowerIndex(Game.Instance.selectedTowerInfo);
			if (towerIndex < 0)
			{
				Debug.LogError("Tower Index error......");
				towerIndex = 0;
			}
			
			packetSender.SendWaveStartOrContinue(charIndex,  reinforceIndexList.ToArray(), 
													towerIndex,  (int)curStamina, false,
													buyPotion1, buyPotion2);
			requestCount++;
		}
	}
	
	public void OnStart(int[] buffs, int towerIndex, bool bStart)
	{
		requestCount--;
		
		Debug.Log("WaveStartWindow Active !!!!!!!");
		this.gameObject.SetActive(true);
		
		Game.Instance.selectedReinforceInfos.Clear();
		Game.Instance.selectedTowerInfo = null;
		
		WaveManager.continueWaveStep = -1;
		WaveManager.continueWaveTime = 0;
		
		foreach(int buffIndex in buffs)
		{
			ReinforceInfo info = waveStartInfo.reinforceInfoList[buffIndex];
			Game.Instance.selectedReinforceInfos.Add(info);
		}
		
		Game.Instance.selectedTowerInfo = waveStartInfo.towerInfoList[towerIndex];
		
		CreateLoadingPanel(waveStageName);
	}
	
	public string loadingPanelPrefabPath = "";
	public LoadingPanel loadingPanel = null;
	public void CreateLoadingPanel(string stageName)
	{
		if (loadingPanel == null)
		{
			loadingPanel = ResourceManager.CreatePrefab<LoadingPanel>(loadingPanelPrefabPath, popupNode, Vector3.zero);
		}
		else
		{
			loadingPanel.gameObject.SetActive(true);
		}
		
		if (loadingPanel != null)
		{
			loadingPanel.ChangeLoadingBackgroundImage(stageName);
			loadingPanel.LoadScene(stageName, this);
		}
	}
	
	public void InitSelectInfo()
	{
		foreach(TowerButton tower in towerButtons)
		{
			tower.checkBox.isChecked = tower.checkBox.startsChecked;
		}
		
		foreach(ReinforceItem item in reinforceButtons)
		{
			item.checkBox.isChecked = false;
		}
	}
	
	public override void OnBack()
	{
		ClosePopup();
		this.gameObject.SetActive(false);
	}
	
	public void OnConfirmPopupCancel(GameObject obj)
	{
		ClosePopup();
	}
	
	public void OnNeedStaminaOK(GameObject obj)
	{
		ClosePopup();
		
		
		CashItemType check = CheckNeedGold(waveStartInfo.staminaRecoverPrice);
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
			
			int towerIndex = waveStartInfo.FindTowerIndex(Game.Instance.selectedTowerInfo);
			if (towerIndex < 0)
			{
				Debug.LogError("Tower Index error......");
				towerIndex = 0;
			}
			
			packetSender.SendRecoveryStamina(Game.Instance.connector.charIndex,  reinforceIndexList.ToArray(), 
											towerIndex, true, true,
											buyPotion1, buyPotion2);
			
			//OnStart(reinforceIndexList.ToArray(), towerIndex, true);
			
			requestCount++;
		}
	}
	
	public override void OnErrorMessage(NetErrorCode errorCode, PopupBaseWindow popupBase)
	{
		requestCount--;
		base.OnErrorMessage(errorCode, popupBase);
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
	
	public override void OnNeedInvenSlotPopupOK(GameObject obj)
	{
		TownUI townUI = GameUI.Instance.townUI;
		if (townUI != null)
		{
			if (townUI.waveWindow != null)
				townUI.waveWindow.OnBack();
		}
		
		base.OnNeedInvenSlotPopupOK(obj);
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
