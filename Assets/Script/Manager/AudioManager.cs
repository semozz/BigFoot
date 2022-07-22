using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

public class SoundInfo
{
	public string soundFileName = "";
	public float lifeTime = 0.0f;
	
	public SoundInfo(string fileName, float time)
	{
		soundFileName = fileName;
		lifeTime = time;
	}
}

static public class AudioManager  {

	static AudioListener mListener;
	
	static Dictionary<string, int> soundCountList = new Dictionary<string, int>();
	static int maxSoundCount = 5;
	
	static List<SoundInfo> soundInfos = new List<SoundInfo>();
	
	static public AudioClip GetSoundClip(string soundFileName)
	{
		int soundCount = 0;
		if (soundCountList.ContainsKey(soundFileName) == true)
		{
			soundCount = soundCountList[soundFileName];
		}
		else
		{
			soundCountList.Add(soundFileName, 0);
		}
		
		AudioClip clip = null;
		if (soundCount < maxSoundCount)
		{
			string soundPath = string.Format("Sounds/{0}", soundFileName);
			if (soundFileName != "")
				clip = ResourceManager.LoadAudio(soundPath);
			
			if (clip != null)
			{
				soundCount++;
				soundCountList[soundFileName] = soundCount;
				
				AddSoundInfo(soundFileName, clip.length);
			}
		}
		
		return clip;
	}
	
	static void AddSoundInfo(string soundFileName, float lifeTime)
	{
		SoundInfo info = new SoundInfo(soundFileName, lifeTime);
		soundInfos.Add(info);
	}
	
	static public void Update()
	{
		List<int> deleteIndexs = new List<int>();
		int nCount = soundInfos.Count;
		for (int index = nCount - 1; index >= 0 ; --index)
		{
			SoundInfo temp = soundInfos[index];
			
			temp.lifeTime -= Time.deltaTime;
			if (temp.lifeTime <= 0.0f)
			{
				int count = soundCountList[temp.soundFileName];
				
				soundCountList[temp.soundFileName] = Mathf.Max(0, count - 1);
				
				deleteIndexs.Add(index);
			}
		}
		
		nCount = deleteIndexs.Count;
		for (int index = 0; index < nCount; ++index)
		{
			int deletIndex = deleteIndexs[index];
			soundInfos.RemoveAt(deletIndex);
		}
	}
	
	static public void PlaySound(string soundFileName, float volume)
	{
		AudioClip clip = GetSoundClip(soundFileName);
		
		PlaySound(clip, volume, 1.0f);
	}
	
	static public void PlaySound(AudioSource source, string soundFileName, float volume)
	{
		AudioClip clip = GetSoundClip(soundFileName);
		
		PlaySound(source, clip, volume, 1.0f);
	}
	
	static public void PlaySound(AudioSource source, string soundFileName, float volume, bool bLoop)
	{
		AudioClip clip = GetSoundClip(soundFileName);
		
		PlaySound(source, clip, volume, 1.0f, bLoop);
	}
	
	static public void PlaySound (AudioSource source, AudioClip clip, float volume, float pitch)
	{
		PlaySound(source, clip, volume, pitch, false);
	}
	
	static public AudioSource PlaySound (AudioSource source, AudioClip clip, float volume, float pitch, bool bLoop)
	{
		volume *=Game.Instance.volumeScale;

		if (clip != null && volume > 0.01f)
		{
			if (source != null)
			{
				source.pitch = pitch;
				source.loop = bLoop;
				
				source.clip = clip;
				source.Play();
				
				return source;
			}
		}
		return null;
	}
	
	static public AudioSource PlaySoundOneShot (AudioClip clip, float volume, float pitch)
	{
		volume *=Game.Instance.volumeScale;

		if (clip != null && volume > 0.01f)
		{
			if (mListener == null)
			{
				mListener = GameObject.FindObjectOfType(typeof(AudioListener)) as AudioListener;

				if (mListener == null)
				{
					PlayerController player = Game.Instance.player;
					if (player != null)
						mListener = player.gameObject.AddComponent<AudioListener>();
				}
			}

			if (mListener != null)
			{
				AudioSource source = mListener.audio;
				if (source == null) source = mListener.gameObject.AddComponent<AudioSource>();
				source.pitch = pitch;
				source.PlayOneShot(clip, volume);
				
				return source;
			}
		}
		return null;
	}
	
	static public AudioSource PlaySound (AudioClip clip, float volume, float pitch)
	{
		volume *=Game.Instance.volumeScale;

		if (clip != null && volume > 0.01f)
		{
			if (mListener == null)
			{
				mListener = GameObject.FindObjectOfType(typeof(AudioListener)) as AudioListener;

				if (mListener == null)
				{
					PlayerController player = Game.Instance.player;
					if (player != null)
						mListener = player.gameObject.AddComponent<AudioListener>();
				}
			}

			if (mListener != null)
			{
				AudioSource source = mListener.audio;
				if (source == null) source = mListener.gameObject.AddComponent<AudioSource>();
				source.pitch = pitch;
				//source.PlayOneShot(clip, volume);
				source.clip = clip;
				source.volume = volume;
				source.Play();
				
				return source;
			}
		}
		return null;
	}
	
	
	static public void PlayBGM(string bgm, float volume)
	{
		AudioClip clip = GetSoundClip(bgm);
		
		PlayBGM(clip, volume, 1.0f);
	}
	
	
	public static string soundEffectPrefab = "NewAsset/Effect/SoundEffect";
	public static SoundEffect bgmSound = null;
	public static void PlayBGM(AudioClip clip, float volume, float pitch)
	{
		if (clip != null && volume > 0.01f)
		{
			if (mListener == null)
			{
				mListener = GameObject.FindObjectOfType(typeof(AudioListener)) as AudioListener;

				if (mListener == null)
				{
					PlayerController player = Game.Instance.player;
					if (player != null)
						mListener = player.gameObject.AddComponent<AudioListener>();
				}
			}

			if (mListener != null)
			{
				if (bgmSound == null)
					bgmSound = ResourceManager.CreatePrefab<SoundEffect>(soundEffectPrefab, mListener.transform, Vector3.zero);
				
				if (bgmSound != null)
				{
					AudioSource source = bgmSound.audioSource;
					
					source.pitch = pitch;
					source.clip = clip;
					source.volume = volume;
					source.loop = true;
					source.Play();		
				}
			}
		}
		else
		{
			StopBGM();
		}
	}
	
	static public void StopBGM ()
	{
		if (bgmSound != null)
		{
			if (bgmSound.audioSource != null)
				bgmSound.audioSource.Stop();
			
			GameObject.DestroyObject(bgmSound.gameObject, 0.0f);
			bgmSound = null;
		}
	}
}
