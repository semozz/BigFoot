using UnityEngine;
using System.Collections;

public class NeedUpdatePopup : NoticePopupWindow {

	private string updateURL = "";
	public string UpdateURL
	{
		set { updateURL = value; }
		get { return updateURL; }
	}
	
	override public void OnClose(GameObject obj)
	{
		DestroyObject(this.gameObject, 0.0f);
		
		if (Game.Instance.AndroidManager)
			Game.Instance.androidManager.CallUntiyUpdateSoftware("");
	}
}
