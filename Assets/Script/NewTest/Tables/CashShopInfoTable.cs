using UnityEngine;
using System.Collections.Generic;

public class CashShopInfoTable : BaseTable 
{
	public List<CashItemInfo> jewelItemInfos = new List<CashItemInfo>();
	public List<CashItemInfo> goldItemInfos = new List<CashItemInfo>();
	public List<CashItemInfo> potionItemInfos = new List<CashItemInfo>();
	public List<CashItemInfo> timeListItemInfos = new List<CashItemInfo>();
	
	public List<CashItemInfo> eventShopItemInfos = new List<CashItemInfo>();
	public List<CashItemInfo> packageItemInfos = new List<CashItemInfo>();
	
	public List<CashItemInfo> randomBoxItemInfos = new List<CashItemInfo>();
	
	public List<CashItemInfo> totalShopItemInfos = new List<CashItemInfo>();
	
	public override void LoadTable(CSVDB db)
	{
		if (db != null)
		{
			foreach(var data in db.data)
			{
				CashItemInfo info = new CashItemInfo();
				
				info.price = Vector3.zero;
				
				info.ItemID = int.Parse(data.Key);
				info.type = (CashItemType)data.Value.GetValue("Type").ToInt();
				info.amount = data.Value.GetValue("Product").ToInt();
				info.itemName = data.Value.GetValue("Name").ToText();
				info.addInfo = data.Value.GetValue("Sale").ToText();
				info.spriteName = data.Value.GetValue("spriteName").ToText();
				
				ValueData tempValue = null;
				string storeItemCode = "";
				tempValue = data.Value.GetValue("TStoreCode");
				
				if (tempValue != null)
				{
					storeItemCode = tempValue.ToText();
					info.storeItemCodes.Add(NetConfig.PublisherType.TStore, storeItemCode);
				}
				
				storeItemCode = "";
				tempValue = data.Value.GetValue("GoogleCode");
				if (tempValue != null)
				{
					storeItemCode = tempValue.ToText();
					info.storeItemCodes.Add(NetConfig.PublisherType.Google, storeItemCode);
				}
				
				tempValue = data.Value.GetValue("ItemType");
				if (tempValue != null)
					info.cashItemType = CashItemInfo.ToCashItemType(tempValue.ToText());
				
				tempValue = data.Value.GetValue("Payment");
				if (tempValue != null)
					info.paymentType = CashItemInfo.ToPaymentType(tempValue.ToText());
				
				tempValue = data.Value.GetValue("EventID");
				if (tempValue != null)
					info.eventID = tempValue.ToInt();
				
				switch(info.paymentType)
				{
				case ePayment.Cash:
				case ePayment.Gold:
					info.price.x = data.Value.GetValue("Price").ToInt();
					break;
				case ePayment.Jewel:
					info.price.y = data.Value.GetValue("Price").ToInt();
					break;
				case ePayment.Medal:
					info.price.z = data.Value.GetValue("Price").ToInt();
					break;
				}
				
				switch(info.cashItemType)
				{
				case eCashItemType.Jewel:
					jewelItemInfos.Add(info);
					break;
				case eCashItemType.JewelPack:
					jewelItemInfos.Insert(0, info);
					break;
				case eCashItemType.Gold:
					goldItemInfos.Add(info);
					break;
				case eCashItemType.Potion1:
				case eCashItemType.Potion2:
					potionItemInfos.Add(info);
					break;
				case eCashItemType.Event:
					eventShopItemInfos.Add(info);
					break;
				case eCashItemType.StartPack:
					packageItemInfos.Add(info);
					break;
				case eCashItemType.BuffPack:
					timeListItemInfos.Add(info);
					break;
				case eCashItemType.RandomBox:
					randomBoxItemInfos.Add(info);
					break;
				}
				
				totalShopItemInfos.Add(info);
			}
		}	
	}
	
	public CashItemInfo GetItemInfoByStoreItemCode(string storeItemCode, NetConfig.PublisherType publisherType)
	{
		if (publisherType == NetConfig.PublisherType.Kakao)
			publisherType = NetConfig.PublisherType.Google;
		
		CashItemInfo findInfo = null;
		foreach(CashItemInfo info in totalShopItemInfos)
		{
			string tempCode = info.GetStoreItemCode(publisherType);
			if (string.IsNullOrEmpty(tempCode) == true)
				continue;
			
			if (tempCode == storeItemCode)
			{
				findInfo = info;
				break;
			}
		}
		
		return findInfo;
	}
	
	public CashItemInfo GetItemInfo(int itemID)
	{
		CashItemInfo findInfo = null;
		foreach(CashItemInfo info in totalShopItemInfos)
		{
			if (info.ItemID == itemID)
			{
				findInfo = info;
				break;
			}
		}
		
		return findInfo;
	}

    public List<int> GetItemIDByEventID(int eventID)
    {
        List<int> itemIDs = new List<int>();

		CashItemInfo findInfo = null;
		foreach(CashItemInfo info in totalShopItemInfos)
		{
            if (info.eventID == eventID)
                itemIDs.Add(info.ItemID);
		}

        return itemIDs;
    }

    public List<int> GetProductIDByEventID(int eventID)
    {
        List<int> itemIDs = new List<int>();

        CashItemInfo findInfo = null;
        foreach (CashItemInfo info in totalShopItemInfos)
        {
            if (info.eventID == eventID)
                itemIDs.Add((int)info.amount);
        }

        return itemIDs;
    }
}
