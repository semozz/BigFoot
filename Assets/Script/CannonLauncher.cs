using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CannonLauncher : MonoBehaviour {
	public ActorInfo myInfo = null;
	
	public Transform launchPos = null;
	
	public Castle.eCannonBallType cannonBallType = Castle.eCannonBallType.Noraml;
	
	public List<GameObject> ballList = new List<GameObject>();
	[HideInInspector]
	public GameObject cannonPrefab = null;
	
	public LifeManager ownerActor = null;
	
	public void DoLaunchProjectile(ActorInfo target)
	{
		if (cannonPrefab == null)
		{
			Debug.LogWarning("Cannon prefab is null.....");
			return;
		}
		
		GameObject go = (GameObject)Instantiate(cannonPrefab);
        if (go == null) return;
        CannonBall cannonBall = go.GetComponent<CannonBall>();
        if (cannonBall == null) return;
		
		go.transform.position = launchPos.position;
		
		
		Vector3 vMoveDir = Vector3.zero;
        Vector3 vTargetDir = vMoveDir = (target.transform.position + new Vector3(0.0f, 1.0f, 0.0f)) - go.transform.position;
		
		cannonBall.TargetDir = vTargetDir;
		cannonBall.MoveDir = vMoveDir.normalized;
        
		cannonBall.SetAttackInfo(ownerActor.attackStateInfo);
		cannonBall.SetOwnerActor(ownerActor);
		cannonBall.SetFired();
		
	}
	
	public void ChangeCannonBallType(Castle.eCannonBallType type)
	{
		int index = (int)type;
		GameObject prefab = null;
		
		int nCount = ballList.Count;
		if (index >= 0 && index < nCount)
			prefab = ballList[index];
		
		cannonPrefab = prefab;
		
		cannonBallType = type;
	}
}
