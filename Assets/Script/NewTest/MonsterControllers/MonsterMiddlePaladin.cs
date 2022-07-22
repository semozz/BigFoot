using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterMiddlePaladin : MonsterPaladin {
	public float limitHP = 0.3f;
	
	public override void Start () {
		base.Start();
		
		if (lifeManager != null)
			lifeManager.onDie = null;
	}
	
	public override bool CheckHealAttack(int randValue, BaseAttackInfo attackInfo)
	{
		healTarget = null;
		
		if (stateController.preState != BaseState.eState.Dashattack)
			return false;
		
		if (attackInfo.IsAvailableAttack(randValue, 0.0f, 0.0f, true) == false)
			return false;
		
		if (this.lifeManager != null && this.lifeManager.attributeManager != null)
		{
			float tempRate = this.lifeManager.GetHPRate();
			
			if (tempRate <= 0.0f || tempRate >= 1.0f)
				return false;
			
			if (tempRate < 1.0f)
			{
				healTarget = this.lifeManager;
				return true;
			}
		}
		
		return false;
	}
	
	public override BaseState.eState ChangeNextState()
	{
		BaseState.eState nextState = base.ChangeNextState();
		
		switch(stateController.currentState)
		{
		case BaseState.eState.Die:
			nextState = BaseState.eState.StageEnd;
			break;
		}
		
		return nextState;
	}
	
	public override void Update()
	{
		base.Update();
		
		float hpRate = this.lifeManager.GetHPRate();
		
		if (bEndCall == false && hpRate <= limitHP)
		{
			Game.Instance.InputPause = true;
			
			ActorManager actorManager = ActorManager.Instance;
			List<ActorInfo> actorList = actorManager.GetActorList(myInfo.myTeam);
			
			if (actorList != null)
			{
				foreach(ActorInfo info in actorList)
				{
					if (info == myInfo)
						continue;
					
					LifeManager lifeManager = info.gameObject.GetComponent<LifeManager>();
					hpRate = lifeManager.GetHPRate();
					if (hpRate <= 0.0f)
						continue;
					
					StateController stateController = info.gameObject.GetComponent<StateController>();
					if (stateController != null)
						stateController.ChangeState(BaseState.eState.StageEnd);
				}
			}
			
			bEndCall = true;
			Invoke("OnEnd", 1.5f);
		}
	}
	
	public bool bEndCall = false;
	public void OnEnd()
	{
		this.isEnableUpdate = false;
		lifeManager.stunDelayTime = 0.0f;
		
		if (buffManager != null)
			buffManager.Init();	
		
		moveController.ChangeDefaultLayer(false);
		stateController.ChangeState(BaseState.eState.Stand);
		
		MonsterEndController endController = this.gameObject.GetComponent<MonsterEndController>();
		if (endController != null)
			endController.SetActivate();
		
		bEndCall = true;
	}
}
