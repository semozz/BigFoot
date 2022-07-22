using UnityEngine;
using System.Collections;

public class MonsterWizard : BaseMonster {
	public GameObject fireBallPrefab = null;
	
	public Vector3 fireBallDeltaPos = new Vector3(4.0f, 7.0f, 0.0f);
	
	// Use this for initialization
	public override void Start () {
		base.Start();
	
	}
	
	public Vector3 GetTargetPos(ActorInfo targetInfo)
	{
		Vector3 targetPos = targetInfo.transform.position;
		Vector3 checkPos = targetPos + (Vector3.up * 0.5f);
		
		Vector3 delta = Vector3.right * targetInfo.colliderRadius;
		Vector3 leftPos = checkPos - delta;
		Vector3 rightPos = checkPos + delta;
		float checkDistance = float.MaxValue;
		
		var groundLayer = 1 << LayerMask.NameToLayer("Ground");
		
		RaycastHit hit;
		if (Physics.Raycast(checkPos, Vector3.down, out hit, float.MaxValue, groundLayer) == true)
		{
			if (checkDistance > hit.distance)
			{
				targetPos = hit.point;
				checkDistance = hit.distance;
			}
		}
		
		if (Physics.Raycast(leftPos, Vector3.down, out hit, float.MaxValue, groundLayer) == true)
		{
			if (checkDistance > hit.distance)
			{
				targetPos = hit.point;
				checkDistance = hit.distance;
			}
		}
		if (Physics.Raycast(rightPos, Vector3.down, out hit, float.MaxValue, groundLayer) == true)
		{
			if (checkDistance > hit.distance)
			{
				targetPos = hit.point;
				checkDistance = hit.distance;
			}
		}
		
		return targetPos;
	}
	
	public override void FireProjectile()
	{
		if (targetInfo == null)
			return;
		
		GameObject go = (GameObject)Instantiate(fireBallPrefab);
        if (go == null) return;
        FireBall fireBall = go.GetComponent<FireBall>();
        if (fireBall == null) return;

        Vector3 vCreatePos = Vector3.zero;
		Vector3 targetPos = GetTargetPos(attackTargetInfo);
		
		vCreatePos = targetPos + fireBallDeltaPos;
		
		go.transform.position = vCreatePos;
		
		Vector3 vMoveDir = targetPos - vCreatePos;
		vMoveDir.Normalize();
		
        fireBall.MoveDir = vMoveDir;
        fireBall.lookDir = vMoveDir;
		fireBall.dropPos = targetPos;
		
		fireBall.bShowDropIndicator = true;
		
		fireBall.SetOwnerActor(lifeManager);
		fireBall.SetAttackInfo(lifeManager.GetCurrentAttackInfo());
        fireBall.SetFired();
		
		if (this.moveController != null)
			this.moveController.SetProjectileCollider(fireBall.detectCollider, attackTargetInfo);
	}
}
