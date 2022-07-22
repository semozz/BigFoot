using UnityEngine;
using System.Collections;

public class EventAreaTrigger : EventTriggerInterface {
	// Use this for initialization
	void Start () {
	
	}
	
	public override void OnCallEventBoxEnter(EventBoxParam param)
	{
		BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
		if (boxCollider != null && boxCollider.isTrigger == false)
			return;
		
		//base.OnEventBoxEnter (param);
		EventTrigger eventTrigger = gameObject.GetComponent<EventTrigger>();
		if (eventTrigger != null)
		{
			gameObject.SendMessage("OnTriggerEvent", SendMessageOptions.DontRequireReceiver);
		}
		
		gameObject.SetActive(false);
	}
	
	public void DeActivate()
	{
		BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
		if (boxCollider != null)
			boxCollider.isTrigger = false;
	}
	
	public void Activate()
	{
		BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
		if (boxCollider != null)
			boxCollider.isTrigger = true;
	}
}
