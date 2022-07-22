using UnityEngine;
using System.Collections;

public class PlayerControlButtonPanel : MonoBehaviour {
	public ActionButton actionAButton = null;
	public ActionButton actionBButton = null;
	public ActionButton jumpButton = null;
	public ActionButton skill1Button = null;
	public ActionButton skill2Button = null;
	
	public GameObject potionRoot = null;
	public ItemActionButton potion1Button = null;
	public ItemActionButton potion2Button = null;
	
	public Joystic joystic = null;
	
	public PlayerController player = null;
	
	public CharInfoData charData = null;
	
	int limitPotionCount = 9;
	// Use this for initialization
	public bool potionEnableStage = false;
	void Start () {
		StageManager stageManager = GameObject.FindObjectOfType(typeof(StageManager)) as StageManager;
		
		potionEnableStage = false;
		if (stageManager != null)
			potionEnableStage = IsPotionEnableStage(stageManager.StageType);
		
		TableManager tableManager = TableManager.Instance;
		StringValueTable stringValueTable = tableManager != null ? tableManager.stringValueTable : null;
		if (stringValueTable != null)
			limitPotionCount = stringValueTable.GetData("LimitPotionCount");
		
		charData = Game.Instance.charInfoData;
		
		if (charData != null && potionEnableStage == true)
		{
			charData.equipPotion1Count = Mathf.Min(limitPotionCount, charData.potion1 + charData.potion1Present);
			charData.equipPotion2Count = Mathf.Min(limitPotionCount, charData.potion2 + charData.potion2Present);
		}
		
		if (potionRoot != null)
			potionRoot.SetActive(potionEnableStage);
	}
	
	void OnDestroy()
	{
		if (charData != null)
		{
			charData.equipPotion1Count = 0;
			charData.equipPotion2Count = 0;
		}
	}
	
	public bool IsPotionEnableStage(StageManager.eStageType type)
	{
		switch(type)
		{
			case StageManager.eStageType.ST_FIELD:
			case StageManager.eStageType.ST_EVENT:
			case StageManager.eStageType.ST_WAVE:
				return true;
		}
		return false;
	}
	
	public void InitButtons(CharacterButton actionA, CharacterButton actionB, CharacterButton skill1, CharacterButton skill2)
	{
		SetButton(actionAButton, actionA);
		SetButton(actionBButton, actionB);
		SetButton(skill1Button, skill1);
		SetButton(skill2Button, skill2);
	}
	
	public void SetButton(ActionButton actionButton, CharacterButton action)
	{
		if (actionButton != null && action != null)
		{
			actionButton.slider = action.slider;
			
			actionButton.defaultBackground = action.defaultSprite;
			actionButton.disableBackground = action.disableSprite;
			
			InitRootNode(action.transform, actionButton.rootNode);
		}
	}
	
	public void SetActionBIcon(string iconName)
	{
		if (this.actionBButton != null)
		{
			this.actionBButton.defaultBackground.spriteName = iconName;
			this.actionBButton.disableBackground.spriteName = string.Format("{0}_disable", iconName);
		}
	}
	
	public void SetEnableActionBButton(bool bEnable)
	{
		if (this.actionBButton != null)
			this.actionBButton.gameObject.SetActive(bEnable);
	}
	
	public void InitRootNode(Transform obj, Transform parent)
	{
		obj.parent = parent;
		
		obj.localPosition = Vector3.zero;
		obj.localScale = Vector3.one;
		obj.localRotation = Quaternion.identity;
	}
	
