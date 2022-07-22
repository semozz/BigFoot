using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class DialogInfo
{
	public enum eDialogType
	{
		Big,
		Normal,
		Small,
	}
	public int stringTableID = 0;
	public float delayTime = 0.0f;
	public int actorIndex = 0;
	public eDialogType dialogType = eDialogType.Normal;
	
	public bool preventInput = false;
	public bool limitAction = true;
};

[System.Serializable]
public class EventDialogTrigger : EventConditionTrigger {
	/*
	public LifeController.eActorType ownerActorType = LifeController.eActorType.PLAYER_ACTOR;
	public LifeController.eActorType targetActorType = LifeController.eActorType.NONE;
	*/
	
	public ActorInfo.ActorType ownerActorType = ActorInfo.ActorType.Player;
	public ActorInfo.ActorType targetActorType = ActorInfo.ActorType.None;
	
	private PlayerController ownerActor = null;
	private BaseMonster targetActor = null;
	
	public List<DialogInfo> dialogInfoList = new List<DialogInfo>();
	private int currentDialogIndex = 0;
	private float delayTime = 0.0f;
	
	public float startCoolTime = 1.5f;
	
	
	private bool isTargetMoveMode = false;
	public Transform playerTarget = null;
	public float playerDeltaMove = 4.0f;
	
	public Transform bossTarget = null;
	public float bossDeltaMove = 4.0f;
	
	
	private bool bPaused = false;
	
	
	private ActorManager actorManager = null;
	
	// Use this for initialization
	void Start () {
		
		Invoke("InitData", 0.2f);
	}
	
	public void InitData()
	{
		delayTime = startCoolTime;
		
		if (actorManager == null)
			actorManager = ActorManager.Instance;
		
		if (ownerActorType == ActorInfo.ActorType.Player)
			ownerActor = Game.Instance.player;
		
		ActorInfo info = actorManager.GetActorInfo(targetActorType);
		if (info != null)
			targetActor = info.gameObject.GetComponent<BaseMonster>();
	}
	
	// Update is called once per frame
	void Update () {
		
		if (isActivate == false)
			return;
		
		delayTime -= Time.deltaTime;
		if (delayTime <= 0.0f)
		{
			delayTime = 0.0f;
			
			if (ownerActor == null)
				return;
			
			BaseState.eState currentState = BaseState.eState.Stand;
			if (ownerActor != null && ownerActor.stateController != null)
				currentState = ownerActor.stateController.currentState;
			
			bool isAvailable = false;
			
			DialogInfo dlgInfo = GetDialogInfo(currentDialogIndex);
			if (dlgInfo != null)
			{
				if (dlgInfo.limitAction == true)
				{
					switch(currentState)
					{
					case BaseState.eState.Stand:
					case BaseState.eState.Stage_clear1:
					case BaseState.eState.Stage_clear2:
						isAvailable = true;
						break;
					default:
						isAvailable = false;
						break;
					}
				}
				else
					isAvailable = true;
			}
			
			if (isAvailable == true)
			{
				//Debug.Log("Actor dialog........... " + currentDialogIndex);
				ChangeDialog(dlgInfo);
				
				if (_EventCondtion != null)
					_EventCondtion.AddCondtionValue(1);
				
				++currentDialogIndex;
			}
		}
	}
	
