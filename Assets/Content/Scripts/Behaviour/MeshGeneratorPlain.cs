using UnityEngine;
using System.Collections;

public class MeshGeneratorPlain : MeshGenerator
{
	public override void CreateMesh( SkinnedMeshRenderer rend )
	{
		base.Clear();

		// create parameter and new mesh
		var min = rend.sharedMesh.vertices.GetMin();
		var max = rend.sharedMesh.vertices.GetMax();
		var width = Mathf.Abs( max.x - min.x );
		var height = Mathf.Abs( max.y - min.y );
		var distX = width / (base.LOD + 1);

		var mesh = new Mesh { name = "Generated Plain Mesh" };

		// calculate vertices
		var vertices = new Vector3[base.LOD * 2 + 4];
		int halfLength = vertices.Length / 2;
		for ( var i = 0; i <= halfLength; i++ )
		{
			vertices[i] = new Vector3 ( min.x + distX * i, min.y, min.z );
		}
		
		base.hull = new int[ halfLength ];
		for ( var j = 0; j < halfLength; j++ )
		{
			vertices[j + halfLength] = new Vector3 ( min.x + distX * j, max.y, max.z );
			base.hull[j] = j;
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
			uvs[k] = new Vector2( Mathf.Abs( vertices[k].x) / height, Mathf.Abs(vertices[k].y / width) );
		}

		// apply to mesh
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;
		mesh.RecalculateNormals();
		rend.sharedMesh = mesh;

		//this.transform.localScale = Vector3.one;
	}
}
