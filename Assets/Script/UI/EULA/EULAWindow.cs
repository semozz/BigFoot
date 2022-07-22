using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EULAWindow : MonoBehaviour {

	public TextListWindow eulaArea = null;
	public TextListWindow privateArea = null;
	
	public List<UICheckbox> checks = new List<UICheckbox>();
	
	public void Start()
	{
		LoginInfo loginInfo = Game.Instance.loginInfo;
		
		if (loginInfo != null)
		{
			if (eulaArea != null)
				eulaArea.webText.resourceURL = loginInfo.eula_url;
			if (privateArea != null)
				privateArea.webText.resourceURL = loginInfo.private_url;
		}
		
		foreach(UICheckbox checkbox in checks)
		{
			checkbox.onStageChangeArg2 = new UICheckbox.OnStateChangeArg2(OnCheckButton);
		}
	}
	
	public void OnCheckButton(UICheckbox checkBox, bool state)
	{
		foreach(UICheckbox checkbox in checks)
		{
			if (checkbox.isChecked == false)
				return;
		}
		
		OnNextStage();
	}
	
	
	public string nextStageName = "Login_New";
	public void OnNextStage()
	{
		LoginInfo loginInfo = Game.Instance.loginInfo;
		if (loginInfo != null)
			loginInfo.eula_Checked = true;
		
		Game.Instance.SaveLoginData();
		
		Application.LoadLevel(nextStageName);
	}
}
