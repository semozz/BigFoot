using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharLevelInfos : MonoBehaviour {

	public List<UILabel> charLevelLabels = new List<UILabel>();
	
	void Awake()
	{
		TableManager tableManager = TableManager.Instance;
		CharExpTable expTable = null;
		if (tableManager != null)
			expTable = tableManager.charExpTable;
		
		CharInfoData charData = Game.Instance.charInfoData;
		CharPrivateData privateData = null;
		
		int level = 0;
		long curExp = 0L;
		
		for (int index = 0; index < 3; ++index)
		{
			privateData = charData.GetPrivateData(index);
			
			if (privateData != null && privateData.baseInfo != null)
				curExp = privateData.baseInfo.ExpValue;
			
			if (expTable != null)
				level = expTable.GetLevel(curExp);
			
			UILabel label = charLevelLabels[index];
			SetLevel(level, label);
		}
		
	}
	
	public void SetLevel(int level, UILabel label)
	{
		if (label != null)
			label.text = string.Format("Lv.{0}", level);
	}
}
