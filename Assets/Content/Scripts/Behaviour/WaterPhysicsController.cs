using System;
using UnityEngine;

[RequireComponent ( typeof ( SkinnedMeshRenderer ), typeof ( MeshGenerator ) )]
public class WaterPhysicsController : MonoBehaviour
{
	// public fields
	public bool useWeightedVertices = true;
	public int LOD = 9;
	public int verticesPerJoint = 1;
	[Range ( 0f, 1f )]
	public float damping = 0.5f;

	// private fields
	private Transform rootJoint;
	private Rigidbody2D previousRigidbody2D = null;
	private const string rootName = "Joint_0_Root";

	private void Awake()
	{
		Clear();
		CreateJoints( this.GetComponent<SkinnedMeshRenderer>() );
	}


	// Creates and applies skinned joints to the mesh in passed renderer
	public void CreateJoints ( SkinnedMeshRenderer rend )
	{
		// generates mesh
		var meshGenerator = this.GetComponent<MeshGenerator> ();
		meshGenerator.CreateMesh ( rend );
		var mesh = rend.sharedMesh;

		var minWorld = mesh.vertices.GetMin ( this.transform );
		var maxWorld = mesh.vertices.GetMax ( this.transform );
		var width = maxWorld.x - minWorld.x;
		var distX = width / LOD;
		var radius = distX / 2;

		// Set joint variables
		var joints = new Transform[LOD + 1];
		var weights = new BoneWeight[mesh.vertexCount];
		var bindPoses = new Matrix4x4[joints.Length];

		// Initialize root joint
		rootJoint = new GameObject ( rootName ).transform;
		rootJoint.parent = this.transform;
		rootJoint.localPosition = Vector3.zero;
		rootJoint.localRotation = Quaternion.identity;
		rootJoint.localScale = Vector3.one;
		rend.rootBone = rootJoint;
		joints[0] = rootJoint;
		bindPoses[0] = rootJoint.worldToLocalMatrix * this.transform.localToWorldMatrix;

		for (var j = 1; j <= LOD; j++)
		{
			var joint = new GameObject ( "Joint_" + j );

			// set position and parent			
			var pos = new Vector3
			{
				x = minWorld.x + radius + distX * (j - 1),
				y = maxWorld.y - radius
			};

			var jointController = joint.AddComponent<WaterJointController> ();
			jointController.Initialize ( pos, minWorld, radius, damping, previousRigidbody2D );

			joint.transform.parent = rootJoint;
			joints[j] = joint.transform;
			previousRigidbody2D = joint.GetComponent<Rigidbody2D> ();

			// Set bones weights
			for (int v = 0; v < mesh.vertexCount; v++)
			{
				var vertWorldPos = this.transform.TransformPoint ( mesh.vertices[v] );
				vertWorldPos.y = joints[j].position.y;

				if (vertWorldPos.x >= joints[j].position.x - radius &&
					 vertWorldPos.x <= joints[j].position.x + radius)
				{
					var vertDistX = 1f;
					if (useWeightedVertices)
					{
						vertDistX = Mathf.Abs ( Mathf.Abs ( joints[j].position.x ) - Mathf.Abs ( vertWorldPos.x ) );
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
		rend.bones = joints;
		rend.sharedMesh = mesh;
	}

	public void Clear ()
	{
		var r = this.GetComponent<SkinnedMeshRenderer> ();
		r.sharedMesh = Primitives.Quad;
		r.rootBone = null;
		var root = this.transform.FindChild( rootName );
		if (root != null)
		{
			DestroyImmediate ( root.gameObject );
			Debug.Log ( "Root destroyed" );
		}
		rootJoint = null;
		GC.Collect ();
	}
}
