using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(StateList))]

class StateManagerInspector : Editor 
{

	static bool bFoldout = false;

    public override void OnInspectorGUI()
    {
        StateList obj = target as StateList;
		
		EditorGUILayout.BeginVertical ();
		
	        EditorGUILayout.BeginHorizontal();
	        bFoldout = EditorGUILayout.Foldout(bFoldout, "StateManager");
	        EditorGUILayout.EndHorizontal();
		
			obj.animObj = (Animation)EditorGUILayout.ObjectField(obj.animObj, typeof(Animation), true);
			if (GUI.changed)
			{
				EditorUtility.SetDirty(target);			
			}
		
			Animation anim = obj.animObj;
			int animCount = 1;
			
			if (anim != null)
				animCount += anim.GetClipCount();
			
			string[] animList = new string[animCount];
			animList[0] = "None";
			
			if (anim != null)
			{
				int counter = 1;
				foreach(AnimationState animState in anim)
				{
					animList[counter++] = animState.name;
				}
			}
			
			int animIndex = 0;
			
	        if (bFoldout == true)
	        {
	            EditorGUILayout.BeginHorizontal();
	            if (GUILayout.Button("Add State") == true)
	            {
	                obj.AddNewState(new CharStateInfo());
	                EditorUtility.SetDirty(target);
	            }
	            EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginVertical();
				foreach(CharStateInfo info in obj.stateList)
				{
					EditorGUILayout.BeginVertical();
						EditorGUILayout.BeginHorizontal();
						
						if (GUILayout.Button("Remove State", GUILayout.Width(200)) == true)
						{
							obj.RemoveState(info);
							
							EditorUtility.SetDirty(target);
							
							EditorGUILayout.EndHorizontal();
							break;
						}
						
						EditorGUILayout.EndHorizontal();
					EditorGUILayout.EndVertical();
						
					
					EditorGUILayout.BeginVertical();
						EditorGUILayout.BeginHorizontal();
						
						Color oldColor = GUI.contentColor;
						GUI.contentColor = Color.yellow;
						BaseState state = info.baseState;
						state.state = (BaseState.eState)EditorGUILayout.EnumPopup(state.state, GUILayout.Width(100)); 
						
						GUI.contentColor = oldColor;
						if (GUI.changed) EditorUtility.SetDirty(target);
						
						for (int index = 0; index < animCount; ++index)
						{
							if (state.animationClip == animList[index])
							{
								animIndex = index;
								break;
							}
						}
						
						animIndex = EditorGUILayout.Popup("AnimationClip", animIndex, animList, EditorStyles.popup);
						if (GUI.changed)
						{
							state.animationClip = animList[animIndex];
							EditorUtility.SetDirty(target);
						}
						
						EditorGUILayout.EndHorizontal();
					
					
						/////////////////////////////////////////////////////////////////////////////////////
						EditorGUILayout.BeginVertical();
							StateInfo charStateInfo = info.stateInfo;
							
							info.patienceFactor = EditorGUILayout.FloatField("PatienceFactor : ", info.patienceFactor, GUILayout.Width(250));
					
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField("State Base Info ------");
							/*
							charStateInfo.attackType = (StateInfo.eAttackType)EditorGUILayout.EnumPopup(charStateInfo.attackType);
							if (GUI.changed) EditorUtility.SetDirty(target);
							
							charStateInfo.attackState = (StateInfo.eAttackState)EditorGUILayout.EnumPopup(charStateInfo.attackState);
							if (GUI.changed) EditorUtility.SetDirty(target);
							*/
				
							charStateInfo.defenseState = (StateInfo.eDefenseState)EditorGUILayout.EnumPopup(charStateInfo.defenseState);
							if (GUI.changed) EditorUtility.SetDirty(target);
							
							EditorGUILayout.EndHorizontal();
				
							/*
							EditorGUILayout.BeginHorizontal();
							charStateInfo.attackRate = EditorGUILayout.FloatField("AttackRate", charStateInfo.attackRate);
							charStateInfo.painValue = EditorGUILayout.FloatField("PainValue", charStateInfo.painValue);
							EditorGUILayout.EndHorizontal();
				
							EditorGUILayout.BeginHorizontal();
							charStateInfo.knockDir = EditorGUILayout.Vector3Field("KnockDir", charStateInfo.knockDir);
							if (GUI.changed) EditorUtility.SetDirty(target);
							//charStateInfo.knockPower = EditorGUILayout.FloatField("KnockPower", charStateInfo.knockPower);
							EditorGUILayout.EndHorizontal();
							*/
				
							EditorGUILayout.BeginHorizontal();
							charStateInfo.fxObjectName = EditorGUILayout.TextField("FXObject", charStateInfo.fxObjectName);
							if (GUI.changed) EditorUtility.SetDirty(target);
							charStateInfo.effectType = (eFXEffectType)EditorGUILayout.EnumPopup(charStateInfo.effectType);
							if (GUI.changed) EditorUtility.SetDirty(target);
				
							charStateInfo.effectScale = EditorGUILayout.FloatField("Scale", charStateInfo.effectScale);
							if (GUI.changed) EditorUtility.SetDirty(target);
							EditorGUILayout.EndHorizontal();
				
							charStateInfo.acquireAbility = EditorGUILayout.FloatField("AcquireAbility", charStateInfo.acquireAbility);
							charStateInfo.requireAbilityValue = EditorGUILayout.FloatField("RequireAbility", charStateInfo.requireAbilityValue);
				
							info.chainAttack = EditorGUILayout.Toggle("ChainAttack : ", info.chainAttack, GUILayout.Width(250));
							if (GUI.changed) EditorUtility.SetDirty(target);
				
							info.cantChangeDir = EditorGUILayout.Toggle("Can'tChangeDir : ", info.cantChangeDir, GUILayout.Width(250));
							if (GUI.changed) EditorUtility.SetDirty(target);
				
							EditorGUILayout.BeginVertical();
							info.walkingEventMoveSpeed = EditorGUILayout.FloatField("WalkingMoveSpeed : ", info.walkingEventMoveSpeed, GUILayout.Width(250));
							if (GUI.changed) 
							{
								info.SetDefaultWalkingSpeed(info.walkingEventMoveSpeed);
								EditorUtility.SetDirty(target);
							}
							
							if (GUILayout.Button("Add Walking Speed") == true)
				            {
				                info.AddWalkingSpeed(0.0f);
				                EditorUtility.SetDirty(target);
				            }
							int walkingSpeedCount = info.walkingSpeedList.Count;
							for(int index = 0; index < walkingSpeedCount; ++index)
							{
								float moveSpeed = info.walkingSpeedList[index];
								if (GUILayout.Button("Remove", GUILayout.Width(200)) == true)
								{
									info.RemoveSpeed(index);
									EditorUtility.SetDirty(target);
									break;
								}
					
								moveSpeed = EditorGUILayout.FloatField("Walking Speed " + index, moveSpeed);
								if (GUI.changed)
									info.SetWalkingSpeed(moveSpeed, index);
							}
							EditorGUILayout.EndVertical();
				
							EditorGUILayout.BeginHorizontal();
							info.moveType = (CharStateInfo.eMoveType)EditorGUILayout.EnumPopup(info.moveType);
							if (GUI.changed) EditorUtility.SetDirty(target);				
							EditorGUILayout.EndHorizontal();
				
						EditorGUILayout.EndVertical();
						//EditorGUILayout.EndVertical();
						
						EditorGUILayout.BeginHorizontal();
						
							EditorGUILayout.BeginVertical();
								EditorGUILayout.LabelField("Attack Info ------");
								
								EditorGUILayout.BeginVertical();
					            if (GUILayout.Button("Add Attack Info") == true)
					            {
					                info.collisionInfoList.Add(new CollisionInfo());
					                EditorUtility.SetDirty(target);
					            }
					            EditorGUILayout.EndVertical();
								
								EditorGUILayout.BeginVertical();
								foreach(CollisionInfo colInfo in info.collisionInfoList)
								{
									EditorGUILayout.BeginVertical();
									if (GUILayout.Button("Remove Attack Info", GUILayout.Width(200)) == true)
									{
										info.collisionInfoList.Remove(colInfo);
										
										EditorUtility.SetDirty(target);
										break;
									}
									EditorGUILayout.EndVertical();
									
									oldColor = GUI.contentColor;
									GUI.contentColor = Color.green;
					
									EditorGUILayout.BeginVertical();
										
										EditorGUILayout.BeginHorizontal();
										EditorGUILayout.LabelField("State Base Info ------");
										
										colInfo.stateInfo.attackType = (StateInfo.eAttackType)EditorGUILayout.EnumPopup(colInfo.stateInfo.attackType);
										if (GUI.changed) EditorUtility.SetDirty(target);
										
										colInfo.stateInfo.attackState = (StateInfo.eAttackState)EditorGUILayout.EnumPopup(colInfo.stateInfo.attackState);
										if (GUI.changed) EditorUtility.SetDirty(target);
										
										colInfo.stateInfo.defenseState = info.stateInfo.defenseState;
										
										EditorGUILayout.EndHorizontal();
						
									EditorGUILayout.EndVertical();
						
									EditorGUILayout.BeginVertical();
										colInfo.colliderName = EditorGUILayout.TextField("Collider :", colInfo.colliderName);
										if (GUI.changed) EditorUtility.SetDirty(target);
									EditorGUILayout.EndVertical();
	
									EditorGUILayout.BeginVertical();
										colInfo.stateInfo.soundFile = EditorGUILayout.TextField("SoundFile", colInfo.stateInfo.soundFile);
										if (GUI.changed) EditorUtility.SetDirty(target);
									EditorGUILayout.EndVertical();
					
									EditorGUILayout.BeginVertical();
										colInfo.stateInfo.isWeaponAttack = EditorGUILayout.Toggle("WeaponAttack", colInfo.stateInfo.isWeaponAttack);
										if (GUI.changed) EditorUtility.SetDirty(target);
									EditorGUILayout.EndVertical();
					
									EditorGUILayout.BeginVertical();
										EditorGUILayout.BeginHorizontal();
										
										colInfo.stateInfo.attackRate = EditorGUILayout.FloatField("AttackRate", colInfo.stateInfo.attackRate);
										if (GUI.changed) EditorUtility.SetDirty(target);
										colInfo.stateInfo.painValue = EditorGUILayout.FloatField("PainValue", colInfo.stateInfo.painValue);
										if (GUI.changed) EditorUtility.SetDirty(target);
										EditorGUILayout.EndHorizontal();
					
										EditorGUILayout.BeginHorizontal();
										colInfo.stateInfo.stunRate = EditorGUILayout.FloatField("StunRate", colInfo.stateInfo.stunRate);
										if (GUI.changed) EditorUtility.SetDirty(target);
					
										colInfo.stateInfo.stunTime = EditorGUILayout.FloatField("StunTime", colInfo.stateInfo.stunTime);
										if (GUI.changed) EditorUtility.SetDirty(target);
										EditorGUILayout.EndHorizontal();
					
										EditorGUILayout.BeginHorizontal();
										colInfo.stateInfo.knockDir = EditorGUILayout.Vector3Field("KnockDir_Normal", colInfo.stateInfo.knockDir);
										if (GUI.changed) EditorUtility.SetDirty(target);
										
										colInfo.stateInfo.knockDir_Air = EditorGUILayout.Vector3Field("KnockDir_Air", colInfo.stateInfo.knockDir_Air);
										if (GUI.changed) EditorUtility.SetDirty(target);
										//colInfo.stateInfo.knockPower = EditorGUILayout.FloatField("KnockPower", colInfo.stateInfo.knockPower);
										EditorGUILayout.EndHorizontal();
					
										EditorGUILayout.BeginHorizontal();
										colInfo.stateInfo.fxObjectName = EditorGUILayout.TextField("FXObject", colInfo.stateInfo.fxObjectName);
										if (GUI.changed) EditorUtility.SetDirty(target);
										colInfo.stateInfo.effectType = (eFXEffectType)EditorGUILayout.EnumPopup(colInfo.stateInfo.effectType);
										if (GUI.changed) EditorUtility.SetDirty(target);
										colInfo.stateInfo.effectScale = EditorGUILayout.FloatField("Scale", colInfo.stateInfo.effectScale);
										if (GUI.changed) EditorUtility.SetDirty(target);
										
										EditorGUILayout.EndHorizontal();
										
						
									EditorGUILayout.EndVertical();
									GUI.contentColor = oldColor;
								}
								EditorGUILayout.EndVertical();
								EditorGUILayout.LabelField("----------------------------------------------------------");
								EditorGUILayout.Separator();
				
							EditorGUILayout.EndVertical();
						
						EditorGUILayout.EndHorizontal();
	
					EditorGUILayout.EndVertical();				
				}
				EditorGUILayout.EndVertical();
			}

		EditorGUILayout.EndVertical ();
    }
}
