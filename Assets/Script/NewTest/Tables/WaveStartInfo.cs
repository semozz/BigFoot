using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;

public class TowerInfo
{
	public enum eTowerType
	{
		None = -1,
		Wood,
		Stone,
		Iron,
	}
	public eTowerType type = eTowerType.Wood;
	public float attack = 0.0f;
	public float health = 0.0f;
	public float needJewel = 0.0f;
	public float needStamina = 0.0f;
	
	public string name = "";
	
	public TowerInfo()
	{
	}
	
	public TowerInfo(eTowerType type, float attackValue, float healthValue, float needJewel, float needStamina, string name)
	{
		this.type = type;
		this.attack = attackValue;
		this.health = healthValue;
		this.needJewel = needJewel;
		this.needStamina = needStamina;
		this.name = name;
	}
	
	public override string ToString()
	{
		return string.Format("{0} {1} {2} {3} {4} {5}", type, attack, health, needJewel, needStamina, name);
	}
}

public class ReinforceAttributeInfo
{
	public AttributeValue.eAttributeType type = AttributeValue.eAttributeType.None;
	public float attributeValue = 0.0f;
}

public class ReinforceInfo
{
	public enum eReinforceType
	{
		None = -1,
		Offense,
		Defense,
		AddLife,
		AddExp,
	}
	public eReinforceType type = eReinforceType.Offense;
	public string infoMsg = "";
	public List<ReinforceAttributeInfo> attributeInfoList = new List<ReinforceAttributeInfo>();
	public Vector3 needGold = Vector3.zero;
	
	public ReinforceInfo()
	{
		
	}
	
	public ReinforceInfo(ReinforceInfo oldInfo)
	{
		this.type = oldInfo.type;
		this.infoMsg = oldInfo.infoMsg;
		this.needGold = oldInfo.needGold;
		
		foreach(ReinforceAttributeInfo temp in oldInfo.attributeInfoList)
		{
			ReinforceAttributeInfo newAttributeInfo = new ReinforceAttributeInfo();
			newAttributeInfo.type = temp.type;
			newAttributeInfo.attributeValue = temp.attributeValue;
			
			this.attributeInfoList.Add(newAttributeInfo);
		}
	}
	
	public void AddAttribute(AttributeValue.eAttributeType type, float _value)
	{
		if (IsExistAttributeValue(type, _value) == true)
			return;
		
		ReinforceAttributeInfo newValue = new ReinforceAttributeInfo();
		newValue.type = type;
		newValue.attributeValue = _value;
		
		attributeInfoList.Add(newValue);
	}
	
	public bool IsExistAttributeValue(AttributeValue.eAttributeType type, float _value)
	{
		foreach(ReinforceAttributeInfo temp in attributeInfoList)
		{
			if (temp.type == type && temp.attributeValue == _value)
				return true;
		}
		
		return false;
	}
	
	public string GetAttributeInfo()
	{
		string msg = "";
		int nCount = attributeInfoList.Count;
		for(int index = 0; index < nCount; ++index)
		{
			ReinforceAttributeInfo info = attributeInfoList[index];
			string line = string.Format("{0}", AttributeValue.GetAttributeName(info.type));
			
			if (index == 0)
			{
				msg = line;
			}
			else
			{
				msg += "/" + line;
			}
			
			if (index == (nCount - 1))
			{
				msg += string.Format("{0:#,###,###.##}%", info.attributeValue * 100.0f);
			}
		}
		
		return msg;
	}
}

public class WaveStartInfo : BaseTable {
	public List<TowerInfo> towerInfoList = new List<TowerInfo>();
	public List<ReinforceInfo> reinforceInfoList = new List<ReinforceInfo>();
	
	public Vector4 staminaRecoverPrice = Vector4.zero;
	public WaveStartInfo()
	{
		
	}
	
	public ReinforceInfo GetBuffInfo(int index)
	{
		ReinforceInfo info = null;
		int nCount = reinforceInfoList.Count;
		if (index < 0 || index >= nCount)
			return info;
		
		info = reinforceInfoList[index];
		return info;
	}
	
	public TowerInfo GetTowerInfo(int index)
	{
		TowerInfo info = null;
		int nCount = towerInfoList.Count;
		if (index < 0 || index >= nCount)
			return info;
		
		info = towerInfoList[index];
		return info;
	}
	
	public ReinforceInfo GetReinforceInfo(int index)
	{
		ReinforceInfo info = null;
		int nCount = reinforceInfoList.Count;
		if (index < 0 || index >= nCount)
			return info;
		
		info = reinforceInfoList[index];
		return info;
	}
	
	public void InitValue()
	{
		towerInfoList.Clear();
		reinforceInfoList.Clear();
	}
	
