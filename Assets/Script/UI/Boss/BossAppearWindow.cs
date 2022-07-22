using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BossInfo
{
	public int id = 0;
	public string bossSpriteName = "";
	public string dialogStr = "";
}

[System.Serializable]
public class KingdomInfo
{
	public int id = 0;
	public string kingdomName = "";
	public string kingdomFalg = "";
}

public class BossAppearWindow : MonoBehaviour {

	public UILabel bossNameLabel = null;
	public UILabel bossDialogLabel = null;
	
	public UISprite kingdomFlagSprite = null;
	public UILabel kingdomNameLabel = null;
	
	public UITexture bossFace = null;
	
	public UIButtonMessage buttonMessage = null;
	
	public float lifeTime = 5.0f;
	public float minLifeTime = 1.0f;
	
	public List<BossInfo> bossInfos = new List<BossInfo>();
	public List<KingdomInfo> kingdomInfos = new List<KingdomInfo>();
	
	private System.DateTime createTime;
	
	public AudioSource audioSource = null;
	void Start()
	{
		if (audioSource != null)
			audioSource.mute = !GameOption.effectToggle;
	}
	
	public void SetBoss(int id)
	{
		BossInfo info = GetBossInfo(id);
		
		TableManager tableManager = TableManager.Instance;
		BossRaidTable bossRaidTable = tableManager != null ? tableManager.bossRaidTable : null;
		
		string bossName = "";
		string dialogStr = "";
		string kingdomNameStr = "";
		string kingdomFlagStr = "";
		
		BossRaidData bossRaidData = null;
		if (bossRaidTable != null)
			bossRaidData = bossRaidTable.GetData(id);
		
		int kingdomID = 0;
		if (bossRaidData != null)
		{
			bossName = bossRaidData.bossName;
			kingdomID = bossRaidData.kingdomID;
		}
		
		KingdomInfo kingdomInfo = GetKingdomInfo(kingdomID);
		
		if (info != null)
		{
			if (bossFace != null)
				bossFace.mainTexture = LoadTexture(info.bossSpriteName);
			
			dialogStr = info.dialogStr;
		}
		
		if (kingdomInfo != null)
		{
			kingdomNameStr = kingdomInfo.kingdomName;
			kingdomFlagStr = kingdomInfo.kingdomFalg;
		}
		
		if (bossNameLabel != null)
			bossNameLabel.text = bossName;
		
		if (bossDialogLabel != null)
			bossDialogLabel.text = dialogStr;
		
		if (kingdomFlagSprite != null)
			kingdomFlagSprite.spriteName = kingdomFlagStr;
		if (kingdomNameLabel != null)
			kingdomNameLabel.text = kingdomNameStr;
		
		
		createTime = System.DateTime.Now;
		
		Invoke("OnOk", lifeTime);
	}
	
	public BossInfo GetBossInfo(int id)
	{
		BossInfo info = null;
		foreach(BossInfo temp in bossInfos)
		{
			if (temp.id == id)
			{
				info = temp;
				break;
			}
		}
		
		return info;
	}
	
	public KingdomInfo GetKingdomInfo(int id)
	{
		KingdomInfo info = null;
		foreach(KingdomInfo temp in kingdomInfos)
		{
			if (temp.id == id)
			{
				info = temp;
				break;
			}
		}
		
		return info;
	}
	
	public void OnOk(GameObject obj)
	{
		System.TimeSpan timeSpan = System.DateTime.Now - createTime;
		if (timeSpan.TotalSeconds < (double)minLifeTime)
			return;
		
		OnOk();
	}
	
	public void OnOk()
	{
		DestroyObject(this.gameObject, 0.1f);

        Game.Instance.SetBossAppear(-1);
		
		TownUI townUI = GameUI.Instance.townUI;
		if (townUI != null)
			townUI.OnEnterTown();
	}
	
	public string texturePath = "IMG/UI/Speak_UpperBody/";
	public Texture LoadTexture(string textureName)
	{
		string pathStr = string.Format("{0}{1}", texturePath, textureName);
		//Texture texture = (Texture2D)Resources.Load(pathStr);
		Texture texture = ResourceManager.LoadTexture(pathStr);
		
		return texture;
	}
}
