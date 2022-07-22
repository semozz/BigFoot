using UnityEngine;
using System.Collections;

public class ComboCountUI : BaseDamageUI {
	public UILabel comboLabel = null;
	
	public Color comboColor = Color.white;
	public Animation anim = null;
	public LifeManager owner = null;
	void Start()
	{
		if (comboLabel != null)
			comboLabel.color = comboColor;
	}
	
	public void SetComboCount(int count)
	{
		if (comboLabel != null)
		{
			comboLabel.text = string.Format("{0}", count);
			
			if (anim != null)
				anim.Play();
		}
	}
	
	void Update()
	{
		this.TotalTime -= Time.deltaTime;
		if (this.TotalTime <= 0.0f)
		{
			if (owner != null)
				owner.DeleteComboUI();
		}
	}
	
	public void DoDestory()
	{
		TweenAlpha tweenAlpha = this.gameObject.AddComponent<TweenAlpha>();
		if (tweenAlpha != null)
		{
			tweenAlpha.duration = 0.5f;
			tweenAlpha.to = 0.0f;
			
			tweenAlpha.eventReceiver = this.gameObject;
			tweenAlpha.callWhenFinished = "FinalDestroy";
			
			tweenAlpha.ignoreTimeScale = true;
		}
	}
	
	public void FinalDestroy()
	{
		DestroyObject(this.gameObject, 0.0f);
	}
}
