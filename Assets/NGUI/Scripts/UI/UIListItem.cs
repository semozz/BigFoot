using UnityEngine;
using System.Collections;

public class UIListItem : MonoBehaviour
{
	public ListItemData data = null;
	
	public float itemHeight = 0.0f;
	void Awake()
	{
		BoxCollider boxCollider = this.gameObject.GetComponent<BoxCollider>();
		if (boxCollider != null)
		{
			itemHeight = boxCollider.size.y;
		}
	}
	
	public virtual void SetData(ListItemData data)
	{
		this.data = data;
	}
	
}
