using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InvenItemInfo
{
	public int slotIndex = -1;
	public Item item = null;
}

public class InventoryManager : MonoBehaviour
{
	public delegate void OnItemChange(int slotIndex, Item _item);
	public OnItemChange onItemChanged = null;
	public OnItemChange onCostumeChanged = null;
	
	public List<Item> itemList = new List<Item>();
	public void SetInvenItemData(List<Item> list)
	{
		itemList.Clear();
		
		foreach(Item item in list)
		{
			itemList.Add(item);
		}
		
		//this.RerangeItem(itemList);
	}
	
	public List<Item> costumeList = new List<Item>();
	public void SetInvenCostumeData(List<Item> list)
	{
		costumeList.Clear();
		
		foreach(Item item in list)
		{
			costumeList.Add(item);
		}
		
		//this.RerangeCostume(costumeList);
	}
	
	public List<Item> materialItemList = new List<Item>();
	public void SetMaterialItemData(List<Item> list)
	{
		materialItemList.Clear();
		
		foreach(Item item in list)
		{
			materialItemList.Add(item);
		}
	}
	
	public List<CostumeSetItem> costumeSetList = new List<CostumeSetItem>();
	public void SetCostumeSetItems(List<CostumeSetItem> list)
	{
		costumeSetList.Clear();
		costumeSetList.AddRange(list);
	}
	
	public int baseCapacity = 9;
	public int addCapacity = 0;
	public int AddCapacity
	{
		get { return addCapacity; }
		set
		{
			addCapacity = value;
			OnChangeCapacity();
		}
	}
	
	public int Capacity
	{
		get { return baseCapacity + addCapacity; }
	}
	
	void Awake()
	{
		int initCapacity = Capacity;
		for (int index = 0; index < initCapacity; ++index)
		{
			itemList.Add(null);
		}
	}
	
	public void OnChangeCapacity()
	{
		int newCap = Capacity;
		int curCap = itemList.Count;
		
		if (curCap < newCap)
		{
			int nCount = newCap - curCap;
			for (int i = 0; i < nCount; ++i)
				itemList.Add(null);
		}
		else if (curCap > newCap)
		{
			int nCount = curCap - newCap;
			for (int i = 0; i < nCount; ++i)
				itemList.RemoveAt(newCap);
		}
	}
	
	public Item GetItem(int slotIndex)
	{
		Item item = null;
		int nCount = itemList.Count;
		if (slotIndex < 0 || slotIndex >= nCount)
			item = null;
		else
			item = itemList[slotIndex];
		
		return item;
	}
	
	public Item GetItem(int slotIndex, GameDef.eItemSlotWindow windowType)
	{
		List<Item> tempList = null;
		
		switch(windowType)
		{
		case GameDef.eItemSlotWindow.Inventory:
			tempList = itemList;
			break;
		case GameDef.eItemSlotWindow.MaterialItem:
			tempList = materialItemList;
			break;
		case GameDef.eItemSlotWindow.Costume:
			tempList = costumeList;
			break;
		}
		
		Item item = null;
		
		if (tempList != null)
		{
			int nCount = tempList.Count;
			if (slotIndex < 0 || slotIndex >= nCount)
				item = null;
			else
				item = tempList[slotIndex];
		}
		
		return item;
	}
	
	public CostumeSetItem GetCostumeSetItem(int slotIndex, GameDef.eItemSlotWindow windowType)
	{
		List<CostumeSetItem> tempList = null;
		switch(windowType)
		{
		case GameDef.eItemSlotWindow.CostumeSet:
			tempList = costumeSetList;
			break;
		}
		
		CostumeSetItem item = null;
		
		if (tempList != null)
		{
			int nCount = tempList.Count;
			if (slotIndex < 0 || slotIndex >= nCount)
				item = null;
			else
				item = tempList[slotIndex];
		}
		
		return item;
	}
	
	public Item GetItemByUID(string UID)
	{
		int slot = GetItemIndexByUID(UID, itemList);
		
		return GetItem (slot);
	}

	public void SetItem(int slotIndex, Item _item)
	{
		int nCount = itemList.Count;
		if (slotIndex < 0 || slotIndex >= nCount)
			return;
		
		itemList[slotIndex] = _item;
		
		if (onItemChanged != null)
			onItemChanged(slotIndex, _item);
	}
	
	public void RemoveItem(int slotIndex)
	{
		int nCount = itemList.Count;
		if (slotIndex < 0 || slotIndex >= nCount)
			return;
		
		Item oldItem = itemList[slotIndex];
		itemList[slotIndex] = null;
		
		if (onItemChanged != null)
			onItemChanged(slotIndex, oldItem);
	}
	
