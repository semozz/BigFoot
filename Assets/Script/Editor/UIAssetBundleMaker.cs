using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class BuildInfo
{
	public enum eBuilType
	{
		Scene,
		Asset,
	}
	public eBuilType buildType = eBuilType.Asset;
	public string defaultName;
	
	public List<string> levels = null;
	public List<UnityEngine.Object> selection = null;
}

public class UIAssetBundleMaker : EditorWindow
{
	string errorString = "";
	
	static string buildCompleteString = "Build Complete!!!";
	static string noNewAssetString = "No new Asset....";
	
	static bool bForceBuild = false;
	
	static bool bAndroidBuildCheck = false;
	static bool bWindowBuildCheck = false;
	static bool bMacBuildCheck = false;
	static bool bIOSBuildCheck = false;

	void OnGUI ()
	{
		string assetRootPath = Application.dataPath.Replace("Assets", "");
		
		BuildTarget origTarget = EditorUserBuildSettings.activeBuildTarget;
		
		PatchTableManager patchTableManager = PatchTableManager.Instance;
		if (patchTableManager == null)
			return;
		
		PatchData startPage = null;
		List<PatchData> patchList = null;
		
		NGUIEditorTools.DrawHeader("BuildTargetInfo");
		bAndroidBuildCheck = EditorGUILayout.Toggle("Android", bAndroidBuildCheck);
		bWindowBuildCheck = EditorGUILayout.Toggle("Window", bWindowBuildCheck);
		bMacBuildCheck = EditorGUILayout.Toggle("Mac", bMacBuildCheck);
		bIOSBuildCheck = EditorGUILayout.Toggle("iOS", bIOSBuildCheck);

		NGUIEditorTools.DrawHeader("PatchTable");
		
		if (GUILayout.Button("AddPatch", GUILayout.Width(100f)))
		{
			if (patchTableManager != null)
				patchTableManager.AddEmptyPatch();
		}
		
		GUILayout.BeginHorizontal();
			patchTableManager.assetBundleURL = EditorGUILayout.TextField("Asset URL", patchTableManager.assetBundleURL);
		GUILayout.EndHorizontal();
		
		/*
		NGUIEditorTools.DrawHeader("ResourceLoadingPage");
		GUILayout.BeginHorizontal();
				GUILayout.Label("BackgroundImage", GUILayout.Width(200f));
				GUILayout.Label("Version", GUILayout.Width(50f));
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
			patchTableManager.resourceLoadImage = EditorGUILayout.TextField(patchTableManager.resourceLoadImage, GUILayout.Width(200f));
			patchTableManager.resourceLoadImageVersion = EditorGUILayout.IntField(patchTableManager.resourceLoadImageVersion, GUILayout.Width(50f));
		GUILayout.EndHorizontal();
		*/
		
		GUILayout.BeginHorizontal();
				GUILayout.Label("Force", GUILayout.Width(50f));
				
				GUILayout.Label("Build", GUILayout.Width(100f));
				GUILayout.Label("Patch Name", GUILayout.Width(150f));
				GUILayout.Label("Patch Path", GUILayout.Width(200f));
				GUILayout.Label("Patch Folder"/*, GUILayout.Width(350f)*/);
				
				GUILayout.Label("FilterType", GUILayout.Width(150f));
				
				GUILayout.Label("Version", GUILayout.Width(50f));
				GUILayout.Label("Pre", GUILayout.Width(50f));
				
				GUILayout.Label("UI Desc", GUILayout.Width(250f));
				
				GUILayout.Label(" ", GUILayout.Width(100f));
		GUILayout.EndHorizontal();
		
		NGUIEditorTools.DrawHeader("StartPage");
		if (patchTableManager.startPage != null)
		{
			startPage = patchTableManager.startPage;
			
			GUILayout.BeginHorizontal();
				startPage.forceBuild = EditorGUILayout.Toggle(startPage.forceBuild, GUILayout.Width(50f));
			
				if (GUILayout.Button("Build", GUILayout.Width(100f)))
				{
					errorString = "";
			
					errorString = Build(startPage);
					
					if (errorString.Contains(buildCompleteString) == true ||
						errorString.Contains(noNewAssetString) == true)
					{
						patchTableManager.SavePatchTable();
					}
					
					EditorUserBuildSettings.SwitchActiveBuildTarget(origTarget);
				}
				
				startPage.name = EditorGUILayout.TextField(startPage.name, GUILayout.Width(150f));
				
				startPage.pathName = EditorGUILayout.TextField(startPage.pathName, GUILayout.Width(200f));
				if (GUILayout.Button(startPage.targetPath/*, GUILayout.Width(350f)*/))
				{
					string targetPath = EditorUtility.OpenFolderPanel("Select Folder", "Path", "");
				
					startPage.targetPath = targetPath.Replace(assetRootPath, "");
				}
			
				startPage.filterType = (PatchData.eFilterType)EditorGUILayout.EnumPopup((PatchData.eFilterType)startPage.filterType, GUILayout.Width(150f));
			
				startPage.version = EditorGUILayout.IntField(startPage.version, GUILayout.Width(50f));
				startPage.preLoading = EditorGUILayout.Toggle(startPage.preLoading, GUILayout.Width(50f));
				
				startPage.desc = EditorGUILayout.TextField(startPage.desc, GUILayout.Width(250f));
				
				GUILayout.Label(" ", GUILayout.Width(100f));
			GUILayout.EndHorizontal();
		}
		
		NGUIEditorTools.DrawHeader("Patch Infos");
		
		if (patchTableManager != null)
			patchList = patchTableManager.patchDataList;
		
		if (patchList != null)
		{
			foreach(PatchData data in patchList)
			{
				GUILayout.BeginHorizontal();
					data.forceBuild = EditorGUILayout.Toggle(data.forceBuild, GUILayout.Width(50f));
				
					if (GUILayout.Button("Build", GUILayout.Width(100f)))
					{
						errorString = "";
				
						errorString = Build(data);
						
						if (errorString.Contains(buildCompleteString) == true ||
							errorString.Contains(noNewAssetString) == true)
						{
							patchTableManager.SavePatchTable();
						}
						
						EditorUserBuildSettings.SwitchActiveBuildTarget(origTarget);
					}
				
					data.name = EditorGUILayout.TextField(data.name, GUILayout.Width(150f));
					
					data.pathName = EditorGUILayout.TextField(data.pathName, GUILayout.Width(200f));
				
					if (GUILayout.Button(data.targetPath/*, GUILayout.Width(350f)*/))
					{
						string targetPath = EditorUtility.OpenFolderPanel("Select Folder", "Path", "");
						data.targetPath = targetPath.Replace(assetRootPath, "");
					}
				
					data.filterType = (PatchData.eFilterType)EditorGUILayout.EnumPopup((PatchData.eFilterType)data.filterType, GUILayout.Width(150f));
				
					data.version = EditorGUILayout.IntField(data.version, GUILayout.Width(50f));
					data.preLoading = EditorGUILayout.Toggle(data.preLoading, GUILayout.Width(50f));
					
					data.desc = EditorGUILayout.TextField(data.desc, GUILayout.Width(250f));
				
					if (GUILayout.Button("Remove", GUILayout.Width(100f)))
					{
						patchTableManager.RemovePatch(data);
						break;
					}
				GUILayout.EndHorizontal();
			}
		}
		
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("ForceBuild", GUILayout.Width(100f)))
		{
			bForceBuild = !bForceBuild;
			
			if (patchTableManager != null)
			{
				startPage = patchTableManager.startPage;
				if (startPage != null)
					startPage.forceBuild = bForceBuild;
				
				patchList = patchTableManager.patchDataList;
				if (patchList != null)
				{
					foreach(PatchData data in patchList)
					{
						data.forceBuild = bForceBuild;
					}
				}
			}
		}
		
