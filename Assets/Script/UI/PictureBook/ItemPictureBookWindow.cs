using UnityEngine;
using System.Collections;

public class ItemPictureBookWindow : BasePictureBookWindow {
	
	public void SetPictureBookInfo(ItemPictureBook itemPictureBook)
	{
		BasePictureBookListWindow listWindow = GetWindow(BasePictureBookListWindow.ePictureBookListType.Item_Weapon);
		if (listWindow != null)
			listWindow.SetInfos(itemPictureBook.weaponItems);
		
		listWindow = GetWindow(BasePictureBookListWindow.ePictureBookListType.Item_Helmet);
		if (listWindow != null)
			listWindow.SetInfos(itemPictureBook.helmetItems);
		
		listWindow = GetWindow(BasePictureBookListWindow.ePictureBookListType.Item_Armor);
		if (listWindow != null)
			listWindow.SetInfos(itemPictureBook.armorItems);
		
		listWindow = GetWindow(BasePictureBookListWindow.ePictureBookListType.Item_Hand);
		if (listWindow != null)
			listWindow.SetInfos(itemPictureBook.handItems);
		
		listWindow = GetWindow(BasePictureBookListWindow.ePictureBookListType.Item_Pants);
		if (listWindow != null)
			listWindow.SetInfos(itemPictureBook.pantsItems);
		
		listWindow = GetWindow(BasePictureBookListWindow.ePictureBookListType.Item_Boots);
		if (listWindow != null)
			listWindow.SetInfos(itemPictureBook.bootsItems);
		
		listWindow = GetWindow(BasePictureBookListWindow.ePictureBookListType.Item_Ring);
		if (listWindow != null)
			listWindow.SetInfos(itemPictureBook.ringItems);
		
		listWindow = GetWindow(BasePictureBookListWindow.ePictureBookListType.Item_Accessory);
		if (listWindow != null)
			listWindow.SetInfos(itemPictureBook.accessoryItems);
	}
}
