using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(WayPointManager))]

public class WayPointManagerInspector : Editor {

	public override void OnInspectorGUI()
	{
		WayPointManager wayPointManager = target as WayPointManager;
		
		string[] wayTypeOptions = {"Walk", "Jump"};
		
		EditorGUILayout.BeginVertical ();
		
		List<GameObject> groundList = new List<GameObject>();
		StageManager stageManager = FindObjectOfType(typeof(StageManager)) as StageManager;
		if (stageManager != null)
			stageManager.FindGrounds(groundList);
		
		string[] targetGroundNameList = new string[groundList.Count];
		int index = 0;
		foreach(GameObject obj in groundList)
		{
			targetGroundNameList[index++] = obj.name;
		}
		
		string[] wayPointMoveDir = {"Right", "Left", "Up"};
		
		if (GUILayout.Button("Add") == true)
		{
			wayPointManager.AddEmptyWayPoint();
		}
		
		wayPointManager.ownerGround = (BoxCollider)EditorGUILayout.ObjectField(wayPointManager.ownerGround, typeof(BoxCollider)) as BoxCollider;
		
		int targetIndex = -1;
		List<WayPoint> wayPointList = wayPointManager.wayPointList;
		foreach(WayPoint wayPoint in wayPointList)
		{
			EditorGUILayout.BeginHorizontal();
			
			if (wayPoint == null)
			{
				wayPointList.Remove(wayPoint);
				break;
			}
			
			targetIndex = -1;
			index = 0;
			foreach(GameObject obj in groundList)
			{
				WayPointManager wayPointMgr = obj.GetComponent<WayPointManager>();
				
				if (wayPointMgr == wayPoint.target)
					targetIndex = index;
				
				index++;
			}
			
			targetIndex = EditorGUILayout.Popup(targetIndex, targetGroundNameList);
			if (GUI.changed)
			{
				if (targetIndex != -1)
				{
					GameObject obj = groundList[targetIndex];
					WayPointManager wayPointMgr = obj.GetComponent<WayPointManager>();
					wayPoint.target = wayPointMgr;
				}
				
				EditorUtility.SetDirty(target);
			}
			
			if (GUILayout.Button("A") == true)
			{
				GameObject obj = new GameObject("WayInfo");
				obj.transform.parent = wayPoint.transform;
				obj.transform.localPosition = Vector3.zero;
				
				WayInfo newWayInfo = obj.AddComponent<WayInfo>();
				BoxCollider area = obj.AddComponent<BoxCollider>();
				newWayInfo.area = area;
				
				wayPoint.AddWayInfo(newWayInfo);
				EditorUtility.SetDirty(target);
				break;
			}
			
			//GUILayout.Button(wayPoint.area.name, GUILayout.Width(90));
			EditorGUILayout.BeginVertical();
			
			
			foreach(WayInfo wayInfo in wayPoint.wayInfoList)
			{
				EditorGUILayout.BeginHorizontal();
				
				Vector3 areaOrigPos = wayInfo.area.transform.position;
				float areaPos = areaOrigPos.x;
				areaPos = EditorGUILayout.FloatField("AreaPos", areaPos);
				if (GUI.changed)
				{
					areaOrigPos.x = areaPos;
					
					wayInfo.area.transform.position = areaOrigPos;
					EditorUtility.SetDirty(target);
				}
				
				Vector3 areaOrigSize = wayInfo.area.size;
				float areaSize = areaOrigSize.x;
				areaSize = EditorGUILayout.FloatField("AreaSize", areaSize);
				if (GUI.changed)
				{
					areaOrigSize.x = Mathf.Max(1.0f, areaSize);
					
					wayInfo.area.size = areaOrigSize;
					EditorUtility.SetDirty(target);
				}
				
				int moveDirIndex = 0;
				if (wayInfo.vDir == Vector3.right)
					moveDirIndex = 0;
				else if (wayInfo.vDir == Vector3.left)
					moveDirIndex = 1;
				else if (wayInfo.vDir == Vector3.up)
					moveDirIndex = 2;
				
				moveDirIndex = EditorGUILayout.Popup(moveDirIndex, wayPointMoveDir);
				if (GUI.changed)
				{
					if (moveDirIndex == 0)
						wayInfo.vDir = Vector3.right;
					else if (moveDirIndex == 1)
						wayInfo.vDir = Vector3.left;
					else if (moveDirIndex == 2)
						wayInfo.vDir = Vector3.up;
				}
				
				wayInfo.wayTypeMask = EditorGUILayout.MaskField(wayInfo.wayTypeMask, wayTypeOptions);
				if (GUI.changed)
				{
					EditorUtility.SetDirty(target);
				}
				
				if (GUILayout.Button("D") == true)
				{
					wayPoint.RemoveWayInfo(wayInfo);
					EditorUtility.SetDirty(target);
					break;
				}
				
				EditorGUILayout.EndHorizontal();
			}
			
			EditorGUILayout.EndVertical();
			
			if (GUILayout.Button("R") == true)
			{
				wayPointManager.RemoveWayPoint(wayPoint);
				EditorUtility.SetDirty(target);
				break;
			}
			
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndVertical();
	}
}