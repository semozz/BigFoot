using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;

public class ArenaRewardData
{
	public int rankStep = 0;
	public string rankName = "";
	public string desc = "";
	public int rewardMedal = 0;
}

public class ArenaRewardInfoTable : BaseTable {
	
	public List<ArenaRewardData> rewardInfos = new List<ArenaRewardData>();
	
	public void InitValue()
	{
		rewardInfos.Clear();
	}
	
	public ArenaRewardData GetRewardInfoByIndex(int index)
	{
		ArenaRewardData data = null;
		
		return data;
	}
	
	public ArenaRewardData GetRewardInfoData(int rankStep)
	{
		ArenaRewardData findData = null;
		foreach(ArenaRewardData data in rewardInfos)
		{
			if (data != null && data.rankStep == rankStep)
			{
				findData = data;
				break;
			}
		}
		
		return findData;
	}
	
	public override void LoadTable(CSVDB db)
	{
		if (db != null)
		{
			int rank = 0;
			foreach(var data in db.data)
			{
				rank = int.Parse(data.Key);
				
				int reward_medal = 0;
				string rank_name = "";
				string desc = "";
				
				ValueData tempData = null;
				
				tempData = data.Value.GetValue("desc");
				if (tempData != null)
					desc = tempData.ToText();
				
				tempData = data.Value.GetValue("rank_name");
				if (tempData != null)
					rank_name = tempData.ToText();
				
				tempData = data.Value.GetValue("reward_medal");
				if (tempData != null)
					reward_medal = tempData.ToInt();
				
				ArenaRewardData newData = new ArenaRewardData(){
					rankStep = rank,
					rankName = rank_name,
					desc = desc,
					rewardMedal = reward_medal
				};
				
				this.rewardInfos.Add(newData);
			}
		}
	}
	
	/*
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
	
	public override void LoadTableFromXML(XmlDocument xmlDoc)
	{
		InitValue();
		
		string innerString = "";
		XmlNodeList data = xmlDoc.GetElementsByTagName("ArenaReward");
		for (int i = 0; i < data.Count; ++i)
		{
			XmlNode dataChilds = data.Item(i);
			
			XmlNodeList allObjects = dataChilds.ChildNodes;
			
			for (int index = 0; index < allObjects.Count; ++index)
			{
				XmlNode node = allObjects.Item(index);
				
				if (node == null)
					continue;
				
				if (node.Name == "Rank")
				{
					innerString = node.InnerText;
					string[] infoStr = innerString.Split(';');
					
					ArenaRewardData newData = new ArenaRewardData();
					newData.rankStep = int.Parse(infoStr[0]);
					newData.rankName = infoStr[1];
					newData.limitPercent = float.Parse(infoStr[2]);
					newData.rewardMedal = int.Parse(infoStr[3]);
					
					rewardInfos.Add(newData);
				}
			}
		}
	}
	*/
}