	public void RemoveItem(int slotIndex, List<Item> tempList)
	{
		int nCount = tempList.Count;
		if (slotIndex < 0 || slotIndex >= nCount)
			return;
		
		Item oldItem = tempList[slotIndex];
		tempList[slotIndex] = null;
		
		if (onItemChanged != null)
			onItemChanged(slotIndex, oldItem);
	}
	
	public void RemoveItemByUID(string uID)
	{
		int slotIndex = GetItemIndexByUID(uID, itemList);
		
		RemoveItem (slotIndex);
	}
	
	public void SetCostume(int slotIndex, Item _item)
	{
		int nCount = costumeList.Count;
		if (slotIndex < 0 || slotIndex >= nCount)
			return;
		
		costumeList[slotIndex] = _item;
		
		if (onCostumeChanged != null)
			onCostumeChanged(slotIndex, _item);
	}
	
	public Item GetCostumeItem(int slotIndex)
	{
		Item item = null;
		
		int nCount = costumeList.Count;
		if (slotIndex < 0 || slotIndex >= nCount)
			return item;
		
		item = costumeList[slotIndex];
		return item;
	}
	
	public void RemoveCostume(int slotIndex)
	{
		int nCount = costumeList.Count;
		if (slotIndex < 0 || slotIndex >= nCount)
			return;
		
		Item oldItem = costumeList[slotIndex];
		costumeList[slotIndex] = null;
		
		if (onCostumeChanged != null)
			onCostumeChanged(slotIndex, oldItem);
	}
	
	public void AddCostume(Item item)
	{
		costumeList.Add(item);
	}
	
	public int GetItemIndexByUID(string uID, List<Item> itemList)
	{
		int itemIndex = -1;
		int nCount = itemList.Count;
		for (int index = 0; index < nCount; ++index)
		{
			Item item = itemList[index];
			
			if (item != null && item.uID == uID)
			{
				itemIndex = index;
				break;
			}
		}
		return itemIndex;
	}
	
	public void AddItem(Item item)
	{
		int itemIndex = GetItemIndexByUID(item.uID, this.itemList);
		if (itemIndex == -1)
		{
			itemList.Add(null);
			SetItem(itemList.Count - 1, item);
		}
		else
		{
			SetItem(itemIndex, item);
		}
		
		/*
		List<InvenItemInfo> invenItemList = GetInvenItemList(item);
		if (invenItemList == null || invenItemList.Count == 0)
		{
			int emptySlotIndex = GetEmptySlotIndex(item);
			if (emptySlotIndex == -1)
			{
				itemList.Add(null);
				
				emptySlotIndex = itemList.Count - 1;
			}
			
			if (emptySlotIndex != -1)
			{
				SetItem(emptySlotIndex, item);
			}
		}
		else
		{
			int maxStack = 1;
			if (item != null && item.itemInfo != null)
				maxStack = item.itemInfo.maxStackCount;
			
			int curTotalCount = item != null ? item.itemCount : 0;
			foreach(InvenItemInfo info in invenItemList)
			{
				if (info != null && info.item != null)
					curTotalCount += info.item.itemCount;
			}
			
			int newCount = 0;
			foreach(InvenItemInfo info in invenItemList)
			{
				if (info == null || info.item == null)
					continue;
				
				if (curTotalCount >= maxStack)
				{
					newCount = maxStack;
					curTotalCount -= maxStack;
				}
				else
				{
					newCount = curTotalCount;
					curTotalCount = 0;
				}
				
				info.item.itemCount = newCount;
				if (newCount == 0)
					SetItem(info.slotIndex, null);
				else
					SetItem(info.slotIndex, info.item);
			}
			
			
			while(curTotalCount > 0)
			{
				int emptySlotIndex = GetEmptySlotIndex(item);
				if (emptySlotIndex == -1)
				{
					itemList.Add(null);
					
					emptySlotIndex = itemList.Count - 1;
				}
				
				if (emptySlotIndex != -1)
				{
					Item restItem = Item.CreateItem(item);
					
					newCount = 0;
					if (curTotalCount >= maxStack)
					{
						newCount = maxStack;
						curTotalCount -= maxStack;
					}
					else
					{
						newCount = curTotalCount;
						curTotalCount = 0;
					}
					
					restItem.itemCount = newCount;
					
					SetItem(emptySlotIndex, restItem);
				}
			}
		}
		*/
	}
	
	public void UpdateItemCount(string UID, int ItemCount)
	{
		Item item = GetItemByUID(UID);
		
		if (item == null)
		{
			Debug.Log("UpdateItemCount Failed");
			return;
		}
		
		item.itemCount = ItemCount;
	}
	
