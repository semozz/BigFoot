using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemSlot : MonoBehaviour {
	public bool bLocked = false;
	public string lockedFrame = "icon_unable";
	public string nullFrame = "icon_frame_nothing";
	
	public int slotIndex = 0;
	public GameDef.eItemSlotWindow slotWindowType = GameDef.eItemSlotWindow.Inventory;
	public GameDef.eSlotType slotType = GameDef.eSlotType.Common;
	
	public UISprite frame = null;
	public ItemIcon itemIcon = null;
	public UILabel itemName = null;
	public UILabel itemCountLabel = null;
	public UILabel itemLevel = null;
	
	public UIButtonMessage buttonMsg = null;
	
	public UISprite priceFrame = null;
	public UISprite goldType = null;
	public UILabel priceLabel = null;
	
	public UISprite selectedFrame = null;
	public UISprite levelFrame = null;
	public string frameSpriteName = null;
	public string arenaFrameSpriteName = null;
	
	public string newItemPrefab = "UI/New_Town_Item";
	public GameObject newItemBadge = null;
	public Transform badgeNode = null;
	public void SetSelected(bool bSelected)
	{
		if (selectedFrame != null)
			selectedFrame.gameObject.SetActive(bSelected);
	}
	
	public void SetMaterial(ReinforceMaterial material)
	{
		SetItem(material != null ? material.item : null);
		
		if (itemIcon != null && itemIcon.icon != null)
		{
			Color color = Color.white;
			if (material == null || material.bCheck == false)
				color = Color.gray;
			
			itemIcon.icon.color = color;
		}
	}
	
	public void SetItem(Item item)
	{
		if (bLocked == true)
			return;
		
		if (bLocked == false && itemIcon != null)
		{
			itemIcon.SetSlotType(slotType);
			itemIcon.SetItem(item);
		}
		
		UpdateFrame(item);
		UpdateItemName(item);
		
		UpdateNewBadge(item);
	}
	
	public void UpdateNewBadge(Item item)
	{
		bool availableBadge = false;
		switch(slotWindowType)
		{
		case GameDef.eItemSlotWindow.Inventory:
		case GameDef.eItemSlotWindow.Costume:
		case GameDef.eItemSlotWindow.MaterialItem:
		case GameDef.eItemSlotWindow.CostumeSet:
			availableBadge = true;
			break;
		}
		
		if (availableBadge == false)
		{
			ClearBadge();
			return;
		}
		
		bool isNew = false;
		if (item != null)
			isNew = item.IsNewItem;
		
		if (isNew == true)
			SetNewBadge();
		else
			ClearBadge();
	}
	
	public void SetNewBadge()
	{
		if (newItemBadge == null)
			newItemBadge = ResourceManager.CreatePrefab(newItemPrefab, this.badgeNode);
	}
	
	public void ClearBadge()
	{
		if (newItemBadge != null)
		{
			DestroyObject(newItemBadge, 0.2f);
			newItemBadge = null;
		}
	}
	
	public Item GetItem()
	{
		return itemIcon != null ? itemIcon.item : null;
	}
	
	public void UpdateItemName(Item item)
	{
		bool countLabelActive = true;
		if (bLocked == true)
			countLabelActive = false;
		else
		{
			if (item == null || item.itemInfo == null)
				countLabelActive = false;
			else
			{
				if (item.itemInfo.maxStackCount > 1 && item.itemCount > 1)
					countLabelActive = true;
				else
					countLabelActive = false;
			}
		}
		if (itemCountLabel != null)
		{
			itemCountLabel.gameObject.SetActive(countLabelActive);
			
			int itemCount = item != null ? item.itemCount : 0;
			if (countLabelActive == true)
				itemCountLabel.text = string.Format("{0}", itemCount);
		}
		if (itemLevel != null && levelFrame != null)
		{
			int level = 0;
			if(item != null && item.itemInfo != null)
				level = item.itemInfo.itemLevel;
			itemLevel.gameObject.SetActive(level > 0);
			levelFrame.gameObject.SetActive(level > 0);
			itemLevel.text = string.Format("{0}", level);
		}
		
		bool isNameLabelActive = bLocked == true ? false : item != null;
		
		bool isPriceSpriteActive = false;
		switch(slotWindowType)
		{
		//case GameDef.eItemSlotWindow.Costume:
		//case GameDef.eItemSlotWindow.Inventory:
		case GameDef.eItemSlotWindow.Shop_Normal:
		case GameDef.eItemSlotWindow.Shop_Costume:
		case GameDef.eItemSlotWindow.Shop_Cash:
			isPriceSpriteActive = true;
			break;
		case GameDef.eItemSlotWindow.StageReward:
		//case GameDef.eItemSlotWindow.StageReward_List:
		case GameDef.eItemSlotWindow.Reinforce_Material:
			isPriceSpriteActive = false;
			isNameLabelActive = true;
			break;
		default:
			isPriceSpriteActive = false;
			isNameLabelActive = false;
			break;
		}
		if (priceFrame != null)
			priceFrame.gameObject.SetActive(isPriceSpriteActive);
		
		
		Vector3 itemPrice = Vector3.zero;
		if (priceLabel != null)
		{
			priceLabel.gameObject.SetActive(isPriceSpriteActive);
			
			if (item != null && item.itemInfo != null && isPriceSpriteActive == true)
			{
				switch(slotWindowType)
				{
				case GameDef.eItemSlotWindow.Shop_Normal:
				case GameDef.eItemSlotWindow.Shop_Costume:
				case GameDef.eItemSlotWindow.Shop_Cash:
					itemPrice = item.itemInfo.buyPrice * item.itemCount;
					//priceLabel.text = string.Format("{0:#,###,###}", item.itemInfo.buyPrice);
					break;
				case GameDef.eItemSlotWindow.Inventory:
				case GameDef.eItemSlotWindow.Costume:
					itemPrice = item.itemInfo.sellPrice * item.itemCount;
					//priceLabel.text = string.Format("{0:#,###,###}", item.itemInfo.sellPrice);
					break;
				default:
					itemPrice = Vector3.zero;
					//priceLabel.text = "";
					break;
				}
			}
			else
			{
				//priceLabel.text = "";
				itemPrice = Vector3.zero;
			}
			
			float price = 0.0f;
			if (itemPrice.x > 0.0f)
				price = itemPrice.x;
			else if (itemPrice.y > 0.0f)
				price = itemPrice.y;
			else if (itemPrice.z > 0.0)
				price = itemPrice.z;
			
			priceLabel.text = string.Format("{0:#,###,###}", price);
		}
		
		if (goldType != null)
		{
			if (item == null || item.itemInfo == null)
				isPriceSpriteActive = false;
			
			goldType.gameObject.SetActive(isPriceSpriteActive);
			
			string spriteName = "Shop_Money01";
			if (itemPrice.x > 0.0f)
			{
				spriteName = "Shop_Money01";
			}
			else if (itemPrice.y > 0.0f)
			{
				spriteName = "Shop_Money02";
			}
			else if (itemPrice.z > 0.0)
			{
				spriteName = "Shop_Money03";
			}
			goldType.spriteName = spriteName;
		}
		
		if (itemName != null)
		{
			itemName.gameObject.SetActive(isNameLabelActive);
			switch(slotWindowType)
			{
			case GameDef.eItemSlotWindow.StageReward:
			case GameDef.eItemSlotWindow.Reinforce_Material:
				if (item != null && item.itemInfo != null)
					itemName.text = item.GetItemName();
				else
					itemName.text = "";
				break;
			default:
				itemName.text = "";
				break;
			}
		}
	}
	
	public void UpdateFrame(Item item)
	{
		string frameName = "";
		bool bActive = true;
		
		if (bLocked == true)
		{
			frameName = lockedFrame;
		}
		else
		{
			if (item == null)
				frameName = nullFrame;
			else
			{
				frameName = "";
				bActive = false;
			}
		}
		
		if (frame != null)
		{
			frame.gameObject.SetActive(bActive);
			frame.spriteName = frameName;
		}
		
		UpdateArena(item);
	}
	
	public void UpdateArena(Item item)
	{
		bool isArena = false;
		if (item != null)
			isArena = item.CheckArenaItem();
		
		if (levelFrame != null)
			levelFrame.spriteName = isArena ? arenaFrameSpriteName: frameSpriteName;
	}
	
	/*
	public List<Item> testItemList = new List<Item>();
	void Start()
	{
		Item newItem = null;
		ItemInfo itemInfo = null;
		
		newItem = new Item();
		itemInfo = new ItemInfo();
		itemInfo.itemName = "Test1";
		itemInfo.itemIcon = "item_body_att_01";
		newItem.itemInfo = itemInfo;
		newItem.itemGrade = Item.eItemGrade.Normal;
		newItem.reinforceStep = Random.Range(0, 6);
		
		testItemList.Add(newItem);
		
		newItem = new Item();
		itemInfo = new ItemInfo();
		itemInfo.itemName = "Test2";
		itemInfo.itemIcon = "item_body_def_01";
		newItem.itemInfo = itemInfo;
		newItem.itemGrade = Item.eItemGrade.High;
		newItem.reinforceStep = Random.Range(0, 6);
		
		testItemList.Add(newItem);
		
		newItem = new Item();
		itemInfo = new ItemInfo();
		itemInfo.itemName = "Test1";
		itemInfo.itemIcon = "item_hand_att_01";
		newItem.itemInfo = itemInfo;
		newItem.itemGrade = Item.eItemGrade.Normal;
		newItem.reinforceStep = Random.Range(0, 6);
		
		testItemList.Add(newItem);
		
		newItem = new Item();
		itemInfo = new ItemInfo();
		itemInfo.itemName = "Test1";
		itemInfo.itemIcon = "item_body_mag_01";
		newItem.itemInfo = itemInfo;
		newItem.itemGrade = Item.eItemGrade.High;
		newItem.reinforceStep = Random.Range(0, 6);
		
		testItemList.Add(newItem);
		
		newItem = new Item();
		itemInfo = new ItemInfo();
		itemInfo.itemName = "Test1";
		itemInfo.itemIcon = "item_hand_def_01";
		newItem.itemInfo = itemInfo;
		newItem.itemGrade = Item.eItemGrade.Normal;
		newItem.reinforceStep = Random.Range(0, 6);
		
		testItemList.Add(newItem);
		
		testItemList.Add(null);
	}
	
	public float delayTime = 1.0f;
	public void Update()
	{
		if (delayTime <= 0.0f)
		{
			int nCount = testItemList.Count;
			int ranIndex = Random.Range(0, nCount);
			
			Item tempItem = testItemList[ranIndex];
			
			SetItem(tempItem);
			
			delayTime = 1.0f;
		}
		
		delayTime -= Time.deltaTime;
	}
	*/
	
	public void SetLock(bool bLock)
	{
		this.bLocked = bLock;
		
		UpdateFrame(null);
		UpdateItemName(null);
	}
	
	public void SetButtonActive(bool bActive)
	{
		buttonMsg.gameObject.GetComponent<BoxCollider>().enabled = bActive;
	}
}
