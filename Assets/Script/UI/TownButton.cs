using UnityEngine;
using System.Collections;

public class TownButton : MonoBehaviour {

	public string badgeNotifyPrefab = "UI/New_Town_Default";
	private GameObject badgeObj = null;
	
	public Transform badgeNode = null;
	
	public int notifyTab = -1;
	
	public void SetBadgeNotify(int tabIndex)
	{
		if (badgeObj == null)   
			badgeObj = ResourceManager.CreatePrefab(badgeNotifyPrefab, badgeNode);
		
		notifyTab = tabIndex;
	}
	
	public void ClearBadgeNotify()
	{
		if (badgeObj != null)
		{
			DestroyObject(badgeObj, 0.2f);
			badgeObj = null;
		}
	}
	
	
	public string eventBadgePrefab = "";
	public Transform eventNode = null;
	private GameObject eventBadge = null;
	public void SetEventBadge(Game.EventInfo eventInfo)
	{
		if (string.IsNullOrEmpty(eventBadgePrefab) == true || eventNode == null)
			return;
		
		if (eventBadge == null)
			eventBadge = ResourceManager.CreatePrefab(eventBadgePrefab, eventNode);
		
		if (eventBadge != null)
		{
			EventTimeInfoPanel eventTimePanel = eventBadge.GetComponent<EventTimeInfoPanel>();
			if (eventTimePanel != null)
				eventTimePanel.SetEventInfo(eventInfo);
		}
	}
	
	public void ClearEventBadge()
	{
		if (eventBadge != null)
		{
			DestroyObject(eventBadge, 0.2f);
			eventBadge = null;
		}
	}
}
