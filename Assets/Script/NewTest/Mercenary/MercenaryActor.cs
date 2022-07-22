using UnityEngine;
using System.Collections;

public class MercenaryActor : BaseMonster {
	public bool  bSetupWaveRate = false;
	
	private MercenaryGenerator generator = null;
	public void SetMercenaryGenerator(MercenaryGenerator gen)
	{
		generator = gen;
	}
	
	
	public void SetMercenaryLevel(int level, MercenaryGenerateInfo.eMercenaryType type, float waveRate)
	{
		
	}
}
