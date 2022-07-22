using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BasePictureBookListWindow : MonoBehaviour {
	public GameObject parentWindow = null;
	
	public enum ePictureBookListType
	{
		NormalMonster,
		HardMonster,
		Item_Weapon,
		Item_Helmet,
		Item_Armor,
		Item_Hand,
		Item_Pants,
		Item_Boots,
		Item_Ring,
		Item_Accessory,
	}
	public ePictureBookListType listType = ePictureBookListType.NormalMonster;
	
	public string infoPanelPrefabPath = "";
	public UIGrid grid = null;
	public UIDraggablePanel dragablePanel = null;
	public UIPanel scrollPanel = null;
	
	public void SetButtonMessage(UIButtonMessage buttonMsg, GameObject target, string funcName)
	{
		if (buttonMsg != null)
		{
			buttonMsg.target = target;
			buttonMsg.functionName = funcName;//"OnTargetDetailWindow";
		}
	}
	
	private List<MonsterPictureBookPanel> pictureBookInfoPanels = new List<MonsterPictureBookPanel>();
	//public void SetInfos(List<MonsterPictureBookInfo> pictureBookList)
	public void SetInfos(Dictionary<int, MonsterPictureBookInfo> pictureBookList)
	{
		if (pictureBookList == null)
			return;
		
		Vector3 vPos = Vector3.zero;
		foreach(var temp in pictureBookList)
		{
			MonsterPictureBookInfo info = temp.Value;
			
			MonsterPictureBookPanel infoPanel = ResourceManager.CreatePrefab<MonsterPictureBookPanel>(infoPanelPrefabPath, grid.transform, vPos);
			if (infoPanel != null)
			{
				infoPanel.parentWindow = this;
				
				infoPanel.SetInfo(info);
				
				pictureBookInfoPanels.Add(infoPanel);
				
				if (this.grid.sorted == true)
					SetSortName(infoPanel);
				
				vPos.y += grid.cellHeight;
			}
		}
		
		//RefreshPanels();
		this.Invoke("RefreshPanels", 0.1f);
	}
	
	public void SetSortName(MonsterPictureBookPanel infoPanel)
	{
		if (infoPanel == null || infoPanel.monsterInfo == null)
			return;
		
		string objName = string.Format("Act{0:D2}_{1}", infoPanel.monsterInfo.act, infoPanel.monsterInfo.id);
		
		infoPanel.name = objName;
	}
	
	private List<ItemPictureBookPanel> itemPictureBookInfoPanels = new List<ItemPictureBookPanel>();
	public void SetInfos(List<ItemPictureBookInfo> pictureBookList)
	{
		if (pictureBookList == null)
			return;
		
		Vector3 vPos = Vector3.zero;
		foreach(ItemPictureBookInfo info in pictureBookList)
		{
			ItemPictureBookPanel infoPanel = ResourceManager.CreatePrefab<ItemPictureBookPanel>(infoPanelPrefabPath, grid.transform, vPos);
			if (infoPanel != null)
			{
				infoPanel.parentWindow = this;
				
				infoPanel.SetInfo(info);
				
				itemPictureBookInfoPanels.Add(infoPanel);
				
				if (this.grid.sorted == true)
					SetSortName(infoPanel);
				
				vPos.y += grid.cellHeight;
			}
		}
		
		//RefreshPanels();
		this.Invoke("RefreshPanels", 0.1f);
	}
	
	public void SetSortName(ItemPictureBookPanel infoPanel)
	{
		if (infoPanel == null || infoPanel.itemPictureInfo == null)
			return;
		
		int itemID = 0;
		ItemInfo itemInfo = infoPanel.itemPictureInfo.item != null ? infoPanel.itemPictureInfo.item.itemInfo : null;
		if (itemInfo != null)
			itemID = itemInfo.itemID;
		
		string objName = string.Format("{0}", itemID);
		
		infoPanel.name = objName;
	}
	
	public void RefreshPanels()
	{
		if (grid != null)
			grid.Reposition();
		
		if (dragablePanel != null)
			dragablePanel.ResetPosition();
		
		if (scrollPanel != null)
			scrollPanel.Refresh();
	}
	
	bool isRefreshCalled = false;
	public void OnEnable()
	{
		if (isRefreshCalled == false && pictureBookInfoPanels.Count > 0)
		{
			//RefreshPanels();
			this.Invoke("RefreshPanels", 0.1f);
			isRefreshCalled = true;
		}
	}
	
	public void OnDisable()
	{
		isRefreshCalled = false;
	}
	
	public void InitList()
	{
		foreach(MonsterPictureBookPanel temp in pictureBookInfoPanels)
			DestroyObject(temp.gameObject, 0.0f);
		pictureBookInfoPanels.Clear();
		
		foreach(ItemPictureBookPanel infoPanel in itemPictureBookInfoPanels)
			DestroyObject(infoPanel.gameObject, 0.0f);
		itemPictureBookInfoPanels.Clear();
		
		isRefreshCalled = false;
	}
}
