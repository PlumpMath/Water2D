using System;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class WaterPhysicsController : MonoBehaviour
{
	public int jointLOD = 9;
	[Range(0f,1f)] public float density = 0.5f;
	public bool useWeightedVertices = true;
	public Mesh initMesh;
	private Renderer rend;
	private Transform rootJoint;
	private Rigidbody2D previousRigidbody2D = null;

	private void Awake ()
	{
		rend = this.GetComponent<Renderer>();
		if ( rend.GetType() == typeof (SkinnedMeshRenderer) && GameObject.Find( "Joint_0_Root" ) == null )
		{
			CreateJoints( (SkinnedMeshRenderer)rend );
		}
	}

	// Creates and applies skinned joints to the mesh in passed renderer
	public void CreateJoints( SkinnedMeshRenderer skinnedRend )
	{
		// copy mesh
		var mesh = skinnedRend.sharedMesh.Copy();
		mesh.name = "Skinned Plane";

		var min = mesh.vertices.GetMin( this.transform );
		var max = mesh.vertices.GetMax( this.transform );
		var width = max.x - min.x;
		var distX = width / jointLOD;
		var radius = distX / 2;

		// Set joint variables
		var joints = new Transform[jointLOD + 1];
		var weights = new BoneWeight[mesh.vertexCount];
		var bindPoses = new Matrix4x4[joints.Length];

		// Initialize root joint
		rootJoint = new GameObject ( "Joint_0_Root" ).transform;
		rootJoint.parent = this.transform;
		rootJoint.localPosition = Vector3.zero;
		rootJoint.localRotation = Quaternion.identity;
		rootJoint.localScale = Vector3.one;
		skinnedRend.rootBone = rootJoint;
		joints[0] = rootJoint;
		bindPoses[0] = rootJoint.worldToLocalMatrix * this.transform.localToWorldMatrix;

		for ( var j = 1; j <= jointLOD; j++ )
		{
			var joint = new GameObject ( "Joint_" + j);
			
			// set position and parent			
			var pos = Vector3.zero;
			pos.x = min.x + radius + distX * (j - 1);
			pos.y = max.y - radius;

			var jointController = joint.AddComponent<WaterJointController> ();
			jointController.Initialize ( pos, min, radius, density, previousRigidbody2D );

			joint.transform.parent = rootJoint;
			joints[j] = joint.transform;
			previousRigidbody2D = joint.GetComponent<Rigidbody2D>();

			// Set bones weights
			for ( int v = 0; v < mesh.vertexCount; v++ )
			{
				var vertWorldPos = this.transform.TransformPoint( mesh.vertices[v] );
				vertWorldPos.y = joints[j].position.y;

				if ( vertWorldPos.x >= joints[j].position.x - radius &&
				     vertWorldPos.x <= joints[j].position.x + radius )
				{
					var vertDistX = 1f;
					if ( useWeightedVertices )
					{
						vertDistX = Mathf.Abs( Mathf.Abs( joints[j].position.x ) - Mathf.Abs( vertWorldPos.x ) );
					}
					weights[v].boneIndex1 = j;
					weights[v].weight1 = vertDistX * radius * mesh.uv[v].y;
				}

				weights[v].boneIndex0 = 0;
				weights[v].weight0 = 1 - weights[v].weight1;
			}

			// Set bind poses
			bindPoses[j] = joints[j].worldToLocalMatrix * this.transform.localToWorldMatrix;
		}

		// apply to mesh and renderer
		mesh.boneWeights = weights;
		mesh.bindposes = bindPoses;
		skinnedRend.bones = joints;
		skinnedRend.sharedMesh = mesh;
	}

	public void Clear()
	{
		if ( rootJoint == null ) return;

		DestroyImmediate ( rootJoint.gameObject );	
		rootJoint = null;
		rend = this.GetComponent<Renderer> ();
		if (rend.GetType () == typeof ( SkinnedMeshRenderer ))
		{
			var r = (SkinnedMeshRenderer)rend;
			r.sharedMesh = initMesh;
			r.rootBone = null;
		}
		GC.Collect ();
	}
}
