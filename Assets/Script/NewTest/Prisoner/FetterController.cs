using UnityEngine;
using System.Collections;

public class FetterController : BaseMonster {
	public enum eFetterState
	{
		Stand,
		Damage,
		Die,
	}
	public eFetterState fetterState = eFetterState.Stand;
	
	public int hitCount = 3;
	
	public Collider bodyCollider = null;
	public PrisonerActor prisoner = null;
	
	public LayerMask layerMaskValue = 0;
	
	public override void Start () {
		base.Start();
		
		this.fetterState = eFetterState.Stand;
		
		Vector3 vPos = this.transform.position;
		RaycastHit hitInfo;
		if (Physics.Raycast(vPos, Vector3.down, out hitInfo, float.MaxValue, layerMaskValue) == true)
		{
			this.transform.position = hitInfo.point;
		}
	}
	
	public override void Update()
	{
		if (Game.Instance.Pause == true)
			return;
		
		switch(this.fetterState)
		{
		case eFetterState.Stand:
			break;
		case eFetterState.Damage:
			break;
		case eFetterState.Die:
			break;
		}
	}
	
	/*
	public override void OnChangeState(CharStateInfo info)
	{
		
	}
	
	public override void OnEndState()
	{
		
	}
	
	public override void OnAnimationBegin()
	{
		
	}
	*/
	
	public override void OnAnimationEnd()
	{
		stateController.animationController.isAnimationPlaying = false;
		
		BaseState.eState nextState = BaseState.eState.Stand;
		
		switch(stateController.currentState)
		{
		case BaseState.eState.Damage:
			nextState = BaseState.eState.Stand;
			break;
		case BaseState.eState.Die:
			nextState = BaseState.eState.Die;
			if (destroyDelayTime >= 0.0f)
			{
				bDestroyCalled = true;
				destroyAlphaDelayTime = destroyDelayTime;
				DestroyObject(this.gameObject, destroyDelayTime);
			}
			break;
		}
		
		stateController.ChangeState(nextState);
	}
	
	/*
	public LayerMask attackableLayers = 0;
	void OnTriggerEnter(Collider other)
	{
		int layerValue = 1 << other.gameObject.layer;
		if ((attackableLayers & layerValue) == 0)
			return;
	
		if (hitCount > 0)
		{
			Debug.Log(this + " OnTriggerEnter : " + other);
			hitCount--;
			
			stateController.ChangeState(BaseState.eState.Damage);
			ApplyDamageEffect();
			
			if (hitCount <= 0)
			{
				stateController.ChangeState(BaseState.eState.Die);
				DestroyObject(this.gameObject, 1.0f);
				
				if (this.prisoner != null)
					this.prisoner.RunAway();
			}
		}
		
	}
	
	public string fxDamage = "FX_damage03";
	public void ApplyDamageEffect()
	{
		if (stateController != null)
			stateController.AddFXDelayInfo(fxDamage, eFXEffectType.ScaleNode, 1.0f, 0.8f);
	}
	*/
	
	
	public int dropRate = 50;
	public int dropStringID = 802;
	public int dontDropStringID = 803;
	public float dropStringDelayTime = 1.0f;
	
	public override void OnDie (LifeManager attacker)
	{
		//base.OnDie (attacker);
		
		int randValue = Random.Range(0, 100);
		int stringID = dontDropStringID;
		if (randValue <= dropRate && this.dropTableID > 0)
		{
			MakeDropItem(attacker);
			stringID = dropStringID;
		}
		
		if (this.prisoner != null)
			this.prisoner.RunAway(stringID, dropStringDelayTime);
	}
}
