using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AwakeningLevelWindow : PopupBaseWindow {
	public TownUI townUI = null;
	
	private AwakeningLevelManager awakeningLevelManager = null;
	
	public Transform skillRoot = null;
	public string awakeningLevelPrefab = "UI/MasteryWindow/AwakeningLevelPanel";
	public List<Transform> levelPosList = new List<Transform>();
	
	public UILabel titleLabel = null;
	public int titleStringID = 234;
	
	public UILabel initButtonGoldLabel = null;
	public UILabel initButtonTitleLabel = null;
	public int initTitleStringID = 226;
	
	public UILabel availablePointLabel = null;
	public int availablePointStringID = 227;
	
	public UILabel canBuyPointTitleLabel = null;
	public int canBuyPointTitleStringID = 228;
	public UILabel canBuyPointLabel = null;
	
	public UILabel buyPointButtonTitle = null;
	public int buyPointButtonStringID = 229;
	
	public UILabel awakeningNameLabel = null;
	public UILabel awakeningStepLabel = null;
	
	public UILabel curAwakeningInfoLabel = null;
	public UILabel nextAwakeningInfoLabel = null;
	
	public UILabel learnAwakeningGoldLabel = null;
	public UILabel learnskillTitleLabel = null;
	public int learnSkillTitleStringID = 231;
	
	public UILabel resetSkillLabel = null;
	public int resetSkillStringID = 232;
	public UILabel saveSkillLabel = null;
	public int saveSkillStringID = 241;
	
	public UIButton initSkillButton = null;
	public UIButtonMessage initSkillButtonMessage = null;
	public UIButton buyPointButton = null;
	public UIButtonMessage buyPointButtonMessage = null;
	public UIButton learnSkillButton = null;
	public UIButtonMessage learnSkillButtonMessage = null;
	
	public UIButton resetSkillButton = null;
	public UIButtonMessage resetSkillButtonMessage = null;
	public UIButton saveSkillButton = null;
	public UIButtonMessage saveSkillButtonMessage = null;
	
	private List<AwakeningLevelIcon> levelIconList = new List<AwakeningLevelIcon>();
	
	private StringTable stringTable = null;
	
	public int requestCount = 0;
	
	private Vector3 needResetMoney = Vector3.zero;
	public override void Awake()
	{
		this.windowType = TownUI.eTOWN_UI_TYPE.AWAKENING;
		
		GameUI.Instance.awakeningWindow = this;
		
		TableManager tableManager = TableManager.Instance;
		StringValueTable stringValueTable = null;
		if (tableManager != null)
		{
			stringTable = tableManager.stringTable;
			stringValueTable = tableManager.stringValueTable;
		}
		
		needResetMoney.y = 100.0f;
		if (stringValueTable != null)
			needResetMoney.y = stringValueTable.GetData("AwakeningSkillInitJewel");
		string initJewelInfoStr = string.Format("{0:#,###,##0}", (int)needResetMoney.y);
		PopupBaseWindow.SetLabelString(initButtonGoldLabel, initJewelInfoStr);
		
		PopupBaseWindow.SetLabelString(canBuyPointTitleLabel, canBuyPointTitleStringID, stringTable);
		
		PopupBaseWindow.SetLabelString(initButtonTitleLabel, initTitleStringID, stringTable);
		PopupBaseWindow.SetLabelString(buyPointButtonTitle, buyPointButtonStringID, stringTable);
		PopupBaseWindow.SetLabelString(learnskillTitleLabel, learnSkillTitleStringID, stringTable);
		
		PopupBaseWindow.SetLabelString(resetSkillLabel, resetSkillStringID, stringTable);
		PopupBaseWindow.SetLabelString(saveSkillLabel, saveSkillStringID, stringTable);
	}
	
	public void InitWindow(AwakeningLevelManager skillManager)
	{
		awakeningLevelManager = skillManager;
		
		string className = "";
		string titleStr = "Awakening Level";
		int level = 0;
		int charIndex = 0;
		if (Game.Instance.connector != null)
			charIndex = Game.Instance.connector.charIndex;
		
		if (awakeningLevelManager != null)
			level = awakeningLevelManager.curLevel;
		
		if (stringTable != null)
		{
			titleStr = stringTable.GetData(titleStringID);
			className = stringTable.GetData(charIndex + 1);
		}
		
		string titleString = string.Format("{0} - {1} {2}", className, titleStr, level);
		PopupBaseWindow.SetLabelString(titleLabel, titleString);
		
		UpdateUI();
		
		
		AwakeningLevelIcon defaultIcon = null;
		if (levelIconList.Count > 0)
			defaultIcon = levelIconList[0];
		
		if (defaultIcon != null)
			OnSelectSkill(defaultIcon.gameObject);
	}
	
	public void UpdateUI()
	{
		UpdateLevelPoint();
		
		if (awakeningLevelManager != null)
		{
			AwakeningLevelIcon levelIcon = null;
			int index = 0;
			Vector3 skillIconPos = Vector3.zero;
			foreach(AwakeningLevel skill in awakeningLevelManager.skillList)
			{
				levelIcon = GetLevelIconByIndex(index);
				if (levelIcon == null)
				{
					skillIconPos = GetAwakeningLevelPos(index);
					levelIcon = ResourceManager.CreatePrefab<AwakeningLevelIcon>(awakeningLevelPrefab, skillRoot, skillIconPos);
					
					levelIconList.Add(levelIcon);
				}
				
				if (levelIcon != null)
				{
					levelIcon.SetSkillInfo(skill);
					levelIcon.message.target = this.gameObject;
					levelIcon.message.functionName = "OnSelectSkill";
				}
				
				index++;
			}
		}
		
		GameObject obj = selectedLevelIcon != null ? selectedLevelIcon.gameObject : null;
		OnSelectSkill(obj);
	}
	
	public AwakeningLevelIcon GetLevelIconByIndex(int index)
	{
		AwakeningLevelIcon levelIcon = null;
		int nCount = levelIconList.Count;
		if (index >= 0 && index < nCount)
			levelIcon = levelIconList[index];
		
		return levelIcon;
	}
	
	public Vector3 GetAwakeningLevelPos(int index)
	{
		Transform trans = null;
		int nCount = levelPosList.Count;
		if (index >= 0 && index < nCount)
			trans = levelPosList[index];
		
		Vector3 pos = Vector3.zero;
		if (trans != null)
			pos = trans.localPosition;
		
		return pos;
	}
	
	private AwakeningLevelIcon selectedLevelIcon = null;
	public void OnSelectSkill(GameObject obj)
	{
		selectedLevelIcon = obj != null ? obj.GetComponent<AwakeningLevelIcon>() : null;
		if (selectedLevelIcon != null)
		{
			foreach(AwakeningLevelIcon icon in levelIconList)
			{
				if (selectedLevelIcon == icon)
					icon.SetSelectFrame(true);
				else
					icon.SetSelectFrame(false);
			}
		}
		else
			selectedLevelIcon = null;
		
		AwakeningLevel skill = selectedLevelIcon != null ? selectedLevelIcon.GetSkill() : null;
		UpdateLevelInfo(skill);
	}
	
	private string availablePointLabelStr = "";
	public void UpdateSkillPointInfo()
	{
		if (string.IsNullOrEmpty(availablePointLabelStr) == true)
			availablePointLabelStr = stringTable.GetData(availablePointStringID);
				
		int availableLevelPoint = 0;
		int canBuyLevelPoint = 0;
		
		if (awakeningLevelManager != null)
		{
			availableLevelPoint = awakeningLevelManager.GetAvailablePoint();
			canBuyLevelPoint = awakeningLevelManager.GetAvailableBuyPoint();
		}
		
		if (availablePointLabel != null)
			availablePointLabel.text = string.Format("{0} {1:#,###,##0}", availablePointLabelStr, availableLevelPoint);
		if (canBuyPointLabel != null)
			canBuyPointLabel.text = string.Format("{0:#,###,##0}", canBuyLevelPoint);
		
		bool isCanBuyPoint = canBuyLevelPoint > 0;
		if (buyPointButton != null)
			buyPointButton.isEnabled = isCanBuyPoint;
	}
	
	public int levelStepFormatID = 230;
	private string levelStepLabelStr = "";
	public void UpdateLevelInfo(AwakeningLevel skill)
	{
		if (string.IsNullOrEmpty(levelStepLabelStr) == true)
			levelStepLabelStr = stringTable.GetData(levelStepFormatID);
		
		string awakeningName = "";
		string awakeningStepInfoStr = "";
		string awakeningCurInfoStr = "";
		string awakeningNextInfoStr = "";
		string awakeningLearnGoldInfoStr = "";
		
		if (skill != null)
		{
			string formatStr = "";
			
			awakeningName = skill.skillName;
			
			if (stringTable != null)
			{
				formatStr = stringTable.GetData(levelStepFormatID);
				awakeningStepInfoStr = string.Format("{0} {1:#,###,##0}/{2:#,###,##0}", levelStepLabelStr, skill.Point, skill.maxPoint);
			}
			
			awakeningCurInfoStr = skill.GetCurSkillInfo();
			awakeningNextInfoStr = skill.GetNextSkillInfo();
			
			awakeningLearnGoldInfoStr = string.Format("{0:#,###,##0}", skill.LearnGold());
		}
		
		if (awakeningNameLabel != null)
			awakeningNameLabel.text = awakeningName;
		if (awakeningStepLabel != null)
			awakeningStepLabel.text = awakeningStepInfoStr;
		if (curAwakeningInfoLabel != null)
			curAwakeningInfoLabel.text = awakeningCurInfoStr;
		if (nextAwakeningInfoLabel != null)
			nextAwakeningInfoLabel.text = awakeningNextInfoStr;
		
		if (learnAwakeningGoldLabel != null)
			learnAwakeningGoldLabel.text = awakeningLearnGoldInfoStr;
		
		bool bCanLearn = false;
		int availablePoint = 0;
		int curSkillPoint = 0;
		if (this.awakeningLevelManager != null)
			availablePoint = awakeningLevelManager.GetAvailablePoint();
		if (skill != null)
			curSkillPoint = skill.Point;
		
		bCanLearn = (availablePoint > 0) && (skill != null) && (curSkillPoint < skill.maxPoint);
		
		if (learnSkillButton != null)
			learnSkillButton.isEnabled = bCanLearn;
	}
	
	public string resetConfirmPopupPrefabPath = "UI/MasteryWindow/AwakenResetConfirmPopup";
	public void OnResetConfirmPopup(GameObject obj)
	{
		if (requestCount > 0)
			return;
		
		BaseConfirmPopup resetConfirmPopup = ResourceManager.CreatePrefab<BaseConfirmPopup>(resetConfirmPopupPrefabPath, popupNode, Vector3.zero);
		if (resetConfirmPopup != null)
		{
			resetConfirmPopup.cancelButtonMessage.target = this.gameObject;
			resetConfirmPopup.cancelButtonMessage.functionName = "OnClosePopup";
			
			resetConfirmPopup.okButtonMessage.target = this.gameObject;
			resetConfirmPopup.okButtonMessage.functionName = "OnResetSkill";
			
			if (resetConfirmPopup.priceValueLabel != null)
				resetConfirmPopup.priceValueLabel.text = string.Format("{0:#,###,##0}", (int)this.needResetMoney.y);
			
			popupList.Add(resetConfirmPopup);
		}
	}
	
	public void OnResetSkill(GameObject obj)
	{
		CashItemType checkType = CheckNeedGold(needResetMoney);
		if (checkType != CashItemType.None)
		{
			ClosePopup();
			OnNeedMoneyPopup(checkType, this);
			return;
		}
		
		IPacketSender packetSender = Game.Instance.packetSender;
		if (packetSender != null)
		{
			requestCount++;
			packetSender.SendRequestAwakeningReset();
		}
	}
	
	public string buyPointPopupPrefab = "UI/MasteryWindow/Point_ConfirmPopup";
	public BuyAwakeningPointPopup buyPointPopup = null;
	public void BuySkillPoint()
	{
		/*
		if (awakeningLevelManager != null)
		{
			awakeningLevelManager.buyPoint += 1;
			
			UpdateSkillPointInfo();
			UpdateLevelPoint();
		}
		*/
		
		if (buyPointPopup == null)
		{
			buyPointPopup = ResourceManager.CreatePrefab<BuyAwakeningPointPopup>(buyPointPopupPrefab, popupNode, Vector3.zero);
			
			if (buyPointPopup != null)
			{
				if (buyPointPopup.cancelButtonMessage != null)
				{
					buyPointPopup.cancelButtonMessage.target = this.gameObject;
					buyPointPopup.cancelButtonMessage.functionName = "OnBuyPointPopupClose";
				}
				if (buyPointPopup.okButtonMessage != null)
				{
					buyPointPopup.okButtonMessage.target = this.gameObject;
					buyPointPopup.okButtonMessage.functionName = "OnBuyLevelPoint";
				}
			}
		}
		
		if (buyPointPopup != null)
		{
			buyPointPopup.gameObject.SetActive(true);
			buyPointPopup.InitWindow(this.awakeningLevelManager);
		}
	}
	
	public void OnBuyPointPopupClose(GameObject obj)
	{
		if (buyPointPopup != null)
		{
			DestroyObject(buyPointPopup.gameObject, 0.1f);
			buyPointPopup = null;
		}
	}
	
	public void OnBuyLevelPoint(GameObject obj)
	{
		if (requestCount > 0)
			return;
		
		if (buyPointPopup != null)
		{
			Vector3 needMoney = Vector3.zero;
			needMoney.y = (float)buyPointPopup.needJewel;
			int buyPoint = buyPointPopup.buyPoint;
			
			OnBuyPointPopupClose(null);
			
			CashItemType checkType = CheckNeedGold(needMoney);
			if (checkType != CashItemType.None)
			{
				OnNeedMoneyPopup(checkType, this);
				return;
			}
			else
			{
				IPacketSender packetSender = Game.Instance.packetSender;
				if (packetSender != null)
				{
					requestCount++;
					packetSender.SendRequestAwakeningBuyPoint(buyPoint);
				}
			}
		}
	}	
	
	private float needGold = 0.0f;
	public void LearnSkill()
	{
		AwakeningLevel skill = selectedLevelIcon != null ? selectedLevelIcon.GetSkill() : null;
		
		if (skill == null)
			return;
		
		skill.AddPoint();
		
		needGold = awakeningLevelManager.GetLearnNeedGold();
		
		UpdateLevelPoint();
	}
	
	public void UpdateLevelPoint()
	{
		if (selectedLevelIcon != null)
		{
			selectedLevelIcon.UpdateInfo();
			
			UpdateLevelInfo(selectedLevelIcon.GetSkill());
		}
		
		UpdateCoinInfo();
		UpdateSkillPointInfo();
		UpdateResetButton();
	}
	
	public void ResetSkill()
	{
		if (awakeningLevelManager != null)
		{
			awakeningLevelManager.ResetSkills();
			needGold = 0.0f;
			
			foreach(AwakeningLevelIcon skillIcon in levelIconList)
				skillIcon.UpdateInfo();
			
			UpdateLevelPoint();
		}
	}
	
	public void SaveSkill()
	{
		if (requestCount > 0)
			return;
		
		Vector3 needMoney = Vector3.zero;
		needMoney.x = needGold;
		
		CashItemType checkType = PopupBaseWindow.CheckNeedGold(needMoney);
		if (checkType != CashItemType.None)
		{
			ClosePopup();
			OnNeedMoneyPopup(checkType, this);
			return;
		}
		else
		{
			SkillUpgradeDBInfo upgradeInfo = new SkillUpgradeDBInfo();
			if (awakeningLevelManager != null)
				awakeningLevelManager.GetApplyPoint(upgradeInfo);
		
			IPacketSender packetSender = Game.Instance.packetSender;
			if (packetSender != null)
			{
				requestCount++;
				packetSender.SendRequestAwakeningUpgrade(upgradeInfo);
			}
		}		
	}
	
	public override void OnBack ()
	{
		requestCount = 0;
		
		ResetSkill();
		
		base.OnBack ();
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
		
		ownGold -= (int)needGold;
				
		SetCoinInfo(ownGold, ownJewel, ownMedal, false);
	}
	
	public override void UpdateCoinInfo (bool increaseEffect)
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
		
		ownGold -= (int)needGold;
		
		SetCoinInfo(ownGold, ownJewel, ownMedal, increaseEffect);
	}
	
	public void UpdateResetButton()
	{
		Vector2 result = Vector2.zero;
		if (awakeningLevelManager != null)
			result = awakeningLevelManager.GetApplyPoint();
		
		if (resetSkillButton != null)
			resetSkillButton.isEnabled = result.y > 0.0f;
		if (saveSkillButton != null)
			saveSkillButton.isEnabled = result.y > 0.0f;
		
		if (initSkillButton != null)
			initSkillButton.isEnabled = result.x > 0.0f;
	}
	
	public void OnResultApply(SkillUpgradeDBInfo info, int gainPoint, int giftPoint, int buyLimitPoint, int buyPoint)
	{
		requestCount = 0;
		
		if (awakeningLevelManager != null)
		{
			awakeningLevelManager.skillPoint = gainPoint;
			awakeningLevelManager.giftPoint = giftPoint;
			awakeningLevelManager.buyPoint = buyPoint;
			awakeningLevelManager.canBuyPoint = buyLimitPoint;
			
			awakeningLevelManager.ApplyUpgradeInfo(info);
		}
		
		needGold = 0.0f;
		
		UpdateUI();
	}
	
	public void OnResultReset(NetErrorCode errorCode, int gainPoint, int giftPoint, int buyLimitPoint, int buyPoint)
	{
		ClosePopup();
		
		requestCount = 0;
		
		if (errorCode == NetErrorCode.OK)
		{
			if (awakeningLevelManager != null)
			{
				awakeningLevelManager.skillPoint = gainPoint;
				awakeningLevelManager.giftPoint = giftPoint;
				awakeningLevelManager.buyPoint = buyPoint;
				awakeningLevelManager.canBuyPoint = buyLimitPoint;
				
				awakeningLevelManager.InitSkills();
			}
			
			UpdateUI();
		}
		else
		{
			OnErrorMessage(errorCode, null);
		}
	}
	
	public void OnResultBuyPoint(NetErrorCode errorCode, int gainPoint, int giftPoint, int buyLimitPoint, int buyPoint)
	{
		requestCount = 0;
		
		if (errorCode == NetErrorCode.OK)
		{
			if (awakeningLevelManager != null)
			{
				awakeningLevelManager.skillPoint = gainPoint;
				awakeningLevelManager.giftPoint = giftPoint;
				awakeningLevelManager.buyPoint = buyPoint;
				awakeningLevelManager.canBuyPoint = buyLimitPoint;
			}
			
			UpdateUI();
		}
		else
		{
			OnErrorMessage(errorCode, null);
		}
	}
	
	public override void OnErrorMessage(NetErrorCode errorCode, PopupBaseWindow popupBase)
	{
		requestCount = 0;
		
		base.OnErrorMessage(errorCode, popupBase);
	}
}