	// Update is called once per frame
	void Update () {
	
		float actionBCoolTimeRate = 0.0f;
		float skill1CoolTimeRate = 0.0f;
		float skill2CoolTimeRate = 0.0f;
		
		bool bEnableActionB = false;
		bool bEnableSkill1 = false;
		bool bEnableSkill2 = false;
		
		
		bool bEnablePotion1 = false;
		bool bEnablePotion2 = false;
		int potion1Count = 0;
		int potion2Count = 0;
		float potion1CoolTimeRate = 0.0f;
		float potion2CoolTimeRate = 0.0f;
		
		if (player != null && charData != null)
		{
			actionBCoolTimeRate = player.GetActionBCoolTimeRate();
			bEnableActionB = actionBCoolTimeRate <= 0.0f && player.CheckStateRequireAbilityValue(player.actionBStartState) == true;
			
			skill1CoolTimeRate = player.GetSkill1CoolTimeRate();
			bEnableSkill1 = skill1CoolTimeRate <= 0.0f && player.CheckStateRequireAbilityValue(BaseState.eState.Skill01) == true;
			
			skill2CoolTimeRate = player.GetSkill2CoolTimeRate();
			bEnableSkill2 = skill2CoolTimeRate <= 0.0f && player.CheckStateRequireAbilityValue(BaseState.eState.Skill02) == true;
			
			potion1CoolTimeRate = player.GetPotion1CoolTimeRate();
			potion1Count = Mathf.Min(limitPotionCount, charData.equipPotion1Count);
			bEnablePotion1 = potion1CoolTimeRate <= 0.0f && potion1Count > 0;
			
			potion2CoolTimeRate = player.GetPotion2CoolTimeRate();
			potion2Count = Mathf.Min(limitPotionCount, charData.equipPotion2Count);
			bEnablePotion2 = potion2CoolTimeRate <= 0.0f && potion2Count > 0;
		}
		
		if (actionBButton != null)
		{
			actionBButton.SetCoolTimeRate(actionBCoolTimeRate);
			actionBButton.SetEnable(bEnableActionB);
		}
		
		if (skill1Button != null)
		{
			skill1Button.SetCoolTimeRate(skill1CoolTimeRate);
			skill1Button.SetEnable(bEnableSkill1);
		}
		
		if (skill2Button != null)
		{
			skill2Button.SetCoolTimeRate(skill2CoolTimeRate);
			skill2Button.SetEnable(bEnableSkill2);
		}
		
		if (potion1Button != null)
		{
			potion1Button.SetCoolTimeRate(potion1CoolTimeRate);
			potion1Button.SetEnable(bEnablePotion1);
			
			potion1Button.SetItemCount(potion1Count);
		}
		
		if (potion2Button != null)
		{
			potion2Button.SetCoolTimeRate(potion2CoolTimeRate);
			potion2Button.SetEnable(bEnablePotion2);
			
			potion2Button.SetItemCount(potion2Count);
		}
		
		PlayerInput input = null;
		if (player != null)
			input = player.input;
		
		if (Game.Instance.Pause == true ||
			Game.Instance.InputPause == true)
		{
			if (input != null)
				input.ResetInput();
			return;
		}
	}
	
	public void OnActionAButtonPress()
	{
		PlayerInput input = null;
		if (player != null)
			input = player.input;
		
		if (input == null)
			return;
		
		if (Game.Instance.Pause == true ||
			Game.Instance.InputPause == true)
		{
			input.ResetInput();
			return;
		}
		
		if (player == null || player.IsAliveState() == false)
			return;
		
		if (player.lifeManager != null && player.lifeManager.stunDelayTime > 0.0f)
			return;
		
		if (input.enableAttackInput == true)
		{
			if (input.bActionAKeyPress == false)
			{
				input.bActionAKeyChanged = true;
				
				input.bActionAKeyPress = true;
				input.bActionAKeyRelease = false;
				
				//Debug.Log("Actin A Key Press..." + Time.time);
			}
			
			input.mActionAKey = true;
		}
	}
	
	public void OnActionAButtonRelease()
	{
		PlayerInput input = null;
		if (player != null)
			input = player.input;
		
		if (input == null)
			return;
		
		if (Game.Instance.Pause == true ||
			Game.Instance.InputPause == true)
		{
			if (input != null)
				input.ResetInput();
			return;
		}
		
		input.mActionAKey = false;
		
		if (player == null || player.IsAliveState() == false)
			return;
		
		if (input.enableAttackInput == true)
		{
			if (input.bActionAKeyPress == true)
			{
				input.bActionAKeyChanged = true;
				
				input.bActionAKeyRelease = true;
				input.bActionAKeyPress = false;
				
				//Debug.Log("Actin A Key Release..." + Time.time);
			}
		}
	}
	
	public void OnActionBButtonClick()
	{
		PlayerInput input = null;
		if (player != null)
			input = player.input;
		
		if (input == null)
			return;
		
		if (Game.Instance.Pause == true ||
			Game.Instance.InputPause == true)
		{
			input.ResetInput();
			return;
		}
		
		if (player == null || player.IsAliveState() == false)
			return;
		
		if (player.GetActionBCoolTimeRate() > 0.0f ||
			player.CheckStateRequireAbilityValue(player.actionBStartState) == false)
		{
			input.mActionBKey = false;
			return;
		}
		
		input.mActionBKey = true;
	}
	
	public void OnActionBButtonRelease()
	{
		PlayerInput input = null;
		if (player != null)
			input = player.input;
		
		if (input == null)
			return;
		
		if (Game.Instance.Pause == true ||
			Game.Instance.InputPause == true)
		{
			input.ResetInput();
			return;
		}
		
		input.mActionBKey = false;
	}
	
