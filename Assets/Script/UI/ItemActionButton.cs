using UnityEngine;
using System.Collections;

public class ItemActionButton : ActionButton {
	public UILabel itemCount = null;
	
	public void SetItemCount(int nCount)
	{
		if (itemCount != null)
			itemCount.text = string.Format("{0}", nCount);
	}
}
