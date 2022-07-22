using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CostumeSetItemPanel : MonoBehaviour {
	public UITexture setTexture = null;
	public UILabel nameLabel = null;
	
	public string goldSprite = "Shop_Money01";
	public string jewelSprite = "Shop_Money02";
	public string medalSprite = "Shop_Money03";
	public UILabel goldInfoLabel = null;
	public UISprite goldType = null;
	
	public Transform setItemNode = null;
	
	public string itemPrefabPath = "UI/Item/ItemSlot";
	public List<ItemSlotInfo> itemSlotPosList = new List<ItemSlotInfo>();
	
	public List<ItemSlot> itemSlots = new List<ItemSlot>();
	
	public GameDef.eItemSlotWindow slotWindowType = GameDef.eItemSlotWindow.Inventory;
	public int slotIndex = -1;
	
	public UIButtonMessage buttonMessage = null;
	
	public UISprite selectedFrame = null;
	public bool isReverseSelected = true;
	public void SetSelected(bool bSelected)
	{
		bool bActive = bSelected;
		if (isReverseSelected == true)
			bActive = !bActive;
		
		if (selectedFrame != null)
			selectedFrame.gameObject.SetActive(bActive);
	}
	
	void Awake()
	{
		foreach(ItemSlotInfo info in itemSlotPosList)
		{
			ItemSlot itemSlot = ResourceManager.CreatePrefab<ItemSlot>(itemPrefabPath, setItemNode, info.trans.localPosition);
			if (itemSlot != null)
			{
				itemSlot.slotWindowType = GameDef.eItemSlotWindow.Reinforce_Item;
				itemSlot.slotType = info.slotType;
				
				itemSlot.buttonMsg.collider.enabled = false;
				
				itemSlot.SetItem(null);
			}
			
			itemSlots.Add(itemSlot);
		}
	}
	
	public string path = "IMG/UI/CostumeSetImage/";
	public Texture LoadTexture(string textureName)
	{
		string pathStr = string.Format("{0}{1}", path, textureName);
		Texture texture = (Texture2D)ResourceManager.LoadTexture(pathStr);
		
		return texture;
	}
	
	
	public CostumeSetItem setItem = null;
	public string defaultTextureName = "";
	public void SetCostumeItem(CostumeSetItem costumeSetItem)
	{
		setItem = costumeSetItem;
		
		//this.gameObject.SetActive(setItem != null);
		
		string textureName = defaultTextureName;
		string setName = "";
		Vector3 goldPrice = Vector3.zero;
		
		List<Item> items = new List<Item>();
		
		if (setItem != null && setItem.setItemInfo != null)
		{
			textureName = setItem.setItemInfo.setSpriteName;
			setName = setItem.setItemInfo.setName;
			
			if (slotWindowType == GameDef.eItemSlotWindow.Shop_CostumeSet)
				goldPrice = setItem.setItemInfo.buyPrice;
			else
				goldPrice = setItem.setItemInfo.sellPrice;
			
			items.AddRange(setItem.items);
		}
		
		int slotCount = itemSlots.Count;
		for (int index = 0; index < slotCount; ++index)
		{
			ItemSlot itemSlot = itemSlots[index];
			
			Item item = GetItem(items, index);
			itemSlot.SetItem(item);
			itemSlot.slotWindowType = this.slotWindowType;
		}
		
		if (setTexture != null)
			setTexture.mainTexture = LoadTexture(textureName);
		if (nameLabel != null)
			nameLabel.text = setName;
		
		SetGoldInfo(goldPrice);
		
	}
	
	public void SetGoldInfo(Vector3 price)
	{
		float priceValue = 0.0f;
		string spriteName = goldSprite;
		if (price.x > 0.0f)
		{
			priceValue = price.x;
			spriteName = goldSprite;
		}
		else if (price.y > 0.0f)
		{
			priceValue = price.y;
			spriteName = jewelSprite;
		}
		else if (price.z > 0.0f)
		{
			priceValue = price.z;
			spriteName = medalSprite;
		}
		
		if (goldType != null)
			goldType.spriteName = spriteName;
		if (goldInfoLabel != null)
			goldInfoLabel.text = string.Format("{0}", (int)priceValue);
	}
	
	public Item GetItem(List<Item> items, int index)
	{
		Item item = null;
		int nCount = items.Count;
		if (index >= 0 && index < nCount)
			item = items[index];
		
		return item;
	}
	
	public bool HasItem()
	{
		bool hasItem = false;
		
		int slotCount = itemSlots.Count;
		for (int index = 0; index < slotCount; ++index)
		{
			ItemSlot itemSlot = itemSlots[index];
			
			if (itemSlot != null)
			{
				Item item = itemSlot.GetItem();
				if (item != null)
				{
					hasItem = true;
					break;
				}
			}
		}
		
		return hasItem;
	}
}
