using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadingPanel : SceneLoader {
	
	public UISlider progressBar = null;
	public UILabel progressInfoLabel = null;
	
	public UILabel msgLabel = null;
	
	public UILabel tooltipLabel = null;
	
	public UITexture loadingBackground = null;
	
	public string path = "";
	public List<string> bgNameList = new List<string>();
	public string defaultBGName = "";
	
	public PopupBaseWindow parentWindow = null;
	
	public string GetStageLoadingTexture(int index)
	{
		string stageLoading = defaultBGName;
		int nCount = bgNameList.Count;
		if (index >= 0 && index < nCount)
			stageLoading = bgNameList[index];
		
		return stageLoading;
	}
	
	public float minLifeTime = 3.0f;
	public void LoadScene(string sceneName, PopupBaseWindow window)
	{
		this.parentWindow = window;
		StartCoroutine(LoadSceneBy(sceneName));
	}
	
	public IEnumerator LoadSceneBy(string sceneName)
	{
		SetToolTip();
		
		//ChangeLoadingBackgroundImage(sceneName);
		yield return new WaitForSeconds(1.0f);
		
		yield return StartCoroutine(LoadSceneByAssetBundle(sceneName));
		
		if (this.errorMsg == "Not Found Theme Bundle")
		{
			DestroyObject(this.gameObject, 0.1f);
			
			if (parentWindow != null)
				parentWindow.OnErrorMessage("Map Error", "Can't found Theme");
		}
		Game.Instance.ResetPause();
		
		float lifeTime = Mathf.Max(0.0f, (minLifeTime - 1.0f));
		yield return new WaitForSeconds(lifeTime);
	}
	
	
	public void ChangeLoadingBackgroundImage(string sceneName)
	{
		string bgName = GetBGTextureName(sceneName);
		string pathStr = string.Format("{0}{1}", path, bgName);
		
		if (loadingBackground != null)
			loadingBackground.mainTexture = LoadTexture(bgName);
	}
	
	public Texture LoadTexture(string textureName)
	{
		string pathStr = string.Format("{0}{1}", path, textureName);
		//Texture texture = (Texture2D)Resources.Load(pathStr);
		Texture texture = ResourceManager.LoadTexture(pathStr);
		
		return texture;
	}
	
	string GetBGTextureName(string sceneName)
	{
		string bgName = "loading_img_0_t";
		if (sceneName.StartsWith("act") == true)
		{
			string numberStr = sceneName.Substring(3);
			
			int stageNumber = 0;
			if (numberStr.Length > 0)
				stageNumber = int.Parse(numberStr);
			
			int index = (stageNumber - 1) / 10;
			bgName = GetStageLoadingTexture(index);
		}
		else if (sceneName == "Wave")
		{
			bgName = "loading_img_101_out";
		}
		else if (sceneName == "TownTest")
		{
			bgName = "loading_img_0_t";
		}
		else if (sceneName == "BossRaid")
		{
			bgName = "loading_img_bossraid";
		}
		else if (sceneName == "WeeklyDungeon201")
		{
			bgName = "loading_img_weeklydungeon";
		}
		else if (sceneName == "WeeklyDungeon202")
		{
			bgName = "loading_img_weeklydungeon";
		}
		else if (sceneName == "WeeklyDungeon1201")
		{
			bgName = "loading_img_weeklydungeon";
		}
		else if (sceneName == "WeeklyDungeon1202")
		{
			bgName = "loading_img_weeklydungeon";
		}
		else if (sceneName == "WeeklyDungeon201")
		{
			bgName = "loading_img_weeklydungeon";
		}
		else if (sceneName == "WeeklyDungeon202")
		{
			bgName = "loading_img_weeklydungeon";
		}
		else if (sceneName == "WeeklyDungeon2201")
		{
			bgName = "loading_img_weeklydungeon";
		}
		else if (sceneName == "WeeklyDungeon2202")
		{
			bgName = "loading_img_weeklydungeon";
		}
		return bgName;
	}
	
	void SetToolTip()
	{
		TableManager tableManager = TableManager.Instance;
		ToolTipTable toolTipTable = null;
		if (tableManager != null)
			toolTipTable = tableManager.tooltipsTable;
		
		if (toolTipTable != null && tooltipLabel != null)
		{
			tooltipLabel.text = toolTipTable.GetRandTooltip();
		}
	}
	
	void FixedUpdate () {
		float loadingRate = 1.0f;
		if (mapLoading != null)
		{
			if (progressBar != null)
				progressBar.gameObject.SetActive(false);
				
			loadingRate = 0.0f;
			if (mapLoading.isDone == true)
				loadingRate = 1.0f;
			else
				loadingRate = mapLoading.progress + 0.15f; // 100%가 안나와서 100%보려고 수정. 물론 100% 못 볼 수도 있음. 더좋은방법은?

            if (loadingRate > 1.0f)
                loadingRate = 1.0f;
			
			if (msgLabel != null)
				msgLabel.text = ((int)(loadingRate * 100)).ToString();
		}
		else if (loadWWW != null)
		{
			loadingRate = 1.0f;
			if (loadWWW.isDone == true)
				loadingRate = 1.0f;
			else
				loadingRate = loadWWW.progress;
			
			if (progressBar != null)
			{
				progressBar.gameObject.SetActive(true);
				progressBar.sliderValue = loadingRate;
			}
			
			if (progressInfoLabel != null)
			{
				if (loadingRate == 1.0f)
					progressInfoLabel.text = string.Format("Data DownLoad Complete");
				else
					progressInfoLabel.text = string.Format("Data DownLoading... {0} %", (int)(loadingRate * 100.0f));
			}
		}
		else
		{
			if (progressBar != null)
				progressBar.sliderValue = 0.0f;
			
			if (msgLabel != null)
				msgLabel.text = "0";
		}
	}
	
	public void SetFirstDownloadMessage(bool bFirst)
	{
		
	}
}
