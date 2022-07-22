using UnityEngine;
using System.Collections;

public class ArenaPlayerInput : PlayerInput {
	public ActorInfo target = null;
	public void ChangeTarget(ActorInfo targetInfo)
	{
		
	}
	
	public override void Update()
	{
		UpdateInput();
	}
	
	public override void UpdateInput ()
	{
		
	}
	
	public override void AddInputs (eControlKey key, bool pressed)
	{
		
	}
}
