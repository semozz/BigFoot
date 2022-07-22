using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ItemSlotInfo
{
	public Transform trans = null;
	public GameDef.eSlotType slotType = GameDef.eSlotType.Common;
}

public class ItemSlotPanel : MonoBehaviour {
	public List<ItemSlotInfo> itemSlotPosList = new List<ItemSlotInfo>();
	//public GameObject itemSlotPrefab = null;
	public string itemSlotPrefabPath = "";
	
	public List<ItemSlot> itemSlots = new List<ItemSlot>();
	
	public Transform itemSlotNode = null;
	
	public string itemClickFuncName = "OnSelectItem";
	
	public GameObject parentObj = null;
	
	public GameDef.eItemSlotWindow slotWindow = GameDef.eItemSlotWindow.Inventory;
	public int startSlotIndex = 0;
	
	public string costumeSetPrefabPath = "";
	public CostumeSetItemPanel costumeSetPanel = null;
	public Transform costumeSetItemNode = null;
	public string costumeSetItemClickFuncName = "OnCostumeSetItem";
	
	void Awake()
	{
		itemSlots.Clear();
		
		int slotIndex = 0;
		GameObject itemSlotPrefab = ResourceManager.LoadPrefab(itemSlotPrefabPath);
		GameObject go = null;
		foreach(ItemSlotInfo info in itemSlotPosList)
		{
			go = null;
			
			//Vector3 origScale = Vector3.one;
			if (itemSlotPrefab != null)
			{
				//origScale = itemSlotPrefab.transform.localScale;
				go = (GameObject)Instantiate(itemSlotPrefab);
			}
			
			if (go != null)
			{
				go.transform.parent = itemSlotNode != null ? itemSlotNode : this.transform;
				
				go.transform.localPosition = info.trans.localPosition;
				go.transform.localScale = Vector3.one;
				go.transform.localRotation = Quaternion.identity;
					
				ItemSlot itemSlot = go.GetComponent<ItemSlot>();
				if (itemSlot != null)
				{
					if (itemSlot.buttonMsg != null)
					{
						itemSlot.buttonMsg.target = this.gameObject;
						itemSlot.buttonMsg.functionName = itemClickFuncName;
					}
					
					itemSlot.slotIndex = startSlotIndex + slotIndex;
					itemSlot.slotWindowType = slotWindow;
					itemSlot.slotType = info.slotType;
					
					itemSlot.SetItem(null);
					
					itemSlots.Add(itemSlot);
				}
			}
			
			++slotIndex;
		}
		
		if (costumeSetPrefabPath != "")
		{
			costumeSetPanel = ResourceManager.CreatePrefab<CostumeSetItemPanel>(costumeSetPrefabPath, costumeSetItemNode, Vector3.zero);
			
			if (costumeSetPanel != null)
			{
				costumeSetPanel.buttonMessage.target = this.gameObject;
				costumeSetPanel.buttonMessage.functionName = costumeSetItemClickFuncName;
				
				costumeSetPanel.slotIndex = 100;
				costumeSetPanel.slotWindowType = GameDef.eItemSlotWindow.Equip;
			}
		}
	}
	
	public void SetEffect(int slotIndex, GameObject effectObj)
	{
		int nCount = itemSlots.Count;
		if (slotIndex < 0 || slotIndex >= nCount)
			return;
		
		ItemSlot itemSlot = itemSlots[slotIndex];
		if (itemSlot != null)
		{
			effectObj.transform.position = itemSlot.transform.position;
			effectObj.SetActive(true);
		}
	}
	
	public void SetItem(int slotIndex, Item item)
	{
		int nCount = itemSlots.Count;
		if (slotIndex < 0 || slotIndex >= nCount)
			return;
		
		ItemSlot itemSlot = itemSlots[slotIndex];
		if (itemSlot != null)
			itemSlot.SetItem(item);
	}
	
	public Item GetItem(int slotIndex)
	{
		Item item = null;
		
		int nCount = itemSlots.Count;
		if (slotIndex < 0 || slotIndex >= nCount)
			return item;
		
		ItemSlot itemSlot = itemSlots[slotIndex];
		if (itemSlot != null)
			item = itemSlot.GetItem();
		
		return item;
	}
	
	/*
	public void SetItem(GameDef.eSlotType type, Item item)
	{
		ItemSlot itemSlot = GetItemSlot(type);
		
		if (itemSlot != null)
			itemSlot.SetItem(item);
	}
	*/
	public void SetEquipItems(List<EquipInfo> equipItems)
	{
		int index = 0;
		foreach(EquipInfo info in equipItems)
		{
			SetItem(index, info.item);
			++index;
		}
	}
	
