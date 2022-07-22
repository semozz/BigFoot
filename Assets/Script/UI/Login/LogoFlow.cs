using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class LogoInfo
{
	public UITexture logoTexture = null;
	public float delayTime = 1.5f;
}

public class LogoFlow : MonoBehaviour {
	public List<LogoInfo> logoInfos = new List<LogoInfo>();
	
	public UILabel messageLabel = null;
	
	// Use this for initialization
	void Start () {
		foreach(LogoInfo info in logoInfos)
		{
			if (info.logoTexture != null)
				info.logoTexture.gameObject.SetActive(false);
		}
		
		DoMessage("LogoFlow Stat.....");
		//Invoke("ClearMessage");
		
		NextLogo();
	}
	
	public float delayTime = 0.0f;
	// Update is called once per frame
	void Update () {
		delayTime -= Time.deltaTime;
		if (delayTime <= 0.0f)
			NextLogo();
	}
	
	public LogoInfo currentLogo = null;
	public void NextLogo()
	{
		int nCount = logoInfos.Count;
		
		SetActiveLogo(currentLogo, false);
		
		if (nCount > 0)
		{
			currentLogo = logoInfos[0];
			
			DoMessage("To Change Next Log Texture....");
			Invoke("ClearMessage", 0.2f);
			
			SetActiveLogo(currentLogo, true);
			
			logoInfos.RemoveAt(0);
		}
		else
		{
			DoMessage("Logo Change End... Go Login Stage...");
			LoadNextStage();
		}
	}
	
	public void SetActiveLogo(LogoInfo info, bool bActive)
	{
		if (info == null)
			return;
		
		if (info.logoTexture != null)
		{
			info.logoTexture.gameObject.SetActive(bActive);
			
			if (bActive == true)
				delayTime = info.delayTime;
		}
	}
	
	public string eulaStage = "";
	public string loginStage = "";
	public string townStage = "";
	
	private bool isLoadCall = false;
	public void LoadNextStage()
	{
		string stageName = eulaStage;
		LoginInfo loginInfo = Game.Instance.loginInfo;
		if (loginInfo != null)
		{
			if (loginInfo.eula_Checked == false)
				stageName = eulaStage;
			else
			{
				stageName = loginStage;
			}
		}
		
		if (isLoadCall == false)
		{
			Application.LoadLevel(stageName);
			isLoadCall = true;
		}
	}
	
	public void OnClick()
	{
		delayTime = 0.0f;
	}
	
	public void DoMessage(string msg)
	{
		if (this.messageLabel != null)
			this.messageLabel.text = msg;
	}
	
	public void ClearMessage()
	{
		DoMessage("");
	}
}
