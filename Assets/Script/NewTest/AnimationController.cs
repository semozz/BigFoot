using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour {
	
	public string animBlendInfoPath = "";
	public AnimationBlendInfo animationBlendInfo = null;
	
	public string currentAnimationName = "";
	
	public float animationEndTime = 0.0f;
	public bool isAnimationPlaying = false;
	
	public Animation anim = null;
	
	private BaseMoveController moveController = null;
	
	void Awake()
	{
		moveController = GetComponent<BaseMoveController>();
	}
	
	// Use this for initialization
	void Start () {
		if (animBlendInfoPath != "" && animBlendInfoPath.Length > 1)
		{
			LoadBlendInfo(animBlendInfoPath);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void LoadBlendInfo(string resourceName)
	{
		AnimationBlendInfoManager infoManager = AnimationBlendInfoManager.instance;
		if (infoManager != null)
		{
			animationBlendInfo = infoManager.GetAnimationBlendInfo(resourceName);
		}
		else
		{
			if (animationBlendInfo == null)
				animationBlendInfo = (AnimationBlendInfo)ResourceManager.LoadObjectFromAssetBundle(resourceName);
			
			if (animationBlendInfo != null)
				animationBlendInfo.rebuildKeys();				
		}
	}
	
	public void ChnageAnimationSpeed(float speedRate)
	{
		if (currentAnimationName == "")
			return;
		
		AnimationState animState = anim[currentAnimationName];
		if ( animState != null && animState.clip != null)
		{
			animState.speed = speedRate;
		}
	}
	
	public void ChangeAnimation(string newAnimationName)
	{
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
		
		if (currentAnimationName == newAnimationName && anim[newAnimationName].wrapMode == WrapMode.Loop)
			return;
		
		//블렌딩 정보가 있고, 블렌드 타임이 설정 되어 있으면 CrossFade로 실행 하고, 그렇지 않으면 Stop/Play호출
		if (blendInfo != null && blendInfo.fBlendTime != 0.0f)
		{
			//Debug.Log("ChangeAnimation - CrossFade : " + newAnimationName + " fadeTime : " + blendInfo.fBlendTime);
			if (anim.IsPlaying(newAnimationName) && anim[newAnimationName].wrapMode == WrapMode.ClampForever)
				anim.Stop(newAnimationName);
			
			anim.CrossFade(newAnimationName, blendInfo.fBlendTime);
		}
		else
		{
			//Debug.Log("ChangeAnimation - Play : " + newAnimationName);
			anim.Stop(newAnimationName);
			anim.Play(newAnimationName);
		}
		
		
		//Debug.Log("ChangeAnimation : " + this.ToString() + " anmation : " + newAnimationName);
		//animation[newAnimationName].speed = MoveSpeedRate;
		
		isAnimationPlaying = true;
		currentAnimationName = newAnimationName;
		
		
		float speedRate = 1.0f;
		if (moveController != null)
			speedRate = moveController.SpeedRate;
		
		AnimationState animState = anim[currentAnimationName];
		if ( animState != null && animState.clip != null)
		{
			if (animState.clip.wrapMode != WrapMode.Loop)
				animationEndTime = animState.clip.length;// * 2.0f;
			else
				animationEndTime = -1;
			
			animState.speed = speedRate;
		}
	}
}
