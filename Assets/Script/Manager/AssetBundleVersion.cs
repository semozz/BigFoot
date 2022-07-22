using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AssetBundleInfo
{
	public string assetBundleName = "";
	public string pathName = "";
	public string desc = "";
	
	public int version = 0;
	public bool preLoading = false;
	
	public PatchData.eFilterType filterType;
	public AssetBundle bundle = null;
	
	public void SetInfo(AssetBundleInfo newInfo)
	{
		this.assetBundleName = newInfo.assetBundleName;
		this.pathName = newInfo.pathName;
		this.desc = newInfo.desc;
		
		this.version = newInfo.version;
		this.preLoading = newInfo.preLoading;
		this.filterType = newInfo.filterType;
		
		if (newInfo.bundle != null)
			this.bundle = newInfo.bundle;		
	}
}

[System.Serializable]
public class AssetBundleVersion {
	public string assetBundleURL = "";
	
	public string resourceLoadImage = "";
	public int resourceLoadImageVersion = 0;
	
	public AssetBundleInfo startPageBundle = new AssetBundleInfo();
	public List<AssetBundleInfo> assetBundlInfos = new List<AssetBundleInfo>();
	
	public AssetBundleInfo GetBundleInfo(string name)
	{
		AssetBundleInfo findInfo = null;
		foreach(AssetBundleInfo info in assetBundlInfos)
		{
			if (info.assetBundleName == name)
			{
				findInfo = info;
				break;
			}
		}
		
		return findInfo;
	}
	
	public AssetBundleInfo GetBundleInfoByPath(string pathName)
	{
		AssetBundleInfo findInfo = null;
		foreach(AssetBundleInfo info in assetBundlInfos)
		{
			if (info.pathName != "" && pathName.StartsWith(info.pathName) == true)
			{
				findInfo = info;
				break;
			}
		}
		
		return findInfo;
	}
	
	public AssetBundleInfo FindBundle(string name)
	{
		AssetBundleInfo findInfo = null;
		foreach(AssetBundleInfo info in assetBundlInfos)
		{
			if (info.assetBundleName == name)
			{
				findInfo = info;
				break;
			}
		}
		
		return findInfo;
	}
	
	public void SetInfo(AssetBundleVersion newInfo)
	{
		if (newInfo == null)
			return;
		
		this.assetBundleURL = newInfo.assetBundleURL;
		this.resourceLoadImage = newInfo.resourceLoadImage;
		this.resourceLoadImageVersion = newInfo.resourceLoadImageVersion;
		
		if (startPageBundle != null)
			startPageBundle.SetInfo(newInfo.startPageBundle);
		else
			startPageBundle = newInfo.startPageBundle;
		
		
		List<AssetBundleInfo> addList = new List<AssetBundleInfo>();
		
		AssetBundleInfo oldInfo = null;
		foreach(AssetBundleInfo info in assetBundlInfos)
		{
			oldInfo = FindBundle(info.assetBundleName);
			if (oldInfo == null)
				addList.Add(info);
			else
				oldInfo.SetInfo(info);
		}
		
		foreach(AssetBundleInfo addInfo in addList)
			assetBundlInfos.Add(addInfo);		
	}
}

public class AssetBundleVersionManager : Singleton<AssetBundleVersionManager>
{
	public AssetBundleVersion assetBundleVersion = null;
	
	public void SetVersionInfo(AssetBundleVersion newInfo)
	{
		if (newInfo == null)
			return;
		
		if (assetBundleVersion == null)
			assetBundleVersion = newInfo;
		else
			assetBundleVersion.SetInfo(newInfo);
	}
	
	public void UpdateDeleteDelay(AssetBundleInfo updateBundle)
	{
		if (assetBundleVersion != null)
		{
			List<AssetBundleInfo> bundleInfos = new List<AssetBundleInfo>();
			bundleInfos.AddRange(assetBundleVersion.assetBundlInfos);
			bundleInfos.Add(assetBundleVersion.startPageBundle);
			
			foreach(AssetBundleInfo info in bundleInfos)
			{
				if (updateBundle != info)
				{
					string url = NetConfig.MakeAssetBundleURL(info.assetBundleName);
					if (Caching.IsVersionCached(url, info.version) == true)
						Caching.MarkAsUsed(url, info.version);
				}
			}
		}
	}
}
