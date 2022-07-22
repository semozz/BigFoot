using UnityEngine;
using System.Collections;

public class EmptyLoadingPage : BaseAssetLoader {
	public string versionTablename = "AssetBundleInfo.xml";
	public string startSceneName = "ResourceLoading";
	
	public GUIText label = null;
	public GUIText progress = null;
    public bool Hive5;
	public NetConfig.HostType testHostType = NetConfig.HostType.DevHost;
	
	void Awake() {
		Game.isHive5 = Hive5;
	}
	
	// Use this for initialization
	void Start () {
		Game game = Game.Instance;
		
		if (game != null)
		{
			game.TestHostType = testHostType;
			
			game.PreInitData();
			game.loadingPage = this;
			
			game.RequestSettingInfos();
			
			//game.CreateNetwork(Hive5);
		}
	}
	
	void OnDestroy()
	{
		if (Game.Instance != null)
			Game.Instance.loadingPage = null;
	}
	
	public void LoadBundleVersion()
	{
		string assetBundleURL = NetConfig.GetRootURL();
		string versionUrl = string.Format("{0}/{1}", assetBundleURL, versionTablename);
		StartCoroutine(LoadAssetBundleVersion(versionUrl));		
	}
	
	public override void SetMessage (string messageStr)
	{
		if (label != null)
			label.text = messageStr;
	}
	
	public override void SetProgress (float progressValue, bool isDownLoading)
	{
		string messageStr = string.Format("{0:##0.##}", progressValue * 100.0f);
		
		if (progress != null)
			progress.text = messageStr;
	}
	
	IEnumerator LoadAssetBundleVersion(string url)
	{
		SetMessage("LoadingAssetBundle Version");
		if (loadWWW!= null)
			CloseWWW();
		
		loadWWW = new WWW(url);
		yield return loadWWW;
		
		string errorMsg = "";
		if (loadWWW != null)
			errorMsg = loadWWW.error;
		
		if (!string.IsNullOrEmpty(errorMsg) && errorMsg.StartsWith("Cannot load cached AssetBundle") == false)
		{
			Debug.Log(errorMsg);
			OnErrorMessageBox();
		}
		else
		{
			string xmlText = System.Text.UTF8Encoding.UTF8.GetString(System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.UTF8, loadWWW.bytes));   
 			
			AssetBundleVersionManager assetBundleMgr = AssetBundleVersionManager.Instance;
			AssetBundleVersion versionInfo = null;//new AssetBundleVersion();
			
			System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(AssetBundleVersion));
			
			System.IO.TextReader textReader = new System.IO.StringReader(xmlText);
			versionInfo = (AssetBundleVersion)serializer.Deserialize(textReader);
			
			assetBundleMgr.SetVersionInfo(versionInfo);
			
			textReader.Close();
			
			AssetBundleInfo startPage = versionInfo != null ? versionInfo.startPageBundle : null;
			if (startPage != null)
			{
				string bundleURL = "";
				
				SetMessage(startPage.assetBundleName);
				bundleURL = NetConfig.MakeAssetBundleURL(startPage.assetBundleName);
				yield return StartCoroutine(LoadResourceLoaderStage(bundleURL, startPage.version, 
																	startPage.assetBundleName, startSceneName));
			}
		}
		
		CloseWWW();
	}
	
	// To open the dialogue from outside of the script.
    public void OnErrorMessageBox()
    {
		if (!Game.Instance.androidManager)
			return;
		
		AndroidExitWindow info = new AndroidExitWindow();
		info.windowType = 1;
		info.Title = "접속오류";
		info.Message = "서버에연결할수 없습니다. 인터넷 연결 상태를 확인하고 다시 시도해주세요.";
		
		Game.Instance.androidManager.CallUnityExitWindow(AlertDialogType.NetworkError);
    }
}
