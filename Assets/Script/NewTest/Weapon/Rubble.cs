using UnityEngine;
using System.Collections;

public class Rubble : BaseWeapon {
	public Collider damageCollider = null;
	public float activateCoolTime = 0.1f;
	public float deactivateCoolTime = 0.1f;
	
	private float delayTime = 0.0f;
	
	public enum eRubbleMode
	{
		None = -1,
		Deactivate,
		Activate,
		Destroy
	}
	private eRubbleMode currentMode = eRubbleMode.None;
	
	void Awake()
	{
		SetMode(eRubbleMode.Deactivate);
	}
	
	protected void SetMode(eRubbleMode mode)
	{
		if (currentMode == mode)
			return;
		
		switch(mode)
		{
		case eRubbleMode.Deactivate:
			delayTime = deactivateCoolTime;
			SetupCollider(damageCollider, false);
			
			hitObjects.Clear();
			break;
		case eRubbleMode.Activate:
			delayTime = activateCoolTime;
			SetupCollider(damageCollider, true);
			break;
		case eRubbleMode.Destroy:
			DestroyObject(gameObject, 0.0f);
			break;
		}
		
		currentMode = mode;
	}
	
	// Update is called once per frame
	void Update () {
		delayTime -= Time.deltaTime;
		if (delayTime <= 0.0f)
		{
			eRubbleMode nextMode = currentMode;
			
			switch(currentMode)
			{
			case eRubbleMode.Deactivate:
				nextMode = Rubble.eRubbleMode.Activate;
				break;
			case eRubbleMode.Activate:
				nextMode = Rubble.eRubbleMode.Destroy;
				break;
			}
			
			SetMode(nextMode);
		}
	}
	
	protected void SetupCollider (Collider c, bool bActive)
    {
        if (c == null) return;

        c.isTrigger = bActive;
        if (c.GetType() == typeof(MeshCollider))
            ((MeshCollider)c).convex = bActive;
        c.gameObject.SetActive(bActive);
    }
	
	public LayerMask attackableLayers = 0;
	public void OnTriggerEnter(Collider other)
	{
		int layerValue = 1 << other.gameObject.layer;
		if ((attackableLayers & layerValue) == 0)
			return;
		
		LifeManager actor = other.gameObject.GetComponent<LifeManager>();
		if (actor != null)
		{
			Debug.Log("Rubble Damage......" + Time.time);
			
			actor.OnDamage(this.attackInfo, this.transform, false);
		}
	}
}
