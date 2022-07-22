using UnityEngine;
using System.Collections;

public class ArenaRewardInfoPanel : MonoBehaviour {
	public string prefabPath = "";
	public Transform rootNode = null;
	
	public float cellHeight = 45.0f;
	public float cellStart = -23.0f;
	
	// Use this for initialization
	void Start ()
	{
		TableManager tableManager = TableManager.Instance;
		ArenaRewardInfoTable arenaRewardInfos = tableManager != null ? tableManager.arenaRewardInfo : null;
		
		if (arenaRewardInfos != null)
		{
			Vector3 vPos = Vector3.zero;
			vPos.y = cellStart;
			
			foreach(ArenaRewardData data in arenaRewardInfos.rewardInfos)
			{
				ArenaRewardInfo infoRow = ResourceManager.CreatePrefab<ArenaRewardInfo>(prefabPath, rootNode, vPos);
				
				if (infoRow != null)
				{
					infoRow.SetInfo(data);
					
					vPos.y -= cellHeight;
				}
			}
		}
	}
}
