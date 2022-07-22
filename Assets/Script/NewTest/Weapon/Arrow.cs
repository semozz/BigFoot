using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Arrow : BaseWeapon 
{
    public enum eArrowMovingType { AMT_LINE, AMT_PARABOLA, AMT_POWERSHOT };

    public eArrowMovingType MovingType = eArrowMovingType.AMT_LINE;

    public float MoveLengthMax = 10.0f;
    public float LastMoveDeltaTime = 0.05f;
    public float MoveSpeed = 12.0f;
    public float UpSpeed = 4.0f;
    public Vector3 MoveDir = Vector3.zero;
    public Vector3 TargetDir = Vector3.zero;

    public float LifeTime = 10.0f;

    //public MoveController.eLookDir LookDir = MoveController.eLookDir.LD_LEFT;
    public Vector3 lookDir = Vector3.left;
	
    public GameObject FXTrailShot = null;
	public GameObject FXPowerShot = null;

    private float mStartTime = 0.0f;
    private float mStartHeight = 0.0f;
    private bool mFired = false;

    private bool mMoveDone = false;
    private float mMoveLength = 0.0f;

    private bool mStop = false;
	
	//private bool IsFallState = false;
	private Vector3 vPrePosition = Vector3.zero;
	
	public string fxHitGround = "";
	public Transform fxHitGroundNode = null;
	
	public float destroyDelayTime = 1.0f;
	
	private string fxAttackEffect = "";
	private float fxAttackScale = 1.0f;
	
	public Collider detectCollider = null;
	public float hitPosModifyValue = 0.25f;
	
	public Collider targetGroundCollider = null;
	
	public override void Start () 
	{
		base.Start();
		
		FXEffectPlay(FXTrailShot, false);
		FXEffectPlay(FXPowerShot, false);
		
		if (LifeTime > 0.0f)
		{
			lifeDelayTime = LifeTime;
			isLifeTime = true;
		}
		else
			isLifeTime = false;
	}
	
	void LateUpdate ()
	{
        if (mStop == true || mFired == false) return;

        Move(Time.deltaTime, Time.time);
	}
	
	float lifeDelayTime = 0.0f;
	bool isLifeTime = false;
	void Update()
	{
		if (isLifeTime == true)
		{
			if (lifeDelayTime <= 0.0f)
			{
				FXEffectPlay(FXTrailShot, false);
				colliderManager.SetupCollider("Weapon Collider", false);
				DestroyObject(gameObject, 0.0f);
			}
			
			lifeDelayTime -= Time.deltaTime;
		}
	}

    void Move(float deltaTime, float currentTime)
    {
        Vector3 vMove = MoveDir * MoveSpeed * deltaTime;
        Vector3 vJump = Vector3.zero;

        mMoveLength = mMoveLength + Mathf.Abs(vMove.x);
       
		if (MovingType == eArrowMovingType.AMT_PARABOLA)
        {
            float fElapsedTime = currentTime - mStartTime;
            float fHeight = (UpSpeed * fElapsedTime) + ((Physics.gravity.y * fElapsedTime * fElapsedTime) * 0.5f);
            vJump = Vector3.up * ((mStartHeight + fHeight) - transform.position.y);
        }
        else if (MovingType == eArrowMovingType.AMT_LINE)
        {
            if (LifeTime == -1.0f &&
				(MoveDir == Vector3.right || MoveDir == Vector3.left) &&
				mMoveDone == false && mMoveLength >= MoveLengthMax * 0.6f)
            {
                mStartTime = currentTime;
                mStartHeight = transform.position.y;
                mMoveDone = true;
            }

            if (mMoveDone == true)
            {
                float fElapsedTime = currentTime - mStartTime;
				float fHeight = ((Physics.gravity.y * fElapsedTime * fElapsedTime) * 0.5f);
                vJump = Vector3.up * ((mStartHeight + fHeight) - transform.position.y);
            }
        }

        vMove = vMove + vJump;
		
		vPrePosition = transform.position;
		
        transform.position = transform.position + vMove;
		
		if (vMove != Vector3.zero)
			transform.rotation = Quaternion.LookRotation(-vMove);
    }

	/*
    public override bool AddHitObject(LifeManager hitActor)
    {
		bool bResult = base.AddHitObject(hitActor);
        if (bResult == true)
		{
			if (isPiercing == false)
			{
				mStop = true;
				FXEffectPlay(FXTrailShot, false);
				gameObject.transform.parent = hitActor.transform.root;
				//SetWeaponCollider(false);
				collideManager.SetupCollider("Weapon Collider", false);
				DestroyObject(gameObject, 0.0f);
			}
		}

        return bResult;
    }
	*/
	
	public override void SetDestroy(LifeManager hitActor)
	{
		if (isPiercing == false)
		{
			mStop = true;
			FXEffectPlay(FXTrailShot, false);
			gameObject.transform.parent = hitActor.transform.root;
			//SetWeaponCollider(false);
			colliderManager.SetupCollider("Weapon Collider", false);
			DestroyObject(gameObject, 0.0f);
		}
	}
	
    public void SetFired()
    {
		bool isPowerShoot = false;
		
        mFired = true;

        mStartTime = Time.time - 0.01f; ;
        mStartHeight = transform.position.y;

        float fDistance = Mathf.Abs (TargetDir.x);
        float fHeight = TargetDir.y;

        float fElapsedTime = 0.0f;
        if (MoveSpeed > 0.0f) fElapsedTime = fDistance / MoveSpeed;

        UpSpeed = (((Mathf.Abs(Physics.gravity.y) * fElapsedTime * fElapsedTime) / 2.0f) + fHeight) / fElapsedTime;
		
		SetAttackFxInfo();
		
        //SetWeaponCollider(true);
		colliderManager.SetupCollider("Weapon Collider", true);
        FXEffectPlay(FXTrailShot, true);
		
		isPowerShoot = false;
		
		if (MovingType == eArrowMovingType.AMT_POWERSHOT)
			isPowerShoot = true;
		
		//Debug.Log("PowerShot .. " + isPowerShoot);
		FXEffectPlay(FXPowerShot, isPowerShoot);
    }

	public void OnTriggerEnter(Collider other)
    {
		if (other.gameObject.name.Contains("LeftWall") == true ||
			other.gameObject.name.Contains("RightWall") == true)
		{
			DestroyObject(this.gameObject, 1.0f);
			return;
		}
		
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerBody"))
		{
			/*
			LifeController controller = other.gameObject.transform.root.GetComponent<LifeController>();
			if (controller != null)
			{
				if (controller.IsAvoidState() == true && attackType == LifeController.eAttackType.AT_ENABLEAVOID)
					return;
			}
			*/
			return;	
		}
		else if (other.gameObject.layer == LayerMask.NameToLayer("MonsterBody"))
		{
			/*
			LifeController controller = param.other.gameObject.transform.root.GetComponent<LifeController>();
			if (controller != null)
			{
				if (controller.IsAvoidState() == true && attackType == LifeController.eAttackType.AT_ENABLEAVOID)
					return;
			}
			*/
			return;	
		}
		
		bool isGroundHit = false;
		Vector3 vGroundHitPos = gameObject.transform.position;
		
		//Ground/Floor에 충돌시 상승중인 발사체는 계속 진행..
		if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
		{
			var groundLayer = 1 << LayerMask.NameToLayer("Ground");
			
			RaycastHit hit;
			Vector3 vStartPos = gameObject.transform.position;
			Vector3 vDir = gameObject.transform.position - vPrePosition;
			vStartPos = vStartPos - (vDir * 10.0f);
			
			vDir.Normalize();
			
			if (Physics.Raycast(vPrePosition, vDir, out hit, 2.0f, groundLayer) == true)
			{
				if (hit.collider.name.Contains("ground") == true)
				{
					if (hit.normal == Vector3.down)
						return;
				}
				else if (hit.collider.name.Contains("floor") == true)
				{
					if (hit.normal != Vector3.up)
						return;
					else
					{
						if (hit.collider != targetGroundCollider)
							return;
					}
				}
				else if (hit.collider.name.Contains("wall") == true)
				{
					if (hit.normal == Vector3.up)
						return;
				}
				
				Debug.Log(hit.collider.name + "   Hit....");
					
				Debug.DrawLine(vPrePosition, hit.point, Color.red);
				
				isGroundHit = true;
				vGroundHitPos = hit.point - (vDir * hitPosModifyValue) ;
			}
			else
				return;
		}
		
		
		mStop = true;
        //SetWeaponCollider (false);
		colliderManager.SetupCollider("Weapon Collider", false);
		
		ResetAttackFxInfo();
        
        FXEffectPlay(FXTrailShot, false);
		
        if (isGroundHit)
		{
			gameObject.transform.parent = other.transform;
			gameObject.transform.position = vGroundHitPos;
			//Move(LastMoveDeltaTime, Time.time + LastMoveDeltaTime);
			
			if (fxHitGround != "")
				SetHitGroundEffect(fxHitGround);
			
			DestroyObject (gameObject, destroyDelayTime);
		}
		else
		{
			DestroyObject (gameObject, 0.0f);
		}
    }

    void SetWeaponCollider(bool bActive)
    {
        Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders)
        {
            c.isTrigger = bActive;
            if (c.GetType() == typeof(MeshCollider))
                ((MeshCollider)c).convex = bActive;
        }
    }
	
	public void ResetAttackFxInfo()
	{
		this.fxAttackFxFileInfo = "";
		this.fxAttackFxScaleInfo = 1.0f;	
	}
	
	public void SetAttackFxInfo()
	{
		this.fxAttackFxFileInfo = this.fxAttackEffect;
		this.fxAttackFxScaleInfo = this.fxAttackScale;
	}
	
	
	private void SetHitGroundEffect(string fxName)
	{
		if (fxHitGroundNode == null)
			return;
		
		//GameObject go = (GameObject)Instantiate(Resources.Load(fxName));
		GameObject go = ResourceManager.CreatePrefab(fxName);
		if (go != null)
		{
			go.transform.parent = this.fxHitGroundNode;
			go.transform.localPosition = Vector3.zero;
			
			FXEffectPlay(go, true);
		}
	}
	
	public void SetAttackEffectInfo(string fxEffectInfo, float fxScale)
	{
		fxAttackEffect = fxEffectInfo;
		fxAttackScale = fxScale;
	}
	
	public override Vector3 GetMoveDir()
	{
		return this.MoveDir;
	}
}
