using UnityEngine;
using System.Collections;

public class MyCharInfos : MonoBehaviour {
	public UILabelEx gold = null;
	public UILabelEx jewel = null;
	public UILabelEx medal = null;
	
	public UISlider currentStaminaValue = null;
	public UILabel staminaInfoLabel = null;
	public UILabel timerLabel = null;
	
	public UILabel ticketInfo = null;
	
	public UILabel levelInfo = null;
	public UILabel expInfo = null;
	public UISlider expRateSlider = null;
	
	public GameObject awakeningLevelRoot = null;
	public UILabel awakeningLevelLabel = null;
	public UISlider awakeningExpSlider = null;
	
	public StageTable stageTable = null;
	public StageRewardTable stageRewardTable = null;
	public WaveStartInfo waveStartInfo = null;
	public StringTable stringTable = null;
	public CharExpTable expTable = null;
	public CharExpTable awakeningExpTable = null;
	public StringValueTable stringValueTable = null;
	void Awake()
	{
		TableManager tableManager = TableManager.Instance;
		if (tableManager != null)
		{
			stageTable = tableManager.stageTable;
			stageRewardTable = tableManager.stageRewardTable;
			waveStartInfo = tableManager.waveStartInfo;
			stringTable = tableManager.stringTable;
			expTable = tableManager.charExpTable;
			awakeningExpTable = tableManager.awakenExpTable;
			stringValueTable = tableManager.stringValueTable;
			
			int addSec = 30;
			if (stringValueTable != null)
			{
				addSec = stringValueTable.GetData(StringValueKey.StaminaRefreshTimeSec);
				addStaminaValue = stringValueTable.GetData(StringValueKey.StaminaAddValue);
			}
			
			addTime = Game.ToTimeSpan(addSec);
		}
	}
	
	CharPrivateData privateData = null;
	void Start()
	{
		GameUI.Instance.myCharInfos = this;
		
		int charIndex = 0;
		CharInfoData charData = Game.Instance.charInfoData;
		if (Game.Instance.connector != null)
			charIndex = Game.Instance.connector.charIndex;
		if (charData != null)
			privateData = charData.GetPrivateData(charIndex);
		
		if (privateData != null)
			refreshExpireTime = privateData.refreshStaminaExpireTime;
		
		UpdateValue();
	}
	
	void OnDestroy()
	{
		GameUI.Instance.myCharInfos = null;
	}
	
	public void UpdateValue()
	{
		UpdateCoinInfo();
		UpdateExpInfo();
		UpdateTicketInfo();
		
		UpdateStamina();
	}
	
	void Update()
	{
		RefreshStaminaValue();
	}
	
	public virtual void UpdateCoinInfo(bool increaseEffect)
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
		
