using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseWeapon : MonoBehaviour {
	public enum eAddResult
	{
		AddOK,
		AlreadyAdd,
		Evade,
	}
	
	public ColliderManager colliderManager = null;
	public AttackStateInfo attackInfo = new AttackStateInfo();
	
	public string fxAttackFxFileInfo = "";
	public float fxAttackFxScaleInfo = 1.0f;
	
	public bool isPiercing = false;
	
	protected List<LifeManager> hitObjects = new List<LifeManager>();
	
	public StateInfo.eAttackCategory AttackCategory = StateInfo.eAttackCategory.None;
	
	public AudioSource audioSource = null;
	public virtual void Start()
	{
		WeaponManager weaponManager = WeaponManager.Instance;
		if (weaponManager != null)
			weaponManager.AddWeapon(this);
		
		if (audioSource != null)
			audioSource.mute = !GameOption.effectToggle;
	}
	
	public virtual void OnDestroy()
	{
		WeaponManager weaponManager = WeaponManager.Instance;
		if (weaponManager != null)
			weaponManager.RemoveWeapon(this);
	}
	
	public void SetOwnerActor(LifeManager actor)
	{
		attackInfo.ownerActor = actor;
	}
	
	public AttackStateInfo GetAttackInfo()
	{
		return attackInfo;	
	}
	
	public void SetAttackInfo(AttackStateInfo info)
	{
		attackInfo.attackDamage = info.attackDamage;
		attackInfo.abilityPower = info.abilityPower;
		attackInfo.addAttackPower = info.addAttackPower;
		attackInfo.addAttackRate = info.addAttackRate;
		attackInfo.attackState = info.attackState;
		
		attackInfo.SetState(info.stateInfo);
	}
	
	public virtual eAddResult AddHitObject(LifeManager hitActor)
    {
        if (hitObjects.Contains(hitActor) == true)
            return eAddResult.AlreadyAdd;
		
		hitObjects.Add(hitActor);
        return eAddResult.AddOK;
    }
	
	public virtual void SetDestroy(LifeManager hitActor)
	{
		
	}
	
	public void FXEffectPlay(GameObject fxObject, bool bPlay)
	{
		if (fxObject == null)
			return;
		
		if (bPlay == true)
		{
			if (fxObject != null && fxObject.audio != null)
			{
				fxObject.audio.mute = !GameOption.effectToggle;
			}
		}
		
		fxObject.SetActive(bPlay);
		
		ParticleRenderer[] renderers = fxObject.GetComponentsInChildren<ParticleRenderer>();
        foreach (ParticleRenderer r in renderers)
            r.gameObject.SetActive(bPlay);
	
		Transform[] childs = fxObject.GetComponentsInChildren<Transform>();
		if (childs == null || childs.Length == 0)
		{
			Animation rootAnim = fxObject.GetComponent<Animation>();
			if (rootAnim != null)
			{
				if (bPlay == true)
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
					
					if (bPlay == true)
						childAnim.Play();
					else
						childAnim.Stop();
				}
			}
		}	
	}
	
	public void AddBuff(float hitRate, GameDef.eBuffType type, float buffValue, float buffTime)
	{
		attackInfo.AddBuff(hitRate, type, buffValue, buffTime);
	}
	
	public virtual Vector3 GetMoveDir()
	{
		Vector3 moveDir = Vector3.zero;
		return moveDir;
	}
	
	public string soundEffectPrefab = "NewAsset/Effect/SoundEffect";
	public void AddSoundEffect(string soundFile, Transform root, SoundEffect.eSoundType soundType)
	{
		if (GameOption.effectToggle == true)
		{
			SoundEffect bombEffect = ResourceManager.CreatePrefab<SoundEffect>(soundEffectPrefab, root);
			if (bombEffect != null)
			{
				bombEffect.isSelfDestroy = true;
				bombEffect.soundFile = soundFile;
				bombEffect.soundType = soundType;
				
				if (root == null)
					bombEffect.transform.position = this.transform.position;
				
				bombEffect.PlayEffect();
			}
		}
	}
}
