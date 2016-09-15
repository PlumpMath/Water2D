using UnityEngine;
using System.Collections;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(WaterPhysicsController))]
public class WaterPhysicsEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		var controller = target as WaterPhysicsController;
		if ( controller == null )
		{
			Debug.LogError( "No WaterPhisicsController found!" );
			return;
		}

		if ( GUILayout.Button( "Create Joints" ) )
		{
			controller.Clear();
			controller.CreateJoints( controller.GetComponent<SkinnedMeshRenderer>() );
		}

		if ( GUILayout.Button( "Clear" ) )
		{
			controller.Clear();
		}
	}
}
