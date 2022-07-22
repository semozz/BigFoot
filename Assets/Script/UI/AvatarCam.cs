using UnityEngine;
using System.Collections;

public class AvatarCam : SimpleAvatarCam {
	public string warriorPrefabPath = "NewAsset/Avatar/Warrior";
	public string assassinPrefabPath = "NewAsset/Avatar/Assassin";
	public string wizardPrefabPath = "NewAsset/Avatar/Wizard";
	
	public GameDef.ePlayerClass playerClass = GameDef.ePlayerClass.CLASS_WARRIOR;
	
	public Transform buttons = null;
	
	public int nCount = 0;
	public void UpdateButtons()
	{
		if (buttons != null)
		{
			if ((nCount % 2) == 0)
				buttons.localPosition += new Vector3(0.0f, 0.0f, 1.01f);
			else
				buttons.localPosition -= new Vector3(0.0f, 0.0f, 1.01f);
			
			
			
			nCount++;
		}
	}
	
	public void ChangeToWarrior(bool bCheck)
	{
		if (bCheck == false)
			return;
		
		Game.Instance.playerClass = playerClass = GameDef.ePlayerClass.CLASS_WARRIOR;
		if (Game.Instance.connector != null)
			Game.Instance.connector.charIndex = 0;
		
		CharInfoData charData = Game.Instance.charInfoData;
		int charIndex = 0;
		CharPrivateData privateData = null;
		if (charData != null)
		{
			privateData = charData.GetPrivateData(charIndex);
			
			privateData.playerClass = GameDef.ePlayerClass.CLASS_WARRIOR;
		}
		
		ChangeAvatar();
		
		UpdateButtons();
	}
	
	public void ChangeToAssassin(bool bCheck)
	{
		if (bCheck == false)
			return;
		
		Game.Instance.playerClass = playerClass = GameDef.ePlayerClass.CLASS_ASSASSIN;
		Game.Instance.connector.charIndex = 1;
		
		CharInfoData charData = Game.Instance.charInfoData;
		int charIndex = 1;
		CharPrivateData privateData = null;
		if (charData != null)
		{
			privateData = charData.GetPrivateData(charIndex);
			
			privateData.playerClass = GameDef.ePlayerClass.CLASS_ASSASSIN;
		}
		
		ChangeAvatar();
		
		UpdateButtons();
	}
	
	public void ChangeToWizard(bool bCheck)
	{
		if (bCheck == false)
			return;
		
		Game.Instance.playerClass = playerClass = GameDef.ePlayerClass.CLASS_WIZARD;
		Game.Instance.connector.charIndex = 2;
		
		CharInfoData charData = Game.Instance.charInfoData;
		int charIndex = 2;
		CharPrivateData privateData = null;
		if (charData != null)
		{
			privateData = charData.GetPrivateData(charIndex);
			
			privateData.playerClass = GameDef.ePlayerClass.CLASS_WIZARD;
		}
		
		ChangeAvatar();
		
		UpdateButtons();
	}
	
	public void ChangeAvatar()
	{
		if (avatar != null)
		{
			DestroyImmediate(avatar.gameObject);
			avatar = null;
		}
		
		GameObject prefab = null;
		switch(playerClass)
		{
		case GameDef.ePlayerClass.CLASS_WARRIOR:
			prefab = ResourceManager.LoadPrefab(warriorPrefabPath);
			break;
		case GameDef.ePlayerClass.CLASS_ASSASSIN:
			prefab = ResourceManager.LoadPrefab(assassinPrefabPath);
			break;
		case GameDef.ePlayerClass.CLASS_WIZARD:
			prefab = ResourceManager.LoadPrefab(wizardPrefabPath);
			break;
		}
		
		Game.Instance.CreateCharacter(Vector3.zero);
		
		int charIndex = -1;
		CharInfoData charData = Game.Instance.charInfoData;
		//CharPrivateData privateData = null;
		if (charData != null && Game.Instance.connector != null)
		{
			charIndex = Game.Instance.connector.charIndex;
			//privateData = charData.GetPrivateData(charIndex);
		}
		
		if (prefab != null)
		{
			GameObject avatarObj = (GameObject)Instantiate(prefab);
			
			if (avatarObj != null)
			{
				avatarObj.transform.parent = avatarNode;
				
				avatarObj.transform.localPosition = Vector3.zero;
				avatarObj.transform.localScale = Vector3.one;
				//avatarObj.transform.localRotation = Quaternion.identity;
				
				avatar = avatarObj.GetComponent<AvatarController>();
				
				ResetDefaultCostume(charIndex, playerClass);
			}
		}
		
		if (cam != null)
		{
			Vector3 targetPos = avatarNode.position;
			targetPos += targetPosAdjust * delta;
			
			cam.transform.LookAt(targetPos);
		}
	}
	
