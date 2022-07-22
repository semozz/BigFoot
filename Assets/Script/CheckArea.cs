using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckArea : MonoBehaviour {
	protected List<LifeManager> hitObjects = new List<LifeManager>();
	public List<LifeManager> HitObjects
	{
		get { return hitObjects; }	
	}
	
	public Collider checkArea = null;
	
	
	
	// Use this for initialization
	void Start () {
		//SetupCollider(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void AddHitObject(LifeManager obj)
	{
		if (obj == null)
			return;
		
		if (hitObjects.Contains(obj) == false)
			hitObjects.Add(obj);
	}
	
	public void SetupCollider(bool bActive)
	{
        if (checkArea == null) return;

        checkArea.isTrigger = bActive;
        if (checkArea.GetType() == typeof(MeshCollider))
            ((MeshCollider) checkArea).convex = bActive;

        checkArea.gameObject.SetActive(bActive);
		
		if (bActive == false)
			hitObjects.Clear();
	}
	
	void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.layer != LayerMask.NameToLayer("MonsterBody"))
			return;
		
		LifeManager obj = other.gameObject.GetComponent<LifeManager>();
		AddHitObject(obj);
	}
	
	void OnTriggerExit (Collider other)
	{
		
	}
	
}
