using UnityEngine;
using System.Collections;

public class UIRootPanel : MonoBehaviour {
	public Transform popUpNode = null;
	
	void Awake()
	{
		GameUI.Instance.uiRootPanel = this;
	}
}
