using UnityEngine;
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
			controller.CreateJoints();
		}

		if ( GUILayout.Button( "Clear" ) )
		{
			controller.Clear();
		}
	}
}
