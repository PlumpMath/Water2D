using System;
using UnityEngine;
using System.Collections;

[RequireComponent ( typeof ( SkinnedMeshRenderer ) )]
public abstract class MeshGenerator : MonoBehaviour
{
	[Tooltip("Level of Detail. Sets how dense the mesh will be.")]
	public int LOD = 3;
	[HideInInspector] public int[] hull;

	public void CreateMesh()
	{
		CreateMesh( this.GetComponent<SkinnedMeshRenderer>() );
	}

	public abstract void CreateMesh(SkinnedMeshRenderer rend);

	public void Clear()
	{
		this.GetComponent<SkinnedMeshRenderer>().sharedMesh = Primitives.Quad;
		var children = this.GetComponentsInChildren<Transform>();
		for ( int i = 1; i < children.Length; i++ )
		{
			if ( children[i] != null )
			{
				DestroyImmediate( children[i].gameObject );
			}
		}
		GC.Collect();
	}
}
