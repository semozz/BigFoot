using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelUpWindow : BaseLevelUpWindow {
	public UILabel nextLevelLabel = null;
	
	public AvatarCam avatarCam = null;
	
	public void SetNextLevel(int nextLevel)
	{
		if (nextLevelLabel != null)
			nextLevelLabel.text = string.Format("{0}", nextLevel);
	}
	
	public void SetAvatar(GameDef.ePlayerClass classType, List<EquipInfo> equipItems, CostumeSetItem costumeSetItem)
	{
		if (avatarCam != null)
		{
			avatarCam.playerClass = classType;
			avatarCam.ChangeAvatar();
			
			SetEquipItems(avatarCam.avatar, equipItems, costumeSetItem);
		}
	}
	
	public void SetEquipItems(AvatarController avatar, List<EquipInfo> equipItems, CostumeSetItem costumeSetItem)
	{
		if (equipItems == null)
			return;
		
		int weaponID = -1;
		int costumeBackID = -1;
		int costumeHeadID = -1;
		int costumeBodyID = -1;
		
		int index = 0;
		if (equipItems != null)
		{
			foreach(EquipInfo info in equipItems)
			{
				switch(info.slotType)
				{
				case GameDef.eSlotType.Weapon:
					if (info.item != null)
						weaponID = info.item.itemInfo.itemID;
					break;
				case GameDef.eSlotType.Costume_Body:
					if (info.item != null)
						costumeBodyID = info.item.itemInfo.itemID;
					break;
				case GameDef.eSlotType.Costume_Back:
					if (info.item != null)
						costumeBackID = info.item.itemInfo.itemID;
					break;
				case GameDef.eSlotType.Costume_Head:
					if (info.item != null)
						costumeHeadID = info.item.itemInfo.itemID;
					break;
				}
				
				++index;
			}
		}
		
		if (costumeSetItem != null)
		{
			foreach(Item item in costumeSetItem.items)
			{
				if (item == null || item.itemInfo == null)
					continue;
				
				switch(item.itemInfo.itemType)
				{
				case ItemInfo.eItemType.Costume_Back:
					costumeBackID = item.itemInfo.itemID;
					break;
				case ItemInfo.eItemType.Costume_Body:
					costumeBodyID = item.itemInfo.itemID;
					break;
				case ItemInfo.eItemType.Costume_Head:
					costumeHeadID = item.itemInfo.itemID;
					break;
				}
			}
		}
		
		if (avatar != null)
		{
			avatar.ChangeCostume(costumeBodyID, costumeHeadID, costumeBackID);
			avatar.ChangeWeapon(weaponID);
		}
	}
}
