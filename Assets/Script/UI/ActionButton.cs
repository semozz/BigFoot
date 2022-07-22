using UnityEngine;
using System.Collections;

public class ActionButton : MonoBehaviour {
	public Transform rootNode = null;
	
	public UISlider slider = null;
	
	public UISprite defaultBackground = null;
	public UISprite disableBackground = null;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void SetEnable(bool bEnable)
	{
		if (disableBackground != null)
			NGUITools.SetActive(disableBackground.gameObject, !bEnable);
		
		if (disableBackground != null && defaultBackground != null)
			NGUITools.SetActive(defaultBackground.gameObject, bEnable);
	}
	
	public void SetCoolTimeRate(float rate)
	{
		if (slider != null)
			slider.sliderValue = rate;
		
		//bool isEnable = rate <= 0.0f;
		//SetEnable(isEnable);	
	}
}
