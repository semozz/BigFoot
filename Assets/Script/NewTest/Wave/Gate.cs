using UnityEngine;
using System.Collections;

public class Gate : BaseMonster {

	public string catapultNodeName = "Bip001 Prop1";
	private Transform catapultNode = null;
	public GameObject catapultPrefab = null;
	
	protected CatapultBall catapultBall = null; 
	
	protected Vector3 targetPos = Vector3.zero;
	protected bool bTargeting = false;
	
	public string faceSprite = "";
	
	public override void Start () {
		base.Start();
		
		Transform[] bones = GetComponentsInChildren<Transform>();
        foreach (Transform bone in bones)
        {
            if (bone.name == catapultNodeName)
			{
				catapultNode = bone;
				break;
			}
        }
		
		WaveModeUI waveModeUI = GameUI.Instance.waveModeUI;
		if (waveModeUI != null && waveModeUI.gateHP != null)
		{
			waveModeUI.gateHP.SetFace(faceSprite);
			
			lifeManager.attributeManager.hpUI = waveModeUI.gateHP.hp;
			lifeManager.attributeManager.hpInfoLabel = waveModeUI.gateHPInfoLabel;
			
			lifeManager.attributeManager.UpdateHPUI();
			
			lifeManager.onHPValueChange = new UISlider.OnValueChange(OnHPValueChange);
		}
	}
	
	public override void ChangeTarget(ActorInfo newTarget)
	{
		if (newTarget != null && newTarget.actorType == ActorInfo.ActorType.Catapult)
			return;
		
		base.ChangeTarget(newTarget);
	}
	
	public override void FireProjectile()
	{
		if (bTargeting == false)
			return;
		
		if (attackTargetInfo == null) return;
		
        if (catapultBall == null) return;
		
		catapultBall.gameObject.SetActive(true);
		
		catapultBall.transform.parent = null;
		Vector3 firePos = catapultNode.position;
		firePos.z = 0.0f;
		catapultBall.transform.position = firePos;
		catapultBall.transform.localScale = Vector3.one;
		
		Vector3 vCreatePos = catapultBall.transform.position;
		//Vector3 targetActorPos = attackTargetInfo.transform.position;
		
		Vector3 vMoveDir = targetPos - vCreatePos;
		catapultBall.TargetDir = vMoveDir;
		
		vMoveDir.Normalize();
		
        catapultBall.MoveDir = vMoveDir;
		
		catapultBall.SetOwnerActor(lifeManager);
		catapultBall.SetAttackInfo(lifeManager.GetCurrentAttackInfo());
        catapultBall.SetFired();
		
		catapultBall.mFireBallMode = FireBall.eFireBall_Mode.DETECT_MODE;
		
		/*
		if (this.moveController != null)
			this.moveController.SetProjectileCollider(catapultBall.detectCollider, targetInfo);
		*/
		
		catapultBall = null;
	}
	
	public override void DoArrowEquip()
	{
		if (catapultNode == null) return;
        
        GameObject go = (GameObject)Instantiate(catapultPrefab);
        if (go == null) return;
        catapultBall = go.GetComponent<CatapultBall>();
        if (catapultBall == null) return;
		
		catapultBall.mFireBallMode = FireBall.eFireBall_Mode.WAIT_MODE;
		
        go.transform.parent = catapultNode;
        go.transform.localPosition = Vector3.zero;
		go.transform.localScale = Vector3.one;
		
		/*
        go.transform.localRotation = Quaternion.Euler(0.0f, -90.0f, 0.0f);
        if (moveController.moveDir == Vector3.right)
            go.transform.localRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
		*/
		
		if (attackTargetInfo != null)
		{
			targetPos = attackTargetInfo.transform.position;
			bTargeting = true;
		}
		else
		{
			targetPos = Vector3.zero;
			bTargeting = false;
		}
		
		if (catapultBall != null)
			catapultBall.gameObject.SetActive(false);
	}
	
	public float slowTimeRate = 0.2f;
	public float slowDelayTime = 1.0f;
	public override void OnDie(LifeManager attacker)
	{
		base.OnDie(attacker);
		
		if (catapultBall != null)
		{
			DestroyObject(catapultBall.gameObject);
			catapultBall = null;
		}
		
		StartSlow();
		
		StageManager stageManager = Game.Instance.stageManager;
		if (stageManager != null)
		{
			StageEndEvent stageEndEvent = stageManager.stageEndEvent;
			if (stageEndEvent != null)
				stageEndEvent.Invoke("OnStageFailed", slowDelayTime);
		}
	}
	
	public void StartSlow()
	{
		Time.timeScale = slowTimeRate;
		Invoke("ResetSlow", slowDelayTime);
	}
	
	public void ResetSlow()
	{
		Time.timeScale = 1.0f;
	}
	
	public void OnHPValueChange (float val)
	{
		WaveModeUI waveModeUI = GameUI.Instance.waveModeUI;
		if (waveModeUI != null && waveModeUI.gateHP != null && val > 0.0f)
		{
			waveModeUI.gateHP.OnWarning();
		}
	}
	
	public override void Update()
	{
		if (Game.Instance.Pause == true)
			return;
		
		base.Update();
		
		if (this.stateController.animationController.isAnimationPlaying == false)
		{			
			switch(this.stateController.currentState)
			{
			case BaseState.eState.Stand:
			case BaseState.eState.Die:
			case BaseState.eState.Knockdown_Die:
				break;
			default:
				this.stateController.ChangeState(BaseState.eState.Stand);
				break;
			}
		}
	}
}
