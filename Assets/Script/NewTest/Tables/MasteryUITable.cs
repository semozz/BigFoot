using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;

public class MasterySlotInfo
{
	public int slotIndex = -1;
	public int masteryID = 0;
	public MasteryIcon.eArrowType arrowType = MasteryIcon.eArrowType.None;
}

public class MasteryGroupInfo
{
	public int groupID = 0;
	public string groupName = "";
	public string groupBackgroundImg = "";
	
	public List<MasterySlotInfo> slotInfos = new List<MasterySlotInfo>();
	
	public void AddMasterySlot(MasterySlotInfo info)
	{
		slotInfos.Add(info);
	}
}

public class CharMasteryInfo
{
	public GameDef.ePlayerClass classType = GameDef.ePlayerClass.CLASS_WARRIOR;
	
	public Dictionary<int, MasteryGroupInfo> groupInfos = new Dictionary<int, MasteryGroupInfo>();
	
	public void AddGroup(int groupID, MasteryGroupInfo groupInfo)
	{
		if (groupInfos.ContainsKey(groupID) == false)
		{
			groupInfos.Add(groupID, groupInfo);
		}
	}
	
	public MasteryGroupInfo GetGroupInfo(int groupID)
	{
		MasteryGroupInfo info = null;
		if (groupInfos.ContainsKey(groupID) == true)
			info = groupInfos[groupID];
		
		return info;
	}
}

public class MasteryUITable : BaseTable{
	public Dictionary<GameDef.ePlayerClass, CharMasteryInfo> masteryUIInfos = new Dictionary<GameDef.ePlayerClass, CharMasteryInfo>();
	
	public MasteryUITable()
	{
		
	}
	
	public void InitValue()
	{
		masteryUIInfos.Clear();
	}
	
	public CharMasteryInfo GetCharMasteryUIInfo(GameDef.ePlayerClass classType)
	{
		CharMasteryInfo info = null;
		if (masteryUIInfos.ContainsKey(classType) == true)
			info = masteryUIInfos[classType];
		
		return info;
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
		
		XmlNodeList data = xmlDoc.GetElementsByTagName("MasteryUIInfo");
		for (int i = 0; i < data.Count; ++i)
		{
			XmlNode dataChilds = data.Item(i);
			
			XmlNodeList allObjects = dataChilds.ChildNodes;
			
			for (int index = 0; index < allObjects.Count; ++index)
			{
				XmlNode node = allObjects.Item(index);
				
				if (node == null)
					continue;
				
				
				if (node.Name == "PlayerClass")
				{
					CharMasteryInfo classMasteryInfo = new CharMasteryInfo();
					LoadClassMasteryInfo(node, classMasteryInfo);
					
					masteryUIInfos.Add(classMasteryInfo.classType, classMasteryInfo);
				}
			}
		}
	}
	
	public void LoadClassMasteryInfo(XmlNode rootNode, CharMasteryInfo classMasteryInfo)
	{
		XmlNodeList allObjects = rootNode.ChildNodes;
		
		for (int index = 0; index < allObjects.Count; ++index)
		{
			XmlNode node = allObjects.Item(index);
			
			if (node == null)
				continue;
			
			if (node.Name == "Class")
			{
				string typeStr = node.InnerText;
				GameDef.ePlayerClass classType = GameDef.ePlayerClass.CLASS_WARRIOR;
				if (typeStr == "Warrior")
					classType = GameDef.ePlayerClass.CLASS_WARRIOR;
				else if (typeStr == "Assassin")
					classType = GameDef.ePlayerClass.CLASS_ASSASSIN;
				else if (typeStr == "Wizard")
					classType = GameDef.ePlayerClass.CLASS_WIZARD;
					
				classMasteryInfo.classType = classType;
			}
			else if (node.Name == "Group")
			{
				MasteryGroupInfo groupInfo = new MasteryGroupInfo();
				LoadGroupInfo(node, groupInfo);
				
				classMasteryInfo.AddGroup(groupInfo.groupID, groupInfo);
			}
		}
	}
	
	public void LoadGroupInfo(XmlNode rootNode, MasteryGroupInfo groupInfo)
	{
		XmlNodeList allObjects = rootNode.ChildNodes;
		
		string innerString = "";
		
		for (int index = 0; index < allObjects.Count; ++index)
		{
			XmlNode node = allObjects.Item(index);
			
			if (node == null)
				continue;
			
			if (node.Name == "GroupInfo")
			{
				innerString = node.InnerText;
				string[] infoStr = innerString.Split(',');
				
				groupInfo.groupID = int.Parse(infoStr[0]);
				groupInfo.groupName = infoStr[1];
				groupInfo.groupBackgroundImg = infoStr[2];
			}
			else if (node.Name == "SlotInfo")
			{
				innerString = node.InnerText;
				string[] infoStr = innerString.Split(',');
				
				MasterySlotInfo slotInfo = new MasterySlotInfo();
				slotInfo.slotIndex = int.Parse(infoStr[0]);
				slotInfo.masteryID = int.Parse(infoStr[1]);
				
				slotInfo.arrowType = MasteryIcon.ToArrowType(infoStr[2]);
				
				groupInfo.AddMasterySlot(slotInfo);
			}
		}
	}
}
