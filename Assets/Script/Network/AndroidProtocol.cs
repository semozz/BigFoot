
using System;

public class TStoreCashItemInfo
{
	public string errorString;		// 응답이올때 채워진다.
	
	public int ItemID;
	//  
	public string TStoreProductCode;
	public string TStoreTID;
	
	public String OriginalJson;
	public String Siginature;
	public int publisherType;
	
	public int Price;
	
	public string itemName;
}


public class AndroidReady
{
	public NetConfig.PublisherType publisher;
	public string cookie;
	public NetConfig.HostType hostType;
	public int netVersion;
}

public class AndroidAchievement
{
	public int groupID;
	public int addCount;
}

public class GoogleInfo
{
	public string Email;
	public string currentPersonName;
}