using UnityEngine;
using System.Collections;


public class ActorInfo : MonoBehaviour {
	public enum ActorType
	{
		None,
		Player,
		Monster,
		BossMonster,
		Npc,
		Casttle,
		Guardian,
		Hunter,
		Catapult,
		Prisoner,
		Escort,
	}
	public ActorType actorType = ActorType.None;
	
	
	public enum TeamNo
	{
		None,
		Team_One,
		Team_Two,
	}
	public TeamNo myTeam = TeamNo.None;
	public TeamNo enemyTeam = TeamNo.None;
	
	public enum MonsterType
	{
		None,
		Soldier,
		Defender,
		Berserk,
		Lancer,
		Archer,
		Wizard,
		Priest,
		Assassin,
		Paladin,
		Butcher,
		Devil,
	}
	public MonsterType monsterType = MonsterType.None;
	
	public float colliderRadius = 0.5f;
	void Start()
	{
		BaseMoveController moveCotrol = gameObject.GetComponent<BaseMoveController>();
		
		if (moveCotrol != null)
		{
			colliderRadius = moveCotrol.colliderRadius + moveCotrol.skinWidth;
		}
		
	}
	
	public Collider GetGroundCollider()
	{
		BaseMoveController moveCotrol = gameObject.GetComponent<BaseMoveController>();
		if (moveCotrol != null)
			return moveCotrol.groundCollider;		
		else
			return null;
	}
	
	public BaseMoveController GetMoveController()
	{
		BaseMoveController moveCotroller = gameObject.GetComponent<BaseMoveController>();
		
		return moveCotroller;
	}
	
	public LifeManager GetLifeManager()
	{
		LifeManager lifeManager = gameObject.GetComponent<LifeManager>();
		return lifeManager;
	}
}
