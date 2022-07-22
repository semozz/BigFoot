using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (Rigidbody))]

public class BaseMoveController : MonoBehaviour {
	public struct sGroundInfo
	{
		public Collider ground;
		public float groundHeight;
		public float groundDistance;
	}
	
	public enum eWeightType
	{
		Heavy,
		Normal,
		Slight
	}
	public eWeightType weightType = eWeightType.Normal;
	
	public float prevMoveSpeed = 0.0f;
	public Vector3 moveDir = Vector3.right;
	public float moveSpeed = 0.0f;
	private float curMoveSpeed = 0.0f;
	
	public float defaultMoveSpeed = 5.0f;
	public float dashMoveSpeed = 10.0f;
	
	public Vector3 jumpPower = Vector3.up;
	public float jumpPowerFactor = 400.0f;
	
	private StateController stateController = null;
	
	public GameObject root = null;
	public GameObject shadowObj = null;
	public Transform shadowNode = null;
	
	public LayerMask layerMask = 0;
	public LayerMask enemyLayerMask = 0;
	
	[HideInInspector]
	public int defaultLayer = 0;
	[HideInInspector]
	public int jumpLayer = 0;
	[HideInInspector]
	public int hideBodyLayer = 0;
	
	[HideInInspector]
	public string hideBodyLayerName = "HideBody";
	
	public string defaultLayerName = "Body";
	public string jumpLayerName = "JumpBody";
	
	public float skinWidth = 0.03f;
	public float colliderRadius = 0.5f;
	public float colliderHeight = 1.0f;
	
	public delegate void OnCollision();
	public OnCollision onCollision = null;
	
	
	private LifeManager lifeManger = null;
	
	private float speedRate = 1.0f;
	public float SpeedRate
	{
		get { return speedRate; }
		set { speedRate = value; }
	}
	
	public bool isMovable = true;
	
	// Use this for initialization
	void Start () {
		
		if (collider != null)
		{
			if (collider.GetType() == typeof(CapsuleCollider))
			{
				CapsuleCollider capsule = (CapsuleCollider)collider;
				colliderRadius = capsule.radius;
				colliderHeight = capsule.height;
			}
			else if (collider.GetType() == typeof(BoxCollider))
			{
				BoxCollider boxCollider = (BoxCollider)collider;
				
				Vector3 vSize = boxCollider.size;
				vSize.x = vSize.z;
				boxCollider.size = vSize;
				
				colliderRadius = vSize.z * 0.5f;
				colliderHeight = vSize.y;
			}
		}
	}
	
