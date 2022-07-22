using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ArenaThornsAttackInfo
{
	public float fxTime = 0.1f;
	public string fxPrefabPath = "";
	public GameObject fxObj = null;
	public float fxDelayTime = 1.0f;
	
	public float attackRate = 1.0f;
	public float attackTime = 0.1f;
	public Collider attackCollider = null;
}

public class ArenaThorns : BaseWeapon {
	public class DummyFXDelayInfo
	{
		public GameObject fxObj = null;
		public float delayTime = 0.0f;
	}
	
	public enum eArenaThornMode
	{
		None,
		Wait,
		Attack,
	}
	public eArenaThornMode curMode = eArenaThornMode.None;
	
	public List<ArenaThornsAttackInfo> attackInfos = new List<ArenaThornsAttackInfo>();
	public List<DummyFXDelayInfo> fxDelayList = new List<DummyFXDelayInfo>();
	
	public float delayTime = 0.0f;
	public int curAttackIndex = -1;
	
	public Vector2 startDelayTime = Vector2.one;
	public Vector2 waitDelayTime = Vector2.one;
	public Vector2 attackRandRate = Vector2.one;
	
	public float attackDamage = 0.0f;
	public float attackAbilityPower = 0.0f;
	public float attackRateValue = 1.0f;
	public StateInfo attackStateInfo = new StateInfo();
	
	public Animation anim = null;
	public string attackAnim = "attack01";
	public string defaultAnim = "stand01";

	void Awake()
	{
		AttackStateInfo newAttackInfo = new AttackStateInfo();
		
		attackStateInfo.attackRate = attackRateValue;
		
		newAttackInfo.attackDamage = attackDamage;
		newAttackInfo.abilityPower = attackAbilityPower;
		
		newAttackInfo.SetOwnerActor(null);
		newAttackInfo.SetState(attackStateInfo);
		
		SetAttackInfo(newAttackInfo);
		SetOwnerActor(null);
	}
	
	public override void Start()
	{
		//이 녀석은 weaponList에 추가 하지 않는다.. 회피? 안되도록..
		curMode = eArenaThornMode.Wait;
		delayTime = Random.Range(startDelayTime.x, startDelayTime.y);
		
		curAttackIndex = -1;
		
		InitAttack();
	}
	
	public override void OnDestroy()
	{
		//이 녀석은 weaponList에 추가 하지 않는다.. 회피? 안되도록..
		foreach(ArenaThornsAttackInfo info in attackInfos)
		{
			DestroyObject(info.fxObj, 0.0f);
		}
	}
	
	public void InitAttack()
	{
		foreach(ArenaThornsAttackInfo info in attackInfos)
		{
			SetupCollider(info.attackCollider, false);
			
			info.fxObj = CreateObjFromPrefab(info.fxPrefabPath);
			FXEffectPlay(info.fxObj, false);
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
	
	public int attackCount = 0;
	public void Update()
	{
		delayTime -= Time.deltaTime;
		
		if (delayTime <= 0.0f)
		{
			switch(curMode)
			{
			case eArenaThornMode.Wait:
				float rate = Random.Range(attackRandRate.x, attackRandRate.y);
				float randValue = Random.Range(0.0f, 1.0f);
				
				bool bAttack = rate > randValue;
				if (bAttack == true)
					curAttackIndex = ChooseAttackIndex();
				else
					curAttackIndex = -1;
				
				
				if (curAttackIndex != -1)
				{
					ArenaThornsAttackInfo attackInfo = GetAttackInfo(curAttackIndex);
					
					attackCount++;
					if (anim != null)
					{
						anim.Stop();
						anim.Play(attackAnim);
					}
					
					AddFXDelayInfo(attackInfo.fxObj, attackInfo.fxDelayTime);
					SetupCollider(attackInfo.attackCollider, true);
					
					delayTime = attackInfo.attackTime;
					curMode = eArenaThornMode.Attack;
				}
				else
				{
					delayTime = Random.Range(waitDelayTime.x, waitDelayTime.y);
				}
				break;
			case eArenaThornMode.Attack:
				if (curAttackIndex != -1)
				{
					attackCount--;
					if (anim != null)
					{
						anim.Stop();
						anim.Play(defaultAnim);
					}
					
					ArenaThornsAttackInfo attackInfo = GetAttackInfo(curAttackIndex);
					if (attackInfo != null)
						SetupCollider(attackInfo.attackCollider, false);
					
					hitObjects.Clear();
				}
				
				delayTime = Random.Range(waitDelayTime.x, waitDelayTime.y);
				
				curMode = eArenaThornMode.Wait;
				break;
			}
		}
		
		UpdateFXDelayInfo();
	}
	
	public int ChooseAttackIndex()
	{
		int index = -1;
		int nCount = this.attackInfos.Count;
		if (nCount > 0)
		{
			index = Random.Range(0, nCount);
		}
		return index;
	}
	
	public ArenaThornsAttackInfo GetAttackInfo(int index)
	{
		ArenaThornsAttackInfo attackInfo = null;
		
		int nCount = this.attackInfos.Count;
		if (index >= 0 && index < nCount)
			attackInfo = attackInfos[index];
		
		return attackInfo;
	}
	
	public LayerMask attackableLayers = 0;
	public void OnTriggerEnter(Collider other)
	{
		int layerValue = 1 << other.gameObject.layer;
		if ((attackableLayers & layerValue) == 0)
			return;
		
		if (curAttackIndex != -1)
		{
			ArenaThornsAttackInfo attackInfo = GetAttackInfo(curAttackIndex);
			if (attackInfo != null)
				this.attackInfo.addAttackPower = attackInfo.attackRate;
		}
		
		LifeManager actor = other.gameObject.GetComponent<LifeManager>();
		if (actor != null)
		{
			Debug.Log("Arena Thorns .... attack To.................." + other.name);
		
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
}
