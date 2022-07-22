using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SpecialStageInfo
{
	public int stageID = -1;
	public GameObject stageInfoObj = null;
	public int stageInfoStringID = -1;
}

public class SpecialMapStartWindow : MapStartWindow {
	
	public override void Start()
	{
		GameUI.Instance.specialMapStartWindow = this;
		
		this.windowType = TownUI.eTOWN_UI_TYPE.SPECIAL_MAP_START;
	}
	
	public override void OnDestroy()
	{
		GameUI.Instance.specialMapStartWindow = null;
	}
	
	public override void InitWindow(int id, int stageType)
	{
		needGold = Vector3.zero;
		buyPotion1 = buyPotion2 = 0;
		
		Game.Instance.selectedReinforceInfos.Clear();
		InitSelectInfo();
		
		int themeID = 1;
		
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
			
			/*
			string[] stageArgs = stageName.Split('-');
			int kingdomID = int.Parse(stageArgs[0]);
			ActiveKingdom(kingdomID - 1);
			*/
		}
		
		UpdateStageInfo(id);
		
		UpdateCoinInfo();
		
		UpdateStamina();
		
		UpdateBuyPotion1();
		UpdateBuyPotion2();
	}
	
	public List<SpecialStageInfo> stageInfos = new List<SpecialStageInfo>();
	public UILabel stageInfoLabel = null;
	private void UpdateStageInfo(int stageID)
	{
		foreach(SpecialStageInfo obj in stageInfos)
		{
			if (obj.stageInfoObj != null)
				obj.stageInfoObj.SetActive(false);
		}
		
		SpecialStageInfo stageObj = GetStageInfo(stageID);
		if (stageObj != null && stageObj.stageInfoObj != null)
			stageObj.stageInfoObj.SetActive(true);
		
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		string stageInfoStr = "";
		if (stringTable != null && stageObj != null)
			stageInfoStr = stringTable.GetData(stageObj.stageInfoStringID);
		
		if (stageInfoLabel != null)
			stageInfoLabel.text = stageInfoStr;
	}
	
	private SpecialStageInfo GetStageInfo(int stageID)
	{
		SpecialStageInfo info = null;
		foreach(SpecialStageInfo temp in stageInfos)
		{
			if (temp.stageID == stageID)
			{
				info = temp;
				break;
			}
		}
		
		return info;
	}
	
	public override void OnStart()
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
			
			//OnStart(selectedStageIndex, selectedStageType, reinforceIndexList.ToArray());
			
			requestCount++;
		}
	}
	
	public override void OnStart(int stageIndex, int stageType, int[] buffs)
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
}
