using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MasteryIcon : MonoBehaviour {
	public enum eArrowType{
		None,
		Arrow_Normal,
		Arrow_Long
	}
	public eArrowType arrowType = eArrowType.None;
	
	public UISprite disableSprite = null;
	public UIButton button = null;
	public UIButtonMessage buttonMessage = null;
	
	public UISprite icon = null;
	
	public UISprite activeFrame = null;
	public UISprite activeArrow = null;
	public UISprite activeLock = null;
	
	public UISprite passiveFrame = null;
	public UISprite passiveArrow = null;
	public UISprite passiveArrowLong = null;
	
	public UISprite selectedFrame = null;
	
	public UILabel masteryPointInfo = null;
	
	public GameObject activeFX = null;
	public GameObject addPointFX = null;
	public GameObject resetFX = null;
	
	public string defaultIconName = "base_common";
	
	public string defaultActiveFrameSpriteName = "frame_skill_active";
	public List<string> activeFrameSpriteName = new List<string>();
	public string GetActiveFrameSpriteName(GameDef.ePlayerClass playerClass)
	{
		int index = (int)playerClass;
		return GetActiveFrameSpriteName(index);
	}
	
	public string GetActiveFrameSpriteName(int index)
	{
		string spriteName = defaultActiveFrameSpriteName;
		
		int nCount = activeFrameSpriteName.Count;
		
		if (index >= 0 && index < nCount)
			spriteName = activeFrameSpriteName[index];
		
		return spriteName;
	}
	
	void Awake()
	{
		SetInfo(null);
		
		SetSelected(false);
	}
	
	public static eArrowType ToArrowType(string str)
	{
		eArrowType arrowType = eArrowType.None;
		if (str == "Arrow_Normal")
			arrowType = eArrowType.Arrow_Normal;
		else if (str == "Arrow_Long")
			arrowType = eArrowType.Arrow_Long;
		
		return arrowType;
	}
	
	public void SetSelected(bool bSelected)
	{
		if (selectedFrame != null)
			selectedFrame.gameObject.SetActive(bSelected);
	}
	
	public MasteryInfo_New masteryInfo = null;
	public void SetInfo(MasteryInfo_New info)
	{
		this.masteryInfo = info;
		
		UpdateUI();
	}
	
	public void SetActiveSprite(UISprite sprite, bool bActive)
	{
		if (sprite != null)
			sprite.gameObject.SetActive(bActive);
	}
	
	public void ShowFrame(MasteryInfo_New.eMasteryActiveType activeType)
	{
		
		if (activeType == MasteryInfo_New.eMasteryActiveType.Active)
		{
			int charIndex = -1;
			ClientConnector connector = Game.Instance.connector;
			if (connector != null)
				charIndex = connector.charIndex;
			
			string activeFrameName = GetActiveFrameSpriteName(charIndex);
			if (activeFrame != null)
				activeFrame.spriteName = activeFrameName;
			
			SetActiveSprite(activeFrame, true);
			SetActiveSprite(passiveFrame, false);
				
			ShowArrow(true, arrowType);
		}
		else
		{
			SetActiveSprite(activeFrame, false);
			SetActiveSprite(passiveFrame, true);
			
			ShowArrow(false, arrowType);
		}
	}
	
	public void ShowArrow(bool bActive, eArrowType type)
	{
		if (bActive == true)
		{
			switch(arrowType)
			{
			case eArrowType.Arrow_Normal:
				SetActiveSprite(activeArrow, true);
				SetActiveSprite(passiveArrow, false);
				SetActiveSprite(passiveArrowLong, false);
				break;
			case eArrowType.Arrow_Long:
				SetActiveSprite(activeArrow, false);
				SetActiveSprite(passiveArrow, false);
				SetActiveSprite(passiveArrowLong, true);
				break;
			default:
				SetActiveSprite(activeArrow, false);
				SetActiveSprite(passiveArrow, false);
				SetActiveSprite(passiveArrowLong, false);
				break;
			}
		}
		else
		{
			switch(arrowType)
			{
			case eArrowType.Arrow_Normal:
				SetActiveSprite(activeArrow, false);
				SetActiveSprite(passiveArrow, true);
				SetActiveSprite(passiveArrowLong, false);
				break;
			case eArrowType.Arrow_Long:
				SetActiveSprite(activeArrow, false);
				SetActiveSprite(passiveArrow, false);
				SetActiveSprite(passiveArrowLong, true);
				break;
			default:
				SetActiveSprite(activeArrow, false);
				SetActiveSprite(passiveArrow, false);
				SetActiveSprite(passiveArrowLong, false);
				break;
			}
		}
	}
	
	public void UpdateUI()
	{
		MasteryInfo_New.eMasteryActiveType activeType = MasteryInfo_New.eMasteryActiveType.Passive;
		string iconName = defaultIconName;
		string masteryPoint = "";
		
		bool isEnable = false;
		
		bool bActiveLock = false;
		
		if (this.masteryInfo != null)
		{
			isEnable = this.masteryInfo.CheckEnable();
			
			activeType = this.masteryInfo.activeType;
			iconName = this.masteryInfo.iconName;
			
			int curPoint = this.masteryInfo.Point;
			
			masteryPoint = string.Format("{0}/{1}", curPoint, this.masteryInfo.maxPoint);
			
			if (this.masteryInfo.activeType == MasteryInfo_New.eMasteryActiveType.Active)
			{
				MasteryInfo_New activeInfo = null;
				if (this.masteryInfo.manager != null)
					activeInfo = this.masteryInfo.manager.activeMastery;
				
				if (activeInfo != null && activeInfo.id != 0 && activeInfo.id != this.masteryInfo.id)
					bActiveLock = true;
			}
		}
		
		SetActiveSprite(activeLock, bActiveLock);
		
		ShowFrame(activeType);
		if (icon != null)
			icon.spriteName = iconName;
		
		if (masteryPointInfo != null)
			masteryPointInfo.text = masteryPoint;
		
		if (disableSprite != null)
			disableSprite.gameObject.SetActive(!isEnable);
	}
}
