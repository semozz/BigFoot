using Hive5;
using Hive5.Model;
using Hive5.Util;

using UnityEngine;
using System.Collections;

public class FaceBookInfo
{
	public string id;	
	public string name;
	public string email;
}

public class LoginPage : MonoBehaviour 
{
	public bool isLoginDone = false;	
	public UIInput idInput = null;
	public BoxCollider idInputCollider = null;
	public UIInput passInput = null;
	public BoxCollider passInputCollider = null;
	
	public UIButton loginButton = null;
	public UIButton signUpButton = null;
	public UIButton googleButton = null;
	public UIButton facebookButton = null;
	public UIButton kakaoButton = null;
	public UIButton guestButton = null;
	
	public UILabel errorMsg = null;
	
	public Transform popupNode = null;
	
	public GameObject signUpPanel = null;
	public GameObject loginPanel = null;
	
	public SignUpController signUpController = null;
	
	void Awake()
	{
		GameUI.Instance.loginPage = this;
		
		ClientConnector connector = Game.Instance.Connector;
		
		if (errorMsg != null)
			errorMsg.gameObject.SetActive(false);		
		
		#if UNITY_ANDROID && !UNITY_EDITOR
		 //Game.Instance.networkManager.androidManager.FacebookInitial();
		#endif			
	}
	
	void Start()
	{				
		if (loginButton != null)
			loginButton.isEnabled = false;
		
		if (signUpButton != null)
			signUpButton.isEnabled = false;
		if (googleButton != null)
			googleButton.isEnabled = false;
		if (facebookButton != null)
			facebookButton.isEnabled = false;
		
		if (idInputCollider != null)
			idInputCollider.enabled = false;
		if (passInputCollider != null)
			passInputCollider.enabled = false;
		
		if (kakaoButton != null)
			kakaoButton.isEnabled = false;
		if (guestButton != null)
			guestButton.isEnabled = false;
		
#if UNITY_ANDROID && !UNITY_EDITOR
		if (Game.Instance.AndroidManager)
			Game.Instance.AndroidManager.CallLocalUser();
#else
		OnPreLoinOK();
#endif
	}
	
	void OnDestroy()
	{
		GameUI.Instance.loginPage = null;
	}
	
	public void OnPreLoinOK()
	{
		LoginInfo loginInfo = Game.Instance.loginInfo;
		if (loginInfo != null)
		{
			string id = loginInfo.loginID;
			string pass = loginInfo.pass;
			
			Debug.Log ("Prelogin id:"+id);
			Debug.Log ("Prelogin accounttype:"+loginInfo.acctountType);
			
			//if (!string.IsNullOrEmpty(id))
			{
				bool bLogin = false;
				switch(loginInfo.acctountType)
				{
				case AccountType.MonsterSide:
					if (idInput != null)
						idInput.text = id;
					
					if (!string.IsNullOrEmpty(pass))
					{
						if (passInput != null)
							passInput.text = "****";
					}
					
					if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(pass))
					{
						OnLogin(id, pass, AccountType.MonsterSide);
						bLogin = true;
					}
					break;
				case AccountType.GooglePlus:
					if (!string.IsNullOrEmpty(id))
					{
						OnClickGooglePlusSigninByEmail(id);
						bLogin = true;
					}
					break;
				case AccountType.Kakao:
					if (!string.IsNullOrEmpty(id))
					{
						OnLoginKakao(id);
						bLogin = true;
					}
					break;
				}
				
				if (bLogin == true)
					return;
			}
		}
		
		if (loginButton != null)
			loginButton.isEnabled = true;
		
		if (signUpButton != null)
			signUpButton.isEnabled = true;
		if (googleButton != null)
			googleButton.isEnabled = true;
		if (facebookButton != null)
			facebookButton.isEnabled = true;
		
		if (idInputCollider != null)
			idInputCollider.enabled = true;
		if (passInputCollider != null)
			passInputCollider.enabled = true;
		
