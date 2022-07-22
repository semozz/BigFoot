using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CurseZone : BaseWeapon {
	public BoxCollider damageCollider = null;
	public float activateCoolTime = 0.5f;
	public float deactivateCooTime = 2.0f;
	
	public float curseBuffDurationTime = 2.0f;
	
	private float delayTime = 0.0f;
	
	public enum eCurseZoneMode
	{
		None = -1,
		Deactivate,
		Activate,
	}
	private eCurseZoneMode currentMode = eCurseZoneMode.None;
	
	public override void Start()
	{
		
	}
	
	public override void OnDestroy()
	{
		
	}
	
	void Awake()
	{
		SetMode(eCurseZoneMode.Deactivate);
	}
	
	protected void SetMode(eCurseZoneMode mode)
	{
		if (currentMode == mode)
			return;
		
		switch(mode)
		{
		case eCurseZoneMode.Deactivate:
			delayTime = deactivateCooTime;
			SetupCollider(damageCollider, false);
			
			hitObjects.Clear();
			break;
		case eCurseZoneMode.Activate:
			delayTime = activateCoolTime;
			SetupCollider(damageCollider, true);
			break;
		}
		
		currentMode = mode;
	}
	
	// Update is called once per frame
	void Update () {
		delayTime -= Time.deltaTime;
		if (delayTime <= 0.0f)
		{
			eCurseZoneMode nextMode = currentMode;
			
			switch(currentMode)
			{
			case eCurseZoneMode.Deactivate:
				nextMode = CurseZone.eCurseZoneMode.Activate;
				break;
			case eCurseZoneMode.Activate:
				nextMode = CurseZone.eCurseZoneMode.Deactivate;
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
	
	public void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer != LayerMask.NameToLayer("PlayerBody"))
			return;
		
		LifeManager actor = other.gameObject.GetComponent<LifeManager>();
		if (actor != null)
		{
			Debug.Log("CurseZone Damage......" + Time.time);
			
			actor.OnDamage(this.attackInfo, other.transform, false);
			
			BuffManager buffManager = actor.buffManager;
			
			if (buffManager != null)
			{
				int buffIndex = buffManager.GetAppliedBuffIndex(GameDef.eBuffType.BT_CURSE, this.attackInfo.ownerActor);
				if (buffIndex != -1)
					buffManager.RemoveBuff(buffIndex);
				
				buffManager.AddBuff(GameDef.eBuffType.BT_CURSE, this.attackInfo.GetAttackDamage(0.0f), curseBuffDurationTime, this.attackInfo.ownerActor, 1);
			}
		}
	}
}
