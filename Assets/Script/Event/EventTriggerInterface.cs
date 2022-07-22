using UnityEngine;
using System.Collections;

public class EventTriggerInterface : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	public virtual void Update () {
	
	}
	
	public void OnEventBoxEnter(EventBoxParam param)
	{
		OnCallEventBoxEnter(param);
	}
	
	public void OnEventBoxExit(EventBoxParam param)
	{
		OnCallEventBoxExit(param);
	}
	
	public void OnTriggerEvent()
	{
		OnCallTriggerEvent();
	}
		
	public virtual void OnCallEventBoxEnter(EventBoxParam param)
	{
		
	}
	
	public virtual void OnCallEventBoxExit(EventBoxParam param)
	{
		
	}
	
	public virtual void OnCallTriggerEvent()
	{
		
	}
}
