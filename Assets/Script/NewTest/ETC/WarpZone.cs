using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WarpInfo
{
	public float delayTime = 0.0f;
	public LifeManager warpActor = null;
}

public class WarpZone : MonoBehaviour {
	public AudioSource audioSource = null;
	public SoundEffect loopSource = null;
	
	public string activeSound = "";
	public string loopSound = "";
	public string warpSound = "";
	
	public BoxCollider triggerArea = null;
	public Transform warpTarget = null;
	
	public GameObject effectNode = null;
	public string createAnim = "create";
	public string readyAnim = "stand";
	public string activeAnim = "active";
	
	public string fxWarpEffect = "";
	public float fxDelayTime = 1.0f;
	
	public float warpDelayTime = 1.0f;
	
	public enum eWarpZoneState
	{
		Ready,
		Activate,
	}
	private eWarpZoneState currentState = eWarpZoneState.Ready;
	
	private List<WarpInfo> warpInfoList = new List<WarpInfo>();
	
	// Use this for initialization
	void Start () {
		ChangeState(eWarpZoneState.Ready);
	}
	
	
	private void ChangeState(eWarpZoneState state)
	{
		if (currentState != state)
			InitWarpInfoList();
		
		currentState = state;
		switch(currentState)
		{
		case eWarpZoneState.Ready:
			SetTrigger(false);
			break;
		case eWarpZoneState.Activate:
			SetTrigger(true);
			break;
		}
	}
	
	private void InitWarpInfoList()
	{
		warpInfoList.Clear();	
	}
	
	private void AddWarpInfo(LifeManager actor, float delayTime)
	{
		int index = 0;
		int nCount = warpInfoList.Count;
		
		int selectedIndex = -1;
		WarpInfo selectedWarpInfo = null;
		
		for (index = 0; index < nCount; ++index)
		{
			WarpInfo info = warpInfoList[index];
			if (info == null)
				continue;
			
			if (info.warpActor == actor)
			{
				selectedIndex = index;
				selectedWarpInfo = info;
				break;
			}
		}
		
		if (selectedIndex == -1)
		{
			WarpInfo newInfo = new WarpInfo();
			newInfo.warpActor = actor;
			newInfo.delayTime = delayTime;
			
			warpInfoList.Add(newInfo);
		}
		else
		{
			selectedWarpInfo.warpActor = actor;
			selectedWarpInfo.delayTime = delayTime;
			
			warpInfoList[selectedIndex] = selectedWarpInfo;
		}
	}
	
	private void RemoveWarpInfo(LifeManager actor)
	{
		int index = 0;
		int nCount = warpInfoList.Count;
		
		for (index = 0; index < nCount; ++index)
		{
			WarpInfo info = warpInfoList[index];
			if (info == null)
				continue;
			
			if (info.warpActor == actor)
			{
				warpInfoList.RemoveAt(index);
				break;
			}
		}
	}
	
	public LayerMask groundLayers = 0;
	private void DoWarp(LifeManager actor)
	{
		if (actor == null)
			return;
		
		actor.stateController.AddFXDelayInfo(fxWarpEffect, eFXEffectType.ScaleNode, 1.0f, fxDelayTime);
		
		RaycastHit hit;
		
		Vector3 vTargetPos = warpTarget.position;
		Vector3 vStartPos = vTargetPos + (Vector3.up * 1.0f);
		
		if (Physics.Raycast(vStartPos, Vector3.down, out hit, Mathf.Infinity, groundLayers) == true)
        {
			vTargetPos = hit.point + Vector3.up;
		}
		
		//if (actor.stateController.IsJumpState() == true)
		//	actor.stateController.SetJumpHeight(vTargetPos.y);
		
		actor.gameObject.transform.position = vTargetPos;
		actor.stateController.ChangeState(BaseState.eState.Stand);
		
		if (GameOption.effectToggle == true)
		{
			float effectVolume = Game.Instance.effectSoundScale;
			AudioManager.PlaySound(audioSource, warpSound, effectVolume);
		}
	}
	
	private void SetTrigger(bool bActivate)
	{
		if (triggerArea != null)
			triggerArea.enabled = bActivate;
		
		if (effectNode != null)
		{
			effectNode.SetActive(bActivate);
			
			if (effectNode.animation != null)
			{
				if (bActivate == true)
				{
					effectNode.animation.Play(this.createAnim);
			
					if (loopSource != null)
						loopSource.PlayEffect(activeSound, loopSound);
				}
				else
				{
					if (loopSource != null)
						loopSource.StopEffect();
				}
			}
		}
	}
	
	public void OnActivate()
	{
		ChangeState(eWarpZoneState.Activate);
	}
	
	public void OnDeactivate()
	{
		ChangeState(eWarpZoneState.Ready);	
	}
	
	// Update is called once per frame
	void Update () {
	
		for (int index = warpInfoList.Count - 1; index >= 0; --index)
		{
			WarpInfo info = warpInfoList[index];
			info.delayTime -= Time.deltaTime;
			
			if (info.delayTime <= 0.0f)
			{
				DoWarp(info.warpActor);
				
				warpInfoList.RemoveAt(index);
			}
		}
	}
	
	public LayerMask acceptableLayers = 0;
	void OnTriggerEnter (Collider other)
	{
		if (currentState != eWarpZoneState.Activate)
			return;
		
		Debug.Log("OnTriggerEnter");
		
		int layerValue = 1 << other.gameObject.layer;
		if ((acceptableLayers & layerValue) == 0)
			return;
		
		LifeManager actor = other.gameObject.transform.root.GetComponent<LifeManager>();
		if (actor == null || actor.GetHPRate() <= 0.0f)
			return;
		
		AddWarpInfo(actor, warpDelayTime);
		
		if (warpInfoList.Count >= 0)
		{
			if (effectNode != null)
			{
				if (effectNode.animation != null)
					effectNode.animation.Play(this.activeAnim);
			}	
		}
	}
	
	void OnTriggerExit (Collider other)
	{
		if (currentState != eWarpZoneState.Activate)
			return;
		
		Debug.Log("OnTriggerExit");
		
		int layerValue = 1 << other.gameObject.layer;
		if ((acceptableLayers & layerValue) == 0)
			return;
		
		LifeManager actor = other.gameObject.transform.root.GetComponent<LifeManager>();
		
		RemoveWarpInfo(actor);
		
		if (warpInfoList.Count <= 0)
		{
			if (effectNode != null)
			{
				if (effectNode.animation != null)
					effectNode.animation.Play(this.readyAnim);
			}	
		}
	}
}
