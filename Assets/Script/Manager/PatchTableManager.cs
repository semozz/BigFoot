using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;

[System.Serializable]
public class PatchFileTempInfo
{
	public string filePath;
	public Object obj;
}

[System.Serializable]
public class PatchFileInfo
{
	public string filePath;
	public string md5Hash;
	
	public PatchFileInfo(string path, string md5)
	{
		filePath = path;
		md5Hash = md5;
	}
	
	public PatchFileInfo()
	{
		filePath = "";
		md5Hash = "";
	}
}

[System.Serializable]
public class SavePatchData
{
	public PatchData.eFilterType filterType;
	
	public string name;
	public string pathName;
	
	public string desc;
	
	public string targetPath;
	
	public int version;
	public bool preLoading;
	
	public List<PatchFileInfo> patchFileInfos = new List<PatchFileInfo>();
	
	public SavePatchData()
	{
		name = "";
		targetPath = "";
		
		desc = "";
		
		filterType = PatchData.eFilterType.Type_Prefab;
		
		version = 0;
	}
	
	public void CopyData(PatchData origData)
	{
		this.filterType = origData.filterType;
		this.name = origData.name;
		this.pathName = origData.pathName;
		
		this.desc = origData.desc;
		
		this.targetPath = origData.targetPath;
		this.version = origData.version;
		this.preLoading = origData.preLoading;
	}
}

[System.Serializable]
public class SavePatchTable
{
	public string assetBundleURL = "";
	
	public string resourceLoadImage = "";
	public int resourceLoadImageVersion = 0;
	
	public SavePatchData startPage = new SavePatchData();
	
	public List<SavePatchData> patchDataList = new List<SavePatchData>();
}

public class PatchData
{
	public enum eFilterType
	{
		Type_Scene,
		Type_Prefab,
		Type_TextAsset,
		Type_Texture,
		Type_Audio,
		Type_Prefab_Texture,
	}
	
	public string name;
	public string pathName;
	
	public string desc;
	
	public string targetPath;
	
	public eFilterType filterType;
	
	public int version;
	public bool preLoading;
	
	public bool forceBuild;
	
	public Dictionary<string, PatchFileInfo> patchFileInfos = new Dictionary<string, PatchFileInfo>();
	
	public PatchData()
	{
		name = "";
		targetPath = "";
		
		filterType = eFilterType.Type_Prefab;
		
		version = 0;
		
		preLoading = false;
		forceBuild = false;
	}
	
	public PatchFileInfo GetFileInfo(string filePath)
	{
		PatchFileInfo findInfo = null;
		if (patchFileInfos.ContainsKey(filePath) == true)
			findInfo = patchFileInfos[filePath];
		
		return findInfo;
	}
	
	public void AddFileInfo(string filePath, string md5Hash)
	{
		if (patchFileInfos.ContainsKey(filePath) == false)
		{
			PatchFileInfo newInfo = new PatchFileInfo();
			newInfo.filePath = filePath;
			newInfo.md5Hash = md5Hash;
			
			patchFileInfos.Add(filePath, newInfo);
		}
		else
		{
			patchFileInfos[filePath].md5Hash = md5Hash;
		}
	}
	
	public void CopyData(SavePatchData data)
	{
		this.filterType = data.filterType;
		this.name = data.name;
		this.pathName = data.pathName;
		
		this.desc = data.desc;
		
		this.targetPath = data.targetPath;
		this.version = data.version;
		this.preLoading = data.preLoading;
	}
}

public class PatchTableManager : Singleton<PatchTableManager>
{
	public string assetBundleURL = "";
	
	public string resourceLoadImage = "";
	public int resourceLoadImageVersion = 0;
	
	public PatchData startPage = new PatchData();
	
	public List<PatchData> patchDataList = new List<PatchData>();
	
	public void Init()
	{
		assetBundleURL = "";
		startPage.patchFileInfos.Clear();
		patchDataList.Clear();
	}
	
	public void AddEmptyPatch()
	{
		PatchData newPatch = new PatchData();
		if (newPatch != null)
			patchDataList.Add(newPatch);
	}
	
	public void RemovePatch(PatchData data)
	{
		if (patchDataList != null)
			patchDataList.Remove(data);
	}
	
	public void SavePatchTable()
	{
		SavePatchTable saveData = ConvertSavePatchTable(this);
		
		System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(saveData.GetType());
		
		string rootPath = Application.dataPath.Replace("Assets", "");
		string patchPath = string.Format("{0}{1}", rootPath, "PatchTable");
		if (System.IO.Directory.Exists(patchPath) == false)
			System.IO.Directory.CreateDirectory(patchPath);
		
		string savePath = string.Format("{0}/{1}", patchPath, "PatchTableInfo.xml");
		
		System.IO.TextWriter textWriter = new System.IO.StreamWriter(savePath);
		serializer.Serialize(textWriter, saveData);
		textWriter.Close();
		serializer = null;
		
		
		///////////////////////////////////////////////////////////////////////////////////
		savePath = string.Format("{0}/{1}", patchPath, "AssetBundleInfo.xml");
		AssetBundleVersion assetBundleVersion = ConvertAssetBundleVersion(this);
		
		serializer = new System.Xml.Serialization.XmlSerializer(assetBundleVersion.GetType());
		textWriter = new System.IO.StreamWriter(savePath);
		serializer.Serialize(textWriter, assetBundleVersion);
		textWriter.Close();
		serializer = null;
	}
	
