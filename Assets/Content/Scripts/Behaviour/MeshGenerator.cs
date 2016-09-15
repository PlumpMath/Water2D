using System;
using UnityEngine;
using System.Collections;

[RequireComponent ( typeof ( SkinnedMeshRenderer ) )]
public abstract class MeshGenerator : MonoBehaviour
{
	[Tooltip("Level of Detail. Sets how dense the mesh will be.")]
	public int LOD = 3;
	[HideInInspector] public int[] hull;
	public ComponentManager manager = new ComponentManager();

	public void Awake()
	{
		CreateMesh();
	}

	public void CreateMesh()
	{
		CreateMesh( manager.GetSkinnedMeshRenderer( this.transform ) );
	}
	
	public abstract void CreateMesh(SkinnedMeshRenderer rend);

	public void Clear()
	{
		manager.GetSkinnedMeshRenderer( this.transform ).sharedMesh = Primitives.Quad;
		var children = this.GetComponentsInChildren<Transform>();
		for ( int i = 1; i < children.Length; i++ )
		{
			DestroyImmediate( children[i].gameObject );
		}
		GC.Collect();
	}
}
