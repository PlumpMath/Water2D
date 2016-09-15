using UnityEngine;

public class MeshGeneratorCircle : MeshGenerator
{
	public float radius = 0.5f;

	public override void CreateMesh( SkinnedMeshRenderer rend )
	{
		// create parameter and new mesh
		var angle = 360f / base.LOD;
		var rotation = Quaternion.AngleAxis( angle, this.transform.forward * -1 );
		var matrix = Matrix4x4.TRS( Vector3.zero, rotation, Vector3.one );
		hull = new int[base.LOD];

		var mesh = new Mesh { name = "Generated Circle" };

		// calculate vertices
		var vertices = new Vector3[base.LOD + 1];
		vertices[0] = Vector3.zero;
		vertices[1] = Vector3.up * radius;
		hull[0] = 1;
		for ( var i = 2; i < vertices.Length; i++ )
		{
			vertices[i] = matrix.MultiplyPoint3x4( vertices[i - 1] );
			hull[i - 1] = i;
		}

		// calculate triangles
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

		// calculate uvs
		var uvs = new Vector2[vertices.Length];
		for (var k = 0; k < uvs.Length; k++)
		{
			var dX = vertices[k].x + radius;
			var dY = vertices[k].y + radius;
			uvs[k] = new Vector2 ( dX / (radius * 2), dY / (radius * 2) );
			//Debug.Log( "Vert: " + vertices[k] + " UV: " + uvs[k] );
		}

		// apply to mesh and renderer
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;
		mesh.RecalculateNormals();
		rend.sharedMesh = mesh;
	}
}
