using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemFrame : MonoBehaviour {
	public UISprite frame = null;
	public UISprite innerFrame = null;
	
	public UILabel reinforce = null;
	
	public string defaultGradeFrameName = "icon_frame_low";
	public List<string> gradeFrameNames = new List<string>();
	
	public string nullFrame = "icon_frame_nothing";
	public string normalFrame = "icon_frame_normal";
	public string costumeFrame = "icon_frame_costume";
	
	public List<string> innerFrameNames = new List<string>();
	
	public void InitFrame(Item item)
	{
		string frameName = nullFrame;
		if (item != null)
		{
			if (item.itemInfo != null)
			{
				switch(item.itemInfo.itemType)
				{
				case ItemInfo.eItemType.Material:
				case ItemInfo.eItemType.Material_Compose:
				case ItemInfo.eItemType.Potion_1:
				case ItemInfo.eItemType.Potion_2:
				case ItemInfo.eItemType.Common:
					frameName = normalFrame;
					break;
				case ItemInfo.eItemType.Costume_Body:
				case ItemInfo.eItemType.Costume_Back:
				case ItemInfo.eItemType.Costume_Head:
					frameName = costumeFrame;
					break;
				default:
					frameName = GetItemGradeFrameName(item.itemGrade);
					break;
				}
			}
			else
			{
				frameName = normalFrame;
			}
		}
		
		if (frame != null)
			frame.spriteName = frameName;
		
		if (reinforce != null)
		{
			int reinforceStep = item != null ? item.reinforceStep : 0;
			
			reinforce.text = string.Format("+{0}", reinforceStep);
			reinforce.gameObject.SetActive(reinforceStep > 0);
		}
		
		int itemRate = -1;
		if (item != null)
			itemRate = item.itemRateStep;
		
		SetInnerFrame(itemRate + 1);
	}
	
	public void SetInnerFrame(int itemRate)
	{
		if (innerFrame != null)
			innerFrame.spriteName = GetInnerFrameName(itemRate);
	}
	
	public string defaultInnerFrameName = "item_nothing";
	public string GetInnerFrameName(int itemRate)
	{
		int nCount = innerFrameNames.Count;
		string frameName = defaultInnerFrameName;
		if (itemRate >= 0 && itemRate < nCount)
			frameName = innerFrameNames[itemRate];
		
		return frameName;
	}
	
	public string GetItemGradeFrameName(int itemGrade)
	{
		string frameName = defaultGradeFrameName;
		int nCount = gradeFrameNames.Count;
		if (itemGrade >= 0 && itemGrade < nCount)
			frameName = gradeFrameNames[itemGrade];
		
		return frameName;
	}
}
