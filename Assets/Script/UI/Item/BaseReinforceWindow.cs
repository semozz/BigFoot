using UnityEngine;
using System.Collections;

public class BaseReinforceWindow : PopupBaseWindow {
	public enum eReinforceStep
	{
		None,
		Wait,
		Progress,
		Result,
		ResultWait,
	}
	public eReinforceStep curStep = eReinforceStep.Wait;
	
	public string resultMsg = "";
	
	public UILabel titleLabel = null;
	public UISlider progressBar = null;
	
	public BaseItemWindow parentWindow = null;
	public Color origPlusColor = Color.green;
	public Color origMinusColor = Color.magenta;
	
	public Color minusColor = Color.blue;
	public Color plusColor = Color.red;
	
	public UIButton startButton = null;
	public UILabel startLabel = null;
	public UILabel itemInfos = null;
	
	public float progressTime = 1.5f;
	public float progressDelayTime = 0.0f;
	
	public float resultWaitTime = 1.0f;
	public float resultDelayTime = 0.0f;
	
	public GameObject progressFx = null;
	public GameObject successFx = null;
	public GameObject failFx = null;
	public bool bSuccess = false;
	protected NetErrorCode resultErrorCode = NetErrorCode.OK;
	
	
	public UILabel needGoldLabel = null;
	public UILabel costLabel = null;
	public int costLabelStringID = -1;
	
	public uint resultItemExp = 0;
	
	public string resultPopupPrefab = "UI/Item/Composition/ReinforceResultWindow";
	public override void Awake()
	{
		base.Awake();
		
		SetFX(progressFx, false);
		SetFX(successFx, false);
		SetFX(failFx, false);
		
		if (startButton != null)
			startButton.defaultColor = Color.white;
	}
	
	public int needGold = 0;
	public void SetNeedGold(int goldValue)
	{
		CharInfoData charData = Game.Instance.charInfoData;
		int ownGold = charData != null ? charData.gold_Value : 0;
		
		this.needGold = goldValue;
		
		string needGoldStr = "";
		needGoldStr = string.Format("{0}{1:#,###,###,##0}[-]", GameDef.RGBToHex(Color.white), goldValue);
		
		if (needGoldLabel != null)
			needGoldLabel.text = needGoldStr;
	}
	
	public virtual void Update()
	{
		
	}
	
	public virtual void SetMode(eReinforceStep step)
	{
		if (curStep == step)
			return;
		
		curStep = step;
		switch(curStep)
		{
		case eReinforceStep.None:
		case eReinforceStep.Wait:
			if (progressBar != null)
				progressBar.gameObject.SetActive(false);
			
			SetFX(progressFx, false);
			SetFX(successFx, false);
			SetFX(failFx, false);
			
			bSuccess = false;
			break;
		case eReinforceStep.Progress:
			if (progressBar != null)
			{
				progressBar.gameObject.SetActive(true);
				progressBar.sliderValue = 0.0f;
			}
			progressDelayTime = progressTime;
			
			SetFX(progressFx, true);
			SetFX(successFx, false);
			SetFX(failFx, false);
			break;
		case eReinforceStep.Result:
			if (progressBar != null)
				progressBar.gameObject.SetActive(true);
			
			resultDelayTime = resultWaitTime;
			
			SetFX(progressFx, false);
			
			SetResultFX(resultErrorCode);
			break;
		}
	}
	
	public virtual void SetResultFX(NetErrorCode errorCode)
	{
		if (resultErrorCode == NetErrorCode.OK)
			SetFX(successFx, bSuccess);
		else
			SetFX(failFx, !bSuccess);
			//OnErrorMessage(resultErrorCode, null);
	}
	
	public virtual void SetStartButtonEnable(bool bEnable)
	{
		if (startButton != null)
		{
			if (bEnable == true)
				startButton.isEnabled = false;
			
			startButton.isEnabled = bEnable;
			
			if (startLabel != null)
				startLabel.color = bEnable == true ? startButton.defaultColor : startButton.disabledColor;
		}
	}
	
	public override void OnBack()
	{
		if (curStep == eReinforceStep.None ||
			curStep == eReinforceStep.Wait ||
			curStep == eReinforceStep.ResultWait)
		{
			
			OnClose();

			if (parentWindow != null)
			{
				parentWindow.OnBack();
			}
		}
	}
	
	public virtual void OnClose()
	{
		if (curStep == eReinforceStep.None ||
			curStep == eReinforceStep.Wait ||
			curStep == eReinforceStep.ResultWait)
		{
			base.OnBack();

			this.gameObject.SetActive(false);
			
			if (parentWindow != null)
			{
				parentWindow.UpdateInventoryWindow(false);
				parentWindow.OnChildWindow(false);
			}
		}
	}
	
