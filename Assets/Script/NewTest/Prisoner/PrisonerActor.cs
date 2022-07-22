using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PrisonerActor : MonoBehaviour {
	public enum ePrisonerState
	{
		Stand,
		Ready,
		Runaway,
	}
	public ePrisonerState prisonerState = ePrisonerState.Stand;
	
	public ActorInfo myInfo = null;
	
	public StateController stateController = null;
	public AnimationEventTrigger animEventTrigger = null;
	
	public BaseMoveController moveController = null;
	
	protected GameObject MeshNode = null;
	protected Renderer[] meshRenderers = null;
	
	public LayerMask layerMaskValue = 0;
	
	public LifeManager lifeManager = null;
	
	void Awake()
	{
		sound = gameObject.GetComponent<AudioSource>();
		if (sound == null)
			sound = gameObject.AddComponent<AudioSource>();
		
		lifeManager = gameObject.GetComponent<LifeManager>();
		
		//InitAttributeData();
	}
	
	public void Start () {
		if (stateController != null)
		{
			stateController.onChangeState = new StateController.OnChangeState(OnChangeState);
			stateController.onEndState = new StateController.OnEndSate(OnEndState);
		}
		
		if (animEventTrigger != null)
		{
			animEventTrigger.onAnimationBegin = new AnimationEventTrigger.OnAnimationEvent(OnAnimationBegin);
			animEventTrigger.onAnimationEnd = new AnimationEventTrigger.OnAnimationEvent(OnAnimationEnd);
			animEventTrigger.onCollisionStart = new AnimationEventTrigger.OnAnimationEvent(OnCollisionStart);
			animEventTrigger.onCollisionStop = new AnimationEventTrigger.OnAnimationEvent(OnCollisionStop);
			animEventTrigger.onWalkingStart = new AnimationEventTrigger.OnAnimationEvent(OnWalkingStart);
			animEventTrigger.onWalkingStop = new AnimationEventTrigger.OnAnimationEvent(OnWalkingStop);
			animEventTrigger.onStrongAttackCheck = new AnimationEventTrigger.OnAnimationEvent(OnStrongAttackCheck);
			animEventTrigger.onFire = new AnimationEventTrigger.OnAnimationEvent(OnFire);
			animEventTrigger.onArrowEquip = new AnimationEventTrigger.OnAnimationEvent(OnArrowEquip);
			
			animEventTrigger.onDialogStart = new AnimationEventTrigger.OnAnimationEvent(OnDialogStart);
			
			animEventTrigger.onPlaySoundA = new AnimationEventTrigger.OnAnimationEventByString(OnPlaySoundA);
			animEventTrigger.onPlaySoundB = new AnimationEventTrigger.OnAnimationEventByString(OnPlaySoundB);
			animEventTrigger.onPlaySoundC = new AnimationEventTrigger.OnAnimationEventByString(OnPlaySoundC);
			
			animEventTrigger.onStopSound = new AnimationEventTrigger.OnAnimationEvent(OnStopSound);
		}
		
		if (moveController != null)
			moveController.onCollision = new BaseMoveController.OnCollision(OnCollision);
		
		Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();
		FindMeshNode(transforms);
		
		
		if (stateController != null)
		{
			stateController.ChangeState(BaseState.eState.Stand);
			this.prisonerState = ePrisonerState.Ready;
		}
		
		Vector3 vPos = this.transform.position;
		RaycastHit hitInfo;
		if (Physics.Raycast(vPos, Vector3.down, out hitInfo, float.MaxValue, layerMaskValue) == true)
		{
			this.transform.position = hitInfo.point;
		}
	}
	
	private void FindMeshNode(Transform[] transforms)
	{
		if (transforms == null)
			return;
		
		foreach (Transform trans in transforms)
		{
			if (trans != null && trans.name == "Mesh")
			{
				MeshNode = trans.gameObject;
				meshRenderers = MeshNode.GetComponentsInChildren<Renderer>();
				break;
			}
		}	
	}
	
	public AudioSource sound = null;
	public void OnPlaySoundA(string soundFileName)
	{
		SoundManager soundManager = null;
		if (stateController != null)
			soundManager = stateController.soundManager;
		
		if (soundManager != null && GameOption.effectToggle == true)
			soundManager.AddSoundEffect(soundFileName, SoundEffect.eSoundType.DontCare);
	}
	
	public void OnPlaySoundB(string soundFileName)
	{
		SoundManager soundManager = null;
		if (stateController != null)
			soundManager = stateController.soundManager;
		
		if (soundManager != null && GameOption.effectToggle == true)
			soundManager.AddSoundEffect(soundFileName, SoundEffect.eSoundType.TryKeep);
	}
	
	public void OnPlaySoundC(string soundFileName)
	{
		SoundManager soundManager = null;
		if (stateController != null)
			soundManager = stateController.soundManager;
		
		if (soundManager != null && GameOption.effectToggle == true)
			soundManager.AddSoundEffect(soundFileName, SoundEffect.eSoundType.CancelByState);
	}
	
	public void OnStopSound()
	{
		SoundManager soundManager = null;
		if (stateController != null)
			soundManager = stateController.soundManager;
		
		if (soundManager != null)
			soundManager.StopSoundEffects(SoundEffect.eSoundType.TryKeep);
	}
	
	public void Update()
	{
		if (Game.Instance.Pause == true)
			return;
		
		switch(this.prisonerState)
		{
		case ePrisonerState.Ready:
			break;
		case ePrisonerState.Stand:
			break;
		case ePrisonerState.Runaway:
			UpdateRunAway();
			break;
		}
	}
	
	public void UpdateRunAway()
	{
		
	}
	
	public virtual void OnChangeState(CharStateInfo info)
	{
		if (moveController != null)
		{
			float moveSpeed = 0.0f;
			switch(info.moveType)
			{
			case CharStateInfo.eMoveType.Dash:
				moveSpeed = moveController.dashMoveSpeed;
				break;
			case CharStateInfo.eMoveType.Run:
				moveSpeed = moveController.defaultMoveSpeed;
				break;
			case CharStateInfo.eMoveType.Keep:
				moveController.prevMoveSpeed = moveController.moveSpeed;
				moveSpeed = moveController.moveSpeed;
				break;
			case CharStateInfo.eMoveType.Stop:
				moveSpeed = 0.0f;
				break;
			}
			
			moveController.moveSpeed = moveSpeed;
		}
	}
	
	public virtual void OnEndState()
	{
		
	}
	
	public void OnAnimationBegin()
	{
		
	}
	
	public void OnAnimationEnd()
	{
		stateController.animationController.isAnimationPlaying = false;
		
		BaseState.eState nextState = BaseState.eState.Stand;
		
		switch(stateController.currentState)
		{
		case BaseState.eState.Run:
			nextState = BaseState.eState.Run;
			break;
		}
		
		stateController.ChangeState(nextState);
	}
	
	public void OnCollisionStart()
	{
		
	}
	
	public void OnCollisionStop()
	{
		
	}
	
	public void OnWalkingStart()
	{
		if (moveController != null)
		{
			stateController.curStateInfo.IncWalkingStep();
			moveController.moveSpeed = stateController.curStateInfo.walkingEventMoveSpeed;
		}
	}
	
	public void OnWalkingStop()
	{
		if (moveController != null)
		{
			float moveSpeed = 0.0f;
			switch(stateController.curStateInfo.moveType)
			{
			case CharStateInfo.eMoveType.Dash:
				moveSpeed = moveController.dashMoveSpeed;
				break;
			case CharStateInfo.eMoveType.Run:
				moveSpeed = moveController.defaultMoveSpeed;
				break;
			case CharStateInfo.eMoveType.Keep:
				moveSpeed = moveController.prevMoveSpeed;
				break;
			case CharStateInfo.eMoveType.Stop:
				moveSpeed = 0.0f;
				break;
			}
			
			moveController.moveSpeed = moveSpeed;
		}
	}
	
	public void OnStrongAttackCheck()
	{
		
	}
	
	public void OnFire()
	{
		
	}
	
	public void OnArrowEquip()
	{
		
	}
	
	public virtual void OnDialogStart()
	{
		
	}
	
	public void OnCollision()
	{
		
	}
	
	
	public float lifeTime = 1.5f;
	public void RunAway(int stringID, float delayTime)
	{
		this.transform.parent = null;
		
		if (stateController != null)
			stateController.ChangeState(BaseState.eState.Run);
		
		DestroyObject(this.gameObject, lifeTime);
		
		EventConditionChecker.Instance.ApplyRescureCount();
		
		Game.Instance.ApplyAchievement(Achievement.eAchievementType.eRescurePrisoner, 1);
		
		if (stringID != -1)
			DoTalk(stringID, delayTime, DialogInfo.eDialogType.Normal, false);
	}
	
	
	public LayerMask eventLayers = 0;
	public List<int> dialogStringIDs = new List<int>();
	public float dialogDelayTime = 1.0f;
	public bool isDialogDone = false;
	void OnTriggerEnter(Collider other)
	{
		int layerValue = 1 << other.gameObject.layer;
		if ((eventLayers & layerValue) == 0)
			return;
		
		if (isDialogDone == false)
		{
			int dlgStringID = -1;
			int nCount = dialogStringIDs.Count;
			int index = -1;
			if (nCount > 0)
				index = Random.Range(0, nCount);
			
			if (index != -1)
				dlgStringID = dialogStringIDs[index];
			
			if (dlgStringID > 0)
				DoTalk(dlgStringID, dialogDelayTime, DialogInfo.eDialogType.Normal, false);
			
			isDialogDone = true;
		}
	}
					
	public void DoTalk(int stringID, float delayTime, DialogInfo.eDialogType dlgType, bool inputPause)
	{
		if (stringID == -1)
			return;
		
		string dlgStr = "";
		StringTable stringTable = TableManager.Instance.stringTable;
		if (stringTable != null)
			dlgStr = stringTable.GetData(stringID);
		
		lifeManager.DoTalk(dlgStr, delayTime, DialogInfo.eDialogType.Normal, inputPause);
	}
}
