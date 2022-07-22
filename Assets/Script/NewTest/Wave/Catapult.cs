using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CatapultUpgradeInfo
{
	public string faceSprite = "";
	public Texture upgradeTexture = null;
	public float attackRate = 1.0f;
	public AttributeValue.eAttributeType attValueType = AttributeValue.eAttributeType.AttackDamage;
}

public class Catapult : BaseMonster {
	
	public string catapultNodeName = "Bip001 Prop1";
	private Transform catapultNode = null;
	public GameObject catapultPrefab = null;
	
	protected CatapultBall catapultBall = null; 
	
	protected Vector3 targetPos = Vector3.zero;
	protected bool bTargeting = false;
	
	public float upgradeTime = 30.0f;
	public float upgradeDelayTime = 0.0f;
	public int curUpgradeIndex = 0;
	public List<CatapultUpgradeInfo> upgradeInfoList = new List<CatapultUpgradeInfo>();
	
	public WaveManager waveManager = null;
	
	public GameObject upgradeEffect = null;
	
	public override void Start () {
		base.Start();
		
		Transform[] bones = GetComponentsInChildren<Transform>();
        foreach (Transform bone in bones)
        {
            if (bone.name == catapultNodeName)
			{
				catapultNode = bone;
				break;
			}
        }
		
		WaveModeUI waveModeUI = GameUI.Instance.waveModeUI;
		if (waveModeUI != null && waveModeUI.catapultHP != null)
		{
			waveModeUI.catapultHP.SetEnable(true);
			
			CatapultUpgradeInfo upInfo = GetUpgradeInfo(this.curUpgradeIndex);
			if (upInfo != null)
				waveModeUI.catapultHP.SetFace(upInfo.faceSprite);
			
			lifeManager.attributeManager.hpUI = waveModeUI.catapultHP.hp;
			lifeManager.attributeManager.hpInfoLabel = waveModeUI.catapultHPInfoLabel;
			
			lifeManager.attributeManager.UpdateHPUI();
		}
	}
	
	public CatapultUpgradeInfo GetUpgradeInfo(int index)
	{
		CatapultUpgradeInfo info = null;
		int nCount = this.upgradeInfoList.Count;
		if (index < 0 && index >= nCount)
			return info;
		
		info = this.upgradeInfoList[index];
		return info;
	}
	
	public override void FireProjectile()
	{
		if (bTargeting == false)
			return;
		
		if (attackTargetInfo == null) return;
		
        if (catapultBall == null) return;
		
		catapultBall.transform.parent = null;
		Vector3 firePos = catapultNode.position;
		firePos.z = 0.0f;
		catapultBall.transform.position = firePos;
		catapultBall.transform.localScale = Vector3.one;
		
		Vector3 vCreatePos = catapultBall.transform.position;
		Vector3 vMoveDir = targetPos - vCreatePos;
		catapultBall.TargetDir = vMoveDir;
		
		vMoveDir.Normalize();
		
        catapultBall.MoveDir = vMoveDir;
		
		catapultBall.SetOwnerActor(lifeManager);
		catapultBall.SetAttackInfo(lifeManager.GetCurrentAttackInfo());
        catapultBall.SetFired();
		
		catapultBall.mFireBallMode = FireBall.eFireBall_Mode.DETECT_MODE;
		
		if (this.moveController != null)
			this.moveController.SetProjectileCollider(catapultBall.detectCollider, attackTargetInfo);
		
		catapultBall = null;
	}
	
