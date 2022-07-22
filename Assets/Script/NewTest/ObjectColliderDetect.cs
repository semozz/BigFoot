using UnityEngine;
using System.Collections;

public class ObjectColliderDetect : MonoBehaviour {
	public GameObject ownerObj = null;
	
	void OnTriggerEnter(Collider other)
	{
		//Debug.Log(other + ". OnTriggerEnter...");
		
		if (ownerObj != null)
			ownerObj.SendMessage("OnTriggerEnter", other, SendMessageOptions.DontRequireReceiver);
	}
	
	void OnTriggerExit(Collider other)
	{
		if (ownerObj != null)
			ownerObj.SendMessage("OnTriggerExit", other, SendMessageOptions.DontRequireReceiver);
	}
	
	void OnCollisionEnter(Collision collision)
	{
		//Debug.Log(collision + ". CollisionEnter...");
	}
}
