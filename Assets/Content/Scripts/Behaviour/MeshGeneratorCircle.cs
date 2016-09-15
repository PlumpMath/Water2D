using UnityEngine;

public class MeshGeneratorCircle : MeshGenerator
{
	public override void CreateMesh( SkinnedMeshRenderer rend )
	{
		Clear();

		// create parameter and new mesh
		var min = rend.sharedMesh.vertices.GetMin();
		var max = rend.sharedMesh.vertices.GetMax();
		var radius = Mathf.Abs( max.y - min.y ) / 2;
		var angle = 360f / base.LOD;
		var rotation = Quaternion.AngleAxis( angle, transform.forward * -1 );
		var matrix = Matrix4x4.TRS( Vector3.zero, rotation, Vector3.one );
		hull = new int[base.LOD];

		var mesh = new Mesh { name = "Generated Circle" };

		// set vertices
		var vertices = new Vector3[base.LOD + 1];
		vertices[0] = min + ( max - min ) / 2;
		vertices[1] = vertices[0] + Vector3.up * radius;
		hull[0] = 1;
		for ( var i = 2; i < vertices.Length; i++ )
		{
			vertices[i] = matrix.MultiplyPoint3x4( vertices[i - 1] );
			hull[i - 1] = i;
		}

		// set triangles
		var triangles = new int[base.LOD * 3];
		for ( int k = 0, t = 0; t < triangles.Length - 3; t += 3, k++ )
		{
			triangles[t] = 0;
			triangles[t + 1] = k + 1;
			triangles[t + 2] = k + 2;
			//Debug.Log( "Triangle: " + triangles[t] + " " + triangles[t + 1] + " " + triangles[t + 2] );
		}

		triangles[triangles.Length - 3] = 0;
		triangles[triangles.Length - 2] = vertices.Length - 1;
		triangles[triangles.Length - 1] = 1;

		// set uvs
		var uvs = new Vector2[vertices.Length];
		for (var k = 0; k < uvs.Length; k++)
		{
			var dX = vertices[k].x - min.x;
			var dY = vertices[k].y - min.y;
			uvs[k] = new Vector2 ( dX / (radius * 2), dY / (radius * 2) );
			//Debug.Log( "Vert: " + vertices[k] + " UV: " + uvs[k] );
		}

		// apply to mesh
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;
		mesh.RecalculateNormals();
		rend.sharedMesh = mesh;
	}
}
