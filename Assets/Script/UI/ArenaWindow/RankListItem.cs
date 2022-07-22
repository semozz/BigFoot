using UnityEngine;
using System.Collections;

public class RankListItemData : ListItemData
{
	public int rankType = 0;
	public string rankTypeName = "";
	public string rankName = "";
}

public class RankListItem : UIListItem {
	public UILabel rankTypeLabel = null;
	public UILabel rankNameLabel = null;
	
	public override void SetData(ListItemData data)
	{
		base.SetData(data);
		
		string rankTypeName = "";
		string rankName = "";
			
		if (data != null && data.GetType() == typeof(RankListItemData))
		{
			RankListItemData rankData = (RankListItemData)data;
			
			if (rankData != null)
			{
				rankTypeName = rankData.rankTypeName;
				rankName = rankData.rankName;
			}
		}
		
		if (rankTypeLabel != null)
			rankTypeLabel.text = rankTypeName;
		if (rankNameLabel != null)
			rankNameLabel.text = rankName;
	}
}
