using UnityEngine;
using System.Collections;

public class BaseDropItem : MonoBehaviour {
	public enum eDropState
	{
		InitState,
		IdleState,
		TwinkleState,
		DestroyState,
	}
	public eDropState curState = eDropState.InitState;
	
	public enum eDropType
	{
		None,
		Potion,
		Coin,
		Jewel,
		Item,
		MaterialItem,
		EventDropItem,
	}
	public eDropType dropType = eDropType.None;
	
	public LayerMask triggerLayerMask;
	
	public Animation anim = null;
	public string initAnimName = "Coin_Drop";
	public string defaultAnimName = "Coin_Stand01";
	public string destroyAnimName = "Coin_Eat";
	public string twinkleAnimName = "twinkle";
	
	public float lifeTime = 5.0f;
	public float twinkleLeftTime = 0.3f;
	
	public AnimationEventTrigger animEventTrigger = null;
	
	public int dropInfoValue = 0;
	public int addValue = 0;
	
	public SoundManager soundManager = null;
	public AudioSource audioSource = null;
	public virtual void Awake()
	{
		if (animEventTrigger != null)
		{
			animEventTrigger.onAnimationEnd = new AnimationEventTrigger.OnAnimationEvent(OnAnimationEnd);
			
			animEventTrigger.onPlaySoundA = new AnimationEventTrigger.OnAnimationEventByString(OnPlaySoundA);
			animEventTrigger.onPlaySoundB = new AnimationEventTrigger.OnAnimationEventByString(OnPlaySoundB);
			animEventTrigger.onPlaySoundC = new AnimationEventTrigger.OnAnimationEventByString(OnPlaySoundC);
			
			animEventTrigger.onStopSound = new AnimationEventTrigger.OnAnimationEvent(OnStopSound);
		}
		
		if (soundManager == null)
			soundManager = this.gameObject.AddComponent<SoundManager>();
	}
	
	public void Update()
	{
		if (Game.Instance.Pause == true)
			return;
		
		switch(curState)
		{
		case eDropState.IdleState:
			lifeTime -= Time.deltaTime;
			if (lifeTime < twinkleLeftTime)
			{
				if (anim != null)
					anim.Play(twinkleAnimName);
				
				curState = eDropState.TwinkleState;
			}
			break;
		}
	}
	
	public void OnAnimationEnd()
	{
		switch(curState)
		{
		case eDropState.InitState:
			if (anim != null)
				anim.Play(defaultAnimName);
			
			curState = eDropState.IdleState;
			break;
		case eDropState.IdleState:
			//Destroy(this.gameObject);
			break;
		case eDropState.TwinkleState:
			Destroy(this.gameObject);
			break;
		case eDropState.DestroyState:
			Destroy(this.gameObject);
			break;
		}
		
		OnStopSound();
	}
	
	public void OnTriggerEnter(Collider other)
	{
		int layerValue = other.gameObject.layer;
		int checkLayerMask = 1 << layerValue;
		
		int maskValue = triggerLayerMask.value;
		
		if ((maskValue & checkLayerMask) == checkLayerMask)
		{
			if (anim != null)
				anim.Play(destroyAnimName);
			
			curState = eDropState.DestroyState;
			
			CharInfoData charInfo = Game.Instance.charInfoData;
			ClientConnector connector = Game.Instance.connector;
			if (charInfo != null && connector != null)
			{
				//int charIndex = connector.charIndex;
				charInfo.AddDropItem(this);
			}
		}
	}
	
	public void OnActivate()
	{
		if (anim != null)
			anim.Play(initAnimName);
		
		curState = eDropState.InitState;
	}
	
	public void OnPlaySoundA(string soundFileName)
	{
		if (soundManager != null && GameOption.effectToggle == true)
			soundManager.AddSoundEffect(soundFileName, SoundEffect.eSoundType.DontCare);
	}
	
	public void OnPlaySoundB(string soundFileName)
	{
		if (soundManager != null && GameOption.effectToggle == true)
			soundManager.AddSoundEffect(soundFileName, SoundEffect.eSoundType.TryKeep);
	}
	
	public void OnPlaySoundC(string soundFileName)
	{
		if (soundManager != null && GameOption.effectToggle == true)
			soundManager.AddSoundEffect(soundFileName, SoundEffect.eSoundType.CancelByState);
	}
	
	public void OnStopSound()
	{
		if (soundManager != null)
			soundManager.StopSoundEffects(SoundEffect.eSoundType.TryKeep);
	}
}