	public static SavePatchTable ConvertSavePatchTable(PatchTableManager patchTable)
	{
		SavePatchTable saveData = new SavePatchTable();
		if (patchTable != null)
		{
			saveData.assetBundleURL = patchTable.assetBundleURL;
			
			if (patchTable.startPage != null)
			{
				saveData.startPage.CopyData(patchTable.startPage);
				foreach(var temp in patchTable.startPage.patchFileInfos)
				{
					PatchFileInfo info = temp.Value;
					saveData.startPage.patchFileInfos.Add(new PatchFileInfo(info.filePath, info.md5Hash));
				}
			}
			
			saveData.resourceLoadImage = patchTable.resourceLoadImage;
			saveData.resourceLoadImageVersion = patchTable.resourceLoadImageVersion;
			
			List<PatchData> dataList = patchTable.patchDataList;
			if (dataList != null)
			{
				foreach(PatchData data in dataList)
				{
					SavePatchData tempData = new SavePatchData();
					tempData.CopyData(data);
					
					foreach(var temp in data.patchFileInfos)
					{
						PatchFileInfo info = temp.Value;
						tempData.patchFileInfos.Add(new PatchFileInfo(info.filePath, info.md5Hash));
					}
					
					saveData.patchDataList.Add(tempData);
				}
			}
			
		}
		
		return saveData;
	}
	
	public static AssetBundleVersion ConvertAssetBundleVersion(PatchTableManager patchTable)
	{
		AssetBundleVersion tempData = null;
		if (patchTable != null)
		{
			tempData = new AssetBundleVersion();
			tempData.assetBundleURL = patchTable.assetBundleURL;
			
			if (patchTable.startPage != null)
			{
				tempData.startPageBundle.assetBundleName = patchTable.startPage.name;
				
				tempData.startPageBundle.desc = patchTable.startPage.desc;
				
				tempData.startPageBundle.version = patchTable.startPage.version;
				tempData.startPageBundle.preLoading = patchTable.startPage.preLoading;
			}
			
			tempData.resourceLoadImage = patchTable.resourceLoadImage;
			tempData.resourceLoadImageVersion = patchTable.resourceLoadImageVersion;
			
			List<PatchData> dataList = patchTable.patchDataList;
			if (dataList != null)
			{
				foreach(PatchData data in dataList)
				{
					AssetBundleInfo bundleInfo = new AssetBundleInfo();
					
					bundleInfo.assetBundleName = data.name;
					bundleInfo.pathName = data.pathName;
					
					bundleInfo.desc = data.desc;
					
					bundleInfo.version = data.version;
					bundleInfo.preLoading = data.preLoading;
					
					bundleInfo.filterType = data.filterType;
					
					tempData.assetBundlInfos.Add(bundleInfo);
				}
			}
		}
		
		return tempData;
	}
	
	public void LoadPatchTable()
	{
		string rootPath = Application.dataPath.Replace("Assets", "");
		string patchPath = string.Format("{0}{1}", rootPath, "PatchTable");
		string savePath = string.Format("{0}/{1}", patchPath, "PatchTableInfo.xml");
		
		if (System.IO.File.Exists(savePath) == false)
		{
			return;
		}
		
		SavePatchTable saveData = new SavePatchTable();
		System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(saveData.GetType());
		
		System.IO.TextReader textReader = new System.IO.StreamReader(savePath);
		saveData = (SavePatchTable)serializer.Deserialize(textReader);
		textReader.Close();
		
		ConvertPatchTableData(saveData, this);
	}
	
	public static void ConvertPatchTableData(SavePatchTable saveData, PatchTableManager tableManager)
	{
		tableManager.Init();
		
		if (saveData != null)
		{
			tableManager.assetBundleURL = saveData.assetBundleURL;
			
			tableManager.resourceLoadImage = saveData.resourceLoadImage;
			tableManager.resourceLoadImageVersion = saveData.resourceLoadImageVersion;
			
			if (saveData.startPage != null)
			{
				tableManager.startPage.CopyData(saveData.startPage);
				
				foreach(PatchFileInfo temp in saveData.startPage.patchFileInfos)
				{
					PatchFileInfo newData = new PatchFileInfo(temp.filePath, temp.md5Hash);
					tableManager.startPage.patchFileInfos.Add(temp.filePath, newData);
				}
			}
			
			List<SavePatchData> dataList = saveData.patchDataList;
			if (dataList != null)
			{
				foreach(SavePatchData data in dataList)
				{
					PatchData tempData = new PatchData();
					tempData.CopyData(data);
					
					foreach(PatchFileInfo temp in data.patchFileInfos)
					{
						PatchFileInfo newData = new PatchFileInfo(temp.filePath, temp.md5Hash);
						tempData.patchFileInfos.Add(temp.filePath, newData);
					}
					
					tableManager.patchDataList.Add(tempData);
				}
			}
			
		}
	}
}

