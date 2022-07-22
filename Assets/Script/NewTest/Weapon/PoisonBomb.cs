using UnityEngine;
using System.Collections;

public class PoisonBomb : BaseWeapon {
	public Collider damage1Collider = null;
	public Collider damage2Collider = null;
	
	public float activate1CoolTime = 0.7f;
	public float activate2CoolTime = 0.8f;
	public float deactivateCoolTime = 0.1f;
	
	private float delayTime = 0.0f;
	
	public enum ePoisonBombMode
	{
		None = -1,
		Deactivate,
		Activate1,
		Deactivate1,
		Activate2,
		Deactivate2,
		Destroy
	}
	private ePoisonBombMode currentMode = ePoisonBombMode.None;
	
	public delegate float CalcPoisionAttack();
	public CalcPoisionAttack calcPoisionAttack = null;
	
	void Awake()
	{
		SetMode(ePoisonBombMode.Deactivate);
	}
	
	protected void SetMode(ePoisonBombMode mode)
	{
		if (currentMode == mode)
			return;
		
		switch(mode)
		{
		case ePoisonBombMode.Deactivate:
			delayTime = activate1CoolTime;
			SetupCollider(damage1Collider, false);
			SetupCollider(damage2Collider, false);
			
			hitObjects.Clear();
			break;
		case ePoisonBombMode.Activate1:
			delayTime = deactivateCoolTime;
			SetupCollider(damage1Collider, true);
			SetupCollider(damage2Collider, false);
			hitObjects.Clear();
			break;
		case ePoisonBombMode.Deactivate1:
			delayTime = activate2CoolTime;
			SetupCollider(damage1Collider, false);
			SetupCollider(damage2Collider, false);
			
			hitObjects.Clear();
			
			//Activate1에서 독 버프 적용 하고, 없앤다...
			this.attackInfo.buffList.Clear();
			break;
		case ePoisonBombMode.Activate2:
			delayTime = deactivateCoolTime;
			SetupCollider(damage1Collider, false);
			SetupCollider(damage2Collider, true);
			break;
		case ePoisonBombMode.Deactivate2:
			delayTime = deactivateCoolTime;
			SetupCollider(damage1Collider, false);
			SetupCollider(damage2Collider, false);
			
			hitObjects.Clear();
			break;
		case ePoisonBombMode.Destroy:
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
			ePoisonBombMode nextMode = currentMode;
			
			switch(currentMode)
			{
			case ePoisonBombMode.Deactivate:
				nextMode = ePoisonBombMode.Activate1;
				break;
			case ePoisonBombMode.Activate1:
				nextMode = ePoisonBombMode.Deactivate1;
				break;
			case ePoisonBombMode.Deactivate1:
				nextMode = ePoisonBombMode.Activate2;
				break;
			case ePoisonBombMode.Activate2:
				nextMode = ePoisonBombMode.Deactivate2;
				break;
			case ePoisonBombMode.Deactivate2:
				nextMode = ePoisonBombMode.Destroy;
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
			if (this.hitObjects.Contains(actor) == true)
				return;
			else
				AddHitObject(actor);
			
			switch(currentMode)
			{
			case ePoisonBombMode.Activate1:
				Debug.Log("PoisonBomb Damage......" + Time.time);
				actor.OnDamage(this.attackInfo, this.transform, false);
				break;
			case ePoisonBombMode.Activate2:
				int stackCount = 1;
				
				int buffIndex = actor.buffManager.GetAppliedBuffIndex(GameDef.eBuffType.BT_POISION, this.attackInfo.ownerActor);
				if (buffIndex != -1)
				{
					BuffManager.stBuff oldBuff = actor.buffManager.mHaveBuff[buffIndex];
				
					stackCount = Mathf.Min(oldBuff.StackCount + 1, (int)GameDef.ePoisonLevel.MAX_COUNT);
					actor.buffManager.RemoveBuff(buffIndex);
				}
				
				float attackValue = 0.0f;
				if (calcPoisionAttack != null)
					attackValue = calcPoisionAttack();
				
				actor.buffManager.AddBuff(GameDef.eBuffType.BT_POISION, attackValue, 9.9f, this.attackInfo.ownerActor, stackCount);
				break;
			}
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
