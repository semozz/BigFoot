using UnityEngine;
using System.Collections;

public class ServerCheckPopup : NoticePopupWindow {

	override public void OnClose(GameObject obj)
	{
		DestroyObject(this.gameObject, 0.0f);
		
		if (Game.Instance.AndroidManager)
			Game.Instance.androidManager.CallUnityExit("serverChecking");
		
		Application.Quit();
	}
}
