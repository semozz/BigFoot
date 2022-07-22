using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArenaLoadingWindow : MonoBehaviour {
	public AvatarCam myAvatarCam = null;
	public AvatarController myAvatar = null;
	
	public AvatarCam enemyAvatarCam = null;
	public AvatarController enemyAvatar = null;
	
	public ArenaMyInfo myRankInfo = null;
	public ArenaMyInfo enemyRankInfo = null;
	
	public UILabel myLevelInfoLabel = null;
	public UILabel myCharNameLabel = null;
	
	public UILabel enemyLevelInfoLabel = null;
	public UILabel enemyCharNameLabel = null;
	
	public void Start()
	{
		//InitWindow(GameDef.ePlayerClass.CLASS_WARRIOR, null, GameDef.ePlayerClass.CLASS_ASSASSIN, null);
	}
	
	public void InitWindow(GameDef.ePlayerClass myClass, List<EquipInfo> myEquips, CostumeSetItem myCostumeSetItem, GameDef.ePlayerClass enemyClass, List<EquipInfo> enemyEquips, CostumeSetItem enemyCostumeSetItem)
	{
		SetMyAvatarInfo(myClass, myEquips, myCostumeSetItem);
		SetEnemyAvatarInfo(enemyClass, enemyEquips, enemyCostumeSetItem);
	}
	
	public void InitWindow(int myRankType, int myRanking, CharPrivateData myPrivateData, int enemyRankType, int enemyRanking, CharPrivateData enemyPrivateData)
	{
		SetMyAvatarInfo((GameDef.ePlayerClass)myPrivateData.baseInfo.CharacterIndex, myPrivateData.equipData, myPrivateData.costumeSetItem);
		SetEnemyAvatarInfo((GameDef.ePlayerClass)enemyPrivateData.baseInfo.CharacterIndex, enemyPrivateData.equipData, enemyPrivateData.costumeSetItem);
		
		TableManager tableManager = TableManager.Instance;
		CharExpTable expTable = tableManager != null ? tableManager.charExpTable : null;
		
		int myLevel = 0;
		int enemyLevel = 0;
		
		if (expTable != null)
		{
			myLevel = expTable.GetLevel(myPrivateData.baseInfo.ExpValue);
			enemyLevel = expTable.GetLevel(enemyPrivateData.baseInfo.ExpValue);
		}
		
		SetMyCharInfo(myLevel, (GameDef.ePlayerClass)myPrivateData.baseInfo.CharacterIndex, myPrivateData.NickName);
		SetEnemyCharInfo(enemyLevel, (GameDef.ePlayerClass)enemyPrivateData.baseInfo.CharacterIndex, enemyPrivateData.NickName);
		
		if (myRankInfo != null)
			myRankInfo.SetMyInfo(myPrivateData.arenaInfo);
		if (enemyRankInfo != null)
			enemyRankInfo.SetMyInfo(enemyPrivateData.arenaInfo);
	}
	
	public void SetMyCharInfo(int level, GameDef.ePlayerClass classType, string charName)
	{
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		string className = "";
		if (stringTable != null)
			className = stringTable.GetData((int)classType + 1);
		
		string levelInfoStr = string.Format("Lv.{0} {1}", level, className);
		
		if (myLevelInfoLabel != null)
			myLevelInfoLabel.text = levelInfoStr;
		
		if (myCharNameLabel != null)
			myCharNameLabel.text = charName;
	}
	
	public void SetEnemyCharInfo(int level, GameDef.ePlayerClass classType, string charName)
	{
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		string className = "";
		if (stringTable != null)
			className = stringTable.GetData((int)classType + 1);
		
		string levelInfoStr = string.Format("Lv.{0} {1}", level, className);
		
		if (enemyLevelInfoLabel != null)
			enemyLevelInfoLabel.text = levelInfoStr;
		
		if (enemyCharNameLabel != null)
			enemyCharNameLabel.text = charName;
	}
	
	public void SetMyAvatarInfo(GameDef.ePlayerClass classType, List<EquipInfo> equipItems, CostumeSetItem costumeSetItem)
	{
		if (myAvatarCam != null)
		{
			myAvatarCam.playerClass = classType;
			myAvatarCam.ChangeAvatar();
			
			myAvatar = myAvatarCam.avatar;
		}
		
		SetAvatar(myAvatar, equipItems, costumeSetItem);
	}
	
	public void SetEnemyAvatarInfo(GameDef.ePlayerClass classType, List<EquipInfo> equipItems, CostumeSetItem costumeSetItem)
	{
		if (enemyAvatarCam != null)
		{
			enemyAvatarCam.playerClass = classType;
			enemyAvatarCam.ChangeAvatar();
			
			enemyAvatar = enemyAvatarCam.avatar;
		}
		
		SetAvatar(enemyAvatar, equipItems, costumeSetItem);
	}
	
	public void SetAvatar(AvatarController avatar, List<EquipInfo> equipItems, CostumeSetItem costumeSetItem)
	{
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
