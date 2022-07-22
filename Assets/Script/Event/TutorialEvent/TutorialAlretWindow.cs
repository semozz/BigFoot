using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialAlretWindow : MonoBehaviour {

	public List<GameObject> objList = new List<GameObject>();
	public GameObject commonObj = null;
	
	public TutorialTask task = null;
	
	void Awake()
	{
		InitData();
	}
	
	public void InitData()
	{
		foreach(GameObject temp in objList)
		{
			if (temp == null)
				continue;
			
			temp.SetActive(false);
		}
		
		if (commonObj != null)
			commonObj.SetActive(false);
	}
	
	public void SetActiveIndex(int index)
	{
		if (index == -1)
		{
			if (commonObj != null)
				commonObj.SetActive(true);
		}
		else
		{
			int nCount = objList.Count;
			for (int idx = 0; idx < nCount; ++idx)
			{
				GameObject obj = objList[idx];
				
				if (obj != null && index == idx)
					obj.SetActive(true);
			}
		}
	}
	
	public void OnSkip()
	{
		if (task != null)
			task.OnSkip();
	}
}
