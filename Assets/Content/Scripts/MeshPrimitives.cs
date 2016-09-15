using UnityEngine;
using System.Collections;

public static class MeshPrimitives
{
	// singelton check
	private static Mesh quad;
	public static Mesh Quad
	{
		get
		{
			if ( quad != null )
			{
				return quad;
			}
			else
			{
				return quad = CreateQuad();
			}
		}
	}

	// creates a primitive quad
	private static Mesh CreateQuad()
	{
		return new Mesh
		{
			name = "MeshPrimitives Quad",
			vertices = new Vector3[]
			{
				new Vector3( -0.5f, -0.5f, 0 ),
				new Vector3( -0.5f, 0.5f, 0 ),
				new Vector3( 0.5f, 0.5f, 0 ),
				new Vector3( 0.5f, -0.5f, 0 )
			},
			triangles = new int[] { 0, 1, 2, 0, 2, 3 },
			uv = new Vector2[]
			{
				new Vector2( 0, 0 ),
				new Vector2( 0, 1f ),
				new Vector2( 1f, 1f ),
				new Vector2( 1f, 0 )
			}
		};
	}
}
