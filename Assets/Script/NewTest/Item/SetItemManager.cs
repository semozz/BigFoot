using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SetItemManager {

	public Dictionary<int, SetItemInfo> setItemInfoList = new Dictionary<int, SetItemInfo>();
	
	public SetItemInfo GetSetItemInfo(int setItemID)
	{
		SetItemInfo info = null;
		if (setItemInfoList.ContainsKey(setItemID) == true)
			info = setItemInfoList[setItemID];
		else
		{
			TableManager tableManager = TableManager.Instance;
			SetItemTable setItemTable = tableManager != null ? tableManager.setItemInfo : null;
			
			if (setItemTable != null)
			{
				info = setItemTable.GetTempInfo(setItemID);
				if (info != null)
					setItemInfoList.Add(setItemID, info);
			}
		}
		return info;
	}
	
	public void ApplySetItem(Item item, AttributeManager attributeManager)
	{
		if (item == null || item.setItemID == -1)
			return;
		
		SetItemInfo setItemInfo = null;
		
		if (setItemInfoList.ContainsKey(item.setItemID) == false)
		{
			TableManager tableManaer = TableManager.Instance;
			SetItemTable setItemTable = tableManaer != null ? tableManaer.setItemInfo : null;
			
			if (setItemTable != null)
				setItemInfo = setItemTable.GetTempInfo(item.setItemID);
			
			if (setItemInfo != null)
				setItemInfoList.Add(item.setItemID, setItemInfo);
		}
		else
		{
			setItemInfo = setItemInfoList[item.setItemID];
		}
		
		if (setItemInfo != null)
			setItemInfo.AddItem(item, attributeManager);
	}
	
	public void RemoveSetItem(Item item, AttributeManager attributeManager)
	{
		if (item == null || item.setItemID == -1)
			return;
		
		SetItemInfo setItemInfo = null;
		
		if (setItemInfoList.ContainsKey(item.setItemID) == false)
		{
			TableManager tableManaer = TableManager.Instance;
			SetItemTable setItemTable = tableManaer != null ? tableManaer.setItemInfo : null;
			
			if (setItemTable != null)
				setItemInfo = setItemTable.GetTempInfo(item.setItemID);
			
			if (setItemInfo != null)
				setItemInfoList.Add(item.setItemID, setItemInfo);
		}
		else
		{
			setItemInfo = setItemInfoList[item.setItemID];
		}
		
		if (setItemInfo != null)
			setItemInfo.RemoveItem(item, attributeManager);
	}
}
