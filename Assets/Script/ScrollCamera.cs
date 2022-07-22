using UnityEngine;
using System.Collections;

public class ScrollCamera : MonoBehaviour 
{
    public StageManager Stage = null;
    public BaseMoveController player = null;

	public float xoffset = 0.0f;
	public float yoffset = 0.0f;
	public float movespeed = 0.1f;
    public bool FollowYAxis = true;
	
	private Vector3 mTargetPos = Vector3.zero;

    //private float mOffsetX = 0.0f;
    //private float mOffsetY = 0.0f;
	
	private float mMinScrollX = 0.0f;
	private float mMaxScrollX = 0.0f;
    private float mMinScrollY = 0.0f;
    private float mMaxScrollY = 0.0f;
	
	protected AnimationBlendInfo animationBlendInfo = null;
	protected string currentAnimationName = "";
	
	public float smoothTime = 0.5f;
	
	public Transform thisTransform = null;
	
	
	public Transform uiRoot = null;
	
    public Vector3 GetTargetPos()
    {
        return mTargetPos;
    }
	
	//public DropItemManager dropItemManager = null;
	public GameCamera gameCamera = null;
	
    void Awake()
    {	
		thisTransform = this.transform;
    }
	
	void Start () 
	{
        // camera.orthographicSize = Screen.height * 0.5f;
		// Screen.SetResolution (960, 640, false);
		
		PlayerController playerActor = Game.Instance.player;
		if (playerActor != null)
			player = playerActor.moveController;
		
        InitCamera();
		
		//LoadBlendInfo("Prefabs/MainCamera/AnimationBlendInfo");
		//ChangeAnimation("Camera_shake_De");
	}

    public void InitCamera(StageManager stage, BaseMoveController player)
    {
        Stage = stage;
        this.player = player;

        InitCamera();
    }

    public void InitCamera()
    {
        //mOffsetX = xoffset * CommonDef.ToPixelRate;
        //mOffsetY = yoffset * CommonDef.ToPixelRate;
		if (Stage == null)
			Stage = GameObject.FindObjectOfType(typeof(StageManager)) as StageManager;
		
        if (Stage)
        {
			float screenHeight = Screen.height;// * scaleheight;
			float screenWidth = Screen.width;// * scaleheight;
			
			Camera cam = null;
			if (gameCamera != null)
				cam = gameCamera.camera;
			
			float camWidth = screenWidth;
			float camHeight = screenHeight;
			if (cam != null)
			{
				Rect rect = cam.rect;
				camWidth = rect.width * screenWidth;
	        	camHeight = rect.height * screenHeight;
			}
			
			/*
			float fHeightRate = Stage.ScreenHeight / Screen.height;// * scaleheight;
			
            mMinScrollX = Stage.transform.position.x + (screenWidth * 0.5f * fHeightRate);
            mMaxScrollX = Stage.transform.position.x + Stage.NearStageWidth - (screenWidth * 0.5f * fHeightRate);
            mMinScrollY = Stage.transform.position.y + (screenHeight * 0.5f * fHeightRate);
            // mMaxScrollY = Stage.transform.position.y + (20.0f * CommonDef.ToPixelRate) - (Stage.ScreenHeight * 0.5f);
            mMaxScrollY = Stage.transform.position.y + Stage.NearStageHeight - (screenHeight * 0.5f * fHeightRate);
            */
			
			float fHeightRate = Stage.ScreenHeight / camHeight;
			
            mMinScrollX = Stage.transform.position.x + (camWidth * 0.5f * fHeightRate);
            mMaxScrollX = Stage.transform.position.x + Stage.NearStageWidth - (camWidth * 0.5f * fHeightRate);
            mMinScrollY = Stage.transform.position.y + (camHeight * 0.5f * fHeightRate);
            // mMaxScrollY = Stage.transform.position.y + (20.0f * CommonDef.ToPixelRate) - (Stage.ScreenHeight * 0.5f);
            mMaxScrollY = Stage.transform.position.y + Stage.NearStageHeight - (camHeight * 0.5f * fHeightRate);
        }

        if (player)
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
    }
	
	void FixedUpdate ()
	{
        UpdateTargetPosition();
	
		/*
		if (Game.instance.IsPause == true)
			return;
		
		int nCount = Game.instance.timeScaleList.Count;
		if (nCount > 0)
		{
			float timeScale = 1.0f;
			float durationTime = 0.0f;
		
			Game.TimeScaleInfo info = Game.instance.timeScaleList[0];
		
			durationTime = info.durationTime;
			durationTime -= Time.deltaTime;
			
			if (durationTime <= 0.0f)
				Game.instance.timeScaleList.RemoveAt(0);	
			else
			{
				info.durationTime = durationTime;
				timeScale = info.timeScale;
				Game.instance.timeScaleList[0] = info;
			}
			
			Time.timeScale = timeScale;
		}
		*/
	}

