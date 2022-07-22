using UnityEngine;
using System.Collections;

public class LoadingSound : MonoBehaviour {
	
	public AudioSource bgm = null;
	public AudioSource stamp = null;
	
	public string bgmPath = "Sound/BGM_Aria";
	public string stampPath = "Sound/Stamp";
	
	void Start()
	{
		bgm.clip = Resources.Load(bgmPath) as AudioClip;
		stamp.clip = Resources.Load(stampPath) as AudioClip;
		bgm.Play();
		stamp.Play();
		Invoke("DestroyStamp", stamp.clip.length);
	}
	
	public void DestroyStamp()
	{
		if (stamp != null)
			DestroyObject(stamp.gameObject);
	}
}
