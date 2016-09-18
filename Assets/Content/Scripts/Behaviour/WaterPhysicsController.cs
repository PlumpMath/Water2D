using System;
using UnityEngine;

[RequireComponent ( typeof ( SkinnedMeshRenderer ), typeof ( MeshGenerator ) )]
public class WaterPhysicsController : MonoBehaviour
{
	// public fields
	public bool createOnAwake = true;
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
		if ( !createOnAwake ) return;
		Clear();
		CreateJoints();
	}

	public void CreateJoints()
	{
		CreateJoints( this.GetComponent<SkinnedMeshRenderer> () );
	}

	// Creates and applies skinned joints to the mesh in passed renderer
	public void CreateJoints ( SkinnedMeshRenderer rend )
	{
		// generates mesh
		var meshGenerator = this.GetComponent<MeshGenerator> ();
		meshGenerator.LOD = this.LOD;
		meshGenerator.CreateMesh ( rend );
		var mesh = rend.sharedMesh;
		var hull = meshGenerator.hull;

		var minWorld = mesh.vertices.GetMin ( this.transform );
		var maxWorld = mesh.vertices.GetMax ( this.transform );
		var radius = (( maxWorld.x - minWorld.x ) / (LOD + 1)) / 2;

		// Set joint variables
		var joints = new Transform[hull.Length - 1];
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

		for ( int j = 0; j < mesh.vertices.Length; j++ )
		{
			weights[j].boneIndex0 = 0;
			weights[j].weight0 = 1;
		}

		for ( int i = 1; i < joints.Length; i++ )
		{
			// create joint
			var joint = new  GameObject( "Joint_" + i );
			joints[i] = joint.transform;

			var jointController = joint.AddComponent<WaterJointController> ();
			var posWorld = this.transform.TransformPoint( mesh.vertices[hull[i]] );
			posWorld.y -= radius;
			jointController.Initialize ( posWorld, minWorld, radius, damping, previousRigidbody2D );
			previousRigidbody2D = joint.GetComponent<Rigidbody2D>();
			joint.transform.parent = rootJoint;
			
			// set bone weights
			weights[hull[i]].boneIndex0 = i;
			weights[hull[i]].weight0 = 1;

			// Set bind poses
			bindPoses[i] = joints[i].worldToLocalMatrix * this.transform.localToWorldMatrix;
		}
		
		// apply to mesh and renderer
		mesh.boneWeights = weights;
		mesh.bindposes = bindPoses;
		rend.bones = joints;
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
			//Debug.Log ( "Root destroyed" );
		}
		rootJoint = null;
		GC.Collect ();
	}
}
