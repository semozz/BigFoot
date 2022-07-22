using UnityEngine;
using System.Collections;

public class MessageBox : MonoBehaviour {

	public UILabel titleLabel = null;
	public UILabel okButtonLabel = null;
	public UILabel messageBoxLabel = null;
	
	public GameObject parent = null;
	public string onCloseFunc = "";
	
	public void OnOK()
	{
		DestroyObject(this.gameObject, 0.0f);
		
		if (parent != null && onCloseFunc != "")
			parent.SendMessage(onCloseFunc, false, SendMessageOptions.DontRequireReceiver);
	}
	
	public void SetMessage(string titleStr, string msgStr)
	{
		if (messageBoxLabel != null)
			messageBoxLabel.text = msgStr;
		
		if (titleLabel != null)
			titleLabel.text = titleStr;
		
	}
}
