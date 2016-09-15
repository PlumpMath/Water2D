using UnityEngine;
using System.Collections;

[RequireComponent ( typeof ( SkinnedMeshRenderer ) )]
public abstract class MeshGenerator : MonoBehaviour
{
	[Tooltip("Level of Detail. Sets how dense the mesh will be.")]
	public int LOD = 3;
	[HideInInspector] public int[] hull;
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
		Renderer.sharedMesh = MeshPrimitives.Quad;
		var children = this.GetComponentsInChildren<Transform>();
		for ( int i = 1; i < children.Length; i++ )
		{
			DestroyImmediate( children[i].gameObject );
		}
	}
}
