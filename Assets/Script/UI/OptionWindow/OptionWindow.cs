using UnityEngine;
using System.Collections;

public class OptionWindow : PopupBaseWindow {
	
	public int toggleOnStringID = 207;
	public int toggleOffStringID = 208;
	
	public UICheckbox bgmToggle = null;
	public UILabel bgmToggleLabel = null;
	
	public UICheckbox effectToggle = null;
	public UILabel effectToggleLabel = null;
	
	public UICheckbox noticeToggle = null;
	public UILabel noticeToggleLabel = null;
	
	public UICheckbox faceToggle = null;
	public UILabel faceToggleLabel = null;
	
	public UIButton warriorHelpButton = null;
	public UIButton assassinHelpButton = null;
	public UIButton wizardHelpButton = null;
	
	public UIButton gotoCafe1 = null;
	public int cafe1URLStringID = 209;
	
	public UIButton gotoCafe2 = null;
	public int cafe2URLStringID = 210;
	
	public UIButton gotoMobilCenter = null;
	public int mobilCenterURLStringID = 211;
	
	public UIButton inputCoupon = null;
	
	public UIButton logoutButton = null;
	public UIButton memberSecession = null;
	
	public UILabel nickNameLabel = null;
	public UILabel idNumberLabel = null;
	public UILabel gameVersionLabel = null;
	
	public int memberInfoStringID = 288;
	
	public GameObject otherNoticeRoot = null;
	public GameObject kakaoNoticeRoot = null;
	
	public UIButton kakaoNoticeButton = null;
	public UILabel kakaoNoticeButtonLabel = null;
	
	private StringTable stringTable = null;
	void Start()
	{
		TableManager tableManager = TableManager.Instance;
		if (tableManager != null)
			stringTable = tableManager.stringTable;
		
		LoginInfo loginInfo = Game.Instance.loginInfo;
		bool isKakaoMode = true;
		if (loginInfo != null)
		{
			if (loginInfo.acctountType == AccountType.Kakao)
				isKakaoMode = true;
		}
		
		Logger.DebugLog(string.Format("OptionWindow Start : account_type : {0}", loginInfo != null ? loginInfo.acctountType.ToString() : "null"));
		
		if (otherNoticeRoot != null)
			otherNoticeRoot.SetActive(!isKakaoMode);
		if (kakaoNoticeRoot != null)
			kakaoNoticeRoot.SetActive(isKakaoMode);
		
		if (kakaoNoticeButtonLabel != null && stringTable != null)
			kakaoNoticeButtonLabel.text = stringTable.GetData(kakaoNiticeStringID);
		
		if (bgmToggle != null)
			bgmToggle.onStageChangeArg2 = new UICheckbox.OnStateChangeArg2(OnBGMToggle);
		if (effectToggle != null)
			effectToggle.onStageChangeArg2 = new UICheckbox.OnStateChangeArg2(OnEffectToggle);
		if (noticeToggle != null)
			noticeToggle.onStageChangeArg2 = new UICheckbox.OnStateChangeArg2(OnNoticeToggle);
		if (faceToggle != null)
			faceToggle.onStageChangeArg2 = new UICheckbox.OnStateChangeArg2(OnFaceToggle);
		
		
		if (bgmToggle != null)
			bgmToggle.isChecked = GameOption.bgmToggle;
		if (effectToggle != null)
			effectToggle.isChecked = GameOption.effectToggle;
		if (noticeToggle != null)
			noticeToggle.isChecked = GameOption.noticeToggle;
		if (faceToggle != null)
			faceToggle.isChecked = GameOption.faceToggle;
		
		UpdateKakaoNotice();
		UpdateInfo();
		
		GameUI.Instance.optionWindow = this;
	}
	
	void OnDestroy()
	{
		GameUI.Instance.optionWindow = null;
	}
	
	public void InitWindow()
	{
		
	}
	
	public override void OnBack()
	{
		base.OnBack();
		DestroyObject(this.gameObject, 0.1f);
	}
	
	public void UpdateInfo()
	{
		if (nickNameLabel != null)
		{
			string nickName = "";
			if (Game.Instance.connector != null)
				nickName = Game.Instance.connector.Nick;
			
			nickNameLabel.text = string.Format("NickName : {0}", nickName);
		}
		
		if (idNumberLabel != null)
		{
			string prefixString = "Kakao ID Number";
			string kakaoUserID = "";
			if (stringTable != null)
				prefixString = stringTable.GetData(memberInfoStringID);
			
			if (Game.Instance.loginInfo != null)
				kakaoUserID = Game.Instance.loginInfo.loginID;
			
			idNumberLabel.text = string.Format("{0} : {1}", prefixString, kakaoUserID);
		}
		
		if (gameVersionLabel != null)
		{
			gameVersionLabel.text = string.Format("Version : Ver.{0}.{1}.{2}", Version.Major, Version.NetVersion, Version.assetServerRevision);
		}
	}
	
