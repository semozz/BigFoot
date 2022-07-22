using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;

public class WaveRewardData
{
	public int minOrder = 0;
	public int maxOrder = 0;
	
	public int rewardJewel = 0;
}

public class WaveRewardInfoTable : BaseTable {
	
	public List<WaveRewardData> rewardInfos = new List<WaveRewardData>();
	
	public void InitValue()
	{
		rewardInfos.Clear();
	}
	
	public WaveRewardData GetRewardInfoByIndex(int index)
	{
		WaveRewardData data = null;
		
		return data;
	}
	
	public WaveRewardData GetRewardInfoData(int rankStep)
	{
		WaveRewardData findData = null;
		foreach(WaveRewardData data in rewardInfos)
		{
			if (data == null)
				continue;
			
			bool bCheck = false;
			
			if (data.minOrder == data.maxOrder)
			{
				if (data.minOrder == rankStep)
					bCheck = true;
			}
			else if (data.maxOrder == -1)
			{
				if (data.minOrder <= rankStep)
					bCheck = true;
			}
			else
			{
				if (data.minOrder <= rankStep && rankStep <= data.maxOrder)
					bCheck = true;
			}
			
			if (bCheck == true)
			{
				findData = data;
				break;
			}
		}
		
		return findData;
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
		
		string innerString = "";
		XmlNodeList data = xmlDoc.GetElementsByTagName("WaveReward");
		for (int i = 0; i < data.Count; ++i)
		{
			XmlNode dataChilds = data.Item(i);
			
			XmlNodeList allObjects = dataChilds.ChildNodes;
			
			for (int index = 0; index < allObjects.Count; ++index)
			{
				XmlNode node = allObjects.Item(index);
				
				if (node == null)
					continue;
				
				if (node.Name == "Info")
				{
					innerString = node.InnerText;
					string[] infoStr = innerString.Split(';');
					
					WaveRewardData newData = new WaveRewardData();
					newData.minOrder = int.Parse(infoStr[0]);
					newData.maxOrder = int.Parse(infoStr[1]);
					
					newData.rewardJewel = int.Parse(infoStr[2]);
					
					rewardInfos.Add(newData);
				}
			}
		}
	}
}
