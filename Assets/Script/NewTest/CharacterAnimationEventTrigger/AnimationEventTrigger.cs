using UnityEngine;
using System.Collections;

public class AnimationEventTrigger : MonoBehaviour {
	public delegate void OnAnimationEvent();
	public delegate void OnAnimationEventByString(string strValue);
	
	public OnAnimationEvent onAnimationBegin = null;
	public OnAnimationEvent onAnimationEnd = null;
	public OnAnimationEvent onCollisionStart = null;
	public OnAnimationEvent onCollisionStop = null;
	public OnAnimationEvent onWalkingStart = null;
	public OnAnimationEvent onWalkingStop = null;
	public OnAnimationEvent onStrongAttackCheck = null;
	public OnAnimationEvent onFire = null;
	public OnAnimationEvent onArrowEquip = null;
	
	public OnAnimationEvent onDisableAttackInput = null;
	public OnAnimationEvent onEnableAttackInput = null;
	
	public OnAnimationEvent onBreakState = null;
	public OnAnimationEvent onCanChangeDir = null;
	
	public OnAnimationEvent onIgnoreActionKey = null;
	
	public OnAnimationEvent onDisableJump = null;
	public OnAnimationEvent onEnableJump = null;
	
	public OnAnimationEvent onJumpBlowAttack = null;
	
	public OnAnimationEvent onRecallMonster = null;
	public OnAnimationEvent onDialogStart = null;
	
	public OnAnimationEventByString onCameraShake = null;
	
	public OnAnimationEventByString onPlaySoundA = null;
	public OnAnimationEventByString onPlaySoundB = null;
	public OnAnimationEventByString onPlaySoundC = null;
	public OnAnimationEvent onStopSound = null;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void OnAnimationBegin()
	{
		if (onAnimationBegin != null)
			onAnimationBegin();
	}
	
	public void OnAnimationEnd()
	{
		if (onAnimationEnd != null)
			onAnimationEnd();
	}
	
	public void OnCollisionStart()
	{
		if (onCollisionStart != null)
			onCollisionStart();
	}
	
	public void OnCollisionStop()
	{
		if (onCollisionStop != null)
			onCollisionStop();
	}
	
	public void OnWalkingStart()
	{
		if (onWalkingStart != null)
			onWalkingStart();
	}
	
	public void OnWalkingStop()
	{
		if (onWalkingStop != null)
			onWalkingStop();
	}
	
	public void OnStrongAttackCheck()
	{
		if (onStrongAttackCheck != null)
			onStrongAttackCheck();
	}
	
	public void OnPlaySoundA(string soundFileName)
	{
		if (onPlaySoundA != null)
			onPlaySoundA(soundFileName);
	}
	
	public void OnPlaySoundB(string soundFileName)
	{
		if (onPlaySoundB != null)
			onPlaySoundB(soundFileName);
	}
	
	public void OnPlaySoundC(string soundFileName)
	{
		if (onPlaySoundC != null)
			onPlaySoundC(soundFileName);
	}
	
	public void OnStopSound()
	{
		if (onStopSound != null)
			onStopSound();
	}
	
	public void OnSoundEvent()
	{
		
	}
	
	public void OnFire()
	{
		if (onFire != null)
			onFire();
	}
	
	public void OnArrowEquip()
	{
		if (onArrowEquip != null)
			onArrowEquip();
	}
	
	public void OnDisableAttackInput()
	{
		if (onDisableAttackInput != null)
			onDisableAttackInput();
	}
	
	public void OnEnableAttackInput()
	{
		if (onEnableAttackInput != null)
			onEnableAttackInput();
	}
	
	public void OnBreakState()
	{
		if (onBreakState != null)
			onBreakState();
	}
	
	public void OnCanChangeDir()
	{
		if (onCanChangeDir != null)
			onCanChangeDir();
	}
	
	public void OnIgnoreActionKey()
	{
		if (onIgnoreActionKey != null)
			onIgnoreActionKey();
	}
	
	public void OnDisableJump()
	{
		if (onDisableJump != null)
			onDisableJump();
	}
	
	public void OnEnableJump()
	{
		if (onEnableJump != null)
			onEnableJump();
	}
	
	public void OnJumpBlowAttack()
	{
		if (onJumpBlowAttack != null)
			onJumpBlowAttack();
	}
	
	public void OnRecallMonster()
	{
		if (onRecallMonster != null)
			onRecallMonster();
	}
	
	public void OnDialogStart()
	{
		if (onDialogStart != null)
			onDialogStart();
	}
	
	public void OnCameraShake(string strValue)
	{
		if (onCameraShake != null)
			onCameraShake(strValue);
	}
}
