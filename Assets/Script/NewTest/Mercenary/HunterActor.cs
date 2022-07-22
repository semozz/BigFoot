using UnityEngine;
using System.Collections;

public class HunterActor : MercenaryActor {
	public string arrowHandNodeName = "Bip01 Prop1";
	private Transform arrowHand = null;
	public GameObject arrowPrefab = null;
	
	protected Vector3 arrowTargetPos = Vector3.zero;
	
	public GameObject arrowDummy = null;
	
	public override void Start () {
		base.Start();
		
		Transform[] bones = GetComponentsInChildren<Transform>();
        foreach (Transform bone in bones)
        {
            if (bone.name == arrowHandNodeName)
			{
				arrowHand = bone;
				break;
			}
        }
		
		if (arrow == null)
			DoArrowEquip();
	}
	
	public override void OnChangeState(CharStateInfo info)
	{
		base.OnChangeState(info);
		
		if (arrow == null)
			DoArrowEquip();
	}
	
	public override void FireProjectile()
	{
		if (arrowHand == null) return;
		if (attackTargetInfo == null) return;
		
		if (arrow == null) return;
		
        arrow.transform.parent = null;
        Vector3 arrowPos = arrowHand.position;
		arrowPos.z = 0.0f;
		arrow.transform.position = arrowPos;
		
        arrow.transform.localScale = Vector3.one;

        Arrow.eArrowMovingType MovingType = Arrow.eArrowMovingType.AMT_LINE;
        switch(stateController.currentState)
		{
		case BaseState.eState.Heavyattack:
            MovingType = Arrow.eArrowMovingType.AMT_POWERSHOT;
			break;
		case BaseState.eState.Attack2:
			MovingType = Arrow.eArrowMovingType.AMT_PARABOLA;
			break;
		}
		
		BaseMoveController targetMoveController = attackTargetInfo.GetMoveController();
		Vector3 targetSize = Vector3.up;
		if (targetMoveController != null)
			targetSize.y = (targetMoveController.colliderHeight + targetMoveController.colliderRadius) * 0.5f;
		
        Vector3 vMoveDir = moveController.moveDir;
		
        Vector3 vTargetDir =  (arrowTargetPos + targetSize) - arrow.gameObject.transform.position;
        
        if (stateController.currentState == BaseState.eState.Attack3)
            vMoveDir = vTargetDir.normalized;

        arrow.MovingType = MovingType;
        arrow.MoveDir = vMoveDir;
        arrow.TargetDir = vTargetDir;
		
		arrow.lookDir = vMoveDir;
		
		arrow.SetAttackInfo(lifeManager.GetCurrentAttackInfo());
		arrow.SetOwnerActor(lifeManager);
        arrow.SetFired();
		
		arrow.targetGroundCollider = attackTargetInfo.GetGroundCollider();
		
		/*
		if (this.moveController != null)
			this.moveController.SetProjectileCollider(arrow.detectCollider, targetInfo);
		*/
		
		arrow = null;
		SetArrow(false);
	}
	
	public Arrow arrow = null;
	public override void DoArrowEquip()
	{
		if (arrowHand == null) return;
        
		if (arrow == null)
		{
	        GameObject go = (GameObject)Instantiate(arrowPrefab);
	        if (go == null) return;
	        arrow = go.GetComponent<Arrow>();
	        if (arrow == null) return;
	
	        go.transform.parent = arrowHand;
	        go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
			
			go.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
	        if (moveController.moveDir == Vector3.right)
	            go.transform.localRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
			
			if (targetInfo != null)
			{
				arrowTargetPos = targetInfo.transform.position;
			}
			else
			{
				arrowTargetPos = Vector3.zero;
			}
			
			SetArrow(true);
		}
	}
	
	public void SetArrow(bool bActive)
	{
		if (arrowDummy != null)
			arrowDummy.SetActive(bActive);
	}
	
	public override bool HasProjectile()
	{
		return true;
	}
}
