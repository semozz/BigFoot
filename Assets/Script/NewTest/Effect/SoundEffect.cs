using UnityEngine;
using System.Collections;

public class SoundEffect : MonoBehaviour {
	public enum eSoundType
	{
		DontCare,
		TryKeep,
		CancelByState
	}
	public eSoundType soundType = eSoundType.TryKeep;
	
	public AudioSource audioSource = null;
	public string soundFile = "";
	public AudioClip audioClip = null;
	
	public bool isSelfDestroy = false;
	public void Update()
	{
		if (lifeTime > 0)
		{
			leftLifeTime -= Time.deltaTime;
		
			if (isSelfDestroy == true && leftLifeTime < 0.0f)
				DestroyObject(this.gameObject, 0.1f);
		}
	}
	
	void OnDestroy()
	{
		StopEffect();
	}
	
	public float lifeTime = -1.0f;
	public float leftLifeTime = 0.0f;
	public void PlayEffect()
	{
		if (GameOption.effectToggle == true)
		{
			float effectVolume = Game.Instance.effectSoundScale;
			
			audioClip = AudioManager.GetSoundClip(this.soundFile);
			if (audioClip != null)
				leftLifeTime = lifeTime = audioClip.length;
			
			AudioManager.PlaySound(audioSource, audioClip, effectVolume, 1.0f);
		}
	}
	
	public void StopEffect()
	{
		if (this.soundType == eSoundType.CancelByState)
			audioSource.Stop();

		leftLifeTime = -1.0f;
	}
	
	
	private string startSound = "";
	private string loopSound = "";
	public void PlayEffect(string start, string loop)
	{
		if (GameOption.effectToggle == true)
		{
			this.startSound = start;
			this.loopSound = loop;
			
			leftLifeTime = this.lifeTime = -1.0f;
			
			float effectVolume = Game.Instance.effectSoundScale;
			
			if (startSound != "")
			{
				string soundPath = string.Format("Sounds/{0}", startSound);
				//AudioClip clip = (AudioClip)Resources.Load(soundPath);
				AudioClip clip = null;
				if (startSound != "")
					clip = ResourceManager.LoadAudio(soundPath);
				
				float delayTime = clip != null ? clip.length : 0.0f;
				
				AudioManager.PlaySound(audioSource, startSound, effectVolume);
				
				Invoke("LoopSound", delayTime);
			}
			else
			{
				AudioManager.PlaySound(audioSource, loopSound, effectVolume);
			}
		}
	}
	
	public void LoopSound()
	{
		if (GameOption.effectToggle == true)
		{
			float effectVolume = Game.Instance.effectSoundScale;
			AudioManager.PlaySound(audioSource, loopSound, effectVolume, true);
		}
	}
}
