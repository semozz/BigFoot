using UnityEngine;
using System.Collections;

public class RewardItemSlot : MonoBehaviour {
	public Animation FXLight = null;
	public GameObject Get = null;
	public GameObject Selected = null;
	
	public void ResetSlot()
	{
		SetGetActive(false);
		SetSelectedActive(false);
		FXLight.gameObject.SetActive(false);
	}
	
	public void SetSelectedActive(bool bActive)
	{
		if( Selected != null )
			Selected.SetActive(bActive);
	}
	
	public void SetGetActive(bool bActive)
	{
		if( Get != null )
			Get.SetActive(bActive);
	}
	
	public void PlayFXLight()
	{
		if( FXLight != null )
			FXLight.gameObject.SetActive(true);
	}
}
