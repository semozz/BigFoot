using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColliderManager : MonoBehaviour {
	public GameObject ownerActor = null;
	public Dictionary<string, Collider> colliderList = new Dictionary<string, Collider>();
	
	public int colliderStep = 0;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void Awake()
	{
		Collider[] colliders = GetComponentsInChildren<Collider>(true);
        foreach (Collider c in colliders)
        {
			AddCollider(c);
		}
	}
	
	public void AddCollider(Collider col)
	{
		if (colliderList.ContainsKey(col.name) == true)
			return;
		
		colliderList.Add(col.name, col);
		SetupCollider(col, false);
	}
	
	public void SetupCollider(Collider col, bool activate)
	{
		if (col == null) return;

        col.isTrigger = activate;
        if (col.GetType() == typeof(MeshCollider))
            ((MeshCollider)col).convex = activate;
        col.gameObject.SetActive(activate);
	}
	
	public void SetupCollider(string name, bool activate)
	{
		Collider col = null;
		
		if (colliderList.ContainsKey(name) == true)
			col = colliderList[name];
		
		SetupCollider(col, activate);
	}
	
	public void InitColliderStep()
	{
		colliderStep = 0;
		
		foreach(var info in colliderList)
		{
			SetupCollider(info.Value, false);	
		}
	}
	
	public void IncColliderStep()
	{
		colliderStep++;
	}
	
	public void SetColliderPos(string name, Vector3 vPos)
	{
		Collider col = null;
		
		if (colliderList.ContainsKey(name) == true)
			col = colliderList[name];
		
		if (col != null)
		{
			Vector3 vOrigPos = col.gameObject.transform.position;
			vOrigPos.x = vPos.x;
			col.gameObject.transform.position = vOrigPos;
		}
	}
}
