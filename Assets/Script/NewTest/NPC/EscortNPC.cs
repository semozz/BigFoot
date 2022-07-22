using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EscortNPC : BaseMonster {
	public Transform targetPos = null;
	
	public BaseState.eState endState = BaseState.eState.StageEnd;
	public BaseState.eState fearState = BaseState.eState.Stage_clear1;
	public BaseState.eState restState = BaseState.eState.Runaway;
	
	public string hpPrefabPath = "UI/Event/EscortNPCHP";
	
	public override void Start () 
	{
		this.monsterCheckDelayTime = this.monsterCheckTime;
		base.Start();
		
		if (targetPos != null)
		{
			Vector3 vDir = targetPos.position - this.transform.position;
			vDir.Normalize();
			
			if (moveController != null)
				moveController.ChangeMoveDir(vDir);
		}
		
		Transform uiRoot = GameUI.Instance.uiRootPanel.transform;
		normalHP = ResourceManager.CreatePrefab<NormalHP>(hpPrefabPath, uiRoot, Vector3.zero);
		if (normalHP != null)
		{
			lifeManager.attributeManager.hpUI = normalHP.hp;
			
			lifeManager.onHPValueChange = new UISlider.OnValueChange(OnHPValueChange);
		}
		
		Game.Instance.escortNPC = this;
	}
	
	public override void OnDestroy ()
	{
		base.OnDestroy ();
		
		if (Game.Instance != null)
			Game.Instance.escortNPC = null;
	}
	
	public void OnHPValueChange (float val)
	{
		if (normalHP != null && val > 0.0f)
		{
			normalHP.OnWarning();
		}
	}
	
	public override void Update()
	{
		if (Game.Instance.Pause == true)
			return;
		
		float hpRate = lifeManager.GetHPRate();
		if (hpRate <= 0.0f)
			return;
		
		switch(stateController.currentState)
		{
		case BaseState.eState.Stand:
			UpdateIdle();
			break;
		case BaseState.eState.Run:
		case BaseState.eState.Dash:
			UpdateMove();
			break;
		case BaseState.eState.Stun:
			UpdateStun();
			break;
		case BaseState.eState.Die:
		case BaseState.eState.Knockdown_Die:
			UpdateDie();
			break;
		default:
			if (stateController.currentState == fearState)
				UpdateFear();
			break;
		}
	}
	
	public override void UpdateIdle()
	{
		float hpRate = lifeManager.GetHPRate();
		
		if (hpRate > 0.0f)
			DoRun();
	}
	
	public void UpdateFear()
	{
		if (monsterList.Count == 0)
		{
			if (monsterCheckDelayTime > 0.0f)
				monsterCheckDelayTime -= Time.deltaTime;
			
			if (monsterCheckDelayTime <= 0.0f)
			{
				monsterCheckDelayTime = 0.0f;
				DoRun();
			}
		}
	}
	
	public void DoRun()
	{
		if (targetPos != null)
		{
			Vector3 vDir = targetPos.position - this.transform.position;
			vDir.y = vDir.z = 0.0f;
			float length = vDir.magnitude;
			
			if (length > limitTargetLength)
			{
				vDir.Normalize();
			
				if (moveController != null)
					moveController.ChangeMoveDir(vDir);
			
				this.stateController.ChangeState(BaseState.eState.Run);
			}
		}
	}
	
	public override void OnChangeState(CharStateInfo info)
	{
		//doBackWalk = false;
		
		if (moveController != null)
		{
			float moveSpeed = 0.0f;
			switch(info.moveType)
			{
			case CharStateInfo.eMoveType.Dash:
				moveSpeed = moveController.dashMoveSpeed;
				break;
			case CharStateInfo.eMoveType.Run:
				moveSpeed = moveController.defaultMoveSpeed;
				break;
			case CharStateInfo.eMoveType.Keep:
				moveController.prevMoveSpeed = moveController.moveSpeed;
				moveSpeed = moveController.moveSpeed;
				break;
			case CharStateInfo.eMoveType.Stop:
				moveSpeed = 0.0f;
				break;
			}
			
			moveController.moveSpeed = moveSpeed;
		}
		
		if (lifeManager != null)
		{
			lifeManager.SetAttackInfo(info.stateInfo);
			
			lifeManager.ClearHitObject();
		}
		
		if (stateController.currentState == BaseState.eState.Stand)
		{
			if (moveController != null)
			{
				moveController.ChangeDefaultLayer(true);
				
				if (this.targetInfo != null)
					UpdateTargetPath(this.targetInfo.gameObject);
			}
			
			rigidbody.velocity = Vector3.zero;
			rigidbody.angularVelocity = Vector3.zero;
		}
		
		if (stateController.colliderManager != null)
			stateController.colliderManager.colliderStep = 0;
		
		if (info != null)
			info.InitWalkingStep();
		
		if (info.baseState.state == BaseState.eState.StageEnd)
		{
			DestroyObject(this.gameObject, 1.0f);
		}
		else if (info.baseState.state == restState)
		{
			if (restCount == 0 && restStartDialogID != -1)
				DoTalk(restStartDialogID, restStartDialogDelayTime, DialogInfo.eDialogType.Normal, false);
		}
	}
	
	public override void UpdateMove()
	{
		if (targetPos != null)
		{
			Vector3 vDir = targetPos.position - this.transform.position;
			vDir.y = vDir.z = 0.0f;
			
			float length = vDir.magnitude;
			
			if (length <= limitTargetLength)
			{
				this.stateController.ChangeState(BaseState.eState.Stand);
			}
		}
		
	}
	
	public List<Collider> monsterList = new System.Collections.Generic.List<Collider>();
	public float monsterCheckTime = 1.0f;
	public float monsterCheckDelayTime = 0.0f;
	
	public void OnTriggerEnter(Collider other)
	{
		float hpRate = lifeManager.GetHPRate();
		if (hpRate <= 0.0f)
			return;
		
		int layerValue = 1 << other.gameObject.layer;
		if ((this.moveController.enemyLayerMask & layerValue) == 0)
			return;
		
		LifeManager monsterLifeMgr = other.gameObject.GetComponent<LifeManager>();
		if (monsterLifeMgr == null || monsterLifeMgr.stateController == null)
			return;
		
		if (monsterLifeMgr.GetHP() < 0.01f)
			return;
		
		bool checkState = false;
		switch(monsterLifeMgr.stateController.currentState)
		{
		case BaseState.eState.StageEnd:
		case BaseState.eState.Die:
		case BaseState.eState.Knockdown_Die:
			checkState = true;
			break;
		}
		if (checkState == true)
			return;
		
		if (monsterList.Contains(other) == true)
			return;
		
		monsterList.Add(other);
		
		if (monsterList.Count > 0)
		{
			//monsterCheckDelayTime = monsterCheckTime;
			
			//bool bChange = false;
			switch(stateController.currentState)
			{
			case BaseState.eState.Stand:
			case BaseState.eState.Run:
				this.stateController.ChangeState(fearState);
				break;
			}
		}
	}
	
	public void OnTriggerExit(Collider other)
	{
		float hpRate = lifeManager.GetHPRate();
		if (hpRate <= 0.0f)
			return;
		
		int layerValue = 1 << other.gameObject.layer;
		if ((this.moveController.enemyLayerMask & layerValue) == 0)
			return;
		
		monsterList.Remove(other);
		
		if (monsterList.Count == 0)
		{
			//monsterCheck = false;
			monsterCheckDelayTime = monsterCheckTime;
		}
	}
	
	
	public int restCount = 0;
	public int restLimitCount = 10;
	public override BaseState.eState ChangeNextState()
	{
		BaseState.eState nextState = base.ChangeNextState();
		
		if (stateController.currentState == restState)
		{
			if (restCount > restLimitCount)
				nextState = BaseState.eState.Stand;
			else
				nextState = restState;
			
			DoHeal();
			
			restCount++;
		}
		else if (stateController.currentState == fearState)
		{
			nextState = fearState;
		}
		else if (stateController.currentState == BaseState.eState.Run)
			nextState = BaseState.eState.Run;
		
		if (nextState == BaseState.eState.Stand)
		{
			if (monsterList.Count > 0)
				nextState = fearState;
		}
		
		return nextState;
	}
	
	public int restStartDialogID = -1;
	public float restStartDialogDelayTime = 1.5f;
	public int restEndDialogID = -1;
	public float restEndDialogDelayTime = 1.5f;
	
	public override void OnEndState ()
	{
		if (stateController.currentState == restState)
		{
			if (restEndDialogID != -1)
				DoTalk(restEndDialogID, restEndDialogDelayTime, DialogInfo.eDialogType.Normal, false);
			
			//restCount = 0;
		}
		
		base.OnEndState ();
	}
	
	public void DoHeal()
	{
		float healthMax = lifeManager.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.HealthMax);
		float healAmount = healthMax * 0.05f;
		
		lifeManager.IncHP(healAmount, true, GameDef.eBuffType.BT_REGENHP);
	}
	
	public override void OnDie (LifeManager attacker)
	{
		base.OnDie (attacker);
		
		StageEndEvent stageEndEvent = null;
		if (moveController != null && moveController.stageManager != null)
			stageEndEvent = moveController.stageManager.stageEndEvent;
		if (stageEndEvent != null)
			stageEndEvent.OnStageFailed();
	}
	
	public void DoRest()
	{
		if (restCount == 0)
		{
			if (stateController.currentState != restState)
				stateController.ChangeState(restState);
		}
	}
}
