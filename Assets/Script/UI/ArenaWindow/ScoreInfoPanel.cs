using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreInfo
{
	public int score = 0;
	public string profilePicURL = "";
	public string profileName = "";
	public GameDef.ePlayerClass classType = GameDef.ePlayerClass.CLASS_WARRIOR;
	public string charName = "";
	
	public static  int Sort(ScoreInfo a, ScoreInfo b)
	{
		int aValue = a != null ? a.score : 0;
		int bValue = b != null ? b.score : 0;
		
		return aValue - bValue;
	}
}

public class ScoreInfoPanel : MonoBehaviour {
	public WebImageTexture profilePicture = null;
	public UISprite characterPicture = null;
	
	public UILabel scoreLabel = null;
	public UILabel profileNameLabel = null;
	public UILabel characterNameLabel = null;
	
	public UILabel waveStepLabel = null;
	public UILabel waveTimeLabel = null;
	
	public List<string> charFacePics = new List<string>();
	
	public UIButtonMessage buttonMessage = null;
	
	
	public List<UILabel> charLevelInfoLabels = new List<UILabel>();
	
	public enum eScoreType
	{
		None,
		eScoreInfo,
		eArenaRankingInfo,
		eWaveRankingInfo
	}
	public eScoreType dataType = eScoreType.None;
	
	// Use this for initialization
	void Start () {
	
	}
	
	public string GetCharFaceName(GameDef.ePlayerClass classType)
	{
		int slotIndex = (int)classType;
		int nCount = charFacePics.Count;
		
		string facePicName = "";
		if (slotIndex >= 0 && slotIndex < nCount)
			facePicName = charFacePics[slotIndex];
		
		return facePicName;
	}
	
	public string GetCharClassName(int charIndex)
	{
		string className = "";
		
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		if (stringTable != null)
			className = stringTable.GetData(charIndex + 1);
		
		return className;
	}
	
	public ScoreInfo scoreInfo = null;
	public void SetInfo(ScoreInfo info)
	{
		dataType = eScoreType.eScoreInfo;
		scoreInfo = info;
		
		string scoreStr = "";
		string profilePicURL = "";
		string profileName = "";
		string charName = "";
		string charFacePic = "";
		
		if (info != null)
		{
			scoreStr = string.Format("{0:#,###,###}", info.score);
			profileName = info.charName;
			charName = GetCharClassName((int)info.classType);
			charFacePic = GetCharFaceName(info.classType);
			
			profilePicURL = info.profilePicURL;
		}
		
		if (scoreLabel != null)
			scoreLabel.text = scoreStr;
		if (profileNameLabel != null)
			profileNameLabel.text = profileName;
		if (characterNameLabel != null)
			characterNameLabel.text = charName;
		
		if (characterPicture != null)
			characterPicture.spriteName = charFacePic;
		if (profilePicture != null)
			profilePicture.SetURL(profilePicURL);
	}
	
	public ArenaRankingInfo arenaRankingInfo = null;
	public void SetInfo(ArenaRankingInfo info)
	{
		dataType = eScoreType.eArenaRankingInfo;
		arenaRankingInfo = info;
		
		string scoreStr = "";
		string profilePicURL = "";
		string profileName = "";
		string charName = "";
		string charFacePic = "";
		
		if (info != null)
		{
			scoreStr = string.Format("{0:#,###,###}", info.ranking);
			profileName = info.NickName;
			charName = GetCharClassName(info.CharacterIndex);
			charFacePic = GetCharFaceName((GameDef.ePlayerClass)info.CharacterIndex);
			
			profilePicURL = "";
		}
		
		if (scoreLabel != null)
			scoreLabel.text = scoreStr;
		if (profileNameLabel != null)
			profileNameLabel.text = profileName;
		if (characterNameLabel != null)
			characterNameLabel.text = charName;
		
		if (characterPicture != null)
			characterPicture.spriteName = charFacePic;
		if (profilePicture != null)
			profilePicture.SetURL(profilePicURL);
		
		SetCharLevelInfo(info.charLevels);
	}
	
	private void SetCharLevelInfo(int[] charLevels)
	{
		foreach(UILabel label in charLevelInfoLabels)
			label.text = string.Format("{0}", "--");
		
		if (charLevels != null)
		{
			int nCount = Mathf.Min(charLevels.Length, charLevelInfoLabels.Count);
			UILabel label = null;
			for (int index = 0; index < nCount; ++index)
			{
				label = charLevelInfoLabels[index];
				if (label != null)
					label.text = string.Format("{0}", charLevels[index]);
			}
		}
	}
	
	public WaveRankingInfo waveRankingInfo = null;
	public void SetInfo(WaveRankingInfo info)
	{
		dataType = eScoreType.eWaveRankingInfo;
		waveRankingInfo = info;
		
		string scoreStr = "";
		string profilePicURL = "";
		string profileName = "";
		string charName = "";
		string charFacePic = "";
		
		string waveStepStr = "";
		string waveTimeStr = "";
		
		if (info != null)
		{
			scoreStr = string.Format("{0:#,###,###}", info.ranking);
			profileName = info.NickName;
			charName = GetCharClassName(info.CharacterIndex);
			charFacePic = GetCharFaceName((GameDef.ePlayerClass)info.CharacterIndex);
			
			profilePicURL = "";
			
			if (info.RecordSec > 0)
			{
				System.TimeSpan time = Game.ToTimeSpan(info.RecordSec);
				waveTimeStr = string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds);
			}
			else
				waveTimeStr = "--:--:--";
			
			waveStepStr = string.Format("{0}", info.RecordStep);
		}
		
		if (scoreLabel != null)
			scoreLabel.text = scoreStr;
		if (profileNameLabel != null)
			profileNameLabel.text = profileName;
		if (characterNameLabel != null)
			characterNameLabel.text = charName;
		
		if (characterPicture != null)
			characterPicture.spriteName = charFacePic;
		if (profilePicture != null)
			profilePicture.SetURL(profilePicURL);
		
		if (waveStepLabel != null)
			waveStepLabel.text = waveStepStr;
		if (waveTimeLabel != null)
			waveTimeLabel.text = waveTimeStr;
	}
}