		if (GUILayout.Button("BuildPatch", GUILayout.Width(100f)))
		{
			errorString = "";
			
			errorString = BuildPatch();
			
			if (errorString.Contains(buildCompleteString) == true ||
				errorString.Contains(noNewAssetString) == true)
			{
				patchTableManager.SavePatchTable();
			}
			
			EditorUserBuildSettings.SwitchActiveBuildTarget(origTarget);
		}
		GUILayout.EndHorizontal();
		
		NGUIEditorTools.DrawHeader("Result");
		GUILayout.Label(errorString);
	}
	
	string Build(PatchData oneData)
	{
		PatchTableManager patchTableManager = PatchTableManager.Instance;
		List<PatchData> patchList = new List<PatchData>();
		patchList.Add(oneData);
		
		List<BuildTarget> buildTargets = new List<BuildTarget>();
		if (bAndroidBuildCheck == true)
			buildTargets.Add(BuildTarget.Android);
		if (bWindowBuildCheck == true)
			buildTargets.Add(BuildTarget.StandaloneWindows);
		if (bMacBuildCheck == true)
			buildTargets.Add(BuildTarget.StandaloneOSXIntel64);
		if (bIOSBuildCheck == true)
			buildTargets.Add(BuildTarget.iPhone);
		
		List<BuildInfo> buildInfos = new List<BuildInfo>();
		
		string errorStr = "";
		if (patchList != null)
		{
			BuildInfo buildInfo = null;
			
			foreach(PatchData data in patchList)
			{
				if (data.name == "" || data.targetPath == "")
				{
					Debug.LogWarning("Data Name or path is invailid !!!!");
					continue;
				}
				
				switch(data.filterType)
				{
				case PatchData.eFilterType.Type_Scene:
					buildInfo = BuildScenePatch(data);
					break;
				default:
					buildInfo = BuildNormalPatch(data);
					break;
				}
				
				if (buildInfo != null)
					buildInfos.Add(buildInfo);
			}
		}
		
		int buildCont = buildInfos.Count;
		
		System.Text.StringBuilder buildErrors = new System.Text.StringBuilder();
		if (buildCont > 0)
		{
			foreach(BuildTarget target in buildTargets)
			{
				foreach(BuildInfo temp in buildInfos)
				{
					int objectCount = 0;
				
					switch(temp.buildType)
					{
					case BuildInfo.eBuilType.Scene:
						if (temp.levels != null)
							objectCount = temp.levels.Count;
						
						if (objectCount > 0)
						{
							string[] levels = temp.levels.ToArray();
							errorStr = BuildScene(temp.defaultName, levels, target);
						}
						
						if (errorStr != "")
							buildErrors.AppendLine(errorStr);
						break;
					case BuildInfo.eBuilType.Asset:
						if (temp.selection != null)
							objectCount = temp.selection.Count;
						
						if (objectCount > 0)
						{
							UnityEngine.Object mainAsset = temp.selection[0];
							UnityEngine.Object[] selection = temp.selection.ToArray();
							
							errorStr = BuildAssetBundle(temp.defaultName, mainAsset, selection, target);
						}
						
						if (errorStr != "")
							buildErrors.AppendLine(errorStr);
						break;
					}
				}
			}
			
			if (buildErrors.Length == 0)
				buildErrors.AppendLine(buildCompleteString);
			else
				buildErrors.Insert(0, "Build Error is ....");
		}
		else
		{
			buildErrors.AppendLine(noNewAssetString);
		}
		
		string completeTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
		buildErrors.AppendLine(completeTime);
		
		errorStr = buildErrors.ToString();
		
		return errorStr;
	}
	
	string BuildPatch()
	{
		PatchTableManager patchTableManager = PatchTableManager.Instance;
		List<PatchData> patchList = new List<PatchData>();
		if (patchTableManager != null)
		{
			if (patchTableManager.patchDataList != null)
				patchList.AddRange(patchTableManager.patchDataList);
			
			if (patchTableManager.startPage != null)
				patchList.Insert(0, patchTableManager.startPage);
		}
		
		List<BuildTarget> buildTargets = new List<BuildTarget>();
		if (bAndroidBuildCheck == true)
			buildTargets.Add(BuildTarget.Android);
		if (bWindowBuildCheck == true)
			buildTargets.Add(BuildTarget.StandaloneWindows);
		if (bMacBuildCheck == true)
			buildTargets.Add(BuildTarget.StandaloneOSXIntel64);
		if (bIOSBuildCheck == true)
			buildTargets.Add(BuildTarget.iPhone);
		
		List<BuildInfo> buildInfos = new List<BuildInfo>();
		
		string errorStr = "";
		if (patchList != null)
		{
			BuildInfo buildInfo = null;
			
			foreach(PatchData data in patchList)
			{
				if (data.name == "" || data.targetPath == "")
				{
					Debug.LogWarning("Data Name or path is invailid !!!!");
					continue;
				}
				
				switch(data.filterType)
				{
				case PatchData.eFilterType.Type_Scene:
					buildInfo = BuildScenePatch(data);
					break;
				default:
					buildInfo = BuildNormalPatch(data);
					break;
				}
				
				if (buildInfo != null)
					buildInfos.Add(buildInfo);
			}
		}
		
		int buildCont = buildInfos.Count;
		
		System.Text.StringBuilder buildErrors = new System.Text.StringBuilder();
		if (buildCont > 0)
		{
			foreach(BuildTarget target in buildTargets)
			{
				foreach(BuildInfo temp in buildInfos)
				{
					int objectCount = 0;
				
					switch(temp.buildType)
					{
					case BuildInfo.eBuilType.Scene:
						if (temp.levels != null)
							objectCount = temp.levels.Count;
						
						if (objectCount > 0)
						{
							string[] levels = temp.levels.ToArray();
							errorStr = BuildScene(temp.defaultName, levels, target);
						}
						
						if (errorStr != "")
							buildErrors.AppendLine(errorStr);
						break;
					case BuildInfo.eBuilType.Asset:
						if (temp.selection != null)
							objectCount = temp.selection.Count;
						
						if (objectCount > 0)
						{
							UnityEngine.Object mainAsset = temp.selection[0];
							UnityEngine.Object[] selection = temp.selection.ToArray();
							
							errorStr = BuildAssetBundle(temp.defaultName, mainAsset, selection, target);
						}
						
						if (errorStr != "")
							buildErrors.AppendLine(errorStr);
						break;
					}
				}
			}
			
			if (buildErrors.Length == 0)
				buildErrors.AppendLine(buildCompleteString);
			else
				buildErrors.Insert(0, "Build Error is ....");
		}
		else
		{
			buildErrors.AppendLine(noNewAssetString);
		}
		
		string completeTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
		buildErrors.AppendLine(completeTime);
		
		errorStr = buildErrors.ToString();
		
		return errorStr;
	}
	
	string GetFilterString(PatchData.eFilterType type)
	{
		string filterString = "";
		switch(type)
		{
		case PatchData.eFilterType.Type_Scene:
			filterString = "*.unity";
			break;
		case PatchData.eFilterType.Type_Prefab:
			filterString = "*.prefab";
			break;
		case PatchData.eFilterType.Type_TextAsset:
			filterString = "*.txt|*.xml";
			break;
		case PatchData.eFilterType.Type_Texture:
			filterString = "*.tga|*.png|*.jpg";
			break;
		case PatchData.eFilterType.Type_Audio:
			filterString = "*.mp3|*.ogg";
			break;
		case PatchData.eFilterType.Type_Prefab_Texture:
			filterString = "*.prefab|*.tga|*.png|*.jpg";
			break;
		}
		
		return filterString;
	}
	
	bool CheckPathFilter(string path, string[] filters)
	{
		bool isContainFilter = false;
		
		if (filters == null)
		{
			isContainFilter = true;
		}
		else
		{
			path = path.ToLower();
			
			foreach(string filter in filters)
			{
				if (path.Contains(filter) == true)
				{
					isContainFilter = true;
					break;
				}
			}
		}
		
		return isContainFilter;
	}
	
	public string CalcMD5Hash(string filePath, Object obj)
	{
		//string[] filePaths = new string[]{filePath, };
		//string[] dependenciesPath = AssetDatabase.GetDependencies(filePaths);
		string rootPath = Application.dataPath.Replace("Assets", "");
		string path = string.Format("{0}{1}", rootPath, filePath);
		
		System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
        System.IO.FileStream stream = System.IO.File.OpenRead(path);
        string md5Hash = System.BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty).ToLower();
		
		return md5Hash;
	}
	
	/*
	void ProcessDirectory(string targetDirectory) 
    {
        // Process the list of files found in the directory.
        string [] fileEntries = System.IO.Directory.GetFiles(targetDirectory);
        foreach(string fileName in fileEntries)
            ProcessFile(fileName);

        // Recurse into subdirectories of this directory.
        string [] subdirectoryEntries = System.IO.Directory.GetDirectories(targetDirectory);
        foreach(string subdirectory in subdirectoryEntries)
            ProcessDirectory(subdirectory);
    }

    // Insert logic for processing found files here.
    void ProcessFile(string path) 
    {
        //Console.WriteLine("Processed file '{0}'.", path);	    
    }
	*/
	
	bool BuildTest(PatchData data, List<PatchFileTempInfo> objectList)
	{
		//string infoStr = "";
		string replaceString = Application.dataPath.Replace("Assets", "");
		string assetPath = "";//data.targetPath.Replace(replaceString, "");
		
		string filterStr = GetFilterString(data.filterType);
		string[] filters = filterStr.Split('|');
		int nCount = filters.Length;
		for (int index = 0; index < nCount; ++index)
		{
			filters[index] = filters[index].Replace("*", "");
		}
		
		string[] pathsFiles = System.IO.Directory.GetFiles(data.targetPath, "*.*", System.IO.SearchOption.AllDirectories);
		
		foreach(string path in pathsFiles)
		{
			if (path.Contains(".meta") == true)
				continue;
			
			if (CheckPathFilter(path, filters) == false)
				continue;
			
			assetPath = path.Replace(replaceString, "");
			
			UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));
			if (obj != null)
			{
				assetPath = assetPath.Replace("\\", "/");
				
				PatchFileTempInfo newTemp = new PatchFileTempInfo();
				newTemp.filePath = assetPath;
				newTemp.obj = obj;
				
				objectList.Add(newTemp);
			}
		}
		
		bool isAssetChanged = false;
		
		foreach(PatchFileTempInfo info in objectList)
		{
			PatchFileInfo oldInfo = data.GetFileInfo(info.filePath);
			string newMD5Hash = CalcMD5Hash(info.filePath, info.obj);
			
			if (oldInfo == null || oldInfo.md5Hash != newMD5Hash)
			{
				isAssetChanged = true;
				
				data.AddFileInfo(info.filePath, newMD5Hash);
			}
		}
		
		return isAssetChanged;
	}
	
	BuildInfo BuildNormalPatch(PatchData data)
	{
		//string errorMsg = "";
		
		List<PatchFileTempInfo> objectList = new List<PatchFileTempInfo>();
		bool isAssetChanged = BuildTest(data, objectList);
		
		System.Text.StringBuilder addFiles = new System.Text.StringBuilder();
		
		List<Object> assetList = new List<Object>();
		if (isAssetChanged == true || data.forceBuild == true)
		{
			data.forceBuild = false;
			data.version += 1;
			
			foreach(PatchFileTempInfo info in objectList)
			{
				assetList.Add(info.obj);
				
				addFiles.AppendLine(info.filePath);
			}
		}
		
		if (addFiles.Length > 0)
		{
			string title = string.Format("{0} AssetBundle add....", data.name);
			addFiles.Insert(0, title);
			
			Debug.Log(addFiles.ToString());
		}
		
		int assetCount = assetList.Count;
		
		BuildInfo buildInfo = null;
		
		if (assetCount != 0)
		{
			buildInfo = new BuildInfo();
			buildInfo.buildType = BuildInfo.eBuilType.Asset;
			buildInfo.defaultName = data.name;
			buildInfo.selection = assetList;
			
			/*
			UnityEngine.Object mainAsset = assetList[0];
			UnityEngine.Object[] selection = assetList.ToArray();
			
			errorMsg = BuildAssetBundle(data.name, mainAsset, selection, BuildTarget.StandaloneWindows);
			if (errorMsg == "")
				errorMsg = BuildAssetBundle(data.name, mainAsset, selection, BuildTarget.Android);
			*/
		}
		
		return buildInfo;
	}
	
	string BuildAssetBundle(string defaultName, Object mainAsset, Object[] selection, BuildTarget buildTarget)
	{
		string errorMsg = "";
		
		string savePath = MakePatchFileName(defaultName, buildTarget);
		
		bool result = BuildPipeline.BuildAssetBundle(mainAsset, selection, savePath, 
													BuildAssetBundleOptions.CollectDependencies | 
													BuildAssetBundleOptions.CompleteAssets,
													buildTarget
													);
		if (result == false)
			errorMsg = string.Format("BuildAssetBundle error : {0} - {1}", defaultName, buildTarget);
		
		return errorMsg;
	}
	
	BuildInfo BuildScenePatch(PatchData data)
	{
		//string errorMsg = "";
		
		List<PatchFileTempInfo> objectList = new List<PatchFileTempInfo>();
		
		bool isAssetChanged = BuildTest(data, objectList);
		
		BuildInfo buildInfo = null;
		
		if (isAssetChanged == true || data.forceBuild == true)
		{
			data.forceBuild = false;
			data.version += 1;
			
			System.Text.StringBuilder addFiles = new System.Text.StringBuilder();
		
			string replaceString = Application.dataPath.Replace("Assets", "");
			List<string> levels = new List<string>();
			foreach(PatchFileTempInfo info in objectList)
			{
				string sceneName = info.filePath.Replace(replaceString, "");
				levels.Add(sceneName);
				
				addFiles.AppendLine(sceneName);
			}
			
			if (addFiles.Length > 0)
			{
				string title = string.Format("{0} Scene AssetBundle add....", data.name);
				addFiles.Insert(0, title);
				
				Debug.Log(addFiles.ToString());
			}
			
			int assetCount = levels.Count;
			
			if (assetCount > 0)
			{
				buildInfo = new BuildInfo();
				buildInfo.buildType = BuildInfo.eBuilType.Scene;
				buildInfo.defaultName = data.name;
				buildInfo.levels = levels;
				
				/*
				errorMsg = BuildScene(data.name, levels, BuildTarget.StandaloneWindows);
				if (errorMsg == "")
					errorMsg = BuildScene(data.name, levels, BuildTarget.Android);
				*/
			}
		}
		
		return buildInfo;
	}
	
	string BuildScene(string defaultName, string[] levels, BuildTarget buildTarget)
	{
		string savePath = MakePatchFileName(defaultName, buildTarget);
		
		string errorMsg = BuildPipeline.BuildStreamedSceneAssetBundle(levels, savePath, buildTarget);
		
		return errorMsg;
	}
	
	string GetPostfixStr(BuildTarget buildTarget)
	{
		string postfixStr = "";
		switch(buildTarget)
		{
		case BuildTarget.StandaloneWindows:
			postfixStr = "_Win";
			break;
		case BuildTarget.Android:
			postfixStr = "_Android";
			break;
		case BuildTarget.StandaloneOSXIntel64:
			postfixStr = "_OSX";
			break;
		case BuildTarget.iPhone:
			postfixStr = "_iOS";
			break;
		default:
			postfixStr = "_Default";
			break;
		}
		
		return postfixStr;
	}
	
	string MakePatchFileName(string defaultName, BuildTarget buildTarget)
	{
		Debug.Log("MakePatchFileName........");
		string fileName = string.Format("{0}{1}.unity3d", defaultName, GetPostfixStr(buildTarget));
		
		string rootPath = Application.dataPath.Replace("Assets", "");
		string patchDate = System.DateTime.Now.ToString("yyyy-MM-dd");
		
		string patchPath = string.Format("{0}{1}", rootPath, patchDate);
		if (System.IO.Directory.Exists(patchPath) == false)
		{
			Debug.Log("Make Dir : " + patchPath);
			System.IO.Directory.CreateDirectory(patchPath);
		}
		
		string savePath = string.Format("{0}/{1}", patchPath, fileName);
		
		return savePath;
	}
}
