using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossClearWindow : MonoBehaviour {
	public GameObject parentObj = null;
	public UITexture bossClear = null;
	public string path = "";
	public string defaultBGName = "";
	public List<string> texureName = new List<string>();
	
	
	public GameObject firstClearObj = null;
	public GameObject hellFirstClearObj = null;
	
	public void SetStageName(string stageName, int stageType, bool isFirstClear)
	{
		string bgName = "";
		if (stageName.Contains("act") == true)
		{
			string numberStr = stageName.Substring(3);
			
			int stageNumber = 0;
			if (numberStr.Length > 0)
				stageNumber = int.Parse(numberStr);
			
			int index = -1;
			if (stageNumber % 10 == 0)
				index = (stageNumber / 10) - 1;
			
			bgName = GetStageClearBGName(index);
		}
		else if (stageName == "Wave")
		{
			bgName = "Clear_Defence";
		}
		else if (stageName == "BossRaid")
		{
			bgName = "Clear_BossRaid";
		}
		
		if (bossClear != null)
			bossClear.mainTexture = LoadTexture(bgName);
		
		if (stageType == 2)
		{
			if (firstClearObj != null)
				firstClearObj.SetActive(false);
			
			if (hellFirstClearObj != null)
				hellFirstClearObj.SetActive(isFirstClear);
		}
		else
		{
			if (hellFirstClearObj != null)
				hellFirstClearObj.SetActive(false);
			
			if (firstClearObj != null)
				firstClearObj.SetActive(isFirstClear);
		}
		
	}
	
	public Texture LoadTexture(string textureName)
	{
		string pathStr = string.Format("{0}{1}", path, textureName);
		//Texture texture = (Texture2D)Resources.Load(pathStr);
		Texture texture = ResourceManager.LoadTexture(pathStr);
		
		return texture;
	}
	
	public string GetStageClearBGName(int index)
	{
		string stageLoading = defaultBGName;
		int nCount = texureName.Count;
		if (index >= 0 && index < nCount)
			stageLoading = texureName[index];
		
		return stageLoading;
	}
	
	public float lifeTime = 1.0f;
	public void Update()
	{
		lifeTime -= Time.deltaTime;
		
		if (lifeTime <= 0.0f)
		{
			if (parentObj != null)
				parentObj.SendMessage("OnFinishBossClear", SendMessageOptions.DontRequireReceiver);
			
			DestroyObject(this.gameObject, 0.0f);
		}
	}
}
