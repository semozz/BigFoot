using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class StateInfo
{
	public enum eAttackType { AT_NONE, AT_ENABLEAVOID, AT_DISABLEAVOID };
	public enum eAttackState { AS_NONE, AS_DAMAGE, AS_KNOCKDOWN };
	public enum eDefenseState { DS_NORMAL, DS_WEAK1, DS_WEAK2, DS_SUPERARMOR, DS_INVINCIBLE, DS_PROTECT, DS_BLOCK, DS_AVOID };
	
	public enum eAttackCategory
	{
		None = -1,
		Normal,
		Special_01,
		Special_02,
		Blow,
		Dash,
		Skill_01,
		Skill_02,
		Max_Count,
	}
	
	public eAttackType attackType = eAttackType.AT_NONE;
	public eAttackState attackState = eAttackState.AS_NONE;
	public eDefenseState defenseState = eDefenseState.DS_NORMAL;
	
	public Vector3 knockDir = Vector3.zero;
	public Vector3 knockDir_Air = Vector3.zero;
	public float knockPower = 0.0f;
	
	public float attackRate = 1.0f;
	public float addAttackRate = 0.0f;
	public float painValue = 0.0f;
	
	public float addCriticalHitRate = 0.0f;
	
	public float stunRate = 0.0f;
	public float addStunRate = 0.0f;
	public float stunTime = 1.0f;
	
	public string fxObjectName = "";
	public eFXEffectType effectType = eFXEffectType.ScaleNode;
	public float effectScale = 1.0f;
	
	public float acquireAbility = 0.0f;
	public float requireAbilityValue = 0.0f;
	
	public float addAcquireAbility = 0.0f;
	public float addRequireAbility = 0.0f;
	
	public float addAcquireAbilityRate = 0.0f;
	public float addRequireAbilityRate = 0.0f;
	
	public string soundFile = "";
	public bool isWeaponAttack = true;
	
	public void SetInfo(StateInfo newInfo)
	{
		this.attackState = newInfo.attackState;
		this.attackType = newInfo.attackType;
		this.defenseState = newInfo.defenseState;
		
		this.attackRate = newInfo.attackRate;
		this.addAttackRate = newInfo.addAttackRate;
		this.painValue = newInfo.painValue;
		
		this.addCriticalHitRate = newInfo.addCriticalHitRate;
		
		this.stunRate = newInfo.stunRate;
		this.addStunRate = newInfo.addStunRate;
		this.stunTime = newInfo.stunTime;
		
		this.knockDir = newInfo.knockDir;
		this.knockDir_Air = newInfo.knockDir_Air;
		
		this.knockPower = newInfo.knockPower;
		
		this.fxObjectName = newInfo.fxObjectName;
		this.effectType = newInfo.effectType;
		this.effectScale = newInfo.effectScale;
		
		this.acquireAbility = newInfo.acquireAbility;
		this.requireAbilityValue = newInfo.requireAbilityValue;
		
		this.addAcquireAbilityRate = newInfo.addAcquireAbilityRate;
		this.addRequireAbilityRate = newInfo.addRequireAbilityRate;
		
		this.soundFile = newInfo.soundFile;
		this.isWeaponAttack = newInfo.isWeaponAttack;
	}
}

[System.Serializable]
public class BaseState
{
	public enum eState {
		None = -1,
		Stand,
		Run,
		Dash,
		JumpStart,
        Jumpland,
		JumpFall,
		Drop,
		Knockdownstart,
		Knockdownfall,
        Knockdownland,
		Down,
        Dashattack,
		Attack1, 
		Attack2, 
		Attack21, 
		Attack3,
        Heavyattack,
        Blowattack,
		JumpAttack,
        Skill01,
        Skill02,
        Returnstand,
		Damage,
        Die,
        Stun,
        Evadecounterattack,
        Evadestart,
        Evadeend,
		Stage_clear1,
		Stage_clear2,
		Attack3_Base,
		Attack3_Focus,
		Attack3_1_Ready,
		Block,
		BlockAttack,
		Fly_Up,
		Fly_Down,
		Fly_DownAttack,
		Knockdown_Die,
		StageEnd,
		AttackB_1,
		AttackB_2,
		AttackB_3,
		Runaway,
		StageEndStart,
		Sleep,
	}
	
	public eState state = eState.Stand;
	public string animationClip = "None";
	
