using UnityEngine;
using System.Collections;

public class RestArea : MonoBehaviour {
	public LayerMask availableLayers = 0;
	
	
	public void OnTriggerEnter(Collider other)
	{
		int layerValue = 1 << other.gameObject.layer;
		if ((availableLayers & layerValue) == 0)
			return;
		
		
		LifeManager lifeManager = other.gameObject.GetComponent<LifeManager>();
		if (lifeManager != null)
		{
			if (lifeManager.myActorInfo.actorType != ActorInfo.ActorType.Escort)
				return;
			
			EscortNPC escortNPC = other.gameObject.GetComponent<EscortNPC>();
			if (escortNPC != null)
				escortNPC.DoRest();
		}
	}
}
