using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MasteryWindow_New : PopupBaseWindow {
	public TownUI townUI = null;
	
	public Transform groupPanelNode = null;
	public string groupPrefabPath = "";
	
	public List<Transform> groupPanelPosInfos = new List<Transform>();
	public Vector3 GetPosInfo(int index)
	{
		Vector3 vPos = Vector3.zero;
		
		int nCount = groupPanelPosInfos.Count;
		if (index >= 0 && index < nCount)
			vPos = groupPanelPosInfos[index].localPosition;
		
		return vPos;
	}
	
	public List<MasteryGroupPanel> groupPanelList = new List<MasteryGroupPanel>();
	
	
	public UILabel masteryName = null;
	public UILabel masteryDesc = null;
	public UILabel masteryNeedInfo = null;
	public UILabel masteryPoint = null;
	public UILabel curMasteryInfo = null;
	public UILabel nextMasteryInfo = null;
	
	public UIButton learnButton = null;
	public UILabel learnButtonLabel = null;
	public UIButton cancelButton = null;
	public UILabel cancelButtonLabel = null;
	public UIButton saveButton = null;
	public UILabel saveButtonLabel = null;
	
	public UIButton initButton = null;
	public UILabel initButtonLabel = null;
	public UILabel initGoldLabel = null;
	
	public UILabel titleLabel = null;
	
	public Vector3 needResetGold = Vector3.zero;
	
	CharMasteryInfo charMasteryInfo = null;
	
	public UISprite nextArrow = null;
	
	public override void Awake()
	{
		this.windowType = TownUI.eTOWN_UI_TYPE.MASTERY;
		
		GameUI.Instance.masteryWindow = this;
	}
	
	public void InitWindow()
	{
		TableManager tableManager = TableManager.Instance;
		MasteryUITable masteryUITable = tableManager != null ? tableManager.masteryUITable : null;
		StringValueTable stringValueTable = tableManager != null ? tableManager.stringValueTable : null;
		
		if (stringValueTable != null)
			needResetGold.y = (float)stringValueTable.GetData("SkillResetPrice");
		
		if (initGoldLabel != null)
			initGoldLabel.text = string.Format("{0}", needResetGold.y);
		
		foreach(MasteryGroupPanel panel in groupPanelList)
		{
			DestroyObject(panel.gameObject, 0.0f);
		}
		groupPanelList.Clear();
		
		int charIndex = -1;
		if (Game.Instance.connector != null)
			charIndex = Game.Instance.connector.charIndex;
		
		CharInfoData charData = Game.Instance.charInfoData;
		CharPrivateData privateData = null;
		if (charData != null)
			privateData = charData.GetPrivateData(charIndex);
		
		MasteryManager_New masteryManager = null;
		PlayerController player = Game.Instance.player;
		if (player != null && player.lifeManager != null)
			masteryManager = player.lifeManager.masteryManager_New;
		
		
		GameDef.ePlayerClass classType = GameDef.ePlayerClass.CLASS_WARRIOR;
		if (privateData != null)
			classType = privateData.playerClass;
		
		if (masteryUITable != null)
			charMasteryInfo = masteryUITable.GetCharMasteryUIInfo(classType);
		
		Vector3 vPos = Vector3.zero;
		if (charMasteryInfo != null)
		{
			int index = 0;
			//int groupID = 0;
			MasteryGroupInfo groupUIInfo = null;
			MasteryGroupPanel groupPanel = null;
			
			foreach(var temp in charMasteryInfo.groupInfos)
			{
				//groupID = temp.Key;
				groupUIInfo = temp.Value;
				
				vPos = GetPosInfo(index);
				groupPanel = ResourceManager.CreatePrefab<MasteryGroupPanel>(groupPrefabPath, groupPanelNode, vPos);
				
				if (groupPanel != null)
				{
					groupPanelList.Add(groupPanel);
					groupPanel.InitWindow(groupUIInfo, masteryManager, this);
				}
				
				++index;
			}
		}
		
		MasteryIcon defaultIcon = null;
		if (groupPanelList.Count > 0)
		{
			MasteryGroupPanel defaultGroup = groupPanelList[0];
			if (defaultGroup != null && defaultGroup.masteryIcons.Count > 0)
				defaultIcon = defaultGroup.masteryIcons[0];
		}
		
		SetSelectedInfo(defaultIcon);
	}
	
	public void ResetSelectedInfo()
	{
		foreach(MasteryGroupPanel masteryGroup in groupPanelList)
		{
			masteryGroup.ResetSelected();
		}
	}
	
	public MasteryIcon selectedMasterySlot = null;
	public void SetSelectedInfo(MasteryIcon icon)
	{
		selectedMasterySlot = icon;
		
		MasteryInfo_New info = null;
		
		if (icon != null)
		{
			icon.SetSelected(true);
			info = icon.masteryInfo;
		}
		
		UpdateMasteryInfo(info);
		UpdateInfo();
	}
	
	public void UpdateMasteryInfo(MasteryInfo_New info)
	{
		string nameStr = "";
		string descStr = "";
		string needInfoStr = "";
		string pointInfoStr = "";
		string curInfoStr = "";
		string nextInfoStr = "";
		
		bool isMax = true;
		
		if (info != null)
		{
			nameStr = info.name;
			descStr = info.desc;
			needInfoStr = MakeNeedInfo(info);
			pointInfoStr = MakePointInfo(info);
			curInfoStr = MakeCurInfo(info);
			nextInfoStr = MakeNextInfo(info);
			
			if (info.activeType == MasteryInfo_New.eMasteryActiveType.Active ||
				(info.maxPoint == (info.addPoint + info.curPoint)))
				isMax = true;
			else
				isMax = false;
		}
		
		if (masteryName != null)
			masteryName.text = nameStr;
		if (masteryDesc != null)
			masteryDesc.text = descStr;
		if (masteryNeedInfo != null)
			masteryNeedInfo.text = needInfoStr;
		if (masteryPoint != null)
			masteryPoint.text = pointInfoStr;
		if (curMasteryInfo != null)
			curMasteryInfo.text = curInfoStr;
		if (nextMasteryInfo != null)
			nextMasteryInfo.text = nextInfoStr;
		
		if (nextArrow != null)
			nextArrow.gameObject.SetActive(!isMax);
		
		bool isEnable = false;
		if (info != null)
		{
			if (info.CheckEnable() == true && info.CanUpgrade() == true && info.manager.CheckUsablePoint() == true)
				isEnable = true;
		}
		
		if (learnButton != null)
			learnButton.isEnabled = isEnable;
		
		int addPoint = 0;
		if (info != null)
			addPoint = info.manager.CheckAddPoint();
		
		isEnable = addPoint > 0;
		
		if (cancelButton != null)
			cancelButton.isEnabled = isEnable;
		
		if (saveButton != null)
			saveButton.isEnabled = isEnable;
	}
	
	
	
	public string MakeCurInfo(MasteryInfo_New info)
	{
		string curInfo = "";
		if (info != null)
		{
			PlayerController player = Game.Instance.player;
			if (player != null)
				curInfo = player.GetCurMasteryInfo_New(info);
		}
		return curInfo;
	}
	
	public string MakeNextInfo(MasteryInfo_New info)
	{
		string curInfo = "";
		if (info != null)
		{
			PlayerController player = Game.Instance.player;
			if (player != null)
				curInfo = player.GetNextMasteryInfo_New(info);
		}
		return curInfo;
	}
	
	public string MakePointInfo(MasteryInfo_New info)
	{
		string pointInfo = "";
		
		if (info != null)
		{
			if (info.maxPoint > 1)
				pointInfo = string.Format("{0} / {1}", info.Point, info.maxPoint);
			else
				pointInfo = string.Format("{0}", info.Point);
		}
		
		return pointInfo;
	}
	
	
	public int needStringLabelID = 41;
	public string MakeNeedInfo(MasteryInfo_New info)
	{
		string needInfo = "";
		string needGroupInfo = "";
		string needMasteryInfo = "";
		
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		string needLabelStr = stringTable != null ? stringTable.GetData(needStringLabelID) : "";
		
		int errorCount = 0;
		if (info != null)
		{
			int curGroupPoint = 0;
			if (info.needGroupID != 0 && info.needGroupPoint != 0)
			{
				curGroupPoint = info.manager.GetGroupPoint(info.needGroupID);
				if (curGroupPoint < info.needGroupPoint)
					errorCount++;
				
				MasteryGroupInfo groupUIInfo = null;
				if (charMasteryInfo != null)
					groupUIInfo = charMasteryInfo.GetGroupInfo(info.needGroupID);
				
				string groupName = groupUIInfo != null ? groupUIInfo.groupName : "";
				needGroupInfo = string.Format("{0} {1} {2}", groupName, info.needGroupPoint, needLabelStr);
			}
			
			int index = 0;
			foreach(int id in info.needMasteryIDs)
			{
				MasteryInfo_New needMastery = info.manager.GetMastery(id);
				if (needMastery == null || needMastery.Point < needMastery.maxPoint)
					errorCount++;
				
				if (index == 0)
					needMasteryInfo = needMastery != null ? needMastery.name : "";
				else
					needMasteryInfo += string.Format(",{0}", needMastery != null ? needMastery.name : "");
			}
			if (info.needMasteryIDs.Count > 0)
				needMasteryInfo += string.Format(" {0}", needLabelStr);
		}
		
		Color infoColor = Color.white;
		if (errorCount != 0)
			infoColor = Color.red;
		
		needInfo = GameDef.RGBToHex(infoColor) + string.Format("{0}\n{1}", needGroupInfo, needMasteryInfo) + "[-]";
		
		return needInfo;
	}
	
	public string resetConfirmPopupPrefabPath = "UI/Item/MasteryWindow/ResetConfirmPopup";
	public void OnResetConfirmPopup(GameObject obj)
	{
		if (requestCount > 0)
			return;
		
		MasteryManager_New masteryManager = null;
		PlayerController player = Game.Instance.player;
		if (player != null && player.lifeManager != null)
			masteryManager = player.lifeManager.masteryManager_New;
		
		if (masteryManager == null || masteryManager.CheckPoint() == 0)
			return;
		
		BaseConfirmPopup resetConfirmPopup = ResourceManager.CreatePrefab<BaseConfirmPopup>(resetConfirmPopupPrefabPath, popupNode, Vector3.zero);
		if (resetConfirmPopup != null)
		{
			resetConfirmPopup.cancelButtonMessage.target = this.gameObject;
			resetConfirmPopup.cancelButtonMessage.functionName = "OnClosePopup";
			
			resetConfirmPopup.okButtonMessage.target = this.gameObject;
			resetConfirmPopup.okButtonMessage.functionName = "OnResetMastery";
			
			if (resetConfirmPopup.priceValueLabel != null)
				resetConfirmPopup.priceValueLabel.text = string.Format("{0:#,###,##0}", this.needResetGold.y);
			
			popupList.Add(resetConfirmPopup);
		}
	}
	
	public void OnResetMastery(GameObject obj)
	{
		CashItemType checkType = CheckNeedGold(needResetGold);
		if (checkType != CashItemType.None)
		{
			ClosePopup();
			OnNeedMoneyPopup(checkType, this);
			return;
		}
		
		IPacketSender packetSender = Game.Instance.packetSender;
		if (packetSender != null)
		{
			packetSender.SendRequestMasteryReset();
			
			requestCount++;
		}
	}
	
	public void OnResultResetMastery(NetErrorCode errorCode)
	{
		ClosePopup();
		
		if (errorCode == NetErrorCode.OK)
		{
			requestCount--;
			
			MasteryManager_New masteryManager = null;
			PlayerController player = Game.Instance.player;
			if (player != null && player.lifeManager != null)
				masteryManager = player.lifeManager.masteryManager_New;
			
			int recoveryPoint = 0;
			if (masteryManager != null)
				recoveryPoint = masteryManager.ResetMastery();
			
			Debug.Log("ResetPoint is " + recoveryPoint);
			
			UpdateUI();
		}
		else
		{
			OnErrorMessage(errorCode, this);
		}
	}
	
	public void OnCancelMastery()
	{
		MasteryManager_New masteryManager = null;
		PlayerController player = Game.Instance.player;
		if (player != null && player.lifeManager != null)
			masteryManager = player.lifeManager.masteryManager_New;
		
		int recoveryPoint = 0;
		if (masteryManager != null)
			recoveryPoint = masteryManager.CancelAddPoint();
		
		Debug.Log("CancelPoint is " + recoveryPoint);
		
		UpdateUI();
	}
	
	public int requestCount = 0;
	public void OnApplyMastery(GameObject obj)
	{
		if (requestCount > 0)
			return;
		
		MasteryManager_New masteryManager = null;
		PlayerController player = Game.Instance.player;
		if (player != null && player.lifeManager != null)
			masteryManager = player.lifeManager.masteryManager_New;
		
		SkillUpgradeDBInfo upgradeInfo = new SkillUpgradeDBInfo();
		if (masteryManager != null)
			masteryManager.GetApplyPoint(upgradeInfo);
		
		IPacketSender packetSender = Game.Instance.packetSender;
		if (packetSender != null)
		{
			packetSender.SendRequestMasteryUpgrade(upgradeInfo);
			
			requestCount++;
		}
	}
	
	public void OnApplyMasteryAndClose(GameObject obj)
	{
		OnApplyMastery(obj);
		
		OnConfirmPopupCancel(obj);
	}
	
	public void OnResultApplyMastery(SkillUpgradeDBInfo info)
	{
		requestCount--;
		
		MasteryManager_New masteryManager = null;
		PlayerController player = Game.Instance.player;
		if (player != null && player.lifeManager != null)
			masteryManager = player.lifeManager.masteryManager_New;
		
		if (masteryManager != null)
			masteryManager.ApplyUpgradeInfo(info);
		
		UpdateUI();
	}
	
	public void UpdateUI()
	{
		foreach(MasteryGroupPanel panel in groupPanelList)
		{
			panel.UpdateUI();
		}
		
		MasteryInfo_New info = selectedMasterySlot != null ? selectedMasterySlot.masteryInfo : null;
		UpdateMasteryInfo(info);
		
		UpdateInfo();
	}
	
	public void UpdateInfo()
	{
		int charIndex = 0;
		CharInfoData charData = Game.Instance.charInfoData;
		if (Game.Instance.connector != null)
			charIndex = Game.Instance.connector.charIndex;
		
		CharPrivateData privateData = null;
		if (charData != null)
			privateData = charData.GetPrivateData(charIndex);
		
		MasteryManager_New masteryManager = null;
		PlayerController player = Game.Instance.player;
		if (player != null && player.lifeManager != null)
			masteryManager = player.lifeManager.masteryManager_New;
		
		int curSkillPoint = privateData.baseInfo.SkillPoint;
		int useSkillPoint = masteryManager != null ? masteryManager.CheckAddPoint() : 0;
		
		if (titleLabel != null)
			titleLabel.text = string.Format("{0}", curSkillPoint - useSkillPoint);
		
		bool bInitMastery = false;
		if (masteryManager != null)
			bInitMastery = masteryManager.CheckPoint() > 0;
		
		if (initButton != null)
			initButton.isEnabled = bInitMastery;
	}
	
	public void OnLearnMastery()
	{
		MasteryInfo_New info = null;
		if (selectedMasterySlot != null)
			info = selectedMasterySlot.masteryInfo;
		
		if (info != null && info.CheckEnable() == true && info.Point < info.maxPoint)
		{
			if (info.activeType == MasteryInfo_New.eMasteryActiveType.Active)
			{
				if (info.manager.activeMastery == null ||
					info.manager.activeMastery.id == 0)
				{
					info.addPoint += 1;
					info.manager.activeMastery = info;
				}
			}
			else
				info.addPoint += 1;
			
			
			UpdateUI();
		}
	}
	
	public override void OnBack()
	{
		MasteryManager_New masteryManager = null;
		PlayerController player = Game.Instance.player;
		if (player != null && player.lifeManager != null)
			masteryManager = player.lifeManager.masteryManager_New;
		
		int addPoint = 0;
		if (masteryManager != null)
			addPoint = masteryManager.CheckAddPoint();
		
		if (addPoint > 0)
		{
			OnApplyConfirmPopup();
			return;
		}
		
		requestCount = 0;
		
		base.OnBack();
	}
	
	public override void OnErrorMessage(NetErrorCode errorCode, PopupBaseWindow popupBase)
	{
		requestCount--;
		
		base.OnErrorMessage(errorCode, popupBase);
	}
	
	public string applyConfirmPopupPrefabPath = "UI/Item/BuyConfirmPopup";
	public void OnApplyConfirmPopup()
	{
		BaseConfirmPopup applyConfirmPopup = ResourceManager.CreatePrefab<BaseConfirmPopup>(applyConfirmPopupPrefabPath, popupNode, Vector3.zero);
		if (applyConfirmPopup != null)
		{
			applyConfirmPopup.cancelButtonMessage.target = this.gameObject;
			applyConfirmPopup.cancelButtonMessage.functionName = "OnConfirmPopupCancel";
			
			applyConfirmPopup.okButtonMessage.target = this.gameObject;
			applyConfirmPopup.okButtonMessage.functionName = "OnApplyMasteryAndClose";
			
			popupList.Add(applyConfirmPopup);
		}
	}
	
	public void OnConfirmPopupCancel(GameObject obj)
	{
		OnCancelMastery();
		
		requestCount = 0;
		
		base.OnBack();
	}
}
