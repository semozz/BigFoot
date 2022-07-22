using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;


public class Path
{
	public WayPointManager target = null;
	public List<WayPoint> pathList = new List<WayPoint>();
	
	public int wayTypeMask = 0;
	
	public bool findPath = false;
	
	public bool CheckPath(WayPointManager node)
	{
		/*
		foreach(WayPoint wayPoint in pathList)
		{
			if (wayPoint.target == node)
			{
				return true;
			}
		}
		*/
		
		
		if (node == null)
			return false;
		
		while(pathList.Count > 0)
		{
			WayPoint wayPoint = pathList[0];
			if (wayPoint == null)
			{
				pathList.RemoveAt(0);
				continue;
			}
			
			if (wayPoint.transform.parent.name == node.name)
				return true;
			else
				pathList.RemoveAt(0);
		}
		
		return false;
	}
	
	public void DebugInfo()
	{
		Debug.Log("Result.....");
		foreach(WayPoint wayPoint in pathList)
		{
			Debug.Log(wayPoint.transform.parent.name + " to " + wayPoint.target.name);
		}
		Debug.Log("End....");
	}
}

[System.Serializable]
public class WayPoint : MonoBehaviour {
	
	public enum eWayType{
		Walk,
		Jump
	}

	public WayPointManager target = null;
	public List<WayInfo> wayInfoList = new List<WayInfo>();
	
	public void AddWayInfo(WayInfo info)
	{
		wayInfoList.Add(info);
	}
	
	public void RemoveWayInfo(WayInfo info)
	{
		wayInfoList.Remove(info);
		
		DestroyImmediate(info.gameObject);
	}

#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		Vector3 areaCenter = Vector3.zero;
		Vector3 targetCenter = Vector3.zero;
		
		Color oricolor = Gizmos.color;
		
		
		BoxCollider targetCollider = null;
		if (target != null)
		{
			targetCollider = target.GetComponent<BoxCollider>();
			
			if (targetCollider != null)
			{
				Bounds targetBounds = targetCollider.bounds;
				targetCenter = targetBounds.center;
				
				//Gizmos.DrawWireCube(targetBounds.center, targetBounds.extents);
			}
		}
		
		foreach(WayInfo info in wayInfoList)
		{
			Gizmos.color = Color.blue;
			if (info.area != null)
			{
				Bounds areaBounds = info.area.bounds;
				areaCenter = areaBounds.center;
				
				Gizmos.DrawWireCube(areaBounds.center, info.area.size);
			}
			
			
			Gizmos.color = Color.red;
			Vector3 delta = Vector3.zero;
			
			Gizmos.DrawLine(areaCenter, areaCenter + info.vDir);
			
			if (info.vDir == Vector3.right)
			{
				delta = new Vector3(-0.2f, -0.2f, 0.0f);
				Gizmos.DrawLine(areaCenter + info.vDir, areaCenter + info.vDir + delta);
				delta = new Vector3(-0.2f, 0.2f, 0.0f);
				Gizmos.DrawLine(areaCenter + info.vDir, areaCenter + info.vDir + delta);
			}
			else if (info.vDir == Vector3.left)
			{
				delta = new Vector3(-0.2f, -0.2f, 0.0f);
				Gizmos.DrawLine(areaCenter + info.vDir, areaCenter + info.vDir - delta);
				delta = new Vector3(-0.2f, 0.2f, 0.0f);
				Gizmos.DrawLine(areaCenter + info.vDir, areaCenter + info.vDir - delta);
			}
			else if (info.vDir == Vector3.up)
			{
				delta = new Vector3( -0.2f, -0.2f, 0.0f);
				Gizmos.DrawLine(areaCenter + info.vDir, areaCenter + info.vDir + delta);
				delta = new Vector3( 0.2f, -0.2f, 0.0f);
				Gizmos.DrawLine(areaCenter + info.vDir, areaCenter + info.vDir + delta);
			}
			
			Gizmos.color = Color.yellow;
			if (info.area != null && target != null)
			{
				Gizmos.DrawLine(areaCenter, targetCenter);
			}
			
			
			Gizmos.color = oricolor;
			
			string wayTypeMsg = "";
			List<string> msg = new List<string>();
			int tempValue = 1 << (int)eWayType.Walk;
			
			if ((info.wayTypeMask & tempValue) == tempValue)
				msg.Add("Walk");
			
			tempValue = 1 << (int)eWayType.Jump;
			if ((info.wayTypeMask & tempValue) == tempValue)
				msg.Add("Jump");
			
			for (int i = 0; i < msg.Count; ++i)
			{
				if (i > 0)
					wayTypeMsg += ",";
				
				wayTypeMsg += msg[i];
				
			}
			
			Handles.Label(info.transform.position, wayTypeMsg);
		}
	}
#endif
	
	public bool Check (int wayTypeMask)
	{
		bool available = true;
		
		bool hasJump = (wayTypeMask & 1 << (int)WayPoint.eWayType.Jump) != 0;
		if (hasJump == false)
			return false;
		
		foreach(WayInfo info in wayInfoList)
		{
			if ((info.wayTypeMask & wayTypeMask) == 0 )
			{
				available = false;
				break;
			}
		}
		return available;
	}
}
