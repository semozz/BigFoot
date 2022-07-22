using UnityEngine;
using System.Collections;

public class DuplicateConnectionPopup : NoticePopupWindow {

	override public void OnClose(GameObject obj)
	{
		DestroyObject(this.gameObject, 0.0f);
		
		if (Game.Instance.AndroidManager)
			Game.Instance.androidManager.CallUnityExit("DuplicateConnection");

		Application.Quit();
	}
}