	public StageManager stageManager = null;
	void Awake()
	{
		defaultLayer = LayerMask.NameToLayer(defaultLayerName);
		jumpLayer = LayerMask.NameToLayer(jumpLayerName);
		hideBodyLayer = LayerMask.NameToLayer(hideBodyLayerName);
		
		stateController = GetComponent<StateController>();
		lifeManger = GetComponent<LifeManager>();
		
		if (shadowObj == null)
		{
			//shadowObj = (GameObject)Instantiate(Resources.Load("NewAsset/Shadow/Square_Shadow"));
			shadowObj = ResourceManager.CreatePrefab("NewAsset/Others/Square_Shadow");
			
			if (shadowObj != null && shadowNode != null)
				shadowObj.transform.parent = shadowNode;
		}
		
		GameObject obj = GameObject.Find("StageManager");
		if (obj != null)
			stageManager = obj.GetComponent<StageManager>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public bool ignoreMonsterBody = false;
	public bool CheckCollision()
	{
		Bounds myBounds = collider.bounds;
		
		Vector3 vDir = moveDir;
		if (moveSpeed < 0.0f)
		{
			vDir *= -1.0f;
		}
		
		Vector3 vDelta = Vector3.up * (colliderHeight * 0.45f);
		
		Vector3 vBoundCenter = myBounds.center;
		Vector3 vBoundBottom = vBoundCenter - vDelta;
		Vector3 vBoundTop = vBoundCenter + vDelta;
		
		Color debugColor1 = Color.blue;
		Color debugColor2 = Color.blue;
		Color debugColor3 = Color.blue;
		
		float radius = colliderRadius + skinWidth;
		
		if (moveSpeed == 0.0f)
			return false;
		
		RaycastHit hitInfo;
		bool bCollision = false;
		
		bool ignoreJump = false;
		switch(stateController.currentState)
		{
		case BaseState.eState.JumpStart:
		case BaseState.eState.JumpFall:
		case BaseState.eState.JumpAttack:
		case BaseState.eState.Knockdownstart:
		case BaseState.eState.Knockdownfall:
			ignoreJump = true;
			break;
		}
		
		int tempLayerMask = layerMask;
		if (ignoreJump == true || ignoreMonsterBody == true)
			tempLayerMask ^= enemyLayerMask;
		
		if (Physics.Raycast(vBoundCenter, vDir, out hitInfo, radius * 1.2f, tempLayerMask) == true)
		{
			if (ignoreJump == true && hitInfo.collider.name.Contains("floor") == true)
			{
				Physics.IgnoreCollision(collider, hitInfo.collider, true);
			}
			else
			{
				BaseMoveController targetMove = hitInfo.collider.GetComponent<BaseMoveController>();
				if (targetMove == null || targetMove.ignoreMonsterBody == false)
				{
					debugColor1 = Color.red;
					bCollision = true;
				}
			}
		}
		else if (Physics.Raycast(vBoundBottom, vDir, out hitInfo, radius * 1.2f, tempLayerMask) == true)
		{
			if (ignoreJump == true && hitInfo.collider.name.Contains("floor") == true)
			{
				Physics.IgnoreCollision(collider, hitInfo.collider, true);
			}
			else
			{
				BaseMoveController targetMove = hitInfo.collider.GetComponent<BaseMoveController>();
				if (targetMove == null || targetMove.ignoreMonsterBody == false)
				{
					debugColor2 = Color.red;
					bCollision = true;
				}
			}
		}
		else if (Physics.Raycast(vBoundTop, vDir, out hitInfo, radius * 1.2f, tempLayerMask) == true)
		{
			if (ignoreJump == true && hitInfo.collider.name.Contains("floor") == true)
			{
				Physics.IgnoreCollision(collider, hitInfo.collider, true);
			}
			else
			{
				BaseMoveController targetMove = hitInfo.collider.GetComponent<BaseMoveController>();
				if (targetMove == null || targetMove.ignoreMonsterBody == false)
				{
					debugColor3 = Color.red;
					bCollision = true;
				}
			}
		}
		
		if (bCollision == true)
		{
			float diff = hitInfo.distance - radius;
			if (diff < 0.0f || Mathf.Abs(diff) <= skinWidth)
			{
				float delta = Mathf.Abs(diff);
				this.transform.position -= (vDir * delta);
				
				if (onCollision != null)
					onCollision();
			}
		}
		
		Debug.DrawLine(vBoundCenter, vBoundCenter + (vDir * colliderRadius), debugColor1);
		Debug.DrawLine(vBoundBottom, vBoundBottom + (vDir * colliderRadius), debugColor2);
		Debug.DrawLine(vBoundTop, vBoundTop + (vDir * colliderRadius), debugColor3);
		
		return bCollision;
	}
	
	void FixedUpdate()
	{
		if (Game.Instance.Pause == true)
			return;
		
		if (stateController == null)
			return;
		
		BaseState.eState curStat = stateController.currentState;
		if (curStat == BaseState.eState.Die ||
			curStat == BaseState.eState.Knockdown_Die)
			return;
		
		curMoveSpeed = moveSpeed;
		
		
		if (CheckCollision() == true)
			curMoveSpeed = 0.0f;
		
		CheckGroundPos(false);
		
		if (rigidbody == null)
			return;
		
		if (curMoveSpeed == 0)
		{
			Vector3 velocity = rigidbody.velocity;
			
			velocity.x = velocity.z = 0.0f;
			rigidbody.velocity = velocity;
		}
		else
		{
			//curMoveSpeed = moveSpeed;//Mathf.Lerp(curMoveSpeed, moveSpeed, 1.0f * Time.deltaTime);
			
			Vector3 limitVelocity = moveDir * curMoveSpeed * speedRate;
			limitVelocity.y = rigidbody.velocity.y;
			Vector3 deltaVelocity = limitVelocity - rigidbody.velocity;
			
			rigidbody.AddForce(deltaVelocity, ForceMode.VelocityChange);
		}
	}
	
	public void UpdateShadow()
	{
		Bounds myBounds = collider.bounds;
		
		RaycastHit hitInfo;
		
		Vector3 vShadowPos = this.transform.position;
		float shadowScale = 1.0f;
		float halfHeight = colliderHeight * 0.5f;
		float fullHeight = colliderHeight;
		
		Vector3 checkPos = myBounds.center;
		Vector3 delta = Vector3.right * this.colliderRadius;
		Vector3 leftPos = checkPos - delta;
		Vector3 rightPos = checkPos + delta;
		
		float checkDistance = float.MaxValue;
		
		int shadowLayr = layerMask ^ enemyLayerMask;
		if (Physics.Raycast(checkPos, Vector3.down, out hitInfo, float.MaxValue, shadowLayr) == true)
		{
			if (checkDistance > hitInfo.distance)
			{
				checkDistance = hitInfo.distance;
				vShadowPos = hitInfo.point;
			}
		}
		
		if (Physics.Raycast(leftPos, Vector3.down, out hitInfo, float.MaxValue, shadowLayr) == true)
		{
			if (checkDistance > hitInfo.distance)
			{
				checkDistance = hitInfo.distance;
				vShadowPos = hitInfo.point;
			}
		}
		
		if (Physics.Raycast(rightPos, Vector3.down, out hitInfo, float.MaxValue, shadowLayr) == true)
		{
			if (checkDistance > hitInfo.distance)
			{
				checkDistance = hitInfo.distance;
				vShadowPos = hitInfo.point;
			}
		}
		
		if (checkDistance > halfHeight)
		{
			float diffScale = fullHeight / checkDistance;
			shadowScale = Mathf.Max(0.3f, Mathf.Min(diffScale, 1.0f));
		}
		
		Vector3 scaleVec = Vector3.one;
		if (shadowObj != null)
		{
			scaleVec = shadowObj.transform.localScale;
			scaleVec.x = scaleVec.y = scaleVec.z = shadowScale;
			shadowObj.transform.localScale = scaleVec;
			
			checkPos.y = vShadowPos.y;
			shadowObj.transform.position = checkPos;
			shadowObj.transform.localRotation = Quaternion.identity;
		}
	}
	
	private Vector3 preVelocity = Vector3.zero;
	void LateUpdate()
	{
		Vector3 velocity = rigidbody.velocity;
		
		CheckGroundPos(false);
		
		if (stateController != null)
		{
			BaseState.eState curStat = stateController.currentState;
			
			/*CapsuleCollider capsule = (CapsuleCollider)collider;
			RaycastHit hitInfo;
			*/	
			
			sGroundInfo info;
			switch(curStat)
			{
			case BaseState.eState.Die:
			case BaseState.eState.Drop:
			case BaseState.eState.Blowattack:
			//case BaseState.eState.Knockdownfall:
			case BaseState.eState.Knockdownland:
			case BaseState.eState.Knockdown_Die:
				break;
			case BaseState.eState.JumpAttack:
			case BaseState.eState.JumpFall:
				
				if (velocity.y < 0.01f)
				{
					info = new sGroundInfo();
						
					CheckGroundAndHeight(out info);
					if (info.groundDistance == 0.0f)
					{
						CheckGroundPos(true);
						
						stateController.ChangeState(BaseState.eState.Jumpland);
						groundCollider = info.ground;
						ChangeDefaultLayer(false);
						
						ApplyFallDamage(this.transform.position);
					}
				}
				break;
			case BaseState.eState.JumpStart:
				if (/*preVelocity.y >= 0.0f && */velocity.y < -0.02f)
				{
					SetJumpCollider(false, true);
					
					this.jumpStartPosition = this.transform.position;
					
					stateController.ChangeState(BaseState.eState.JumpFall);
					if (gameObject.layer == defaultLayer)
						ChangeJumpLayer();
				}
				break;
			case BaseState.eState.Knockdownstart:
				if (/*preVelocity.y >= 0.0f && */velocity.y < -0.02f)
				{
					SetJumpCollider(false, false);
					
					this.jumpStartPosition = this.transform.position;
					
					stateController.ChangeState(BaseState.eState.Knockdownfall);
				}
				else
				{
					//int a = 0;
				}
				break;
			case BaseState.eState.Knockdownfall:
				info = new sGroundInfo();
				CheckGroundAndHeight(out info);
				if (velocity.y < 0.01f || info.groundDistance == 0.0f)
				{
					
					if (info.groundDistance == 0.0f)
					{
						CheckGroundPos(true);
						
						groundCollider = info.ground;
						
						if (lifeManger != null && lifeManger.GetHP() <= 0.0f)
						{
							stateController.ChangeState(BaseState.eState.Knockdown_Die);
							ChangeHideLayer();
						}
						else
						{
							stateController.ChangeState(BaseState.eState.Knockdownland);
							ChangeDefaultLayer(false);
							
							ApplyFallDamage(this.transform.position);
						}
						
						rigidbody.velocity = Vector3.zero;
					}
				}
				break;
			default:
				if (curStat != BaseState.eState.JumpFall && 
					curStat != BaseState.eState.Jumpland &&
					velocity.y < 0.0f)
				{
					info = new sGroundInfo();
					
					CheckGroundAndHeight(out info);
					if (info.groundDistance > 0.0f)
					{
						SetJumpCollider(false, false);
						
						this.jumpStartPosition = this.transform.position;
						
						stateController.ChangeState(BaseState.eState.JumpFall);
						if (gameObject.layer == defaultLayer)
							ChangeJumpLayer();
					}
				}
				break;
			}
		}
		
		preVelocity = velocity;
		
		UpdateLimitArea();
		
		UpdateShadow();
	}
	
	public void CheckGroundAndHeight(out sGroundInfo groundInfo)
	{
		groundInfo.groundDistance = -1.0f;
		groundInfo.groundHeight = 0.0f;
		groundInfo.ground = null;
		
		RaycastHit hitInfo;
		
		Bounds myBounds = collider.bounds;
		float fRadius = colliderRadius;
		Vector3 delta = Vector3.right * fRadius;
		
		Vector3 centerPos = myBounds.center;
		Vector3 leftPos = centerPos - delta;
		Vector3 rightPos = centerPos + delta;
		
		bool bCheckGround = false;
		
		float checkDistance = float.MaxValue;
		int checkLayer = layerMask ^ enemyLayerMask;
		if (Physics.Raycast(centerPos, Vector3.down, out hitInfo, checkDistance, checkLayer) == true)
		{
			bCheckGround = true;
			checkDistance = hitInfo.distance;
			
			groundInfo.groundHeight = hitInfo.point.y;
			groundInfo.ground = hitInfo.collider;
		}
		
		if (Physics.Raycast(leftPos, Vector3.down, out hitInfo, checkDistance, checkLayer) == true)
		{
			bCheckGround = true;
			checkDistance = hitInfo.distance;
			
			groundInfo.groundHeight = hitInfo.point.y;
			groundInfo.ground = hitInfo.collider;
		}
		
		if (Physics.Raycast(rightPos, Vector3.down, out hitInfo, checkDistance, checkLayer) == true)
		{
			bCheckGround = true;
			checkDistance = hitInfo.distance;
			
			groundInfo.groundHeight = hitInfo.point.y;
			groundInfo.ground = hitInfo.collider;
		}
		
		if (bCheckGround == true && checkDistance != float.MaxValue)
		{
			//if (checkDistance > (colliderHeight * 0.5f))
			checkDistance -= colliderHeight * 0.5f;
			
			if (checkDistance < 0.1f)
				checkDistance = 0.0f;
			
			groundInfo.groundDistance = checkDistance;
		}
		else
		{
			
		}
	}
	
	public void ChangeJumpLayer()
	{
		this.gameObject.layer = jumpLayer;
	}
	
	public void ChangeHideLayer()
	{
		this.gameObject.layer = hideBodyLayer;
	}
	
	public Collider groundCollider = null;
	public void ChangeDefaultLayer(bool bCheck)
	{
		this.gameObject.layer = defaultLayer;
		
		if (bCheck == true)
		{
			sGroundInfo groundInfo = new sGroundInfo();
			CheckGroundAndHeight(out groundInfo);
			if (groundInfo.ground != null)
				groundCollider = groundInfo.ground;
		}
		
		SetJumpCollider(false, false);
	}
	
	public void SetJumpCollider(bool bIgnoreGround, bool bIgnoreBody)
	{
		Collider myCol = collider;
		stageManager.SetJumpCollider(myCol, bIgnoreGround);
		
		ActorManager actorMgr = ActorManager.Instance;
		if (actorMgr != null)
			actorMgr.SetJumpCollider(this.lifeManger.myActorInfo, myCol, bIgnoreBody);
	}
	
	public void SetProjectileCollider(Collider projectileCollider, ActorInfo targetInfo)
	{
		Collider targetGroundCollider = null;
		if (targetInfo != null)
			targetGroundCollider = targetInfo.GetGroundCollider();
		
		if (stageManager != null)
			stageManager.SetProjectileCollider(projectileCollider, this.groundCollider, targetGroundCollider);
	}
	
	public void TestPathFind()
	{
		List<GameObject> groundList = new List<GameObject>();
		
		if (stageManager != null)
			stageManager.FindGrounds(groundList);
		
		int nCount = groundList.Count;
		int randIndex = Random.Range(0, nCount);
		
		Path path = new Path();
		
		if (nCount > 0)
		{
			GameObject targetObj = groundList[randIndex];
			
			WayPointManager targetWay = targetObj.GetComponent<WayPointManager>();
			
			WayPointManager curWay = groundCollider.gameObject.GetComponent<WayPointManager>();
			
			if (curWay != null && targetWay != null)
			{
				//Debug.Log("Cur Ground ... : " + groundCollider.name);
				//Debug.Log("Selected Target ... : " + targetObj.name);
				
				curWay.FindPath(targetWay, path, 2);
			}
		}
		
		
		if (path.findPath == true)
			path.DebugInfo();
		else
			Debug.Log("find path failed.....");
	}
	
	protected float jumpTime = 0.0f;
	public void DoJump()
	{
		if (isMovable == false)
			return;
		
		float curTime = Time.time;
		if (curTime - jumpTime < 0.5)
			return;
		
		rigidbody.AddForce(jumpPower * jumpPowerFactor, ForceMode.Force);
		
		Debug.Log("DoJump : " + curTime);
		
		jumpTime = curTime;
		ChangeJumpLayer();
		
		SetJumpCollider(true, true);
	}
	
	public void DoFall()
	{
		moveSpeed = 0.0f;
		
		Vector3 velocity = rigidbody.velocity;
		//if (velocity.y == 0.0f)
		velocity.x = velocity.z = 0.0f;
		
		rigidbody.AddForce(jumpPower * jumpPowerFactor * -2.0f, ForceMode.Force);
		
		ChangeJumpLayer();
		
		SetJumpCollider(false, false);
	}
	
	private float knockDownStartTime = 0.0f;
	public void DoKnockDown(Vector3 knockPower)
	{
		if (isMovable == false)
			return;
		
		if (Time.time - knockDownStartTime < 0.1f)
			return;
		
		knockDownStartTime = Time.time;
		
		rigidbody.velocity = Vector3.zero;
		preVelocity = Vector3.zero;
		
		Debug.Log("KnockDown.... " + Time.time);
		rigidbody.AddForce(knockPower, ForceMode.Force);
		
		ChangeJumpLayer();
		
		SetJumpCollider(true, true);
	}
	
	public void ChangeMoveDir(Vector3 dir)
	{
		if (isMovable == false)
			return;
		/*
		if (dir != Vector3.left && dir != Vector3.right)
			return;
		*/
		
		dir.Normalize();
		if (dir.x > 0.8f)
		{
			dir.x = 1.0f;
			dir.y = 0.0f;
			dir.z = 0.0f;
		}
		else if (dir.x < - 0.8f)
		{
			dir.x = -1.0f;
			dir.y = 0.0f;
			dir.z = 0.0f;
		}
		else
		{
			dir.x = 1.0f;
			dir.y = 0.0f;
			dir.z = 0.0f;
		}
		
		if (moveDir == dir)
			return;
		
		moveDir = dir;
		
		Vector3 vLookAt = Vector3.back;
        if (dir == Vector3.left)
            vLookAt = Vector3.left;
        else if (dir == Vector3.right)
            vLookAt = Vector3.right;

        root.transform.rotation = Quaternion.LookRotation(vLookAt);

        
		Vector3 vScale = root.transform.localScale;
        if (dir == Vector3.right && vScale.x < 0.0f)
        {
            vScale.x *= -1.0f;
            root.transform.localScale = vScale;
        }
        else if (dir == Vector3.left && vScale.x > 0.0f)
        {
            vScale.x *= -1.0f;
            root.transform.localScale = vScale;
        }
	}
	
	void OnCollisionEnter(Collision collision) 
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Trigger"))
			return;
		
		//Debug.Log("Collision Enter....." + Time.time);
		
		Vector3 normal = collision.contacts[0].normal;
		
		Rigidbody otherRigidBody = collision.rigidbody;
		if (otherRigidBody != null)
		{
			if (normal.y >= 0.8f)
			{
				Vector3 addValue = normal + moveDir + Physics.gravity;
				
				rigidbody.AddForce(addValue, ForceMode.Force);
			}
		}
		
		
		if (stateController != null)
		{
			BaseState.eState curStat = stateController.currentState;
			
			switch(curStat)
			{
			case BaseState.eState.JumpStart:
			case BaseState.eState.JumpAttack:
			case BaseState.eState.JumpFall:
				if (normal.y >= 0.8f)
				{
					CheckGroundPos(true);
					
					stateController.ChangeState(BaseState.eState.Jumpland);
					
					groundCollider = collision.collider;
					ChangeDefaultLayer(false);
				}
				
				//this.transform.position += normal * skinWidth;
				break;
			case BaseState.eState.Knockdownstart:
				if (normal.y >= 0.8f ||
					//collision.relativeVelocity.y < 0.0f)
					this.rigidbody.velocity.y < 0.0f)
				{
					CheckGroundPos(true);
					stateController.ChangeState(BaseState.eState.Knockdownfall);
				}
				else
				{
					//int a = 0;
					Debug.LogError("OnCollisionEnter.. " + curStat);
				}
				break;
			case BaseState.eState.Knockdownfall:
				if (normal.y >= 0.8f)
				{
					CheckGroundPos(true);
					
					groundCollider = collision.collider;
					
					if (lifeManger != null && lifeManger.GetHP() <= 0.0f)
					{
						stateController.ChangeState(BaseState.eState.Knockdown_Die);
						ChangeHideLayer();
					}
					else
					{
						stateController.ChangeState(BaseState.eState.Knockdownland);
						ChangeDefaultLayer(false);
					}
				}
				break;
			case BaseState.eState.Drop:
				if (normal.y >= 0.8f)
				{
					CheckGroundPos(true);
					
					stateController.ChangeState(BaseState.eState.Blowattack);
					//moveSpeed = 0.0f;
					groundCollider = collision.collider;
					ChangeDefaultLayer(false);
				}
				break;
			default:
				//this.transform.position += normal * skinWidth;
				break;
			}
		}
		
		if (normal.y == 0.0f && isMovable == true)
		{
			this.transform.position += normal * (colliderRadius + skinWidth);
		}
    }
	
