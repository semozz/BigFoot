using UnityEngine;
using System.Collections;

public class ResourcePreLoader : BaseAssetLoader {
	public string versionTablename = "AssetBundleInfo.xml";
	public string backgroundImageName = "LoadingBackground.png";
	
	public UISlider progress = null;
	public UILabel loadingInfoLabel = null;
	
	public UITexture backgroundTexture = null;
	
	public UILabel message = null;
	
	public string eulaScene = "EULA";
	public string loginScene = "Login_New";
	
	//MaxCache Size.. 1200 -> 1.2GB
	public int maxCachSize = 1200;
	public string loadingSoundPrefabPath = "Effect/AudioListener";
	
	// Use this for initialization
	IEnumerator Start () {
		Game.Instance.isResourceLoading = true;
		
		LoadingSound loadingSound = FindObjectOfType(typeof(LoadingSound)) as LoadingSound;
		if (GameOption.bgmToggle)
		{
			if (loadingSound == null)
				loadingSound = ResourceManager.CreatePrefab<LoadingSound>(loadingSoundPrefabPath);
			DontDestroyOnLoad(loadingSound);
		}
		else if (loadingSound != null)
			DestroyObject(loadingSound.gameObject);
		yield return new WaitForSeconds(0.5f);
		
		Caching.maximumAvailableDiskSpace = maxCachSize*1024*1024;
		
		double freeSpace = Caching.spaceFree;
		double occupied = Caching.spaceOccupied;
		
		SetMessage("Wating.....");
		yield return new WaitForSeconds(0.5f);
		
		string versionUrl = string.Format("{0}/{1}", NetConfig.GetRootURL(), versionTablename);
		yield return StartCoroutine(LoadAssetBundleVersion(versionUrl));
	}
	
	public override void SetMessage(string messageStr)
	{
		if (message != null)
			message.text = messageStr;
	}
	
	public override void SetProgress (float progressValue, bool isDownLoading)
	{
		if (progress != null)
			progress.sliderValue = progressValue;
		
		if (loadingInfoLabel != null)
		{
			string infoStr = "";
			if (isDownLoading == true)
			{
				if (progressValue < 1.0f)
					infoStr = string.Format("Data Downloading {0}%", (int)(progressValue * 100.0f));
				else
					infoStr = string.Format("Data Download Complete");
			}
			else
			{
				if (progressValue < 1.0f)
					infoStr = string.Format("File Opening {0}%", (int)(progressValue * 100.0f));
				else
					infoStr = string.Format("File Open Complete");
			}
			
			loadingInfoLabel.text = infoStr;
		}
	}
	
	
	public IEnumerator LoadScene(string url, int version, string sceneName)
	{
		SetMessage(sceneName);
		
		if (loadWWW!= null)
			CloseWWW();
		
		while(Caching.ready == false)
			yield return null;
		
		loadWWW = WWW.LoadFromCacheOrDownload(url, version);
		yield return loadWWW;
		
		string errorMsg = "";
		if (loadWWW != null)
			errorMsg = loadWWW.error;
	
		if (!string.IsNullOrEmpty(errorMsg) && errorMsg.StartsWith("Cannot load cached AssetBundle") == false)
		{
			Debug.Log(loadWWW.error);
			
			if (GameUI.Instance.MessageBox != null)
				GameUI.Instance.MessageBox.SetMessage("Stage Load Error!!!");
		}
		else
		{
			Resources.UnloadUnusedAssets();
			yield return 0;
			var bundle = loadWWW.assetBundle;
			
			//bundle.LoadAll();
			
			SetMessage(sceneName);
			mapLoading = Application.LoadLevelAsync(sceneName);
			yield return mapLoading;
			
			if (bundle != null)
			{
				bundle.Unload(false);
				bundle = null;
			}
		}
		
		CloseWWW();
	}
	
	IEnumerator LoadAssetBundleVersion(string url)
	{
		SetMessage("LoadingAssetBundle Version");
		
		if (loadWWW!= null)
			CloseWWW();
		
		AssetBundleVersionManager assetBundleVersionMgr = AssetBundleVersionManager.Instance;
		AssetBundleVersion versionInfo = assetBundleVersionMgr.assetBundleVersion;
		
		System.Collections.Generic.List<AssetBundleInfo> assetBundlInfos = versionInfo != null ? versionInfo.assetBundlInfos : null;
			
		if (assetBundlInfos != null)
		{
			string bundleURL = "";
			foreach(AssetBundleInfo info in assetBundlInfos)
			{
				if (info.preLoading == true)
				{
					SetMessage(info.desc);
					bundleURL = NetConfig.MakeAssetBundleURL(info.assetBundleName);
					
					assetBundleVersionMgr.UpdateDeleteDelay(info);
					yield return new WaitForSeconds(0.01f);
					
					yield return StartCoroutine(LoadAssetBundle(bundleURL, info.version, info.assetBundleName, info));
				}
			}
			
			SetMessage("Table Data Loading...");
			yield return new WaitForSeconds(0.5f);
			AssetBundleInfo tableBundle = versionInfo.GetBundleInfo("Tables");
			if (tableBundle != null)
			{
				TableManager tableManager = TableManager.Instance;
				if (tableManager != null)
					tableManager.LoadTables(tableBundle.bundle);
				
				tableBundle.bundle.Unload(false);
				tableBundle.bundle = null;
			}
			SetProgress(1.0f, false);
			yield return new WaitForSeconds(0.5f);
			
			SetMessage("Resource Patch Complete...");
			yield return new WaitForSeconds(0.5f);
			SetMessage("Game Starting...");
			yield return new WaitForSeconds(0.5f);
			
			Game.Instance.isResourceLoading = false;
			
			AssetBundleInfo bundleInfo = versionInfo.GetBundleInfo("PreLoadingScene");
			if (bundleInfo != null)
			{
				string nextSceneName = eulaScene;
				LoginInfo loginInfo = Game.Instance.loginInfo;
				if (loginInfo != null)
				{
					if (loginInfo.eula_Checked == false)
						nextSceneName = eulaScene;
					else
						nextSceneName = loginScene;
				}
				
				bundleURL = NetConfig.MakeAssetBundleURL(bundleInfo.assetBundleName);
				yield return StartCoroutine(LoadScene(bundleURL, bundleInfo.version, nextSceneName));
			}
		}
		
		CloseWWW();
	}
}
