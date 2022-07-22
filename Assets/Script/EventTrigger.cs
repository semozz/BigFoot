using UnityEngine;
using System.Collections;

public class EventTrigger : MonoBehaviour {
	public GameObject ownerObject = null;
	
	void OnTriggerEnter(Collider other)
	{
		if (ownerObject != null)
			ownerObject.SendMessage("OnTriggerEnter", other, SendMessageOptions.DontRequireReceiver);
	}
	
	void OnTriggerExit(Collider other)
	{
		if (ownerObject != null)
			ownerObject.SendMessage("OnTriggerExit", other, SendMessageOptions.DontRequireReceiver);
	}
}