	public override void DoArrowEquip()
	{
		if (catapultNode == null) return;
        
        GameObject go = (GameObject)Instantiate(catapultPrefab);
        if (go == null) return;
        catapultBall = go.GetComponent<CatapultBall>();
        if (catapultBall == null) return;
		
		catapultBall.mFireBallMode = FireBall.eFireBall_Mode.WAIT_MODE;
		
        go.transform.parent = catapultNode;
        go.transform.localPosition = Vector3.zero;
		go.transform.localScale = Vector3.one;
		
        go.transform.localRotation = Quaternion.Euler(0.0f, -90.0f, 0.0f);
        if (moveController.moveDir == Vector3.right)
            go.transform.localRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
		
		if (attackTargetInfo != null)
		{
			targetPos = attackTargetInfo.transform.position;
			bTargeting = true;
		}
		else
		{
			targetPos = Vector3.zero;
			bTargeting = false;
		}
	}
	
	public override void Update()
	{
		if (Game.Instance.Pause == true)
			return;
		
		if (upgradeDelayTime <= 0.0f)
		{
			upgradeDelayTime = upgradeTime;
			
			if (CheckUpgradeState() == true)
			{
				int nCount = upgradeInfoList.Count;
				if (curUpgradeIndex < nCount)
				{
					CatapultUpgradeInfo info = upgradeInfoList[curUpgradeIndex];
					OnUpgrade(info);
					curUpgradeIndex++;
				}
			}
		}
		upgradeDelayTime -= Time.deltaTime;
		
		base.Update();
		
		if (this.stateController.animationController.isAnimationPlaying == false)
		{			
			switch(this.stateController.currentState)
			{
			case BaseState.eState.Stand:
			case BaseState.eState.Die:
			case BaseState.eState.Knockdown_Die:
				break;
			default:
				this.stateController.ChangeState(BaseState.eState.Stand);
				break;
			}
		}
	}
	
	public bool CheckUpgradeState()
	{
		float hpValue = lifeManager.GetHP();
		bool isDieState = false;
		switch(stateController.currentState)
		{
		case BaseState.eState.Die:
		case BaseState.eState.Knockdown_Die:
			isDieState = true;
			break;
		}
		
		if (hpValue > 0.01f && isDieState == false)
			return true;
		else
			return false;
	}
		
	public void OnUpgrade(CatapultUpgradeInfo info)
	{
		if (this.stateController != null)
			this.stateController.ChangeState(BaseState.eState.Stand);
		
		WaveModeUI waveModeUI = GameUI.Instance.waveModeUI;
		if (waveModeUI != null && waveModeUI.catapultHP != null)
		{
			if (info != null)
				waveModeUI.catapultHP.SetFace(info.faceSprite);
		}
		
		if (lifeManager.attributeManager != null)
		{
			AttributeValue attValue = lifeManager.attributeManager.GetAttribute(info.attValueType);
			if (attValue != null)
				attValue.addRate = info.attackRate;
			
			lifeManager.attributeManager.UpdateValue(attValue);
		}
		
		foreach(Renderer render in meshRenderers)
		{
			render.material.mainTexture = info.upgradeTexture;
		}
		
		StartUpgradeEffect();
	}
	
	public void StartUpgradeEffect()
	{
		if (upgradeEffect != null)
		{
			stateController.FXPlayObject(upgradeEffect, true);
		}
		
		Invoke("StopUpgradeEffect", 1.5f);
	}
	
	public void StopUpgradeEffect()
	{
		if (upgradeEffect != null)
		{
			stateController.FXPlayObject(upgradeEffect, false);
		}
	}
	
	public GameObject dieFX = null;
	public override void OnDie(LifeManager attacker)
	{
		base.OnDie(attacker);
		
		if (dieFX != null)
			dieFX.SetActive(true);
		
		if (catapultBall != null)
		{
			DestroyObject(catapultBall.gameObject, 0.2f);
			catapultBall = null;
		}
		
		if (this.waveManager != null)
			this.waveManager.catapult = null;
		
		WaveModeUI waveModeUI = GameUI.Instance.waveModeUI;
		if (waveModeUI != null && waveModeUI.catapultHP != null)
		{
			waveModeUI.catapultHP.hp.sliderValue = 0.0f;
			waveModeUI.catapultHP.Invoke("DisableUI", 0.2f);
		}
	}
}
