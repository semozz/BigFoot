using UnityEngine;
using System.Collections;

public class BaseAssetLoader : MonoBehaviour {
	protected WWW loadWWW = null;
	protected AsyncOperation mapLoading = null;
	
	public virtual void Update()
	{
		float progressValue = 1.0f;
		bool isDownLoading = false;
		
		if (mapLoading != null)
		{
			if (mapLoading.isDone == false)
				progressValue = mapLoading.progress;
			else
				progressValue = 1.0f;
		}
		else if (loadWWW != null)
		{
			isDownLoading = true;
			
			if (loadWWW.isDone == false)
				progressValue = loadWWW.progress;
			else
				progressValue = 1.0f;
		}
		
		SetProgress(progressValue, isDownLoading);
	}
	
	public virtual void UpdateUIInfo(float progressValue)
	{
				
	}
	
	public virtual void SetMessage(string messageStr)
	{
		
	}
	
	public virtual void SetProgress(float progressValue, bool isDownLoading)
	{
		
	}
	
	public void CloseWWW()
	{
		if (loadWWW != null)
		{
			loadWWW.Dispose();
			loadWWW = null;
		}
	}
	
	public IEnumerator LoadAssetBundle(string url, int version, string bundleName, AssetBundleInfo info)
	{
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
			string infoStr = string.Format("url:{0}/bundleName:{1}/errorMsg:{2}", url, bundleName, errorMsg); 
			Logger.DebugLog(infoStr);
			
			if (GameUI.Instance.MessageBox != null)
				GameUI.Instance.MessageBox.SetMessage("AssetBundle Error!!!");
		}
		else
		{
			Resources.UnloadUnusedAssets();
			yield return 0;
			AssetBundle bundle = loadWWW.assetBundle;
			
			bool isSaveBundle = false;
			
			if (info != null &&
				info.preLoading == true && info.filterType != PatchData.eFilterType.Type_Scene)
			{
				string infoStr = "";
				
				if (info.bundle != null && bundle == null)
				{
					infoStr = string.Format("Keep Old Asset Bundle Set {0} : {1}", info.assetBundleName, info.bundle.ToString());
					Debug.Log(infoStr);
				}
				else
				{
					info.bundle = bundle;
					
					infoStr = string.Format("Asset Bundle Set {0} : {1}", info.assetBundleName, bundle == null ? "Not Found" : bundle.ToString());
					Debug.Log(infoStr);
				}
				
				isSaveBundle = true;
			}
			
			if (isSaveBundle == false)
			{
				if (bundle)
					bundle.Unload(false);
				
				bundle = null;
			}
		}
		
		CloseWWW();
	}
	
	public IEnumerator LoadResourceLoaderStage(string url, int version, string bundleName, string stageName)
	{
		string messageStr = string.Format("{0} AssetBundle Loading..", bundleName);
		SetMessage(messageStr);
		
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
			
			Game.Instance.AndroidManager.CallUnityExitWindow(AlertDialogType.NetworkError);	
		}
		else
		{
			Resources.UnloadUnusedAssets();
			yield return 0;
			AssetBundle bundle = loadWWW.assetBundle;
			
			messageStr = string.Format("ResourceLoading Page Loading..");
			SetMessage(messageStr);
			
			mapLoading = Application.LoadLevelAsync(stageName);
			yield return mapLoading;
			
			if (bundle != null)
			{
				bundle.Unload(false);
				bundle = null;
			}
		}
		
		CloseWWW();
	}
}
