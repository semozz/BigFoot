using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ModeSelectButtonInfo
{
	public GameObject selectObj;
	public GameObject unSelectObj;
	
	public GameObject fxObj;
}

public class MapSelect : PopupBaseWindow 
{
	public GameObject stageButtonRoot = null;
	public List<StageButton> stageButtons = new List<StageButton>();
	
	public List<StageButton> specialStageButtons = new List<StageButton>();
	
	public List<ModeSelectButtonInfo> modSeletButtonInfos = new List<ModeSelectButtonInfo>();
	
	public TownUI townUI = null;
	
	public UIScrollBar mapScroll = null;
	
	public override void Awake()
	{
		this.windowType = TownUI.eTOWN_UI_TYPE.MAPSELECT;
		
		ScanStageButtons();
		
		//InitMap(0);
		//GameUI.Instance.mapSelect = this;
	}
	
	public void ScanStageButtons()
	{
		stageButtons.Clear();

		StageButton obj;
		Component[] objs = transform.GetComponentsInChildren(typeof(StageButton), true);

		for (int i = 0; i < objs.Length; ++i)
		{
			obj = (StageButton)objs[i];
			if (obj.name.Contains("Stage") == false)
				continue;
			
			stageButtons.Add(obj);
		}
		
		stageButtons.Sort(SortByName);
	}
	
	static public int SortByName (StageButton a, StageButton b) { return string.Compare(a.name, b.name); }
	
	
	public void InitMap(int targetType)
	{
		if (this.audio != null)
			this.audio.mute = !GameOption.effectToggle;
				
		this.stageType = -1;
		
		switch(targetType)
		{
		case 0:
			this.OnNormalMode(null);
			break;
		case 1:
			this.OnHardMode(null);
			break;
		case 2:
			this.OnHellMode(null);
			break;
		}
		
		if (TownUI.stageIndex >= 0)
		{
			OnMapStart(TownUI.stageIndex);
			TownUI.stageIndex = -1;
		}
		
		if (TownUI.notifyOpen)
		{
			NotifyOpenPopup();
			TownUI.notifyOpen = false;
		}
	}
	
	public void InitStageButtons()
	{
		int index = 0;
		int nCount = stageButtons.Count;
		
		GameDef.ePlayerClass _class = Game.Instance.playerClass;
		CharPrivateData privateData = Game.Instance.GetCharPrivateData(_class);
		
		Vector2 minMax = Vector2.zero;
		Vector3 buttonPos = Vector3.zero;
		
		List<StageInfo> modeStageInfos = privateData != null ? privateData.GetModeStageInfos(stageType) : null;
		
		StageButton gotoStage = null;
		for (index = 0; index < nCount; ++index)
		{
			StageButton button = stageButtons[index];
			StageInfo info = null;
			
			info = privateData != null ? privateData.GetStageInfo(modeStageInfos, index) : null;
			
			if (button != null)
			{
				button.ChangeState(info != null ? info.stageInfo : StageButton.eStageButton.Locked);
			
				buttonPos = button.transform.localPosition;
			
				minMax.x = Mathf.Min(minMax.x, buttonPos.x);
				minMax.y = Mathf.Max(minMax.y, buttonPos.x);
				
				if (info != null &&
					info.stageInfo != StageButton.eStageButton.Locked)
					gotoStage = button;
			}
		}
		
		if (gotoStage != null)
		{
			Vector3 gotoPos = gotoStage.transform.localPosition;
			mapScrollRate = (gotoPos.x - minMax.x) / (minMax.y - minMax.x);
			
			if (mapScroll != null)
				mapScroll.scrollValue = mapScrollRate;
			
			Invoke("InitMapScroll", 0.2f);
		}
		
		UpdateSpecialStage();
	}
	
	private void UpdateSpecialStage()
	{
		foreach(StageButton button in specialStageButtons)
		{
			button.gameObject.SetActive(false);
			//button.ChangeState(StageButton.eStageButton.Locked);
		}
		
		CharInfoData charData = Game.Instance.charInfoData;
		if (charData != null)
		{
			foreach(int index in charData.specialStageInfo)
			{
				StageButton button = specialStageButtons[index];
				if (button != null)
				{
					button.gameObject.SetActive(true);
					
					button.ChangeState(StageButton.eStageButton.Normal);
				}
			}
		}
	}
	
	public float mapScrollRate = 0.0f;
	public void InitMapScroll()
	{
		if (mapScroll != null)
			mapScroll.scrollValue = mapScrollRate;
	}
	
	public override void OnBack()
	{
		base.OnBack();
	}
	
	public string mapStartPrefabPath = "UI/MapSelect/MapStartWindow";
	public MapStartWindow mapStart = null;
	public void OnMapStart(int stageID)
	{
		if (CheckModeLevelLimit(stageType, stageID) == false)
			return;
		
		if (mapStart == null)
			mapStart = ResourceManager.CreatePrefab<MapStartWindow>(mapStartPrefabPath, popupNode, Vector3.one);
		else
			mapStart.gameObject.SetActive(true);
		
		if (mapStart != null)
			mapStart.InitWindow(stageID, stageType);
	}
	