	public void OnBGMToggle(UICheckbox checkBox, bool bCheck)
	{
		bool bToggle = bCheck;
		string toggleString = "";
		if (bToggle == true)
			toggleString = stringTable != null ? stringTable.GetData(toggleOnStringID) : "On";
		else
			toggleString = stringTable != null ? stringTable.GetData(toggleOffStringID) : "Off";
		
		if (bgmToggleLabel != null)
			bgmToggleLabel.text = toggleString;
		
		Game.Instance.BGMToggle(bToggle);
		Game.Instance.SaveGameOption();
	}
	
	public void OnEffectToggle(UICheckbox checkBox, bool bCheck)
	{
		bool bToggle = bCheck;
		string toggleString = "";
		if (bToggle == true)
			toggleString = stringTable != null ? stringTable.GetData(toggleOnStringID) : "On";
		else
			toggleString = stringTable != null ? stringTable.GetData(toggleOffStringID) : "Off";
		
		if (effectToggleLabel != null)
			effectToggleLabel.text = toggleString;
		
		Game.Instance.EffectSoundToggle(bToggle);
		Game.Instance.SaveGameOption();
	}
	
	public void OnNoticeToggle(UICheckbox checkBox, bool bCheck)
	{
		bool bToggle = bCheck;
		string toggleString = "";
		if (bToggle == true)
			toggleString = stringTable != null ? stringTable.GetData(toggleOnStringID) : "On";
		else
			toggleString = stringTable != null ? stringTable.GetData(toggleOffStringID) : "Off";
		
		if (noticeToggleLabel != null)
			noticeToggleLabel.text = toggleString;
		
		Game.Instance.NoticeToggle(bToggle);
		Game.Instance.SaveGameOption();
		
		IPacketSender sender = Game.Instance.PacketSender;
		if (sender != null)
			sender.SendIgnorePush(bToggle);
	}
	
