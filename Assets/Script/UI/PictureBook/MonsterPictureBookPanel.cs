using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterPictureBookPanel : MonoBehaviour {
	public BasePictureBookListWindow parentWindow = null;
	
	public GameObject lockObj = null;
	
	public UILabel nameLabel = null;
	public UILabel attributeLabel = null;
	public UILabel descLabel = null;
	
	public Transform mobRoot = null;
	
	public List<string> actFlagNames = new List<string>();
	
	public string defaultFlagName = "";
	public string GetActFlag(int act)
	{
		string flagName = defaultFlagName;
		int nCount = actFlagNames.Count;
		if (act >= 0 && act < nCount)
			flagName = actFlagNames[act];
		
		return flagName;
	}
	
	public MonsterPictureBookInfo monsterInfo = null;
	public void SetInfo(MonsterPictureBookInfo info)
	{
		monsterInfo = info;
		
		string monsterNameStr = "";
		string attributeInfoStr = "";
		string descStr = "";
		bool isOpen = false;
		
		if (monsterInfo != null && monsterInfo.isOpen == true)
		{
			monsterNameStr = monsterInfo.name;
			attributeInfoStr = monsterInfo.GetAttributeInfo();
			descStr = monsterInfo.desc;
			
			MakeMobInfo(monsterInfo.id);
				
			isOpen = monsterInfo.isOpen;
		}
		
		if (nameLabel != null)
			nameLabel.text = monsterNameStr;
		if (attributeLabel != null)
			attributeLabel.text = attributeInfoStr;
		if (descLabel != null)
			descLabel.text = descStr;
		
		if (lockObj != null)
			lockObj.SetActive(!isOpen);
	}
	
	public GameObject mobFaceObj = null;
	public void MakeMobInfo(int mobFaceID)
	{
		if (mobFaceObj != null)
		{
			DestroyObject(mobFaceObj, 0.0f);
			mobFaceObj = null;
		}
		
		string path = "UI/BossBattleFace/";
		string prefabPath = path + string.Format("{0}", mobFaceID);
			
		mobFaceObj = ResourceManager.CreatePrefab(prefabPath, mobRoot, Vector3.zero);
	}
}
