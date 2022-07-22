using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;

public class XMLTools
{
	public static string charInfoFileName = "CharacterInfos.xml";
	
	public static string pathForDocumentsFile( string filename ) 
	{ 
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			string path = Application.dataPath.Substring( 0, Application.dataPath.Length - 5 );
			path = path.Substring( 0, path.LastIndexOf( '/' ) );
			return System.IO.Path.Combine( System.IO.Path.Combine( path, "Documents" ), filename );
		}
		else if(Application.platform == RuntimePlatform.Android)
		{
			string path = Application.persistentDataPath;	
			path = path.Substring(0, path.LastIndexOf( '/' ) );	
			return System.IO.Path.Combine (path, filename);
		}
		else 
		{
			string path = Application.dataPath;	
			path = path.Substring(0, path.LastIndexOf( '/' ) );
			return System.IO.Path.Combine (path, filename);
		}
	}
	
	public static void LoadCharacterInfos(CharInfoData infos)
	{
		return;
		
		/*
		string path = pathForDocumentsFile(charInfoFileName);
		FileInfo fileInfo = new FileInfo(path);
		if (fileInfo == null || fileInfo.Exists == false)
			return;
		
		XmlDocument xmlDoc = new XmlDocument();
		XmlReader reader = XmlReader.Create(path);
		
		if (xmlDoc != null && reader != null)
		{
			xmlDoc.Load(reader);
			
			XmlNodeList data = xmlDoc.GetElementsByTagName("CharacterInfos");
			for (int i = 0; i < data.Count; ++i)
			{
				XmlNode dataChilds = data.Item(i);
				
				XmlNodeList allObjects = dataChilds.ChildNodes;
				
				int privateIndex = 0;
				for (int index = 0; index < allObjects.Count; ++index)
				{
					XmlNode node = allObjects.Item(index);
					
					if (node == null)
						continue;
					
					if (node.Name == "PrivateInfos")
						LoadPrivateInfos(node, infos.privateDatas[privateIndex++]);
					else if (node.Name == "CommonInfos")
						LoadCommonInfos(node, infos);
				}
			}
			
			reader.Close();
		}
		*/
	}
	
	public static void LoadPrivateInfos(XmlNode rootNode, CharPrivateData info)
	{

	}
	
	public static void LoadCommonInfos(XmlNode rootNode, CharInfoData info)
	{
		XmlNodeList allObjects = rootNode.ChildNodes;
		
		for (int index = 0; index < allObjects.Count; ++index)
		{
			XmlNode node = allObjects.Item(index);
			
			if (node == null)
				continue;
			
			if (node.Name == "InventoryNormal")
			{
				LoadItems(info.inventoryNormalData, node);
			}
			else if (node.Name == "InventoryCostume")
			{
				LoadItems(info.inventoryCostumeData, node);
			}
			else if (node.Name == "StageRewardItem")
			{
				LoadItems(info.stageRewardItems, node);
			}
		}
	}
	
	public static void LoadEquipItems(List<EquipInfo> equipItems, XmlNode node)
	{
		XmlNodeList allObjects = node.ChildNodes;
		
		for (int index = 0; index < allObjects.Count; ++index)
		{
			XmlNode childNode = allObjects.Item(index);
			
			XmlNodeList allChildObjects = childNode.ChildNodes;
			
			EquipInfo equipInfo = CreateEquipSlotInfo(allChildObjects[0], allChildObjects[1]);
			
			if (equipItems != null && equipInfo != null)
				equipItems.Add(equipInfo);
		}
	}
	
	public static EquipInfo CreateEquipSlotInfo(XmlNode slotInfoNode, XmlNode itemInfoNode)
	{
		EquipInfo newEquipInfo = null;
		if (slotInfoNode != null)
		{
			newEquipInfo = new EquipInfo();
			
			string strSlot = slotInfoNode.InnerText;
			int slotIndex = -1;
			if (int.TryParse(strSlot, out slotIndex) == false)
				slotIndex = -1;
			
			newEquipInfo.SetSlotType(slotIndex);
		}
		
		if (itemInfoNode != null)
		{
			int itemID = -1;
			string uID = "";
			int itemCount = 0;
			int itemGrade = 0;
			int itemReinforce = 0;
			
			string[] itemInfos = itemInfoNode.InnerText.Split(',');
			
			itemID = int.Parse(itemInfos[0]);
			uID = itemInfos[1];
			itemCount = int.Parse(itemInfos[2]);
			itemGrade = int.Parse(itemInfos[3]);
			itemReinforce = int.Parse(itemInfos[4]);
			
			if (itemID != -1)
			{
				Item newItem = Item.CreateItem(itemID, uID, itemGrade, itemReinforce, itemCount);
				
				if (newEquipInfo != null)
					newEquipInfo.item =  newItem;
			}
		}
		
		return newEquipInfo;
	}
	
	public static void LoadItems(List<Item> itemList, XmlNode node)
	{
		XmlNodeList allObjects = node.ChildNodes;
		
		for (int index = 0; index < allObjects.Count; ++index)
		{
			XmlNode childNode = allObjects.Item(index);
			
			XmlNodeList allChildObjects = childNode.ChildNodes;
			
			Item newItem = CreateItem(allChildObjects[0], allChildObjects[1]);
			
			if (itemList != null && newItem != null)
				itemList.Add(newItem);
		}
	}
	
	public static Item CreateItem(XmlNode slotInfoNode, XmlNode itemInfoNode)
	{
		Item newItem = null;
		
		if (itemInfoNode != null)
		{
			int itemID = -1;
			string uID = "";
			int itemCount = 0;
			int itemGrade = 0;
			int itemReinforce = 0;
			
			string[] itemInfos = itemInfoNode.InnerText.Split(',');
			
			itemID = int.Parse(itemInfos[0]);
			uID = itemInfos[1];
			itemCount = int.Parse(itemInfos[2]);
			itemGrade = int.Parse(itemInfos[3]);
			itemReinforce = int.Parse(itemInfos[4]);
			
			if (itemID != -1)
			{
				newItem = Item.CreateItem(itemID, uID, itemGrade, itemReinforce, itemCount);
			}
		}
		
		return newItem;
	}
	
	public static void LoadStageInfos(List<StageInfo> stageList, XmlNode node)
	{
		XmlNodeList allObjects = node.ChildNodes;
		
		for (int index = 0; index < allObjects.Count; ++index)
		{
			XmlNode childNode = allObjects.Item(index);
			
			string[] itemInfos = childNode.InnerText.Split(',');
			
			if (itemInfos.Length == 2)
			{
				//int stageIndex = int.Parse(itemInfos[0]);
				string stageState = itemInfos[1];
				
				StageInfo stageInfo = new StageInfo();
				if (stageState == "Normal")
					stageInfo.stageInfo = StageButton.eStageButton.Normal;
				else if (stageState == "Locked")
					stageInfo.stageInfo = StageButton.eStageButton.Locked;
				else if (stageState == "Clear")
					stageInfo.stageInfo = StageButton.eStageButton.Clear;
				
				stageList.Add(stageInfo);
			}
		}
	}
	
	public static void LoadMasteryInfos(List<MasterySaveData> masteryList, XmlNode node)
	{
		XmlNodeList allObjects = node.ChildNodes;
		
		for (int index = 0; index < allObjects.Count; ++index)
		{
			XmlNode childNode = allObjects.Item(index);
			
			string[] itemInfos = childNode.InnerText.Split(',');
			
			if (itemInfos.Length == 2)
			{
				int tableID = int.Parse(itemInfos[0]);
				int level = int.Parse(itemInfos[1]);
				
				MasterySaveData masterySaveData = new MasterySaveData();
				masterySaveData.tableID = tableID;
				masterySaveData.level = level;
				
				masteryList.Add(masterySaveData);
			}
		}
	}
	
	public static void SaveCharacterInfos(CharInfoData infoDatas)
	{
		string path = pathForDocumentsFile(charInfoFileName);
		XmlDocument xmlDoc = new XmlDocument();
		
		XmlElement elementRoot = xmlDoc.CreateElement("CharacterInfos");
		xmlDoc.AppendChild(elementRoot);
		
		SaveCharacterInfo(xmlDoc, elementRoot, infoDatas);
		
		if (xmlDoc != null)
		{
			using (TextWriter sw = new StreamWriter(path, false, System.Text.Encoding.UTF8))
			{
				xmlDoc.Save(sw);
				sw.Close();
			}
		}
	}
	
	public static void SaveCharacterInfo(XmlDocument xmlDoc, XmlElement root, CharInfoData info)
	{
		
	}
	
	public static void SetItemInfo(XmlDocument xmlDoc, XmlElement xmRoot, string nodeName, List<Item> itemList)
	{
		if (itemList == null)
			return;
		
		XmlElement itemRootNode = xmlDoc.CreateElement(nodeName);
		if (itemRootNode != null)
		{
			int index = 0;
			foreach(Item item in itemList)
			{
				XmlElement itemNode = xmlDoc.CreateElement("Item");
				if (itemNode == null)
					continue;
				
				XmlElement slotInfo = xmlDoc.CreateElement("Slot");
				if (slotInfo != null)
				{
					slotInfo.InnerText = index.ToString();
					
					itemNode.AppendChild(slotInfo);
				}
				
				XmlElement itemInfoNode = xmlDoc.CreateElement("ItemInfo");
				if (itemInfoNode != null)
				{
					
					int itemID = -1;
					string uID = "";
					int itemCount = 0;
					int itemGrade = 0;
					int itemReinforce = 0;
					
					if (item != null && item.itemInfo != null)
					{
						itemID = item.itemInfo.itemID;
						uID = item.uID;
						itemCount = item.itemCount;
						
						itemGrade = (int)item.itemGrade;
						itemReinforce = item.reinforceStep;
					}
					
					itemInfoNode.InnerText = string.Format("{0},{1},{2},{3},{4}", itemID, uID, itemGrade, itemReinforce, itemCount);
					
					itemNode.AppendChild(itemInfoNode);
				}
					
				itemRootNode.AppendChild(itemNode);
				
				++index;
			}
			
			xmRoot.AppendChild(itemRootNode);
		}
	}
	
	public static void SetEquipItemInfo(XmlDocument xmlDoc, XmlElement xmRoot, string nodeName, List<EquipInfo> itemList)
	{
		if (itemList == null)
			return;
		
		XmlElement itemRootNode = xmlDoc.CreateElement(nodeName);
		if (itemRootNode != null)
		{
			int index = 0;
			foreach(EquipInfo info in itemList)
			{
				XmlElement itemNode = xmlDoc.CreateElement("Item");
				if (itemNode == null)
					continue;
				
				XmlElement slotInfo = xmlDoc.CreateElement("Slot");
				if (slotInfo != null)
				{
					slotInfo.InnerText = index.ToString();
					
					itemNode.AppendChild(slotInfo);
				}
				
				XmlElement itemInfoNode = xmlDoc.CreateElement("ItemInfo");
				if (itemInfoNode != null)
				{
					
					int itemID = -1;
					string uID = "";
					int itemCount = 0;
					int itemGrade = 0;
					int itemReinforce = 0;
					
					if (info != null && info.item != null && info.item.itemInfo != null)
					{
						itemID = info.item.itemInfo.itemID;
						uID = info.item.uID;
						itemCount = info.item.itemCount;
						
						itemGrade = (int)info.item.itemGrade;
						itemReinforce = info.item.reinforceStep;
					}
					
					itemInfoNode.InnerText = string.Format("{0},{1},{2},{3},{4}", itemID, uID, itemGrade, itemReinforce, itemCount);
					//itemInfoNode.InnerText = itemID.ToString() + "," + itemCount.ToString() + "," + itemGrade.ToString() + "," + itemReinforce.ToString();
					
					itemNode.AppendChild(itemInfoNode);
				}
					
				itemRootNode.AppendChild(itemNode);
				
				++index;
			}
			
			xmRoot.AppendChild(itemRootNode);
		}
	}
	
	public static void SetStageInfo(XmlDocument xmlDoc, XmlElement xmRoot, string nodeName, List<StageInfo> stageList)
	{
		if (stageList == null)
			return;
		
		XmlElement stageRootNode = xmlDoc.CreateElement(nodeName);
		if (stageRootNode != null)
		{
			int index = 0;
			foreach(StageInfo info in stageList)
			{
				XmlElement stageNode = xmlDoc.CreateElement("Stage");
				if (stageNode != null)
				{
					stageNode.InnerText = index.ToString() +"," + info.stageInfo;
				}
				
				stageRootNode.AppendChild(stageNode);
				
				++index;
			}
			
			xmRoot.AppendChild(stageRootNode);
		}
	}
	
	public static void SetMasteryInfo(XmlDocument xmlDoc, XmlElement xmRoot, string nodeName, List<MasterySaveData> masteryDataList)
	{
		if (masteryDataList == null)
			return;
		
		XmlElement masteryRootNode = xmlDoc.CreateElement(nodeName);
		if (masteryRootNode != null)
		{
			int index = 0;
			foreach(MasterySaveData info in masteryDataList)
			{
				XmlElement masteryNode = xmlDoc.CreateElement("Mastery");
				if (masteryNode != null)
				{
					masteryNode.InnerText = string.Format("{0},{1}", info.tableID, info.level);
				}
				
				masteryRootNode.AppendChild(masteryNode);
				
				++index;
			}
			
			xmRoot.AppendChild(masteryRootNode);
		}
	}
}
