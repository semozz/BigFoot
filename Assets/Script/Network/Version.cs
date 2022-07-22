using UnityEngine;
using System.Collections;
using System.Text;
/*
 * version
 * 0.8 : 2014.9월 ClosedBeta버전.
 * 0.7 : 사운드 + 퀄리티업~.
 * 0.6 : 보스레이드.
 * 0.5 : 업적?
 * 0.4 : 투기장.
 * 0.3.23:특성도적완료
 */

public class Version : MonoBehaviour
{
	static public int Major = 0;
	static public int Minor = 8;
	static public int assetServerRevision = 5983;	// 
	
	static public int NetVersion = 3;	// 클라이언트 앱 버전.
	
	public UILabel txtVersion;
	
	void Awake()
	{
		
		txtVersion = gameObject.GetComponentInChildren<UILabel>();
		StringBuilder sb = new StringBuilder();
		sb.Append("version.");
		sb.Append(Major);
		sb.Append(".");
		sb.Append(NetVersion);
		sb.Append(".");
		sb.Append(assetServerRevision);
		
		txtVersion.text = sb.ToString();
	}
	
	void Start()
	{
		
	}
}