using UnityEngine;
using System.Collections;

public class CashItemConfirmWindow : BaseConfirmPopup {
	public CashItemInfo itemInfo = null;
	
	public int packageItemPostfixStringID = 25;
	
	public void SetCashItemInfo(CashItemInfo info)
	{
		itemInfo = info;
		
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		string itemName = "";
		string msg = "";
		string moneySpriteName = cashMoneySpriteName;
		string priceValueStr = "";
		string priceValueStrPost = "";
			
		
		if (itemInfo != null)
		{
			itemName = info.itemName;
			
			string msgPostfix = "";
			string cashItemType = "";
			float price = 0.0f;
			
			switch(info.paymentType)
			{
			case ePayment.Cash:
				price = info.price.x;
				moneySpriteName = cashMoneySpriteName;
				priceValueStrPost = stringTable != null ? stringTable.GetData(158) : "Won";
				break;
			case ePayment.Gold:
				price = info.price.x;
				moneySpriteName = goldMoneySpriteName;
				break;
			case ePayment.Jewel:
				price = info.price.y;
				moneySpriteName = jewelMoneySpriteName;
				break;
			case ePayment.Medal:
				price = info.price.z;
				moneySpriteName = medalMoneySpriteName;
				break;
			}
			
			switch(info.cashItemType)
			{
			case eCashItemType.Jewel:
				cashItemType = stringTable != null ? stringTable.GetData(jewelStringID) : "Jewel";
				
				msgPostfix = stringTable != null ? stringTable.GetData(messageStringID) : "";
				msg = string.Format("{0} {1:#,###,###}{2}", cashItemType, info.amount, msgPostfix);
				break;
			case eCashItemType.Gold:
				cashItemType = stringTable != null ? stringTable.GetData(goldStringID) : "Gold";
				
				msgPostfix = stringTable != null ? stringTable.GetData(messageStringID) : "";
				msg = string.Format("{0} {1:#,###,###}{2}", cashItemType, info.amount, msgPostfix);
				break;
			default:
				cashItemType = info.itemName;
				
				msgPostfix = stringTable != null ? stringTable.GetData(packageItemPostfixStringID) : "";
				msg = string.Format("{0}{1}", itemName, msgPostfix);
				break;
			}
			
			/*
			switch(info.type)
			{
			case CashItemType.CashToJewel:
				price = info.price.x;
				moneySpriteName = cashMoneySpriteName;
				cashItemType = stringTable != null ? stringTable.GetData(jewelStringID) : "Jewel";
				priceValueStrPost = stringTable != null ? stringTable.GetData(158) : "Won";
				break;
			case CashItemType.PackageItem:
				price = info.price.x;
				moneySpriteName = cashMoneySpriteName;
				cashItemType = stringTable != null ? stringTable.GetData(jewelStringID) : "Jewel";
				priceValueStrPost = stringTable != null ? stringTable.GetData(158) : "Won";
				break;
			case CashItemType.JewelToGold:
				price = info.price.y;
				moneySpriteName = jewelMoneySpriteName;
				cashItemType = stringTable != null ? stringTable.GetData(goldStringID) : "Gold";
				break;
			case CashItemType.CashToMeat:
				price = info.price.y;
				moneySpriteName = jewelMoneySpriteName;
				cashItemType = stringTable != null ? stringTable.GetData(meatStringID) : "Meat";
				break;
			}
			*/
			
			/*
			switch(info.type)
			{
			case CashItemType.PackageItem:
				msgPostfix = stringTable != null ? stringTable.GetData(packageItemPostfixStringID) : "";
				msg = string.Format("{0}{1}", itemName, msgPostfix);
				break;
			default:
				msgPostfix = stringTable != null ? stringTable.GetData(messageStringID) : "";
				msg = string.Format("{0} {1:#,###,###}{2}", cashItemType, info.amount, msgPostfix);
				break;
			}
			*/
			
			priceValueStr = string.Format("{0:#,###,###} {1}", price, priceValueStrPost);
		}
		
		if (titleLabel != null)
			titleLabel.text = itemName;
		if (messageLabel != null)
			messageLabel.text = msg;
		if (priceValueLabel != null)
			priceValueLabel.text = priceValueStr;
		if (moneyType != null)
			moneyType.spriteName = moneySpriteName;
	}
}