	private void ChangeDialog(DialogInfo dlgInfo)
	{
		//dialogInfoList[currentDialogIndex];
		if (dlgInfo != null)
		{
			LifeManager actor = null;
			int talkID = -1;
			
			if (dlgInfo.actorIndex == 0 && ownerActor != null)
			{
				int classIndex = 0;
				switch(ownerActor.classType)
				{
				case GameDef.ePlayerClass.CLASS_WARRIOR:
					classIndex = 500;
					break;
				case GameDef.ePlayerClass.CLASS_ASSASSIN:
					classIndex = 600;
					break;
				case GameDef.ePlayerClass.CLASS_WIZARD:
					classIndex = 700;
					break;
				}
				
				talkID = classIndex + dlgInfo.stringTableID;
				actor = ownerActor.lifeManager;
			}
			else if (dlgInfo.actorIndex == 1 && targetActor != null)
			{
				talkID = dlgInfo.stringTableID;
				actor = targetActor.lifeManager;				
			}
			
			if (talkID != -1 && actor != null)
			{
				string msg = "";
				TableManager tableManager = TableManager.Instance;
				StringTable stringTable = null;
				if (tableManager != null)
					stringTable = tableManager.stringTable;
				
				if (stringTable != null)
					msg = stringTable.GetData(talkID);
				//Debug.Log("Dialog .... " + msg);
				actor.DoTalk(msg, dlgInfo.delayTime, dlgInfo.dialogType, dlgInfo.preventInput);
			}
			
			delayTime = dlgInfo.delayTime;
		}		
	}
	
	private DialogInfo GetDialogInfo(int index)
	{
		DialogInfo info = null;
		if (index < 0 || index >= dialogInfoList.Count)
			return info;
		
		info = dialogInfoList[index];
		return info;
	}
	
	public override void OnActivate()
	{		
		PlayerActorWarp();
			
		base.OnActivate();
	}
	
	public override void OnComplete()
	{
		this.isActivate = false;
		
		if (isTargetMoveMode == true)
		{
			if (playerTarget != null)
			{
				if (ownerActor != null)
					ownerActor.OnCompleteDialogEvent();
			}
			
			if (bossTarget != null)
			{
				if (targetActor != null)
					targetActor.OnCompleteDialogEvent();
			}
			
			isTargetMoveMode = false;
		}
		
		if (bPaused == true)
			OnDialogEnd();
	}
	
	public override void UnActivate()
	{
		base.UnActivate();
	}
	
	public void OnStageClear()
	{
		/*
		if (Game.instance != null)
			Game.instance.SetStageClearMode(true);
		*/
	}
	
	public void OnDialogStart()
	{
		/*
		if (Game.instance != null)
		{
			Game.instance.IsInputPause = true;
			Game.instance.IsPause = true;
			
			this.bPaused = true;
		}
		*/
		
		this.bPaused = true;
	}
	
	public void OnDialogEnd()
	{
		/*
		if (Game.instance != null)
		{
			Game.instance.IsInputPause = false;
			Game.instance.IsPause = false;
			
			this.bPaused = false;
		}
		*/
		
		this.bPaused = false;
	}
	
	public void PlayerActorWarp()
	{
		/*
		if (playerTarget != null)
		{
			PlayerController player = Game.instance.player;
			if (player != null)
			{
				RaycastHit hit;
				
				Vector3 vPos = playerTarget.position + Vector3.up * 100.0f;
				if (Physics.Raycast(vPos, Vector3.down, out hit, Mathf.Infinity, player.FloorMask) == true)
				{
					player.transform.position = hit.point + Vector3.up;
					
					player.LookDir = MoveController.eLookDir.LD_RIGHT;
					player.UpdateLookDirection();
					
					player.SetTargetMovePos(playerTarget.position + Vector3.right * playerDeltaMove);
					
					isTargetMoveMode = true;
				}			
			}
		}
		
		if (bossTarget != null)
		{
			if (bossActor != null)
			{
				RaycastHit hit;
				
				Vector3 vPos = bossTarget.position + Vector3.up * 100.0f;
				if (Physics.Raycast(vPos, Vector3.down, out hit, Mathf.Infinity, bossActor.FloorMask) == true)
				{
					bossActor.transform.position = hit.point + Vector3.up;
				
					bossActor.LookDir = MoveController.eLookDir.LD_LEFT;
					bossActor.UpdateLookDirection();
					
					bossActor.SetTargetMovePos(bossTarget.position + Vector3.left * bossDeltaMove);
					
					isTargetMoveMode = true;
				}
			}
		}
		
		if (isTargetMoveMode == true)
		{
			Game.instance.IsInputPause = true;
		}
		*/
	}
}
