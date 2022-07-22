using UnityEngine;
using System.Collections;

public class CreateNickNameWindow : MonoBehaviour {

	public UILabel titleLabel = null;
	public int titleStringID = -1;
	
	public UIInput nickNameLabel = null;
	
	public UIButton checkButton = null;
	public UILabel checkButtonLabel = null;
	public int checkButtonStringID = -1;
	
	public UIButton createButton = null;
	public UILabel createButtonLabel = null;
	public UISprite createButtonSprite = null;
	public int createButtonStringID = -1;
	
	public UILabel messageLabel = null;
	public int defaultMessageStringID = -1;
	public int checkOKMessageStringID = -1;
	public Color errorLabelColor = Color.red;
	public Color okLabelColor = Color.white;
	
	public int minLength = 2;
	public int maxLength = 8;
	
	public bool bChecked = false;
	public void Start()
	{
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		SetLabel(titleLabel, titleStringID, stringTable);
		SetLabel(checkButtonLabel, checkButtonStringID, stringTable);
		SetLabel(createButtonLabel, createButtonStringID, stringTable);
		SetLabel(messageLabel, defaultMessageStringID, stringTable);
		
		nickNameLabel.maxChars = maxLength;
		
		CheckButtons();
	}
	
	public void SetLabel(UILabel label, int stringID, StringTable stringTable)
	{
		if (label != null && stringTable != null && stringID != -1)
		{
			label.text = stringTable.GetData(stringID);
		}
	}
	
	public int requestCount = 0;
	public void OnRequestCheckNickName(GameObject obj)
	{
		if (requestCount > 0)
			return;
		
		string nickName = "";
		if (nickNameLabel != null)
			nickName = nickNameLabel.text;
		
		int nickCount = nickName.Length;
		if (nickCount < minLength || nickCount > maxLength)
		{
			OnError(NetErrorCode.NickNameInvalidLength);
			return;
		}
		
		IPacketSender sender = Game.Instance.PacketSender;
		if (sender != null)
			sender.RequestCheckNickName(nickName);
		
		requestCount++;
	}
	
	public void OnRequestCreateNickName(GameObject obj)
	{
		if (requestCount > 0)
			return;
		
		string nickName = "";
		if (nickNameLabel != null)
			nickName = nickNameLabel.text;
		
		int nickCount = nickName.Length;
		if (nickCount < minLength || nickCount > maxLength)
		{
			OnError(NetErrorCode.NickNameInvalidLength);
			return;
		}
		
		IPacketSender sender = Game.Instance.PacketSender;
		if (sender != null)
			sender.RequestCreateNickName(nickName);
		
		requestCount++;
	}
	
	public string nextScene = "SelectCharacter_New";
	public void OnNickNameCreate(NetErrorCode errorCode)
	{
		if (errorCode == NetErrorCode.OK)
		{
			Application.LoadLevelAsync(nextScene);
		}
		else
		{
			nickNameLabel.text = "";
			OnError(errorCode);
		}
	}
	
	public void OnNickNameCheck(NetErrorCode errorCode)
	{
		bChecked = (errorCode == NetErrorCode.OK);
		CheckButtons();
		
		if (bChecked == false)
		{
			nickNameLabel.text = "";
		}
	
		OnError(errorCode);

		requestCount--;
	}
	
	public void OnError(NetErrorCode errorCode)
	{
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		int stringID = defaultMessageStringID;
		Color labelColor = Color.white;
		if (errorCode == NetErrorCode.OK)
		{
			stringID = checkOKMessageStringID;
			labelColor = okLabelColor;
		}
		else
		{
			stringID = (int)errorCode;
			labelColor = errorLabelColor;
		}
		
		if (messageLabel != null)
			messageLabel.color = labelColor;
		
		SetLabel(messageLabel, stringID, stringTable);
	}
	
	public void CheckButtons()
	{
		if (checkButton != null)
			checkButton.isEnabled = true;
		
		if (createButton != null)
			createButton.isEnabled = bChecked;
		
		if (createButtonSprite != null)
			createButtonSprite.enabled = bChecked;
	}
}
