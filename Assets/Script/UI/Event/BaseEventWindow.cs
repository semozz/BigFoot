using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseEventWindow : PopupBaseWindow {
	public List<GameObject> eventItems = new List<GameObject>();
	
	
	public void SetAttandanceCheck(int checkDay)
	{
		foreach(GameObject obj in eventItems)
			obj.SetActive(false);
		
		int nCount = eventItems.Count;
		GameObject checkObj = null;
		for (int index = 0; index < checkDay; ++index)
		{
			checkObj = eventItems[index];
			if (checkObj != null)
				checkObj.SetActive(true);
		}
	}
}
