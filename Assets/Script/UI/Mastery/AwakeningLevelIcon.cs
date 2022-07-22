using UnityEngine;
using System.Collections;

public class AwakeningLevelIcon : MonoBehaviour {

	public UILabel skillNameLabel = null;
	public UILabel curPointLabel = null;
	public UILabel skillInfoLabel = null;
	
	public UISprite skillIcon = null;
	public UISprite selectedSprite = null;
	public UISprite backgroundSprite = null;
	
	public UIButtonMessage message = null;
	
	private AwakeningLevel awakeningSkill = null;
	
	public Color deactiveColor = Color.gray;
	public Color activeColor = Color.white;
	
	public void SetSkillInfo(AwakeningLevel skill)
	{
		awakeningSkill = skill;
		
		SetIcon();
		
		UpdateInfo();
		
		SetSelectFrame(false);
	}
	
	public void SetIcon()
	{
		if (skillIcon != null)
		{
			string iconName = awakeningSkill != null ? awakeningSkill.skillIconName : "";
			skillIcon.spriteName = iconName;
		}
	}
	
	public AwakeningLevel GetSkill()
	{
		return awakeningSkill;
	}
	
	public void UpdateInfo()
	{
		if (skillNameLabel != null)
			skillNameLabel.text = awakeningSkill != null ? awakeningSkill.skillName : "";
		
		if (curPointLabel != null)
			curPointLabel.text = awakeningSkill != null ? string.Format("{0}", awakeningSkill.Point) : "";
		
		if (skillInfoLabel != null)
			skillInfoLabel.text = awakeningSkill != null ? awakeningSkill.GetCurSkillInfo() : "";
		
		int curPoint = 0;
		if (awakeningSkill != null)
			curPoint = awakeningSkill.Point;
		
		/*
		Color spriteColor = Color.white;
		if (curPoint > 0)
			spriteColor = activeColor;
		else
			spriteColor = deactiveColor;
		
		if (backgroundSprite != null)
			backgroundSprite.color = spriteColor;
		*/
	}
	
	public void SetSelectFrame(bool bSelected)
	{
		if (selectedSprite != null)
			selectedSprite.gameObject.SetActive(bSelected);
		
		Color spriteColor = Color.white;
		if (bSelected == false)
			spriteColor = deactiveColor;
		else
			spriteColor = activeColor;
		
		if (skillIcon != null)
			skillIcon.color = spriteColor;
	}
}
