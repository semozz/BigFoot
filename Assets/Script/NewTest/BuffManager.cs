using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuffManager : MonoBehaviour {
	
	public struct stBuff
    {
        public GameDef.eBuffType BuffType;
        public float StartTime;
        public float PeriodTime;
        public float AbilityValue;
        public float UpdateTime;
		
		public int StackCount;
		
		public bool bWillDelete;
		
		public LifeManager Owner;
		
		public FXInfo fxInfo;
		
		public float poisonInfectRate;
		public void InitValue()
		{
			BuffType = GameDef.eBuffType.BT_NONE;
			StartTime = 0.0f;
			PeriodTime = 0.0f;
			AbilityValue = 0.0f;
			UpdateTime = 0.0f;
			
			StackCount = 0;
			bWillDelete = false;
			
			Owner = null;
			fxInfo = null;
			
			poisonInfectRate = 0.0f;
			
			soundEffects = new List<SoundEffect>();
		}
		
		public List<SoundEffect> soundEffects;// = new List<SoundEffect>();
		public void AddSoundEffect(SoundEffect soundEffect)
		{
			if (soundEffect != null)
				soundEffects.Add(soundEffect);
		}
		
		public void StopSoundEffects()
		{
			foreach(SoundEffect effect in soundEffects)
			{
				effect.StopEffect();
				GameObject.DestroyObject(effect.gameObject, 0.0f);
			}
			
			soundEffects.Clear();
		}
    }
	
	public List<stBuff> mHaveBuff = new List<stBuff>();
	
	[HideInInspector]
	public LifeManager ownerActor = null;
	
	public float berserkModeAttackRate = 0.0f;
	
	public float changeBuffColorCooTime = 0.5f;
	public float changeBuffColorDelayTime = 0.0f;
	public Color currentBuffColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);
	
	private Color defaultBuffColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);
	
	public delegate FXInfo SelectBuffFX(GameDef.eBuffType buff);
	public SelectBuffFX onSelectBuffFX = null;
	
	
	public delegate void OnAddBuff(stBuff buff);
	private BaseMoveController moveController = null;
	
	public string poisonStartSound = "poison_start";
	public string poisonLoopSound = "poison_looping";
	public string curseStartSound = "curse_start";
	public string curseLoopSound = "curse_looping";
	public string manaShieldStartSound = "manaShield_start";
	public string manaShieldLoopSound = "manaShield_looping";
	
	void Awake()
	{
		moveController = GetComponent<BaseMoveController>();
		ownerActor = GetComponent<LifeManager>();
		
		ownerActor.buffManager = this;
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	public void UpdateBuff(int index, stBuff buff)
	{
		int nCount = mHaveBuff.Count;
		if (index < 0 || index >= nCount)
		{
			Debug.Log("index invalid.....");
		}
		else
			mHaveBuff[index] = buff;
	}
	
	// Update is called once per frame
	void Update () {
		float curHPValue = this.ownerActor.GetHP();
		if (curHPValue <= 0.01f)
		{
			Init();
			return;
		}
		
		float berserkRate = 0.0f;
		float tempMoveSpeedRate = 1.0f;
		
		List<Color> tempColors = new List<Color>();
        for (int i = mHaveBuff.Count - 1; i >= 0; i--)
        {
			if (i < 0 || i >= mHaveBuff.Count)
				break;
			
            stBuff buff = mHaveBuff [i];

            switch (buff.BuffType)
            {
            case GameDef.eBuffType.BT_CURSE:
                if (Time.time >= buff.UpdateTime + 3.0f)
                {
					if (IsInvincibleState() == false)
					{
						float fDamage = buff.AbilityValue / (buff.PeriodTime / 3.0f);
						float resultDamage = this.ownerActor.CalcMagicDamage(buff.Owner, fDamage);
						
						this.ownerActor.DecHP(resultDamage, buff.Owner, false, buff.BuffType);
					}

                    buff.UpdateTime += 3.0f;
                    //mHaveBuff[i] = buff;
					UpdateBuff(i, buff);
                }
				
				tempColors.Add(GameDef.CurseColor);
                break;
            case GameDef.eBuffType.BT_MANASHIELD :
                if (buff.AbilityValue <= 0.0f)
				{
					buff.PeriodTime = 0.0f;
					buff.StartTime = Time.time - Time.deltaTime;
				}
                break;
            case GameDef.eBuffType.BT_RED_POTION :
                if (Time.time >= buff.UpdateTime + 1.0f)
                {
                    float fValue = ownerActor.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.HealthMax) * 0.03f;
					
                    ownerActor.IncHP(fValue, true, buff.BuffType);
                    
					buff.UpdateTime += 1.0f;
                    buff.AbilityValue -= 0.03f;
					if (buff.AbilityValue <= 0.0f)
						buff.PeriodTime = 0.0f;

					//mHaveBuff[i] = buff;
					UpdateBuff(i, buff);
                }
                break;
            case GameDef.eBuffType.BT_YELLOW_POTION :
                break;
			case GameDef.eBuffType.BT_POISION:
				if (Time.time >= buff.UpdateTime)
                {
					if (IsInvincibleState() == false)
					{
						float fDamage = buff.AbilityValue * buff.StackCount;
						float resultDamage = this.ownerActor.CalcMagicDamage(buff.Owner, fDamage);
						
						this.ownerActor.DecHP(resultDamage, buff.Owner, false, buff.BuffType);
						
						if (buff.poisonInfectRate > 0.0f)
							PoisonInfectBuff(buff, buff.Owner);
					}
					
					buff.UpdateTime = Time.time + 2.0f;
					//mHaveBuff[i] = buff;
					UpdateBuff(i, buff);
                }
				
				tempColors.Add(GameDef.GetPoisonLevelColor(buff.StackCount - 1));
				break;
			case GameDef.eBuffType.BT_BERSERK:
				if (buff.PeriodTime != -1 &&
					Time.time >= buff.StartTime + buff.PeriodTime)
					berserkRate = 0.0f;
				else
					berserkRate += buff.AbilityValue;
				break;
			case GameDef.eBuffType.BT_SLOW:
				if (buff.PeriodTime != -1.0f && Time.time >= buff.StartTime + buff.PeriodTime)
					tempMoveSpeedRate *= 1.0f;
				else
					tempMoveSpeedRate *= buff.AbilityValue;
				break;
			case GameDef.eBuffType.BT_REGENHP :
				//5초 동안 1초에 한번씩 (1/5)만큼 hp 채운다.
                if (Time.time >= buff.UpdateTime + 1.0f)
                {
                    float fValue = buff.AbilityValue * 0.2f;
					
                    ownerActor.IncHP(fValue, true, buff.BuffType);
                    
					buff.UpdateTime += 1.0f;
                    
					UpdateBuff(i, buff);
                }
                break;
			case GameDef.eBuffType.BT_REFLECTDAMAGE:
				tempColors.Add(GameDef.ReflectColor);
				break;
			case GameDef.eBuffType.BT_BOSSRAID_PHASE2:
				tempColors.Add(GameDef.BerserkColor);
				break;
            }

            if (buff.PeriodTime != -1.0f && Time.time >= buff.StartTime + buff.PeriodTime)
			{
				ownerActor.RemoveFXDelayInfo(buff.fxInfo);
				
				buff.bWillDelete = true;
				
				//mHaveBuff[i] = buff;
				UpdateBuff(i, buff);
			}
        }
		
		if (berserkRate != 0.0f)
			tempColors.Add(GameDef.BerserkColor);
		
		if (tempMoveSpeedRate < 1.0f)
			tempColors.Add(GameDef.SlowColor);
		
		SetMoveSpeedRate(tempMoveSpeedRate);
		
		UpdateDeleteBuff();
		
		UpdateBuffColor(tempColors);
		
		changeBuffColorDelayTime -= Time.deltaTime;
	}
	
	protected bool applyBuffColor = false;
	public void UpdateBuffColor(List<Color> tempColors)
	{
		Color selectedColor = Color.white;
		if (tempColors.Count == 0)
			tempColors.Add(defaultBuffColor);
			
		int selectedColorIndex = Random.Range(0, tempColors.Count);
		selectedColor = tempColors[selectedColorIndex];
		//bool applyBuffColor = false;
		
		//현재 기본 버프 없은 상태에서
		if (currentBuffColor == defaultBuffColor)
		{
			//버프 색상이 선택되면 색상 변경...
			if (currentBuffColor != selectedColor)
			{
				currentBuffColor = selectedColor;
				applyBuffColor = true;
				changeBuffColorDelayTime = changeBuffColorCooTime;
			}
		}
		else
		{
			//버프 상태에서 버프 없는 상태로 변경 되는 경우 바로 적용
			if (selectedColor == defaultBuffColor)
			{
				currentBuffColor = selectedColor;
				applyBuffColor = true;
				changeBuffColorDelayTime = changeBuffColorCooTime;
			}
			//버프 상태인 경우 다른 버프 상태로는 시간 경과 후 적용
			else if (changeBuffColorDelayTime <= 0.0f && currentBuffColor != selectedColor)
			{
				currentBuffColor = selectedColor;
				applyBuffColor = true;
				changeBuffColorDelayTime = changeBuffColorCooTime;
			}
		}
		
//		if (applyBuffColor == false)
//			return;
		
		float changeColorRate = 1.0f;
		if (changeBuffColorCooTime != 0.0f)
			changeColorRate = 1.0f - (changeBuffColorDelayTime / changeBuffColorCooTime);
				
		
		UpdateBodyColor(currentBuffColor, changeColorRate);
		
		if (changeBuffColorDelayTime <= 0.0f)
		{
			changeBuffColorDelayTime = changeBuffColorCooTime;
			applyBuffColor = false;	
		}
	}
	
	public void UpdateBodyColor(Color buffColor, float changeRate)
	{
		if (ownerActor != null)
			ownerActor.UpdateBodyColor(buffColor, changeRate);
	}
	
	public void UpdateDeleteBuff()
	{
		for (int i = mHaveBuff.Count - 1; i >= 0; i--)
        {
            stBuff buff = mHaveBuff [i];
		
			if (buff.bWillDelete == true)
			{
				RemoveBuffAttribueValue(buff);
				mHaveBuff.RemoveAt(i);
			}
		}
	}
	
	public bool IsInvincibleState()
	{
		return ownerActor.IsInvincibleState();
	}
	
	public virtual void AddBuff(GameDef.eBuffType e, float fValue, float fPeriodTime, LifeManager owner, int stackCount)
	{
		float hpRate = this.ownerActor.GetHPRate();
		if (hpRate <= 0.0f)
			return;
		
		FXInfo fxInfo = null;
		if (onSelectBuffFX != null)
			fxInfo = onSelectBuffFX(e);
		
		AddBuffEffect(e, fValue, fPeriodTime, owner, fxInfo, stackCount);
	}
	
	protected void AddBuffEffect(GameDef.eBuffType e, float fValue, float fPeriodTime, LifeManager owner, FXInfo fxInfo, int stackCount)
    {
        stBuff buff = new stBuff();
		buff.InitValue();
		
        buff.BuffType = e;
        buff.AbilityValue = fValue;
        buff.StartTime = Time.time;
        buff.PeriodTime = fPeriodTime;
        buff.UpdateTime = buff.StartTime;
		
		buff.StackCount = stackCount;
		buff.Owner = owner;
		buff.fxInfo = fxInfo;
		
		string startSound = "";
		string loopSound = "";
		
		switch(e)
		{
		case GameDef.eBuffType.BT_INVINCIBLE:
		case GameDef.eBuffType.BT_MANASHIELD:
			ownerActor.ResetReceivePainValue();
			ownerActor.ResetPainDelayTime();
			break;
		case GameDef.eBuffType.BT_DEC_ATTACK_RATE:
			this.ownerActor.attributeManager.SubValueRate(AttributeValue.eAttributeType.AbilityPower, buff.AbilityValue);
			this.ownerActor.attributeManager.SubValueRate(AttributeValue.eAttributeType.AttackDamage, buff.AbilityValue);
			break;
		case GameDef.eBuffType.BT_BOSSRAID_PHASE2:
			this.ownerActor.attributeManager.AddValueRate(AttributeValue.eAttributeType.AbilityPower, buff.AbilityValue);
			this.ownerActor.attributeManager.AddValueRate(AttributeValue.eAttributeType.AttackDamage, buff.AbilityValue);
			break;
		}
		
		switch(e)
		{
		case GameDef.eBuffType.BT_POISION:
			startSound = poisonStartSound;
			loopSound = poisonLoopSound;
			
			float incPoisonInfectRate = 0.0f;
			if (owner != null)
				incPoisonInfectRate = owner.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.IncPoisonInfectRate);
			if (incPoisonInfectRate > 0.0f)
				buff.poisonInfectRate = incPoisonInfectRate;
			break;
		case GameDef.eBuffType.BT_CURSE:
			startSound = curseStartSound;
			loopSound = curseLoopSound;
			break;
		case GameDef.eBuffType.BT_MANASHIELD:
			startSound = manaShieldStartSound;
			loopSound = manaShieldLoopSound;
			break;
		}
		
		AddBuffSound(buff, startSound, loopSound);
		
        mHaveBuff.Add(buff);
		
		if (fxInfo != null && ownerActor != null)
			ownerActor.AddFXDelayInfo(fxInfo, fPeriodTime);
    }

    public int GetBuff(GameDef.eBuffType e)
    {
        for (int i=0;i < mHaveBuff.Count;i++)
        {
            if (mHaveBuff[i].BuffType == e && mHaveBuff[i].bWillDelete == false)
                return i;
        }

        return -1;
    }
	
	public int GetAppliedBuffIndex(GameDef.eBuffType buffType, LifeManager owner)
	{
		for (int i=0;i < mHaveBuff.Count;i++)
        {
            if (mHaveBuff[i].BuffType == buffType && mHaveBuff[i].Owner == owner && mHaveBuff[i].bWillDelete == false)
                return i;
        }
		
		return -1;
	}
	
	public void RemoveBuff(int index)
	{
		if (index < 0 || index >= mHaveBuff.Count)
			return;
		
		
		if (mHaveBuff[index].fxInfo != null)
		{
			ownerActor.RemoveFXDelayInfo(mHaveBuff[index].fxInfo);
		}
		
		stBuff info = mHaveBuff[index];
		
		RemoveBuffAttribueValue(info);
		
		mHaveBuff.RemoveAt(index);
	}
	
	public void RemoveBuffAttribueValue(stBuff info)
	{
		switch(info.BuffType)
		{
		case GameDef.eBuffType.BT_INVINCIBLE:
		case GameDef.eBuffType.BT_MANASHIELD:
			ownerActor.ResetReceivePainValue();
			ownerActor.ResetPainDelayTime();
			
			if (info.BuffType == GameDef.eBuffType.BT_MANASHIELD)
			{
				float incAbilityPowerRate = ownerActor.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.IncAbilityPowerWhenManaShield);
				if (incAbilityPowerRate != 0.0f)
					ownerActor.attributeManager.SubValueRate(AttributeValue.eAttributeType.AbilityPower, incAbilityPowerRate);
			}
			break;
		case GameDef.eBuffType.BT_REFLECTDAMAGE:
			this.ownerActor.ApplyReflectDamage(info.AbilityValue);
			break;
		case GameDef.eBuffType.BT_DEC_ATTACK_RATE:
			this.ownerActor.attributeManager.AddValueRate(AttributeValue.eAttributeType.AbilityPower, info.AbilityValue);
			this.ownerActor.attributeManager.AddValueRate(AttributeValue.eAttributeType.AttackDamage, info.AbilityValue);
			break;
		case GameDef.eBuffType.BT_SLOW:
			break;
		case GameDef.eBuffType.BT_BOSSRAID_PHASE2:
			this.ownerActor.attributeManager.SubValueRate(AttributeValue.eAttributeType.AbilityPower, info.AbilityValue);
			this.ownerActor.attributeManager.SubValueRate(AttributeValue.eAttributeType.AttackDamage, info.AbilityValue);
			break;
		}
		
		info.StopSoundEffects();
	}
	
	public void SetMoveSpeedRate(float speedRate)
	{
		if (moveController != null)
			moveController.SpeedRate = speedRate;
		
		if (ownerActor != null)
			ownerActor.ChangeAnimationSpeed(speedRate);
	}
	
	public void Init()
	{
		int nCount = mHaveBuff.Count;
		while(nCount > 0)
		{
			RemoveBuff(0);
			
			nCount = mHaveBuff.Count;
		}
	}
	
	public float infectionDist = 1.5f;
	public void PoisonInfectBuff(stBuff buff, LifeManager attacker)
	{
		float rate = buff.poisonInfectRate * buff.StackCount;
		
		int rateValue = Mathf.RoundToInt(rate * 100.0f);
		int randValue = Random.Range(0, 100);
		bool isInfection = rateValue >= randValue;
		if (isInfection == true)
		{
			ActorManager actorManager = ActorManager.Instance;
			if (actorManager != null)
			{
				List<ActorInfo> actorList = actorManager.GetActorList(ownerActor.myActorInfo.myTeam);
				List<LifeManager> infectActorList = new List<LifeManager>();
				
				if (actorList != null)
				{
					LifeManager actor = null;
					int buffIndex = -1;
					Vector3 vPos = this.ownerActor.transform.position;
					
					foreach(ActorInfo info in actorList)
					{
						actor = info.gameObject.GetComponent<LifeManager>();
						if (this.ownerActor == actor)
							continue;
						
						if (actor == null || actor.GetHPRate() <= 0.0f)
							continue;
						
						Vector3 vDiff = actor.gameObject.transform.position - vPos;
						float diff_distance = Mathf.Max(0.0f, Mathf.Abs(vDiff.magnitude) - (ownerActor.myActorInfo.colliderRadius + actor.myActorInfo.colliderRadius));
						
						if (diff_distance > infectionDist)
							continue;
						
						buffIndex = actor.buffManager.GetBuff(GameDef.eBuffType.BT_POISION);
						if (buffIndex != -1)
							continue;
						
						infectActorList.Add(actor);
					}
					
					foreach(LifeManager targetActor in infectActorList)
						targetActor.buffManager.AddBuff(GameDef.eBuffType.BT_POISION, buff.AbilityValue, buff.PeriodTime, attacker, 1);
				}
			}
		}
	}
	
	public bool HasDebuff()
	{
		bool hasDebuff = false;
		foreach(stBuff buff in mHaveBuff)
		{
			switch(buff.BuffType)
			{
			case GameDef.eBuffType.BT_CURSE:
			case GameDef.eBuffType.BT_POISION:
			case GameDef.eBuffType.BT_SLOW:
				hasDebuff = true;
				break;
			}
		}
		
		return hasDebuff;
	}
	
	public void AddBuffSound(stBuff buff, string startSound, string loopSound)
	{
		if (startSound == "" && loopSound == "")
			return;
		
		if (ownerActor.stateController != null)
		{
			SoundManager soundMangaer = ownerActor.stateController.soundManager;
			if (soundMangaer != null)
			{
				string soundEffectPrefab = soundMangaer.soundEffectPrefab;
				GameObject soundRoot = soundMangaer.soundRoot;
				
				SoundEffect soundEffect = ResourceManager.CreatePrefab<SoundEffect>(soundEffectPrefab, soundRoot.transform, Vector3.zero);
				if (soundEffect != null)
				{
					soundEffect.PlayEffect(startSound, loopSound);
					
					buff.AddSoundEffect(soundEffect);
				}
			}
		}
	}
}
