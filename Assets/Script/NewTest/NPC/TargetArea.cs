using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetArea : MonoBehaviour {
	private List<LifeManager> targetActorList = new List<LifeManager>();
	public int checkCount = 2;
	
	public LayerMask availableLayers = 0;
	
	public List<EventTriggerInfo> eventTriggerInfos = new List<EventTriggerInfo>();
	
	private bool isCalledEvent = false;
	public void DoCallEvent()
	{
		foreach(EventTriggerInfo info in eventTriggerInfos)
		{
			if (info != null && info.EventListener != null && info.CallFuncName.Length > 0)
			{
				info.EventListener.SendMessage(info.CallFuncName, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
	
	public void OnTriggerEnter(Collider other)
	{
		int layerValue = 1 << other.gameObject.layer;
		if ((availableLayers & layerValue) == 0)
			return;
		
		LifeManager lifeManager = other.gameObject.GetComponent<LifeManager>();
		if (lifeManager == null)
			return;
		
		float hpRate = lifeManager.GetHPRate();
		if (hpRate <= 0.0f)
			return;
		
		if (targetActorList.Contains(lifeManager) == true)
			return;
		
		targetActorList.Add(lifeManager);
		
		if (targetActorList.Count >= checkCount)
		{
			if (isCalledEvent == false)
			{
				DoCallEvent();
				isCalledEvent = true;
			}
		}
	}
	
	public void OnTriggerExit(Collider other)
	{
		int layerValue = 1 << other.gameObject.layer;
		if ((availableLayers & layerValue) == 0)
			return;
		
		LifeManager lifeManager = other.gameObject.GetComponent<LifeManager>();
		if (lifeManager == null)
			return;
		
		float hpRate = lifeManager.GetHPRate();
		if (hpRate <= 0.0f)
			return;
		
		targetActorList.Remove(lifeManager);
	}
	
	
	public BoxCollider areaCollider = null;
	EscortNPC targetNPC = null;
	void Start()
	{
		areaCollider = (BoxCollider)this.collider;
		
		targetNPC = GameObject.FindObjectOfType(typeof(EscortNPC)) as EscortNPC;
	}
	
	public void OnActivate()
	{
		if (areaCollider != null)
			areaCollider.enabled = true;
	}
	
	public void OnDeactivate()
	{
		if (areaCollider != null)
			areaCollider.enabled = false;
	}
	
	public void Update()
	{
		if (areaCollider == null || areaCollider.enabled == false)
			return;
		
		if (targetNPC != null)
		{
			BoxCollider targetNPCCollider = (BoxCollider)targetNPC.collider;
			
			Bounds targetBounds = targetNPCCollider.bounds;
			Bounds thisBounds = areaCollider.bounds;
			
			if (thisBounds.Intersects(targetBounds) == true)
			{
				if (targetActorList.Contains(targetNPC.lifeManager) == false)
					targetActorList.Add(targetNPC.lifeManager);
			}
		}
	}
}
