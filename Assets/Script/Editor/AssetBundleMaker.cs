using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;

public class AssetBundleMaker
{
	[MenuItem("Tools/BigFoot/MakeAssetBundle")]
	static public void OpenAssetBundleMaker ()
	{
		PatchTableManager patchTableManager = PatchTableManager.Instance;
		if (patchTableManager == null)
			return;
		
		patchTableManager.LoadPatchTable();
		
		EditorWindow.GetWindow<UIAssetBundleMaker>(false, "Patch Table Create", true);
	}
	
	[MenuItem("Tools/BigFoot/ExportToEclipse")]
	static public void ExportToEclipseProject ()
	{
		string[] levels = new string[] {"Assets/Scene/BI.unity", "Assets/Scene/EmptyStart.unity"};
		
		//string rootPath = Application.dataPath.Replace("Assets", "");
		string dateStr = System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm");
		
		//string exportPath = string.Format("{0}Export/{1}", rootPath, dateStr);
		string exportPath = string.Format("X:\\Export\\{0}", dateStr);
		if (System.IO.Directory.Exists(exportPath) == false)
			System.IO.Directory.CreateDirectory(exportPath);
		
		string resultStr = BuildPipeline.BuildPlayer(levels, exportPath, BuildTarget.Android, BuildOptions.AcceptExternalModificationsToPlayer);
		
		string defaultPath = "BigFootEclipse";
		string copyPath = string.Format("X:\\Export\\{0}", defaultPath);
		
		ClearFolder(copyPath);
		
		Copy(exportPath, copyPath);
		
		Debug.Log(resultStr);
	}
	
	/*
	[MenuItem("Tools/BigFoot/DeleteTest")]
	static public void DeleteTest ()
	{
		string exportPath = string.Format("X:\\Export\\{0}", "2014-12-22-18-22");
		
		string defaultPath = "BigFootEclipse";
		string copyPath = string.Format("X:\\Export\\{0}", defaultPath);
		
		ClearFolder(copyPath);
		
		Copy(exportPath, copyPath);
	}
	*/
	
	public static void ClearFolder(string FolderName)
	{
	    System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(FolderName);
	
	    foreach (System.IO.FileInfo fi in dir.GetFiles())
	    {
	        fi.IsReadOnly = false;
	        fi.Delete();
	    }
	
	    foreach (System.IO.DirectoryInfo di in dir.GetDirectories())
	    {
	        ClearFolder(di.FullName);
	        di.Delete();
	    }
	}
	
	public static void Copy(string sourceDir, string targetDir)
	{
		if (System.IO.Directory.Exists(targetDir) == false)
		{
			//infoStr = string.Format("Create DIR {0}", targetDir);
			//Debug.Log(infoStr);
			System.IO.Directory.CreateDirectory(targetDir);
		}
	    
		
	    foreach(var file in System.IO.Directory.GetFiles(sourceDir))
		{
			//infoStr = string.Format("File Copy To {0}", System.IO.Path.Combine(targetDir, System.IO.Path.GetFileName(file)));
			//Debug.Log(infoStr);
			System.IO.File.Copy(file, System.IO.Path.Combine(targetDir, System.IO.Path.GetFileName(file)));
		}
	
	    foreach(var directory in System.IO.Directory.GetDirectories(sourceDir))
		{
			//infoStr = string.Format("Change DIR {0}", System.IO.Path.Combine(targetDir, System.IO.Path.GetFileName(directory)));
			//Debug.Log(infoStr);
			
			Copy(directory, System.IO.Path.Combine(targetDir, System.IO.Path.GetFileName(directory)));
		}
	}
	
	
	
	[MenuItem("Tools/BigFoot/StageSprite")]
	static public void TransStageManagerSprite()
	{
		string themePath = EditorUtility.OpenFolderPanel("Select Theme Folder", "Scene", "");
		
		if (themePath == "")
		{
			Debug.LogError("did not Select Scene Directory!!!!");
			return;
		}
		
		string[] pathsFiles = System.IO.Directory.GetFiles(themePath, "*.unity", System.IO.SearchOption.AllDirectories);
		
		
		string stageSpriteRootPath = "Assets/AssetBundles/StageImages/";
		string texturePath = "";
		
		string themeName = "";
		string fileName = "";
		
		foreach(string path in pathsFiles)
		{
			EditorApplication.OpenScene(path);
			
			StageManager stageManager = (StageManager)GameObject.FindObjectOfType(typeof(StageManager));
			
			bool isChanged = false;
			
			if (stageManager != null)
			{
				BasicSprite[] stageSprites = stageManager.GetComponentsInChildren<BasicSprite>();
				
				foreach(BasicSprite sprite in stageSprites)
				{
					if (sprite != null && sprite.SpriteTexture != null)
					{
						
						themeName = "";
						fileName = "";
						
						texturePath = AssetDatabase.GetAssetPath(sprite.SpriteTexture);
						
						texturePath = texturePath.Replace(stageSpriteRootPath, "");
						
						int pos = texturePath.IndexOf("/");
						if (pos != -1)
							themeName = texturePath.Remove(pos);
						
						fileName = GetFileName(texturePath);
						
						if (themeName != "" && fileName != "")
						{
							string spritePath = string.Format("{0}_Images/{1}", themeName, fileName);
							
							string infoStr = string.Format("BasicSprite texturePath set.. : {0}", spritePath);
							Debug.LogWarning(infoStr);
							
							sprite.texturePath = spritePath;
							
							isChanged = true;
						}
					}	
				}
			}
			
			if (isChanged == true)
				EditorApplication.SaveScene(path);
		}
	}
	
	
	public static string GetFileName(string path)
	{
		string fileName = "";
		
		string[] splits = path.Split('/');
		int nCount = splits.Length;
		
		if (nCount > 0)
			fileName = splits[nCount - 1];
		
		int pos = fileName.LastIndexOf(".");
		if (pos != -1)
			fileName = fileName.Remove(pos);
		
		return fileName;
	}
}
