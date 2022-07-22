using UnityEngine;
using System.Collections;

public class PictureBookWindow : PopupBaseWindow {
	public TownUI townUI = null;
	
	public TabButtonController tabButtonController = null;
	
	public override void Awake()
	{
		this.windowType = TownUI.eTOWN_UI_TYPE.PICTUREBOOK;
		
		InitWindow();
	}
	
	public void InitWindow()
	{
		CharInfoData charData = Game.Instance.charInfoData;
		CharPrivateData privateData = null;
		int charIndex = 0;
		if (Game.Instance.connector != null)
			charIndex = Game.Instance.connector.charIndex;
		
		MonsterPictureBook monsterPictureBook = null;
		ItemPictureBook itemPictureBook = null;
		if (charData != null)
		{
			monsterPictureBook = charData.monsterPictureBook;
			itemPictureBook = charData.itemPictureBook;
			
			privateData = charData.GetPrivateData(charIndex);
		}
		
		if (itemPictureBook != null && charData != null)
			itemPictureBook.UpdateItemPictureBook(charData);
		
		if (monsterPictureBook != null && privateData != null)
			monsterPictureBook.UpdateMonsterPictureBook(charData);
		
		MonsterPictureBookWindow monsterPictureBookWindow = null;
		ItemPictureBookWindow itemPictureBookWindow = null;
		
		BasePictureBookWindow pictureBook = GetPictureBook(BasePictureBookWindow.ePictureBookType.MonsterPictureBook);
		if (pictureBook != null)
			monsterPictureBookWindow = (MonsterPictureBookWindow)pictureBook;
		
		if (monsterPictureBookWindow != null && monsterPictureBook != null)
			monsterPictureBookWindow.SetPictureBookInfo(monsterPictureBook);
		
		pictureBook = GetPictureBook(BasePictureBookWindow.ePictureBookType.ItemPictureBook);
		if (pictureBook != null)
			itemPictureBookWindow = (ItemPictureBookWindow)pictureBook;
		if (itemPictureBookWindow != null)
			itemPictureBookWindow.SetPictureBookInfo(itemPictureBook);
	}
	
	public BasePictureBookWindow GetPictureBook(BasePictureBookWindow.ePictureBookType type)
	{
		BasePictureBookWindow baseWindow = null;
		if (tabButtonController != null)
		{
			foreach(TabButtonInfo info in tabButtonController.tabInfos)
			{
				BasePictureBookWindow tempWindow = info.viewWindow != null ? info.viewWindow.GetComponent<BasePictureBookWindow>() : null;
				if (tempWindow != null && tempWindow.type == type)
				{
					baseWindow = tempWindow;
					
					if (baseWindow != null)
						baseWindow.parentWindow = this.gameObject;
					
					break;
				}
			}
		}
		
		return baseWindow;
	}
	
	public override void OnBack ()
	{
		base.OnBack();
		
		this.gameObject.SetActive(false);
		
		foreach(TabButtonInfo info in tabButtonController.tabInfos)
		{
			BasePictureBookWindow tempWindow = info.viewWindow != null ? info.viewWindow.GetComponent<BasePictureBookWindow>() : null;
			if (tempWindow != null)
				tempWindow.InitList();
		}
	}
}
