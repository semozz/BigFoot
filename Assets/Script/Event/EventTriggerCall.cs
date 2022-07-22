using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class EventTriggerInfo
{
	public GameObject EventListener;
	public string CallFuncName = "OnCallByEventTrigger";
}

[System.Serializable]
public class EventTriggerCall : MonoBehaviour {
	public List<EventTriggerInfo> eventTriggerInfos = new List<EventTriggerInfo>();
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void OnTriggerEvent()
	{
		if (eventTriggerInfos == null)
			return;
		
		for (int index = 0; index < eventTriggerInfos.Count; ++index)
		{
			EventTriggerInfo info = eventTriggerInfos[index];
			
			if (info != null && info.EventListener != null && info.CallFuncName.Length > 0)
			{
				//Debug.Log("Function Call : " + info.CallFuncName);
				
				info.EventListener.SendMessage(info.CallFuncName, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
