using UnityEngine;
using System.Collections;

public class RestartPopup : NoticePopupWindow
{

    override public void OnClose(GameObject obj)
    {
        DestroyObject(this.gameObject, 0.0f);

        //Game.Instance.ResetLoginInfo(false);

        Resources.UnloadUnusedAssets();
        Application.LoadLevelAsync(0);
    }
	
	public int mobilCenterURLStringID = 211;
	public void OnGotoMobileCenter(GameObject obj)
	{
		TableManager tableManager = TableManager.Instance;
		if (tableManager != null)
		{
			string gotoURL = tableManager.stringTable != null ? tableManager.stringTable.GetData(mobilCenterURLStringID) : "";
			if (gotoURL != "")
				Application.OpenURL(gotoURL);
		}
		else
			OnClose(obj);
	}
}
