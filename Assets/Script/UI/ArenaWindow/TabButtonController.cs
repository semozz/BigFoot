using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TabButtonInfo
{
	public UICheckbox tabButton = null;
	
	public UILabel tabButtonLabel = null;
	public int tabButtonLabelStringID = -1;
	
	public Color selectedColor = Color.black;
	public Color unSelectedColor = Color.white;
	
	public string viewPrefabPath = "";
	public GameObject viewWindow = null;
}

public class TabButtonController : MonoBehaviour {
	public List<TabButtonInfo> tabInfos = new List<TabButtonInfo>();
	
	public Transform tabView = null;
	
	public bool createOnAwake = true;
	
	// Use this for initialization
	void Awake () {
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		foreach(TabButtonInfo info in tabInfos)
		{
			if (createOnAwake == true)
			{
				info.viewWindow = ResourceManager.CreatePrefab(info.viewPrefabPath, tabView, Vector3.zero);
				
				if (info.viewWindow != null)
					info.viewWindow.gameObject.SetActive(info.tabButton.startsChecked);
			}
			
			info.tabButton.onStageChangeArg2 = new UICheckbox.OnStateChangeArg2(OnChangeTab);
			
			if (info.tabButtonLabel != null && info.tabButtonLabelStringID != -1)
				info.tabButtonLabel.text = stringTable.GetData(info.tabButtonLabelStringID);
		}
	}
	
	public void OnChangeTab(UICheckbox checkBox, bool bCheck)
	{
		foreach(TabButtonInfo info in tabInfos)
		{
			if (checkBox == info.tabButton)
			{
				if (info.viewWindow != null)
					info.viewWindow.gameObject.SetActive(bCheck);
				else
				{
					if (bCheck == true)
					{
						info.viewWindow = ResourceManager.CreatePrefab(info.viewPrefabPath, tabView, Vector3.zero);
				
						if (info.viewWindow != null)
							info.viewWindow.gameObject.SetActive(bCheck);
					}
				}
			}
			
			if (info.tabButtonLabel != null)
			{
				Color labelColor = info.selectedColor;
				if (info.tabButton.isChecked == true)
					labelColor = info.selectedColor;
				else
					labelColor = info.unSelectedColor;
				
				info.tabButtonLabel.color = labelColor;
			}
		}
	}
	
	public T GetViewWindow<T>(int index) where T : Component
	{
		object comp = null;
		GameObject obj = null;
		
		int nCount = tabInfos.Count;
		
		if (index >= 0 && index < nCount)
			obj = tabInfos[index].viewWindow;
		
		if (obj != null)
			comp = obj.GetComponent<T>();
		
		return (T)comp;
	}
}
