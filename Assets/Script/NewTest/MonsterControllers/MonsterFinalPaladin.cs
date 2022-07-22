using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterFinalPaladin : MonsterFinalBerserk {
	
	private float alphaValue = 1.0f;
	private bool recallMonster = false;
	public float fadeOutSpeed = 0.3f;
	
	public GameObject transformMonster = null;
	
	public override void Start()
	{
		base.Start();
		
		if (animEventTrigger != null)
		{
			animEventTrigger.onRecallMonster = new AnimationEventTrigger.OnAnimationEvent(OnRecallMonster);
		}
	}
	
	public void OnRecallMonster()
	{
		if (lifeManager.isBossRaidMonster == true)
			return;
		
		ResetMonster();
		
		GameObject newMonster = null;
		
		if (this.transformMonster != null)
		{
			newMonster = (GameObject)Instantiate(this.transformMonster);
			if (newMonster != null)
			{
				RaycastHit hit;
				Vector3 vStartPos = this.gameObject.transform.position + (Vector3.up * 50.0f);
				var layerMask = 1 << LayerMask.NameToLayer("Ground");
				if (Physics.Raycast(vStartPos, Vector3.down, out hit, Mathf.Infinity, layerMask) == true)
				{
					newMonster.transform.position = hit.point;
				}
			}
		}
		
		this.alphaValue = 1.0f;
		recallMonster = true;
	}
	
	public void ResetMonster()
	{
		WaveManager waveManager = null;
		if (GameUI.Instance != null)
			waveManager = GameUI.Instance.waveManager;
		
		if (waveManager != null)
			waveManager.ResetMonsterGeneratorByBossRecall();
		
		ActorManager actorManager = ActorManager.Instance;
		List<ActorInfo> teamList = null;
		if (actorManager != null)
			teamList = actorManager.GetActorList(myInfo.myTeam);
		
		if (teamList != null)
		{
			foreach(ActorInfo info in teamList)
			{
				if (info == myInfo)
					continue;
				
				BaseMonster monster = info.gameObject.GetComponent<BaseMonster>();
				if (monster != null)
				{
					AttributeValue health = monster.lifeManager.attributeManager.GetAttribute(AttributeValue.eAttributeType.Health);
					if (health != null)
						health.baseValue = 0.0f;
					
					monster.lifeManager.attributeManager.UpdateValue(health);
					
					monster.stateController.ChangeState(BaseState.eState.Die);
				}
			}
		}
		
		
	}
	
	public override void Update ()
	{
		base.Update();
		
		if (recallMonster == true)
		{
			alphaValue = Mathf.Lerp(alphaValue, 0.0f, fadeOutSpeed);
			
			if (meshRenderers != null)
			{
				foreach(Renderer renderer in meshRenderers)
				{
					if (renderer.material != null && renderer.material.HasProperty("_Alpha") == true)
					{
						/*
						float origAlpha = renderer.material.GetFloat("_Alpha");
						origAlpha = alphaValue;
						
						renderer.material.SetFloat("_Alpha", origAlpha);
						
						if (origAlpha <= 0.01f)
							renderer.enabled = false;
						*/
						
						Color origAlpha = renderer.material.GetColor("_Alpha");
						origAlpha.a = alphaValue;
						
						renderer.material.SetColor("_Alpha", origAlpha);
						
						if (origAlpha.a <= 0.01f)
							renderer.enabled = false;
					}
				}
			}
			
			//ShadowOnOff(alphaValue != 0.0f);
		}
	}
	
	public override BaseState.eState ChangeNextState()
	{
		BaseState.eState nextState = base.ChangeNextState();
		
		switch(stateController.currentState)
		{
		case BaseState.eState.Die:
		case BaseState.eState.Knockdown_Die:
			DestroyObject(gameObject, 0.0f);
			break;
		}
		
		return nextState;
	}
}
