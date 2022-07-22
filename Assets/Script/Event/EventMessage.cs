using UnityEngine;
using System.Collections;

public class EventMessage : MonoBehaviour {
	
	public UILabel titleLabel = null;
	public UILabel msgLabel = null;
	
	public GameObject animationObject = null;
	
	public enum eDisplayMode
	{
		Step_0,
		Step_1,
		Step_2,
		Step_3,
		Step_4,
		MaxCount,
	}
	private eDisplayMode currentMode = eDisplayMode.Step_0;
	
	public float lifeTime = 4.5f;
	public float[] defaultTime = new float[5];
	private float currentDelayTime = 0.0f;
	
	// Use this for initialization
	void Start () {
		SetTextAlpha(0.0f);
		
		if (this.audio != null)
			this.audio.mute = !GameOption.effectToggle;
	}
	
	public bool isDontDestroy = false;
	void Update () {
		lifeTime -= Time.deltaTime;
		
		if (lifeTime <= 0.0f && isDontDestroy == false)
		{
			DestroyImmediate(this.gameObject);
			return;
		}
		
		if (currentDelayTime > 0.0f)
			currentDelayTime -= Time.deltaTime;
		
		float alphaValue = 0.0f;
		switch(currentMode)
		{
		case eDisplayMode.Step_0:
			alphaValue = 0.0f;
			break;
		case eDisplayMode.Step_1:
			alphaValue = 1.0f - (currentDelayTime / defaultTime[(int)eDisplayMode.Step_1]);
			break;
		case eDisplayMode.Step_2:
			alphaValue = 1.0f;
			break;
		case eDisplayMode.Step_3:
			alphaValue = (currentDelayTime / defaultTime[(int)eDisplayMode.Step_3]);
			break;
		case eDisplayMode.Step_4:
			alphaValue = 0.0f;
			break;
		}
		
		SetTextAlpha(alphaValue);
		
		if (currentDelayTime <= 0.0f)
			currentMode = ChangeStep(currentMode);
	}
	
	private eDisplayMode ChangeStep(eDisplayMode mode)
	{
		eDisplayMode newMode = mode;
		switch(mode)
		{
		case eDisplayMode.Step_0:
			newMode = eDisplayMode.Step_1;
			break;
		case eDisplayMode.Step_1:
			newMode = eDisplayMode.Step_2;
			break;
		case eDisplayMode.Step_2:
			newMode = eDisplayMode.Step_3;
			break;
		case eDisplayMode.Step_3:
			newMode = eDisplayMode.Step_4;
			break;
		case eDisplayMode.Step_4:
			newMode = eDisplayMode.Step_4;
			break;
		}
		
		currentDelayTime = defaultTime[(int)newMode];
		
		return newMode;
	}
	
	public void SetMessage(string title, string msg, float delayTime)
	{
		currentDelayTime = defaultTime[0];
		
		if (titleLabel != null)
		{
			titleLabel.text = title;	
		}
		
		if (msgLabel != null)
		{
			msgLabel.text = msg;
		}
	}
	
	public void SetTextAlpha(float alpha)
	{
		/*
		if (this.titleLabel != null)
		{
			Color origColor = this.titleLabel.Color;
			origColor.a = alpha;
			this.titleLabel.Color = origColor;
		}
		
		if (this.msgLabel != null)
		{
			Color origColor = this.msgLabel.Color;
			origColor.a = alpha;
			this.msgLabel.Color = origColor;
		}
		*/
	}
}