	public void OnKakaoNoticeChange(GameObject obj)
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		if (Game.Instance.AndroidManager)
			Game.Instance.AndroidManager.CallKakaoMessageBlock();
#endif		
	}
	
	
	public int kakaoNoticeOnStringID = 290;
	public int kakaoNoticeOffStringID = 291;
	public int kakaoNiticeStringID = 292;
	public void UpdateKakaoNotice()
	{
		string toggleString = "";
		if (GameOption.noticeToggle == true)
			toggleString = stringTable != null ? stringTable.GetData(kakaoNoticeOnStringID) : "On";
		else
			toggleString = stringTable != null ? stringTable.GetData(kakaoNoticeOffStringID) : "Off";
		
		if (noticeToggleLabel != null)
			noticeToggleLabel.text = toggleString;
	}
	
	public void OnFaceToggle(UICheckbox checkBox, bool bCheck)
	{
		bool bToggle = bCheck;
		string toggleString = "";
		if (bToggle == true)
			toggleString = stringTable != null ? stringTable.GetData(toggleOnStringID) : "On";
		else
			toggleString = stringTable != null ? stringTable.GetData(toggleOffStringID) : "Off";
		
		if (faceToggleLabel != null)
			faceToggleLabel.text = toggleString;
		
		Game.Instance.FaceToggle(bToggle);
		Game.Instance.SaveGameOption();
		
		IPacketSender sender = Game.Instance.PacketSender;
		if (sender != null)
			sender.SendProfileImageOnOff(bToggle);
	}
	
	
	public string warriorHelpPrefab = "Option_Tutorial_Mulaco_popup";
	public string assassinHelpPrefab = "Option_Tutorial_Kaya_popup";
	public string wizardHelpPrefab = "Option_Tutorial_Papalu_popup";
	
	
	public OptionHelpPopup warriorHelpPopup = null;
	public void OnWarriorHelp(GameObject obj)
	{
		if (warriorHelpPopup == null)
		{
			string prefabPath = string.Format("UI/Option/{0}", warriorHelpPrefab);
			warriorHelpPopup = ResourceManager.CreatePrefab<OptionHelpPopup>(prefabPath, popupNode);
			
			if (warriorHelpPopup.closeButtonMessage != null)
			{
				warriorHelpPopup.closeButtonMessage.target = this.gameObject;
				warriorHelpPopup.closeButtonMessage.functionName = "CloseWarriorHelp";
			}
		}
	}
	
	public void CloseWarriorHelp(GameObject obj)
	{
		if (warriorHelpPopup != null)
		{
			DestroyObject(warriorHelpPopup.gameObject, 0.2f);
			warriorHelpPopup = null;
		}
	}
	
	public OptionHelpPopup assassinHelpPopup = null;
	public void OnAssassingHelp(GameObject obj)
	{
		if (assassinHelpPopup == null)
		{
			string prefabPath = string.Format("UI/Option/{0}", assassinHelpPrefab);
			assassinHelpPopup = ResourceManager.CreatePrefab<OptionHelpPopup>(prefabPath, popupNode);
			
			if (assassinHelpPopup.closeButtonMessage != null)
			{
				assassinHelpPopup.closeButtonMessage.target = this.gameObject;
				assassinHelpPopup.closeButtonMessage.functionName = "CloseAssassinrHelp";
			}
		}
	}
	
	public void CloseAssassinrHelp(GameObject obj)
	{
		if (assassinHelpPopup != null)
		{
			DestroyObject(assassinHelpPopup.gameObject, 0.2f);
			assassinHelpPopup = null;
		}
	}
	
	public OptionHelpPopup wizardHelpPopup = null;
	public void OnWizardHelp(GameObject obj)
	{
		if (wizardHelpPopup == null)
		{
			string prefabPath = string.Format("UI/Option/{0}", wizardHelpPrefab);
			wizardHelpPopup = ResourceManager.CreatePrefab<OptionHelpPopup>(prefabPath, popupNode);
			
			if (wizardHelpPopup.closeButtonMessage != null)
			{
				wizardHelpPopup.closeButtonMessage.target = this.gameObject;
				wizardHelpPopup.closeButtonMessage.functionName = "CloseWizardrHelp";
			}
		}
	}
	
	public void CloseWizardrHelp(GameObject obj)
	{
		if (wizardHelpPopup != null)
		{
			DestroyObject(wizardHelpPopup.gameObject, 0.2f);
			wizardHelpPopup = null;
		}
	}
	
	public void OnGotoCafe1(GameObject obj)
	{
		string gotoURL = stringTable != null ? stringTable.GetData(cafe1URLStringID) : "";
		if (gotoURL != "")
			Application.OpenURL(gotoURL);
	}
	
	public void OnGotoCafe2(GameObject obj)
	{
		string gotoURL = stringTable != null ? stringTable.GetData(cafe2URLStringID) : "";
		if (gotoURL != "")
			Application.OpenURL(gotoURL);
	}
	
	public void OnGotoMobileCenter(GameObject obj)
	{
		string gotoURL = stringTable != null ? stringTable.GetData(mobilCenterURLStringID) : "";
		if (gotoURL != "")
			Application.OpenURL(gotoURL);
	}
	
	public string couponWindowPrefab = "UI/Option/Coupon_popup";
	public void OnInputCoupon(GameObject obj)
	{
		CouponWindow couponWindow = ResourceManager.CreatePrefab<CouponWindow>(couponWindowPrefab, popupNode, Vector3.zero);
		if (couponWindow != null)
			couponWindow.InitWindow();
	}
	
	public string accountPopupPrefab = "UI/Option/AccountPopup";
	public int logoOutTitleID = 212;
	public int logoOutMessageID = 213;
	public void OnLogout(GameObject obj)
	{
		BaseConfirmPopup popup = ResourceManager.CreatePrefab<BaseConfirmPopup>(accountPopupPrefab, popupNode, Vector3.zero);
		if (popup != null)
		{
			popupList.Add(popup);
			
			popup.SetMessage(logoOutTitleID, logoOutMessageID);
			
			if (popup.okButtonMessage != null)
			{
				popup.okButtonMessage.target = this.gameObject;
				popup.okButtonMessage.functionName = "OnLogoutOK";
			}
			
			if (popup.cancelButtonMessage != null)
			{
				popup.cancelButtonMessage.target = this.gameObject;
				popup.cancelButtonMessage.functionName = "OnClosePopup";
			}
		}
	}
	
	public override void ClosePopup()
	{
		base.ClosePopup();
		
		if (accountInputPopup != null)
		{
			DestroyObject(accountInputPopup.gameObject, 0.2f);
			accountInputPopup = null;
		}
	}
	
	public void OnLogoutOK(GameObject obj)
	{
		OptionWindow.OnLogoutConfirmed();
	}
	
	public static void OnLogoutConfirmed()
	{
		Game.Instance.ResetLoginInfo(true);
		Game.Instance.ResetInitData();
		
		Game.Instance.AndroidManager.OnClickGoogleLogout();
		
		Application.LoadLevel(0);
	}
	
	public int memberSecessionTitleID = 214;
	public int memberSecessionMessageID = 215;
	public void OnMemberSecession(GameObject obj)
	{
		LoginInfo loginInfo = Game.Instance.loginInfo;
		bool needPassword = false;
		if (loginInfo != null)
		{
			switch(loginInfo.acctountType)
			{
			case AccountType.GooglePlus:
			case AccountType.Kakao:
				Debug.Log("KaKao or Google.. ..");
				needPassword = false;
				break;
			default:
				Debug.Log("need Password");
				break;
			}
		}
		
		BaseConfirmPopup popup = ResourceManager.CreatePrefab<BaseConfirmPopup>(accountPopupPrefab, popupNode, Vector3.zero);
		if (popup != null)
		{
			popupList.Add(popup);
			
			popup.SetMessage(memberSecessionTitleID, memberSecessionMessageID);
			
			if (popup.okButtonMessage != null)
			{
				popup.okButtonMessage.target = this.gameObject;
				if (needPassword == true)
					popup.okButtonMessage.functionName = "OnMemberSecessionAcountInput";
				else
					popup.okButtonMessage.functionName = "OnMemberSecessionNoneAcountInput";
			}
			
			if (popup.cancelButtonMessage != null)
			{
				popup.cancelButtonMessage.target = this.gameObject;
				popup.cancelButtonMessage.functionName = "OnClosePopup";
			}
		}
	}
	
	public void OnMemberSecessionNoneAcountInput()
	{
		string userid = "";
		AccountType type = AccountType.Kakao;
		LoginInfo loginInfo = Game.Instance.loginInfo;
		
		if (loginInfo != null)
		{
			userid = loginInfo.loginID;
			type = loginInfo.acctountType;
		}
		
		OnMemberSecessionOK(userid, "", type);
	}
	
	public string accountInputPopupPrefab = "UI/Option/AccountInputPopup";
	AccountInputPopup accountInputPopup = null;
	public void OnMemberSecessionAcountInput()
	{
		ClosePopup();
		
		if (accountInputPopup == null)
			accountInputPopup = ResourceManager.CreatePrefab<AccountInputPopup>(accountInputPopupPrefab, popupNode, Vector3.zero);
		
		if (accountInputPopup != null)
		{
			LoginInfo loginInfo = null;
			if (Game.Instance != null)
				loginInfo = Game.Instance.loginInfo;
			
			string accountStr = "";
			string passStr = "";
			AccountType accountType = AccountType.MonsterSide;
			if (loginInfo != null)
			{
				accountStr = loginInfo.loginID;
				accountType = loginInfo.acctountType;
			}
			
			accountInputPopup.SetAccountInfo(accountStr, passStr, accountType);
			
			if (accountInputPopup.okButtonMessage != null)
			{
				accountInputPopup.okButtonMessage.target = this.gameObject;
				accountInputPopup.okButtonMessage.functionName = "OnMemberSecessionInputOK";
			}
			
			if (accountInputPopup.cancelButtonMessage != null)
			{
				accountInputPopup.cancelButtonMessage.target = this.gameObject;
				accountInputPopup.cancelButtonMessage.functionName = "OnClosePopup";
			}
		}
	}
	
	public void OnMemberSecessionInputOK()
	{
		if (accountInputPopup != null)
		{
			string accountStr = "";
			string passStr = "";
			AccountType accountType = AccountType.MonsterSide;
			
			accountInputPopup.GetAccountInfo(out accountStr, out passStr, out accountType);
			
			if (passStr.Length == 0)
			{
				accountInputPopup.OnPassError();
			}
			else
			{
				OnMemberSecessionOK(accountStr, passStr, accountType);
			}
		}
	}
	
	public void OnMemberSecessionOK(string account, string pass, AccountType type)
	{
		IPacketSender sender = Game.Instance.PacketSender;
		if (sender != null)
			sender.SendRequestMemberSecession(account, pass, type);
		//test용..
		OnLogoutOK(null);
	}
}