	private void CheckGroundPos(bool bResetVelocity)
	{
		sGroundInfo groundInfo = new sGroundInfo();
		CheckGroundAndHeight(out groundInfo);
		
		if (groundInfo.groundDistance == 0.0f)
		{
			//this.transform.position = hitInfo.point;
			groundCollider = groundInfo.ground;
			
			if (bResetVelocity == true)
			{
				rigidbody.velocity = Vector3.zero;
				rigidbody.angularVelocity = Vector3.zero;
			}
		}
	}
	
	public void ModifyGroundPos()
	{
		sGroundInfo groundInfo = new sGroundInfo();
		CheckGroundAndHeight(out groundInfo);
		
		Vector3 myPos = this.transform.position;
		if (myPos.y < groundInfo.groundHeight)
		{
			myPos.y = groundInfo.groundHeight;
			
			this.groundCollider = groundInfo.ground;
			this.transform.position = myPos;
		}
	}
	
	public bool CheckObstacle(out Vector3 pos, float limitLength)
	{
		Bounds myBounds = collider.bounds;
		
		bool bCheckObstacle = false;
		Vector3 hitPos = Vector3.zero;
		
		Vector3 vCenter = myBounds.center;

		//var mask = 1 << LayerMask.NameToLayer("Obstacle");
	
		RaycastHit hitInfo;
		if (Physics.Raycast(vCenter, moveDir, out hitInfo, limitLength, layerMask) == true)
		{
			int layerValue = hitInfo.collider.gameObject.layer;
			int tempLayerMask = 1 << layerValue;
			
			string hitObjName = hitInfo.collider.gameObject.name;
			if (hitObjName.Contains("Gate") || hitObjName.Contains("LeftWall") || hitObjName.Contains("RightWall"))
			{
				bCheckObstacle = false;
			}
			else
			{
				if (layerValue == LayerMask.NameToLayer("Obstacle") ||
					((this.enemyLayerMask.value & tempLayerMask) == tempLayerMask))
				{
					bCheckObstacle = true;
					hitPos = hitInfo.point;
				}
			}
		}
		pos = hitPos;
		
		return bCheckObstacle;
	}
	
