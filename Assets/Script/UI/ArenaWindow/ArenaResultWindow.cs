using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArenaResultWindow : MonoBehaviour {
	public string stageName = "TownTest";
	
	public Transform popupNode = null;
	
	public GameObject winFX = null;
	public GameObject loseFX = null;
	
	public ArenaMyInfo myArenaInfo = null;
	public MyCharInfo myCharInfo = null;
	
	public ArenaMyInfo enemyArenaInfo = null;
	public MyCharInfo enemyCharInfo = null;
	
	public AvatarCam myAvatarCam = null;
	public AvatarController myAvatar = null;
	
	public AvatarCam enemyAvatarCam = null;
	public AvatarController enemyAvatar = null;
	
	void Awake()
	{
		/*
		if (myArenaInfo != null)
			myArenaInfo.SetMyInfo(null);
		if (myCharInfo != null)
			myCharInfo.SetCharInfo(1, GameDef.ePlayerClass.CLASS_WARRIOR, "My");
		
		if (enemyArenaInfo != null)
			enemyArenaInfo.SetMyInfo(null);
		if (enemyCharInfo != null)
			enemyCharInfo.SetCharInfo(1, GameDef.ePlayerClass.CLASS_WARRIOR, "Enemy");
		
		SetMyAvatarInfo(GameDef.ePlayerClass.CLASS_WARRIOR, null);
		SetEnemyAvatarInfo(GameDef.ePlayerClass.CLASS_WARRIOR, null);
		*/
		
		SetFX(winFX, false);
		SetFX(loseFX, false);
	}
	
	public void InitWindow(CharPrivateData myPrivate, ArenaInfo myArena, CharPrivateData enemyPrivate, ArenaInfo enymyArena, bool bWin)
	{
		TableManager tableManager = TableManager.Instance;
		CharExpTable expTable = tableManager != null ? tableManager.charExpTable : null;
		
		int myCharLevel = 0;
		int enemyCharLevel = 0;
		if (expTable != null && myPrivate != null && enemyPrivate != null)
		{
			myCharLevel = expTable.GetLevel(myPrivate.baseInfo.ExpValue);
			enemyCharLevel = expTable.GetLevel(enemyPrivate.baseInfo.ExpValue);
		}
		
		if (myArenaInfo != null)
			myArenaInfo.SetMyInfo(myArena);
		if (myCharInfo != null)
			myCharInfo.SetCharInfo(myCharLevel, (GameDef.ePlayerClass)myPrivate.baseInfo.CharacterIndex, myPrivate.NickName);
		
		if (enemyArenaInfo != null)
			enemyArenaInfo.SetMyInfo(enymyArena);
		if (enemyCharInfo != null)
			enemyCharInfo.SetCharInfo(enemyCharLevel, (GameDef.ePlayerClass)enemyPrivate.baseInfo.CharacterIndex, enemyPrivate.NickName);
		
		SetMyAvatarInfo((GameDef.ePlayerClass)myPrivate.baseInfo.CharacterIndex, myPrivate.equipData, myPrivate.costumeSetItem, bWin);
		SetEnemyAvatarInfo((GameDef.ePlayerClass)enemyPrivate.baseInfo.CharacterIndex, enemyPrivate.equipData, enemyPrivate.costumeSetItem, !bWin);
		
		SetFX(winFX, bWin);
		SetFX(loseFX, !bWin);
	}
	
	public void SetMyAvatarInfo(GameDef.ePlayerClass classType, List<EquipInfo> equipItems, CostumeSetItem costumeSetItem, bool bWin)
	{
		if (myAvatarCam != null)
		{
			myAvatarCam.playerClass = classType;
			myAvatarCam.ChangeAvatar();
			
			myAvatar = myAvatarCam.avatar;
		}
		
		SetAvatar(myAvatar, equipItems, costumeSetItem);
		
		if (myAvatar != null && bWin == false)
			myAvatar.SetLoseAnim();
	}
	
	public void SetEnemyAvatarInfo(GameDef.ePlayerClass classType, List<EquipInfo> equipItems, CostumeSetItem costumeSetItem, bool bWin)
	{
		if (enemyAvatarCam != null)
		{
			enemyAvatarCam.playerClass = classType;
			enemyAvatarCam.ChangeAvatar();
			
			enemyAvatar = enemyAvatarCam.avatar;
		}
		
		SetAvatar(enemyAvatar, equipItems, costumeSetItem);
		
		if (enemyAvatar != null && bWin == false)
			enemyAvatar.SetLoseAnim();
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
	
	public void OnGoTown(GameObject obj)
	{
		if (isRankUp == true)
		{
			CreateLevelUpWindow();
		}
		else
		{
			OnLevelUpFinished();
		}
	}
	
	public string levelUpWindowPrefabPath = "";
	public int warriorCharNameStringID = -1;
	public int assassinCharNameStringID = -1;
	public int wizardCharNameStringID = -1;
	
	public void CreateLevelUpWindow()
	{
		RankUpWindow levelUpWindow = ResourceManager.CreatePrefab<RankUpWindow>(levelUpWindowPrefabPath, popupNode, Vector3.zero);
		
		CharInfoData charData = Game.Instance.charInfoData;
		CharPrivateData privateData = null;
		int charIndex = -1;
		if (Game.Instance.connector != null)
			charIndex = Game.Instance.connector.charIndex;
		
		if (charData != null)
			privateData = charData.GetPrivateData(charIndex);
		
		string charNameStr = "";
		string userIDStr = "";
		List<EquipInfo> equipItems = null;
		CostumeSetItem costumeSetItem = null;
		
		if (privateData != null)
		{
			equipItems = privateData.equipData;
			costumeSetItem = privateData.costumeSetItem;
			
			int charNameStringID = -1;
			switch(privateData.baseInfo.CharacterIndex)
			{
			case 0:
				charNameStringID = warriorCharNameStringID;
				break;
			case 1:
				charNameStringID = assassinCharNameStringID;
				break;
			case 2:
				charNameStringID = wizardCharNameStringID;
				break;
			}
			
			TableManager tableManager = TableManager.Instance;
			StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
			
			if (stringTable != null)
				charNameStr = stringTable.GetData(charNameStringID);
			
			userIDStr = privateData.NickName;
		}
		
		int curLevel = 0;
		PlayerController player = Game.Instance.player;
		if (player != null && player.lifeManager != null)
			curLevel = player.lifeManager.charLevel;
		
		if (levelUpWindow != null)
		{
			levelUpWindow.parentObj = this.gameObject;
			levelUpWindow.SetCharInfo(curLevel, charNameStr, userIDStr);
			levelUpWindow.SetNextRank(this.nextRankType);
			
			levelUpWindow.SetAvatar((GameDef.ePlayerClass)charIndex, equipItems, costumeSetItem);
		}
	}
	
	public void OnLevelUpFinished()
	{
		CreateLoadingPanel(stageName);
		MonsterGenerator.isSurrendMode = false;
		
		DestroyObject(this.gameObject, 0.0f);
	}
	
	public string loadingPanelPrefabPath = "";
	public LoadingPanel loadingPanel = null;
	public void CreateLoadingPanel(string stageName)
	{
		TownUI.firstWindowType = TownUI.eTOWN_UI_TYPE.ARENA;
		
		if (loadingPanel == null)
		{
			Transform uiRoot = this.transform.parent;
			loadingPanel = ResourceManager.CreatePrefab<LoadingPanel>(loadingPanelPrefabPath, uiRoot, Vector3.zero);
		}
		else
		{
			loadingPanel.gameObject.SetActive(true);
			//reinforceWindow.InitMap();
		}
		
		if (loadingPanel != null)
			loadingPanel.LoadScene(stageName, null);
	}
	
	protected void SetFX(GameObject fxObject, bool bPlay)
	{
		if (fxObject == null)
			return;
		
		if (bPlay == true)
			fxObject.SetActive(bPlay);
		
		Transform[] childs = fxObject.GetComponentsInChildren<Transform>();
		if (childs == null || childs.Length == 0)
		{
			Animation rootAnim = fxObject.GetComponent<Animation>();
			if (rootAnim != null)
			{
				if (bPlay == true)
				{
					rootAnim.Play();
					rootAnim.Sample();
				}
				else
				{
					rootAnim.Stop();
				}
				
				foreach(AnimationState state in rootAnim)
					rootAnim[state.name].speed = 1.0f;
			}
			
			ParticleSystem particleSystem = fxObject.GetComponent<ParticleSystem>();
			if (particleSystem != null)
			{
				if (bPlay == true)
					particleSystem.Play();
				else
				{
					particleSystem.Stop();
					particleSystem.Clear();
					if (particleSystem.particleEmitter != null)
						particleSystem.particleEmitter.ClearParticles();
				}
				
				particleSystem.playbackSpeed = 1.0f;
			}
			else
			{
				if (fxObject.particleEmitter != null)
					fxObject.particleEmitter.emit = bPlay;
			}
		}
		else
		{
			for (int childIndex = 0; childIndex < childs.Length; ++childIndex)
			{
				GameObject child = childs[childIndex].gameObject;
				
				if (child != null)
				{
					Animation childAnim = child.GetComponent<Animation>();
					if (childAnim != null)
					{
						//Debug.Log("Animation Name : " + childAnim.name + (bPlay == true ? " Play" : " Stop"));
						
						if (childAnim.renderer != null)
							childAnim.renderer.enabled = bPlay;
						
						if (bPlay == true)
						{
							childAnim.Play();
							childAnim.Sample();
						}
						else
						{
							childAnim.Stop();
						}
						
						foreach(AnimationState state in childAnim)
							childAnim[state.name].speed = 1.0f;
					}
					
					ParticleSystem particleSystem = child.GetComponent<ParticleSystem>();
					if (particleSystem != null)
					{
						if (bPlay == true)
							particleSystem.Play();
						else
						{
							particleSystem.Stop();
							particleSystem.Clear();
							if (particleSystem.particleEmitter != null)
								particleSystem.particleEmitter.ClearParticles();
						}
						
						particleSystem.playbackSpeed = 1.0f;
					}
					else
					{
						if (child.particleEmitter != null)
							child.particleEmitter.emit = bPlay;
					}
				}
			}
		}
		
		if (bPlay == false)
			fxObject.SetActive(false);
	}
	
	public void OnTargetDetailView()
	{
		long targetUserIndexID = Game.Instance.arenaTargetUserIndexID;
		CharPrivateData enemyPrivateData = Game.Instance.arenaTargetInfo;
		
		int targetCharIndex = 0;
		string platform = Game.Instance.Connector.Platform;
		if (enemyPrivateData != null)
		{
			targetCharIndex = enemyPrivateData.baseInfo.CharacterIndex;
			platform = enemyPrivateData.platform;
		}
		
		IPacketSender sender = Game.Instance.packetSender;
		if (sender != null && targetUserIndexID != -1 && targetCharIndex != -1)
		{
			if (TownUI.detailRequestCount > 0)
				return;
			
			TownUI.detailRequestCount++;
			TownUI.detailWindowRoot = this.popupNode;
		
			sender.SendRequestTargetEquipItem(targetUserIndexID, targetCharIndex, platform);
		}
	}
	
	
	protected int curRankType = -1;
	protected int nextRankType = -1;
	protected bool isRankUp = false;
	public void SetRankUp(int curRankType, int nextRankType)
	{
		this.curRankType = curRankType;
		this.nextRankType = nextRankType;
		
		isRankUp = false;
		if (nextRankType < curRankType)
			isRankUp = true;
	}
}
