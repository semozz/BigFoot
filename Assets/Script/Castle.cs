using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Castle : BaseMonster {
	public CannonLauncher launcher = null;
	
	public enum eCasstleGrade
	{
		Grade0,
		Grade1,
		Grade2
	}
	public eCasstleGrade grade = eCasstleGrade.Grade0;
	
	public enum eCannonBallType
	{
		Noraml,
		Ice,
		Fire,
		Poison,
	};
	public eCannonBallType cannonBallType = eCannonBallType.Noraml;
	
	public GameObject casstleBodyNode = null;
	[HideInInspector]
	public GameObject casstleBody = null;
	public List<GameObject> bodyList = new List<GameObject>();
	
	public int upgrade = 0;
	public int maxUpgradeLimit = 5;
	
	public BaseState.eState normalAttack = BaseState.eState.Attack1;
	public BaseState.eState fireAttack = BaseState.eState.Attack2;
	public BaseState.eState iceAttack = BaseState.eState.Attack21;
	public BaseState.eState poisonAttack = BaseState.eState.Attack3;
	
	// Use this for initialization
	public override void Start () {
		base.Start();
	
	}
	
	void Awake()
	{
		ChangeCasttleBody(grade);
		ChangeCannonBallType(cannonBallType);
		
		ActorManager actorManager = ActorManager.Instance;
		myInfo = GetComponent<ActorInfo>();
		if (myInfo != null)
		{
			if (actorManager != null)
				actorManager.AddActor(myInfo.myTeam, myInfo);
		}
	}
	
	public override void Update()
	{
		base.Update();
	}
	
	public void ChangeCasttleBody(eCasstleGrade _grade)
	{
		int index = (int)_grade;
		GameObject prefab = null;
		
		int nCount = bodyList.Count;
		if (index >= 0 && index < nCount)
			prefab = bodyList[index];
		
		if (casstleBody != null)
			DestroyObject(casstleBody);
		
		casstleBody = (GameObject)Instantiate(prefab);
		casstleBody.transform.parent = casstleBodyNode.transform;
		casstleBody.transform.localPosition = Vector3.zero;
		
		UpdateComponentInfo(casstleBody);
		
		grade = _grade;
	}
	
	public void UpdateComponentInfo(GameObject obj)
	{
		this.animEventTrigger = obj.GetComponent<AnimationEventTrigger>();
		
		
		if (this.stateController != null)
		{
			this.stateController.stateList = obj.GetComponent<StateList>();
		}
		
		AnimationController animController = this.GetComponent<AnimationController>();
		if (animController != null)
			animController.anim = obj.GetComponent<Animation>();
	}
	
	public void ChangeCannonBallType(Castle.eCannonBallType type)
	{
		if (launcher != null)
			launcher.ChangeCannonBallType(type);
	}
	
	public override void FireProjectile()
	{
		//Debug.Log("Castle Fire.....");
		
		OnCollisionStart();
		
		if (launcher != null)
			launcher.DoLaunchProjectile(targetInfo);
		
		OnCollisionStop();
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
				
				if (CanAttackState(info) == false)
					continue;
				
				if (info.IsAvailableAttack(randValue, diffX, diffY, bSameGround) == false)
					continue;
				
				BaseState.eState attackState = info.attackState;
				switch(cannonBallType)
				{
				case eCannonBallType.Noraml:
					attackState = normalAttack;
					break;
				case eCannonBallType.Fire:
					attackState = fireAttack;
					break;
				case eCannonBallType.Ice:
					attackState = iceAttack;
					break;
				case eCannonBallType.Poison:
					attackState = poisonAttack;
					break;
				}
				
				if (attackState != info.attackState)
					continue;
				
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
}
