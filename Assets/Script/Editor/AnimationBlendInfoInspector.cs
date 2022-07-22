using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(AnimationBlendInfo))]

class AnimationBlendInfoInspector : Editor {
	
	Vector2 scrollPos = new Vector2(0, 0);
	
	int nSelectedRowIndex = -1;
	int nSelectedColIndex = -1;
	
	Color SelectedRowColor = Color.green;
	Color SelectedColColor = Color.yellow;
	
	public override void OnInspectorGUI()
    {
        AnimationBlendInfo obj = target as AnimationBlendInfo;
		
		obj.rebuildKeys();
		int nCount = obj.GetAnimationClipCount();
				
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
		EditorGUILayout.BeginVertical ();
		string labelName = "None";
		
		for (int nRowIndex = -1; nRowIndex < nCount; ++nRowIndex)
		{
			EditorGUILayout.BeginHorizontal();
			
			for (int nColIndex = -1; nColIndex < nCount; ++nColIndex)
			{
				if (nRowIndex == -1 && nColIndex == -1)
				{
					labelName = "None";
					//EditorGUILayout.SelectableLabel(labelName, GUILayout.Width (100));
					GUILayout.Button(labelName, GUILayout.Width (100));
					//continue;
				}
				else if (nRowIndex == -1 && nColIndex > -1)
				{
					labelName = obj.GetAnimationName(nColIndex);
					//EditorGUILayout.SelectableLabel(labelName);
					
					if (nColIndex == nSelectedColIndex)
						GUI.contentColor = SelectedColColor;
					else
						GUI.contentColor = Color.white;
					
					if (GUILayout.Button(labelName, GUILayout.Width (100)))
						nSelectedColIndex = nColIndex;
					
					GUI.contentColor = Color.white;
					//continue;
				}
				else if (nRowIndex > -1 && nColIndex == -1)
				{
					labelName = obj.GetAnimationName(nRowIndex);
					//EditorGUILayout.SelectableLabel(labelName);
					if (nRowIndex == nSelectedRowIndex)
						GUI.contentColor = SelectedRowColor;
					else
						GUI.contentColor = Color.white;
					
					if (GUILayout.Button(labelName, GUILayout.Width (100)))
						nSelectedRowIndex = nRowIndex;
					
					GUI.contentColor = Color.white;
					//continue;
				}
				else
				{
					if (nRowIndex == nColIndex)
					{
						GUI.contentColor = Color.grey;
						
						labelName = "unable";
						//EditorGUILayout.SelectableLabel(labelName);
						GUILayout.Button(labelName, GUILayout.Width (100));
						//continue;
						
						GUI.contentColor = Color.white;
					}
					else
					{
						BlendInfo info = obj.GetBlendInfo(nRowIndex, nColIndex);
					
						if (info == null)
							continue;
						
						if (nColIndex == nSelectedColIndex && nRowIndex == nSelectedRowIndex)
							GUI.contentColor = Color.red;
						else if (nColIndex == nSelectedColIndex)
							GUI.contentColor = SelectedColColor;
						else if (nRowIndex == nSelectedRowIndex)
							GUI.contentColor = SelectedRowColor;
						else
							GUI.contentColor = Color.white;
						
						//info.fBlendTime = EditorGUILayout.Slider(info.fBlendTime, 0.0f, 1.0f, GUILayout.Width (100));
						info.fBlendTime = EditorGUILayout.FloatField(info.fBlendTime, GUILayout.Width (100));
						
						GUI.contentColor = Color.white;
						
						if (GUI.changed)
						{
							EditorUtility.SetDirty(target);
							break;
						}
					}
				}				
			}
			EditorGUILayout.EndHorizontal();
			
			//EditorGUILayout.EndVertical();
		}
		EditorGUILayout.EndVertical();
		
		EditorGUILayout.EndScrollView();
	}
}
