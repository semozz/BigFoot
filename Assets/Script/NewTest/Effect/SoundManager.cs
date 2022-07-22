using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour {

	public GameObject soundRoot = null;
	
	void Start()
	{
		if (soundRoot == null)
		{
			soundRoot = new GameObject("SoundRoot");
			soundRoot.transform.parent = this.transform;
			soundRoot.transform.localPosition = Vector3.zero;
		}
	}
	
	void Update()
	{
		UpdateSoundEffect();
	}
	
	public string soundEffectPrefab = "NewAsset/Effect/SoundEffect";
	public void AddSoundEffect(string soundFileName, SoundEffect.eSoundType soundType)
	{
		foreach(SoundEffect effect in soundEffects)
		{
			if (effect.soundType == soundType && effect.soundFile == soundFileName)
			{
				effect.PlayEffect();
				return;
			}
		}
		
		SoundEffect soundEffect = ResourceManager.CreatePrefab<SoundEffect>(soundEffectPrefab, soundRoot.transform, Vector3.zero);
		if (soundEffect != null)
		{
			soundEffect.soundFile = soundFileName;
			soundEffect.soundType = soundType;
			
			AddSoundEffect(soundEffect);
		}
	}
	
	public List<SoundEffect> soundEffects = new List<SoundEffect>();
	public void AddSoundEffect(SoundEffect soundEffect)
	{
		if (soundEffect != null)
		{
			soundEffects.Add(soundEffect);
			soundEffect.PlayEffect();
		}
	}
	
	public void StopSoundEffects(SoundEffect.eSoundType soundType)
	{
		foreach(SoundEffect effect in soundEffects)
		{
			if (effect.soundType == soundType)
			{
				effect.StopEffect();
			}
		}
	}
	
	public void UpdateSoundEffect()
	{
		List<int> deleteIndexs = new List<int>();
		int nCount = soundEffects.Count;
		for( int index = 0; index < nCount; ++index)
		{
			SoundEffect effect = soundEffects[index];
			if (effect == null ||
				effect.lifeTime > 0.0f && effect.leftLifeTime < 0.0f)
			{
				deleteIndexs.Add(index);
			}
		}
		
		nCount = deleteIndexs.Count;
		for (int index = nCount - 1; index >= 0; --index)
		{
			SoundEffect effect = soundEffects[index];
			if (effect != null)
				DestroyObject(effect.gameObject, 0.1f);
			
			soundEffects.RemoveAt(index);
		}
	}
}
