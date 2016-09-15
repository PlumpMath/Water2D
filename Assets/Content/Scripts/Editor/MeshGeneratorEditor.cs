using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MeshGenerator), true)]
public class MeshGeneratorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		var generator = target as MeshGenerator;

		if (generator == null) return;
		if ( GUILayout.Button( "Create Mesh" ) )
		{
			generator.Clear();
			generator.CreateMesh();
		}

		if ( GUILayout.Button( "Clear" ) )
		{
			generator.Clear();
		}
	}
}
