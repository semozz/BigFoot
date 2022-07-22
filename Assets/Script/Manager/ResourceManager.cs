using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
public class ResourceInfo
{
	public enum eResourceType
	{
		PlayerCharacter,
		ArenaMonster,
		Avatar,
		Monster,
		FXObject,
		Sound,
		Texture,
		UI,
		Weapon,
		Costume,
		Mercenary,		
		Others,
	}
	
	public eResourceType type = eResourceType.FXObject;
	public string fileName = "";
}

public class ResourcePrefixInfo
{
	public ResourceInfo.eResourceType type;
	public string prefixPath = "";
	
	public ResourcePrefixInfo(ResourceInfo.eResourceType resType, string prefix)
	{
		this.type = resType;
		this.prefixPath = prefix;
	}
}
*/

public class ResourceManager : MonoSingleton<ResourceManager> {
	
	public static T CreatePrefabByResource<T>(string prefabPath, Transform root, Vector3 vPos) where T : Component
	{
		object comp = null;
		
		GameObject newObj = null;
		GameObject prefab = (GameObject)Resources.Load(prefabPath);
		if (prefab != null)
			newObj = (GameObject)Instantiate(prefab);
		
		Vector3 origPos = Vector3.zero;
		Vector3 origScale = Vector3.one;
		Quaternion origQuat = Quaternion.identity;
		if (newObj != null)
		{
			origPos = newObj.transform.localPosition;
			origScale = newObj.transform.localScale;
			origQuat = newObj.transform.localRotation;
		}
		
		if (newObj != null)
		{
			if (root != null)
				newObj.transform.parent = root;
			
			newObj.transform.localPosition = vPos == Vector3.zero ? origPos : vPos;
			newObj.transform.localScale = origScale;
			newObj.transform.localRotation = origQuat;
			
			comp = newObj.GetComponent<T>();
		}
		
		return (T)comp;
	}
	
	public static T CreatePrefab<T>(GameObject prefab) where T : Component
	{
		object comp = null;
		
		GameObject newObj = null;
		if (prefab != null)
			newObj = (GameObject)Instantiate(prefab);
		if (newObj != null)
			comp = newObj.GetComponent<T>();
		
		return (T)comp;
	}
	
	public static T CreatePrefab<T>(GameObject prefab, Transform root, Vector3 vPos) where T : Component
	{
		object comp = null;
		
		GameObject newObj = null;
		if (prefab != null)
			newObj = (GameObject)Instantiate(prefab);
		
		Vector3 origPos = Vector3.zero;
		Vector3 origScale = Vector3.one;
		Quaternion origQuat = Quaternion.identity;
		if (newObj != null)
		{
			origPos = newObj.transform.localPosition;
			origScale = newObj.transform.localScale;
			origQuat = newObj.transform.localRotation;
		}
		
		if (newObj != null)
		{
			if (root != null)
				newObj.transform.parent = root;
			
			newObj.transform.localPosition = vPos == Vector3.zero ? origPos : vPos;
			newObj.transform.localScale = origScale;
			newObj.transform.localRotation = origQuat;
			
			comp = newObj.GetComponent<T>();
		}
		
		return (T)comp;
	}
	
	public static GameObject CreatePrefab(GameObject prefabObj, Transform root, Vector3 vPos)
	{
		GameObject newObj = null;
		if (prefabObj != null)
			newObj = (GameObject)Instantiate(prefabObj);
		
		Vector3 origPos = Vector3.zero;
		Vector3 origScale = Vector3.one;
		Quaternion origQuat = Quaternion.identity;
		if (newObj != null)
		{
			origPos = newObj.transform.localPosition;
			origScale = newObj.transform.localScale;
			origQuat = newObj.transform.localRotation;
		}
		
		if (newObj != null)
		{
			if (root != null)
				newObj.transform.parent = root;
			
			newObj.transform.localPosition = vPos == Vector3.zero ? origPos : vPos;;
			newObj.transform.localScale = origScale;
			newObj.transform.localRotation = origQuat;
		}
		
		return newObj;
	}
	
