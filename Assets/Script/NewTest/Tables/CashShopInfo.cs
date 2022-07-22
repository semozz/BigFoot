using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;

public enum eCashItemType
{
	Jewel,
	Gold,
	Potion1,
	Potion2,
	Event,
	StartPack,
	BuffPack,
	RandomBox,
	JewelPack,
}

public enum ePayment
{
	Cash,
	Jewel,
	Gold,
	Medal,
}

public class CashItemInfo
{
	public int ItemID;
	public CashItemType type = CashItemType.CashToJewel;	
	public string addInfo = "";
	public string itemName = "";
	public float amount = 0.0f;
	
	public Vector3 price = Vector3.zero;
	
	public string spriteName = "";
	
	public Dictionary<NetConfig.PublisherType, string> storeItemCodes = new Dictionary<NetConfig.PublisherType, string>();
	
	public eCashItemType cashItemType = eCashItemType.Jewel;
	public ePayment paymentType = ePayment.Jewel;
	public int eventID = 0;
	
	public CashItemInfo()
	{
	}
	
	public CashItemInfo(int ItemID, CashItemType type, string addInfo, string name, float amount, Vector3 price, string spriteName, eCashItemType cashType, ePayment payType)
	{
		this.ItemID = ItemID;
		this.type = type;
		this.addInfo = addInfo;
		this.itemName = name;
		this.amount = amount;
		this.price = price;
		this.spriteName = spriteName;
		
		this.cashItemType = cashType;
		this.paymentType = payType;
	}
	
	public string GetStoreItemCode(NetConfig.PublisherType type)
	{
		if (type == NetConfig.PublisherType.Kakao)
			type = NetConfig.PublisherType.Google;
		
		string storeItemCode = "";
		if (storeItemCodes.ContainsKey(type) == true)
			storeItemCode = storeItemCodes[type];
		
		return storeItemCode;
	}
	
	public string ToString()
	{
		string storeCode = "";
		foreach(var code in storeItemCodes)
			storeCode += code.Value + ",";
		
		string infoStr = string.Format("ID:{0} type:{1} addInfo:{2} name:{3} amount:{4} price:{5} storeCode:{6}",
			this.ItemID, this.type, this.addInfo, this.itemName, this.amount, this.price, storeCode);
		
		return infoStr;
	}
	
	public static eCashItemType ToCashItemType(string str)
	{
		eCashItemType type = eCashItemType.Jewel;
		
		if (str == "Jewel")
			type = eCashItemType.Jewel;
		else if (str == "Gold")
			type = eCashItemType.Gold;
		else if (str == "Potion1")
			type = eCashItemType.Potion1;
		else if (str == "Potion2")
			type = eCashItemType.Potion2;
		else if (str == "Event")
			type = eCashItemType.Event;
		else if (str == "StartPack")
			type = eCashItemType.StartPack;
		else if (str == "BuffPack")
			type = eCashItemType.BuffPack;
		else if (str == "RandomBox")
			type = eCashItemType.RandomBox;
		else if (str == "JewelPack")
			type = eCashItemType.JewelPack;
		
		return type;
	}
	
	public static ePayment ToPaymentType(string str)
	{
		ePayment type = ePayment.Jewel;
		
		if (str == "Cash")
			type = ePayment.Cash;
		else if (str == "Jewel")
			type = ePayment.Jewel;
		else if (str == "Gold")
			type = ePayment.Gold;
		else if (str == "Medal")
			type = ePayment.Medal;
		
		return type;
	}
}

/*
public class CashShopInfo {
	public List<CashItemInfo> jewelItemInfos = new List<CashItemInfo>();
	public List<CashItemInfo> goldItemInfos = new List<CashItemInfo>();
	
	public CashShopInfo()
	{
		
	}
	
	public CashItemInfo GetJewelInfo(int index)
	{
		CashItemInfo info = null;
		int nCount = jewelItemInfos.Count;
		if (index < 0 || index >= nCount)
			return info;
		
		info = jewelItemInfos[index];
		return info;
	}
	
	public CashItemInfo GetGoldInfo(int index)
	{
		CashItemInfo info = null;
		int nCount = goldItemInfos.Count;
		if (index < 0 || index >= nCount)
			return info;
		
		info = goldItemInfos[index];
		return info;
	}
	
	public void InitValue()
	{
		jewelItemInfos.Clear();
		goldItemInfos.Clear();
	}
	
	public void Load(string filePath)
	{
		InitValue();
		
		TextAsset textAsset = (TextAsset)Resources.Load(filePath, typeof(TextAsset));
		XmlTextReader textReader = new XmlTextReader(new StringReader(textAsset.text));
		XmlDocument xmlDoc = new XmlDocument();
		
		if (xmlDoc != null && textReader != null)
		{
			xmlDoc.Load(textReader);
			
			XmlNodeList data = xmlDoc.GetElementsByTagName("CashShop");
			for (int i = 0; i < data.Count; ++i)
			{
				XmlNode dataChilds = data.Item(i);
				
				XmlNodeList allObjects = dataChilds.ChildNodes;
				
				for (int index = 0; index < allObjects.Count; ++index)
				{
					XmlNode node = allObjects.Item(index);
					
					if (node == null)
						continue;
					
					if (node.Name == "JewelItems")
					{
						LoadJewelItem(node, jewelItemInfos);
					}
					else if (node.Name == "GoldItems")
					{
						LoadGoldItem(node, goldItemInfos);
					}
				}
			}
			
			textReader.Close();
		}
	}
	
	public void LoadJewelItem(XmlNode rootNode, List<CashItemInfo> jewelItems)
	{
		XmlNodeList allObjects = rootNode.ChildNodes;
		for (int index = 0; index < allObjects.Count; ++index)
		{
			XmlNode node = allObjects.Item(index);
			
			if (node == null)
				continue;
			
			if (node.Name == "JewelItem")
			{
				string[] attributeInfos = node.InnerText.Split(',');
				
				int ItemID = int.Parse(attributeInfos[0]);
				string addInfo = attributeInfos[1];
				string itemName = attributeInfos[2];
				float amount = float.Parse(attributeInfos[3]);
				float gold = float.Parse(attributeInfos[4]);
				string spriteName = attributeInfos[5];
				
				Vector3 price = Vector3.zero;
				price.x = gold;
				
				CashItemInfo newInfo = new CashItemInfo(ItemID, CashItemType.CashToJewel, addInfo, itemName, amount, price, spriteName);
				jewelItems.Add(newInfo);
			}
		}
	}
	
	public void LoadGoldItem(XmlNode rootNode, List<CashItemInfo> goldItems)
	{
		XmlNodeList allObjects = rootNode.ChildNodes;
		for (int index = 0; index < allObjects.Count; ++index)
		{
			XmlNode node = allObjects.Item(index);
			
			if (node == null)
				continue;
			
			if (node.Name == "GoldItem")
			{
				string[] attributeInfos = node.InnerText.Split(',');
				
				int ItemID = int.Parse(attributeInfos[0]);
				string addInfo = attributeInfos[1];
				string itemName = attributeInfos[2];
				float amount = float.Parse(attributeInfos[3]);
				float jewel = float.Parse(attributeInfos[4]);
				string spriteName = attributeInfos[5];
				
				Vector3 price = Vector3.zero;
				price.y = jewel;
				
				CashItemInfo newInfo = new CashItemInfo(ItemID, CashItemType.JewelToGold, addInfo, itemName, amount, price, spriteName);
				goldItems.Add(newInfo);
			}
		}
	}
}
*/