using UnityEngine;
using System.Collections;

public class SimpleAvatarCam : MonoBehaviour {
	public Camera cam = null;
	public Transform avatarNode = null;
	
	public Vector3 targetPosAdjust = Vector3.up;
	public float delta = 1.0f;
	
	public string avatarPrefabPath = "";
	public AvatarController avatar = null;
	
	public bool bCreateOnStart = false;
	public void Start()
	{
		if (bCreateOnStart == true && avatarPrefabPath != "")
		{
			avatar = ResourceManager.CreatePrefab<AvatarController>(avatarPrefabPath, avatarNode, Vector3.zero);
		}
	}
	
	// Update is called once per frame
	public virtual void Update () {
		if (cam != null)
		{
			Vector3 targetPos = avatarNode.position;
			targetPos += targetPosAdjust * delta;
			
			cam.transform.LookAt(targetPos);
		}
	}
	
	
}
