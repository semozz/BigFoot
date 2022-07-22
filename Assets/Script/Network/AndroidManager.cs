using UnityEngine;
using System.Collections;

public enum AlertDialogType
{
	KeyDownClose = 0, 
	SessionExpired, 
	HackDetected , 
	NetworkError
}

public class AndroidManager : MonoBehaviour
{
	public string FacebookAppID = "270753093116693";
	public string KeyHashes = "rZy1YF/aXemIj8mCl5nBSIgxfzg=";

#if UNITY_ANDROID && !UNITY_EDITOR
	private AndroidJavaObject curActivity;

	void Awake()
	{
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		curActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
		
	}
#endif	
	
	void AndroidCall(string functionName, string arg)
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		if (curActivity == null)
		{
			Debug.Log ("AndroidManager curActivity NULL");
			return;
		}
		
		curActivity.Call(functionName, arg);
		#endif
	}
	
	public void SendGoogleLogin(string email)
	{
		Debug.Log ("SendGoogleLogin");
		#if UNITY_ANDROID && !UNITY_EDITOR
		
		AndroidCall ("SendGooglePlusSigninWithEmail", email);
		#endif 
	}

	public void OnClickKakaoLink(string Nick)
	{
		Debug.Log ("OnClickKakaoLink");
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidCall ("SendkakaoLinkMessage", Nick);	
		#endif			
	}
	
	public void OnClickGooglePlusSignin()
	{
		Debug.Log ("OnClickGooglePlusSignin");
		#if UNITY_ANDROID && !UNITY_EDITOR
		string arg = "";
		
		AndroidCall ("SendGooglePlusSignin", arg);
		#endif			
	}
	
	public void OnClickGoogleLogout()
	{
		Debug.Log ("OnClickGoogleLogout");
		#if UNITY_ANDROID && !UNITY_EDITOR
		string arg = "";
		
		AndroidCall ("SendGoogleRevokeAccess", arg);
		
		AndroidCall("OnKakaoLogout", arg);
		#endif			
	}
	
	public void OnClickFacebookLogin()
	{
		Debug.Log ("OnClickFacebookLogin");
		#if UNITY_ANDROID && !UNITY_EDITOR
		string arg = "";
		
		AndroidCall ("SendFacebookLogin", arg);
		#endif			
	}
	
	public void OnClickFacebookLogout()
	{
		Debug.Log ("OnClickFacebookLogout");
		#if UNITY_ANDROID && !UNITY_EDITOR
		string arg = "";
		AndroidCall ("SendFacebookLogout", arg);
		#endif			
	}

	public void FacebookInitial()
	{
		Debug.Log ("FacebookInitial");
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidCall ("SendInitFacebook", FacebookAppID);
		#endif			
	}

	
	public void OnClickBuyCashItem(TStoreCashItemInfo info)
	{
		Debug.Log ("OnClickBuyCashItem");
		string jsonInfo = LitJson.JsonMapper.ToJson(info);
		
#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidCall("CallUnityBuyCashItem", jsonInfo);
#else
		Game.Instance.InvokeTStorePaymentSucceed(jsonInfo);
#endif
	}
	
	public void CallUnityExit(string reason)
	{
		Debug.Log ("CallUnityExit");
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidCall("CallUnityExit", reason);
		#endif			
	}
	
	
	public void CallReadyGoogleRegID(string userindexid)
	{
		Debug.Log("CallReadyGoogleRegID");
		#if UNITY_ANDROID && !UNITY_EDITOR
		string arg = "";
		
		AndroidCall("CallReadyGoogleRegID", userindexid);
		#endif			
	}
		
	// 종료 윈도우가 뜬다.
	public void CallUnityExitWindow(AlertDialogType type)
	{
		Debug.Log("CallUnityExitWindow by:" + type);
		
		int intType = (int)type;
		
		#if UNITY_ANDROID && !UNITY_EDITOR
		//string jsonInfo = LitJson.JsonMapper.ToJson(info);		
		
		AndroidCall("CallUnityExitWindow", intType.ToString());
		#endif			
	}
	
	// Networkmanager가 생성되엿음. cookie값을 보내라.
	public void CallUntiyNetworkReady()
	{
		Debug.Log("CallUntiyNetworkReady");
		
		#if UNITY_ANDROID && !UNITY_EDITOR
		string arg = "";
			
		AndroidCall("CallUntiyNetworkReady", arg);
		#else
		// 윈도우는 보내줄수 없으니 온것처럼.
		Game.Instance.InvokeXignCode("cokietest;publisherType");
		#endif	
	}
	
	public void CallUnityAchievement(int GroupID, int addCount)
	{
		Debug.Log("CallUnityAchievement " + GroupID.ToString());
		
		AndroidAchievement info = new AndroidAchievement();
		info.groupID = GroupID;
		info.addCount = addCount;
		
		#if UNITY_ANDROID && !UNITY_EDITOR
		string arg = LitJson.JsonMapper.ToJson(info);
		AndroidCall("CallUnityAchievement", arg);
		#endif			
	}
	
	public void CallUnityReview(string args)
	{
		Debug.Log("CallUnityReview ");
		
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidCall("CallUnityReview", args);
		#endif	
	}
	
	public void CallUntiyUpdateSoftware(string args)
	{
		Debug.Log("CallUntiyUpdateSoftware ");
		
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidCall("CallUntiyUpdateSoftware", args);
		#endif
	}
	
	public void OnKakaoLogin()
	{
		Debug.Log("Kakao Login First...");
		
#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidCall ("OnKakaoLogin", "");
#endif
	}
	
	public void OnKakaoLogout()
	{
		Debug.Log("Kakao LogOut...");
#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidCall ("OnKakaoLogout", "");
#endif
		
	}
	
	public void OnCheckKakaoLogin()
	{
		Debug.Log("Check Kakao Login...");
		
#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidCall ("OnCheckKakaoLogin", "");
#endif	
	}
	
	public void CallLocalUser()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidCall ("OnLocalUser", "");
#endif
	}
	
	public void CallUnityAwake()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidCall ("OnGameAwake", "");
#endif
	}
	
	public void CallUnRegisterKakao()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidCall ("OnKakaoUnregist", "");
#endif
		
	}
	
	public void SendKakaoMessage(string userID)
	{
#if UNITY_EDITOR
		Game.Instance.OnKakaoInviteComplete(userID);
#elif UNITY_ANDROID
		AndroidCall ("OnSendKakaoMessage", userID);
#endif
		
	}
	
	public void CallUnityUpdateKakaoFriends()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidCall ("OnUpdateKakaoFriends", "");
#endif
	}
	
	public void CallKakaoMessageBlock()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidCall ("OnMessageBlockDialog", "");
#endif
	}
	
	public void RequestSettingInfos()
	{
#if UNITY_EDITOR
		Game.Instance.OnSetHive5AppKey(Hive5.Hive5Config.AppKey);
#elif UNITY_ANDROID
		AndroidCall ("OnRequestSettingInfos", "");
#endif
		
	}
	
	public void SendPartyTrackEvent(string eventName)
	{
#if !UNITY_EDITOR && UNITY_ANDROID
		AndroidCall ("OnPartyTrackEvent", eventName);
#endif
	}

}