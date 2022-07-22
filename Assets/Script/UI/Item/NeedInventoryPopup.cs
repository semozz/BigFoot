using UnityEngine;
using System.Collections;

public class NeedInventoryPopup : BasePopup {
	
	void Awake()
	{
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		if (stringTable != null)
		{
			if (cancelButtonLabel != null)
				cancelButtonLabel.text = stringTable.GetData(cancelButtonStringID);
			
			if (okButtonLabel != null)
				okButtonLabel.text = stringTable.GetData(okButtonStringID);
			
			if (titleLabel != null)
				titleLabel.text = stringTable.GetData(titleStringID);
			
			if (messageLabel != null)
				messageLabel.text = stringTable.GetData(messageStringID);
		}
	}
}
