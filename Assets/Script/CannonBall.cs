using UnityEngine;
using System.Collections;

public class CannonBall : BaseWeapon {
	
	public enum eMode
	{
		DETECT_MODE,
		DAMAGE_MODE,
		DESTROY_MODE,
	};
	protected eMode _mode = eMode.DETECT_MODE;
	
	public Vector3 MoveDir = Vector3.zero;
    public float MoveSpeed = 12.0f;
	
	//충돌후 데미지 영역 지속 시간.
	public float DamageDetectCoolTime = 0.06f;
	private float DamageDetectDelayTime = 0.0f;
	
	//충돌후 데미지 이펙트 지속 시간
	public float DamageEffectCoolTime = 1.0f;
	private float DamageEffectDelayTime = 0.0f;
	
	public float limitMoveLength = -1.0f;
	private float moveLength = 0.0f;
	
	//화염구 이펙트들..
	public GameObject FXBomb = null;	//화염구 폭발될때 표시될 이펙트
	
	public GameObject FXDetect = null; //화염구 발사 될때 표시될 이펙트
	
	
	private string fxAttackNormal = "";
	private float fxNormalScale = 1.0f;
	
	
	private float mStartTime = 0.0f;
    private float mStartHeight = 0.0f;
	
	public float UpSpeed = 4.0f;
    public Vector3 TargetDir = Vector3.zero;
	
	public override void Start()
	{
		
	}
	
	public override void OnDestroy()
	{
		
	}
	
	void LateUpdate ()
	{
		switch(_mode)
		{
		case eMode.DETECT_MODE:
			Vector3 vMove = MoveDir * MoveSpeed * Time.deltaTime;
			
			float fElapsedTime = Time.time - mStartTime;
            float fHeight = (UpSpeed * fElapsedTime) + ((Physics.gravity.y * fElapsedTime * fElapsedTime) / 2.0f);
            Vector3 vJump = Vector3.up * ((mStartHeight + fHeight) - transform.position.y);
			
			
			vMove = vMove + vJump;
			
        	transform.position = transform.position + vMove;
			
			if (limitMoveLength != -1.0f)
			{
				moveLength += vMove.magnitude;				
				if (moveLength >= limitMoveLength)
					SetDamageMode();
			}
			break;
		case eMode.DAMAGE_MODE:
			DamageDetectDelayTime -= Time.deltaTime;
			DamageEffectDelayTime -= Time.deltaTime;
			
			if (DamageDetectDelayTime <= 0.0f)
			{
				//SetupCollider(mDamageCollider, false);
				colliderManager.SetupCollider("Damage Collider", false);
				DamageDetectDelayTime = 0.0f;
			}
			
			if (DamageEffectDelayTime <= 0.0f)
			{
				FXEffectPlay(FXBomb, false);
				_mode = eMode.DESTROY_MODE;
				DamageEffectDelayTime = 0.0f;
			}
			
			break;
		case eMode.DESTROY_MODE:
			break;
		}
	}
	
	public void SetFired()
    {
		mStartTime = Time.time - 0.01f; ;
        mStartHeight = transform.position.y;

        float fDistance = TargetDir.magnitude;//Mathf.Abs (TargetDir.x);
        float fHeight = TargetDir.y;

        float fElapsedTime = 0.0f;
        if (MoveSpeed > 0.0f) fElapsedTime = fDistance / MoveSpeed;

        UpSpeed = (((Mathf.Abs(Physics.gravity.y) * fElapsedTime * fElapsedTime) / 2.0f) + fHeight) / fElapsedTime;
		
		//Debug.Log("Set Fired . DetectCollider Un Activate...");
        //SetupCollider(mDetectCollider, true);
		colliderManager.SetupCollider("Detect Collider", true);
		
		FXEffectPlay(FXDetect, true);
		
		//SetupCollider(mDamageCollider, false);
		colliderManager.SetupCollider("Damage Collider", false);
		FXEffectPlay(FXBomb, false);
		
		//DetectMode인 경우는 무조건 회피 가능
		//damageMode인 경우는 무조건 회피 불가
		_mode = eMode.DETECT_MODE;
		attackInfo.stateInfo.attackType = StateInfo.eAttackType.AT_ENABLEAVOID;
    }

    public override BaseWeapon.eAddResult AddHitObject(LifeManager hitActor)
    {
        if (hitObjects.Contains(hitActor) == true)
            return BaseWeapon.eAddResult.AlreadyAdd;
		/*
		StateController stateController = hitActor.GetComponent<StateController>();
		CollisionInfo hitAttackInfo = null;
		if (stateController != null)
			hitAttackInfo = stateController.curCollisionInfo;
		
		if (hitAttackInfo != null && hitAttackInfo.stateInfo.defenseState == StateInfo.eDefenseState.DS_AVOID &&
			_mode == eMode.DETECT_MODE &&
		    attackInfo.stateInfo.attackType == StateInfo.eAttackType.AT_ENABLEAVOID)
		{
			hitObjects.Add(hitActor);
			return false;
		}
		*/
        hitObjects.Add(hitActor);

        return BaseWeapon.eAddResult.AddOK;
    }
	
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer != LayerMask.NameToLayer("Ground"))
			return;
		
        //Debug.Log("FireBall.... OnTriggerEnter...");
		
		//mStop = true;
		
		switch(_mode)
		{
		case eMode.DETECT_MODE:
			SetDamageMode();
			break;
		case eMode.DAMAGE_MODE:
			
			break;
		case eMode.DESTROY_MODE:
				
			break;
		}
	}
	
	void OnTriggerExit(Collider other)
	{
			
	}
	
	void OnCollisionEnter(Collision collision)
	{
		Debug.Log("FireBall.... CollisionEnter...");
	}

	void SetDamageMode()
	{
		//Debug.Log("DetectCollider Un Activate...");
        colliderManager.SetupCollider("Detect Collider", false);
		FXEffectPlay(FXDetect, false);
		
		
		this.fxAttackFxFileInfo = this.fxAttackNormal;
		this.fxAttackFxScaleInfo = this.fxNormalScale;
		
		//Debug.Log("DamageCollider Activate...");
        colliderManager.SetupCollider("Damage Collider", true);
		FXEffectPlay(FXBomb, true);
		
		//이펙트 지속 시간.
		DamageEffectDelayTime = DamageEffectCoolTime;
		
		//데미지 영역 지속 시간은 짧게 유지..
		DamageDetectDelayTime = DamageDetectCoolTime;
		
		_mode = eMode.DAMAGE_MODE;
		
		attackInfo.stateInfo.attackType = StateInfo.eAttackType.AT_DISABLEAVOID;
		
		DestroyObject(gameObject, 0.95f);	
	}
	
	public void SetAttackFxInfo(string fxInfo, float fxScale)
	{
		fxAttackNormal = fxInfo;
		fxNormalScale = fxScale;
	}
}
