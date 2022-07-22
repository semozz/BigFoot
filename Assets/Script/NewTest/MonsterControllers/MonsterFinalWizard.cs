using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterFinalWizard : MonsterWizard {
	
	public float teleportACoolTime = 30.0f;
	public float teleportBCoolTime = 23.0f;
	public float addCoolTime = 5.0f;
	
	private float teleportADelayTime = 0.0f;
	private float teleportBDelayTime = 0.0f;
	
	
	public float manaShieldRate = 1.0f;
	public float manaShieldBuffTime = 2.0f;
	
	public enum eTeleportType
	{
		None,
		TeleportToPlayerFront,
		TeleportTargetArea,
	}
	private eTeleportType teleportType = eTeleportType.None;
	public List<Transform> teleportTarget = new List<Transform>();
	private int teleportTargetIndex = -1;
	
	public BaseState.eState specialAttack = BaseState.eState.Attack2;
	
	// Use this for initialization
	public override void Start () {
		base.Start();
		
		if (moveController != null && moveController.stageManager != null)
		{
			this.teleportTarget.Clear();
			
			foreach(TeleportTarget info in moveController.stageManager.teleportTargetList)
			{
				if (info.target != null)
					this.teleportTarget.Add(info.target.transform);
			}
		}
		
		teleportADelayTime = teleportACoolTime;
		teleportBDelayTime = teleportBCoolTime;
	}
	
	private bool CheckTeleport(bool bNormalState)
	{
		bool teleportB = false;
		bool teleportA = false;
		
		if (teleportBDelayTime <= 0.0f)
			teleportB = true;
		if (teleportADelayTime <= 0.0f)
			teleportA = true;
		
		
		bool bAvailableState = false;
		BaseState.eState currentState = stateController.currentState;
		
		if (bNormalState == true)
		{
			switch(currentState)
			{
			case BaseState.eState.Stun:
			case BaseState.eState.Stand:
			case BaseState.eState.Damage:
				bAvailableState = true;
				break;
			}
		}
		else
		{
			bAvailableState = true;
		}
		
		if (bAvailableState == false)
			return false;
		
		if (teleportB == true)
		{
			int teleportTargetCount = 0;
			if (this.teleportTarget != null)
				teleportTargetCount =this.teleportTarget.Count;
			
			if (teleportTargetCount > 0)
			{
				teleportType = eTeleportType.TeleportTargetArea;
				teleportTargetIndex = (teleportTargetIndex + 1) % teleportTargetCount;
				
				stateController.ChangeState(BaseState.eState.Fly_Up);
				return true;
			}
		}
		
		if (teleportA == true)
		{
			teleportType = eTeleportType.TeleportToPlayerFront;
						
			stateController.ChangeState(BaseState.eState.Fly_Up);
			return true;
		}
		
		return false;
	}
	
	private void Teleport()
	{
		Vector3 targetPos = this.gameObject.transform.position;
		
		switch(this.teleportType)
		{
		case eTeleportType.TeleportTargetArea:
			targetPos = GetTeleportTarget(teleportTargetIndex);
			
			//자기 쿨타임은 리셋 하고, 
			teleportBDelayTime = teleportBCoolTime;
			//다른 쿨타임은 증가
			teleportADelayTime += addCoolTime;
			
			break;
		case eTeleportType.TeleportToPlayerFront:
			if (targetInfo != null && targetInfo.actorType == ActorInfo.ActorType.Player)
			{
				targetPos = targetInfo.transform.position;
				
				BaseMoveController baseMove = targetInfo.gameObject.GetComponent<BaseMoveController>();
				
				Vector3 searchDir = Vector3.zero;
				if (baseMove != null)
					searchDir = baseMove.moveDir;
				
				Vector3 findPos = targetPos;
				
				var layerMaskValue = 1 << LayerMask.NameToLayer("Ground");
				
				for (int i = 1; i >= -1; --i)
				{
					RaycastHit hit;
					Vector3 vStartPos = targetPos + (searchDir * (float)i * baseMove.colliderRadius) + Vector3.up;
					if (Physics.Raycast(vStartPos, Vector3.down, out hit, Mathf.Infinity, layerMaskValue) == true)
					{
						findPos = hit.point;
						break;
					}
				}
				
				targetPos = findPos;
			}
			
			//자기 쿨타임은 리셋하고.
			teleportADelayTime = teleportACoolTime;
			//다른 쿨타임은 증가
			teleportBDelayTime += addCoolTime;
			break;
		}
		
		this.gameObject.transform.position = targetPos;
		
		/*
		//캐릭터 앞으로 순간 이동후 바로 생체 폭탄 시전...
		if (this.teleportType == eTeleportType.TeleportToPlayerFront)
			stateController.ChangeState(specialAttack);
		*/
	}
	
	private Vector3 GetTeleportTarget(int index)
	{
		Vector3 targetPos = this.gameObject.transform.position;
		
		Transform trans = null;
		int nCount = this.teleportTarget.Count;
		if (index >= 0 && index < nCount)
			trans = this.teleportTarget[index];
		
		if (trans != null)
			targetPos = trans.position;
		
		RaycastHit hit;
		Vector3 vStartPos = targetPos + (Vector3.up * 1.0f);
		var layerMask = 1 << LayerMask.NameToLayer("Ground");
		if (Physics.Raycast(vStartPos, Vector3.down, out hit, Mathf.Infinity, layerMask) == true)
		{
			targetPos = hit.point;
		}
		
		return targetPos;
	}
	
	public override void Update()
	{
		if (targetInfo != null)
		{
			teleportADelayTime -= Time.deltaTime;
			teleportBDelayTime -= Time.deltaTime;
		}
		
		base.Update();
		
		CheckTeleport(true);
	}
	
	public override BaseState.eState ChangeNextState()
	{
		BaseState.eState nextState = base.ChangeNextState();
		
		switch(stateController.currentState)
		{
		case BaseState.eState.Fly_Up:
			Teleport();
			//캐릭터 앞으로 순간 이동후 바로 생체 폭탄 시전...
			if (this.teleportType == eTeleportType.TeleportToPlayerFront)
			{
				nextState = specialAttack;
				
				float abilityPower = lifeManager.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.AbilityPower);
				float manaShieldValue = abilityPower * manaShieldRate;
				
				int buffIndex = lifeManager.buffManager.GetAppliedBuffIndex(GameDef.eBuffType.BT_MANASHIELD, lifeManager);
				if (buffIndex != -1)
					lifeManager.buffManager.RemoveBuff(buffIndex);
				
				lifeManager.buffManager.AddBuff(GameDef.eBuffType.BT_MANASHIELD, manaShieldValue, manaShieldBuffTime, lifeManager, 1);
			}
			break;
		}
		
		if (stateController.currentState != BaseState.eState.Fly_Up &&
			nextState == BaseState.eState.Stand)
		{
			if (CheckTeleport(false) == true)
				nextState = stateController.currentState;
		}
		
		return nextState;
	}
	
	public override void OnFire()
	{
		OnCollisionStart();
		
		FireProjectile();
		
		OnCollisionStop();
	}
}
