using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum eFXEffectType
{
	ScaleNode,
	RootNode,
	CameraNode,
	Toggle,
	AddUINode,
}

public class FXInfo
{
	public bool firstTime = true;
	public string fxName = "";
	public eFXEffectType effectType;
	public GameObject fxObject;
	
	public float scale = 1.0f;
	public Vector3 origScale = Vector3.one;
	
	public FXInfo()
	{
		effectType = eFXEffectType.ScaleNode;
		fxObject = null;
	}
}

public struct FXDelayInfo
{
	public FXInfo fxInfo;
	public float fEndTime;
}

public class StateController : MonoBehaviour {
	public StateList stateList = null;
	public AnimationController animationController = null;
	public ColliderManager colliderManager = null;
	
	
	public BaseState.eState currentState = BaseState.eState.None;
	public BaseState.eState preState = BaseState.eState.Stand;
	
	[HideInInspector]
	public CharStateInfo curStateInfo = null;
	//public CollisionInfo curCollisionInfo = null;
	
	public delegate void OnChangeState(CharStateInfo info);
	public OnChangeState onChangeState = null;
	
	public delegate void OnEndSate();
	public OnEndSate onEndState = null;
	
	public List<FXInfo> fxList = new List<FXInfo>();
	public List<FXDelayInfo> fxDelayList = new List<FXDelayInfo>();
	
	protected Transform fxScaleNode = null;
	protected Transform fxRootNode = null;
	protected Transform fxShadowNode = null;
	protected Transform fxAddUINode = null;
	
	public string standUpAnim = "stand02";
	public string standDownAnim = "stand03";
	
	public StageManager stageManager = null;
	
	public BaseState.eState beginState = BaseState.eState.Stand;
	
	
	private bool isLocked = false;
	public bool Locked 
	{
		set { isLocked = value; }
		get { return isLocked; }
	}
	
	void Awake () {
		
		Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();
		FindFXNode(transforms);
		
		if (stateList != null)
		{
			ChangeState(beginState);
		}
		
		GameObject obj = GameObject.Find("StageManager");
		if (obj != null)
			stageManager = obj.GetComponent<StageManager>();
	}
	
	public SoundManager soundManager = null;
	void Start()
	{
		if (soundManager == null)
			soundManager = this.gameObject.AddComponent<SoundManager>();
	}
	
	// Update is called once per frame
	void Update () {
		UpdateFXDelayInfo();
	}
	
	public void ChangeState(BaseState.eState newState)
	{
		bool isLockCheck = true;
		switch(newState)
		{
		case BaseState.eState.Knockdownstart:
		case BaseState.eState.Knockdownfall:
		case BaseState.eState.Knockdownland:
		case BaseState.eState.Knockdown_Die:
		case BaseState.eState.Die:
			isLockCheck = false;
			break;
		}
		
		if (isLockCheck == true)
		{
			//죽으면 더이상 상태 변경 안되도록...
			if (Locked == true)
				return;
		}
		
		if (currentState != BaseState.eState.Damage && currentState == newState)
			return;
		
		if (IsContainState(newState) == false)
			return;
		
		//Debug.Log("Pre : " + currentState + " Next : " + newState);
		EndState(newState);
		
		preState = currentState;
		
		currentState = newState;
		BeginState(currentState);
	}
	
	public bool IsContainState(BaseState.eState state)
	{
		CharStateInfo stateInfo = stateList.GetState(state);
		return (stateInfo != null);
	}
	
	public virtual void BeginState(BaseState.eState state)
	{
		CharStateInfo stateInfo = null;
		if (stateList != null)
			stateInfo = stateList.GetState(state);
		
		if (stateInfo == null)
		{
			stateInfo = stateList.GetState(BaseState.eState.Stand);
			currentState = BaseState.eState.Stand;
		}
		
		if (colliderManager != null)
			colliderManager.InitColliderStep();
		
		if (animationController != null)
		{
			animationController.ChangeAnimation(stateInfo.baseState.animationClip);
		}
		
		ChangeStateInfo(stateInfo);
		
		if (stateInfo != null && stateInfo.stateInfo.fxObjectName != "")
			AddFXObject(stateInfo.stateInfo.fxObjectName, stateInfo.stateInfo.effectType, 1.0f);
	}
	
