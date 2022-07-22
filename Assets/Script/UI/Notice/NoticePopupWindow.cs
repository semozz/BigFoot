using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class NoticeItem
{
	public enum eNoticeType
	{
		None,
		ImageURL,
		Message,
	}
	public eNoticeType type = eNoticeType.None;
	
	public long noticeID = 0;
	public string imgURL = "";
	public string message = "";
	public string linkURL = "";
	
	public int order = 0;
	
	static public int SortByOrder (NoticeItem a, NoticeItem b) 
	{ 
		return a.order - b.order; 
	}
}

public class NoticePopupWindow : MonoBehaviour {
	public enum Fatal_Error
	{
		None,
		GetUserError,
		TimeOut,
	}
	private Fatal_Error errorCode = Fatal_Error.None;
	
	public NoticeItem noticeItem = null;
	
	public WebImageTexture image = null;
	public UILabel message = null;
	
	public UICheckbox noticeIgnore = null;
	
	public UIButton gotoButton = null;
	public UIButtonMessage buttonMessage = null;
	
	public void SetMessage(string msg)
	{
		NoticeItem notice = new NoticeItem();
		notice.message = msg;
		notice.type = NoticeItem.eNoticeType.Message;
		
		SetNotice(notice);
	}
	
	public void SetMessage(NetErrorCode errorCode)
	{
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		string msg = "NetError!!!";
		if (stringTable != null)
			msg = stringTable.GetData((int)errorCode);
		
		SetMessage(msg);
	}
	
	public void SetNotice(NoticeItem notice)
	{
		noticeItem = notice;
		
		if (notice != null)
		{
			if (notice.imgURL != "")
			{
				if (image != null)
				{
					image.gameObject.SetActive(true);
					image.SetURL(notice.imgURL);
				}
				
				if (gotoButton != null)
					gotoButton.isEnabled = false;
				
				if (message != null)
					message.gameObject.SetActive(false);
			}
			else
			{
				if (gotoButton != null)
					gotoButton.isEnabled = false;
				
				if (image != null)
					image.gameObject.SetActive(false);
				
				if (message != null)
				{
					message.gameObject.SetActive(true);
					message.text = notice.message;
				}
			}
		}
	}
	
	public void OnBack()
	{
		DestroyObject(this.gameObject, 0.0f);
		GameUI.Instance.townUI.OnEnterTown();
	}
	
	public void OnNoticeCheck()
	{
		if (noticeIgnore != null)
		{
			bool bCheck = noticeIgnore.isChecked;
			long noticeID = noticeItem != null ? noticeItem.noticeID : 0;
			
			if (bCheck == true)
			{
				IPacketSender sender = Game.Instance.PacketSender;
				
				if (sender != null)
					sender.SendPopupNoticeIgnore(noticeID);
				
				OnBack();
			}
		}
	}
	
	public void LoadComplete()
	{
		if (this.gotoButton != null)
			this.gotoButton.isEnabled = true;
	}
	
	public void GoToLink(GameObject obj)
	{
		if (this.noticeItem != null && this.noticeItem.linkURL != "")
			Application.OpenURL(this.noticeItem.linkURL);
	}
	
	virtual public void OnClose(GameObject obj)
	{
		DestroyObject(this.gameObject, 0.1f);
		
		switch(errorCode)
		{
		case Fatal_Error.GetUserError:
			OptionWindow.OnLogoutConfirmed();
			break;
		case Fatal_Error.TimeOut:
			OnTimeErrorProcess();
			break;
		}
	}
	
	public void SetFatalError(Fatal_Error error, int errorMessageStringID)
	{
		this.errorCode = error;
		
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		string msg = "FatalError!!!";
		if (stringTable != null)
			msg = stringTable.GetData(errorMessageStringID);
		
		SetMessage(msg);
	}
	
	
	private string functionName = "";
	private string paramJson = null;
	public void SetTimeOut(string functionName, string parameterJson)
	{
		this.errorCode = Fatal_Error.TimeOut;
		
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		string msg = "TimeOut!!!";
		if (stringTable != null)
			msg = stringTable.GetData((int)NetErrorCode.WebTimeOut);
		
		SetMessage(msg);
		
		this.functionName = functionName;
		this.paramJson = parameterJson;
	}
	
	private void OnTimeErrorProcess()
	{
		IPacketSender sender = Game.Instance.PacketSender;
		
		if (sender != null)
			sender.TimeOutProcess(functionName, paramJson);
	}
}
