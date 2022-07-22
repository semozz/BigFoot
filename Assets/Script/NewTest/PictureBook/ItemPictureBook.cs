using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemPictureBookInfo
{
	public Item item = null;
	public bool isOpen = false;
	
	public ItemPictureBookInfo()
	{
		item = null;
		isOpen = false;
	}
	
	public static ItemPictureBookInfo CreateInfo(ItemInfo info)
	{
		ItemPictureBookInfo newInfo = new ItemPictureBookInfo();
		newInfo.item = Item.CreateItem(info.itemID, "", 0, 0, 1, -1, 0);
		if (newInfo.item != null)
		{
			//if (newInfo.item.itemInfo.buyPrice.z == 0.0f)
			//	newInfo.item.isTemplateItem = true;
			//else
			//	newInfo.item.isTemplateItem = false;
			
			newInfo.item.isTemplateItem = true;
		}
		
		newInfo.isOpen = false;
		
		return newInfo;
	}
}

public class ItemPictureBook 
{
	public List<ItemPictureBookInfo> weaponItems = new List<ItemPictureBookInfo>();
	public List<ItemPictureBookInfo> helmetItems = new List<ItemPictureBookInfo>();
	public List<ItemPictureBookInfo> armorItems = new List<ItemPictureBookInfo>();
	public List<ItemPictureBookInfo> handItems = new List<ItemPictureBookInfo>();
	public List<ItemPictureBookInfo> pantsItems = new List<ItemPictureBookInfo>();
	public List<ItemPictureBookInfo> bootsItems = new List<ItemPictureBookInfo>();
	public List<ItemPictureBookInfo> ringItems = new List<ItemPictureBookInfo>();
	public List<ItemPictureBookInfo> accessoryItems = new List<ItemPictureBookInfo>();
	
	public List<ItemPictureBookInfo> totalList = new List<ItemPictureBookInfo>();
	
	public void AddItemPictureInfo(ItemInfo.eItemType type, ItemPictureBookInfo info)
	{
		List<ItemPictureBookInfo> itemList = null;
		
		switch(type)
		{
		case ItemInfo.eItemType.Weapon:
			itemList = weaponItems;
			break;
		case ItemInfo.eItemType.Head:
			itemList = helmetItems;
			break;
		case ItemInfo.eItemType.Armor:
			itemList = armorItems;
			break;
		case ItemInfo.eItemType.Hand:
			itemList = handItems;
			break;
		case ItemInfo.eItemType.Pants:
			itemList = pantsItems;
			break;
		case ItemInfo.eItemType.Boots:
			itemList = bootsItems;
			break;
		case ItemInfo.eItemType.Ring:
			itemList = ringItems;
			break;
		case ItemInfo.eItemType.Accessories:
			itemList = accessoryItems;
			break;
		}
		
		if (itemList != null)
			itemList.Add(info);
		
		if (totalList != null)
			totalList.Add(info);
	}
	
	public void InitData ()
	{
		TableManager tableManager = TableManager.Instance;
		ItemTable itemTable = tableManager != null ? tableManager.itemTable : null;
		
		if (itemTable != null)
		{
			foreach(var temp in itemTable.dataList)
			{
				ItemInfo itemInfo = temp.Value;
				
				ItemPictureBookInfo newInfo = ItemPictureBookInfo.CreateInfo(itemInfo);
				
				AddItemPictureInfo(itemInfo.itemType, newInfo);
			}
		}
	}
	
	public void UpdateItemPictureBook(CharInfoData charData)
	{
		foreach(ItemPictureBookInfo temp in totalList)
			temp.isOpen = false;
		
		TableManager tableManager = TableManager.Instance;
		CharExpTable expTable = tableManager != null ? tableManager.charExpTable : null;
		
		if (charData != null)
		{
			long maxExp = 0;
			foreach(CharPrivateData data in charData.privateDatas)
			{
				if (data.baseInfo.ExpValue > maxExp)
					maxExp = data.baseInfo.ExpValue;
			}
			
			int maxLevel = expTable.GetLevel(maxExp);
			
			foreach(ItemPictureBookInfo pictureInfo in totalList)
			{
				if (pictureInfo != null && pictureInfo.item != null && pictureInfo.item.itemInfo != null)
				{
					pictureInfo.isOpen = (pictureInfo.item.itemInfo.equipLevel <= maxLevel);
				}
			}
		}
	}
}