	public void ChangeAvatar(GameDef.ePlayerClass classType)
	{
		if (playerClass == classType)
			return;
		
		if (avatar != null)
		{
			DestroyImmediate(avatar.gameObject);
			avatar = null;
		}
		
		GameObject prefab = null;
		playerClass = classType;
		switch(playerClass)
		{
		case GameDef.ePlayerClass.CLASS_WARRIOR:
			prefab = ResourceManager.LoadPrefab(warriorPrefabPath);
			break;
		case GameDef.ePlayerClass.CLASS_ASSASSIN:
			prefab = ResourceManager.LoadPrefab(assassinPrefabPath);
			break;
		case GameDef.ePlayerClass.CLASS_WIZARD:
			prefab = ResourceManager.LoadPrefab(wizardPrefabPath);
			break;
		}
		
		int charIndex = (int)playerClass;
		
		if (prefab != null)
		{
			GameObject avatarObj = (GameObject)Instantiate(prefab);
			
			if (avatarObj != null)
			{
				avatarObj.transform.parent = avatarNode;
				
				avatarObj.transform.localPosition = Vector3.zero;
				avatarObj.transform.localScale = Vector3.one;
				//avatarObj.transform.localRotation = Quaternion.identity;
				
				avatar = avatarObj.GetComponent<AvatarController>();
				
				ResetDefaultCostume(charIndex, playerClass);
			}
		}
		
		if (cam != null)
		{
			Vector3 targetPos = avatarNode.position;
			targetPos += targetPosAdjust * delta;
			
			cam.transform.LookAt(targetPos);
		}
	}
	
	public void ChangeAvatarByCostume(int charIndex, GameDef.ePlayerClass costumeClass)
	{
		if (avatar != null)
		{
			DestroyImmediate(avatar.gameObject);
			avatar = null;
		}
		
		this.playerClass = costumeClass;
		
		GameObject prefab = null;
		switch(costumeClass)
		{
		case GameDef.ePlayerClass.CLASS_WARRIOR:
			prefab = ResourceManager.LoadPrefab(warriorPrefabPath);
			break;
		case GameDef.ePlayerClass.CLASS_ASSASSIN:
			prefab = ResourceManager.LoadPrefab(assassinPrefabPath);
			break;
		case GameDef.ePlayerClass.CLASS_WIZARD:
			prefab = ResourceManager.LoadPrefab(wizardPrefabPath);
			break;
		}
		
		if (prefab != null)
		{
			GameObject avatarObj = (GameObject)Instantiate(prefab);
			
			if (avatarObj != null)
			{
				avatarObj.transform.parent = avatarNode;
				
				avatarObj.transform.localPosition = Vector3.zero;
				avatarObj.transform.localScale = Vector3.one;
				//avatarObj.transform.localRotation = Quaternion.identity;
				
				avatar = avatarObj.GetComponent<AvatarController>();
				
				ResetDefaultCostume(charIndex, costumeClass);
			}
		}
		
		if (cam != null)
		{
			Vector3 targetPos = avatarNode.position;
			targetPos += targetPosAdjust * delta;
			
			cam.transform.LookAt(targetPos);
		}
	}
	
	public void ResetDefaultCostume(int charIndex, GameDef.ePlayerClass costumeClass)
	{
		int weaponID = -1;
		int bodyID = -1;
		int headID =-1;
		int backID = -1;
		
		CharInfoData charData = Game.Instance.charInfoData;
		CharPrivateData privateData = null;
		if (charData != null)
			privateData = charData.GetPrivateData(charIndex);
		
		if (privateData != null && avatar != null)
		{
			foreach(EquipInfo info in privateData.equipData)
			{
				switch(info.slotType)
				{
				case GameDef.eSlotType.Weapon:
					if (info.item != null && info.item.itemInfo != null && privateData.playerClass == costumeClass)
						weaponID = info.item.itemInfo.itemID;
					break;
				case GameDef.eSlotType.Costume_Body:
					if (info.item != null && info.item.itemInfo != null)
						bodyID = info.item.itemInfo.itemID;
					break;
				case GameDef.eSlotType.Costume_Head:
					if (info.item != null && info.item.itemInfo != null)
						headID = info.item.itemInfo.itemID;
					break;
				case GameDef.eSlotType.Costume_Back:
					if (info.item != null && info.item.itemInfo != null)
						backID = info.item.itemInfo.itemID;
					break;
				}
			}
			
			
			if (charIndex == (int)costumeClass)
			{
				CostumeSetItem costumeSetItem = privateData.costumeSetItem;
				
				if (costumeSetItem != null)
				{
					foreach(Item item in costumeSetItem.items)
					{
						if (item == null || item.itemInfo == null)
							continue;
						
						switch(item.itemInfo.itemType)
						{
						case ItemInfo.eItemType.Costume_Back:
							backID = item.itemInfo.itemID;
							break;
						case ItemInfo.eItemType.Costume_Body:
							bodyID = item.itemInfo.itemID;
							break;
						case ItemInfo.eItemType.Costume_Head:
							headID = item.itemInfo.itemID;
							break;
						}
					}
				}
			}
			
			avatar.ChangeCostume(bodyID, headID, backID);
			avatar.ChangeWeapon(weaponID);
		}
	}
}
