using UnityEngine;
using System.Collections;

public class SceneLoader : MonoBehaviour {
	public GameObject bundleLoadingInfo = null;
	public GameObject mapLoadingInfo = null;
	
	protected WWW loadWWW = null;
	protected AsyncOperation mapLoading = null;
	
	public string errorMsg = "";
	public IEnumerator LoadSceneByAssetBundle(string sceneName)
	{
		if (bundleLoadingInfo != null)
			bundleLoadingInfo.SetActive(false);
		if (mapLoadingInfo != null)
			mapLoadingInfo.SetActive(false);
		
		AssetBundleInfo bundleInfo = null;
		
		bundleInfo = FindAssetBundleBySceneName(sceneName);
		if (bundleInfo != null)
			yield return StartCoroutine(LoadScene(bundleInfo, sceneName));
		else
			errorMsg = "Not Found Theme Bundle";
	}
	
	AssetBundleInfo FindStageImageAssetBundleBySceneName(string sceneName)
	{
		AssetBundleInfo info = null;
		
		AssetBundleVersionManager versionMgr = AssetBundleVersionManager.Instance;
		AssetBundleVersion versionInfo = versionMgr.assetBundleVersion;
		
		int actID = 0;
		int themeMaxCount = 20;
		int hardModeMask = 1000;
		int themeID = 0;
		
		string bundleName = "";
		string themeName = "";
		
		if (sceneName.StartsWith("act") == true)
		{
			string numberStr = sceneName.Substring(3);
		
			bool isHardMode = false;
			
			if (numberStr.Length > 0)
				actID = int.Parse(numberStr);
			
			isHardMode = (actID & hardModeMask) == hardModeMask;
			
			actID = actID % hardModeMask;
			themeID = (actID / themeMaxCount) + 1;
			
			themeName = string.Format("Theme{0}", themeID);
		}
		else if (CheckEventStageName(sceneName) == true)
		{
			themeName = "Event";
		}
		
		if (themeName != null)
			bundleName = string.Format("{0}_Images", themeName);
		
		info = versionInfo.GetBundleInfo(bundleName);
		
		return info;
	}
	
	AssetBundleInfo FindAssetBundleBySceneName(string sceneName)
	{
		AssetBundleInfo info = null;
		
		AssetBundleVersionManager versionMgr = AssetBundleVersionManager.Instance;
		AssetBundleVersion versionInfo = versionMgr.assetBundleVersion;
		
		TableManager tableManager = TableManager.Instance;
		StringValueTable stringValueTable = tableManager != null ? tableManager.stringValueTable : null;
		
		int actID = 0;
		int themeMaxCount = 20;
		int hardModeMask = 1000;
		int themeID = 0;
		
		if (stringValueTable != null)
			themeMaxCount = stringValueTable.GetData("StageCountPerTheme");
		
		
		string bundleName = "";
		
		if (sceneName.StartsWith("act") == true)
		{
			string numberStr = sceneName.Substring(3);
		
			bool isHardMode = false;
			
			if (numberStr.Length > 0)
				actID = int.Parse(numberStr);
			
			isHardMode = actID > hardModeMask;
			
			actID = actID % hardModeMask;
			themeID = ((actID - 1) / themeMaxCount) + 1;
			
			bundleName = string.Format("Theme{0}", themeID);
		}
		else if (CheckEventStageName(sceneName) == true)
		{
			bundleName = "EventScene";
		}
		else
		{
			bundleName = "PreLoadingScene";
		}
		
		info = versionInfo.GetBundleInfo(bundleName);
		
		return info;
	}
	
	private bool CheckEventStageName(string sceneName)
	{
		string[] checkNames = {"Arena", "Wave", "BossRaid", "TutorialMap", "WeeklyDungeon" };
		bool bChecked = false;
		foreach(string name in checkNames)
		{
			if (sceneName.Contains(name) == true)
			{
				bChecked = true;
				break;
			}
		}
		
		return bChecked;
	}
	
	void CloseWWW()
	{
		if (loadWWW != null)
		{
			loadWWW.Dispose();
			loadWWW = null;
		}
	}
	
	public IEnumerator LoadScene(AssetBundleInfo bundleInfo, string sceneName)
	{
		string url = NetConfig.MakeAssetBundleURL(bundleInfo.assetBundleName);
		
		if (loadWWW!= null)
			CloseWWW();
		
		while(Caching.ready == false)
			yield return null;
		
		if (Caching.IsVersionCached(url, bundleInfo.version) == false)
		{
			if (bundleLoadingInfo != null)
				bundleLoadingInfo.SetActive(true);
		}
		
		
		AssetBundleVersionManager versionMgr = AssetBundleVersionManager.Instance;
		if (versionMgr != null)
			versionMgr.UpdateDeleteDelay(bundleInfo);
		
		loadWWW = WWW.LoadFromCacheOrDownload(url, bundleInfo.version);
		yield return loadWWW;
		
		
		if (bundleLoadingInfo != null)
			bundleLoadingInfo.SetActive(false);
		
		string errorMsg = "";
		if (loadWWW != null)
			errorMsg = loadWWW.error;
	
		if (!string.IsNullOrEmpty(errorMsg) && errorMsg.StartsWith("Cannot load cached AssetBundle") == false)
		{
			Debug.Log(loadWWW.error);
			
			Game.Instance.AndroidManager.CallUnityExitWindow(AlertDialogType.NetworkError);;
		}
		else
		{
			Resources.UnloadUnusedAssets();
			yield return 0;
			
			AssetBundle bundle = null;
			
			try
			{
				bundle = loadWWW.assetBundle;
			}
			catch(System.Exception e)
			{
				Debug.LogWarning(e);
			}
			
			if (mapLoadingInfo != null)
				mapLoadingInfo.SetActive(true);
		
			
			mapLoading = Application.LoadLevelAsync(sceneName);
			yield return mapLoading;
			
			
			if (bundle != null)
			{
				bundle.Unload(false);
				bundle = null;
			}
			
			Debug.Log("Scene Load Complete.....");
		}
		
		CloseWWW();
	}
	
	IEnumerator LoadAssetBundle(AssetBundleInfo bundleInfo)
	{
		string url = NetConfig.MakeAssetBundleURL(bundleInfo.assetBundleName);
		
		if (loadWWW!= null)
			CloseWWW();
		
		while(Caching.ready == false)
			yield return null;
		
		loadWWW = WWW.LoadFromCacheOrDownload(url, bundleInfo.version);
		yield return loadWWW;
		
		string errorMsg = "";
		if (loadWWW != null)
			errorMsg = loadWWW.error;
		
		if (!string.IsNullOrEmpty(errorMsg) && errorMsg.StartsWith("Cannot load cached AssetBundle") == false)
		{
			Debug.Log(errorMsg);
			
			Game.Instance.AndroidManager.CallUnityExitWindow(AlertDialogType.NetworkError);
		}
		else
		{
			Resources.UnloadUnusedAssets();
			yield return 0;
			
			try
			{
				bundleInfo.bundle = loadWWW.assetBundle;
			}
			catch(System.Exception e)
			{
				Debug.LogWarning(e);
			}
		}
		
		CloseWWW();
	}
}
