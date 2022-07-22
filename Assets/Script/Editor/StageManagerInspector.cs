using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(StageManager))]
class StageManagerInspector : Editor
{
    private static bool bShowNearStage = false;
    private static bool bShowFrontStage = false;
    private static bool bShowFarStage = false;
    private static bool bShowGroundInfo = true;
	private static bool bShowFloorInfo = false;
	private static bool bShowMiddleStage = false;
	
	//private static bool bShowPathInfo = false;
    
	private static Vector2 vNearScrollPos = Vector2.zero;
    private static Vector2 vFrontScrollPos = Vector2.zero;
	private static Vector2 vFarScrollPos = Vector2.zero;
	private static Vector2 vMiddleScrollPos = Vector2.zero;
	
	//private static Vector2 vPathScrollPos = Vector2.zero;
	
    public override void OnInspectorGUI()
    {
        StageManager obj = target as StageManager;
		
		EditorGUILayout.BeginVertical ();
		
		obj.uiRootPrefab = EditorGUILayout.TextField("UI Root", obj.uiRootPrefab);
		
        obj.ScreenWidth = EditorGUILayout.FloatField ("ScreenWidth", obj.ScreenWidth);
		if (GUI.changed)
		{
			obj.UpdateData ();
			EditorUtility.SetDirty (target);
		}
		obj.ScreenHeight = EditorGUILayout.FloatField ("ScreenHeight", obj.ScreenHeight);
		if (GUI.changed)
		{
			obj.UpdateData ();
			EditorUtility.SetDirty (target);
		}

        obj.StageType = (StageManager.eStageType)EditorGUILayout.EnumPopup("StageType", obj.StageType, GUILayout.Width(200));
        if (GUI.changed)
            EditorUtility.SetDirty(target);
		
		obj.dayMode = (StageManager.eDayMode)EditorGUILayout.EnumPopup("DayMode", obj.dayMode, GUILayout.Width(200));
        if (GUI.changed)
            EditorUtility.SetDirty(target);
		obj.dayColor = EditorGUILayout.ColorField("DayColor", obj.dayColor);
		 if (GUI.changed)
            EditorUtility.SetDirty(target);
		obj.nightColor = EditorGUILayout.ColorField("NightColor", obj.nightColor);
		 if (GUI.changed)
            EditorUtility.SetDirty(target);
		
		obj.stageBlockWidth = EditorGUILayout.IntField("BlockWidth", obj.stageBlockWidth);
        if (GUI.changed)
        {
            obj.UpdateData();
            EditorUtility.SetDirty(target);
        }
        obj.stageBlockHeight = EditorGUILayout.IntField("BlockHeight", obj.stageBlockHeight);
        if (GUI.changed)
        {
            obj.UpdateData();
            EditorUtility.SetDirty(target);
        }

        EditorGUILayout.Separator();
        bShowNearStage = EditorGUILayout.Foldout(bShowNearStage, "Near Stage");
        if (bShowNearStage == true)
        {
            EditorGUI.indentLevel++;
            obj.NearStageWBlockCount = EditorGUILayout.IntField("WBlockCount", obj.NearStageWBlockCount);
            if (GUI.changed)
            {
                obj.UpdateData();
                EditorUtility.SetDirty(target);
            }
            obj.NearStageHBlockCount = EditorGUILayout.IntField("HBlockCount", obj.NearStageHBlockCount);
            if (GUI.changed)
            {
                obj.UpdateData();
                EditorUtility.SetDirty(target);
            }
			
            EditorGUILayout.LabelField("Width", obj.NearStageWidth.ToString());
            EditorGUILayout.LabelField("Height", obj.NearStageHeight.ToString());

            EditorGUILayout.BeginHorizontal();
            obj.NearStageScrollHeight = EditorGUILayout.FloatField("ScrollHeight", obj.NearStageScrollHeight, GUILayout.Width(200));
            if (GUI.changed)
                EditorUtility.SetDirty(target);

            if (GUILayout.Button("Copy", GUILayout.Width(80)) == true)
            {
                obj.NearStageScrollHeight = obj.NearStageHeight;
                EditorUtility.SetDirty(target);
            }
            EditorGUILayout.EndHorizontal();

            int oldcount = obj.NearStageTextures.Count;
            int newcount = obj.NearStageWBlockCount * obj.NearStageHBlockCount;
            if (newcount != oldcount && newcount >= 0)
            {
                obj.SetNearStageTextures(newcount);
                obj.UpdateData();
                EditorUtility.SetDirty(target);
            }

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel("Textures");

            vNearScrollPos = EditorGUILayout.BeginScrollView(vNearScrollPos, true, false, GUILayout.Height(obj.NearStageHBlockCount * 80));
            List<Texture> listTex = null;

            listTex = obj.NearStageTextures;
            for (int i = 0; i < obj.NearStageHBlockCount; i++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < obj.NearStageWBlockCount; j++)
                {
                    int npos = (i * obj.NearStageWBlockCount) + j;
                    listTex[npos] = EditorGUILayout.ObjectField(listTex[npos], typeof(Texture), true, GUILayout.Width(64), GUILayout.Height(64)) as Texture;
                    if (GUI.changed)
                    {
                        obj.UpdateData();
                        EditorUtility.SetDirty(target);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Separator();
        bShowFrontStage = EditorGUILayout.Foldout(bShowFrontStage, "Front Stage");
        if (bShowFrontStage == true)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel("Textures");
            vFrontScrollPos = EditorGUILayout.BeginScrollView(vFrontScrollPos, true, false, GUILayout.Height(obj.NearStageHBlockCount * 80));

            List <Texture> listTex = obj.FrontStageTextures;
            for (int i = 0; i < obj.NearStageHBlockCount; i++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < obj.NearStageWBlockCount; j++)
                {
                    int npos = (i * obj.NearStageWBlockCount) + j;
                    if (npos < listTex.Count)
                    {
                        listTex[npos] = EditorGUILayout.ObjectField(listTex[npos], typeof(Texture), true, GUILayout.Width(64), GUILayout.Height(64)) as Texture;
                        if (GUI.changed)
                        {
                            obj.UpdateData();
                            EditorUtility.SetDirty(target);
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }
		
		EditorGUILayout.Separator();
        bShowMiddleStage = EditorGUILayout.Foldout(bShowMiddleStage, "Middle Stage");
        if (bShowMiddleStage == true)
        {
            EditorGUI.indentLevel++;

            obj.MiddleStageWBlockCount = EditorGUILayout.IntField("WBlockCount", obj.MiddleStageWBlockCount);
            if (GUI.changed)
            {
                obj.UpdateData();
                EditorUtility.SetDirty(target);
            }
            obj.MiddleStageHBlockCount = EditorGUILayout.IntField("HBlockCount", obj.MiddleStageHBlockCount);
            if (GUI.changed)
            {
                obj.UpdateData();
                EditorUtility.SetDirty(target);
            }
			
			obj.middleBlockWidth = EditorGUILayout.IntField("BlockWidth", obj.middleBlockWidth);
		    if (GUI.changed)
		    {
		        obj.UpdateData();
		        EditorUtility.SetDirty(target);
		    }
		    obj.middleBlockHeight = EditorGUILayout.IntField("BlockHeight", obj.middleBlockHeight);
		    if (GUI.changed)
		    {
		        obj.UpdateData();
		        EditorUtility.SetDirty(target);
		    }
			
			obj.middleStageVScrollRate = EditorGUILayout.FloatField("VScrollRate : ", obj.middleStageVScrollRate);
			if (GUI.changed)
			{
				obj.UpdateData();
				EditorUtility.SetDirty(target);
			}
				
            EditorGUILayout.LabelField("Width", obj.MiddleStageWidth.ToString());
            EditorGUILayout.LabelField("Height", obj.MiddleStageHeight.ToString());

            int oldcount = obj.MiddleStageTextures.Count;
            int newcount = obj.MiddleStageWBlockCount * obj.MiddleStageHBlockCount;
            if (newcount != oldcount && newcount >= 0)
            {
                obj.SetMiddleStageTextures(newcount);
                obj.UpdateData();
                EditorUtility.SetDirty(target);
            }

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel("Textures");
            vMiddleScrollPos = EditorGUILayout.BeginScrollView(vMiddleScrollPos, true, false, GUILayout.Height(obj.MiddleStageHBlockCount * 80));

            List <Texture> listTex = obj.MiddleStageTextures;
            for (int i = 0; i < obj.MiddleStageHBlockCount; i++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < obj.MiddleStageWBlockCount; j++)
                {
                    int npos = (i * obj.MiddleStageWBlockCount) + j;
                    listTex[npos] = EditorGUILayout.ObjectField(listTex[npos], typeof(Texture), true, GUILayout.Width(64), GUILayout.Height(64)) as Texture;
                    if (GUI.changed)
                    {
                        obj.UpdateData();
                        EditorUtility.SetDirty(target);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Separator();
        bShowFarStage = EditorGUILayout.Foldout(bShowFarStage, "Far Stage");
        if (bShowFarStage == true)
        {
            EditorGUI.indentLevel++;

            obj.FarStageWBlockCount = EditorGUILayout.IntField("WBlockCount", obj.FarStageWBlockCount);
            if (GUI.changed)
            {
                obj.UpdateData();
                EditorUtility.SetDirty(target);
            }
            obj.FarStageHBlockCount = EditorGUILayout.IntField("HBlockCount", obj.FarStageHBlockCount);
            if (GUI.changed)
            {
                obj.UpdateData();
                EditorUtility.SetDirty(target);
            }
			
			obj.farBlockWidth = EditorGUILayout.IntField("BlockWidth", obj.farBlockWidth);
		    if (GUI.changed)
		    {
		        obj.UpdateData();
		        EditorUtility.SetDirty(target);
		    }
		    obj.farBlockHeight = EditorGUILayout.IntField("BlockHeight", obj.farBlockHeight);
		    if (GUI.changed)
		    {
		        obj.UpdateData();
		        EditorUtility.SetDirty(target);
		    }
			
			obj.farStageVScrollRate = EditorGUILayout.FloatField("VScrollRate : ", obj.farStageVScrollRate);
			if (GUI.changed)
			{
				obj.UpdateData();
				EditorUtility.SetDirty(target);
			}
				
            EditorGUILayout.LabelField("Width", obj.FarStageWidth.ToString());
            EditorGUILayout.LabelField("Height", obj.FarStageHeight.ToString());

            int oldcount = obj.FarStageTextures.Count;
            int newcount = obj.FarStageWBlockCount * obj.FarStageHBlockCount;
            if (newcount != oldcount && newcount >= 0)
            {
                obj.SetFarStageTextures(newcount);
                obj.UpdateData();
                EditorUtility.SetDirty(target);
            }

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel("Textures");
            vFarScrollPos = EditorGUILayout.BeginScrollView(vFarScrollPos, true, false, GUILayout.Height(obj.FarStageHBlockCount * 80));

            List <Texture> listTex = obj.FarStageTextures;
            for (int i = 0; i < obj.FarStageHBlockCount; i++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < obj.FarStageWBlockCount; j++)
                {
                    int npos = (i * obj.FarStageWBlockCount) + j;
                    listTex[npos] = EditorGUILayout.ObjectField(listTex[npos], typeof(Texture), true, GUILayout.Width(64), GUILayout.Height(64)) as Texture;
                    if (GUI.changed)
                    {
                        obj.UpdateData();
                        EditorUtility.SetDirty(target);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Separator();
        bShowGroundInfo = EditorGUILayout.Foldout(bShowGroundInfo, "Ground");
        if (bShowGroundInfo)
        {
            EditorGUI.indentLevel++;
			
			obj.GroundLayer = EditorGUILayout.LayerField("GroundLayer", obj.GroundLayer, GUILayout.Width(240));
			if (GUI.changed)
            {
                obj.ChangeGroundLayer(obj.GroundLayer);
                EditorUtility.SetDirty(target);
				
				GUI.changed = false;
            }
			
			obj.groundOffset = EditorGUILayout.Vector3Field("GroundOffset ", obj.groundOffset);
			if (GUI.changed)
			{
				obj.UpdateGroundOffset();
				EditorUtility.SetDirty(target);
				
				GUI.changed = false;
			}
			
            EditorGUILayout.BeginHorizontal();
            //EditorGUILayout.PrefixLabel((i + 1) + "");
            obj.GroundCount = EditorGUILayout.IntField("Count", obj.GroundCount);
            if (GUI.changed)
            {
				obj.SetGroundInfos(obj.GroundCount);
                obj.UpdateData();
                EditorUtility.SetDirty(target);
				
				GUI.changed = false;
            }
            EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginVertical();
            
			for (int i = 0; i < obj.groundInfos.Count; ++i)
			{
				EditorGUILayout.BeginHorizontal();
				
				GroundInfo info = obj.groundInfos[i];
				
				info.fSize = EditorGUILayout.FloatField("Size", info.fSize);
				if (GUI.changed)
	            {
				    obj.UpdateData();
	                EditorUtility.SetDirty(target);
					
					GUI.changed = false;
					break;
	            }
				
				info.vOffset.y = EditorGUILayout.FloatField("Offset", info.vOffset.y);
				
				EditorGUILayout.EndHorizontal();
			}
           	EditorGUILayout.EndVertical();
            
            
			EditorGUILayout.Separator();
			EditorGUILayout.BeginHorizontal();
			obj.leftSideWall.fSize = EditorGUILayout.FloatField("LeftWallHeight", obj.leftSideWall.fSize);
			obj.leftSideWall.vOffset.x = EditorGUILayout.FloatField("LeftWallOffset", obj.leftSideWall.vOffset.x);
			if (GUI.changed)
			{
				obj.UpdateData();
                EditorUtility.SetDirty(target);
				
				GUI.changed = false;
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			obj.rightSideWall.fSize = EditorGUILayout.FloatField("RightWallHeight", obj.rightSideWall.fSize);
			obj.rightSideWall.vOffset.x = EditorGUILayout.FloatField("RightWallOffset", obj.rightSideWall.vOffset.x);
			if (GUI.changed)
			{
				obj.UpdateData();
                EditorUtility.SetDirty(target);
				
				GUI.changed = false;
			}
			EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }
		
		EditorGUILayout.Separator();
		bShowFloorInfo = EditorGUILayout.Foldout(bShowFloorInfo, "Floor");
        if (bShowFloorInfo)
        {
			EditorGUI.indentLevel++;
			
			obj.FloorLayer = EditorGUILayout.LayerField("FloorLayer", obj.FloorLayer, GUILayout.Width(240));
            if (GUI.changed)
            {
                obj.ChangeFloorLayer(obj.FloorLayer);
                EditorUtility.SetDirty(target);
				
				GUI.changed = false;
            }
			
            EditorGUILayout.BeginHorizontal();
            //EditorGUILayout.PrefixLabel((i + 1) + "");
            obj.floorCount = EditorGUILayout.IntField("Count", obj.floorCount);
            if (GUI.changed)
            {
				obj.SetFloorInfos(obj.floorCount);
                obj.UpdateData();
                EditorUtility.SetDirty(target);
				
				GUI.changed = false;
            }
            EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginVertical();
            
			for (int i = 0; i < obj.floorInfos.Count; ++i)
			{
				EditorGUILayout.BeginHorizontal();
				
				GroundInfo info = obj.floorInfos[i];
				
				info.fSize = EditorGUILayout.FloatField("Size", info.fSize);
				if (GUI.changed)
	            {
				    obj.UpdateData();
	                EditorUtility.SetDirty(target);
					
					GUI.changed = false;
					break;
	            }
				
				info.vOffset = EditorGUILayout.Vector3Field("Offset", info.vOffset);
				if (GUI.changed)
	            {
				    obj.UpdateData();
	                EditorUtility.SetDirty(target);
					
					GUI.changed = false;
	            }
				
				EditorGUILayout.EndHorizontal();
			}
           	EditorGUILayout.EndVertical();
			
            EditorGUI.indentLevel--;
		}
		
        EditorGUILayout.Separator();
        ScrollCamera camera = (ScrollCamera)EditorGUILayout.ObjectField("StageCamera", obj.StageCamera, typeof(ScrollCamera), true);
        if (GUI.changed) 
		{
			obj.SetScrollCamera(camera);
			EditorUtility.SetDirty(target);
		}
        obj.PlayerTarget = (Transform)EditorGUILayout.ObjectField("PlayerTarget", obj.PlayerTarget, typeof(Transform), true);
        if (GUI.changed) EditorUtility.SetDirty(target);
		obj.ArenaTarget = (Transform)EditorGUILayout.ObjectField("ArenaTarget", obj.ArenaTarget, typeof(Transform), true);
        if (GUI.changed) EditorUtility.SetDirty(target);
		obj.towerTarget = (Transform)EditorGUILayout.ObjectField("TowerTarget", obj.towerTarget, typeof(Transform), true);
        if (GUI.changed) EditorUtility.SetDirty(target);
		
		obj.bgmClip = (AudioClip)EditorGUILayout.ObjectField("BGMClip", obj.bgmClip, typeof(AudioClip), true);
        if (GUI.changed) EditorUtility.SetDirty(target);
		obj.bossBGMClip = (AudioClip)EditorGUILayout.ObjectField("BossBGMClip", obj.bossBGMClip, typeof(AudioClip), true);
        if (GUI.changed) EditorUtility.SetDirty(target);
		
		obj.stageRewardID = EditorGUILayout.IntField("RewardID", obj.stageRewardID);
		if (GUI.changed) EditorUtility.SetDirty(target);
		
		EditorGUILayout.Separator();
		EditorGUILayout.LabelField("PlayerTarget....");
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Add") == true)
        {
            obj.AddEmptyPlayerTarget();
            EditorUtility.SetDirty(target);
        }
		EditorGUILayout.EndHorizontal();
		for (int i = 0; i < obj.playerTargetList.Count; i++)
		{
			PlayerTargetInfo playerTargetInfo = obj.playerTargetList[i];

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Remove") == true)
			{
				obj.playerTargetList.RemoveAt(i);
				EditorUtility.SetDirty(target);

				EditorGUILayout.EndHorizontal();
				break;
			}
			
			playerTargetInfo.type = (PlayerTargetInfo.ePlayerTargetType)EditorGUILayout.EnumPopup(playerTargetInfo.type, GUILayout.Width(80));
			playerTargetInfo.target = (Transform)EditorGUILayout.ObjectField(playerTargetInfo.target, typeof(Transform), true);
			if (GUI.changed) 
				EditorUtility.SetDirty(target);
			
			EditorGUILayout.EndHorizontal();
		}
		//////////////////////////////////////////////////////
		
		
		EditorGUILayout.Separator();
		EditorGUILayout.LabelField("ArenaTarget....");
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Add") == true)
        {
            obj.AddEmptyArenaTarget();
            EditorUtility.SetDirty(target);
        }
		EditorGUILayout.EndHorizontal();
		for (int i = 0; i < obj.arenaTargetList.Count; i++)
		{
			PlayerTargetInfo arenaTargetInfo = obj.arenaTargetList[i];

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Remove") == true)
			{
				obj.arenaTargetList.RemoveAt(i);
				EditorUtility.SetDirty(target);

				EditorGUILayout.EndHorizontal();
				break;
			}
			
			arenaTargetInfo.type = (PlayerTargetInfo.ePlayerTargetType)EditorGUILayout.EnumPopup(arenaTargetInfo.type, GUILayout.Width(80));
			arenaTargetInfo.target = (Transform)EditorGUILayout.ObjectField(arenaTargetInfo.target, typeof(Transform), true);
			if (GUI.changed) 
				EditorUtility.SetDirty(target);
			
			EditorGUILayout.EndHorizontal();
		}
		//////////////////////////////////////////////////////
		
		//////////////////////////////////////////////////////////////////////
		EditorGUILayout.Separator();
		EditorGUILayout.LabelField("Teleport....");
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Add") == true)
        {
            obj.AddEmptyTeleportTarget();
            EditorUtility.SetDirty(target);
        }
		EditorGUILayout.EndHorizontal();
		
		for (int i = 0; i < obj.teleportTargetList.Count; i++)
		{
			TeleportTarget teleport = obj.teleportTargetList[i];

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Remove") == true)
			{
				obj.teleportTargetList.RemoveAt(i);
				EditorUtility.SetDirty(target);

				EditorGUILayout.EndHorizontal();
				break;
			}

			teleport.target = (GameObject)EditorGUILayout.ObjectField(teleport.target, typeof(GameObject), true);
			if (GUI.changed) 
				EditorUtility.SetDirty(target);
			
			EditorGUILayout.EndHorizontal();
		}
		////////////////////////////////////////////////////////////////////////
		
		//////////////////////////////////////////////////////////////////////
		EditorGUILayout.Separator();
		EditorGUILayout.LabelField("TreasureBox....");
		obj.treasureBoxRate = EditorGUILayout.FloatField("Rate", obj.treasureBoxRate);
		
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Add") == true)
        {
            obj.AddEmptyTreasureBoxTarget();
            EditorUtility.SetDirty(target);
        }
		EditorGUILayout.EndHorizontal();
		
		for (int i = 0; i < obj.treasureBoxTargetList.Count; i++)
		{
			TreasureBoxTarget treasureBox = obj.treasureBoxTargetList[i];

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Remove") == true)
			{
				obj.treasureBoxTargetList.RemoveAt(i);
				EditorUtility.SetDirty(target);

				EditorGUILayout.EndHorizontal();
				break;
			}

			treasureBox.target = (GameObject)EditorGUILayout.ObjectField(treasureBox.target, typeof(GameObject), true);
			if (GUI.changed) 
				EditorUtility.SetDirty(target);
			
			EditorGUILayout.EndHorizontal();
		}
		////////////////////////////////////////////////////////////////////////
		
		//////////////////////////////////////////////////////////////////////
		EditorGUILayout.Separator();
		EditorGUILayout.LabelField("GatePrefabPaths....");
		
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Add") == true)
        {
            obj.gatePrefabPath.Add("");
            EditorUtility.SetDirty(target);
        }
		EditorGUILayout.EndHorizontal();
		
		for (int i = 0; i < obj.gatePrefabPath.Count; i++)
		{
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Remove") == true)
			{
				obj.gatePrefabPath.RemoveAt(i);
				EditorUtility.SetDirty(target);

				EditorGUILayout.EndHorizontal();
				break;
			}

			obj.gatePrefabPath[i] = EditorGUILayout.TextField("Gate path : ", obj.gatePrefabPath[i]);
			if (GUI.changed) 
				EditorUtility.SetDirty(target);
			
			EditorGUILayout.EndHorizontal();
		}
		////////////////////////////////////////////////////////////////////////
		
		//////////////////////////////////////////////////////////////////////
		EditorGUILayout.Separator();
		EditorGUILayout.LabelField("BossRaid MonsterGenerator....");
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Add") == true)
        {
            obj.AddEmptyMonsterGenerator();
            EditorUtility.SetDirty(target);
        }
		EditorGUILayout.EndHorizontal();
		
		for (int i = 0; i < obj.bossRaidMonsterGenerators.Count; i++)
		{
			MonsterGenerator generator = obj.bossRaidMonsterGenerators[i];

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Remove") == true)
			{
				obj.bossRaidMonsterGenerators.RemoveAt(i);
				EditorUtility.SetDirty(target);

				EditorGUILayout.EndHorizontal();
				break;
			}

			obj.bossRaidMonsterGenerators[i] = (MonsterGenerator)EditorGUILayout.ObjectField(obj.bossRaidMonsterGenerators[i], typeof(MonsterGenerator), true);
			if (GUI.changed) 
				EditorUtility.SetDirty(target);
			
			EditorGUILayout.EndHorizontal();
		}
		////////////////////////////////////////////////////////////////////////
		
		//////////////////////////////////////////////////////////////////////
		EditorGUILayout.Separator();
		EditorGUILayout.LabelField("BossRaid MonsterGeneratorPhase2....");
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Add") == true)
        {
            obj.AddEmptyMonsterGeneratorByPhase2();
            EditorUtility.SetDirty(target);
        }
		EditorGUILayout.EndHorizontal();
		
		for (int i = 0; i < obj.bossRaidPhase2MonsterGenerators.Count; i++)
		{
			MonsterGenerator generator = obj.bossRaidPhase2MonsterGenerators[i];

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Remove") == true)
			{
				obj.bossRaidPhase2MonsterGenerators.RemoveAt(i);
				EditorUtility.SetDirty(target);

				EditorGUILayout.EndHorizontal();
				break;
			}

			obj.bossRaidPhase2MonsterGenerators[i] = (MonsterGenerator)EditorGUILayout.ObjectField(obj.bossRaidPhase2MonsterGenerators[i], typeof(MonsterGenerator), true);
			if (GUI.changed) 
				EditorUtility.SetDirty(target);
			
			EditorGUILayout.EndHorizontal();
		}
		////////////////////////////////////////////////////////////////////////
		
		EditorGUILayout.Separator();
        obj.ghostObject = (GameObject)EditorGUILayout.ObjectField("Ghost", obj.ghostObject, typeof(GameObject), true);
        if (GUI.changed) EditorUtility.SetDirty(target);
		obj.princessObject = (GameObject)EditorGUILayout.ObjectField("Princess", obj.princessObject, typeof(GameObject), true);
        if (GUI.changed) EditorUtility.SetDirty(target);
		obj.tempNpcObject = (GameObject)EditorGUILayout.ObjectField("TempNPC", obj.tempNpcObject, typeof(GameObject), true);
        if (GUI.changed) EditorUtility.SetDirty(target);
		
        EditorGUILayout.EndVertical ();
    }
}














