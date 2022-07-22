using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterFinalDevil : BaseMonster {
	public GameObject dropIndicatorEffect = null;
	
	public float flyDurationCoolTime = 3.5f;
	private float flyDurationDelayTime = 0.0f;
	
	private int flyUpCount = 0;
	
	public BaseState.eState repulseAttackState = BaseState.eState.Attack2;
	public BaseState.eState flyUpState = BaseState.eState.Fly_Up;
	
	// Use this for initialization
	public override void Start () {
		base.Start();
		
		DisplayIndicator(false);
		
		/*
		animEventTrigger.onAnimationBegin += new AnimationEventTrigger.OnAnimationEvent(OnAnimationBegin);
		animEventTrigger.onAnimationEnd += new AnimationEventTrigger.OnAnimationEvent(OnAnimationEnd);
		*/
	}
	
	public override void Update()
	{
		base.Update();
		
		switch(stateController.currentState)
		{
		case BaseState.eState.Fly_Up:
			UpdateFlyUp();
			break;
		}
	}
	
	private void DisplayIndicator(bool bShow)
	{
		if (dropIndicatorEffect != null)
			dropIndicatorEffect.SetActive(bShow);
	}
	
	public override void OnAnimationBegin()
	{
		base.OnAnimationBegin();
		
		switch(stateController.currentState)
		{
		case BaseState.eState.Fly_Up:
			moveController.ChangeJumpLayer();
			flyDurationDelayTime = flyDurationCoolTime;
			break;
		}
	}
	
	public override void OnEndState()
	{
		base.OnEndState();
		
		switch(stateController.currentState)
		{
		case BaseState.eState.Fly_Up:
			flyUpCount = 0;
			if (moveController != null && moveController.collider != null)
				moveController.collider.enabled = true;
			break;
		}
	}
	public override BaseState.eState ChangeNextState()
	{
		BaseState.eState nextState = base.ChangeNextState();
		
		switch(stateController.currentState)
		{
		case BaseState.eState.Fly_Up:
			nextState = BaseState.eState.Fly_Up;
			flyUpCount++;
			if (moveController != null && moveController.collider != null)
				moveController.collider.enabled = false;
			break;
		case BaseState.eState.Fly_Down:
			moveController.ChangeDefaultLayer(true);
			nextState = BaseState.eState.Fly_DownAttack;
			break;
		}
		
		return nextState;
	}
	
	public void UpdateFlyUp()
	{
		if (flyUpCount == 0)
			return;
		
		flyDurationDelayTime -= Time.deltaTime;
		
		bool bDisplayIndicator = false;
		if (flyDurationDelayTime > 0.0f && flyDurationDelayTime <= 1.0f)
			bDisplayIndicator = true;
		
		DisplayIndicator(bDisplayIndicator);
			
		if (flyDurationDelayTime <= 0.0f)
		{
			flyDurationDelayTime = flyDurationCoolTime;
			stateController.ChangeState(BaseState.eState.Fly_Down);
		}
		
		UpdateTargetFallow();
	}
	
	public void UpdateTargetFallow()
	{
		Vector3 findPos = this.gameObject.transform.position;
		
		if (targetInfo != null)
		{
			Vector3 startPos = targetInfo.gameObject.transform.position + Vector3.up * 1.0f;
			
			var layerMask = 1 << LayerMask.NameToLayer("Ground");
			
			RaycastHit hit;
			if (Physics.Raycast(startPos, Vector3.down, out hit, Mathf.Infinity, layerMask) == true)
				findPos = hit.point;
		}
		
		this.gameObject.transform.position = findPos;
	}
	
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
				
				if (info.IsAvailableAttack(randValue, diffX, diffY, bSameGround) == false)
					continue;
				
				if (info.attackState == repulseAttackState)
				{
					if (CheckRepulseAttack() == false)
						continue;
				}
				else if (info.attackState == flyUpState)
				{
					if (CanAttackState(info) == false || CheckFlyUpAttack() == false)
						continue;
				}
				else 
				{
					if (CanAttackState(info) == false)
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
	
	public bool CheckRepulseAttack()
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
	
	public bool CheckFlyUpAttack()
	{
		float hpRate = lifeManager.GetHPRate();
		
		return hpRate <= 0.5f;
	}
	
	public override void OnChangeState(CharStateInfo info)
	{
		base.OnChangeState(info);
		
		if (stateController.currentState == repulseAttackState)
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
	
	public override void UpdateDamage()
	{
		base.UpdateDamage();
		
		if (this.moveController != null && targetInfo != null)
		{
			WayPointManager targetWayMgr = GetWayPointManager(targetInfo);
			WayPointManager curWayMgr = GetWayPointManager(this.moveController.groundCollider);
			
			bool bSameGround = targetWayMgr == curWayMgr;
			
			Vector3 targetPos = targetInfo.transform.position;
			float targetRadius = targetInfo.colliderRadius;
			
			//Vector3 obstaclePos = targetPos;
			
			BaseAttackInfo attackInfo = null;
			if (targetInfo != null && CheckAttackTarget(targetInfo) == true)
				attackInfo = ChooseAttackIndex(targetPos, targetRadius, bSameGround);
			
			if (attackInfo != null)
			{
				ChangeMoveDir(targetPos);
				
				stateController.ChangeState(attackInfo.attackState);
				attackInfo.ResetCoolTime();
				
				attackTargetInfo = targetInfo;
				return;
			}
		}
	}
	
	public override void UpdateStun()
	{
		base.UpdateStun();
		
		if (this.moveController != null && targetInfo != null)
		{
			WayPointManager targetWayMgr = GetWayPointManager(targetInfo);
			WayPointManager curWayMgr = GetWayPointManager(this.moveController.groundCollider);
			
			bool bSameGround = targetWayMgr == curWayMgr;
			
			Vector3 targetPos = targetInfo.transform.position;
			float targetRadius = targetInfo.colliderRadius;
			
			//Vector3 obstaclePos = targetPos;
			
			BaseAttackInfo attackInfo = null;
			if (targetInfo != null && CheckAttackTarget(targetInfo) == true)
				attackInfo = ChooseAttackIndex(targetPos, targetRadius, bSameGround);
			
			if (attackInfo != null)
			{
				ChangeMoveDir(targetPos);
				
				stateController.ChangeState(attackInfo.attackState);
				attackInfo.ResetCoolTime();
				
				attackTargetInfo = targetInfo;
				return;
			}
		}
	}
}
