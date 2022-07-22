using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AvatarController : MonoBehaviour {
	public GameObject defaultWeapon = null;
	public List<Transform> weaponNodes = new List<Transform>();
	public List<GameObject> weaponList = new List<GameObject>();
	
	public Animation anim = null;
	
	public GameObject meshNode = null;
	protected Renderer[] meshRenderers = null;
	
	public AnimationEventTrigger animTrigger = null;
	
	void Awake()
	{
		Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();
		FindMeshNode(transforms);
	}
	
	void Start()
	{
		if (animTrigger != null)
			animTrigger.onAnimationEnd = new AnimationEventTrigger.OnAnimationEvent(OnAnimationEnd);
	}
	
	private void FindMeshNode(Transform[] transforms)
	{
		if (transforms == null)
			return;
		
		foreach (Transform trans in transforms)
		{
			if (trans != null && trans.name == "Mesh")
			{
				meshNode = trans.gameObject;
				meshRenderers = meshNode.GetComponentsInChildren<Renderer>();
				break;
			}
		}	
	}
	
	public GameObject CreateObjectByPrefab(GameObject prefab, Transform root)
	{
		GameObject newObj = null;
		if (prefab != null)
		{
			newObj = (GameObject)GameObject.Instantiate(prefab);
			
			if (newObj != null)
			{
				newObj.transform.parent = root;
				
				Transform objTrans = newObj.transform;
				objTrans.localPosition = Vector3.zero;
				objTrans.localRotation = Quaternion.identity;
				objTrans.localScale = Vector3.one;
			}
		}
		
		return newObj;
	}
	
	public void ChangeWeapon(int weaponID)
	{
		foreach(GameObject wpObj in weaponList)
			DestroyObject(wpObj);
		
		weaponList.Clear();
		
		string path = "WP/";
		string fileName = "";
			
		GameObject prefab = null;
		if (weaponID != -1)
		{
			WeaponTable weaponTable = null;
			TableManager tableManager = TableManager.Instance;
			if (tableManager != null)
				weaponTable = tableManager.weaponTable;
			
			if (weaponTable != null)
				fileName = weaponTable.GetData(weaponID);
			
			if (fileName.Contains("not Found!") == true)
				fileName = "";
		}
		
		if (fileName != "")
			prefab = ResourceManager.LoadPrefab(path + fileName);
		
		if (prefab == null)
			prefab = defaultWeapon;
		
		foreach(Transform trans in weaponNodes)
		{
			GameObject newWeapon = CreateObjectByPrefab(prefab, trans);
			weaponList.Add(newWeapon);
		}
	}
	
	public Transform FindNode(string nodeName)
	{
		Transform findNode = null;
		
		Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();
		if (transforms != null)
		{
			foreach (Transform trans in transforms)
			{
				if (trans != null && trans.name == nodeName)
				{
					findNode = trans;
					break;
				}
			}
		}
		
		return findNode;
	}
	
	public GameObject costumeBack = null;
	public GameObject costumeHead = null;
	
	public int costumeBackID = -1;
	public int costumeHeadID = -1;
	
	public Texture defaultCostumeTexture = null;
	public Texture costumeTexture = null;
	public int costumeBodyID = -1;
	public void ChangeCostume(int bodyID, int headID, int backID)
	{
		CostumeTable costumeTable = null;
		TableManager tableManager = TableManager.Instance;
		if (tableManager != null)
			costumeTable = tableManager.costumeTable;
		
		if (costumeTable == null)
			return;
		
		string path = "NewAsset/Costume/";
		
		CostumeInfo info = null;
		Transform prefabNode = null;
		GameObject prefab = null;
		if (costumeBackID != backID)
		{
			costumeBackID = backID;
			
			if (costumeBack != null)
				DestroyObject(costumeBack);
			
			if (backID != -1)
			{
				info = costumeTable.GetData(backID);
				
				if (info != null)
				{
					prefab = ResourceManager.LoadPrefab(path + info.prefabFileName);
					prefabNode = FindNode(info.option);
				}
				
				if (prefab != null)
					costumeBack = CreateObjectByPrefab(prefab, prefabNode);
			}
		}
		
		info = null;
		prefabNode = null;
		prefab = null;
		if (costumeHeadID != headID)
		{
			costumeHeadID = headID;
			
			if (costumeHead != null)
				DestroyObject(costumeHead);
			
			if (headID != -1)
			{
				info = costumeTable.GetData(headID);
				
				if (info != null)
				{
					prefab = ResourceManager.LoadPrefab(path + info.prefabFileName);
					prefabNode = FindNode(info.option);
				}
				
				if (prefab != null)
					costumeHead = CreateObjectByPrefab(prefab, prefabNode);
			}
		}
		
		info = null;
		if (costumeBodyID != bodyID)
		{
			costumeBodyID = bodyID;
			
			info = costumeTable.GetData(bodyID);
			if (info != null)
				costumeTexture = ResourceManager.LoadTexture(path + info.prefabFileName);
			else
				costumeTexture = null;
		}
		if (costumeTexture == null)
			costumeTexture = defaultCostumeTexture;
		
		ChangeCostumeTexture(costumeTexture);
	}
	
	public void ChangeCostumeTexture(Texture costumeTexture)
	{
		if (meshRenderers == null)
			return;
		
		foreach(Renderer renderer in meshRenderers)
		{
			if (renderer.material != null && renderer.material.mainTexture != null)
			{
				renderer.material.mainTexture = costumeTexture;
			}
		}
	}
	
	public string loserPoseAni = "loser_pose";
	public void SetLoseAnim()
	{
		SetAnim(loserPoseAni);
	}
	
	
	public string curAnim = "";
	public void SetAnim(string animName)
	{
		if (anim != null)
		{
			anim.Stop();
			anim.Play(animName);
			
			curAnim = animName;
		}
	}
	
	public string defaultAnim = "stand_pose02";
	public string unSelectAnim = "stand_pose_to_pose02";
	public void OnAnimationEnd()
	{
		if (curAnim == unSelectAnim)
			SetAnim(defaultAnim);
	}
}
