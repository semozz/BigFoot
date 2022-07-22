using UnityEngine;
using System.Collections;

public class MonsterCreateTask : TutorialTask
{
	public MonsterGenerator monsterGenerator = null;
	public MonsterKillCounter monsterKillCounter = null;
	
	
	public override void DoStart ()
	{
		base.DoStart ();
		
		if (monsterGenerator != null)
			monsterGenerator.OnActivate();
		if (monsterKillCounter != null)
			monsterKillCounter.OnActivate();
	}
	
	public override void DoEnd ()
	{
		base.DoEnd ();
		
		if (monsterGenerator != null)
			monsterGenerator.OnDeactivate();
	}
	
	public override bool IsComplete ()
	{
		if (monsterKillCounter != null)
			return monsterKillCounter._EventCondtion.IsComplete;
		else
			return base.IsComplete();
	}
}