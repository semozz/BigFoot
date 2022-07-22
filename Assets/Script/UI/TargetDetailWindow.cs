using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetDetailWindow : PopupBaseWindow {
	public UILabel titleLabel = null;
	
	public AvatarCam avatarCam = null;
	public AvatarController avatar = null;
	
	public ItemSlotPanel equipItemPage = null;
	
	public CharacterInfos charInfos = null;
	public ArenaInfos arenaInfos = null;
	public DefenceInfos defenceInfos = null;
	public ItemInfoPage itemInfoPage = null;
	
	public List<CharInfoButton> charButtons = new List<CharInfoButton>();

    public string platform = "kakao";
	public long targetUserID = -1;

    public void InitWindow(int initIndex, string accountName, TargetInfoAll[] infos, long myUserID, string targetUserPlatform, long targetUserID, int isFriend)
	{
        this.platform = targetUserPlatform;
		this.targetUserID = targetUserID;
		
		titleLabel.text = accountName;
		
		int nCount = charButtons.Count;
		int nInfoCount = infos != null ? infos.Length : 0;
		
		bool bSelected = false;
		CharInfoButton selectedButton = null;
		for (int index = 0; index < nCount; ++index)
		{
			CharInfoButton button = charButtons[index];
			
			TargetInfoAll info = null;
			if (index >= 0 && index < nInfoCount)
				info = infos[index];
			
			bSelected = index == initIndex;
			if (button != null)
			{
				button.SetPlayerInfo(index, info);
				button.IsSelected = bSelected;
				
				if (bSelected == true)
					selectedButton = button;
			}
		}
		
		SetInfo(selectedButton);
		
		
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		if (stringTable != null)
		{
			if (inviteButtonLabel != null && inviteButtonStringID != -1)
				inviteButtonLabel.text = stringTable.GetData(inviteButtonStringID);
			if (rejectButtonLabel != null && rejectButtonStringID != -1)
				rejectButtonLabel.text = stringTable.GetData(rejectButtonStringID);
		}
		
		UpdateFriendFuncButton(myUserID, targetUserID, isFriend);
	}
	
	public GameObject inviteButtonObj = null;
	public GameObject rejectButtonObj = null;
	public UILabel inviteButtonLabel = null;
	public int inviteButtonStringID = -1;
	public UILabel rejectButtonLabel = null;
	public int rejectButtonStringID = -1;
	public void UpdateFriendFuncButton(long myUserID, long targetUserID, int isFriend)
	{
		if (myUserID == targetUserID)
		{
			SetActiveObject(inviteButtonObj, false);
			SetActiveObject(rejectButtonObj, false);
		}
		else
		{
			SetActiveObject(inviteButtonObj, isFriend == 0);
			SetActiveObject(rejectButtonObj, isFriend == 1);
		}
	}
	
	public void SetActiveObject(GameObject obj, bool bActive)
	{
		if (obj != null)
			obj.SetActive(bActive);
	}
	
	public void SetAvatar(GameDef.ePlayerClass classType, List<EquipInfo> equipItems, CostumeSetItem costumeSetItem)
	{
		if (avatarCam != null)
		{
			avatarCam.playerClass = classType;
			avatarCam.ChangeAvatar();
			
			avatar = avatarCam.avatar;
		}
		
		SetEquipItems(avatar, equipItems, costumeSetItem);
	}
	
	public void SetEquipItems(AvatarController avatar, List<EquipInfo> equipItems, CostumeSetItem costumeSetItem)
	{
		ResetItems(equipItemPage);
		
		if (equipItems == null)
			return;
		
		int weaponID = -1;
		int costumeBackID = -1;
		int costumeHeadID = -1;
		int costumeBodyID = -1;
		
		int index = 0;
		foreach(EquipInfo info in equipItems)
		{
			if (equipItemPage != null)
				equipItemPage.SetItem(info.slotIndex, info.item);
			
			if (info.item == null || info.item.itemInfo == null)
				continue;
			
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
		
		if (equipItemPage != null && equipItemPage.costumeSetPanel != null)
		{
			equipItemPage.costumeSetPanel.SetCostumeItem(costumeSetItem);
			
			bool hasCostumeSet = costumeSetItem != null;
			
			equipItemPage.costumeSetPanel.gameObject.SetActive(hasCostumeSet);
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
	
	public void ResetItems(ItemSlotPanel itemSlotPanel)
	{
		int slotCount = 0;
		if (itemSlotPanel != null)
			slotCount = itemSlotPanel.itemSlots.Count;
		
		for (int index = 0; index < slotCount; ++index)
		{
			itemSlotPanel.SetItem(index, null);
		}
	}
	
	public override void OnBack()
	{
		DestroyObject(this.gameObject, 0.1f);
	}
	
	public void OnChangeChar(GameObject obj)
	{
		CharInfoButton pushButton = obj.GetComponent<CharInfoButton>();
		if (pushButton != null && pushButton.IsSelected == false)
		{
			int selectedIndex = pushButton.index;
			bool bSelected = false;
			foreach(CharInfoButton button in this.charButtons)
			{
				bSelected = button.index == selectedIndex;
				if (button != null)
				{
					button.IsSelected = bSelected;
				}
			}
			
			SetInfo(pushButton);
		}
	}
	
	public void SetInfo(CharInfoButton infoButton)
	{
		if (infoButton != null && infoButton.targetPrivateData != null)
		{
			PlayerController player = infoButton.player;
			CharPrivateData privateData = infoButton.targetPrivateData;
			GameDef.ePlayerClass classType = (GameDef.ePlayerClass)privateData.baseInfo.CharacterIndex;
			SetAvatar(classType, privateData.equipData, privateData.costumeSetItem);
			
			if (charInfos != null)
				charInfos.SetCharInfo(player);
			if (arenaInfos != null)
				arenaInfos.SetInfo(privateData.arenaInfo);
			if (defenceInfos != null)
				defenceInfos.SetInfo(privateData.waveInfo);
			
			if (itemInfoPage != null)
				itemInfoPage.player = player;
			
			OnSelectItem(null);
			OnCostumeSetItem(null);
		}
	}
	
	public void OnSelectItem(GameObject button)
	{
		ItemSlot itemSlot = null;
		GameObject parent = null;
		
		if (button != null)
			parent = button.transform.parent.gameObject;
		
		if (parent != null)
			itemSlot = parent.GetComponent<ItemSlot>();
		
		Item item = null;
		int slotIndex = -1;
		GameDef.eItemSlotWindow slotWindow = GameDef.eItemSlotWindow.Inventory;
		
		if (itemSlot != null)
		{
			if (itemSlot.itemIcon != null)
				item = itemSlot.itemIcon.item;
			
			slotIndex = itemSlot.slotIndex;
			slotWindow = itemSlot.slotWindowType;
		}
		
		OnSelectItem(item, slotIndex, slotWindow);
	}
	
	public void OnCostumeSetItem(GameObject obj)
	{
		CostumeSetItemPanel costumeSetItemPanel = null;
		
		int slotIndex = -1;
		GameDef.eItemSlotWindow slotWindow = GameDef.eItemSlotWindow.Inventory;
		
		if (obj != null)
			costumeSetItemPanel =  obj.GetComponent<CostumeSetItemPanel>();
		
		if (costumeSetItemPanel != null)
		{
			CostumeSetItem setItem = costumeSetItemPanel.setItem;
			slotIndex = costumeSetItemPanel.slotIndex;
			slotWindow = costumeSetItemPanel.slotWindowType;
			
			OnSelectItem(setItem, slotIndex, slotWindow);
		}
	}
	
	public virtual void OnSelectItem(Item item, int slotIndex, GameDef.eItemSlotWindow slotWindow)
	{
		if (equipItemPage != null)
		{
			equipItemPage.InitSelectedSlot();
			equipItemPage.SetSelectedSlot(slotIndex, true);
		}
				
		if (itemInfoPage != null)
		{
			itemInfoPage.SetCostumeSetItem(null);
			itemInfoPage.SetItem(item);
		}
	}
	
	public virtual void OnSelectItem(CostumeSetItem item, int slotIndex, GameDef.eItemSlotWindow slotWindow)
	{
		if (equipItemPage != null)
		{
			equipItemPage.InitSelectedSlot();
			equipItemPage.SetSelectedSlot(slotIndex, true);
		}
				
		if (itemInfoPage != null)
		{
			itemInfoPage.SetItem(null);
			itemInfoPage.SetCostumeSetItem(item);
		}
	}
	
	void OnDestroy()
	{
		TownUI.detailRequestCount--;
	}
	
	public void OnInviteFriend(GameObject obj)
	{
		IPacketSender sender = Game.Instance.packetSender;
		if (sender != null)
		{
			sender.SendRequestInviteFriend(this.targetUserID);
		}
		
		FriendWindow friendWindow = GameUI.Instance.friendWindow;
		if (friendWindow != null)
		{
			friendWindow.RemoveInviteList(this.targetUserID);
		}
		
		if (obj != null)
			obj.SetActive(false);
	}
	
	public void OnRejectFriend(GameObject obj)
	{
		IPacketSender sender = Game.Instance.packetSender;
		if (sender != null)
		{
			sender.SendRequestDeleteFriend(this.targetUserID, this.platform);
		}
		
		FriendWindow friendWindow = GameUI.Instance.friendWindow;
		if (friendWindow != null)
		{
			friendWindow.RemoveFriendList(this.targetUserID);
		}
		
		if (obj != null)
			obj.SetActive(false);
	}
}
