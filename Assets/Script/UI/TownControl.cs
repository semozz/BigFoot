using UnityEngine;
using System.Collections;

public class TownControl : MonoBehaviour {
	public Camera townCamera = null;
	
	public GameObject leftTrigger = null;
	public GameObject rightTrigger = null;
	
	public GameObject rightIndicator = null;
	public GameObject leftIndicator = null;
	
	
	// Update is called once per frame
	void Update () {
		bool isIn = false;
		
		isIn = isCameraArea(townCamera, leftTrigger);
		
		SetActive(rightIndicator, !isIn);
		SetActive(leftIndicator, isIn);
		
		isIn = isCameraArea(townCamera, rightTrigger);
		
		SetActive(rightIndicator, isIn);
		SetActive(leftIndicator, !isIn);
		
	}
	
	public bool isCameraArea(Camera cam, GameObject obj)
	{
		bool isIn = false;
		Vector3 pos = cam.WorldToViewportPoint(obj.transform.position);
		if(pos.z > 0 && pos.x >= 0.0f && pos.x <=1.0f && pos.y >= 0.0f && pos.y <=1.0f) 
		{
			isIn = true;
		}
		
		return isIn;
	}
	
	public void SetActive(GameObject obj, bool isActive)
	{
		if (obj != null)
			obj.SetActive(isActive);
	}
}