	public void EndState(BaseState.eState state)
	{
		//curCollisionInfo = null;
		if (onEndState != null)
			onEndState();
		
		if (curStateInfo != null && curStateInfo.stateInfo.fxObjectName != "")
			RemoveFXObject(curStateInfo.stateInfo.fxObjectName, curStateInfo.stateInfo.effectType);
		
		if (soundManager != null)
			soundManager.StopSoundEffects(SoundEffect.eSoundType.CancelByState);
	}
	
	
	private void FindFXNode(Transform[] transforms)
	{
		if (transforms == null)
			return;
		
		foreach(Transform trans in transforms)
		{
			if (trans == null)
				continue;
			
			if (trans.name == "FX_scale")
				this.fxScaleNode = trans;
			else if (trans.name == "Root")
				this.fxRootNode = trans;
			else if (trans.name == "FX_Shadow")
				this.fxShadowNode = trans;
			else if (trans.name == "Add_UI")
				this.fxAddUINode = trans;
			
			if (fxScaleNode != null && fxRootNode != null && fxShadowNode != null && fxAddUINode != null)
				break;
		}
	}
	
	/*
	private void FindMeshNode(Transform[] transforms)
	{
		if (transforms == null)
			return;
		
		foreach (Transform trans in transforms)
		{
			if (trans != null && trans.name == "Mesh")
			{
				meshNode = trans.gameObject;
				meshRenderers = meshNode.GetComponentsInChildren<Renderer>();
				break;
			}
		}	
	}
	*/
	protected Dictionary<string, FXInfo> fxObjectList = new Dictionary<string, FXInfo>();
	protected string fxPath = "FXObjects/";
	public FXInfo GetFXObject(string fxName, eFXEffectType effectType)
	{
		if (fxName == "")
			return null;
		
		FXInfo fxInfo = null;
		if (fxObjectList.ContainsKey(fxName) == false)
		{
			GameObject fxObject = ResourceManager.CreatePrefab(fxPath + fxName);
			if (fxObject != null)
			{
				fxInfo = new FXInfo();
				fxInfo.fxObject = fxObject;
				fxInfo.effectType = effectType;
				fxInfo.fxName = fxName;
				fxInfo.firstTime = true;
				
				fxInfo.origScale = fxObject.transform.localScale;
				fxInfo.scale = 1.0f;
				
				fxObjectList.Add(fxName, fxInfo);
			}			
		}
		else
			fxInfo = fxObjectList[fxName];
		
		return fxInfo;
	}
	
	public void AddFXObject(string fxObjectName, eFXEffectType type, float scale)
	{
		FXInfo fxInfo = GetFXObject(fxObjectName, type);
		if (fxInfo != null)
			fxInfo.scale = scale;
		
		AddFXObject(fxInfo);
	}
	
	public void AddFXObject(FXInfo fxInfo)
	{
		if (fxInfo != null)
		{
			FXPlay(fxInfo, true);
			fxList.Add(fxInfo);
		}
	}
	
	public void AddFXDelayInfo(string fxObjectName, eFXEffectType type, float scale, float periodTime)
	{
		FXInfo fxInfo = GetFXObject(fxObjectName, type);
		if (fxInfo != null)
			fxInfo.scale = scale;
		
		AddFXDelayInfo(fxInfo, periodTime);
	}
	
	public void AddFXDelayInfo(FXInfo fxInfo, float periodTime)
	{
		if (fxInfo == null)
			return;
		
		RemoveFXDelayInfo(fxInfo);
		
		FXDelayInfo addInfo = new FXDelayInfo();
		addInfo.fxInfo = fxInfo;
		
		if (periodTime == -1.0f)
			addInfo.fEndTime = -1.0f;
		else
			addInfo.fEndTime = Time.time + periodTime;
		
		FXPlay(fxInfo, true);
		
		fxDelayList.Add(addInfo);
	}
	
	public void RemoveFXObject(string fxObjectName, eFXEffectType type)
	{
		FXInfo fxInfo = GetFXObject(fxObjectName, type);
		RemoveFXObject(fxInfo);
	}
	
	public void RemoveFXObject(FXInfo fxInfo)
	{
		if (fxInfo != null)
		{
			FXPlay(fxInfo, false);
			fxList.Remove(fxInfo);
		}
	}
	
	public void RemoveFXDelayInfo(FXInfo fxInfo)
	{
		if (fxInfo == null)
			return;
		
		//같은 오브젝트가 있으면 애니메이션 정지 하고, 리스트에서 제거
		for (int index = 0; index < fxDelayList.Count; ++index)
		{
			FXDelayInfo delayInfo = fxDelayList[index];
			if (fxInfo == delayInfo.fxInfo)
			{
				FXPlay(delayInfo.fxInfo, false);
				
				fxDelayList.RemoveAt(index);
				break;
			}
		}
	}
	
