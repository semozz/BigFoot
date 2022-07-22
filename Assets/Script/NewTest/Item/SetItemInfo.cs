using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Set attribute info.
/// 세트아이템 스텝별 효과 정보.
/// </summary>
public class SetAttributeInfo
{
	public bool isAcive = false;	//활성화 여부.
	
	public int limitCount = -1;		//최소 아이템 갯수.
	public List<AttributeValue> attributeList = new List<AttributeValue>();	//능력치.
	
	public void AddAttribute(AttributeValue.eAttributeType type, float _value)
	{
		if (IsExistAttributeValue(type) == true)
			return;
		
		AttributeValue newValue = new AttributeValue(type, _value, 0.0f, 0.0f);
		newValue.calcType = AttributeValue.eAttributeCalcType.ItemAttribute;
		
		attributeList.Add(newValue);
	}
	
	public bool IsExistAttributeValue(AttributeValue.eAttributeType type)
	{
		foreach(AttributeValue temp in attributeList)
		{
			if (temp.valueType == type)
				return true;
		}
		
		return false;
	}
	
	public SetAttributeInfo()
	{
		
	}
	
	public SetAttributeInfo(SetAttributeInfo oldInfo)
	{
		isAcive = false;
		limitCount = oldInfo.limitCount;
		
		foreach(AttributeValue temp in oldInfo.attributeList)
		{
			AddAttribute(temp.valueType, temp.baseValue);
		}
	}
}

/// <summary>
/// Set item info.
/// 세트아이템 정보.
/// </summary>
public class SetItemInfo{
	public int setItemID  = -1;
	public string setItemName = "";
	
	public List<SetAttributeInfo> setAttributeList = new List<SetAttributeInfo>();
	
	public List<Item> setItemList = new List<Item>();
	
	public List<SetAttributeInfo> availAttributeList = new List<SetAttributeInfo>();
	
	public SetItemInfo()
	{
		
	}
	
	public SetItemInfo(SetItemInfo origInfo)
	{
		this.setItemID = origInfo.setItemID;
		this.setItemName = origInfo.setItemName;
		
		setAttributeList.Clear();
		foreach(SetAttributeInfo temp in origInfo.setAttributeList)
		{
			SetAttributeInfo newInfo = new SetAttributeInfo(temp);
			setAttributeList.Add(newInfo);
		}
	}
	
	public void AddItem(Item item, AttributeManager attributeManager)
	{
		if (item == null || item.setItemID != setItemID)
			return;
		
		setItemList.Add(item);
		
		UpdateSetItemInfo(attributeManager);
	}
	
	public void RemoveItem(Item item, AttributeManager attributeManager)
	{
		if (item == null || item.setItemID != setItemID)
			return;
		
		setItemList.Remove(item);
		
		UpdateSetItemInfo(attributeManager);
	}
	
	public void UpdateSetItemInfo(AttributeManager attributeManager)
	{
		int itemCount = setItemList.Count;
		
		//리스트에서 지우기 전에 능력치 제거 먼저 해야함..
		if (attributeManager != null)
		{
			foreach(SetAttributeInfo info in availAttributeList)
			{
				foreach(AttributeValue _value in info.attributeList)
				{
					attributeManager.SubValue(_value.valueType, _value.Value);
				}
			}
		}
		availAttributeList.Clear();
		
		foreach(SetAttributeInfo info in setAttributeList)
		{
			//활성화 제한 아이템 갯수를 충족 하는지...
			info.isAcive = (info.limitCount <= itemCount);
			
			if (info.isAcive == true)
				availAttributeList.Add(info);
		}
		
		//유효한 리스트의 능력치들 추가 적용...
		if (attributeManager != null)
		{
			foreach(SetAttributeInfo info in availAttributeList)
			{
				foreach(AttributeValue _value in info.attributeList)
				{
					attributeManager.AddValue(_value.valueType, _value.Value);
				}
			}
		}
	}
}