		if (kakaoButton != null)
			kakaoButton.isEnabled = true;
		if (guestButton != null)
			guestButton.isEnabled = true;
	}
	
	public void OnLogin()
	{
		string id = "";
		string pass = "";
		if (idInput != null)
			id = idInput.text;
		if (passInput != null)
			pass = passInput.text;
		
		if (id == "" || pass == "")
		{
			if (errorMsg != null)
			{
				errorMsg.gameObject.SetActive(true);
				errorMsg.text = "ID or Password input....!!!";
			}
			return;
		}
		
		OnLogin(id, pass, AccountType.MonsterSide);
	}
	
	public void OnLogin(string id, string pass, AccountType accountType)
	{
		loginID = id;
		loginPass = pass;
		loginAccountType = accountType;
		
		Game.Instance.packetSender.SendLogin(id, pass, accountType);
		
		if (loginButton != null)
			loginButton.isEnabled = false;
	}
	
	private string loginID = "";
	private string loginPass = "";
	private AccountType loginAccountType = AccountType.MonsterSide;
	
	public void OnLoginError(int errorCode)
	{
		bool idReset = true;
		bool passReset = true;
		
		Game.Instance.loginInfo = null;
		Game.Instance.SaveLoginData();
		
		if (idReset == true && idInput != null)
			idInput.text = "";
		if (passReset == true && passInput != null)
			passInput.text ="";
		
		if (loginButton != null)
			loginButton.isEnabled = true;
		
		if (signUpButton != null)
			signUpButton.isEnabled = true;
		if (googleButton != null)
			googleButton.isEnabled = true;
		if (facebookButton != null)
			facebookButton.isEnabled = true;
		
		if (idInputCollider != null)
			idInputCollider.enabled = true;
		if (passInputCollider != null)
			passInputCollider.enabled = true;
		
		if (kakaoButton != null)
			kakaoButton.isEnabled = true;
		if (guestButton != null)
			guestButton.isEnabled = true;
	}
	
	public string nextScene = "SelectCharacter";
	public void OnLoginOK()
	{
		Debug.Log("LoginPage :: OnLoginOK");
		
		LoginInfo loginInfo = Game.Instance.loginInfo;
		if (loginInfo != null && !string.IsNullOrEmpty(loginID))
		{
			loginInfo.loginID = loginID;
			loginInfo.pass = loginPass;
			loginInfo.acctountType = loginAccountType;
			
			loginInfo.loginDate = System.DateTime.Now;
		}
		
		Game.Instance.SaveLoginData();
		
		Application.LoadLevelAsync(nextScene);
	}
	
	public void OnClickGooglePlusSignin()
	{
		Debug.Log("OnClickGooglePlusSignin");
#if UNITY_ANDROID && !UNITY_EDITOR
		if (Game.Instance.AndroidManager)
			Game.Instance.AndroidManager.OnClickGooglePlusSignin();
#endif
	}
	
	public void OnClickGooglePlusSigninByEmail(string email)
	{
		Debug.Log("OnClickGooglePlusSigninByEmail:" + email);
#if UNITY_ANDROID && !UNITY_EDITOR
		if (Game.Instance.AndroidManager)
			Game.Instance.AndroidManager.SendGoogleLogin(email);
#endif
	}
	
	public void OnClickFacebook()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		if (Game.Instance.AndroidManager)
			Game.Instance.AndroidManager.OnClickFacebookLogin();
#endif
	}
	
	public void OnClickFacebookLogOut()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		if (Game.Instance.AndroidManager)
			Game.Instance.AndroidManager.OnClickFacebookLogout();
#endif
	}
	
	public string createNickNamePrefab = "UI/Login/CreateNickName";
	CreateNickNameWindow createNickNameWindow = null;
	public void OnCreateNickName()
	{
		if (signUpController != null)
			signUpController.CloseSignUp();
		
		if (createNickNameWindow == null)
			createNickNameWindow =ResourceManager.CreatePrefab<CreateNickNameWindow>(createNickNamePrefab, popupNode, Vector3.zero);
	}
	
	public void OnCheckNickResult(NetErrorCode errorCode)
	{
		if (createNickNameWindow != null)
			createNickNameWindow.OnNickNameCheck(errorCode);
	}
	
	public void OnCreateNickResult(NetErrorCode errorCode)
	{
		if (createNickNameWindow != null)
			createNickNameWindow.OnNickNameCreate(errorCode);
	}
	
	
	public void OnClickLoginGuest()
	{
		
	}
	
	public void OnClickLoginKakao()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		if (Game.Instance.AndroidManager)
			Game.Instance.AndroidManager.OnKakaoLogin();
		
		if (kakaoButton != null)
			kakaoButton.isEnabled = false;
		
		if (guestButton != null)
			guestButton.isEnabled = false;
#endif	
	}
	
	public void OnLoginKakao(string kakaoUserID)
	{
		Game.Instance.PacketSender.SendKakaoLogin(kakaoUserID);		
	}
}
