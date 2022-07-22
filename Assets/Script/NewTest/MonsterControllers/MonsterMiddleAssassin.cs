using UnityEngine;
using System.Collections;

public class MonsterMiddleAssassin : BaseMonster {
	public float vanishDurationCoolTime = 3.5f;
	//나타 나는 시간 범위 설정용.
	public float beginRate = 0.8f;
	public float endRate = 1.2f;
	
	private float vanishDurationDelayTime = 0.0f;
	
	
	public GameObject recallMonsterPrefab = null;
	
	public float vanishAttackRange = 1.0f;
	
	// Use this for initialization
	public override void Start () {
		base.Start();
	}
	
	public override void Update()
	{
		base.Update();
		
		switch(stateController.currentState)
		{
		case BaseState.eState.Fly_Up:
			break;
		case BaseState.eState.Fly_Down:
			UpdateVanish();
			break;
		}
	}
	
	public void RecallMonster()
	{
		if (recallMonsterPrefab == null)
			return;
		
		GameObject go = (GameObject)Instantiate(recallMonsterPrefab);
		
		go.transform.position = this.transform.position;
	}
	
	public void HideMesh(bool bHide)
	{
		if (lifeManager != null && lifeManager.meshNode != null)
			lifeManager.meshNode.SetActive(bHide == false);
	}
	
	public override void OnAnimationBegin()
	{
		base.OnAnimationBegin();
		
		switch(stateController.currentState)
		{
		case BaseState.eState.Fly_Up:
			break;
		case BaseState.eState.Fly_Down:
			RecallMonster();
			break;
		}
	}
	
	public override void OnChangeState(CharStateInfo info)
	{
		base.OnChangeState(info);
		
		switch(stateController.currentState)
		{
		case BaseState.eState.Fly_Down:
			float beginTime = vanishDurationCoolTime * 0.8f;
			float endTime = vanishDurationCoolTime * 1.2f;
			vanishDurationDelayTime = Random.Range(beginTime, endTime);
			HideMesh(true);
			moveController.ChangeHideLayer();
			break;
		}
	}
	
	public override void OnEndState()
	{
		base.OnEndState();
		
		switch(stateController.currentState)
		{
		case BaseState.eState.Fly_Up:
			break;
		case BaseState.eState.Fly_Down:
			HideMesh(false);
			moveController.ChangeDefaultLayer(true);
			break;
		}
	}
	public override BaseState.eState ChangeNextState()
	{
		BaseState.eState nextState = base.ChangeNextState();
		
		switch(stateController.currentState)
		{
		case BaseState.eState.Fly_Up:
			nextState = BaseState.eState.Fly_Down;
			break;
		case BaseState.eState.Fly_Down:
			nextState = BaseState.eState.Fly_Down;
			break;
		}
		
		return nextState;
	}
	
	public void UpdateVanish()
	{
		bool bAvailState = true;
		
		if (targetInfo != null)
		{
			StateController state = targetInfo.gameObject.GetComponent<StateController>();
			if (state != null)
				bAvailState = !state.IsJumpState();
		}
		
		vanishDurationDelayTime -= Time.deltaTime;
			
		if (vanishDurationDelayTime <= 0.0f && bAvailState == true)
		{
			vanishDurationDelayTime = vanishDurationCoolTime;
			
			SetVanishAttackPos();
			stateController.ChangeState(BaseState.eState.Fly_DownAttack);
		}
	}
	
	private void SetVanishAttackPos()
	{
		if (targetInfo != null)
		{
			Vector3 attackPos = targetInfo.transform.position;
			
			BaseMoveController targetMove = targetInfo.gameObject.GetComponent<BaseMoveController>();
			Vector3 moveDir = -this.moveController.moveDir;
			if (targetMove != null)
				moveDir = targetMove.moveDir;
			
			RaycastHit hit;
			bool findAttackPos = false;
			float findAttackDelta = vanishAttackRange;
			Vector3 targetPos = Vector3.zero;
			
			LayerMask tempLayerMask = targetMove.layerMask ^ targetMove.enemyLayerMask;
			
			//1. 타겟 뒤쪽으로 공격 지점을 찾는다..
			Vector3 vStartPos = targetInfo.transform.position - (moveDir * targetInfo.colliderRadius);
			if (Physics.Raycast(vStartPos, -moveDir, out hit, findAttackDelta, tempLayerMask) == true)
				findAttackDelta = Mathf.Max(0.0f, (hit.distance - targetInfo.colliderRadius));
			
			if (findAttackDelta > 0.0f)
			{
				targetPos = vStartPos - (moveDir * findAttackDelta);
				if (Physics.Raycast(targetPos + (Vector3.up * 0.5f), Vector3.down, out hit, float.MaxValue, targetMove.layerMask) == true)
				{
					if (hit.collider == targetInfo.GetGroundCollider())
					{
						attackPos = hit.point;
						findAttackPos = true;
					}
				}
			}
			
			
			//2. 뒷쪽으로 공격 지점을 못 찾을 경우, 앞으로 공격 지점을 찾는다.
			if (findAttackPos == false)
			{
				findAttackDelta = vanishAttackRange;
				vStartPos = targetInfo.transform.position + (moveDir * targetInfo.colliderRadius);
				if (Physics.Raycast(vStartPos, moveDir, out hit, findAttackDelta, tempLayerMask) == true)
					findAttackDelta = Mathf.Max(0.0f, (hit.distance - targetInfo.colliderRadius));
	
				if (findAttackDelta > 0.0f)
				{
					targetPos = vStartPos + (moveDir * findAttackDelta);
					if (Physics.Raycast(targetPos + (Vector3.up * 0.5f), Vector3.down, out hit, float.MaxValue, targetMove.layerMask) == true)
					{
						if (hit.collider == targetInfo.GetGroundCollider())
						{
							attackPos = hit.point;
							findAttackPos = true;
						}
					}
				}
			}
			
			//3. 앞/뒷쪽으로 모두 공격지점을 못 찾을 경우.
			if (findAttackPos = false)
				attackPos = targetInfo.transform.position;
			
			this.transform.position = attackPos;
			
			Vector3 diff = targetInfo.transform.position - attackPos;
			moveDir = Vector3.right;
			if (diff.x <= 0.0f)
				moveDir = Vector3.left;
			else
				moveDir = Vector3.right;
			
			if (moveController != null)
				moveController.ChangeMoveDir(moveDir);
		}
	}
}
