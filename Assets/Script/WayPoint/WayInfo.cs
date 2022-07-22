using UnityEngine;
using System.Collections;

[System.Serializable]
public class WayInfo : MonoBehaviour
{
	public BoxCollider area = null;
	public int wayTypeMask = 0;
	
	public Vector3 vDir = Vector3.right;
}
