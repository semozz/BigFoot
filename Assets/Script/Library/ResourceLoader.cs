using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResourceLoader : MonoSingleton<ResourceLoader> 
{
	Dictionary <string, GameObject> dataList;
	
	public ResourceLoader()
	{
		dataList = new Dictionary<string, GameObject>();
	}
	GameObject FindAndCreate(string key)
	{
		if (dataList.ContainsKey(key))
		{
			dataList[key].SetActive(true);
			
			return dataList[key];
		}
		
		GameObject prefab = (GameObject)Resources.Load(key);	
		if (prefab != null)
			dataList.Add (key, prefab);
		
		return prefab;
	}
	
	public GameObject Clone(Transform parent, string path)
	{
		GameObject prefab = FindAndCreate(path);
		
		GameObject newObj = null;
		if (prefab != null)
			newObj = (GameObject)Instantiate(prefab);
		
		//MessageBox msgBox = null;
		if (newObj != null)
		{
			//msgBox = newObj.GetComponent<MessageBox>();
			
			Vector3 origPos = Vector3.zero;
			Vector3 origScale = Vector3.one;
			Quaternion origQuat = Quaternion.identity;
			if (newObj != null)
			{
				origPos = newObj.transform.localPosition;
				origScale = newObj.transform.localScale;
				origQuat = newObj.transform.localRotation;
			}
			
			newObj.transform.parent = parent;
			
			newObj.transform.localPosition = origPos;
			newObj.transform.localScale = origScale;
			newObj.transform.localRotation = origQuat;
		}
		
		return newObj;
	}
	
	public T CloneComponent<T>(Transform parent, string path) where T: Component 
	{
		GameObject prefab = Clone(parent, path);
		
		if (!prefab)
			return null;
		
		T result = prefab.GetComponent<T>();
		
		return result;
	}
		
}
