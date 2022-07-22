using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInput : MonoBehaviour {
	public enum eControlKey { CK_LEFT, CK_RIGHT, CK_UP, CK_DOWN };
    public enum eControlEvent { CE_JUMP, CE_DOWN, CE_REDPOTION, CE_YELLOWPOTION };
	public enum eLookDir { LD_RIGHT, LD_LEFT, LD_FRONT };
	
	public struct stInputData {
		public eControlKey key;
		public bool pressed;
		public float time;
		public eLookDir dir;
	};
	
	public Vector2 mMoveKey = Vector2.zero;
	public bool mActionAKey = false;
    public bool mActionBKey = false;
	public bool mSkillKey = false;
	
	public bool mSkill1Button = false;
	public bool mSkill2Button = false;
	
	public bool enableAttackInput = false;
	
	public bool mJumpKey = false;
	public bool jumpBlowAttack = false;
	public int jumpBlowAttackCount = 0;
	
	private List <stInputData> mLeftInputs = new List <stInputData> ();
	private List <stInputData> mRightInputs = new List <stInputData> ();
	
	public eLookDir curLookDir = eLookDir.LD_RIGHT;
	
	public bool UseKeyboard = true;
	
	
	private BaseMoveController playerMove = null;
	private StateController charController = null;
	private LifeManager lifeManager = null;
	
	public Vector3 lastMoveDir = Vector3.zero;
	
	private PlayerController player = null;
	
	void Awake()
	{
		playerMove = GetComponent<BaseMoveController>();
		charController = GetComponent<StateController>();
		lifeManager = GetComponent<LifeManager>();
		player = GetComponent<PlayerController>();
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	//private bool rightArrowPress = false;
	
	// Update is called once per frame
	public virtual void Update ()
	{
		UpdateInput();
	}
	
	public virtual void AddInputs (eControlKey key, bool pressed)
	{
		stInputData data;
		
		// auto mode stop when pressed key
		if( player != null && player.isAutoMode == true )
		{
			if( pressed == true || mJumpKey || mActionAKey || mActionBKey || mSkillKey || mSkill1Button || mSkill2Button )
				player.isAIMode = false;
			else
				player.isAIMode = true;
		}
		
		data.key = key;
		data.time = Time.time;
		data.pressed = pressed;
		data.dir = curLookDir;
		
		Vector3 moveDir = playerMove.moveDir;
		
		BaseState.eState currentState = charController.currentState;
		BaseState.eState nextState = currentState;
		
		switch (key)
		{
		case eControlKey.CK_LEFT :
			if (pressed == true)
				moveDir = Vector3.left;
			
			if (mLeftInputs.Count > 0 && mLeftInputs [mLeftInputs.Count - 1].pressed == data.pressed)
			{
				if (pressed == true)
				{
					switch(charController.currentState)
					{
					case BaseState.eState.Jumpland:
					case BaseState.eState.Blowattack:
						if (bCanBreakState == true)
							nextState = BaseState.eState.Run;
						break;
					case BaseState.eState.Stand:
						nextState = BaseState.eState.Run;
						break;
					case BaseState.eState.JumpAttack:
					case BaseState.eState.JumpStart:
					case BaseState.eState.JumpFall:
						if (playerMove.moveSpeed == 0.0f)
							playerMove.moveSpeed = playerMove.defaultMoveSpeed;
						break;
					}
				}
				else if( player.isAIMode == false )
				{
					switch(currentState)
					{
					case BaseState.eState.Stand:
					case BaseState.eState.Run:
					case BaseState.eState.Dash:
						nextState = BaseState.eState.Stand;
						break;
					}
				}
				break;
			}
		
			if (mLeftInputs.Count > 0 && data.time - mLeftInputs [mLeftInputs.Count - 1].time > 0.2f)
				mLeftInputs.Clear ();
		
			mLeftInputs.Add (data);
			if (mLeftInputs.Count > 3)
				mLeftInputs.RemoveAt (0);
		
			if (mLeftInputs.Count == 3)
			{
				if (mLeftInputs [2].pressed == true && mLeftInputs [1].pressed == false && mLeftInputs [0].pressed == true)
				{
					if (currentState == BaseState.eState.Stand)
						nextState = BaseState.eState.Dash;
				}
			}
			break;
		case eControlKey.CK_RIGHT :
			if (pressed == true)
				moveDir = Vector3.right;
			if (mRightInputs.Count > 0 && mRightInputs [mRightInputs.Count - 1].pressed == data.pressed)
			{
				if (pressed == true)
				{
					switch(charController.currentState)
					{
					case BaseState.eState.Jumpland:
					case BaseState.eState.Blowattack:
						if (bCanBreakState == true)
							nextState = BaseState.eState.Run;
						break;
					case BaseState.eState.Stand:
						nextState = BaseState.eState.Run;
						break;
					case BaseState.eState.JumpAttack:
					case BaseState.eState.JumpStart:
					case BaseState.eState.JumpFall:
						if (playerMove.moveSpeed == 0.0f)
							playerMove.moveSpeed = playerMove.defaultMoveSpeed;
						break;
					}
				}
				else if( player.isAIMode == false )
				{
					switch(currentState)
					{
					case BaseState.eState.Stand:
					case BaseState.eState.Run:
					case BaseState.eState.Dash:
						nextState = BaseState.eState.Stand;
						break;
					}	
				}
				break;
			}
		
			if (mRightInputs.Count > 0 && data.time - mRightInputs [mRightInputs.Count - 1].time > 0.2f)
				mRightInputs.Clear ();
		
			mRightInputs.Add (data);
			if (mRightInputs.Count > 3)
				mRightInputs.RemoveAt (0);
		
			if (mRightInputs.Count == 3)
			{
				if (mRightInputs [2].pressed == true && mRightInputs [1].pressed == false && mRightInputs [0].pressed == true)
				{
					if (currentState == BaseState.eState.Stand)
						nextState = BaseState.eState.Dash;
				}
			}
			break;
		}
		
		bool cantChangeDirState = false;
		switch(currentState)
		{
		case BaseState.eState.Die:
		case BaseState.eState.Knockdownstart:
		case BaseState.eState.Knockdownfall:
		case BaseState.eState.Knockdown_Die:
			cantChangeDirState = true;
			break;
		}
		
		lastMoveDir = moveDir;
		if (cantChangeDirState == false && bCanChangeDir == true)
			playerMove.ChangeMoveDir(moveDir);
		
		//playerMove.moveSpeed = moveSpeed;
		
		if (currentState != nextState)
			charController.ChangeState(nextState);
	}
	
	
	public bool bActionAKeyPress = false;
	public bool bActionAKeyRelease = false;
	public bool bActionAKeyChanged = false;
	
	public bool bCanBreakState = false;
	public bool bCanChangeDir = true;
	
	public bool ignoreActionKeyChange = false;
	
	public bool enableJump = true;
	
	public void ResetActionKeyInfo()
	{
		bActionAKeyChanged = false;
		bActionAKeyPress = false;
		bActionAKeyRelease = false;
	}
	
	public void ResetInput()
	{
		mActionAKey = false;
        mActionBKey = false;
        mSkillKey = false;
		mJumpKey = false;
		
		mMoveKey = Vector2.zero;
		
		mLeftInputs.Clear();
		mRightInputs.Clear();
	}
	
	public virtual void UpdateInput ()
	{

		if (Game.Instance.Pause == true || Game.Instance.InputPause == true)
		{
			ResetInput();
			return;
		}

#if UNITY_EDITOR
		UseKeyboard = true;
#else
		RuntimePlatform platform = Application.platform;
		switch(platform)
		{
		case RuntimePlatform.WindowsPlayer:
		case RuntimePlatform.WindowsWebPlayer:
		case RuntimePlatform.FlashPlayer:
			UseKeyboard = true;
			break;
		default:
			UseKeyboard = false;
			break;
		}
#endif
		
        if (UseKeyboard == true)
        {
            if (Input.GetKey(KeyCode.RightArrow) == true) 
				mMoveKey.x = 1.0f;
            else if (Input.GetKey(KeyCode.LeftArrow) == true) 
				mMoveKey.x = -1.0f;
            else 
				mMoveKey.x = 0.0f;
			
            if (Input.GetKeyDown(KeyCode.UpArrow) == true)
                OnControlEvent(eControlEvent.CE_JUMP);
            if (Input.GetKeyDown(KeyCode.DownArrow) == true)
                OnControlEvent(eControlEvent.CE_DOWN);
			
			if (enableAttackInput == true)
			{
				if (bActionAKeyPress == false && Input.GetKeyDown(KeyCode.LeftControl) == true && lifeManager.stunDelayTime <= 0.0f)
				{
					bActionAKeyChanged = true;
					
					bActionAKeyPress = true;
					bActionAKeyRelease = false;
					
					//Debug.Log("Actin A Key Press..." + Time.time);
				}
				
				if (bActionAKeyPress == true && Input.GetKeyUp(KeyCode.LeftControl) == true && lifeManager.stunDelayTime <= 0.0f)
				{
					bActionAKeyChanged = true;
					
					bActionAKeyRelease = true;
					bActionAKeyPress = false;
					
					//Debug.Log("Actin A Key Release..." + Time.time);
				}
				
	            mActionAKey = Input.GetKey(KeyCode.LeftControl) && lifeManager.stunDelayTime <= 0.0f;
	            mActionBKey = Input.GetKey(KeyCode.LeftAlt);
	            //mSkillKey = Input.GetKey(KeyCode.LeftShift);
				
				if (mMoveKey.x == 0.0f && Input.GetKey(KeyCode.LeftShift) == true)
					mSkill2Button = true;
				else
					mSkill2Button = false;
				
				if (mMoveKey.x != 0.0f && Input.GetKey(KeyCode.LeftShift) == true && lifeManager.stunDelayTime <= 0.0f)
					mSkill1Button = true;
				else
					mSkill1Button = false;
			}
        }
		
		if (UseKeyboard == true)
		{
			if (mMoveKey.x > 0.0f)
				AddInputs (eControlKey.CK_RIGHT, true);
			else if (mMoveKey.x < 0.0f)
				AddInputs (eControlKey.CK_LEFT, true);
			else
			{
				AddInputs (eControlKey.CK_RIGHT, false);
				AddInputs (eControlKey.CK_LEFT, false);
			}
		}
	}

    public void OnControlEvent (eControlEvent e)
    {
		//GameDef.ePotionUseResult result = GameDef.ePotionUseResult.OK;
		
        switch (e)
        {
        case eControlEvent.CE_JUMP :
			
			switch(charController.currentState)
			{
			case BaseState.eState.JumpStart:
			case BaseState.eState.JumpFall:
			case BaseState.eState.JumpAttack:
			case BaseState.eState.Knockdownfall:
				if (jumpBlowAttack == true && jumpBlowAttackCount == 0)
				{
					this.lifeManager.stunDelayTime = 0.0f;
					charController.ChangeState(BaseState.eState.Drop);
					playerMove.DoFall();
				}
				break;
			case BaseState.eState.Die:
			case BaseState.eState.Damage:
			case BaseState.eState.Knockdownstart:
			case BaseState.eState.Knockdownland:
			case BaseState.eState.Knockdown_Die:
			case BaseState.eState.Blowattack:
			case BaseState.eState.Drop:
				break;
			default:
				if (enableJump == true && lifeManager.stunDelayTime <= 0.0f)
				{
					charController.ChangeState(BaseState.eState.JumpStart);
					playerMove.DoJump();
				}
				break;
			}
			
			break;
		case eControlEvent.CE_DOWN:
			break;
		case eControlEvent.CE_REDPOTION:
			break;
		case eControlEvent.CE_YELLOWPOTION:
			break;
        }
    }
}