	public static eState ToState(string typeStr)
	{
		eState state = eState.None;
		if (typeStr == "Stand")
			state = eState.Stand;
		else if (typeStr == "Run")
			state = eState.Run;
		else if (typeStr == "Dash")
			state = eState.Dash;
		else if (typeStr == "JumpStart")
			state = eState.JumpStart;
		else if (typeStr == "Jumpland")
			state = eState.Jumpland;
		else if (typeStr == "JumpFall")
			state = eState.JumpFall;
		else if (typeStr == "Drop")
			state = eState.Drop;
		else if (typeStr == "Knockdownstart")
			state = eState.Knockdownstart;
		else if (typeStr == "Knockdownfall")
			state = eState.Knockdownfall;
		else if (typeStr == "Knockdownland")
			state = eState.Knockdownland;
		else if (typeStr == "Down")
			state = eState.Down;
		else if (typeStr == "Dashattack")
			state = eState.Dashattack;
		else if (typeStr == "Attack1")
			state = eState.Attack1;
		else if (typeStr == "Attack2")
			state = eState.Attack2;
		else if (typeStr == "Attack21")
			state = eState.Attack21;
		else if (typeStr == "Attack3")
			state = eState.Attack3;
		else if (typeStr == "Heavyattack")
			state = eState.Heavyattack;
		else if (typeStr == "Blowattack")
			state = eState.Blowattack;
		else if (typeStr == "JumpAttack")
			state = eState.JumpAttack;
		else if (typeStr == "Skill01")
			state = eState.Skill01;
		else if (typeStr == "Skill02")
			state = eState.Skill02;
		else if (typeStr == "Returnstand")
			state = eState.Returnstand;
		else if (typeStr == "Damage")
			state = eState.Damage;
		else if (typeStr == "Die")
			state = eState.Die;
		else if (typeStr == "Stun")
			state = eState.Stun;
		else if (typeStr == "Evadecounterattack")
			state = eState.Evadecounterattack;
		else if (typeStr == "Evadestart")
			state = eState.Evadestart;
		else if (typeStr == "Evadeend")
			state = eState.Evadeend;
		else if (typeStr == "Stage_clear1")
			state = eState.Stage_clear1;
		else if (typeStr == "Evadestart")
			state = eState.Evadestart;
		else if (typeStr == "Stage_clear2")
			state = eState.Stage_clear2;
		else if (typeStr == "Attack3_Base")
			state = eState.Attack3_Base;
		else if (typeStr == "Attack3_Focus")
			state = eState.Attack3_Focus;
		else if (typeStr == "Attack3_1_Ready")
			state = eState.Attack3_1_Ready;
		else if (typeStr == "Block")
			state = eState.Block;
		else if (typeStr == "BlockAttack")
			state = eState.BlockAttack;
		else if (typeStr == "Fly_Up")
			state = eState.Fly_Up;
		else if (typeStr == "Fly_Down")
			state = eState.Fly_Down;
		else if (typeStr == "Fly_DownAttack")
			state = eState.Fly_DownAttack;
		else if (typeStr == "Knockdown_Die")
			state = eState.Knockdown_Die;
		else if (typeStr == "StageEnd")
			state = eState.StageEnd;
		else if (typeStr == "AttackB_1")
			state = eState.AttackB_1;
		else if (typeStr == "AttackB_2")
			state = eState.AttackB_2;
		else if (typeStr == "AttackB_3")
			state = eState.AttackB_3;
		
		
		return state;
	}
}

[System.Serializable]
public class CollisionInfo
{
	public string colliderName = "";
	public StateInfo stateInfo = new StateInfo();
}

[System.Serializable]
public class CharStateInfo
{
	public BaseState baseState = new BaseState();
	public StateInfo stateInfo = new StateInfo();
	
	public List<CollisionInfo> collisionInfoList = new List<CollisionInfo>();
	
	public bool chainAttack = false;
	
	public bool cantChangeDir = false;
	
	public float patienceFactor = 1.0f;
	public float walkingEventMoveSpeed = 0.0f;
	
	public int walkingStep = 0;
	public List<float> walkingSpeedList = new List<float>();
	
	public void AddWalkingSpeed(float speed)
	{
		walkingSpeedList.Add(speed);
	}
	
	public void SetDefaultWalkingSpeed(float speed)
	{
		int nCount = walkingSpeedList.Count;
		if (nCount == 0)
			AddWalkingSpeed(speed);
		else
			SetWalkingSpeed(speed, 0);
	}
	
	public void RemoveSpeed(int index)
	{
		int nCount = walkingSpeedList.Count;
		if (index >= 0 && nCount > index)
			walkingSpeedList.RemoveAt(index);
	}
	
	public void SetWalkingSpeed(float speed, int index)
	{
		int nCount = walkingSpeedList.Count;
		if (index >= 0 && nCount > index)
			walkingSpeedList[index] = speed;
	}
	
	public void InitWalkingStep()
	{
		walkingStep = 0;
	}
	
	public void IncWalkingStep()
	{
		int nCount = walkingSpeedList.Count;
		if (nCount > 0)
		{
			walkingEventMoveSpeed = walkingSpeedList[walkingStep];
			walkingStep = (walkingStep + 1) % nCount;
		}
		else
		{
			walkingEventMoveSpeed = 0.0f;
			walkingStep = 0;
		}
	}
	
	public enum eMoveType
	{
		Run,
		Dash,
		Stop,
		Keep,
	}
	public eMoveType moveType = eMoveType.Stop;
}