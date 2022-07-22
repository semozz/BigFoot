using UnityEngine;
using System.Collections;

public class SignUpController : MonoBehaviour {
	public Transform popupNode = null;
	
	public string signupPopupPrefab = "";
	public SignUpWindow signupWindow = null;
	
	public void OnSignUp(GameObject obj)
	{
		if (signupWindow == null)
			signupWindow =ResourceManager.CreatePrefab<SignUpWindow>(signupPopupPrefab, popupNode, Vector3.zero);
	}
	
	public void CloseSignUp()
	{
		if (signupWindow != null)
		{
			DestroyObject(signupWindow.gameObject, 0.0f);
			signupWindow = null;
		}
	}
}
