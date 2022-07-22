using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]

[System.Serializable]
public class BasicSprite : MonoBehaviour 
{
    public Mesh sharedMesh { get { if (mMeshFilter) return mMeshFilter.sharedMesh; return null; } }

	public Texture SpriteTexture = null;
	public int Width = 0;
	public int Height = 0;
	public Vector3 Direction = Vector3.up;

    private GameObject mInstance = null;
	private Renderer mRenderer = null;
	private MeshFilter mMeshFilter = null;

    private Vector3[] mVertices = new Vector3[4];
    private Vector2[] mUV = new Vector2[4];
    private int[] mIndices = new int[6];
	
	static Vector2 TexToUV (float offsetx, float offsety, float texwidth, float texheight)
    {
        if (texwidth <= 0.0f || texheight <= 0.0f)
            return new Vector2 (0.0f, 0.0f);

        return new Vector2 (offsetx / texwidth, offsety / texheight);
    }
	
	void Awake ()
	{
#if UNITY_EDITOR
		InitData ();
#endif
	}
	
	void Start ()
	{
#if UNITY_EDITOR
		UpdateData ();
#endif
	}
	
	void Update ()
	{
	}
	
	void OnDrawGizmos ()
	{
#if UNITY_EDITOR
		if (mInstance != null || Application.isPlaying == true)
            return;

		Awake ();
		Start ();
		
		Update ();
#endif
	}
	
	public void InitData ()
	{
#if UNITY_EDITOR
        mInstance = gameObject;

        mRenderer = GetComponentInChildren<Renderer>();
        if (mRenderer.sharedMaterial)
            DestroyImmediate(mRenderer.sharedMaterial);

        mRenderer.sharedMaterial = new Material(Shader.Find("Unlit/Transparent"));

        mMeshFilter = GetComponentInChildren<MeshFilter>();
        if (mMeshFilter.sharedMesh)
            DestroyImmediate(mMeshFilter.sharedMesh);

        mMeshFilter.sharedMesh = new Mesh ();
#endif
	}
	
	
	public string texturePath = "";
	public static string GetFileName(string path)
	{
		string fileName = "";
		
		string[] splits = path.Split('/');
		int nCount = splits.Length;
		
		if (nCount > 0)
			fileName = splits[nCount - 1];
		
		int pos = fileName.LastIndexOf(".");
		if (pos != -1)
			fileName = fileName.Remove(pos);
		
		return fileName;
	}
	
	public void SetData (Texture tex, int tw, int th, Vector3 offset, Vector3 dir)
	{
		InitData ();
		
		SpriteTexture = tex;
		
		Width = tw;
		Height = th;
		Direction = dir;
		
		transform.Translate (offset);
		
		UpdateData ();
	}
	
	public void UpdateData ()
	{
		if (mRenderer && mRenderer.sharedMaterial)
		{
			mRenderer.sharedMaterial.mainTexture = SpriteTexture;
		}
		
        float tw = (float) Width;
        float th = (float) Height;
		
		if (Direction == Vector3.up || Direction == Vector3.down)
		{
	        mVertices[0] = new Vector3 (0.0f, 0.0f, 0.0f);
	        mVertices[1] = new Vector3 (tw, 0.0f, 0.0f);
	        mVertices[2] = new Vector3 (0.0f, 0.0f, th);
	        mVertices[3] = new Vector3 (tw, 0.0f, th);		
		}
		else if (Direction == Vector3.back || Direction == Vector3.forward)
		{
	        mVertices[0] = new Vector3 (0.0f, 0.0f, 0.0f);
	        mVertices[1] = new Vector3 (tw, 0.0f, 0.0f);
	        mVertices[2] = new Vector3 (0.0f, th, 0.0f);
	        mVertices[3] = new Vector3 (tw, th, 0.0f);
		}
		else if (Direction == Vector3.right || Direction == Vector3.left)
		{
	        mVertices[0] = new Vector3 (0.0f, 0.0f, 0.0f);
	        mVertices[1] = new Vector3 (0.0f, 0.0f, tw);
	        mVertices[2] = new Vector3 (0.0f, th, 0.0f);
	        mVertices[3] = new Vector3 (0.0f, th, tw);
		}
		
        mUV[0] = new Vector2 (0.0f, 0.0f);;//TexToUV (0.0f, 0.0f, tw, th);
        mUV[1] = new Vector2 (1.0f, 0.0f);//TexToUV (tw, 0.0f, tw, th);
        mUV[2] = new Vector2 (0.0f, 1.0f);//TexToUV (0.0f, th, tw, th);
        mUV[3] = new Vector2 (1.0f, 1.0f);//TexToUV (tw, th, tw, th);
		
        mIndices[0] = 0;
        mIndices[1] = 2;
        mIndices[2] = 1;
        mIndices[3] = 1;
        mIndices[4] = 2;
        mIndices[5] = 3;

        if (mMeshFilter && mMeshFilter.sharedMesh)
        {
            mMeshFilter.sharedMesh.Clear();
            mMeshFilter.sharedMesh.subMeshCount = 1;
            mMeshFilter.sharedMesh.vertices = mVertices;
            mMeshFilter.sharedMesh.uv = mUV;
            mMeshFilter.sharedMesh.triangles = mIndices;
            mMeshFilter.sharedMesh.RecalculateBounds();
            mMeshFilter.sharedMesh.Optimize();
        }
	}

    public float GetLengthXAxis()
    {
        Vector3 v1 = transform.TransformPoint(mVertices[0]);
        Vector3 v2 = transform.TransformPoint(mVertices[1]);

        return Mathf.Abs(v2.x - v1.x);
    }

    public float GetLengthYAxis()
    {
        Vector3 v1 = transform.TransformPoint(mVertices[0]);
        Vector3 v2 = transform.TransformPoint(mVertices[1]);

        return Mathf.Abs(v2.y - v1.y);
    }
}
