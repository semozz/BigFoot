using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationBlendInfoManager
{
	private static AnimationBlendInfoManager mInstance = null;
	
	public static AnimationBlendInfoManager instance
	{
		get
		{
			if (mInstance == null)
				mInstance = new AnimationBlendInfoManager();
			
			return mInstance;
		}
	}
	
	private Dictionary<string, AnimationBlendInfo> animationBlendInfoLst = new Dictionary<string, AnimationBlendInfo>();
	
	public AnimationBlendInfo GetAnimationBlendInfo(string resourceName)
	{
		AnimationBlendInfo info = null;
		if (animationBlendInfoLst.ContainsKey(resourceName)	== false)
		{
			//info = Resources.Load(resourceName, typeof(AnimationBlendInfo)) as AnimationBlendInfo;
			info = (AnimationBlendInfo)ResourceManager.LoadObjectFromAssetBundle(resourceName);
			if (info != null)
			{
				info.rebuildKeys();
				animationBlendInfoLst.Add(resourceName, info);
			}
		}
		else
			info = animationBlendInfoLst[resourceName];
		
		return info;
	}
	
	public void InitInfoList()
	{
		animationBlendInfoLst.Clear();	
	}
}
