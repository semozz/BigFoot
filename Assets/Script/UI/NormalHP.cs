using UnityEngine;
using System.Collections;

public class NormalHP : MonoBehaviour {
	public UISlider hp = null;
	
	public GameObject warningObj = null;
	public float warningDelayTime = 1.0f;
	
	void Start()
	{
		if (warningObj != null && warningObj.audio != null)
			warningObj.audio.mute = !GameOption.effectToggle;
	}
	
	public void SetEnable(bool bEnable)
	{
		this.gameObject.SetActive(bEnable);
	}
	
	public virtual void DisableUI()
	{
		SetEnable(false);
	}
	
	public void OnWarning()
	{
		if (warningObj != null)
			warningObj.SetActive(true);
		
		Invoke("OffWarning", warningDelayTime);
	}
	
	public void OffWarning()
	{
		if (warningObj != null)
			warningObj.SetActive(false);
	}
}
