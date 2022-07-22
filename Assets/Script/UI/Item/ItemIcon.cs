using UnityEngine;
using System.Collections;

public class ItemIcon : MonoBehaviour {
	public ItemFrame frame = null;
	public UISprite icon = null;
	
	public Item item = null;
	
	public string defaultIconName = "item_nothing";
	public GameDef.eSlotType slotType = GameDef.eSlotType.Common;
	
	public void SetSlotType(GameDef.eSlotType type)
	{
		slotType = type;
	}
	
	public void SetItem(Item _item)
	{
		item = _item;
		
		UpdateUI();
	}
	
	public void UpdateUI()
	{
		UpdateFrame();
		UpdateIcon();
	}
	
	public void UpdateFrame()
	{
		if (frame != null)
			frame.InitFrame(item);
	}
	
	public void UpdateIcon()
	{
		string iconName = defaultIconName;
		switch(slotType)
		{
		case GameDef.eSlotType.Head:
			iconName = "base_head";
			break;
		case GameDef.eSlotType.Hand:
			iconName = "base_hand";
			break;
		case GameDef.eSlotType.Pants:
			iconName = "base_pants";
			break;
		case GameDef.eSlotType.Boots:
			iconName = "base_boots";
			break;
		case GameDef.eSlotType.Armor:
			iconName = "base_armor";
			break;
		case GameDef.eSlotType.Weapon:
			iconName = "base_weapon";
			break;
		case GameDef.eSlotType.Accessories:
			iconName = "base_accessory";
			break;
		case GameDef.eSlotType.Ring:
			iconName = "base_ring";
			break;
		case GameDef.eSlotType.Potion_1:
			iconName = "base_potion_1";
			break;
		case GameDef.eSlotType.Potion_2:
			iconName = "base_potion_2";
			break;
		case GameDef.eSlotType.Costume_Back:
			iconName = "base_costume_back";
			break;
		case GameDef.eSlotType.Costume_Body:
			iconName = "base_costume_body";
			break;
		case GameDef.eSlotType.Costume_Head:
			iconName = "base_costume_head";
			break;
		default:
			iconName = "base_common";
			break;
		}
		
		if (item != null && item.itemInfo != null)
			iconName = item.itemInfo.itemIcon;
		
		if (icon != null)
			icon.spriteName = iconName;
	}
}
