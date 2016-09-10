using UnityEngine;
using System.Collections;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(VertexIDTest))]
public class VertexIDEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		VertexIDTest vertexID = target as VertexIDTest;

		if (vertexID == null) return;

		if ( GUILayout.Button( "Move" ) )
		{
			vertexID.MoveTransform();
		}

		if ( GUILayout.Button( "Reset" ) )
		{
			vertexID.Reset();
		} 
	}
}
