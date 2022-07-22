using UnityEngine;
using System.Collections;

public class CatapultBall : FireBall {
	public float LastMoveDeltaTime = 0.05f;
    
    public float UpSpeed = 4.0f;
    public Vector3 TargetDir = Vector3.zero;
	
	private float mStartTime = 0.0f;
    private float mStartHeight = 0.0f;
    // private bool mFired = false;
	
	public override void Move(float deltaTime, float currentTime)
    {
        Vector3 vMove = MoveDir * MoveSpeed * deltaTime;
        Vector3 vJump = Vector3.zero;

		float fElapsedTime = currentTime - mStartTime;
		float fHeight = (UpSpeed * fElapsedTime) + ((Physics.gravity.y * fElapsedTime * fElapsedTime) * 0.5f);
		vJump = Vector3.up * ((mStartHeight + fHeight) - transform.position.y);

        vMove = vMove + vJump;
		
        transform.position = transform.position + vMove;
		
		//transform.rotation = Quaternion.LookRotation(-vMove);
    }
	
	public override void OnTriggerEnter(Collider other)
	{
		base.OnTriggerEnter(other);
	}
	
	public override void SetFired()
    {
		base.SetFired();
		
		// mFired = true;

        mStartTime = Time.time - 0.01f; ;
        mStartHeight = transform.position.y;

        float fDistance = Mathf.Abs (TargetDir.x);
        float fHeight = TargetDir.y;

        float fElapsedTime = 0.0f;
        if (MoveSpeed > 0.0f) fElapsedTime = fDistance / MoveSpeed;

		UpSpeed = (((Mathf.Abs(Physics.gravity.y) * fElapsedTime * fElapsedTime) / 2.0f) + fHeight) / fElapsedTime;
		
		colliderManager.SetupCollider(detectCollider, true);
	}
	
	public override Vector3 GetMoveDir()
	{
		return this.MoveDir;
	}
}
