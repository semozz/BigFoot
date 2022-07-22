using UnityEngine;
using System.Collections;

public class BasePictureBookWindow : MonoBehaviour {
	public enum ePictureBookType
	{
		MonsterPictureBook,
		ItemPictureBook,
	}
	public ePictureBookType type = ePictureBookType.MonsterPictureBook;
	
	public GameObject parentWindow = null;
	
	public TabButtonController tabButtonController = null;
	
	public BasePictureBookListWindow GetWindow(BasePictureBookListWindow.ePictureBookListType type)
	{
		BasePictureBookListWindow listWindow = null;
		if (tabButtonController != null)
		{
			foreach(TabButtonInfo info in tabButtonController.tabInfos)
			{
				BasePictureBookListWindow tempWindow = info.viewWindow != null ? info.viewWindow.GetComponent<BasePictureBookListWindow>() : null;
				if (tempWindow != null && tempWindow.listType == type)
				{
					listWindow = tempWindow;
					
					if (listWindow != null)
						listWindow.parentWindow = this.gameObject;
					
					break;
				}
			}
		}
		
		return listWindow;
	}
	
	public void InitList()
	{
		foreach(TabButtonInfo info in tabButtonController.tabInfos)
		{
			BasePictureBookListWindow tempWindow = info.viewWindow != null ? info.viewWindow.GetComponent<BasePictureBookListWindow>() : null;
			if (tempWindow != null)
			{
				tempWindow.InitList();
			}
		}
	}
}
