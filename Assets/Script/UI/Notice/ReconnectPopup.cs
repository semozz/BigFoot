using UnityEngine;
using System.Collections;
using System.Reflection;

public class ReconnectPopup : NoticePopupWindow {
	private string msg = "";
	private string url = "";

    public string methodName;
    public System.Object[] parameters;
    IPacketSender sender;
	
	public void SetInfo(string msg, string url)
	{
		this.msg = msg;
		this.url = url;
	}
	
	
	override public void OnClose(GameObject obj)
	{
		DestroyObject(this.gameObject, 0.0f);

        if (string.IsNullOrEmpty(methodName) == false)
        {
            MethodInfo mi = typeof(Hive5PacketSender).GetMethod(methodName);

            if (mi != null)
                mi.Invoke(sender, parameters);

            return;
        }
		
		ClientConnector connector = Game.Instance.Connector;
		if (connector != null)
		{
			connector.SendPacketReal(this.url, this.msg);
		
			string infoStr = string.Format("ReconnectProcess : msg - {0}, url({1})", this.msg, this.url);
			Debug.Log(infoStr);
		}
	}


    public void SetInfo(IPacketSender sender, string methodName, System.Object[] parameters)
    {
        this.methodName = methodName;
        this.parameters = parameters;
        this.sender = sender;

    }
}
