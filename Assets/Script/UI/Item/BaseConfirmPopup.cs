using UnityEngine;
using System.Collections;

public class BaseConfirmPopup : BasePopup {
	public UILabel pricePrefixLabel = null;
	public UISprite moneyType = null;
	public UILabel priceValueLabel = null;
	
	public int meatStringID = 206;
	public int goldStringID = -1;
	public int jewelStringID = -1;
	public int prefixStringID = -1;
	//public int messageStringID = -1;
	
	public string cashMoneySpriteName = "";
	public string jewelMoneySpriteName = "";
	public string goldMoneySpriteName = "";
	public string medalMoneySpriteName = "Price_money_Big_Medal";
	
	void Awake()
	{
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		if (stringTable != null)
		{
			if (cancelButtonLabel != null && cancelButtonStringID != -1)
				cancelButtonLabel.text = stringTable.GetData(cancelButtonStringID);
			
			if (okButtonLabel != null && okButtonStringID != -1)
				okButtonLabel.text = stringTable.GetData(okButtonStringID);
			
			if (pricePrefixLabel != null && prefixStringID != -1)
				pricePrefixLabel.text = stringTable.GetData(prefixStringID);
			
			if (titleLabel != null && titleStringID != -1)
				titleLabel.text = stringTable.GetData(titleStringID);
			if (messageLabel !=null && messageStringID != -1)
				messageLabel.text = stringTable.GetData(messageStringID);
		}
	}
	
	public void Wait()
	{
		if (okButtonCollider != null)
			okButtonCollider.enabled = false;
	}
	
	//public Item item = null;
	
	public void SetGoldInfo(string itemName, Vector3 priceValue)
	{
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		float price = 0.0f;
			
		string moneySpriteName = goldMoneySpriteName;
		if (priceValue.x > 0.0f)
		{
			price = priceValue.x;
			moneySpriteName = goldMoneySpriteName;
		}
		else if (priceValue.y > 0.0f)
		{
			price = priceValue.y;
			moneySpriteName = jewelMoneySpriteName;
		}
		else if (priceValue.z > 0.0f)
		{
			price = priceValue.z;
			moneySpriteName = medalMoneySpriteName;
		}
		
		string msg = stringTable != null ? stringTable.GetData(messageStringID) : "";
		string priceValueStr = string.Format("{0:#,###,###}", price);
		
		if (titleLabel != null)
			titleLabel.text = itemName;
		if (messageLabel != null)
			messageLabel.text = msg;
		if (priceValueLabel != null)
			priceValueLabel.text = priceValueStr;
		if (moneyType != null)
			moneyType.spriteName = moneySpriteName;
	}
	
	public void SetGoldInfo(CostumeSetItem costumeSetItem, bool isBuy)
	{
		string itemName = "";
		Vector3 itemPrice = Vector3.zero;
		
		if (costumeSetItem != null && costumeSetItem.setItemInfo != null)
		{
			itemName = costumeSetItem.setItemInfo.setName;
			
			if (isBuy == true)
				itemPrice = costumeSetItem.setItemInfo.buyPrice;
			else
				itemPrice = costumeSetItem.setItemInfo.sellPrice;
		}
		
		SetGoldInfo(itemName, itemPrice);
	}
	
	public void SetGoldInfo(Item item, bool isBuy)
	{
		string itemName = "";
		Vector3 itemPrice = Vector3.zero;
		
		if (item != null && item.itemInfo != null)
		{
			itemName = item.itemInfo.itemName;
			
			if (isBuy == true)
				itemPrice = item.itemInfo.buyPrice * item.itemCount;
			else
				itemPrice = item.GetSellPrice() * item.itemCount;
		}
		
		SetGoldInfo(itemName, itemPrice);
	}
	
	public void SetMessage(string title, string message)
	{
		if (titleLabel != null)
			titleLabel.text = title;
		
		if (messageLabel != null)
			messageLabel.text = message;
	}
}
