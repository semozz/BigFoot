using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterFinalBerserk : BaseMonster {
	public BaseState.eState specialAttack = BaseState.eState.Attack3;
	public BaseState.eState stunAfterAttack = BaseState.eState.Attack2;
	public float stunTimeAfterAttack = 2.0f;
	
	public override BaseAttackInfo ChooseAttackIndex(Vector3 targetPos, float targetRadius, bool bSameGround)
	{
		LifeManager targetLifeMgr = null;
		if (targetInfo != null)
			targetLifeMgr = targetInfo.GetComponent<LifeManager>();
		
		if (targetLifeMgr != null && targetLifeMgr.GetHP() <= 0.0f)
			return null;
		
		BaseAttackInfo attackInfo = null;
		
		//if (CanAttackState() == true)
		{
			List<BaseAttackInfo> availableAttackList = new List<BaseAttackInfo>();
			
			int randValue = Random.Range(0, 100);
			Vector3 vDiff = targetPos - this.transform.position;
			float diffX = Mathf.Max(0.0f, Mathf.Abs(vDiff.x) - (myInfo.colliderRadius + targetRadius));
			float diffY = vDiff.y;
			
			if (Mathf.Abs(diffY) < 0.1f)
				diffY = 0.0f;
			
			int nCount = attackList.Count;
			for (int i = 0; i < nCount; ++i)
			{
				BaseAttackInfo info = attackList[i];
				
				if (CanAttackState(info) == false)
					continue;
				
				if (info.IsAvailableAttack(randValue, diffX, diffY, bSameGround) == false)
					continue;
				
				if (specialAttack == info.attackState)
				{
					if (CheckSpecialAttack() == false)
						continue;
				}
				
				availableAttackList.Add(info);
			}
			
			nCount = availableAttackList.Count;
			if (nCount > 1)
				availableAttackList.Sort(BaseAttackInfo.SortFunc);
			
			if (nCount > 0)
				attackInfo = availableAttackList[0];
		}
		
		return attackInfo;
	}
	
	private bool CheckSpecialAttack()
	{
		BuffManager buffManager = null;
		if (lifeManager != null)
			buffManager = this.lifeManager.buffManager;
		
		bool hasDebuff = false;
		bool availState = false;
		
		if (buffManager != null)
		{
			foreach(BuffManager.stBuff buff in buffManager.mHaveBuff)
			{
				switch(buff.BuffType)
				{
				case GameDef.eBuffType.BT_CURSE:
				case GameDef.eBuffType.BT_POISION:
				case GameDef.eBuffType.BT_SLOW:
					hasDebuff = true;
					break;
				}
			}
		}
		
		switch(stateController.currentState)
		{
		case BaseState.eState.Stun:
		case BaseState.eState.Damage:
			availState = true;
			break;
		}
		
		return hasDebuff || availState;
	}
	
	public override void UpdateDamage()
	{
		base.UpdateDamage();
		
		DoSpecialAttack();
	}
	
	public override void UpdateStun()
	{
		base.UpdateStun();
		
		DoSpecialAttack();
	}
	
	private void DoSpecialAttack()
	{
		if (targetInfo != null)
		{
			WayPointManager targetWayMgr = GetWayPointManager(targetInfo);
			WayPointManager curWayMgr = GetWayPointManager(this.moveController.groundCollider);
			
			bool bSameGround = targetWayMgr == curWayMgr;
			
			Vector3 targetPos = targetInfo.transform.position;
			float targetRadius = targetInfo.colliderRadius;
			
			Vector3 obstaclePos = targetPos;
			if (moveController != null && moveController.CheckObstacle(out obstaclePos, limitObstacleLength) == true)
				targetPos = obstaclePos;
			
			BaseAttackInfo attackInfo = null;
			if (targetInfo != null && CheckAttackTarget(targetInfo) == true)
				attackInfo = ChooseAttackIndex(targetPos, targetRadius, bSameGround);
				
			if (attackInfo != null && attackInfo.attackState == specialAttack)
			{
				stateController.ChangeState(attackInfo.attackState);
				attackInfo.ResetCoolTime();
				
				attackTargetInfo = targetInfo;
			}
		}
	}
	
	public override void OnChangeState(CharStateInfo info)
	{
		base.OnChangeState(info);
		
		if (stateController.currentState == specialAttack)
		{
			RemoveAllDebuff();
		}
	}
	
	private void RemoveAllDebuff()
	{
		BuffManager buffManager = null;
		if (lifeManager != null)
			buffManager = this.lifeManager.buffManager;
		
		//bool hasDebuff = false;
		//bool availState = false;
		
		if (buffManager != null)
		{
			List<int> deBuffIndexList = new List<int>();
			int nCount = buffManager.mHaveBuff.Count;
			int index = 0;
			for (index = nCount - 1; index >= 0; --index)
			{
				BuffManager.stBuff buff = buffManager.mHaveBuff[index];
				
				switch(buff.BuffType)
				{
				case GameDef.eBuffType.BT_CURSE:
				case GameDef.eBuffType.BT_POISION:
				case GameDef.eBuffType.BT_SLOW:
					deBuffIndexList.Add(index);
					break;
				}
			}
			
			
			foreach(int buffIndex in deBuffIndexList)
			{
				buffManager.RemoveBuff(buffIndex);
			}
		}
	}
	
	public override BaseState.eState ChangeNextState()
	{
		BaseState.eState nextState = base.ChangeNextState();
		
		if (stunAfterAttack != BaseState.eState.None &&
			stateController.currentState == stunAfterAttack)
		{
			lifeManager.stunDelayTime = stunTimeAfterAttack;
			nextState = BaseState.eState.Stun;
		}
		
		return nextState;
	}
	
	public override bool CanAttackState(BaseAttackInfo attackInfo)
	{
		bool canAttack = base.CanAttackState(attackInfo);
		
		switch(stateController.currentState)
		{
		case BaseState.eState.Damage:
		case BaseState.eState.Stun:
			canAttack = true;
			break;
		}
		
		return canAttack;
	}
}