	public void UpdateFXDelayInfo()
	{
		List<FXDelayInfo> deleteInfos = new List<FXDelayInfo>();
			
		for (int index = 0; index < fxDelayList.Count; ++index)
		{
			FXDelayInfo info = fxDelayList[index];
			if (info.fEndTime == -1)
				continue;
			
			deleteInfos.Add(info);
		}
		
		foreach(FXDelayInfo info in deleteInfos)
		{
			if (info.fEndTime <= Time.time)
			{
				FXPlay(info.fxInfo, false);
				
				fxDelayList.Remove(info);
			}	
		}
	}
	
	protected void FXPlay(FXInfo fxInfo, bool bPlay)
	{
		if (fxInfo == null || fxInfo.fxObject == null)
			return;
		
		Vector3 origPos = Vector3.zero;
		Quaternion origRotate = Quaternion.identity;
		Vector3 origScale = Vector3.one;
		if (fxInfo.firstTime == true)
		{
			origPos = fxInfo.fxObject.gameObject.transform.position;//this.fxRootNode.transform.position;
			origRotate = fxInfo.fxObject.gameObject.transform.rotation;
			origScale = fxInfo.fxObject.gameObject.transform.localScale;
		}
	
		if (fxInfo != null && fxInfo.fxObject != null)
		{
			if (fxInfo.fxObject.audio != null)
				fxInfo.fxObject.audio.mute = !GameOption.effectToggle;
		}
		
		switch(fxInfo.effectType)
		{
		case eFXEffectType.RootNode:
			if (fxInfo.firstTime == true)
			{
				fxInfo.fxObject.gameObject.transform.parent = this.fxRootNode;
			
				fxInfo.fxObject.transform.localPosition = origPos;
				fxInfo.fxObject.transform.localRotation = origRotate;
				fxInfo.fxObject.transform.localScale = origScale;
			}

			break;
		case eFXEffectType.ScaleNode:
			if (fxInfo.firstTime == true)
			{
				fxInfo.fxObject.gameObject.transform.parent = this.fxScaleNode;
			
				fxInfo.fxObject.transform.localPosition = origPos;
				fxInfo.fxObject.transform.localRotation = origRotate;
				fxInfo.fxObject.transform.localScale = origScale;
			}
			break;
		case eFXEffectType.CameraNode:
			if (fxInfo.firstTime == true)
			{
				//fxInfo.fxObject.gameObject.transform.parent = Game.instance.camera.gameObject.transform;
				
				ScrollCamera scrollCamera = null;
				if (stageManager != null && stageManager.StageCamera != null)
				{
					scrollCamera = stageManager.StageCamera;
					if (scrollCamera != null && scrollCamera.uiRoot != null)
						fxInfo.fxObject.gameObject.transform.parent = scrollCamera.uiRoot;
				}
				
				fxInfo.fxObject.transform.localPosition = origPos;
				fxInfo.fxObject.transform.localRotation = origRotate;
				fxInfo.fxObject.transform.localScale = origScale;
			}
			break;
		case eFXEffectType.AddUINode:
			if (fxInfo.firstTime == true)
			{
				fxInfo.fxObject.gameObject.transform.parent = this.fxAddUINode;
			
				fxInfo.fxObject.transform.localPosition = origPos;
				fxInfo.fxObject.transform.localRotation = origRotate;
				fxInfo.fxObject.transform.localScale = origScale;
			}
			break;
		case eFXEffectType.Toggle:
			//fxInfo.fxObject.SetActiveRecursively(bPlay);
			break;
		}
		
		
		if (bPlay == true)
		{
			fxInfo.fxObject.transform.localScale = fxInfo.origScale * fxInfo.scale;	
		}
		else
		{
			fxInfo.fxObject.transform.localScale = fxInfo.origScale;
		}
		
		if (fxInfo.firstTime == true)
			fxInfo.firstTime = false;
		
		FXPlayObject(fxInfo.fxObject, bPlay);
	}
	