	public void OnJumpButtonClick()
	{
		PlayerInput input = null;
		if (player != null)
			input = player.input;
		
		if (input == null)
			return;
		
		if (Game.Instance.Pause == true ||
			Game.Instance.InputPause == true)
		{
			input.ResetInput();
			return;
		}
		
		if (player == null || player.IsAliveState() == false)
		{
			input.mJumpKey = false;
			return;
		}
		
		input.mJumpKey = true;
		input.OnControlEvent(PlayerInput.eControlEvent.CE_JUMP);
	}

	
	public void OnSkill1ButtonClick()
	{
		PlayerInput input = null;
		if (player != null)
			input = player.input;
		
		if (input == null)
			return;
		
		if (Game.Instance.Pause == true ||
			Game.Instance.InputPause == true)
		{
			input.ResetInput();
			return;
		}
		
		if (player == null || player.IsAliveState() == false)
			return;
		
		if (player.lifeManager != null && player.lifeManager.stunDelayTime > 0.0f)
			return;
		
		if (player.GetSkill1CoolTimeRate() > 0.0f ||
			player.CheckStateRequireAbilityValue(BaseState.eState.Skill01) == false)
		{
			input.mSkill1Button = false;
			return;
		}
		
		input.mSkill1Button = true;
	}
	
	public void OnSkill1ButtonRelease()
	{
		PlayerInput input = null;
		if (player != null)
			input = player.input;
		
		if (input == null)
			return;
		
		if (Game.Instance.Pause == true ||
			Game.Instance.InputPause == true)
		{
			input.ResetInput();
			return;
		}
		
		input.mSkill1Button = false;
	}
	
	public void OnSkill2ButtonClick()
	{
		PlayerInput input = null;
		if (player != null)
			input = player.input;
		
		if (input == null)
			return;
		
		if (Game.Instance.Pause == true ||
			Game.Instance.InputPause == true)
		{
			input.ResetInput();
			return;
		}
		
		if (player == null || player.IsAliveState() == false)
			return;
		
		if (player.GetSkill2CoolTimeRate() > 0.0f ||
			player.CheckStateRequireAbilityValue(BaseState.eState.Skill02) == false)
		{
			input.mSkill2Button = false;
			return;
		}
		
		input.mSkill2Button = true;
	}
	
	public void OnSkill2ButtonRelease()
	{
		PlayerInput input = null;
		if (player != null)
			input = player.input;
		
		if (input == null)
			return;
		
		if (Game.Instance.Pause == true ||
			Game.Instance.InputPause == true)
		{
			input.ResetInput();
			return;
		}
		
		input.mSkill2Button = false;
	}
	
	public void OnPotion1Click()
	{
		if (Game.Instance.Pause == true ||
			Game.Instance.InputPause == true)
		{
			return;
		}
		
		if (player == null || player.IsAliveState() == false)
			return;
		
		if (player.lifeManager != null && player.lifeManager.stunDelayTime > 0.0f)
			return;
		
		if (player.GetPotion1CoolTimeRate() > 0.0f)
			return;
		
		player.UsePotion(ItemInfo.eItemType.Potion_1);
	}
	
	public void OnPotion2Click()
	{
		if (Game.Instance.Pause == true ||
			Game.Instance.InputPause == true)
		{
			return;
		}
		
		if (player == null || player.IsAliveState() == false)
			return;
		
		if (player.lifeManager != null && player.lifeManager.stunDelayTime > 0.0f)
			return;
		
		if (player.GetPotion2CoolTimeRate() > 0.0f)
			return;
		
		player.UsePotion(ItemInfo.eItemType.Potion_2);
	}
	
	
	public void SetTutorialMode(int step)
	{
		System.Collections.Generic.List<GameObject> groupList = new System.Collections.Generic.List<GameObject>();
		groupList.Add(this.joystic.gameObject);
		groupList.Add(this.actionAButton.gameObject);
		groupList.Add(this.jumpButton.gameObject);
		groupList.Add(null);
		groupList.Add(this.skill2Button.gameObject);
		groupList.Add(this.skill1Button.gameObject);
		groupList.Add(this.potionRoot);
		
		int nCount = groupList.Count;
		for(int index = 0; index < nCount; ++index)
		{
			GameObject obj = groupList[index];
			if (obj == null)
				continue;
			
			bool isAvtive = (step >= index);
			obj.SetActive(isAvtive);
		}
	}
}
