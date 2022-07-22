using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemPictureBookPanel : MonoBehaviour {
	public BasePictureBookListWindow parentWindow = null;
	public GameObject lockObj = null;
	
	public ItemPictureBookInfo itemPictureInfo = null;
	
	public ItemInfoPage itemInfoPage = null;
	
	public string itemSlotPrefabPath = "";
	public Transform slotNode = null;
	public ItemSlot itemSlot = null;
	
	void Awake()
	{
		itemSlot = ResourceManager.CreatePrefab<ItemSlot>(itemSlotPrefabPath, slotNode, Vector3.zero);
		if (itemSlot != null)
			itemSlot.slotWindowType = GameDef.eItemSlotWindow.Special;
	}
	
	public void SetInfo(ItemPictureBookInfo info)
	{
		itemPictureInfo = info;
		
		bool isOpen = false;
		Item item = null;
		if (itemPictureInfo != null)
		{
			isOpen = itemPictureInfo.isOpen;
			item = itemPictureInfo.item;
		}
		
		if (itemInfoPage != null)
			itemInfoPage.SetItem(item);
		
		if (itemSlot != null)
		{
			itemSlot.SetItem(item);
			
			itemSlot.gameObject.SetActive(isOpen);
		}
		
		if (lockObj != null)
			lockObj.SetActive(!isOpen);
	}
}
