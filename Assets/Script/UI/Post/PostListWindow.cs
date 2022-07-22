using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PostListWindow : MonoBehaviour {

	public PostWindow parentWindow = null;
	
	public string dataPanelPrefabPath = "";
	public UIGrid grid = null;
	public UIDraggablePanel dragablePanel = null;
	public UIPanel scrollPanel = null;
	
	public Transform lastItemPos = null;
	public PostItemPanel lastItem = null;
	public string lastItemID = "-1";
	
	public StringTable stringTable = null;
    public List<MailInfo> lstPostInfos;
	private bool repositinNeed = false;
	
	public void Start()
	{
		TableManager tableManager = TableManager.Instance;
		stringTable = tableManager != null ? tableManager.stringTable : null;
		
		if (stringTable != null)
		{
			
		}
		
		if (dragablePanel != null)
			dragablePanel.onDragFinished = new UIDraggablePanel.OnDragFinished(OnDragFinished);
	}
	
	void Update()
	{
		if (repositinNeed)
		{
			if (grid != null && !grid.repositionNow)
			{
				CheckLastItem();
				repositinNeed = false;
			}
		}
		
	}
	
	public void InitWindow()
	{
		RefreshList();
		foreach(PostItemPanel infoPanel in postItemPanels)
			DestroyObject(infoPanel.gameObject, 0.0f);
		
		postItemPanels.Clear();
	}
	
	private List<PostItemPanel> postItemPanels = new List<PostItemPanel>();
	public int SetPostInfos(List<MailInfo> postInfos)
	{
		InitWindow();
		
		return AddPostInfos(postInfos);
	}
	
	public int AddPostInfos(List<MailInfo> postInfos)
	{
        this.lstPostInfos = postInfos;

		if (this.grid != null)
			this.grid.sorted = true;
		
		Vector3 vPos = Vector3.zero;
		int nCount = postInfos.Count;

		for (int index = 0; index < nCount; ++index)
		{
			MailInfo postItem = postInfos[index];
			
			PostItemPanel infoPanel = ResourceManager.CreatePrefab<PostItemPanel>(dataPanelPrefabPath, grid.transform, vPos);
			if (infoPanel != null)
			{
				infoPanel.SetPostItem(postItem);
				
				postItemPanels.Add(infoPanel);
				
				infoPanel.parentObj = this.parentWindow;
				
				SetSortName(infoPanel);
				
				//lastItem = infoPanel;
				lastItemID = postItem.Index;
			}
		}
		
		if (grid != null)
		{
			grid.repositionNow = true;
			repositinNeed = true;
		}
		
		return postItemPanels.Count;
	}
	
	public void RefreshList()
	{
		if (grid != null)
			grid.Reposition();
		
		if (dragablePanel != null)
			dragablePanel.ResetPosition();
		
		if (scrollPanel != null)
		{
			scrollPanel.Refresh();
			
			Vector4 clipRange = scrollPanel.clipRange;
			clipRange.x = clipRange.y = 0.0f;
			
			scrollPanel.clipRange = clipRange;
			scrollPanel.transform.localPosition = new Vector3(0.0f, 10.0f, 0.0f);
		}
		
		CheckLastItem();
	}
	
	private void CheckLastItem()
	{
		if (grid != null)
		{
			PostItemPanel[] panels = grid.GetComponentsInChildren<PostItemPanel>();
			if (panels == null || panels.Length == 0)
			{
				lastItem = null;
				lastItemID = "-1";
			}
			else
			{
				float minValue = float.MaxValue;
				foreach(PostItemPanel temp in panels)
				{
					Vector3 vPos = temp.transform.localPosition;
					if (minValue > vPos.y)
					{
						lastItem = temp;
						minValue = vPos.y;
					}
				}
			}
		}
	}
	
	public void SetSortName(PostItemPanel infoPanel)
	{
		string name = "ZZZ";
		bool isRead = false;
		if (infoPanel != null && infoPanel.itemData != null)
		{
            name = string.Format("{0:0####}", infoPanel.itemData.Index);
			isRead = infoPanel.IsRead();
		}
		
		string sortName = "";
		
		if (isRead)
			sortName = string.Format("ZZZ_{0}_PostInfo", name);
		else
			sortName = string.Format("AAA_{0}_PostInfo", name);
		
		infoPanel.name = sortName;
	}

    public void SetReadMailAll()
    {
        foreach (PostItemPanel infoPanel in postItemPanels)
        {
            if (infoPanel != null && infoPanel.itemData != null)
            {
                infoPanel.itemData.bOpened = 1;

                infoPanel.OnRead();

                SetSortName(infoPanel);
            }
        }
    }
	
	public void SetReadMail(string mailIndex)
	{
		foreach(PostItemPanel infoPanel in postItemPanels)
		{
			if (infoPanel != null && infoPanel.itemData != null && infoPanel.itemData.Index == mailIndex)
			{
				infoPanel.itemData.bOpened = 1;
				
				infoPanel.OnRead();
				
				SetSortName(infoPanel);
				break;
			}
		}
		if (grid != null)
		{
			grid.repositionNow = true;
			repositinNeed = true;
		}
		
		if (CheckUnReadPostCount() < 10 && Game.Instance.postItemCount > 20 && parentWindow != null)
			parentWindow.SendMessage("RequestMoreItems", lastItemID, SendMessageOptions.DontRequireReceiver);
	}
	
	public int CheckUnReadPostCount()
	{
		int nCount = 0;
		foreach(PostItemPanel infoPanel in postItemPanels)
		{
			if (infoPanel != null && infoPanel.itemData != null)
			{
				if (infoPanel.itemData.bOpened == 0)
					nCount++;
			}
		}
		
		return nCount;
	}
	
	public float refreshLimitLength = 0.7f;
	public void OnDragFinished()
	{
		if (lastItem == null || lastItemPos == null)
			return;
		
		Vector3 limitPos = lastItemPos.position;
		Vector3 lastPos = lastItem.transform.position;
		
		Vector3 lastDiff = limitPos - lastPos;
		if (limitPos.y < lastPos.y &&
			Mathf.Abs(lastDiff.y) > refreshLimitLength)
		{
			Debug.Log("Lower Post Item List Request......");
			
			if (parentWindow != null)
				parentWindow.SendMessage("RequestMoreItems", lastItemID, SendMessageOptions.DontRequireReceiver);
			
			return;
		}
	}
}
