using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoToMapButton : MonoBehaviour {
	public GameObject parentWindow = null;
	
	public GotoMapInfo.eGotoType gotoType;
	public int stageType;		//Shop일 경우 tabID로 사용.
	public int stageID;
	
	public List<ModeTypeInfo> modeTypeInfos = new List<ModeTypeInfo>();
	public List<KingdomInfo> kingdomInfos = new List<KingdomInfo>();
	
	public UIButtonMessage buttonMessage = null;
	public UIButton gotoButton = null;
	public UILabel gotoEnableLabel = null;
	public UILabel gotoDisableLabel = null;
	
	public int gotoEnableStringID = 257;
	public int gotoDisablestringID = 258;
	
	public GameObject gotoStageNode = null;
		public UISprite kingdomSprite = null;
		public UILabel kingdomNameLabel = null;
		public UISprite modeSprite = null;
		public UILabel modeNameLabel = null;
		public UILabel stageNameLabel = null;
		public int stageNameTitleID = 92;
		private string stageNameTitleStr = "";
	
	public GameObject gotoOtherNode = null;
		public UISprite gotoSprite = null;
		public UILabel gotoInfoLabel = null;
	
	public string gotoShopSprite = "";
	public string gotoGambleSprite = "";
	public string gotoDefenceSprite = "";
	public int gotoShopInfoStringID = 0;
	public int gotoGambleInfoStringID = 0;
	public int gotoDefenceInfoStringID = 0;
	
	public ModeTypeInfo GetModeTypeInfo(int stageType)
	{
		ModeTypeInfo info = null;
		int nCount = modeTypeInfos.Count;
		if (stageType >= 0 && stageType < nCount)
			info = modeTypeInfos[stageType];
		
		return info;
	}
	
	public KingdomInfo GetKingdomInfo(int kingdomIndex)
	{
		KingdomInfo info = null;
		int nCount = kingdomInfos.Count;
		if (kingdomIndex >= 0 && kingdomIndex < nCount)
			info = kingdomInfos[kingdomIndex];
		
		return info;
	}
	
	public void SetGotoButtonInfo(GotoMapInfo.eGotoType gotoType, int stageType, int stageID)
	{
		this.gotoType = gotoType;
		this.stageType = stageType;
		this.stageID = stageID;
		
		switch(gotoType)
		{
		case GotoMapInfo.eGotoType.eGoto_Stage:
			if (gotoStageNode != null)
				gotoStageNode.SetActive(true);
			if (gotoOtherNode != null)
				gotoOtherNode.SetActive(false);
			SetKingdomInfo(stageID, stageType);
			break;
		case GotoMapInfo.eGotoType.eGoto_Gamble:
		case GotoMapInfo.eGotoType.eGoto_Shop:
		case GotoMapInfo.eGotoType.eGoto_Defence:
			if (gotoStageNode != null)
				gotoStageNode.SetActive(false);
			if (gotoOtherNode != null)
				gotoOtherNode.SetActive(true);
			
			SetGotoOtherInfo(gotoType, stageType, stageID);
			break;
		}
	}
	
	public void SetGotoOtherInfo(GotoMapInfo.eGotoType gotoType, int stageType, int stageID)
	{
		string spriteName = "";
		string infoStr = "";
		
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		switch(gotoType)
		{
		case GotoMapInfo.eGotoType.eGoto_Gamble:
			spriteName = gotoGambleSprite;
			if (stringTable != null)
				infoStr = stringTable.GetData(gotoGambleInfoStringID);
			break;
		case GotoMapInfo.eGotoType.eGoto_Shop:
			spriteName = gotoShopSprite;
			if (stringTable != null)
				infoStr = stringTable.GetData(gotoShopInfoStringID);
			break;
		case GotoMapInfo.eGotoType.eGoto_Defence:
			spriteName = gotoDefenceSprite;
			if (stringTable != null)
				infoStr = stringTable.GetData(gotoDefenceInfoStringID);
			CharInfoData charInfo = Game.Instance.charInfoData;
			int charIndex = Game.Instance.connector.charIndex;
			if (charInfo != null)
			{
				CharPrivateData privateData = charInfo.GetPrivateData(charIndex);
				this.gotoButton.isEnabled = privateData.baseInfo.Level >= 10;
			}
			break;
		}
		
		if (string.IsNullOrEmpty(spriteName) == true)
		{
			if (this.gotoSprite != null)
				this.gotoSprite.gameObject.SetActive(false);
		}
		else
		{
			if (gotoSprite != null)
			{
				this.gotoSprite.gameObject.SetActive(true);
				this.gotoSprite.spriteName = spriteName;
			}
		}
		
		if (gotoInfoLabel != null)
			gotoInfoLabel.text = infoStr;
		
		if (this.gotoEnableLabel != null)
			this.gotoEnableLabel.gameObject.SetActive(true);
		if (this.gotoDisableLabel != null)
			this.gotoDisableLabel.gameObject.SetActive(false);
	}
	
	public void SetKingdomInfo(int stageID, int stageType)
	{
		TableManager tableManager = TableManager.Instance;
		StageTable stageTable = null;
		StringValueTable stringValueTable = null;
		StringTable stringTable = null;
		
		if (tableManager != null)
		{
			stageTable = tableManager.stageTable;
			stringValueTable = tableManager.stringValueTable;
			stringTable = tableManager.stringTable;
		}
		
		if (stringTable != null && stageNameTitleStr == "" && stageNameTitleID != -1)
			stageNameTitleStr = stringTable.GetData(stageNameTitleID);
		
		int stageCountPerTheme = 20;
		if (stringValueTable != null)
			stageCountPerTheme = stringValueTable.GetData("StageCountPerTheme");
		
		int kingdomID = ((stageID - 1) / stageCountPerTheme);
		
		StageTableInfo stageTableInfo = null;
		if (stageTable != null)
			stageTableInfo = stageTable.GetData(stageID);
		
		CharInfoData charData = Game.Instance.charInfoData;
		CharPrivateData privateData = null;
		if (charData != null)
			privateData = charData.GetPrivateData(Game.Instance.connector.charIndex);
		
		bool thisStageOpen = false;
		if (privateData != null)
		{
			List<StageInfo> stageList = privateData.GetModeStageInfos(stageType);
			StageInfo stageInfo = privateData.GetStageInfo(stageList, stageID - 1);
			
			if (stageInfo != null)
				thisStageOpen = stageInfo.stageInfo == StageButton.eStageButton.Clear;
		}
		
		ModeTypeInfo modeInfo = GetModeTypeInfo(stageType);
		KingdomInfo kingdomInfo = GetKingdomInfo(kingdomID);
		
		SetModeInfo(modeInfo);
		SetKingdomInfo(kingdomInfo, stageTableInfo);
		
		if (this.gotoButton != null)
			this.gotoButton.isEnabled = thisStageOpen;
		
		if (this.gotoEnableLabel != null)
			this.gotoEnableLabel.gameObject.SetActive(thisStageOpen);
		if (this.gotoDisableLabel != null)
			this.gotoDisableLabel.gameObject.SetActive(!thisStageOpen);
	}
	
	public void SetModeInfo(ModeTypeInfo modeInfo)
	{
		string spriteName = "";
		string modeNameStr = "";
		Color labelColor = Color.white;
		
		if (modeInfo != null)
		{
			TableManager tableManager = TableManager.Instance;
			StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
			
			spriteName = modeInfo.spriteName;
			if (stringTable != null)
				modeNameStr = stringTable.GetData(modeInfo.stringID);
			labelColor = modeInfo.modeColor;
		}
		
		if (string.IsNullOrEmpty(spriteName) == true)
		{
			if (this.modeSprite != null)
				this.modeSprite.gameObject.SetActive(false);
		}
		else
		{
			if (modeSprite != null)
			{
				this.modeSprite.gameObject.SetActive(true);
				this.modeSprite.spriteName = spriteName;
			}
		}
		
		if (modeNameLabel != null)
			modeNameLabel.text = modeNameStr;
		
	}
	public void SetKingdomInfo(KingdomInfo kingdomInfo, StageTableInfo stageInfo)
	{
		string spriteName = "";
		string kingdomNameStr = "";
		string stageNameStr = "";
		
		if (kingdomInfo != null)
		{
			spriteName = kingdomInfo.kingdomFalg;
			kingdomNameStr = kingdomInfo.kingdomName;
		}
		
		if (stageInfo != null)
		{
			kingdomNameStr = stageInfo.kingdom;
			stageNameStr = string.Format("{0} {1}", stageNameTitleStr, stageInfo.stageNumber);
		}
		
		if (string.IsNullOrEmpty(spriteName) == true)
		{
			if (this.kingdomSprite != null)
				this.kingdomSprite.gameObject.SetActive(false);
		}
		else
		{
			if (this.kingdomSprite != null)
			{
				this.kingdomSprite.gameObject.SetActive(true);
				this.kingdomSprite.spriteName = spriteName;
			}
		}
		
		if (kingdomNameLabel != null)
			kingdomNameLabel.text = kingdomNameStr;
		if (stageNameLabel != null)
			stageNameLabel.text = stageNameStr;
	}
	
	public void OnGoto()
	{
		TownUI townUI = GameUI.Instance.townUI;
		
		if (parentWindow != null)
			parentWindow.SendMessage("OnCloseByGoto", SendMessageOptions.DontRequireReceiver);
		
		switch(this.gotoType)
		{
		case GotoMapInfo.eGotoType.eGoto_Stage:
			if (townUI != null)
				townUI.OnMapSelect(this.stageType, this.stageID);
			break;
		case GotoMapInfo.eGotoType.eGoto_Gamble:
			if (townUI != null)
				townUI.OnGambleWindow();
			break;
		case GotoMapInfo.eGotoType.eGoto_Shop:
			if (townUI != null)
				townUI.OnShopByTab(this.stageType);
			break;
		case GotoMapInfo.eGotoType.eGoto_Defence:
			if (townUI != null)
				townUI.OnRequestWaveInfo();
			break;
		}
	}
}
