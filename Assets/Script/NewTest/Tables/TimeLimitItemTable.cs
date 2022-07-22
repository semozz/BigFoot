using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimeLimitBuffInfo
{
	public System.DateTime endTime;
	public AttributeValue.eAttributeType buffType;
	public TimeLimitBuffItemInfo.eTimeLimitBuffItemType itemType;
	public float buffValue;
}

public class TimeLimitBuffItemInfo
{
	public enum eTimeLimitBuffItemType
	{
		GoldAndExpBuffItem = 0,
		JewelBuffItem = 1,
	}
	
	public eTimeLimitBuffItemType type = eTimeLimitBuffItemType.GoldAndExpBuffItem;
	
	public int periodDay = 1;

	public List<TimeLimitBuffInfo> buffList = new List<TimeLimitBuffInfo>();
	
	public void AddBuffInfo(AttributeValue.eAttributeType type, TimeLimitBuffItemInfo.eTimeLimitBuffItemType itemType, float buffValue)
	{
		TimeLimitBuffInfo buffInfo = new TimeLimitBuffInfo();
		buffInfo.buffType = type;
		buffInfo.itemType = itemType;
		buffInfo.buffValue = buffValue;
		
		buffList.Add(buffInfo);
	}
}

public class TimeLimitItemTable : BaseTable {

	public Dictionary<int, TimeLimitBuffItemInfo> dataList = new Dictionary<int, TimeLimitBuffItemInfo>();
	
	public TimeLimitBuffItemInfo GetData(int id)
	{
		TimeLimitBuffItemInfo info = null;
		if (dataList != null && dataList.ContainsKey(id) == true)
			info = dataList[id];
		
		return info;
	}
	
	public TimeLimitBuffItemInfo GetDataByType(string buff_type)
	{
		TimeLimitBuffItemInfo.eTimeLimitBuffItemType type = TimeLimitBuffItemInfo.eTimeLimitBuffItemType.GoldAndExpBuffItem;
		
		if (buff_type == "BuffPack")
			type = TimeLimitBuffItemInfo.eTimeLimitBuffItemType.GoldAndExpBuffItem;
		else if (buff_type == "JewelPack")
			type = TimeLimitBuffItemInfo.eTimeLimitBuffItemType.JewelBuffItem;
		
		TimeLimitBuffItemInfo info = null;
		if (dataList != null)
		{	
			foreach (var data in dataList)
			{
				if (data.Value.type == type)
				{
					info = data.Value;
					break;
				}
			}
		}
		
		return info;
	}
	
	public override void LoadTable(CSVDB db)
	{
		if (db != null)
		{
			int id = 0;
			TimeLimitBuffItemInfo info = null;
			
			ValueData buffValueData = null;
			string fieldName = "";
			float buffValue = 0.0f;
			
			List<float> buffValueList = new List<float>();
			foreach(var data in db.data)
			{
				id = int.Parse(data.Key);
				
				info = new TimeLimitBuffItemInfo();
				info.type = (TimeLimitBuffItemInfo.eTimeLimitBuffItemType)data.Value.GetValue("buffType").ToInt();
				
				buffValueList.Clear();
				for(int index = 0; ; ++index)
				{
					fieldName = string.Format("buffValue_{0}",  index);
					buffValueData = data.Value.GetValue(fieldName);
					
					if (buffValueData == null)
						break;
					
					buffValue = buffValueData.ToFloat();
					buffValueList.Add(buffValue);
				}
				
				switch(info.type)
				{
				case TimeLimitBuffItemInfo.eTimeLimitBuffItemType.GoldAndExpBuffItem:
					info.AddBuffInfo(AttributeValue.eAttributeType.IncGainGold, info.type, GetBuffValue(0, buffValueList));
					info.AddBuffInfo(AttributeValue.eAttributeType.IncGainExp, info.type, GetBuffValue(1, buffValueList));
					break;
				case TimeLimitBuffItemInfo.eTimeLimitBuffItemType.JewelBuffItem:
					info.AddBuffInfo(AttributeValue.eAttributeType.DailyGainJewel, info.type, GetBuffValue(0, buffValueList));
					info.AddBuffInfo(AttributeValue.eAttributeType.DailyGainAwakenPoint, info.type, GetBuffValue(1, buffValueList));
					break;
				}
				
				dataList.Add(id, info);
			}
		}
		
	}
	
	private float GetBuffValue(int index, List<float> list)
	{
		float buffValue = 0.0f;
		int nCount = list.Count;
		if (index >= 0 && index < nCount)
			buffValue = list[index];
		
		return buffValue;
	}
}
