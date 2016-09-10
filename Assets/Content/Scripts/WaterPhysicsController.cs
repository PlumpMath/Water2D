using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class WaterPhysicsController : MonoBehaviour
{
	public int jointLOD = 9;
	[Range(0f,1f)] public float density = 0.5f;
	public bool weightedVertices = true;
	public Mesh initMesh;
	private Renderer rend;
	private Transform rootJoint;

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
		//var height = max.y - min.y;
		var distX = width / jointLOD;
		var radius = distX / 2;
		//Debug.Log( "Width: " + width + "  DistX: " + distX + "  MinX: " + min.x);

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
			joint.transform.position = pos;
			joint.transform.localRotation = Quaternion.identity;
			joint.transform.localScale = Vector3.one;
			joint.transform.parent = rootJoint;
			joints[j] = joint.transform;

			// add rigidbody
			var rigid2D = joint.AddComponent<Rigidbody2D> ();
			rigid2D.mass = 0;
			rigid2D.drag = 0;
			rigid2D.angularDrag = 0;
			rigid2D.gravityScale = 0;
			rigid2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

			// add collider
			var circleColl = joint.AddComponent<CircleCollider2D> ();
			circleColl.isTrigger = true;
			circleColl.radius = radius;

			// add spring joint
			var springBottom = joint.AddComponent<SpringJoint2D> ();
			springBottom.autoConfigureDistance = false;
			springBottom.distance = Mathf.Abs( Mathf.Abs( pos.y ) - Mathf.Abs( min.y ));
			springBottom.connectedAnchor = new Vector2 ( pos.x, min.y );
			springBottom.frequency = density;
			springBottom.dampingRatio = density / 10;

			// add WaterJointController
			var jointController = joint.AddComponent<WaterJointController> ();
			jointController.rigid2D = rigid2D;
			jointController.springJoint2D = springBottom;

			// Set bones weights
			for ( int v = 0; v < mesh.vertexCount; v++ )
			{
				var vertWorldPos = this.transform.TransformPoint( mesh.vertices[v] );
				vertWorldPos.y = joints[j].position.y;

				if ( vertWorldPos.x >= joints[j].position.x - radius &&
				     vertWorldPos.x <= joints[j].position.x + radius )
				{
					var vertDistX = 1f;
					if ( weightedVertices )
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

			// early out or add spring joints to connect them
			if (j <= 1) continue;
			var springConnect = joint.AddComponent<SpringJoint2D> ();
			springConnect.autoConfigureDistance = false;
			springConnect.distance = distX;
			springConnect.connectedBody = joints[j - 1].GetComponent<Rigidbody2D> ();
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