	public List<InvenItemInfo> GetInvenItemList(Item item)
	{
		List<InvenItemInfo> invenItemList = new List<InvenItemInfo>();
		
		if (item == null || item.itemInfo == null)
			return invenItemList;
		
		int nCount = itemList.Count;
		Item oldItem = null;
		for (int index = 0; index < nCount; ++index)
		{
			oldItem = itemList[index];
			
			if (oldItem == null || oldItem.itemInfo == null)
				continue;
			
			if (oldItem.itemInfo.itemID != item.itemInfo.itemID)
				continue;
			
			if (oldItem.itemGrade != item.itemGrade || oldItem.reinforceStep != item.reinforceStep)
				continue;
			
			if (oldItem.itemCount >= oldItem.itemInfo.maxStackCount)
				continue;
			
			InvenItemInfo invenInfo = new InvenItemInfo();
			invenInfo.slotIndex = index;
			invenInfo.item = oldItem;
			
			invenItemList.Add(invenInfo);
		}
		
		return invenItemList;
	}
	
	public int GetEmptySlotIndex(Item item)
	{
		int equpIndex = -1;
		int nCount = itemList.Count;
		
		Item oldItem = null;
		for (int index = 0; index < nCount; ++index)
		{
			oldItem = itemList[index];
			
			if (oldItem == null || oldItem.itemInfo == null)
			{
				equpIndex = index;
				break;
			}
		}
		
		return equpIndex;
	}
	
	/*
	public void RerangeItem()
	{
		itemList.Sort(SortByNull);
	}
	
	public void RerangeCostume()
	{
		this.costumeList.Sort(SortByNull);
	}
	*/
	
	public bool CheckMaterial(ReinforceMaterialInfo info)
	{
		bool bCheck = false;
		int needItemCount = info.nItemCount;
		
		foreach(Item item in this.materialItemList)
		{
			if (item == null || item.itemInfo == null)
				continue;
			
			if (item.itemInfo.itemID == info.nItemID)
			{
				needItemCount -= item.itemCount;
			}
		}
		
		if (needItemCount <= 0)
			bCheck = true;
		
		return bCheck;
	}
	
	public void RemoveMaterial(ReinforceMaterial material)
	{
	
		if (material == null || material.item == null || material.item.itemInfo == null)
			return;
		
		int totalCount = material.item.itemCount;
		
		List<int> removeIndexList = new List<int>();
		
		int nCount = materialItemList.Count;
		for (int index = 0; index < nCount; ++index)
		{
			Item item = materialItemList[index];
			
			if (item == null || item.itemInfo == null)
				continue;
			
			if (item.itemInfo.itemID == material.item.itemInfo.itemID)
			{
				int curCount = item.itemCount;
				int deleteCount = Mathf.Min(curCount, totalCount);
				
				item.itemCount -= deleteCount;
				totalCount -= deleteCount;
				
				if (item.itemCount <= 0)
					removeIndexList.Add(index);
				
				if (totalCount == 0)
					break;
			}
		}
		
		foreach(int removeIndex in removeIndexList)
		{
			SetItem(removeIndex, null);
		}
	}
	
	public void GetCompositionMaterials(Item composItem, List<CompositionMaterialInfo> materials)
	{
		if (composItem == null || composItem.itemInfo == null)
			return;
		
		int nCount = this.itemList.Count;
		for (int index = 0; index < nCount; ++index)
		{
			Item item = this.itemList[index];
			
			if (item == null || item.itemInfo == null)
				continue;
			
			if (item == composItem)
				continue;
			
			if (item.itemInfo.itemID != composItem.itemInfo.itemID ||
				item.itemGrade != composItem.itemGrade)
				continue;
			
			CompositionMaterialInfo info = new CompositionMaterialInfo();
			info.slotIndex = index;
			info.item = item;
			
			materials.Add(info);
		}
	}
	
	public void RemoveCompositionMaterial(int slotIndex, Item materialItem)
	{
		int nCount = materialItemList.Count;
		
		if (materialItem == null ||
			(slotIndex < 0 || slotIndex >= nCount))
			return;
		
		Item item = materialItemList[slotIndex];
		
		if (item == materialItem)
		{
			item.itemCount = item.itemCount - 1;
			
			if (item.itemCount <= 0)
				RemoveMaterialItem(slotIndex);
		}
	}
	
	public void RemoveMaterialItem(int slotIndex)
	{
		RemoveItem (slotIndex, materialItemList);
	}
	
	public void GetCompositionAddMaterials(Item composItem, List<CompositionMaterialInfo> materialsList)
	{
		if (composItem == null || composItem.itemInfo == null)
			return;
		
		int nCount = materialItemList.Count;
		for (int index = 0; index < nCount; ++index)
		{
			Item item = materialItemList[index];
			
			if (item == null || item.itemInfo == null)
				continue;
			
			if (item.itemInfo.itemType != ItemInfo.eItemType.Material_Compose)
				continue;
			
			CompositionMaterialInfo info = new CompositionMaterialInfo();
			info.slotIndex = index;
			info.item = item;
			
			materialsList.Add(info);
		}
	}
}
