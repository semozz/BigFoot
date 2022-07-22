using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemPageScrollView : BaseItemScrollView {
	
	//public GameObject pagePrefab = null;
	public string pagePrefabPath = "";
	public Transform pageRootNode = null;
	
	//public GameObject pageButtonPrefab = null;
	public string pageButtonPrefabPath = "";
	public Transform pageButtonNode = null;
	public float pageButtonPadding = 5.0f;
	
	
	public int maxItems = 24;
	public int itemPerPage = 8;
	public int maxPageCount = -1;
	public int pageCount = 0;
	
	public bool isExpandableSlotPanel = false;
	
	public List<ItemSlotPanel> itemSlotPanelList = new List<ItemSlotPanel>();
	public List<GameObject> pageButtons = new List<GameObject>();
	
	public override void Awake()
	{
		if (isInit == false)
			InitAwake();
	}
	
	public override void InitAwake()
	{
		//isInit = true;
		if (isInit == true)
			return;
		
		pageCount = (maxItems - 1) / itemPerPage;
		if ((maxItems - 1) % itemPerPage > 0)
			pageCount++;
		
		GameObject page = null;
		GameObject pageButton = null;
		
		GameObject pageButtonPrefab = ResourceManager.LoadPrefab(pageButtonPrefabPath);
		Vector3 buttonSize = Vector3.one;
		if (pageButtonPrefab != null)
		{
			BoxCollider boxCollider = (BoxCollider)pageButtonPrefab.collider;
			buttonSize = boxCollider.size;
		}
		
		float totalSizeX = (buttonSize.x * pageCount) + (pageButtonPadding * pageCount - 1);
		float startX = -totalSizeX * 0.5f;
		float curXPos = startX;
		
		
		GameObject pagePrefab = ResourceManager.LoadPrefab(pagePrefabPath);
		ItemSlotPanel itemSlotPanelPrefab = null;
		if (pagePrefab != null)
		{
			itemSlotPanelPrefab = pagePrefab.GetComponent<ItemSlotPanel>();
			if (itemSlotPanelPrefab != null)
			{
				itemSlotPanelPrefab.parentObj = this.parent;
				itemSlotPanelPrefab.slotWindow = this.slotWindow;
			}
		}
		
		for(int pageIndex = 0; pageIndex < pageCount; ++pageIndex)
		{
			if (itemSlotPanelPrefab != null)
			{
				itemSlotPanelPrefab.startSlotIndex = (pageIndex * itemPerPage);
			}
			
			page = AddGridItem(pagePrefab, pageRootNode, pagePrefab.transform.localPosition);
			if (page != null)
			{
				Destroy(page.GetComponent<UIPanel>());
				
				ItemSlotPanel itemSlotPanel = page.GetComponent<ItemSlotPanel>();
				if (itemSlotPanel != null)
					itemSlotPanelList.Add(itemSlotPanel);
			}
			
			pageButton = CreatePrefab(pageButtonPrefab, pageButtonNode, pageButtonPrefab.transform.localPosition);
			if (pageButton != null)
			{
				pageButton.transform.localPosition = new Vector3(curXPos, 0.0f, 0.0f);
				
				curXPos += buttonSize.x + pageButtonPadding;
				
				UICheckbox checkBox = pageButton.GetComponent<UICheckbox>();
				if (checkBox != null)
				{
					checkBox.radioButtonRoot = pageButtonNode;
					if (pageIndex == 0)
						checkBox.startsChecked = true;
					
					checkBox.eventReceiver = this.gameObject;
				}
				
				pageButtons.Add(pageButton);
			}
		}
		
		if (horScrollBar != null)
			horScrollBar.onChange += OnHorizontalBar;
		
		isInit = true;
	}
	
	public int curPage = 0;
	void OnHorizontalBar (UIScrollBar sb)
	{
		float pageRate = 0.0f;
		if (horScrollBar != null)
			pageRate = horScrollBar.scrollValue;
		
		pageRate = Mathf.Clamp(pageRate, 0.0f, 1.0f);
		int newPage = (int)(pageCount * pageRate);
		
		if (newPage != curPage)
		{
			curPage = newPage;
			OnChangePage(curPage);
			
			Debug.Log("Page : " + curPage);
		}
	}
	
	public void OnChangePage(int page)
	{
		if (pageButtonNode != null)
		{
			UICheckbox[] cbs = pageButtonNode.GetComponentsInChildren<UICheckbox>(true);

			for (int i = 0, imax = cbs.Length; i < imax; ++i)
			{
				UICheckbox cb = cbs[i];
				
				if (cb != null)
				{
					cb.isChecked = (i == page);
				}
			}
		}
	}
	
	public bool CheckLastPageActive()
	{
		bool isLastPage = false;
		
		if (pageButtonNode != null)
		{
			UICheckbox[] cbs = pageButtonNode.GetComponentsInChildren<UICheckbox>(true);
			
			int maxPage = cbs.Length;
			
			for (int i = 0, imax = maxPage; i < imax; ++i)
			{
				UICheckbox cb = cbs[i];
				
				if (cb != null && cb.isChecked == true)
				{
					isLastPage = (i == (maxPage - 1) && i != (this.maxPageCount - 1));
					
					if (isLastPage == true)
						break;
				}
			}
		}
		
		return isLastPage;
	}
	
	public void OnPageButtonActive(bool bActive)
	{
		if (pageButtonNode != null && bActive)
		{
			/*
			UICheckbox[] cbs = pageButtonNode.GetComponentsInChildren<UICheckbox>(true);
			
			int maxPage = cbs.Length;
			
			bool isLastPage = false;
			
			for (int i = 0, imax = maxPage; i < imax; ++i)
			{
				UICheckbox cb = cbs[i];
				
				if (cb != null && cb.isChecked == true)
				{
					isLastPage = (i == (maxPage - 1));
					this.parent.SendMessage("OnLastPageOn", isLastPage, SendMessageOptions.DontRequireReceiver);
				}
			}
			*/
			
			this.parent.SendMessage("UpdateExpandButtonPanel", SendMessageOptions.DontRequireReceiver);
		}
	}
	
	void OnEnable()
	{
		if (uiGrid != null)
			uiGrid.Invoke("Reposition", 0.2f);
	}
	
	public override void SetItem(int slotIndex, Item item)
	{
		int pageIndex = slotIndex / itemPerPage;
		int index = slotIndex % itemPerPage;
		
		int pageCount = itemSlotPanelList.Count;
		
		if (pageIndex < 0)
			return;
		
		if (pageIndex >= pageCount)
		{
			if (maxPageCount == -1 || pageCount < maxPageCount)
				AddPagePanel(pageIndex);
			else
				return;
		}
		
		ItemSlotPanel itemSlotPanel = itemSlotPanelList[pageIndex];
		
		if (itemSlotPanel != null)
			itemSlotPanel.SetItem(index, item);
	}
	
	public override Item GetItem(int slotIndex)
	{
		Item item = null;
		
		int pageIndex = slotIndex / itemPerPage;
		int index = slotIndex % itemPerPage;
		
		int pageCount = itemSlotPanelList.Count;
		
		if (pageIndex < 0 || pageIndex >= pageCount)
			return item;
		
		ItemSlotPanel itemSlotPanel = itemSlotPanelList[pageIndex];
		
		if (itemSlotPanel != null)
			item = itemSlotPanel.GetItem(index);
		
		return item;
	}
	
	public void AddPagePanel(int pageIndex)
	{
		GameObject page = null;
		GameObject pagePrefab = ResourceManager.LoadPrefab(pagePrefabPath);
		ItemSlotPanel itemSlotPanelPrefab = null;
		if (pagePrefab != null)
		{
			itemSlotPanelPrefab = pagePrefab.GetComponent<ItemSlotPanel>();
			if (itemSlotPanelPrefab != null)
			{
				itemSlotPanelPrefab.parentObj = this.parent;
				itemSlotPanelPrefab.slotWindow = this.slotWindow;
			}
		}
		
		if (itemSlotPanelPrefab != null)
		{
			itemSlotPanelPrefab.startSlotIndex = (pageIndex * itemPerPage);
		}
		
		page = AddGridItem(pagePrefab, pageRootNode, pagePrefab.transform.localPosition);
		if (page != null)
		{	
			Destroy(page.GetComponent<UIPanel>());
			
			ItemSlotPanel itemSlotPanel = page.GetComponent<ItemSlotPanel>();
			if (itemSlotPanel != null)
			{
				itemSlotPanelList.Add(itemSlotPanel);
				pageCount = itemSlotPanelList.Count;
			}
		}
		
		RebuildPageButton();
	}
	
	public void RebuildPageButton()
	{
		foreach(GameObject obj in pageButtons)
		{
			DestroyImmediate(obj);
		}
		pageButtons.Clear();
		
		int pageCount = itemSlotPanelList.Count;
		
		GameObject pageButton = null;
		
		GameObject pageButtonPrefab = ResourceManager.LoadPrefab(pageButtonPrefabPath);
		Vector3 buttonSize = Vector3.one;
		if (pageButtonPrefab != null)
		{
			BoxCollider boxCollider = (BoxCollider)pageButtonPrefab.collider;
			buttonSize = boxCollider.size;
		}
		
		float totalSizeX = (buttonSize.x * pageCount) + (pageButtonPadding * pageCount - 1);
		float startX = -totalSizeX * 0.5f;
		float curXPos = startX;
		
		for(int pageIndex = 0; pageIndex < pageCount; ++pageIndex)
		{
			pageButton = CreatePrefab(pageButtonPrefab, pageButtonNode, pageButtonPrefab.transform.localPosition);
			if (pageButton != null)
			{
				pageButton.transform.localPosition = new Vector3(curXPos, 0.0f, 0.0f);
				
				curXPos += buttonSize.x + pageButtonPadding;
				
				UICheckbox checkBox = pageButton.GetComponent<UICheckbox>();
				if (checkBox != null)
				{
					checkBox.radioButtonRoot = pageButtonNode;
					if (pageIndex == 0)
						checkBox.startsChecked = true;
					
					checkBox.eventReceiver = this.gameObject;
				}
				
				pageButtons.Add(pageButton);
			}
		}
		
	}
	
	public override void InitSelectedSlot()
	{
		foreach(ItemSlotPanel slotPanel in itemSlotPanelList)
		{
			if (slotPanel != null)
				slotPanel.InitSelectedSlot();
		}
	}
	
	public override void SetSelectedSlot(int slotIndex, bool bSelected)
	{
		int pageIndex = slotIndex / itemPerPage;
		int index = slotIndex % itemPerPage;
		
		int pageCount = itemSlotPanelList.Count;
		
		if (pageIndex < 0 || pageIndex >= pageCount)
			return;
		
		ItemSlotPanel itemSlotPanel = itemSlotPanelList[pageIndex];
		
		if (itemSlotPanel != null)
			itemSlotPanel.SetSelectedSlot(index, bSelected);
	}
	
	public override void ChangePage(int selIndex)
	{
		int newPage = (selIndex / itemPerPage) + 1;
		
		float newPageFloatValue = (float)newPage;
		float pageCount = (float)itemSlotPanelList.Count;
		
		float pageRate = 0.0f;
		if (newPage == 1)
			pageRate = 0.0f;
		else if (pageCount == newPageFloatValue)
			pageRate = 1.0f;
		else if (pageCount > 0.0)
			pageRate = newPageFloatValue / pageCount;
		
		this.horScrollBar.scrollValue = pageRate;
		
		if (uiGridPanel != null)
		{
			Vector4 clipRange = uiGridPanel.clipRange;
			clipRange.y = 0.0f;
			uiGridPanel.clipRange = clipRange;
		}
	}
	
	public override void SetLockRestSlots (int startIndex, bool bLock)
	{
		int pageCount = itemSlotPanelList.Count;
		
		int maxCount = pageCount * itemPerPage;
		
		for (int slotIndex = startIndex; slotIndex < maxCount; ++slotIndex)
		{
			int pageIndex = slotIndex / itemPerPage;
			int index = slotIndex % itemPerPage;
		
			if (pageIndex < 0 || pageIndex >= pageCount)
				continue;
			
			ItemSlotPanel itemSlotPanel = itemSlotPanelList[pageIndex];
			
			if (itemSlotPanel != null)
				itemSlotPanel.SetLockSlot(index, bLock);
		}
	}
	
	public void SetEnableButton(bool bEnable)
	{
		foreach(ItemSlotPanel slotPanel in itemSlotPanelList)
		{
			if (slotPanel != null)
			{
				if (slotPanel.itemSlots != null)
				{
					foreach(ItemSlot slot in slotPanel.itemSlots)
					{
						if (slot != null && slot.buttonMsg != null)
						{
							Collider buttonCollider = slot.buttonMsg.gameObject.collider;
							if (buttonCollider != null)
								buttonCollider.enabled = bEnable;
						}
					}
				}
			}
		}
	}
}
