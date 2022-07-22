using UnityEngine;
using System.Collections;

public class CashItemPanel : MonoBehaviour {
	public UISprite itemSprite = null;
	
	public UILabel addInfoLabel = null;
	public UILabel itemNameLabel = null;
	public UILabel amountInfoLabel = null;
	
	public UILabel itemPriceLabel = null;
	
	public UIButton buyButton = null;
	public UIButtonMessage buttonMessage = null;
	
	public string defaultSpriteName = "Shop_Black01_549_44";
	
	public CashItemInfo itemInfo = null;
	
	public void Awake()
	{
		UpdateInfo();
	}
	
	public void UpdateInfo()
	{
		string addInfoStr = "";
		string itemName = "";
		string amountStr = "";
		string priceStr = "";
		string spriteName = defaultSpriteName;
		string priceSpriteName = "";
		
		if (itemInfo != null)
		{
			addInfoStr = itemInfo.addInfo;
			itemName = itemInfo.itemName;
			amountStr = string.Format("{0:#,###,###}", itemInfo.amount);
			
			spriteName = itemInfo.spriteName;
						
			float priceValue = 0.0f;
			switch(itemInfo.paymentType)
			{
			case ePayment.Cash:
			case ePayment.Gold:
				priceValue = itemInfo.price.x;
				break;
			case ePayment.Jewel:
				priceValue = itemInfo.price.y;
				break;
			case ePayment.Medal:
				priceValue = itemInfo.price.z;
				break;
			}
			
			priceStr = string.Format("{0:#,###,###}", priceValue);
		}
		
		if (addInfoLabel != null)
			addInfoLabel.text = addInfoStr;
		//if (amountInfoLabel != null)
		//	amountInfoLabel.text = itemName;
		
		if (itemNameLabel != null)
			itemNameLabel.text = itemName;
		
		if (itemPriceLabel != null)
			itemPriceLabel.text = priceStr;
		
		if (itemSprite != null)
			itemSprite.spriteName = spriteName;
	}
	
	public void SetCashItem(CashItemInfo info)
	{
		itemInfo = info;
		
		UpdateInfo();
		
		if (buyButton != null)
			buyButton.isEnabled = (itemInfo != null);
	}	
}
