using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossRaidWindow : PopupBaseWindow {
	public TownUI townUI = null;
	
	public BossRaidListWindow bossRaidListWindow = null;
	
	public System.TimeSpan addTime = new System.TimeSpan(0, 0, 25);
	public int addStaminaValue = 0;
	
	public Vector3 staminaRecoveryValue = Vector3.zero;
	
	void Start()
	{
		TableManager tableManager = TableManager.Instance;
		if (tableManager != null)
		{
			WaveStartInfo waveStartInfo = tableManager.waveStartInfo;
			staminaRecoveryValue = waveStartInfo.staminaRecoverPrice;
			
			StringValueTable stringValueTabel = tableManager.stringValueTable;
			int addSec = 30;
			if (stringValueTabel != null)
			{
				addSec = stringValueTabel.GetData(StringValueKey.StaminaRefreshTimeSec);
				addStaminaValue = stringValueTabel.GetData(StringValueKey.StaminaAddValue);
			}
			
			addTime = Game.ToTimeSpan(addSec);
		}
		
		GameUI.Instance.bossRaidWindow = this;
	}
	
	void OnDestroy()
	{
		GameUI.Instance.bossRaidWindow = null;
	}
	
	public void InitWindow(List<BossRaidInfo> bossRaidInfos)
	{
		CharInfoData charData = Game.Instance.charInfoData;
		if (charData != null)
		{
			int charIndex = Game.Instance.connector.charIndex;
			CharPrivateData privateData = charData.GetPrivateData(charIndex);
			
			if (privateData != null)
				refreshExpireTime = privateData.refreshStaminaExpireTime;
		}
		
		if (bossRaidListWindow != null)
			bossRaidListWindow.SetInfos(bossRaidInfos);
	}
	
	public int requestCount = 0;
	public override void OnBack()
	{
		requestCount = 0;
		base.OnBack();
		
		if (bossRaidListWindow != null)
			bossRaidListWindow.InitList();

	}
	
	public long selectedBossIndex = -1;
	public void OnRequestBossRaidStart(GameObject obj)
	{
		if (requestCount > 0)
			return;
		
		if (obj == null)
			return;
		
		int curStamina = 0;
		int charIndex = Game.Instance.connector.charIndex;
		CharPrivateData privateData = Game.Instance.charInfoData.GetPrivateData(charIndex);
		if (privateData != null)
			curStamina = (privateData.baseInfo.StaminaCur + privateData.baseInfo.StaminaPresent);
			
		
		Transform parentTrans = obj.transform.parent;
		BossRaidBasicInfoPanel bossRaidInfoPanel = parentTrans != null ? parentTrans.gameObject.GetComponent<BossRaidBasicInfoPanel>() : null;
		
		int needStamina = 0;
		if (bossRaidInfoPanel != null)
		{
			needStamina = bossRaidInfoPanel.needStamina;
			
			selectedBossIndex = bossRaidInfoPanel.bossRaidInfo.index;
			selectBossRaidInfo = bossRaidInfoPanel.bossRaidInfo;
			
			if (curStamina < needStamina)
			{
				OnNeedStaminaPopup(this.staminaRecoveryValue);
				return;
			}
				
			
			IPacketSender sender = Game.Instance.packetSender;
			if (sender != null)
			{
				sender.SendRequestBossRaidStart(selectedBossIndex, false, selectBossRaidInfo.ownerPlatform, selectBossRaidInfo.ownerPlatformUserID);
				
				requestCount++;
			}
		}
		else
			selectBossRaidInfo = null;
	}
	
	public string failPopupPrefabPath = "UI/Boss/BossRaidFailPopup";
	public int bossAlreadyDeathMessageStringID = -1;
	public int bossRunAwayMessageStringID = -1;
	public override void OnErrorMessage(NetErrorCode errorCode, PopupBaseWindow popupBase)
	{
		requestCount = 0;
		
		switch(errorCode)
		{
		default:
			base.OnErrorMessage(errorCode, popupBase);
			break;
		case NetErrorCode.BossAlreadyDeath:
		case NetErrorCode.BossRunAway:
			BaseConfirmPopup basePopup = ResourceManager.CreatePrefab<BaseConfirmPopup>(failPopupPrefabPath, popupNode, Vector3.zero);
			if (basePopup != null)
			{
				int messageID = -1;
				if (errorCode == NetErrorCode.BossAlreadyDeath)
					messageID = bossAlreadyDeathMessageStringID;
				else
					messageID = bossRunAwayMessageStringID;
				
				basePopup.okButtonMessage.target = this.gameObject;
				basePopup.okButtonMessage.functionName = "OnClosePopup";
				
				basePopup.SetMessage(messageID);
				
				this.popupList.Add(basePopup);
			}
			break;
		}
	}
	
	public string bossRaidScene = "BossRaid";
	private BossRaidInfo selectBossRaidInfo = null;
	public void OnBossRaidStart(long index, bool isPhase2)
	{
		Debug.Log("BossRaidWindow Active !!!!!!!");
		this.gameObject.SetActive(true);
		
		if (selectBossRaidInfo != null)
		{
			Game.Instance.bossIndex = index;
			Game.Instance.bossID = selectBossRaidInfo.bossID;
			Game.Instance.isPhase2 = isPhase2;
			Game.Instance.bossHP = selectBossRaidInfo.curHP;
            Game.Instance.ownerPlatform = selectBossRaidInfo.ownerPlatform;
            Game.Instance.ownerPlatformUserID = selectBossRaidInfo.ownerPlatformUserID;
            
			CreateLoadingPanel(bossRaidScene);
		}
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
	
	public void Update()
	{
		RefreshStaminaValue();
	}
	
	public System.DateTime refreshExpireTime = System.DateTime.Now;
	public void RefreshStaminaValue()
	{
		System.TimeSpan restTime = new System.TimeSpan(0, 0, 0);
		UpdateRefreshTime(out restTime);
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
		}
	}
	
	public void OnConfirmPopupCancel(GameObject obj)
	{
		ClosePopup();
		
		rewardInfoPopup = null;
	}
	
	public void OnNeedStaminaOK(GameObject obj)
	{
		ClosePopup();
		
		CashItemType check = CheckNeedGold((int)staminaRecoveryValue.x, (int)staminaRecoveryValue.y, (int)staminaRecoveryValue.z);
		if (check != CashItemType.None)
		{
			OnNeedMoneyPopup(check, this);
			return;
		}
		
		//스테미나 회복 요청 패킷? 보내고 대기??...
		IPacketSender packetSender = Game.Instance.packetSender;
		if (packetSender != null)
		{
			packetSender.SendRecoveryStaminaByBossRaidStart(selectedBossIndex, selectBossRaidInfo.ownerPlatform, selectBossRaidInfo.ownerPlatformUserID);
			
			requestCount++;
		}
	}
	
	public string rewardInfoPopupPrefab = "UI/Boss/BossRaidRewardInfo";
	public BasePopup rewardInfoPopup = null;
	public void OnRewardInfoOpen(GameObject obj)
	{
		if (rewardInfoPopup == null)
		{
			rewardInfoPopup = ResourceManager.CreatePrefab<BasePopup>(rewardInfoPopupPrefab, popupNode, Vector3.zero);
		
			if (rewardInfoPopup != null)
			{
				if (rewardInfoPopup.cancelButtonMessage != null)
				{
					rewardInfoPopup.cancelButtonMessage.target = this.gameObject;
					rewardInfoPopup.cancelButtonMessage.functionName = "OnConfirmPopupCancel";
				}
			}
			
			this.popupList.Add(rewardInfoPopup);
		}
			
	}
}