	public bool CheckObstacleMonster(out Vector3 pos, GameObject targetObject, float limitLength)
	{
		Bounds myBounds = collider.bounds;
		
		bool bCheckObstacle = false;
		Vector3 hitPos = Vector3.zero;
		
		Vector3 vCenter = myBounds.center;

		//var mask = 1 << LayerMask.NameToLayer("Obstacle");
	
		RaycastHit hitInfo;
		if (Physics.Raycast(vCenter, moveDir, out hitInfo, limitLength, layerMask) == true)
		{
			int layerValue = hitInfo.collider.gameObject.layer;
			int tempLayerMask = 1 << layerValue;
			
			string hitObjName = hitInfo.collider.gameObject.name;
			if (hitObjName.Contains("Gate") || hitObjName.Contains("LeftWall") || hitObjName.Contains("RightWall"))
			{
				bCheckObstacle = false;
			}
			else
			{
				if (layerValue == LayerMask.NameToLayer("MonsterBody") &&
					targetObject != hitInfo.collider.gameObject)
				{
					bCheckObstacle = true;
					hitPos = hitInfo.point;
				}
			}
		}
		pos = hitPos;
		
		return bCheckObstacle;
	}
	
	public float CheckDistance(Vector3 vDir, float limitLeght)
	{
		float distance = -1.0f;
		
		Vector3 vCenter = collider.bounds.center;

		RaycastHit hitInfo;
		if (Physics.Raycast(vCenter, vDir, out hitInfo, limitLeght, this.layerMask) == true)
		{
			distance = hitInfo.distance;
		}
		
		return distance;
	}
	
