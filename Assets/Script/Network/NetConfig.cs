
public class NetConfig
{
	public enum HostType
	{
		DevHost = 0,
		TestHost = 1,
		RealHost = 2,
	}
	
	public enum PublisherType
	{
		None = 0,
		Google = 1,
		Apple = 2,
		TStore = 3,
		Kakao = 4,
	}
	
	public const string RealHostURL = "http://1.234.6.31:8000/";
	public const string TestHostURL = "http://1.234.90.224/";
	public const string DevHostURL = "http://192.168.0.2/";
	
	public const string NoticeRealHostURL = "http://1.234.7.208:8000/";
	public const string NoticeTestHostURL = "http://1.234.90.224/";
	public const string NoticeDevHostURL = "http://192.168.0.2/";
	
	
	//public static string assetBundleURLRoot_REAL = "http://rzp72gkgja.c-cdn.tcloudbiz.com/patch";
	//public static string assetBundleURLRoot_TEST = "http://rzp72gkgja.c-cdn.tcloudbiz.com/patch_test";
	//public static string assetBundleURLRoot_DEV = "http://monsterside.ipdisk.co.kr:8000/patch_kakao";
	
	public static string assetBundleURLRoot_REAL = "http://lo1c5pqu04.ecn.cdn.ofs.kr/patch";
	public static string assetBundleURLRoot_TEST = "http://lo1c5pqu04.ecn.cdn.ofs.kr/patch_test";
	//public static string assetBundleURLRoot_TEST = "http://lo1c5pqu04.ecn.cdn.ofs.kr/patch_tw_test";	//해외 테스트용.
	public static string assetBundleURLRoot_DEV = "http://monsterside.ipdisk.co.kr:8000/patch_kakao";
	
	public static NetConfig.HostType hostType = NetConfig.HostType.DevHost;
	public static NetConfig.PublisherType curPublisher = NetConfig.PublisherType.None;
	
	public static string GetRootURL()
	{
		string rootURL = "";
		switch(hostType)
		{
		case NetConfig.HostType.RealHost:
			rootURL = assetBundleURLRoot_REAL;
			break;
		case NetConfig.HostType.TestHost:
			rootURL = assetBundleURLRoot_TEST;
			break;
		default:
			rootURL = assetBundleURLRoot_DEV;
			break;
		}
		
		return rootURL;
	}
	
	public static string MakeAssetBundleURL(string defaultName)
	{
		string url = "";
		string postfixStr = "";
		switch(UnityEngine.Application.platform)
		{
		case UnityEngine.RuntimePlatform.WindowsEditor:
		case UnityEngine.RuntimePlatform.WindowsPlayer:
			postfixStr = "_Win";
			break;
		case UnityEngine.RuntimePlatform.Android:
			postfixStr = "_Android";
			break;
		case UnityEngine.RuntimePlatform.OSXEditor:
		case UnityEngine.RuntimePlatform.OSXPlayer:
			postfixStr = "_OSX";
			break;
		case UnityEngine.RuntimePlatform.IPhonePlayer:
			postfixStr = "_iOS";
			break;
		}
		
		string rootURL = GetRootURL();
		
		url = string.Format("{0}/{1}{2}.unity3d", rootURL, defaultName, postfixStr);
		
		return url;
	}
	
	/*
	//  서버점검공지서버.
	public const string RealServerCheckHostURL = "http://1.234.90.215:8000/";	
	public const string TestServerCheckHostURL = "http://1.234.90.224/";	
	public const string DevServerCheckHostURL = "http://192.168.0.2/";		//
	
	*/
	
}