	public int FindReinforceIndex(ReinforceInfo info)
	{
		int selectedIndex = -1;
		int nCount = reinforceInfoList.Count;
		for (int index = 0; index < nCount; ++index)
		{
			ReinforceInfo temp = reinforceInfoList[index];
			if (temp != null && info != null &&
				temp.type == info.type)
			{
				selectedIndex = index;
				break;
			}
		}
		
		return selectedIndex;
	}
	
	public int FindTowerIndex(TowerInfo info)
	{
		int selectedIndex = -1;
		int nCount = this.towerInfoList.Count;
		for (int index = 0; index < nCount; ++index)
		{
			TowerInfo temp = towerInfoList[index];
			if (temp == info)
			{
				selectedIndex = index;
				break;
			}
		}
		
		return selectedIndex;
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
	
	public override void LoadTableFromXML(XmlDocument xmlDoc)
	{
		InitValue();
		
		XmlNodeList data = xmlDoc.GetElementsByTagName("Wave");
		for (int i = 0; i < data.Count; ++i)
		{
			XmlNode dataChilds = data.Item(i);
			
			XmlNodeList allObjects = dataChilds.ChildNodes;
			
			for (int index = 0; index < allObjects.Count; ++index)
			{
				XmlNode node = allObjects.Item(index);
				
				if (node == null)
					continue;
				
				if (node.Name == "StaminaRecoverPrice")
				{
					string[] priceInfos = node.InnerText.Split(',');
					staminaRecoverPrice.x = float.Parse(priceInfos[0]);
					staminaRecoverPrice.y = float.Parse(priceInfos[1]);
					staminaRecoverPrice.z = float.Parse(priceInfos[2]);
					staminaRecoverPrice.w = float.Parse(priceInfos[3]);
				}
				else if (node.Name == "Tower")
				{
					TowerInfo.eTowerType towerType = TowerInfo.eTowerType.Wood;
					float attackValue = 0.0f;
					float healthValue = 0.0f;
					float needJewel = 0.0f;
					float needStamina = 0.0f;
					string name = "";
					
					string[] towerInfos = node.InnerText.Split(',');
					
					string towerTypeStr = towerInfos[0];
					if (towerTypeStr == "Wood")
						towerType = TowerInfo.eTowerType.Wood;
					else if (towerTypeStr == "Stone")
						towerType = TowerInfo.eTowerType.Stone;
					else if (towerTypeStr == "Iron")
						towerType = TowerInfo.eTowerType.Iron;
					
					attackValue = float.Parse(towerInfos[1]);
					healthValue = float.Parse(towerInfos[2]);
					needJewel = float.Parse(towerInfos[3]);
					needStamina = float.Parse(towerInfos[4]);
					name = towerInfos[5];
					
					TowerInfo newTowerInfo = new TowerInfo(towerType, attackValue, healthValue, needJewel, needStamina, name);
					towerInfoList.Add(newTowerInfo);
				}
				else if (node.Name == "Reinforce")
				{
					ReinforceInfo newReinforce = new ReinforceInfo();
					LoadReinforce(node, newReinforce);
					
					reinforceInfoList.Add(newReinforce);
				}
			}
		}
	}
	
	public void LoadReinforce(XmlNode rootNode, ReinforceInfo reinforceInfo)
	{
		XmlNodeList allObjects = rootNode.ChildNodes;
		
		for (int index = 0; index < allObjects.Count; ++index)
		{
			XmlNode node = allObjects.Item(index);
			
			if (node == null)
				continue;
			
			if (node.Name == "Type")
			{
				string typeStr = node.InnerText;
				ReinforceInfo.eReinforceType reinforceType = ReinforceInfo.eReinforceType.Offense;
				if (typeStr == "Offense")
					reinforceType = ReinforceInfo.eReinforceType.Offense;
				else if (typeStr == "Defense")
					reinforceType = ReinforceInfo.eReinforceType.Defense;
				else if (typeStr == "AddLife")
					reinforceType = ReinforceInfo.eReinforceType.AddLife;
				else if (typeStr == "AddExp")
					reinforceType = ReinforceInfo.eReinforceType.AddExp;
				
				reinforceInfo.type = reinforceType;
			}
			else if (node.Name == "Info")
			{
				reinforceInfo.infoMsg = node.InnerText;
			}
			else if (node.Name == "Attribute")
			{
				string[] attributeInfos = node.InnerText.Split(',');
				
				AttributeValue.eAttributeType attValueType = AttributeValue.eAttributeType.None;
				float attributeValue = 0.0f;
				
				attValueType = AttributeValue.GetAttributeType(attributeInfos[0]);
				attributeValue = float.Parse(attributeInfos[1]);
				
				reinforceInfo.AddAttribute(attValueType, attributeValue);
			}
			else if (node.Name == "Gold")
			{
				string[] goldInfos = node.InnerText.Split(',');
				
				reinforceInfo.needGold.x = float.Parse(goldInfos[0]);
				reinforceInfo.needGold.y = float.Parse(goldInfos[1]);
				reinforceInfo.needGold.z = float.Parse(goldInfos[2]);
			}
		}
	}
}
