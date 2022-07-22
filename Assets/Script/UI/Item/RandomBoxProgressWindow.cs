using UnityEngine;
using System.Collections;

public class RandomBoxProgressWindow : PopupBaseWindow {
	public RandomBoxEventWindow parentWindow = null;
	
	public GameObject fxNode = null;
	
	public ItemSlot itemSlot = null;
	public Transform itemSlotNode = null;
	public string itemSlotPrefab = "UI/Item/ItemSlot";
	
	public UISprite otherSprite = null;
	public UILabel resultNameLabel = null;
	
	public string goldSpriteName = "";
	public string meatSpriteName = "";
	public string buffPackageSpriteName = "";
	public string couponSpriteName = "";
	
	public int goldStringID = 17;
	public int meatStringID = 206;
	public int buffPackageStringID = 266;
	public int couponStringID = 218;
	
	public void Start()
	{
		if (this.audio != null)
			this.audio.mute = !GameOption.effectToggle;
	}
	
	public float delayTime = 2.0f;
	public void SetRandomBox(PacketRandombox packet)
	{
		this.resultPacket = packet;
		
		Invoke("UpdateResultInfo", delayTime);
	}
	
	private PacketRandombox resultPacket = null;
	public void UpdateResultInfo()
	{
		string resultItemName = "";
		if (resultPacket != null)
		{
			if (resultPacket.itemID != 0)
			{
				if (otherSprite != null)
					otherSprite.gameObject.SetActive(false);
				
				Item resultItem = Item.CreateItem(resultPacket.itemID, "", resultPacket.itemGrade, 0, 1, resultPacket.itemRate, 0);
				
				if (resultItem != null && resultItem.itemInfo != null)
					resultItemName = resultItem.itemInfo.itemName;
				
				itemSlot = ResourceManager.CreatePrefab<ItemSlot>(itemSlotPrefab, itemSlotNode);
				if (itemSlot != null)	
				{
					itemSlot.slotType = GameDef.eSlotType.Common;
					itemSlot.SetItem(resultItem);
				}
			}
			else
			{
				TableManager tableManager = TableManager.Instance;
				StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
				string otherSpriteName = "";
				int nameStringID = -1;
				if (resultPacket.gold != 0)
				{
					otherSpriteName = goldSpriteName;
					nameStringID = goldStringID;
				}
				else if (resultPacket.meat != 0)
				{
					otherSpriteName = meatSpriteName;
					nameStringID = meatStringID;
				}
				else if (resultPacket.buffPackDay != 0)
				{
					otherSpriteName = buffPackageSpriteName;
					nameStringID = buffPackageStringID;
				}
				else if (resultPacket.coupon != 0)
				{
					otherSpriteName = couponSpriteName;
					nameStringID = couponStringID;
				}
				
				if (stringTable != null && nameStringID != -1)
					resultItemName = stringTable.GetData(nameStringID);
				
				if (string.IsNullOrEmpty(otherSpriteName) == false)
				{
					if (otherSprite != null)
					{
						otherSprite.gameObject.SetActive(true);
						otherSprite.spriteName = otherSpriteName;
					}
					
				}
				else
				{
					if (otherSprite != null)
						otherSprite.gameObject.SetActive(false);
				}
			}
		}
		
		if (resultNameLabel != null)
			resultNameLabel.text = resultItemName;
	}
	
	public void OnClose()
	{
		if (parentWindow != null)
			parentWindow.CloseRandomResult();
	}
	
}