	public ItemSlot GetItemSlot(GameDef.eSlotType type)
	{
		int nCount = itemSlots.Count;
		
		
		List<ItemSlot> slotList = new List<ItemSlot>();
		
		ItemSlot tempSlot = null;
		for (int index = 0; index < nCount; ++index)
		{
			tempSlot = itemSlots[index];
			
			if (tempSlot == null || tempSlot.slotType != type)
				continue;
			
			slotList.Add(tempSlot);
		}
		
		ItemSlot selectedSlot = null;
		nCount = slotList.Count;
		if (nCount == 1)
		{
			selectedSlot = slotList[0];
		}
		else if (nCount > 1)
		{
			for (int index = 0; index < nCount; ++index)
			{
				tempSlot = slotList[index];
				
				if (tempSlot.itemIcon != null && tempSlot.itemIcon.item == null)
				{
					selectedSlot = tempSlot;
					break;
				}
			}
			
			if (selectedSlot == null)
				selectedSlot = slotList[0];
		}
		
		return selectedSlot;
	}
	
	public void OnSelectItem(GameObject button)
	{
		if (parentObj != null)
		{
			parentObj.SendMessage(itemClickFuncName, button, SendMessageOptions.DontRequireReceiver);
		}
	}
	
	public void OnCostumeSetItem(GameObject obj)
	{
		if (parentObj != null)
		{
			parentObj.SendMessage(costumeSetItemClickFuncName, obj, SendMessageOptions.DontRequireReceiver);
		}
	}
	
	public void InitSelectedSlot()
	{
		int nCount = itemSlots.Count;
		for (int i = 0; i < nCount; ++i)
		{
			SetSelectedSlot(i, false);
		}
		
		SetSelectedSlot(100, false);
	}
	
	public void SetSelectedSlot(int slotIndex, bool bSelected)
	{
		ItemSlot itemSlot = null;
		int nCount = itemSlots.Count;
		if (slotIndex >= 0 && slotIndex < nCount)
			itemSlot = itemSlots[slotIndex];
		
		if (itemSlot != null)
			itemSlot.SetSelected(bSelected);
		else if (this.costumeSetPanel != null)
			this.costumeSetPanel.SetSelected(bSelected);
	}
	
	
	public bool isShowCostume = false;
	public void ActiveCostumeSlot(bool bActive)
	{
		isShowCostume = bActive;
		
		bool hasCostumeSet = false;
		if (costumeSetPanel != null)
			hasCostumeSet = costumeSetPanel.HasItem();
		
		bool costumeSetActive = bActive;
		if (hasCostumeSet == true)
			costumeSetActive = false;
		else
			costumeSetActive = bActive;
		
		int nCount = itemSlots.Count;
		for (int i = 0; i < nCount; ++i)
		{
			ItemSlot itemSlot = itemSlots[i];
			
			switch(itemSlot.slotType)
			{
			case GameDef.eSlotType.Costume_Body:
			case GameDef.eSlotType.Costume_Head:
			case GameDef.eSlotType.Costume_Back:
				itemSlot.gameObject.SetActive(costumeSetActive);
				break;
			default:
				itemSlot.gameObject.SetActive(!bActive);
				break;
			}
		}
		
		if (costumeSetPanel != null)
		{
			if (bActive == false)
				costumeSetPanel.gameObject.SetActive(bActive);
			else
				costumeSetPanel.gameObject.SetActive(!costumeSetActive);
		}
	}
	
	public void ResetItems()
	{
		int nCount = itemSlots.Count;
		for (int i = 0; i < nCount; ++i)
		{
			ItemSlot itemSlot = itemSlots[i];
			
			itemSlot.SetItem(null);
		}
	}
	
	public void RemoveCostumeSetItems(int costumeSetID, List<Item> costumeSetItems)
	{
		foreach(ItemSlot itemSlot in itemSlots)
		{
			Item item = null;
			if (itemSlot != null)
				item = itemSlot.GetItem();
			
			if (item != null && item.itemInfo != null)
			{
				if (item.itemInfo.costumeSetItemID == costumeSetID)
				{
					costumeSetItems.Add(item);
					
					itemSlot.SetItem(null);
				}
			}
		}
	}
	
	public Item FindItemByType(Item item)
	{
		Item findItem = null;
		
		if (item != null && item.itemInfo != null)
		{
			ItemInfo.eItemType itemType = item.itemInfo.itemType;
			
			foreach(ItemSlot itemSlot in itemSlots)
			{
				Item tempItem = itemSlot.itemIcon.item;
				if (tempItem == null || tempItem.itemInfo == null)
					continue;
				
				if (tempItem.itemInfo.itemType == itemType)
				{
					findItem = tempItem;
					break;
				}
			}
			
			if (findItem == null)
			{
				if (costumeSetPanel != null && costumeSetPanel.setItem != null)
				{
					foreach(Item temp in costumeSetPanel.setItem.items)
					{
						if (temp == null || temp.itemInfo == null)
							continue;
						
						if (temp.itemInfo.itemType == itemType)
						{
							findItem = temp;
							break;
						}
					}
				}
			}
		}
		
		return findItem;
	}
	
	public void SetLockSlot(int slotIndex, bool bLock)
	{
		ItemSlot itemSlot = null;
		int nCount = itemSlots.Count;
		if (slotIndex >= 0 && slotIndex < nCount)
			itemSlot = itemSlots[slotIndex];
		
		if (itemSlot != null)
			itemSlot.SetLock(bLock);
	}
}