	public void OnStart()
	{
		if (curStep == eReinforceStep.Wait ||
			curStep == eReinforceStep.ResultWait)
		{
			//UseGold();
			
			CashItemType cashCheck = PopupBaseWindow.CheckNeedGold(this.needGold, 0, 0);
			
			if (cashCheck != CashItemType.None)
			{
				OnNeedMoneyPopup(cashCheck, this);
				return;
			}
			
			
			SendRequest();
			
			SetStartButtonEnable(false);
			SetMode(eReinforceStep.Progress);
		}
	}
	
	public virtual void SendRequest()
	{
		
	}
	
	public virtual void UseGold()
	{
		
	}
	
	protected void SetFX(GameObject fxObject, bool bPlay)
	{
		if (fxObject == null)
			return;
		
		if (bPlay == true)
		{
			AudioSource audioSource = fxObject.audio;
			if (audioSource != null)
				audioSource.mute = !GameOption.effectToggle;
			
			fxObject.SetActive(bPlay);
		}
		
		Transform[] childs = fxObject.GetComponentsInChildren<Transform>();
		if (childs == null || childs.Length == 0)
		{
			Animation rootAnim = fxObject.GetComponent<Animation>();
			if (rootAnim != null)
			{
				if (bPlay == true)
				{
					rootAnim.Play();
					rootAnim.Sample();
				}
				else
				{
					rootAnim.Stop();
				}
				
				foreach(AnimationState state in rootAnim)
					rootAnim[state.name].speed = 1.0f;
			}
			
			ParticleSystem particleSystem = fxObject.GetComponent<ParticleSystem>();
			if (particleSystem != null)
			{
				if (bPlay == true)
					particleSystem.Play();
				else
				{
					particleSystem.Stop();
					particleSystem.Clear();
					if (particleSystem.particleEmitter != null)
						particleSystem.particleEmitter.ClearParticles();
				}
				
				particleSystem.playbackSpeed = 1.0f;
			}
			else
			{
				if (fxObject.particleEmitter != null)
					fxObject.particleEmitter.emit = bPlay;
			}
		}
		else
		{
			for (int childIndex = 0; childIndex < childs.Length; ++childIndex)
			{
				GameObject child = childs[childIndex].gameObject;
				
				if (child != null)
				{
					Animation childAnim = child.GetComponent<Animation>();
					if (childAnim != null)
					{
						//Debug.Log("Animation Name : " + childAnim.name + (bPlay == true ? " Play" : " Stop"));
						
						if (childAnim.renderer != null)
							childAnim.renderer.enabled = bPlay;
						
						if (bPlay == true)
						{
							childAnim.Play();
							childAnim.Sample();
						}
						else
						{
							childAnim.Stop();
						}
						
						foreach(AnimationState state in childAnim)
							childAnim[state.name].speed = 1.0f;
					}
					
					ParticleSystem particleSystem = child.GetComponent<ParticleSystem>();
					if (particleSystem != null)
					{
						if (bPlay == true)
							particleSystem.Play();
						else
						{
							particleSystem.Stop();
							particleSystem.Clear();
							if (particleSystem.particleEmitter != null)
								particleSystem.particleEmitter.ClearParticles();
						}
						
						particleSystem.playbackSpeed = 1.0f;
					}
					else
					{
						if (child.particleEmitter != null)
							child.particleEmitter.emit = bPlay;
					}
				}
			}
		}
		
		if (bPlay == false)
			fxObject.SetActive(false);
	}
	
	
	protected Item origItem = null;
	protected Item resultItem = null;
	public virtual void OnResultPopup(Item origItem, Item reinforceItem)
	{
		
	}
	
	public virtual void CloseResultPopup()
	{
		
	}
	
	public void UpdateItemInfos(Item item, uint addExp)
	{
		if (itemInfos != null)
		{
			int charLevel = 1;
			PlayerController player = Game.Instance.player;
			if (player != null)
				charLevel = player.lifeManager.charLevel;
			
			TableManager tableManager = TableManager.Instance;
			ArenaItemRateTable arenaItemRateTable = tableManager != null ? tableManager.arenaItemRateTable : null;
			
			float arenaItemRate = arenaItemRateTable != null ? arenaItemRateTable.GetItemRate(charLevel) : 1.0f;
			
			string itemInfoStr = "";
			if (item != null)
			{
				bool isArenaItem = item.CheckArenaItem();
				if (isArenaItem == true)
					item.SetArenaItemRate(arenaItemRate);
				
				itemInfoStr = item.GetReinforceAttributeInfo(addExp, origPlusColor, origMinusColor, plusColor, minusColor);
				
				if (isArenaItem == true)
					item.ResetArenaItemRate();
			}
			
			itemInfos.text = itemInfoStr;
		}
	}
}
