using UnityEngine;
using System.Collections;

public class BaseDamageUI : MonoBehaviour {
	public float TotalTime = 0.5f;
	public Vector3 speed = new Vector3(0.0f, 1.0f, 0.0f);
	
	private Vector3 sumMoveDelta = Vector3.zero;
	private Transform traceObject = null;
	public Transform TraceObject
	{
		get { return traceObject; }
		set { traceObject = value; }
	}
	
	// Update is called once per frame
	void Update () {
		Move();
	}
	
	public virtual void Move()
	{
		sumMoveDelta += this.speed * Time.deltaTime;
		
		this.gameObject.transform.localPosition = sumMoveDelta;
	}
}
