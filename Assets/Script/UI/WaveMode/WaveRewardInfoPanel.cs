using UnityEngine;
using System.Collections;

public class WaveRewardInfoPanel : MonoBehaviour {
	public string prefabPath = "";
	public Transform rootNode = null;
	
	public float cellHeight = 45.0f;
	public float cellStart = -23.0f;
	
	// Use this for initialization
	void Start ()
	{
		TableManager tableManager = TableManager.Instance;
		WaveRewardInfoTable waveRewardInfos = tableManager != null ? tableManager.waveRewardInfo : null;
		
		if (waveRewardInfos != null)
		{
			Vector3 vPos = Vector3.zero;
			vPos.y = cellStart;
			
			foreach(WaveRewardData data in waveRewardInfos.rewardInfos)
			{
				WaveRewardInfo infoRow = ResourceManager.CreatePrefab<WaveRewardInfo>(prefabPath, rootNode, vPos);
				
				if (infoRow != null)
				{
					infoRow.SetInfo(data);
					
					vPos.y -= cellHeight;
				}
			}
		}
	}
}
