using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ShockWaveAttackInfo
{
	public float moveTime = 0.3f;
	public float moveSpeed = 1.0f;
	
	public float fxTime = 0.1f;
	public string fxPrefabPath = "";
	public GameObject fxObj = null;
	public float fxDelayTime = 1.0f;
	
	public float attackRate = 1.0f;
	public float attackTime = 0.1f;
	public Collider attackCollider = null;
	
	public string soundFile = "";
}

public class ShockWave : BaseWeapon {
	public class DummyFXDelayInfo
	{
		public GameObject fxObj = null;
		public float delayTime = 0.0f;
	}
	
	public enum eShockWaveMode
	{
		None,
		Move,
		FX,
		Attack,
		DestroyWait,
	}
	public eShockWaveMode curMode = eShockWaveMode.None;
	
	public List<ShockWaveAttackInfo> attackInfos = new List<ShockWaveAttackInfo>();
	
	public float moveSpeed = 1.0f;
	
	public float delayTime = 0.0f;
	
	public int attackIndex = -1;
	public ShockWaveAttackInfo curAttackInfo = null;
	
	public Vector3 moveDir = Vector3.zero;
	
	public List<DummyFXDelayInfo> fxDelayList = new List<DummyFXDelayInfo>();
	
	public override void Start()
	{
		base.Start();
	}
	
	public void InitAttack()
	{
		foreach(ShockWaveAttackInfo info in attackInfos)
		{
			SetupCollider(info.attackCollider, false);
			
			info.fxObj = CreateObjFromPrefab(info.fxPrefabPath);
			FXEffectPlay(info.fxObj, false);
		}
		
		curAttackInfo = GetNextAttackInfo(++attackIndex);
		if (curAttackInfo != null)
		{
			curMode = eShockWaveMode.Move;
			
			moveSpeed = curAttackInfo.moveSpeed;
			delayTime = curAttackInfo.moveTime;
		}
	}
	
	public GameObject CreateObjFromPrefab(string prefabPath)
	{
		GameObject newObj = null;
		
		GameObject prefab = ResourceManager.LoadPrefab(prefabPath);
		if (prefab != null)
			newObj = (GameObject)Instantiate(prefab);
		
		return newObj;
	}
	
	protected void SetupCollider (Collider c, bool bActive)
    {
        if (c == null) return;

        c.isTrigger = bActive;
        if (c.GetType() == typeof(MeshCollider))
            ((MeshCollider)c).convex = bActive;
        c.gameObject.SetActive(bActive);
    }
	
	public void Update()
	{
		delayTime -= Time.deltaTime;
		
		if (delayTime <= 0.0f)
		{
			switch(curMode)
			{
			case eShockWaveMode.Move:
				delayTime = curAttackInfo.fxTime;
				curMode = eShockWaveMode.FX;
				
				if (curAttackInfo.attackCollider != null && curAttackInfo.fxObj != null)
					curAttackInfo.fxObj.transform.position = curAttackInfo.attackCollider.transform.position;
				
				AddFXDelayInfo(curAttackInfo.fxObj, curAttackInfo.fxDelayTime);
				
				if (attackInfo.ownerActor != null)
				{
					//float effectVolume = Game.Instance.effectSoundScale;
					//AudioManager.PlaySound(attackInfo.ownerActor.audioSource, curAttackInfo.soundFile, effectVolume);
					AddSoundEffect(curAttackInfo.soundFile, null, SoundEffect.eSoundType.DontCare);
				}
				
				break;
			case eShockWaveMode.FX:
				delayTime = curAttackInfo.attackTime;
				curMode = eShockWaveMode.Attack;
				SetupCollider(curAttackInfo.attackCollider, true);
				break;
			case eShockWaveMode.Attack:
				SetupCollider(curAttackInfo.attackCollider, false);
				
				hitObjects.Clear();
				
				curAttackInfo = GetNextAttackInfo(++attackIndex);
				if (curAttackInfo == null)
				{
					curMode = eShockWaveMode.DestroyWait;
				}
				else
				{
					delayTime = curAttackInfo.moveTime;
					moveSpeed = curAttackInfo.moveSpeed;
					
					curMode = eShockWaveMode.Move;
				}
				break;
			}
		}
		
		if (curMode == eShockWaveMode.DestroyWait)
		{
			if (fxDelayList.Count == 0)
			{
				DestroyObject(this.gameObject, 0.0f);
			}
		}
		
		UpdateFXDelayInfo();
	}
	
	public override void OnDestroy()
	{
		base.OnDestroy();
		
		foreach(ShockWaveAttackInfo info in attackInfos)
		{
			DestroyObject(info.fxObj, 0.0f);
		}
	}
	
	public ShockWaveAttackInfo GetNextAttackInfo(int index)
	{
		ShockWaveAttackInfo tempAttackInfo = null;
		
		int nCount = this.attackInfos.Count;
		if (index < 0 || index >= nCount)
			return tempAttackInfo;
		
		tempAttackInfo = attackInfos[index];
		return tempAttackInfo;
	}
	
	public void LateUpdate()
	{
		if (curMode != eShockWaveMode.Move)
			return;
		
		float deltaTime = Time.deltaTime;
		
		Vector3 vMove = moveDir * moveSpeed * deltaTime;
		
		Vector3 oldPos = transform.position;
		Vector3 vNewPos = oldPos + vMove;
		
		int groundMask = 1 << LayerMask.NameToLayer("Ground");
		RaycastHit hit;
		if (Physics.Raycast(vNewPos + (Vector3.up * 2.0f), Vector3.down, out hit, float.MaxValue, groundMask) == true)
		{
			vNewPos = hit.point;
		}
		else
			vNewPos = oldPos;
		
		
		transform.position = vNewPos;
	}
	
	public LayerMask attackableLayers = 0;
	public void OnTriggerEnter(Collider other)
	{
		int layerValue = 1 << other.gameObject.layer;
		if ((attackableLayers & layerValue) == 0)
			return;
		
		if (curAttackInfo != null)
			this.attackInfo.addAttackRate = this.curAttackInfo.attackRate;
		
		LifeManager actor = other.gameObject.GetComponent<LifeManager>();
		if (actor != null)
		{
			Debug.Log("ShockWave .... attack To.................." + other.name);
		
			actor.OnDamage(this.attackInfo, this.transform, false);
		}
	}
	
	
	public void AddFXDelayInfo(GameObject fxObj, float periodTime)
	{
		if (fxObj == null)
			return;
		
		DummyFXDelayInfo addInfo = new DummyFXDelayInfo();
		addInfo.fxObj = fxObj;
		addInfo.delayTime = periodTime;
		
		FXEffectPlay(fxObj, true);
		
		fxDelayList.Add(addInfo);
	}
	
	public void UpdateFXDelayInfo()
	{
		List<DummyFXDelayInfo> deleteInfos = new List<DummyFXDelayInfo>();
			
		for (int index = 0; index < fxDelayList.Count; ++index)
		{
			DummyFXDelayInfo info = fxDelayList[index];
			info.delayTime -= Time.deltaTime;
			
			if (info.delayTime > 0.0f)
				continue;
			
			deleteInfos.Add(info);
		}
		
		foreach(DummyFXDelayInfo info in deleteInfos)
		{
			FXEffectPlay(info.fxObj, false);
				
			fxDelayList.Remove(info);
		}
	}
	
	public override Vector3 GetMoveDir()
	{
		return this.moveDir;
	}
}
