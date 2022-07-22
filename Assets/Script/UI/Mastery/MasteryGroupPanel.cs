using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MasteryGroupPanel : MonoBehaviour {
	public UISprite background = null;
	public UILabel groupNameLabel = null;
	public UILabel groupPointLabel = null;
	
	public List<Transform> slotPosInfos = new List<Transform>();
	public string masterySlotPrefabPath = "UI/MasteryWindow/MasterySlot";
	
	public Transform slotNode = null;
	public List<MasteryIcon> masteryIcons = new List<MasteryIcon>();
	public string defaultBGName = "base01";
	
	public MasteryWindow_New rootWindow = null;
	public MasteryManager_New manager = null;
	public MasteryGroupInfo groupInfo = null;
	public void InitWindow(MasteryGroupInfo uiInfo, MasteryManager_New manager, MasteryWindow_New rootWindow)
	{
		this.manager = manager;
		this.groupInfo = uiInfo;
		this.rootWindow = rootWindow;
		
		foreach(MasteryIcon icon in masteryIcons)
		{
			DestroyObject(icon.gameObject, 0.0f);
		}
		masteryIcons.Clear();
		
		string groupName = "";
		string groupBGName = defaultBGName;
		
		if (groupInfo != null)
		{
			groupName = groupInfo.groupName;
			groupBGName = groupInfo.groupBackgroundImg;
			
			int index = 0;
			int nCount = groupInfo.slotInfos.Count;
			MasterySlotInfo slotInfo = null;
			Vector3 vPos = Vector3.zero;
			
			MasteryIcon masteryIcon = null;
			MasteryInfo_New mastery = null;
			
			for(index = 0; index < nCount; ++index)
			{
				slotInfo = groupInfo.slotInfos[index];
				
				if (slotInfo.masteryID == 0)
					continue;
				
				if (manager != null)
					mastery = manager.GetMastery(slotInfo.masteryID);
				
				vPos = GetSlotPos(index);
				masteryIcon = ResourceManager.CreatePrefab<MasteryIcon>(masterySlotPrefabPath, slotNode, vPos);
				
				if (masteryIcon != null)
				{
					masteryIcon.arrowType = slotInfo.arrowType;
					
					masteryIcon.SetInfo(mastery);
					masteryIcons.Add(masteryIcon);
					
					if (masteryIcon.buttonMessage != null)
					{
						masteryIcon.buttonMessage.functionName = "OnSlotClick";
						masteryIcon.buttonMessage.target = this.gameObject;
					}
				}
			}
		}
		
		if (groupNameLabel != null)
			groupNameLabel.text = groupName;
		
		if (background != null)
			background.spriteName = groupBGName;
		
		UpdateGroupPointInfo();
	}
	
	public void UpdateGroupPointInfo()
	{
		string groupPointStr = "";
		int groupPoint = 0;
		if (manager != null && groupInfo != null)
			groupPoint = manager.GetGroupPoint(groupInfo.groupID);
		
		groupPointStr = string.Format("{0}", groupPoint);
		
		if (groupPointLabel != null)
			groupPointLabel.text = groupPointStr;
	}
	
	public Vector3 GetSlotPos(int index)
	{
		Vector3 vPos = Vector3.zero;
		
		int nCount = slotPosInfos.Count;
		if (index >= 0 && index < nCount)
			vPos = slotPosInfos[index].localPosition;
		
		return vPos;
	}
	
	public void ResetSelected()
	{
		foreach(MasteryIcon icon in masteryIcons)
		{
			icon.SetSelected(false);
		}
	}
	
	public void OnSlotClick(GameObject obj)
	{
		MasteryIcon icon = obj.GetComponent<MasteryIcon>();
		
		if (icon != null)
		{
			if (rootWindow != null)
			{
				if (rootWindow.selectedMasterySlot != icon)
				{
					rootWindow.ResetSelectedInfo();
					rootWindow.SetSelectedInfo(icon);
				}
			}
		}
	}
	
	public void UpdateUI()
	{
		foreach(MasteryIcon icon in masteryIcons)
		{
			icon.UpdateUI();
		}
		
		UpdateGroupPointInfo();
	}
}
