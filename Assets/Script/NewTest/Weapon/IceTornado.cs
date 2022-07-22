using UnityEngine;
using System.Collections;

public class IceTornado : BaseWeapon {
	public Collider damageCollider = null;
	
	public float lifeTime = 5.0f;
	public float leftLifeTime = 0.0f;
	
	public float activateCoolTime = 0.4f;
	public float deactivateCoolTime = 0.1f;
	
	private float delayTime = 0.0f;
	
	public enum eIceTornadoMode
	{
		None = -1,
		Deactivate,
		Activate,
		Destroy
	}
	private eIceTornadoMode currentMode = eIceTornadoMode.None;
	
	void Awake()
	{
		leftLifeTime = lifeTime;
		SetMode(eIceTornadoMode.Deactivate);
	}
	
	protected void SetMode(eIceTornadoMode mode)
	{
		if (currentMode == mode)
			return;
		
		switch(mode)
		{
		case eIceTornadoMode.Deactivate:
			delayTime = deactivateCoolTime;
			SetupCollider(damageCollider, false);
			
			hitObjects.Clear();
			break;
		case eIceTornadoMode.Activate:
			delayTime = activateCoolTime;
			SetupCollider(damageCollider, true);
			break;
		case eIceTornadoMode.Destroy:
			DestroyObject(gameObject, 0.0f);
			break;
		}
		
		currentMode = mode;
	}
	
	// Update is called once per frame
	void Update () {
		delayTime -= Time.deltaTime;
		leftLifeTime -= Time.deltaTime;
		
		if (leftLifeTime <= 0.0f)
		{
			SetMode(eIceTornadoMode.Destroy);
			return;
		}
		else
		{
			if (delayTime <= 0.0f)
			{
				eIceTornadoMode nextMode = currentMode;
				
				switch(currentMode)
				{
				case eIceTornadoMode.Deactivate:
					nextMode = eIceTornadoMode.Activate;
					break;
				case eIceTornadoMode.Activate:
					nextMode = eIceTornadoMode.Deactivate;
					break;
				}
				
				SetMode(nextMode);
			}
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
	
	public void SetMoveDir(Vector3 dir)
	{
		if (dir != Vector3.left && dir != Vector3.right)
			return;
		
		Vector3 vScale = this.transform.localScale;
        if (dir == Vector3.right && vScale.x < 0.0f)
        {
            vScale.x *= -1.0f;
            this.transform.localScale = vScale;
        }
        else if (dir == Vector3.left && vScale.x > 0.0f)
        {
            vScale.x *= -1.0f;
            this.transform.localScale = vScale;
        }
	}
}
