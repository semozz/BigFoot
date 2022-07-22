using UnityEngine;
using System.Collections;

public class NeedMoneyWindow : BasePopup {
	public UISprite moneyType = null;
	
	public string goldSprite = "";
	public string jewelSprite = "";
	public string medalSprite = "";
	
	public int titleString_1_ID = -1;
	public int titleString_2_ID = -1;
	public int titleString_3_ID = -1;
	public int messageString_1_ID = -1;
	public int messageString_2_ID = -1;
	public int messageString_3_ID = -1;
	
	public int goldStringID = -1;
	public int jewelStringID = -1;
	public int medalStringID = -1;
	
	public string title_1_Str = "";
	public string title_2_Str = "";
	public string title_3_Str = "";
	public string msg_1_Str = "";
	public string msg_2_Str = "";
	public string msg_3_Str = "";
	
	public string goldString = "";
	public string jewelString = "";
	public string medalString = "";
	
	public CashItemType needMoneyType = CashItemType.JewelToGold;
	
	public UIButtonMessage medalButtonMessage = null;
	public UILabel medalButtonLabel = null;
	void Awake()
	{
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		if (stringTable != null)
		{
			title_1_Str = stringTable.GetData(titleString_1_ID);
			title_2_Str = stringTable.GetData(titleString_2_ID);
			title_3_Str = stringTable.GetData(titleString_3_ID);
			msg_1_Str = stringTable.GetData(messageString_1_ID);
			msg_2_Str = stringTable.GetData(messageString_2_ID);
			msg_3_Str = stringTable.GetData(messageString_3_ID);
			
			goldString = stringTable.GetData(goldStringID);
			jewelString = stringTable.GetData(jewelStringID);
			medalString = stringTable.GetData(medalStringID);
			
			if (cancelButtonLabel != null)
				cancelButtonLabel.text = stringTable.GetData(cancelButtonStringID);
			if (okButtonLabel != null)
				okButtonLabel.text = stringTable.GetData(okButtonStringID);
			
			if (medalButtonLabel != null)
				medalButtonLabel.text = stringTable.GetData(cancelButtonStringID);
		}
	}
	
	public PopupBaseWindow popupBaseWindow = null;
	public void SetInfo(CashItemType type, PopupBaseWindow popupBase)
	{
		needMoneyType = type;
		popupBaseWindow = popupBase;
		
		string titleString = "";
		string msgString = "";
		string moneySprite = jewelSprite;
		
		if (okButtonObj != null)
			okButtonObj.SetActive(true);
		
		switch(type)
		{
		case CashItemType.CashToJewel:
			titleString = string.Format("{0}{1}", jewelString, title_1_Str);
			msgString = string.Format("{0}{1}", jewelString, msg_1_Str);
			moneySprite = jewelSprite;
			break;
		case CashItemType.JewelToGold:
			titleString = string.Format("{0}{1}", goldString, title_2_Str);
			msgString = string.Format("{0}{1}", goldString, msg_2_Str);
			moneySprite = goldSprite;
			break;
		case CashItemType.Medal:
			titleString = string.Format("{0}{1}", medalString, title_3_Str);
			msgString = string.Format("{0}{1}", medalString, msg_3_Str);
			moneySprite = medalSprite;
			if (okButtonObj != null)
				okButtonObj.SetActive(false);
			break;
		}
		
		switch(type)
		{
		case CashItemType.CashToJewel:
		case CashItemType.JewelToGold:
			if (medalButtonMessage != null)
				medalButtonMessage.gameObject.SetActive(false);
			
			if (cancelButtonMessage != null)
				cancelButtonMessage.gameObject.SetActive(true);
			
			if (okButtonMessage != null)
				okButtonMessage.gameObject.SetActive(true);
			
			break;
		case CashItemType.Medal:
			if (medalButtonMessage != null)
				medalButtonMessage.gameObject.SetActive(true);
			
			if (cancelButtonMessage != null)
				cancelButtonMessage.gameObject.SetActive(false);
			
			if (okButtonMessage != null)
				okButtonMessage.gameObject.SetActive(false);
			
			cancelButtonMessage = medalButtonMessage;
			break;
		}
		
		if (titleLabel != null)
			titleLabel.text = titleString;
		if (messageLabel != null)
			messageLabel.text = msgString;
		if (moneyType != null)
			moneyType.spriteName = moneySprite;
	}
}
