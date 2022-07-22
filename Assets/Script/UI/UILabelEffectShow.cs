using UnityEngine;
using System.Collections;

public class UILabelEffectShow : MonoBehaviour 
{
	UILabel text;
	Animator Ani;
	
	Color addColor;
	Color delColor;
	
	public AudioSource audioSource = null;
	public string soundFileName = "Item_Buy_Sell";
	void Awake()
	{
		text = gameObject.GetComponentInChildren<UILabel>();
		
		text.effectStyle = UILabel.Effect.Outline;
		
		//addColor = Color(238, 226, 168);
		//delColor = color(245,51,5);

	}
	
	void Start()
	{
		if (audioSource == null)
			audioSource = gameObject.AddComponent<AudioSource>();
	}
	
	public void SetValue(int current, int target)
	{
		if (!text)
			return;
		
		if (current == target)
			return;
		
		int diff = target - current;
		
		string param1;
		
		if (diff > 0)
		{
			param1 = "+";
			text.color =  Color.yellow;

		}
		else
		{
			param1 = "-";
			text.color = 	Color.red;
		}
			
			
		string str = string.Format("{0}{1}", param1, Mathf.Abs(target-current));		

		
		text.text = str;
		
		//text..r = 238;
		//text.color.g = 226;
		//text.color.b =  168;
		

		gameObject.SetActive(true);
		gameObject.animation.Play();
		
		if (GameOption.effectToggle == true)
		{
			AudioClip clip = AudioManager.GetSoundClip(soundFileName);
			if (audioSource != null && clip != null)
			{
				float effectVolume = Game.Instance.effectSoundScale;
				AudioManager.PlaySound(audioSource, clip, effectVolume, 1.0f);
			}
		}
	}
	
	public void OnLabelEffectEnd()
	{
		gameObject.SetActive(false);
	}
}