	public float fallDamageMinDistance = 10.0f;
	public float fallDamageMaxDistance = 20.0f;
	public float fallDamageRate = 0.5f;
	public Vector3 jumpStartPosition = Vector3.zero;
	public void ApplyFallDamage(Vector3 curPosition)
	{
		Vector3 vDiff = jumpStartPosition - curPosition;
		float diffY = Mathf.Abs(vDiff.y);
		
		float healthMax = this.lifeManger.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.HealthMax);
		float damageRate = 0.0f;
		if (diffY >= fallDamageMinDistance)
		{
			float fallDistGap = fallDamageMaxDistance - fallDamageMinDistance;
			float rate = (diffY - fallDamageMinDistance) / fallDistGap;
			damageRate = Mathf.Clamp01(rate);
		}
		
		damageRate = fallDamageRate * damageRate;
		float fallDamage = healthMax * damageRate;
		if (fallDamage > 0.0f)
		{
			this.lifeManger.DecHP(fallDamage, null, false, GameDef.eBuffType.BT_NONE);
		}
	}
	
	public void UpdateLimitArea()
	{
		if (stageManager != null)
		{
			Vector3 curPos = this.transform.position;
			
			bool bChecked = false;
			
			if (curPos.x < stageManager.stageAreaMinX + this.colliderRadius)
			{
				bChecked = true;
				curPos.x = stageManager.stageAreaMinX + (this.colliderRadius * 2.2f);
			}
			else if (curPos.x > (stageManager.stageAreaMaxX - this.colliderRadius))
			{
				bChecked = true;
				curPos.x = stageManager.stageAreaMaxX - (this.colliderRadius * 2.2f);
			}
			
			if (curPos.y < stageManager.stageAreaMinY)
			{
				bChecked = true;
				
				Vector3 hitPos = curPos;
				if (GetTargetPos(curPos, this.colliderRadius, Vector3.up, out hitPos) == true)
				{
					curPos = hitPos;
					
					if (GetTargetPos(curPos, this.colliderRadius, Vector3.down, out hitPos) == true)
					{
						curPos = hitPos + Vector3.up;
					}
					else
					{
						curPos += Vector3.up;
					}
				}
			}
			
			if (bChecked == true)
			{
				rigidbody.velocity = Vector3.zero;
				rigidbody.angularVelocity = Vector3.zero;
				
				this.transform.position = curPos;
			}
		}
	}
	
	public bool GetTargetPos(Vector3 vPos, float radius, Vector3 vDir, out Vector3 hitPos)
	{
		Vector3 checkPos = vPos + (-vDir * 1.1f);
		
		Vector3 delta = Vector3.right * radius;
		Vector3 leftPos = checkPos - delta;
		Vector3 rightPos = checkPos + delta;
		float checkDistance = float.MaxValue;
		
		var groundLayer = 1 << LayerMask.NameToLayer("Ground");
		
		hitPos = vPos;
		
		bool bChecked = false;
		RaycastHit hit;
		if (Physics.Raycast(checkPos, vDir, out hit, float.MaxValue, groundLayer) == true)
		{
			if (checkDistance > hit.distance)
			{
				bChecked = true;
				
				hitPos = hit.point;
				checkDistance = hit.distance;
			}
		}
		
		if (Physics.Raycast(leftPos, vDir, out hit, float.MaxValue, groundLayer) == true)
		{
			if (checkDistance > hit.distance)
			{
				bChecked = true;
				
				hitPos = hit.point;
				checkDistance = hit.distance;
			}
		}
		if (Physics.Raycast(rightPos, vDir, out hit, float.MaxValue, groundLayer) == true)
		{
			if (checkDistance > hit.distance)
			{
				bChecked = true;
				
				hitPos = hit.point;
				checkDistance = hit.distance;
			}
		}
		
		return bChecked;
	}
}