    void UpdateTargetPosition()
    {
        if (player == null) return;
		
		Vector3 currentPos = thisTransform.position;
		mTargetPos = currentPos;
		
        if (player.moveDir == Vector3.right)
            mTargetPos.x = player.transform.position.x + xoffset;
        else
            mTargetPos.x = player.transform.position.x - xoffset;

        if (FollowYAxis == true)
            mTargetPos.y = player.transform.position.y + yoffset;

        if (mTargetPos.x < mMinScrollX) 
			mTargetPos.x = mMinScrollX;
        if (mTargetPos.x > mMaxScrollX) 
			mTargetPos.x = mMaxScrollX;
        if (mTargetPos.y < mMinScrollY) 
			mTargetPos.y = mMinScrollY;
        if (mTargetPos.y > mMaxScrollY) 
			mTargetPos.y = mMaxScrollY;

    	float diff = mTargetPos.x - transform.position.x;
		if (Mathf.Abs(diff) > 0.01f)
			mTargetPos.x = Mathf.SmoothDamp(transform.position.x, mTargetPos.x, ref movespeed, smoothTime);
		
		if (mTargetPos != currentPos)
			thisTransform.position = mTargetPos;
    }
	
	public Vector2 GetMoveRate()
	{
		Vector2 moveRate = Vector2.zero;
		if (mMaxScrollX - mMinScrollX != 0.0f)
		{
			moveRate.x = (mTargetPos.x - mMinScrollX) / (mMaxScrollX - mMinScrollX);
			moveRate.y = (mTargetPos.y - mMinScrollY) / (mMaxScrollY - mMinScrollY);
		}
		return moveRate;
	}
	
	protected void LoadBlendInfo(string resourceName)
	{
		AnimationBlendInfoManager infoManager = AnimationBlendInfoManager.instance;
		if (infoManager != null)
		{
			animationBlendInfo = infoManager.GetAnimationBlendInfo(resourceName);
		}
		else
		{
			if (animationBlendInfo == null)
				//animationBlendInfo = Resources.Load(resourceName, typeof(AnimationBlendInfo)) as AnimationBlendInfo;
				animationBlendInfo = (AnimationBlendInfo)ResourceManager.LoadObjectFromAssetBundle(resourceName);
			
			if (animationBlendInfo != null)
				animationBlendInfo.rebuildKeys();				
		}
	}
	
	public void ChangeAnimation(string newAnimationName)
	{
		if (animation == null || animation[newAnimationName] == null)
			return;
		
		BlendInfo blendInfo = null;
		
		if (animationBlendInfo != null)
		{	
			int nRowIndex = animationBlendInfo.GetAnimationIndex(currentAnimationName);	//시작 애니메이션
			
			int nColIndex = animationBlendInfo.GetAnimationIndex(newAnimationName);		//목표 애니메이션		
			
			if (nColIndex > -1 && 
				nRowIndex > -1)
			{
				blendInfo = animationBlendInfo.GetBlendInfo(nRowIndex, nColIndex);
			}
		}
		
		if (currentAnimationName == newAnimationName && animation[newAnimationName].wrapMode == WrapMode.Loop)
			return;
		
		//블렌딩 정보가 있고, 블렌드 타임이 설정 되어 있으면 CrossFade로 실행 하고, 그렇지 않으면 Stop/Play호출
		if (blendInfo != null && blendInfo.fBlendTime != 0.0f)
		{
			//Debug.Log("ChangeAnimation - CrossFade : " + newAnimationName + " fadeTime : " + blendInfo.fBlendTime);
			if (animation.IsPlaying(newAnimationName) && animation[newAnimationName].wrapMode == WrapMode.ClampForever)
				animation.Stop(newAnimationName);
			
			animation.CrossFade(newAnimationName, blendInfo.fBlendTime);
		}
		else
		{
			//Debug.Log("ChangeAnimation - Play : " + newAnimationName);
			animation.Stop(newAnimationName);
			animation.Play(newAnimationName);
		}
		
		currentAnimationName = newAnimationName;
		
		//Debug.Log("ScrollCamera Animation : " + currentAnimationName);
	}
	
	public void InitCameraSize()
	{
		Debug.Log("ScrollCamera InitCameraSize : " + currentAnimationName);
		
		if (gameCamera != null)
			gameCamera.InitCameraSize();
	}
	
#if UNITY_EDITOR
	void OnDrawGizmos ()
	{
		Gizmos.color = new Color(1f, 0.4f, 0f);
		
		float minX = Stage.transform.position.x;
		float maxX = minX + Stage.NearStageWidth;
		float minY = Stage.transform.position.y;
		float maxY = minY + Stage.NearStageHeight;
		
		Vector3 upLeft = new Vector3(minX, maxY, 0.0f);
		Vector3 upRight = new Vector3(maxX, maxY, 0.0f);
		Vector3 downLeft = new Vector3(minX, minY, 0.0f);
		Vector3 downRight = new Vector3(maxX, minY, 0.0f);
		
		Color origColor = Gizmos.color;
		Gizmos.color = Color.red;
		
		Gizmos.DrawLine(upLeft, upRight);
		Gizmos.DrawLine(upRight, downRight);
		Gizmos.DrawLine(downRight, downLeft);
		Gizmos.DrawLine(downLeft, upLeft);
		
		Gizmos.color = origColor;
	}
#endif
}
