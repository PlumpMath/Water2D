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

		var waterCont = target as WaterPhysicsController;
		if ( waterCont == null )
		{
			Debug.LogError( "No WaterPhisicsController found!" );
			return;
		}

		if ( GUILayout.Button( "Create Joints" ) )
		{
			waterCont.Clear();
			waterCont.CreateJoints( waterCont.GetComponent<SkinnedMeshRenderer>() );
		}

		if ( GUILayout.Button( "Clear" ) )
		{
			waterCont.Clear();
		}
	}
}
