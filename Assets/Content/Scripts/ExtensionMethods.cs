using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;

public static class ExtensionMethods
{
	public static Vector3 GetMin( this Vector3[] vec, Transform trans )
	{
		var targetVec = new Vector3[vec.Length];
		for ( int i = 0; i < vec.Length; i++ )
		{
			targetVec[i] = trans.TransformPoint( vec[i]);
		}
		return GetMin( targetVec );
	}

	public static Vector3 GetMin( this Vector3[] vec )
	{
		var targetVec = vec[0];

		foreach ( var v in vec )
		{
			if ( targetVec.x > v.x )
			{
				targetVec.x = v.x;
			}

			if (targetVec.y > v.y)
			{
				targetVec.y = v.y;
			}

			if (targetVec.z > v.z)
			{
				targetVec.z = v.z;
			}
		}

		return targetVec;
	}

	public static Vector3 GetMax ( this Vector3[] vec, Transform trans )
	{
		var targetVec = new Vector3[vec.Length];
		for (int i = 0; i < vec.Length; i++)
		{
			targetVec[i] = trans.TransformPoint ( vec[i] );
		}
		return GetMax ( targetVec );
	}

	public static Vector3 GetMax ( this Vector3[] vec )
	{
		var targetVec = vec[0];

		foreach (var v in vec)
		{
			if (targetVec.x < v.x)
			{
				targetVec.x = v.x;
			}

			if (targetVec.y < v.y)
			{
				targetVec.y = v.y;
			}

			if (targetVec.z < v.z)
			{
				targetVec.z = v.z;
			}
		}

		return targetVec;
	}

	public static Mesh Copy( this Mesh mesh )
	{
		if ( mesh != null )
		{
			var targetMesh = new Mesh
			{
				vertices = mesh.vertices,
				uv = mesh.uv,
				triangles = mesh.triangles
			};
			targetMesh.RecalculateNormals();
			return targetMesh;
		}
		else
		{
			Debug.LogError( "Mesh to copy equals null" );
			return mesh;
		}
	}
}
