using UnityEngine;
using System.Collections;

public class SignupErrorPopup : MonoBehaviour {

	public UILabel messageLabel = null;
	
	public UIButtonMessage okButtonMessage = null;
	
	public void OnOK(GameObject obj)
	{
		DestroyObject(this.gameObject, 0.0f);
	}
	
	public void SetMessage(int stringID)
	{
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		if (messageLabel != null && stringTable != null && stringID != -1)
		{
			messageLabel.text = stringTable.GetData(stringID);
		}
	}
}