		SetCoinInfo(ownGold, ownJewel, ownMedal, increaseEffect);
	}
	
	public virtual void UpdateCoinInfo()
	{
		// default
		UpdateCoinInfo(false);
	}
	
	public void SetCoinInfo(int fGold, int fJewel, int fMedal, bool increaseEffect)
	{
		if (gold != null)
			gold.SetValue(fGold, increaseEffect);
		if (jewel != null)
			jewel.SetValue(fJewel, increaseEffect);
		if (medal != null)
			medal.SetValue(fMedal, increaseEffect);
	}
	
	public System.DateTime refreshExpireTime = System.DateTime.Now;
	public System.TimeSpan addTime = new System.TimeSpan(0, 0, 25);
	public int addStaminaValue = 0;
	public void RefreshStaminaValue()
	{
		System.TimeSpan restTime = new System.TimeSpan(0, 0, 0);
		UpdateRefreshTime(out restTime);
		
		string timeText = "";
		if (restTime.TotalSeconds < 0 || 
			(privateData != null && privateData.baseInfo.StaminaCur >= privateData.baseInfo.StaminaMax))
			timeText = "--:--";
		else
			timeText = string.Format("{0:D2}:{1:D2}", restTime.Minutes, restTime.Seconds);
		
		timerLabel.text = timeText;
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
			
			refreshExpireTime = System.DateTime.Now + addTime;
			
			if (privateData != null)
			{
				if (privateData.baseInfo.StaminaCur < privateData.baseInfo.StaminaMax)
				{
					newStamina = privateData.baseInfo.StaminaCur + (addCount * addStaminaValue);
					newStamina = Mathf.Min(privateData.baseInfo.StaminaMax, newStamina);
					
					privateData.baseInfo.StaminaCur = newStamina;
					
					IPacketSender packetSender = Game.Instance.packetSender;
					if (packetSender != null)
					{
						packetSender.SendUpdateStamina(newStamina);
					}
					
					restTime = refreshExpireTime - nowTime;
				}
				else
				{
					restTime = new System.TimeSpan(0, 0, -1);
				}
				
				privateData.SetStaminaRefreshTime((int)addTotalSecs);
			}
			
			UpdateStamina();
		}
	}
	
	public void UpdateStamina()
	{
		float curStamina = 0.0f;
		float maxStamina = 0.0f;
		
		if (privateData != null)
		{
			curStamina = (float)(privateData.baseInfo.StaminaCur + privateData.baseInfo.StaminaPresent);
			maxStamina = (float)privateData.baseInfo.StaminaMax;
		}
		
		float rate = 0.0f;
		if (maxStamina > 0.0f)
			rate = curStamina / maxStamina;
		
		if (currentStaminaValue != null)
			currentStaminaValue.sliderValue = rate;
		
		if (staminaInfoLabel != null)
			staminaInfoLabel.text = string.Format("{0}/{1}", (int)curStamina, (int)maxStamina);
	}
	
	public void UpdateTicketInfo()
	{
        int curTicket = Game.Instance.charInfoData.ticket;
		int maxTicket = 10;

		if (stringValueTable != null)
			maxTicket = stringValueTable.GetData("MaxTicketCount");
		
		if (ticketInfo != null)
			ticketInfo.text = string.Format("{0}/{1}", curTicket, maxTicket);
	}
	
	public void UpdateExpInfo()
	{
		long curExp = 0L;
		long maxExp = 0L;
		int curLevel = 0;
		int awakeningLevel = 0;
		
		float expRate = 0.0f;
		UISlider expSlider = null;
		UILabel lvLabel = null;
		if (privateData != null)
		{
			ExpInfo expTableInfo = null;
			
			if (expTable != null)
				curLevel = expTable.GetLevel(privateData.baseInfo.ExpValue);
			
			if (awakeningExpTable != null)
				awakeningLevel = awakeningExpTable.GetLevel(privateData.baseInfo.AExp);
			
			if (privateData.baseInfo.AExp > 0)
			{
				curExp = privateData.baseInfo.AExp;
				expRate = awakeningExpTable.GetProgressRate(privateData.baseInfo.AExp);
				expTableInfo = awakeningExpTable.GetExpInfo(awakeningLevel);
				
				expSlider = awakeningExpSlider;
				
				
				if (awakeningLevelRoot != null)
					awakeningLevelRoot.SetActive(true);
				
				if (expRateSlider != null)
					expRateSlider.gameObject.SetActive(false);
				if (awakeningExpSlider != null)
					awakeningExpSlider.gameObject.SetActive(true);
			}
			else
			{
				curExp = privateData.baseInfo.ExpValue;
				expRate = expTable.GetProgressRate(privateData.baseInfo.ExpValue);
				expTableInfo = expTable.GetExpInfo(curLevel);
				
				expSlider = expRateSlider;
				
				if (awakeningLevelRoot != null)
					awakeningLevelRoot.SetActive(false);
				
				if (expRateSlider != null)
					expRateSlider.gameObject.SetActive(true);
				if (awakeningExpSlider != null)
					awakeningExpSlider.gameObject.SetActive(false);
			}
			
			if (expTableInfo != null)
			{
				if (expTableInfo.needExp == 0L)
				{
					curExp = expTableInfo.baseExp;
					maxExp = curExp;
				}
				else
				{
					curExp -= expTableInfo.baseExp;
					maxExp = expTableInfo.needExp;
				}
			}
		}
		
		if (levelInfo != null)
			levelInfo.text = string.Format("{0}", curLevel);
		
		if (awakeningLevelLabel != null)
			awakeningLevelLabel.text = string.Format("{0}", awakeningLevel);
		
		if (expInfo != null)
			expInfo.text = string.Format("{0}/{1}", curExp, maxExp);
		
		if (expSlider != null)
			expSlider.sliderValue = expRate;
	}
}