	public bool CheckModeLevelLimit(int stageType, int stageID)
	{
		TableManager tableManager = TableManager.Instance;
		StageTable stageTable = null;
		CharExpTable expTable = null;
		if (tableManager != null)
		{
			stageTable = tableManager.stageTable;
			expTable = tableManager.charExpTable;
		}
		
		int charLevel = 1;
		int limitLevel = 1;
		
		CharInfoData charData = Game.Instance.charInfoData;
		if (charData != null)
		{
			int charIndex = Game.Instance.connector.charIndex;
			CharPrivateData privateData = charData.GetPrivateData(charIndex);
			
			if (privateData != null && expTable != null)
				charLevel = expTable.GetLevel(privateData.baseInfo.ExpValue);
		}
		
		if (stageTable != null)
		{
			StageTableInfo stageTableInfo = stageTable.GetData(stageID);
			BasicStageInfo basicStageInfo = stageTableInfo != null ? stageTableInfo.GetBasicStageInfo(stageType) : null;
			
			if (basicStageInfo != null)
				limitLevel = basicStageInfo.limitLevel;
		}
		
		if (charLevel < limitLevel)
		{
			PopupLimitLevel(limitLevel);
			return false;
		}
		
		return true;
	}
	
	public string limitLevelPopupPrefabPath = "UI/MapSelect/LimitLevelPopup";
	public void PopupLimitLevel(int limiLevel)
	{
		BaseConfirmPopup popup = ResourceManager.CreatePrefab<BaseConfirmPopup>(limitLevelPopupPrefabPath, popupNode, Vector3.zero);
		
		if (popup != null)
		{
			TableManager tableManager = TableManager.Instance;
			StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
			
			popup.okButtonMessage.target = this.gameObject;
			popup.okButtonMessage.functionName = "OnCancelPopup";
			
			if (popup.messageLabel != null)
			{
				string formatString = string.Format("{0} Under Level Limit!!!", limiLevel);
				if (stringTable != null)
					formatString = stringTable.GetData(popup.messageStringID);
				
				popup.messageLabel.text = string.Format(formatString, limiLevel);
			}
			
			popupList.Add(popup);
		}
	}
	
	public int stageType = 0;
	public void OnHardMode(GameObject obj)
	{
		if (stageType == 1)
			return;
		
		stageType = 1;
		SetModeButton(stageType);
		
		InitStageButtons();
	}
	
	public void OnNormalMode(GameObject obj)
	{
		if (stageType == 0)
			return;
		
		stageType = 0;
		SetModeButton(stageType);
		
		InitStageButtons();
	}
	
	public void OnHellMode(GameObject obj)
	{
		if (stageType == 2)
			return;
		
		stageType = 2;
		SetModeButton(stageType);
		
		InitStageButtons();
	}
	
	public void SetModeButton(int stageType)
	{
		int nCount = modSeletButtonInfos.Count;
		for (int type = 0; type < Game.maxStageType; ++type)
		{
			ModeSelectButtonInfo info = modSeletButtonInfos[type];
			
			if (info == null)
				continue;
			
			SetModeButton(info, type == stageType);
		}
	}
	
	private void SetModeButton(ModeSelectButtonInfo info, bool isActive)
	{
		if (info.selectObj != null)
			info.selectObj.SetActive(isActive);
		if (info.unSelectObj != null)
			info.unSelectObj.SetActive(!isActive);
		
		if (info.fxObj != null)
			info.fxObj.SetActive(isActive);
	}
	
	public string hardOpenPrefabPath = "UI/MapSelect/HardOpenPopup";
	public string hellOpenPrefabPath = "UI/MapSelect/HellOpenPopup";
	public void NotifyOpenPopup()
	{
		string prefabPath = "";
		switch(stageType)
		{
		case 0:
		case 1:
			prefabPath = hardOpenPrefabPath;
			break;
		case 2:
			prefabPath = hellOpenPrefabPath;
			break;
		}
		BaseConfirmPopup notifyOpenPopup = null;
		notifyOpenPopup = ResourceManager.CreatePrefab<BaseConfirmPopup>(prefabPath, popupNode, Vector3.one);
		if (notifyOpenPopup != null)
		{
			notifyOpenPopup.okButtonMessage.target = this.gameObject;
			notifyOpenPopup.okButtonMessage.functionName = "OnCancelPopup";
			popupList.Add(notifyOpenPopup);
		}
	}
	
	public string specialMapStartPrefabPath = "UI/MapSelect/SpecialStageMapStartWindow";
	public MapStartWindow specialMapStart = null;
	public void OnSpecialMapStart(int stageID)
	{
		if (CheckModeLevelLimit(stageType, stageID) == false)
			return;
		
		if (specialMapStart == null)
			specialMapStart = ResourceManager.CreatePrefab<MapStartWindow>(specialMapStartPrefabPath, popupNode, Vector3.one);
		else
			specialMapStart.gameObject.SetActive(true);
		
		if (specialMapStart != null)
			specialMapStart.InitWindow(stageID, stageType);
	}
	
	public void OnCancelPopup(GameObject obj)
	{
		ClosePopup();
	}
}