	public static string GetFileName(string path)
	{
		string fileName = "";
		
		string[] splits = path.Split('/');
		int nCount = splits.Length;
		
		if (nCount > 0)
			fileName = splits[nCount - 1];
		
		return fileName;
	}
	
	public static AssetBundleInfo GetAssetBundleInfo(string path)
	{
		AssetBundleInfo bundleInfo = null;
		
		AssetBundleVersion assetBundleVersion = null;
		AssetBundleVersionManager assetBundleVersionMgr = AssetBundleVersionManager.Instance;
		if (assetBundleVersionMgr != null)
			assetBundleVersion = assetBundleVersionMgr.assetBundleVersion;
		
		if (assetBundleVersion != null)
			bundleInfo = assetBundleVersion.GetBundleInfoByPath(path);
		
		return bundleInfo;
	}
	
	public static Object LoadObjectFromAssetBundle(string path)
	{
		string fileName = GetFileName(path);
		
		AssetBundleInfo bundleInfo = GetAssetBundleInfo(path);
		
		Object loadObj = null;
		if (bundleInfo != null && bundleInfo.bundle == true)
			loadObj = bundleInfo.bundle.Load(fileName);
		
		if (loadObj == null)
		{
			string infoStr = string.Format("{0} matching Bundle Not Found !!!", path);
			Debug.LogError(infoStr);
		}
		
		return loadObj;
	}
	
	public static GameObject LoadPrefabFromAssetBundle(string path)
	{
		string fileName = GetFileName(path);
		
		AssetBundleInfo bundleInfo = GetAssetBundleInfo(path);
		
		GameObject loadObj = null;
		if (bundleInfo != null && bundleInfo.bundle == true)
			loadObj = (GameObject)bundleInfo.bundle.Load(fileName, typeof(GameObject));
		
		if (loadObj == null)
		{
			string infoStr = string.Format("{0} matching Bundle Not Found !!!", path);
			Debug.LogError(infoStr);
		}
		
		return loadObj;
	}
	
	public static Texture2D LoadTextureFromAssetBundle(string path)
	{
		string fileName = GetFileName(path);
		
		AssetBundleInfo bundleInfo = GetAssetBundleInfo(path);
		
		Texture2D loadObj = null;
		if (bundleInfo != null && bundleInfo.bundle == true)
			loadObj = (Texture2D)bundleInfo.bundle.Load(fileName, typeof(Texture2D));
		
		
		if (loadObj == null)
		{
			string infoStr = string.Format("{0} matching Bundle Not Found !!!", path);
			Debug.LogError(infoStr);
		}
		
		return loadObj;
	}
	
	public static AudioClip LoadAudioFromAssetBundle(string path)
	{
		string fileName = GetFileName(path);
		
		AssetBundleInfo bundleInfo = GetAssetBundleInfo(path);
		
		AudioClip loadObj = null;
		if (bundleInfo != null && bundleInfo.bundle == true)
			loadObj = (AudioClip)bundleInfo.bundle.Load(fileName, typeof(AudioClip));
		
		
		if (loadObj == null)
		{
			string infoStr = string.Format("{0} matching Bundle Not Found !!!", path);
			Debug.LogError(infoStr);
		}
		
		return loadObj;
	}
	
	public static TextAsset LoadTextAssetFromAssetBundle(string path)
	{
		string fileName = GetFileName(path);
		
		AssetBundleInfo bundleInfo = GetAssetBundleInfo(path);
		
		TextAsset loadObj = null;
		if (bundleInfo != null && bundleInfo.bundle == true)
			loadObj = (TextAsset)bundleInfo.bundle.Load(fileName, typeof(TextAsset));
		
		
		if (loadObj == null)
		{
			string infoStr = string.Format("{0} matching Bundle Not Found !!!", path);
			Debug.LogError(infoStr);
		}
		
		return loadObj;
	}
	
