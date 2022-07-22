using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossRaidListWindow : MonoBehaviour {
	public GameObject parentWindow = null;
	
	public string infoPanelPrefabPath = "";
	public UIGrid grid = null;
	public UIDraggablePanel dragablePanel = null;
	public UIPanel scrollPanel = null;
	
	private List<BossRaidInfoPanel> bossRaidInfoPanelList = new List<BossRaidInfoPanel>();
	public void SetInfos(List<BossRaidInfo> bossRaidInfos)
	{
		if (bossRaidInfos == null)
			return;
		
		Vector3 vPos = Vector3.zero;
		int nCount = bossRaidInfos != null ? bossRaidInfos.Count : 0;
		for (int index = 0; index < nCount; ++index)
		{
			BossRaidInfo info = bossRaidInfos[index];
			
			BossRaidInfoPanel infoPanel = ResourceManager.CreatePrefab<BossRaidInfoPanel>(infoPanelPrefabPath, grid.transform, vPos);
			if (infoPanel != null)
			{
				infoPanel.parentWindow = this;
				
				infoPanel.SetBossInfo(info);
				
				SetButtonMessage(infoPanel.normalInfo.buttonMessage, this.parentWindow, "OnRequestBossRaidStart");
				
				bossRaidInfoPanelList.Add(infoPanel);
				
				//if (this.grid.sorted == true)
				//	SetSortName(infoPanel);
				
				//vPos.y += grid.cellHeight;
			}
		}
		
		this.Invoke("RefreshPanels", 0.1f);
	}
	
	public void SetButtonMessage(UIButtonMessage buttonMsg, GameObject target, string funcName)
	{
		if (buttonMsg != null)
		{
			buttonMsg.target = target;
			buttonMsg.functionName = funcName;//"OnTargetDetailWindow";
		}
	}
	
	public void RefreshPanels()
	{
		if (grid != null)
			grid.Reposition();
		
		if (dragablePanel != null)
			dragablePanel.ResetPosition();
		
		if (scrollPanel != null)
			scrollPanel.Refresh();
	}
	
	public void InitList()
	{
		foreach(BossRaidInfoPanel temp in bossRaidInfoPanelList)
		{
			DestroyObject(temp.gameObject, 0.0f);
		}
		bossRaidInfoPanelList.Clear();
		
		//isRefreshCalled = false;
	}
	
	public void SetSortName(BossRaidInfoPanel infoPanel)
	{
		if (infoPanel == null || infoPanel.bossRaidInfo == null)
			return;
		
		string objName = string.Format("{0:D8}", infoPanel.bossRaidInfo.bossID);
		infoPanel.name = objName;
	}
	
	public BossRaidInfo GetBossRaidInfo(int index)
	{
		BossRaidInfo bossRaidInfo = null;
		foreach(BossRaidInfoPanel temp in bossRaidInfoPanelList)
		{
			if (temp.bossRaidInfo.index == index)
			{
				bossRaidInfo = temp.bossRaidInfo;
				break;
			}
		}
		
		return bossRaidInfo;
	}
}
