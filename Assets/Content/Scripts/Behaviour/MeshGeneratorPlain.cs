using UnityEngine;
using System.Collections;

public class MeshGeneratorPlain : MeshGenerator
{
	public float width = 1f;
	public float height = 1f;

	public override void CreateMesh( SkinnedMeshRenderer rend )
	{
		// create parameter and new meshs
		var deltaDistance = width / (base.LOD + 1);
		var mesh = new Mesh { name = "Generated Plain" };

		// calculate vertices
		var vertices = new Vector3[base.LOD * 2 + 4];
		int halfLength = vertices.Length / 2;
		base.hull = new int[halfLength];

		for ( var i = 0; i <= halfLength; i++ )
		{
			vertices[i] = new Vector3 ( -width/2 + deltaDistance * i, -height/2, 0 );
		}
		
		for ( var j = 0; j < halfLength; j++ )
		{
			vertices[j + halfLength] = new Vector3 ( -width/2 + deltaDistance * j, height/2, 0 );
			base.hull[j] = j + halfLength;
		}

		// calculate triangles
		var triangles = new int[(vertices.Length - 2) * 3];
		for (int t = 0, i = 0; i < triangles.Length; i += 6, t++)
		{
			triangles[i] = t;
			triangles[i + 1] = triangles[i + 4] = t + halfLength;
			triangles[i + 2] = triangles[i + 3] = t + 1;
			triangles[i + 5] = t + halfLength + 1;
		}

		// calculate uv
		var uvs = new Vector2[ vertices.Length ];
		for ( var k = 0; k < uvs.Length; k++ )
		{
			var dX = vertices[k].x - vertices[0].x;
			var dY = vertices[k].y - vertices[0].y;
			uvs[k] = new Vector2( dX / width,  dY / height );
			//Debug.Log(  "Vertex:" + vertices[k] + " uv:  " + uvs[k].x + ", " + uvs[k].y );
		}

		// apply to mesh and renderer
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;
		mesh.RecalculateNormals();
		rend.sharedMesh = mesh;
	}
}
