using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(MonsterGenerator))]

public class MonsterGeneratorInspector : Editor
{
	static bool bFoldout = false;
	
	public override void OnInspectorGUI()
	{
		MonsterGenerator obj = target as MonsterGenerator;
		
		List<string> layerNames = new List<string>();
		for (int index = 0; index < 32; ++index)
		{
			string layerName = LayerMask.LayerToName(index);
			if (index < 8 && layerName.Length < 1)
				layerName = "BuiltinLayer " + index;
			
			if (layerName.Length > 0)
				layerNames.Add(layerName);
		}
		
		string[] layerNamesArray = layerNames.ToArray();
		
		
		EditorGUILayout.BeginVertical();
		
		obj.triggerLength = EditorGUILayout.FloatField("TriggerLength", obj.triggerLength);
		if (GUI.changed == true)
		{
			obj.UpdateData();
			EditorUtility.SetDirty(target);
		}
		
		obj.isTriggerMode = EditorGUILayout.Toggle("IsTriggerMode", obj.isTriggerMode);
		if (GUI.changed == true)
		{
			obj.UpdateData();
			EditorUtility.SetDirty(target);
		}
		
		obj.triggerLayerMask = EditorGUILayout.MaskField("TriggerMask", obj.triggerLayerMask, layerNamesArray, EditorStyles.layerMaskField);
		if (GUI.changed == true)
		{
			EditorUtility.SetDirty(target);
		}
		
		obj.genCoolTime = EditorGUILayout.FloatField("CoolTime", obj.genCoolTime);
		if (GUI.changed == true)
		{
			EditorUtility.SetDirty(target);
		}
		
		obj.subLimitCount = EditorGUILayout.IntField("SubLimitCount", obj.subLimitCount);
		if (GUI.changed == true)
		{
			EditorUtility.SetDirty(target);
		}
		
		obj.totalCount = EditorGUILayout.IntField("TotalCount", obj.totalCount);
		if (GUI.changed == true)
		{
			EditorUtility.SetDirty(target);
		}
		
		obj.monsterLinkID = EditorGUILayout.IntField("BossRaidID", obj.monsterLinkID);
		if (GUI.changed == true)
		{
			EditorUtility.SetDirty(target);
		}
		
		obj.genTargetPos = (GeneratorTarget)EditorGUILayout.ObjectField("GeneratorTarget", obj.genTargetPos, typeof(GeneratorTarget), true);
		if (GUI.changed == true)
		{
			EditorUtility.SetDirty(target);
		}
		
		obj.genPosPickLayerMask = EditorGUILayout.MaskField("GenPosMask", obj.genPosPickLayerMask, layerNamesArray, EditorStyles.layerMaskField);
		if (GUI.changed == true)
		{
			EditorUtility.SetDirty(target);
		}
		
		EditorGUILayout.BeginHorizontal();
		bFoldout = EditorGUILayout.Foldout(bFoldout, "GenerateInfos");
		EditorGUILayout.EndHorizontal();
		
		if (bFoldout == true)
		{
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Add") == true)
			{
				obj.generatorInfos.Add(new GeneratorInfo());
				EditorUtility.SetDirty(target);
			}
			EditorGUILayout.EndHorizontal();
			
			int nCount = obj.generatorInfos.Count;
			for (int index = 0; index < nCount; ++index)
			{
				EditorGUILayout.BeginHorizontal();
			
				GeneratorInfo info = obj.generatorInfos[index];
				if (GUILayout.Button("Remove") == true)
				{
					obj.generatorInfos.RemoveAt(index);
					EditorUtility.SetDirty(target);
					break;
				}
				
				info.monster = (GameObject)EditorGUILayout.ObjectField(info.monster, typeof(GameObject), true);
				if (GUI.changed == true)
					EditorUtility.SetDirty(target);
				
				
				EditorGUILayout.EndHorizontal();
			}
		}
		
		EditorGUILayout.EndVertical();
	}
}
