using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

public class WayPointManager : MonoBehaviour {
	public List<WayPoint> wayPointList = new List<WayPoint>();
	public BoxCollider ownerGround = null;
	
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void AddWayPoint(WayPoint newWayPoint)
	{
		wayPointList.Add(newWayPoint);
	}
	
	public WayPoint AddEmptyWayPoint()
	{
		GameObject newWayPointObj = new GameObject("WayPoint");
		newWayPointObj.transform.parent = this.gameObject.transform;
		newWayPointObj.transform.localPosition = new Vector3(0.0f, 1.0f, 0.0f);
		
		WayPoint wayPoint = newWayPointObj.AddComponent<WayPoint>();
		
		/*BoxCollider area = newWayPointObj.AddComponent<BoxCollider>();
		
		wayPoint.area = area;
		area.size = new Vector3(1.0f, 1.0f, 10.0f);
		
		int defaultWayTypeValue = 1 << (int)WayPoint.eWayType.Walk;
		wayPoint.wayTypeMask = defaultWayTypeValue;
		*/
		
		AddWayPoint(wayPoint);
		
		return wayPoint;
	}
	
	public void RemoveWayPoint(WayPoint wayPoint)
	{
		wayPointList.Remove(wayPoint);
		DestroyImmediate(wayPoint.gameObject);
	}
	
#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		Color origColor = Gizmos.color;
		
		Color green = Color.green;
		green.a = 0.5f;
		
		Gizmos.color = green;
		if (ownerGround != null)
		{
			Bounds areaBounds = ownerGround.bounds;
			
			Gizmos.DrawWireCube(areaBounds.center, ownerGround.size);
		}
		
		Gizmos.color = origColor;
		
		Handles.Label(transform.position, this.name);
	}
#endif
	
	public void FindPath(WayPointManager target, Path path, int wayTypeMask)
	{
		path.findPath = false;
		path.pathList.Clear();
		
		path.target = target;
		path.wayTypeMask = wayTypeMask;
		
		//Debug.Log("Start.......");
		
		List<WayPointManager> searchList = new List<WayPointManager>();
		
		if (this == target)
		{
			path.findPath = true;
			return;
		}
		
		//Debug.Log("SearchList Add..." + this.name);
		
		//Debug.Log("Function call....");
		
		int nCount = wayPointList.Count;
		Path[] paths = new Path[nCount];
		
		//WayPointManager startWayPoint = null;
		
		for (int i = 0; i < nCount; ++i)
		{
			searchList.Clear();
			searchList.Add(this);
			
			Path tempPath = new Path();
			paths[i] = tempPath;
			
			WayPoint start = wayPointList[i];
			
			if (start.Check(wayTypeMask) == false)
				continue;
			
			tempPath.pathList.Add(start);
			searchList.Add(start.target);
			
			if (target == start.target)
			{
				//Debug.Log("Find....");
				
				tempPath.findPath = true;
			}
			else
			{
				tempPath.findPath = start.target.FindPath(target, paths[i], wayTypeMask, searchList);				
			}
			
			//Debug.Log(i + " Path Finding...." + tempPath.findPath + " pathList : " + tempPath.pathList.Count);
		}
		
		
		int selectedIndex = -1;
		int pathCount = int.MaxValue;
		for (int i = 0; i < nCount; ++i)
		{
			Path tempPath = paths[i];
			if (tempPath == null)
				continue;
			
			if (tempPath.findPath == false)
				continue;
			
			if (tempPath.pathList.Count < pathCount)
			{
				pathCount = tempPath.pathList.Count;
				selectedIndex = i;
				
				//Debug.Log("PathCount ... " + pathCount);
			}
		}
		
		if (selectedIndex != -1)
		{
			path.findPath = true;
			path.pathList.AddRange(paths[selectedIndex].pathList);
		}
		else
		{
			path.findPath = false;
			path.pathList.Clear();
		}
		
		//FindPath(target, path, wayTypeMask, searchList);
	}
	
	public bool FindPath(WayPointManager target, Path path, int wayTypeMask, List<WayPointManager> searchList)
	{
		foreach(WayPoint wayPoint in wayPointList)
		{
			if (wayPoint.target == target)
			{
				//Debug.Log("Target Find...");
				path.pathList.Add(wayPoint);
				return true;
			}
			
			if (searchList.Contains(wayPoint.target) == true)
			{
				//Debug.Log("SearchList contain .... " + wayPoint.target.name);
				continue;
			}
			
			//Debug.Log("Add SearchList... " + wayPoint.target.name);
			searchList.Add(wayPoint.target);
			
			if (wayPoint.Check(wayTypeMask) == false)
			{
				//Debug.Log("Check failed....");
				continue;
			}
			
			//Debug.Log("Add Path...." + wayPoint.name);
			path.pathList.Add(wayPoint);
			
			if (wayPoint.target.FindPath(target, path, wayTypeMask, searchList) == false)
			{
				path.pathList.Remove(wayPoint);
				
				//Debug.Log("Find failed, Remove Path.. " + wayPoint.name);
			}
			else
			{
				
				return true;
			}
		}
		
		return false;
	}
}
