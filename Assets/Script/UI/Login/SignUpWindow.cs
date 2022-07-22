using UnityEngine;
using System.Collections;

public class SignUpWindow : MonoBehaviour {
	public UILabel idLabel = null;
	public UILabel passwordLabel = null;
	public UILabel passConfirmLabel = null;
	
	public UIButton signupButton = null;
	
	public Transform popupNode = null;
	
	void Start()
	{
		GameUI.Instance.signupWindow = this;
	}
	
	void OnDestroy()
	{
		GameUI.Instance.signupWindow = null;
	}
	
	void OnClose(GameObject obj)
	{
		DestroyObject(this.gameObject, 0.0f);
	}
	
	public void OnRequestSignUp(GameObject obj)
	{
		string id = idLabel.text;
		string pass1 = passwordLabel.text;
		string pass2 = passConfirmLabel.text;
		
		bool checkID = CheckInputID(id);
		if (checkID == false)
		{
			OnErrorSignUp((int)NetErrorCode.AccountIDNotEmail);
			return;
		}
		
		bool passCheck = CheckPassword(pass1, pass2);
		if (passCheck == false)
		{
			OnErrorSignUp((int)NetErrorCode.LoginPassword_Wrong);
			return;
		}
		
		IPacketSender sender = Game.Instance.PacketSender;
		if (sender != null)
			sender.RequestSignup(id, pass1, pass2);
		
		//OnErrorSignUp(Random.Range(100, 110));
	}
	
	public string errorPopupPrefab = "";

    public void OnErrorSignUp(NetErrorCode errorCode)
    {
        OnErrorSignUp((int)errorCode);
    }

	public void OnErrorSignUp(int errorID)
	{
		SignupErrorPopup errorPopup =ResourceManager.CreatePrefab<SignupErrorPopup>(errorPopupPrefab, popupNode, Vector3.zero);
		if (errorPopup != null)
			errorPopup.SetMessage(errorID);
	}
	
	public bool CheckPassword(string pass1, string pass2)
	{
		if (pass1 != pass2)
			return false;
		
		return true;
	}
	
	public bool CheckInputID(string id)
	{
		if (id == "")
			return false;
		
		string[] splitStr = id.Split('@');
		if (splitStr.Length != 2)
			return false;
		
		string prefixStr = splitStr[0];
		if (prefixStr == "")
			return false;
		
		string postfixStr = splitStr[1];
		if (postfixStr == "")
			return false;
		
		string[] postSplit = postfixStr.Split('.');
		if (postSplit.Length != 2)
			return false;
		
		string post1Str = postSplit[0];
		string post2Str = postSplit[1];
		
		if (post1Str == "" || post2Str == "")
			return false;
		
		return true;
	}
}
