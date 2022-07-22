using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterFinalPriest : MonsterPriest {
	private CheckArea checkArea = null;
	public GameObject checkAreaPrefab = null;
	
	public GameObject curseZonePrefab = null;
	
	public float curseZoneLifeTime = 3.0f;
	public float curseZoneActiveTime = 0.5f;
	public float curseZoneDeactiveTime = 2.0f;
	
	// Use this for initialization
	public override void Start () {
		base.Start();
	}
	
	public override void OnChangeState(CharStateInfo info)
	{
		base.OnChangeState(info);
		
		switch(stateController.currentState)
		{
		case BaseState.eState.Attack2:
			GameObject go = (GameObject)Instantiate(checkAreaPrefab);
			if (go != null)
				checkArea = go.GetComponent<CheckArea>();
			if (checkArea != null)
			{
				checkArea.gameObject.transform.position = this.gameObject.transform.position;
				checkArea.SetupCollider(true);
			}
			break;
		}
	}
	
	public override void OnEndState()
	{
		base.OnEndState();
		
		switch(stateController.currentState)
		{
		case BaseState.eState.Attack2:
			if (checkArea != null)
			{
				checkArea.SetupCollider(false);
				DestroyObject(checkArea.gameObject, 0.0f);
				checkArea = null;
			}
			break;
		}
	}
	
	public override bool CheckHealAttack(int randValue, BaseAttackInfo attackInfo)
	{
		return base.CheckHealAttack(randValue, attackInfo);
	}
	
	public override bool CheckManaShieldAttack(int randValue, BaseAttackInfo attackInfo)
	{
		return base.CheckHealAttack(randValue, attackInfo);
	}
	
	public override void FireProjectile()
	{
		switch (stateController.currentState)
        {
        case BaseState.eState.Attack1:
			//저주 지역 생성...
			GameObject go = (GameObject)Instantiate(curseZonePrefab);
			CurseZone curseZone = null;
			if (go != null)
				curseZone = go.GetComponent<CurseZone>();
			
			if (curseZone != null)
			{
				curseZone.SetAttackInfo(lifeManager.GetCurrentAttackInfo());
				curseZone.SetOwnerActor(lifeManager);
				
				curseZone.activateCoolTime = curseZoneActiveTime;
				curseZone.deactivateCooTime = curseZoneDeactiveTime;
				
				curseZone.gameObject.transform.position = this.gameObject.transform.position;
				
				RaycastHit hit;
				Vector3 vPos = this.gameObject.transform.position;
				
				if (targetInfo != null)
					vPos = attackTargetInfo.transform.position + Vector3.up;
				
				var layerMask = 1 << LayerMask.NameToLayer("Ground");
				if (Physics.Raycast(vPos, Vector3.down, out hit, Mathf.Infinity, layerMask) == true)
				{		
					curseZone.gameObject.transform.position = hit.point;
				}

				DestroyObject(go, curseZoneLifeTime);
			}
			break;
		case BaseState.eState.Attack2:
			if (checkArea != null)
			{
				float healValue = lifeManager.GetAbilityPower();
				
				List<LifeManager> hitList = checkArea.HitObjects;
				foreach(LifeManager hitObj in hitList)
				{
					if (hitObj == null)
						continue;
					
					ActorInfo actorInfo = hitObj.myActorInfo;
					switch(actorInfo.actorType)
					{
					case ActorInfo.ActorType.Monster:
					case ActorInfo.ActorType.BossMonster:
						float hpRate = hitObj.GetHPRate();
						if (hpRate <= 0.0f || hpRate >= 1.0f)
							continue;
						
						hitObj.AddFXDelayInfo(fxHealTarget, fxHealTime);
						hitObj.IncHP(healValue, true, GameDef.eBuffType.BT_REGENHP);
						break;
					}
				}
			}
			
            break;
        case BaseState.eState.Attack3:
			BuffManager buffManager = null;
			if (lifeManager != null)
				buffManager = lifeManager.buffManager;
			
			if (buffManager != null)
			{
				int buffIndex = buffManager.GetAppliedBuffIndex(GameDef.eBuffType.BT_MANASHIELD, lifeManager);
				if (buffIndex != -1)
					buffManager.RemoveBuff(buffIndex);
				
				buffManager.AddBuff(GameDef.eBuffType.BT_MANASHIELD, lifeManager.GetAbilityPower(), manaShieldBuffDurationTime, lifeManager, 1);
			}
			break;
        }
	}
}
