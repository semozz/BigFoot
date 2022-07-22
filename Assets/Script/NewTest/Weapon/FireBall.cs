using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireBall : BaseWeapon 
{
    public Vector3 MoveDir = Vector3.zero;
    public float MoveSpeed = 12.0f;
	
	public float lifeTime = 2.0f;
	public float exploreTime = 0.0f;
	
    //public MoveController.eLookDir LookDir = MoveController.eLookDir.LD_LEFT;
    public Vector3 lookDir = Vector3.left;
    
	//화염구 이펙트들..
	public GameObject FXBomb = null;	//화염구 폭발될때 표시될 이펙트
	
	public GameObject FXDetect = null; //화염구 발사 될때 표시될 이펙트
	
	//충돌후 데미지 영역 지속 시간.
	public float DamageDetectCoolTime = 0.06f;
	private float DamageDetectDelayTime = 0.0f;
	
	//충돌후 데미지 이펙트 지속 시간
	public float DamageEffectCoolTime = 1.0f;
	private float DamageEffectDelayTime = 0.0f;
	
	public float limitMoveLength = -1.0f;
	private float moveLength = 0.0f;
	
	public LifeManager targetObject = null;
	
	private string fxAttackNormal = "";
	private float fxNormalScale = 1.0f;
	
	public string colliderLayerName1 = "PlayerBody";
	public string colliderLayerName2 = "JumpPlayerBody";
	
	public enum eFireBall_Mode
	{
		WAIT_MODE,
		DETECT_MODE,
		DAMAGE_MODE,
		DESTROY_MODE,
	};
	public eFireBall_Mode mFireBallMode = eFireBall_Mode.DETECT_MODE;
	
	public Collider detectCollider = null;
	
	public GameObject dropIndicatorEffect = null;
	[HideInInspector]
	public Vector3 dropPos = Vector3.zero;
	
	public bool bShowDropIndicator = false;
	
    void Awake()
    {
		ToggleFXEffect(FXDetect, true);
		ToggleFXEffect(FXBomb, false);
    }
	
	public virtual void Move(float deltaTime, float currentTime)
	{
		Vector3 vMove = MoveDir * MoveSpeed * Time.deltaTime;
    	transform.position = transform.position + vMove;
		
		if (limitMoveLength != -1.0f)
		{
			moveLength += vMove.magnitude;				
			if (moveLength >= limitMoveLength)
				SetDamageMode();
		}
	}
	
	void LateUpdate ()
	{
		switch(mFireBallMode)
		{
		case eFireBall_Mode.DETECT_MODE:
			Move(Time.deltaTime, Time.time);
			break;
		case eFireBall_Mode.DAMAGE_MODE:
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
				ToggleFXEffect(FXBomb, false);
				mFireBallMode = eFireBall_Mode.DESTROY_MODE;
				DamageEffectDelayTime = 0.0f;
			}
			
			break;
		case eFireBall_Mode.DESTROY_MODE:
			break;
		}
		
		if (dropIndicatorEffect != null && dropIndicatorEffect.activeInHierarchy == true)
			dropIndicatorEffect.transform.position = dropPos;
		
		if (mFireBallMode == eFireBall_Mode.DETECT_MODE &&
			exploreTime <= Time.time)
			SetDamageMode();
	}
	
	
	public string defaultSound = "";
    public virtual void SetFired()
    {
		colliderManager.SetupCollider("Detect Collider", true);
		
		ToggleFXEffect(FXDetect, true);
		
		colliderManager.SetupCollider("Damage Collider", false);
		ToggleFXEffect(FXBomb, false);
		
		//DetectMode인 경우는 무조건 회피 가능
		//damageMode인 경우는 무조건 회피 불가
		mFireBallMode = eFireBall_Mode.DETECT_MODE;
		attackInfo.stateInfo.attackType = StateInfo.eAttackType.AT_ENABLEAVOID;
		
		exploreTime = Time.time + lifeTime;
		
		if (dropIndicatorEffect != null)
		{
			dropIndicatorEffect.SetActive(bShowDropIndicator);
			
			dropIndicatorEffect.transform.position = dropPos;
		}
		
		if (GameOption.effectToggle == true)
		{
			float effectVolume = Game.Instance.effectSoundScale;
			AudioManager.PlaySound(audioSource, defaultSound, effectVolume);
		}
    }

    public override BaseWeapon.eAddResult AddHitObject(LifeManager hitActor)
    {
        if (hitObjects.Contains(hitActor) == true)
            return BaseWeapon.eAddResult.AlreadyAdd;
		
		AttackStateInfo hitAttackInfo = hitActor.GetCurrentAttackInfo();
		if (hitAttackInfo != null && hitAttackInfo.stateInfo.defenseState == StateInfo.eDefenseState.DS_AVOID &&
			mFireBallMode == eFireBall_Mode.DETECT_MODE &&
		    attackInfo.stateInfo.attackType == StateInfo.eAttackType.AT_ENABLEAVOID)
		{
			hitObjects.Add(hitActor);
			return BaseWeapon.eAddResult.Evade;
		}
		
		hitObjects.Add(hitActor);

        return BaseWeapon.eAddResult.AddOK;
    }
	
	public virtual void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer(colliderLayerName1) ||
			other.gameObject.layer == LayerMask.NameToLayer(colliderLayerName2))
		{
			switch(mFireBallMode)
			{
			case eFireBall_Mode.DETECT_MODE:
				SetDamageMode();
				break;
			}
		}
		else
			return;
	}
	
	void OnTriggerExit(Collider other)
	{
			
	}
	
	void OnCollisionEnter(Collision collision)
	{
		Debug.Log("FireBall.... CollisionEnter...");
	}
	
	public string explosionSound = "";
	public float destroyDelayTime = 0.95f;
	void SetDamageMode()
	{
		if (audioSource != null)
			audioSource.Stop();
		
		//Debug.Log("DetectCollider Un Activate...");
        colliderManager.SetupCollider("Detect Collider", false);
		ToggleFXEffect(FXDetect, false);
		
		this.fxAttackFxFileInfo = this.fxAttackNormal;
		this.fxAttackFxScaleInfo = this.fxNormalScale;
		
		//Debug.Log("DamageCollider Activate...");
        colliderManager.SetupCollider("Damage Collider", true);
		ToggleFXEffect(FXBomb, true);
		
		//이펙트 지속 시간.
		DamageEffectDelayTime = DamageEffectCoolTime;
		
		//데미지 영역 지속 시간은 짧게 유지..
		DamageDetectDelayTime = DamageDetectCoolTime;
		
		mFireBallMode = eFireBall_Mode.DAMAGE_MODE;
		
		attackInfo.stateInfo.attackType = StateInfo.eAttackType.AT_DISABLEAVOID;
		
		DestroyObject(gameObject, destroyDelayTime);
		
		//float effectVolume = Game.Instance.effectSoundScale;
		//AudioManager.PlaySound(audioSource, explosionSound, effectVolume);
		AddSoundEffect(explosionSound, null, SoundEffect.eSoundType.DontCare);
	}

    void SetupCollider(Collider c, bool bActive)
    {
        if (c == null) return;

        c.isTrigger = bActive;
        if (c.GetType() == typeof(MeshCollider))
            ((MeshCollider)c).convex = bActive;
    }
	
	void ToggleFXEffect(GameObject fxObject, bool isActivate)
	{
		if (fxObject != null)
		{
			fxObject.SetActive(isActivate);
		
			ParticleRenderer[] renderers = fxObject.GetComponentsInChildren<ParticleRenderer>();
	        foreach (ParticleRenderer r in renderers)
	            r.gameObject.SetActive(isActivate);
			
			
			Transform[] childs = fxObject.GetComponentsInChildren<Transform>();
			if (childs == null || childs.Length == 0)
			{
				Animation rootAnim = fxObject.GetComponent<Animation>();
				if (rootAnim != null)
				{
					if (isActivate == true)
						rootAnim.Play();
					else
						rootAnim.Stop();
				}
				
				return;
			}
			
			for (int childIndex = 0; childIndex < childs.Length; ++childIndex)
			{
				GameObject child = childs[childIndex].gameObject;
				
				if (child != null)
				{
					Animation childAnim = child.GetComponent<Animation>();
					if (childAnim != null)
					{
						//Debug.Log("Animation Name : " + childAnim.name + (bPlay == true ? " Play" : " Stop"));
						
						if (isActivate == true)
							childAnim.Play();
						else
							childAnim.Stop();
					}
				}
			}
		}
	}
	
	public void SetAttackFxInfo(string fxInfo, float fxScale)
	{
		fxAttackNormal = fxInfo;
		fxNormalScale = fxScale;
	}
	
	public override Vector3 GetMoveDir()
	{
		return this.MoveDir;
	}
}
