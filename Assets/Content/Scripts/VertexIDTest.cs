using UnityEngine;
using System.Collections;

public class VertexIDTest : MonoBehaviour
{
	public KeyCode key;
	public SkinnedMeshRenderer meshRenderer;
	private int id = 0;

	public void Update ()
	{
		if ( Input.GetKeyDown( key ) )
		{
			MoveTransform();
		}
	}

	public void MoveTransform()
	{
		this.transform.position = meshRenderer.transform.TransformPoint( meshRenderer.sharedMesh.vertices[id] );
		id++;
	}

	public void Reset()
	{
		id = 0;
	}
}
