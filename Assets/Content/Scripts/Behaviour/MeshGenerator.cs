using UnityEngine;
using System.Collections;

[RequireComponent ( typeof ( SkinnedMeshRenderer ) )]
public abstract class MeshGenerator : MonoBehaviour
{
	public int LOD = 3;
	public Mesh initMesh;
	protected int[] hull;
	private SkinnedMeshRenderer rendererP;
	private SkinnedMeshRenderer Renderer
	{
		get
		{
			if ( rendererP != null )
			{
				return rendererP;
			}
			return rendererP = this.GetComponent<SkinnedMeshRenderer>();
		}
	}


	public void Awake()
	{
		CreateMesh();
	}

	public void CreateMesh()
	{
		CreateMesh( Renderer );
	}

	public abstract void CreateMesh(SkinnedMeshRenderer rend);

	public void Clear()
	{
		Renderer.sharedMesh = initMesh;
	}
}
