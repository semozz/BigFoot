using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;

public class SetItemTable : BaseTable {
	public Dictionary<int, SetItemInfo> setItemInfos = new Dictionary<int, SetItemInfo>();
	
	
	public SetItemInfo GetInfo(int setItemID)
	{
		SetItemInfo info = null;
		if (setItemInfos.ContainsKey(setItemID) == true)
			info = setItemInfos[setItemID];
		
		return info;
	}
	
	public SetItemInfo GetTempInfo(int setItemID)
	{
		SetItemInfo origInfo = GetInfo(setItemID);
		SetItemInfo newTempInfo = null;
		if (origInfo != null)
			newTempInfo = new SetItemInfo(origInfo);
		
		return newTempInfo;
	}
	
	public void InitValue()
	{
		setItemInfos.Clear();
	}
	
	public void Load(string fileName)
	{
		//TextAsset textAsset = (TextAsset)Resources.Load(filePath, typeof(TextAsset));
		string path = string.Format("NewAsset/Tables/{0}.xml", fileName);
		TextAsset textAsset = ResourceManager.LoadTextAsset(path);
		
		XmlTextReader textReader = new XmlTextReader(new StringReader(textAsset.text));
		XmlDocument xmlDoc = new XmlDocument();
		
		if (xmlDoc != null && textReader != null)
		{
			xmlDoc.Load(textReader);
			
			LoadTableFromXML(xmlDoc);
			
			textReader.Close();
		}
	}
	
	public override void LoadTableFromXML (XmlDocument xmlDoc)
	{
		InitValue();
		
		XmlNodeList data = xmlDoc.GetElementsByTagName("SetItems");
		for (int i = 0; i < data.Count; ++i)
		{
			XmlNode dataChilds = data.Item(i);
			
			XmlNodeList allObjects = dataChilds.ChildNodes;
			
			for (int index = 0; index < allObjects.Count; ++index)
			{
				XmlNode node = allObjects.Item(index);
				
				if (node == null)
					continue;
				
				if (node.Name == "SetItem")
				{
					SetItemInfo newInfo = new SetItemInfo();
					LoadSetItemInfo(node, newInfo);
					
					setItemInfos.Add(newInfo.setItemID, newInfo);
				}
			}
		}
	}
	
	public void LoadSetItemInfo(XmlNode rootNode, SetItemInfo newInfo)
	{
		XmlNodeList allObjects = rootNode.ChildNodes;
		
		for (int index = 0; index < allObjects.Count; ++index)
		{
			XmlNode node = allObjects.Item(index);
			
			if (node == null)
				continue;
			
			if (node.Name == "ID")
			{
				string infoStr = node.InnerText;
				newInfo.setItemID = int.Parse(infoStr);
			}
			else if (node.Name == "Name")
			{
				newInfo.setItemName = node.InnerText;
			}
			else if (node.Name == "Info")
			{
				SetAttributeInfo setInfo = new SetAttributeInfo();
				LoadSetAttributeInfo(node, setInfo);
				newInfo.setAttributeList.Add(setInfo);
			}
		}
	}
	
	public void LoadSetAttributeInfo(XmlNode rootNode, SetAttributeInfo newInfo)
	{
		XmlNodeList allObjects = rootNode.ChildNodes;
		
		for (int index = 0; index < allObjects.Count; ++index)
		{
			XmlNode node = allObjects.Item(index);
			
			if (node == null)
				continue;
			
			if (node.Name == "LimitCount")
			{
				string infoStr = node.InnerText;
				newInfo.limitCount = int.Parse(infoStr);
			}
			else if (node.Name == "Attribute")
			{
				string[] attributeInfos = node.InnerText.Split(';');
				
				AttributeValue.eAttributeType attValueType = AttributeValue.eAttributeType.None;
				float attributeValue = 0.0f;
				
				attValueType = AttributeValue.GetAttributeType(attributeInfos[0]);
				attributeValue = float.Parse(attributeInfos[1]);
				
				newInfo.AddAttribute(attValueType, attributeValue);
			}
		}
	}
	
}