	public static T CreatePrefab<T>(string path) where T : Component
	{
		GameObject prefabObj = LoadPrefab(path);//LoadPrefabFromAssetBundle(path);
		
		return CreatePrefab<T>(prefabObj);
	}

	public static T CreatePrefab<T>(string path, Transform root) where T : Component
	{
		GameObject prefabObj = LoadPrefab(path);//LoadPrefabFromAssetBundle(path);
		
		return CreatePrefab<T>(prefabObj, root, Vector3.zero);
	}
	
	public static T CreatePrefab<T>(string path, Transform root, Vector3 vPos) where T : Component
	{
		GameObject prefabObj = LoadPrefab(path);//LoadPrefabFromAssetBundle(path);
		
		return CreatePrefab<T>(prefabObj, root, vPos);
	}
	
	public static GameObject CreatePrefab(string path, Transform root)
	{
		GameObject prefabObj = LoadPrefab(path);//LoadPrefabFromAssetBundle(path);
		
		return CreatePrefab(prefabObj, root, Vector3.zero);
	}
	
	public static GameObject CreatePrefab(string path, Transform root, Vector3 vPos)
	{
		GameObject prefabObj = LoadPrefab(path);//LoadPrefabFromAssetBundle(path);
		
		return CreatePrefab(prefabObj, root, vPos);
	}
	
	public static GameObject CreatePrefab(string path)
	{
		GameObject prefabObj = LoadPrefab(path);//LoadPrefabFromAssetBundle(path);
		
		GameObject newObj = null;
		if (prefabObj != null)
			newObj = (GameObject)Instantiate(prefabObj);
		
		return newObj;
	}
	
	public static GameObject LoadPrefab(string path)
	{
#if UNITY_EDITOR
		string newPath = path;
		
		if (path.StartsWith("NewAsset/") == true)
			newPath = string.Format("Assets/{0}.prefab", path.Replace("NewAsset/", "AssetBundles/"));
		else
			newPath = string.Format("Assets/AssetBundles/{0}.prefab", path);
		
		GameObject obj = UnityEditor.AssetDatabase.LoadAssetAtPath(newPath, (typeof(GameObject))) as GameObject;
		
		return obj;
		
#else
		return LoadPrefabFromAssetBundle(path);
#endif
	}
	
	public static Texture2D LoadTexture(string path)
	{
#if UNITY_EDITOR
		string newPath = path;
		
		if (path.StartsWith("NewAsset/") == true)
			newPath = string.Format("Assets/{0}.png", path.Replace("NewAsset/", "AssetBundles/"));
		else
			newPath = string.Format("Assets/AssetBundles/{0}.png", path);
		
		Texture2D obj = UnityEditor.AssetDatabase.LoadAssetAtPath(newPath, (typeof(Texture2D))) as Texture2D;
		
		return obj;
		
#else
		return LoadTextureFromAssetBundle(path);
#endif
	}
	
	public static AudioClip LoadAudio(string path)
	{
#if UNITY_EDITOR
		string newPath = path;
		
		if (path.StartsWith("NewAsset/") == true)
			newPath = string.Format("Assets/{0}.ogg", path.Replace("NewAsset/", "AssetBundles/"));
		else
			newPath = string.Format("Assets/AssetBundles/{0}.ogg", path);
		
		AudioClip obj = UnityEditor.AssetDatabase.LoadAssetAtPath(newPath, (typeof(AudioClip))) as AudioClip;
		
		return obj;
		
#else
		return LoadAudioFromAssetBundle(path);
#endif
	}
	
	public static TextAsset LoadTextAsset(string path)
	{
#if UNITY_EDITOR
		string newPath = string.Format("Assets/{0}", path.Replace("NewAsset/", "AssetBundles/"));
		TextAsset obj = UnityEditor.AssetDatabase.LoadAssetAtPath(newPath, (typeof(TextAsset))) as TextAsset;
		
		return obj;
		
#else
		return LoadTextAssetFromAssetBundle(path);
#endif
	}
}