	public void FXPlayObject(GameObject fxObject, bool bPlay)
	{
		if (bPlay == true)
			fxObject.SetActive(bPlay);
		
		Transform[] childs = fxObject.GetComponentsInChildren<Transform>();
		if (childs == null || childs.Length == 0)
		{
			Animation rootAnim = fxObject.GetComponent<Animation>();
			if (rootAnim != null)
			{
				if (bPlay == true)
				{
					rootAnim.Play();
					rootAnim.Sample();
				}
				else
				{
					rootAnim.Stop();
				}
				
				foreach(AnimationState state in rootAnim)
					rootAnim[state.name].speed = 1.0f;
			}
			
			ParticleSystem particleSystem = fxObject.GetComponent<ParticleSystem>();
			if (particleSystem != null)
			{
				if (bPlay == true)
					particleSystem.Play();
				else
				{
					particleSystem.Stop();
					particleSystem.Clear();
					if (particleSystem.particleEmitter != null)
						particleSystem.particleEmitter.ClearParticles();
				}
				
				particleSystem.playbackSpeed = 1.0f;
			}
			else
			{
				if (fxObject.particleEmitter != null)
					fxObject.particleEmitter.emit = bPlay;
			}
		}
		else
		{
			for (int childIndex = 0; childIndex < childs.Length; ++childIndex)
			{
				GameObject child = childs[childIndex].gameObject;
				
				if (child != null)
				{
					Animation childAnim = child.GetComponent<Animation>();
					if (childAnim != null)
					{
						//Debug.Log("Animation Name : " + childAnim.name + (bPlay == true ? " Play" : " Stop"));
						
						if (childAnim.renderer != null)
							childAnim.renderer.enabled = bPlay;
						
						if (bPlay == true)
						{
							childAnim.Play();
							childAnim.Sample();
						}
						else
						{
							childAnim.Stop();
						}
						
						foreach(AnimationState state in childAnim)
							childAnim[state.name].speed = 1.0f;
					}
					
					ParticleSystem particleSystem = child.GetComponent<ParticleSystem>();
					if (particleSystem != null)
					{
						if (bPlay == true)
							particleSystem.Play();
						else
						{
							particleSystem.Stop();
							particleSystem.Clear();
							if (particleSystem.particleEmitter != null)
								particleSystem.particleEmitter.ClearParticles();
						}
						
						particleSystem.playbackSpeed = 1.0f;
					}
					else
					{
						if (child.particleEmitter != null)
							child.particleEmitter.emit = bPlay;
					}
				}
			}
		}
		
		if (bPlay == false)
			fxObject.SetActive(false);
	}
	
	public void ChangeStateInfo(CharStateInfo info)
	{
		curStateInfo = info;
		
		if (onChangeState != null)
			onChangeState(curStateInfo);
	}
	
	public bool IsJumpState()
	{
		bool isJump = false;
		
		switch(currentState)
		{
		case BaseState.eState.Down:
		case BaseState.eState.JumpFall:
		case BaseState.eState.JumpStart:
		case BaseState.eState.Knockdownstart:
		case BaseState.eState.Knockdownfall:
			isJump = true;
			break;
		}
		
		return isJump;
	}
	
	public void ChangeAnimationSpeed(float speedRate)
	{
		if (animationController != null)
			animationController.ChnageAnimationSpeed(speedRate);
		
		foreach(FXDelayInfo info in fxDelayList)
		{
			ChangeFXSpeedRate(info.fxInfo, speedRate);
		}
		
		foreach(FXInfo fxInfo in fxList)
		{
			ChangeFXSpeedRate(fxInfo, speedRate);
		}
	}

	protected void ChangeFXSpeedRate(FXInfo fxInfo, float speedRate)
	{
		if (fxInfo == null)
			return;
		
		Transform[] childs = null;
		if (fxInfo.fxObject != null)
			childs = fxInfo.fxObject.GetComponentsInChildren<Transform>();
		
		if (childs == null || childs.Length == 0)
		{
			Animation rootAnim = fxInfo.fxObject.GetComponent<Animation>();
			if (rootAnim != null)
			{
				foreach(AnimationState state in rootAnim)
				{
					rootAnim[state.name].speed = speedRate;	
				}
			}
			
			ParticleSystem particleSystem = fxInfo.fxObject.GetComponent<ParticleSystem>();
			if (particleSystem != null)
				particleSystem.playbackSpeed = speedRate;
			
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
					foreach(AnimationState state in childAnim)
					{
						childAnim[state.name].speed = speedRate;	
					}
				}
				
				ParticleSystem particleSystem = child.GetComponent<ParticleSystem>();
				if (particleSystem != null)
					particleSystem.playbackSpeed = speedRate;
			}
		}
	}
	
	/*
	void OnGUI()
	{
		Vector3 vPos = this.gameObject.transform.position;
		
		ScrollCamera scrollCamera = stageManager.StageCamera;
		Camera cam = null;
		if (scrollCamera != null)
			cam = scrollCamera.gameObject.GetComponent<Camera>();
		
		if (cam != null)
		{
			Vector3 screenPos = cam.WorldToScreenPoint(vPos);
			
			string msg = this.preState + "-> " + this.currentState.ToString();
			GUI.Label(new Rect(screenPos.x, screenPos.y, 300, 20), msg);
		}
	}
	*/
}
