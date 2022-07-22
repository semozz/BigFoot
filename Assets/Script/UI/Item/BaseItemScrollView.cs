using UnityEngine;
using System.Collections;

public class BaseItemScrollView : MonoBehaviour {
	public GameObject parent = null;
	public GameDef.eItemSlotWindow slotWindow = GameDef.eItemSlotWindow.Inventory;
	
	public UIGrid uiGrid = null;
	public UIPanel uiGridPanel = null;
	public UIDraggablePanel uiDraggablePanel = null;
	
	public UIScrollBar horScrollBar = null;
	
	
	protected bool isInit = false;
	public virtual void Awake()
	{
		
	}
	
	public virtual void InitData()
	{
		
	}
	
	public virtual void InitAwake()
	{
		
	}
	
	public virtual void InitSelectedSlot()
	{
		
	}
	
	public virtual void SetSelectedSlot(int slotIndex, bool bSelected)
	{
		
	}
	
	public virtual Item GetItem(int slotIndex)
	{
		Item item = null;
		return item;
	}
	
	public virtual void SetLockRestSlots(int startIndex, bool bLock)
	{
		
	}
	
	public virtual void SetItem(int slotIndex, Item item)
	{
		
	}
	
	public virtual void SetItem(int slotIndex, CostumeSetItem item)
	{
		
	}
	
	public virtual void SetItem(int slotIndex, CashItemInfo item)
	{
		
	}
	
	public virtual void ChangePage(int selIndex)
	{
		
	}
	
	public virtual void RemoveItem(int index)
	{
		
	}
	
	protected GameObject CreatePrefab(GameObject prefab, Transform root, Vector3 pos)
	{
		GameObject newObj = (GameObject)Instantiate(prefab);
			
		newObj.transform.parent = root;
		
		newObj.transform.localPosition = pos;
		newObj.transform.localScale = Vector3.one;
		newObj.transform.localRotation = Quaternion.identity;
		
		return newObj;
	}
	
	protected GameObject AddGridItem(GameObject prefab, Transform root, Vector3 pos)
	{
		bool widgetAreStatic = false;
		if (uiGridPanel != null)
		{
			widgetAreStatic = uiGridPanel.widgetsAreStatic;
			uiGridPanel.widgetsAreStatic = false;
		}
		
		GameObject newObj = (GameObject)Instantiate(prefab);
		
		newObj.transform.parent = root;
		//NGUITools.AddChild(pageRootNode.gameObject, newObj);
		
		newObj.transform.localPosition = pos;
		newObj.transform.localScale = Vector3.one;
		newObj.transform.localRotation = Quaternion.identity;
		
		if (uiGrid != null && uiGridPanel != null)
		{
			uiGrid.Reposition();
			uiGridPanel.Refresh();
			
			if (uiDraggablePanel != null)
				uiDraggablePanel.UpdateScrollbars(true);
			
			uiGridPanel.widgetsAreStatic = widgetAreStatic;
		}
		
		
		return newObj;
	}
	
	public void InitPage()
	{
		this.horScrollBar.scrollValue = 0.0f;
		if (uiGridPanel != null)
		{
			Vector4 clipRange = uiGridPanel.clipRange;
			clipRange.x = 0.0f;
			clipRange.y = 0.0f;
			
			uiGridPanel.clipRange = clipRange;
			
			Vector3 vPos = uiGridPanel.transform.localPosition;
			vPos.x = 0.0f;
			uiGridPanel.transform.localPosition = vPos;
		}
	}
}
