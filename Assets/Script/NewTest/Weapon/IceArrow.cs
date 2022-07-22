using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IceArrow : BaseWeapon {
	
	public enum eIceArrowType
	{
		NormalArrow,
		SuperArrow
	}
	
	public eIceArrowType IceArrowType = eIceArrowType.NormalArrow;
	
	public float MoveLengthMax = 4.0f;
    public float LastMoveDeltaTime = 0.01f;
    public float MoveSpeed = 12.0f;
    public Vector3 MoveDir = Vector3.zero;
    
    public float LifeTime = 10.0f;

    private float mMoveLength = 0.0f;

	//충돌후 데미지 영역 지속 시간.
	public float DamageDetectCoolTime = 0.06f;
	private float DamageDetectDelayTime = 0.0f;
	
	public string targetLayerName = "MonsterBody";
	
	private string fxAttackNormal = "";
	private string fxAttackSuper = "";
	private float fxNormalScale = 1.0f;
	private float fxSuperScale = 1.25f;
	
	public enum eIceBall_Mode
	{
		DETECT_MODE,
		DAMAGE_MODE,
		DESTROY_MODE,
	};
	
	protected eIceBall_Mode mIceBallMode = eIceBall_Mode.DETECT_MODE;
	
	public GameObject FXDetectB = null;
	public GameObject FXDetectN = null;
	public GameObject FXBombB = null;
	public GameObject FXBombN = null;
	
	public string defaultSound = "";
    public string explosion1Sound = "";
     public string explosion2Sound = "";
	
	public void InitData()
	{
		FXEffectPlay(FXDetectB, false);
		FXEffectPlay(FXDetectN, false);
		FXEffectPlay(FXBombB, false);
		FXEffectPlay(FXBombN, false);
	}
	
	void LateUpdate ()
	{
		switch(mIceBallMode)
		{
		case eIceBall_Mode.DETECT_MODE:
			Move(Time.deltaTime, Time.time);
			break;
		case eIceBall_Mode.DAMAGE_MODE:
			DamageDetectDelayTime -= Time.deltaTime;
				
			if (DamageDetectDelayTime <= 0.0f)
			{
				colliderManager.SetupCollider("Bomb_Collider_Normal", false);
				colliderManager.SetupCollider("Bomb_Collider_Super", false);
				
				mIceBallMode = eIceBall_Mode.DESTROY_MODE;
			}
			break;
		case eIceBall_Mode.DESTROY_MODE:
			break;
		}
	}

    void Move(float deltaTime, float currentTime)
    {
        Vector3 vMove = MoveDir * MoveSpeed * deltaTime;
        
		mMoveLength = mMoveLength + Mathf.Abs(vMove.x);
		if (mMoveLength >= MoveLengthMax)
		{
			//범위 벗어 날때 회피불가 설정..
			//이전에 add된 액터들은 더 이상 피해를 입지 않도록...
			List<LifeManager> oldList = new List<LifeManager>();
			oldList.AddRange(this.hitObjects);
			
			SetDestroy(false);
			
			foreach(LifeManager actor in oldList)
				hitObjects.Add(actor);	
			
			return;
		}
		
		//vPrePosition = transform.position;
		
        transform.position = transform.position + vMove;

        vMove.x = vMove.x * -1.0f;
        vMove.y = vMove.y * -1.0f;
		if (vMove != Vector3.zero)
       		transform.rotation = Quaternion.LookRotation(vMove);
    }

    public override BaseWeapon.eAddResult AddHitObject(LifeManager actor)
    {
		if (this.hitObjects.Contains(actor) == true)
            return BaseWeapon.eAddResult.AlreadyAdd;
		
		if (actor != null)
		{
			if (actor.stateController.curStateInfo.stateInfo.defenseState == StateInfo.eDefenseState.DS_AVOID && 
			    mIceBallMode == eIceBall_Mode.DETECT_MODE &&
			    this.attackInfo.stateInfo.attackType == StateInfo.eAttackType.AT_ENABLEAVOID)
			{
				hitObjects.Add(actor);
				return BaseWeapon.eAddResult.Evade;
			}
		}
		
        hitObjects.Add(actor);

		return BaseWeapon.eAddResult.AddOK;
    }
	
	public void SetFired()
    {
		if (MoveDir.x < 0.0f)
		{
			Vector3 scale = gameObject.transform.localScale;
			scale.x *= -1.0f;
			gameObject.transform.localScale = scale;
		}
		
		colliderManager.SetupCollider("Detect_Collider", true);
		colliderManager.SetupCollider("Bomb_Collider_Normal", false);
		colliderManager.SetupCollider("Bomb_Collider_Super", false);

		switch(IceArrowType)
		{
		case eIceArrowType.NormalArrow:
			ToggleFXEffect(FXDetectB, false);
			ToggleFXEffect(FXDetectN, true);
			
			ToggleFXEffect(FXBombB, false);
			ToggleFXEffect(FXBombN, false);
			break;
		case eIceArrowType.SuperArrow:
			ToggleFXEffect(FXDetectB, true);
			ToggleFXEffect(FXDetectN, false);
			
			ToggleFXEffect(FXBombB, false);
			ToggleFXEffect(FXBombN, false);
			break;
		}
		
		if (GameOption.effectToggle == true)
		{
			float effectVolume = Game.Instance.effectSoundScale;
			AudioManager.PlaySound(audioSource, defaultSound, effectVolume);
		}
    }

    public void OnTriggerEnter(Collider other)
    {
		if (other.gameObject.name.Contains("LeftWall") == true ||
			other.gameObject.name.Contains("RightWall") == true)
		{
			DestroyObject(this.gameObject, 1.0f);
			return;
		}
		
		if (other.gameObject == this.attackInfo.ownerActor.gameObject)
			return;
		
        if (other.gameObject.layer == LayerMask.NameToLayer(targetLayerName))
		{
			//몬스터에 맞은 경우 삭제는 몬스터 OnDamage함수에서 회피/블럭을 하지 않은 경우 SetDestroy함수 호출로 삭제 한다.
			/*
			LifeController controller = param.other.gameObject.transform.root.GetComponent<LifeController>();
			if (controller != null)
			{
				if (controller.IsAvoidState() == true && 
				    mFireBallMode == eFireBall_Mode.DETECT_MODE &&
				    AttackType == LifeController.eAttackType.AT_ENABLEAVOID)
					return;
			}
			*/
			return;
		}
		
		//bool isGroundHit = false;
		//Vector3 vGroundHitPos = Vector3.zero;
		
		//Ground/Floor에 충돌시 상승중인 발사체는 계속 진행..
		if (other.gameObject.layer == LayerMask.NameToLayer("Ground") ||
		    other.gameObject.layer == LayerMask.NameToLayer("Floor"))
		{
			//Debug.Log("Ground, Floor Collision....");
		}

        switch(mIceBallMode)
		{
		case eIceBall_Mode.DETECT_MODE:
			SetDestroy(true);
			break;
		case eIceBall_Mode.DAMAGE_MODE:
			
			break;
		case eIceBall_Mode.DESTROY_MODE:
				
			break;
		}
    }
	
	public override void SetDestroy(LifeManager actor)
	{
		SetDestroy(true);
	}
	
	public void SetDestroy(bool bBomb)
	{
		//hitObjects.Clear();
		
		this.colliderManager.SetupCollider("Detect_Collider", false);
		
		if (audioSource != null)
			audioSource.Stop();
		
		float effectVolume = Game.Instance.effectSoundScale;
		switch(IceArrowType)
		{
		case eIceArrowType.NormalArrow:
			this.fxAttackFxFileInfo = this.fxAttackNormal;
			this.fxAttackFxScaleInfo = this.fxNormalScale;
			
			colliderManager.SetupCollider("Bomb_Collider_Normal", true);
			colliderManager.SetupCollider("Bomb_Collider_Super", false);
			
			ToggleFXEffect(FXDetectB, false);
			ToggleFXEffect(FXDetectN, false);
				
			if (bBomb == true)
			{
				ToggleFXEffect(FXBombB, false);
				ToggleFXEffect(FXBombN, true);
			
				//AudioManager.PlaySound(audioSource, explosion1Sound, effectVolume);
				AddSoundEffect(explosion1Sound, null, SoundEffect.eSoundType.DontCare);
			}
			
			break;
		case eIceArrowType.SuperArrow:
			this.fxAttackFxFileInfo = this.fxAttackSuper;
			this.fxAttackFxScaleInfo = this.fxSuperScale;
			
			colliderManager.SetupCollider("Bomb_Collider_Normal", false);
			colliderManager.SetupCollider("Bomb_Collider_Super", true);
			
			ToggleFXEffect(FXDetectB, false);
			ToggleFXEffect(FXDetectN, false);
		
			if (bBomb == true)
			{	
				ToggleFXEffect(FXBombB, true);
				ToggleFXEffect(FXBombN, false);
			
				//AudioManager.PlaySound(audioSource, explosion2Sound, effectVolume);
				AddSoundEffect(explosion2Sound, null, SoundEffect.eSoundType.DontCare);
			}
			
			break;
		}
		
		//데미지 영역 지속 시간은 짧게 유지..
		DamageDetectDelayTime = DamageDetectCoolTime;
		
		mIceBallMode = eIceBall_Mode.DAMAGE_MODE;
		this.attackInfo.stateInfo.attackType = StateInfo.eAttackType.AT_DISABLEAVOID;
		
		//이펙트 지속 시간은 발사체 자체는 유지되어야 한다.
		DestroyObject(gameObject, 0.7f);
	}
	
	void ToggleFXEffect(GameObject fxObject, bool isActivate)
	{
		if (fxObject != null)
		{
			fxObject.SetActive(isActivate);
		
			ParticleRenderer[] renderers = fxObject.GetComponentsInChildren<ParticleRenderer>();
	        foreach (ParticleRenderer r in renderers)
	            r.gameObject.SetActive(isActivate);
		}
	}
	
	public void SetAttackEffectInfo(string normalInfo, float normalScale, string superInfo, float superScale)
	{
		this.fxAttackNormal = normalInfo;
		this.fxNormalScale = normalScale;
		
		this.fxAttackSuper = superInfo;
		this.fxSuperScale = superScale;
	}
	
	public bool IsDetectMode()
	{
		return mIceBallMode == eIceBall_Mode.DETECT_MODE;
	}
	
	public override Vector3 GetMoveDir()
	{
		return this.MoveDir;
	}
}
