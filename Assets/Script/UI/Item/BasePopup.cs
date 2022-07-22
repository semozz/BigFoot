using UnityEngine;
using System.Collections;

public class BasePopup : MonoBehaviour {
	public UILabel titleLabel = null;
	public UILabel messageLabel = null;
	public int titleStringID = -1;
	public int messageStringID = -1;
	
	public UILabel cancelButtonLabel = null;
	public UILabel okButtonLabel = null;
	
	public int cancelButtonStringID = -1;
	public int okButtonStringID = -1;
	
	public UIButtonMessage cancelButtonMessage = null;
	public UIButtonMessage okButtonMessage = null;
	
	public Collider okButtonCollider = null;
	public GameObject okButtonObj = null;
	
	public void SetMessage(string msg)
	{
		if (messageLabel != null)
			messageLabel.text = msg;
	}
	
	public void SetMessage(int messageID)
	{
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		if (stringTable != null)
		{
			if (messageLabel !=null && messageID != -1)
				messageLabel.text = stringTable.GetData(messageID);
		}
	}
	
	public void SetMessage(int titleID, int messageID)
	{
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		if (stringTable != null)
		{
			if (messageLabel !=null && messageID != -1)
				messageLabel.text = stringTable.GetData(messageID);
			
			if (titleLabel != null && titleID != -1)
				titleLabel.text = stringTable.GetData(titleID);
		}
	}
}
