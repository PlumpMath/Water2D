using System;
using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Renderer))]
public class WaterPhysicsController : MonoBehaviour
{
	public int boneAmount = 1;
	private Renderer rend;
	private Transform rootJoint;
	private Mesh initMesh;

	private void Awake ()
	{
		rend = this.GetComponent<Renderer>();
		if ( rend.GetType() == typeof (SkinnedMeshRenderer) )
		{
			Clear();
			CreateJoints( (SkinnedMeshRenderer)rend );
		}
	}

	// Creates and applies skinned joints to the mesh in passed renderer
	public void CreateJoints( SkinnedMeshRenderer skinnedRend )
	{
		// copy mesh
		initMesh = skinnedRend.sharedMesh.Copy();
		initMesh.name = "Init Plane";
		var mesh = skinnedRend.sharedMesh.Copy();
		mesh.name = "Skinned Plane";

		// Create joints
		rootJoint = new  GameObject("Joint_0_Root").transform;
		rootJoint.parent = this.transform;
		rootJoint.localPosition = Vector3.zero;
		rootJoint.localRotation = Quaternion.identity;
		rootJoint.localScale = Vector3.one;
		skinnedRend.rootBone = rootJoint;

		var min = mesh.vertices.GetMin( this.transform );
		var max = mesh.vertices.GetMax( this.transform );
		var width = max.x - min.x;
		//var height = max.y - min.y;
		var distX = width / boneAmount;
		var radius = distX / 2;
		//Debug.Log( "Width: " + width + "  DistX: " + distX + "  MinX: " + min.x);

		var joints = new Transform[boneAmount + 1];
		joints[0] = rootJoint;

		for ( int i = 1; i <= boneAmount; i++ )
		{
			var joint = new GameObject ( "Joint_" + i );

			// set position and parent
			var pos = Vector3.zero;
			pos.x = min.x + radius + distX * (i - 1);
			pos.y = max.y - radius;
			joint.transform.position = pos;
			joint.transform.localRotation = Quaternion.identity;
			joint.transform.localScale = Vector3.one;
			joint.transform.parent = rootJoint;

			// add rigidbody
			var rigid2D = joint.AddComponent<Rigidbody2D> ();
			rigid2D.mass = 0;
			rigid2D.drag = 0;
			rigid2D.angularDrag = 0;
			rigid2D.gravityScale = 0;
			rigid2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

			// add collider
			var circleColl = joint.AddComponent<CircleCollider2D>();
			circleColl.isTrigger = true;
			circleColl.radius = radius;

			// add spring joint
			var springTop = joint.AddComponent<SpringJoint2D> ();
			springTop.autoConfigureDistance = false;
			springTop.distance = 1f + UnityEngine.Random.Range( 0.0f, 0.3f );
			springTop.connectedAnchor = new Vector2( pos.x, max.y );

			// add spring joints to connect joints
			if ( i > 1 )
			{
				var springConnect = joint.AddComponent<SpringJoint2D> ();
				springConnect.autoConfigureDistance = false;
				springConnect.distance = distX;
				springConnect.connectedBody = joints[i - 1].GetComponent<Rigidbody2D>();
			}

			joints[i] = joint.transform;
		}


		// Set bones weights
		BoneWeight[] weights = new BoneWeight[mesh.vertexCount];
		
		for ( int v = 0; v < mesh.vertexCount; v++ )
		{
			for ( int j = 1; j < joints.Length; j++ )
			{
				var vertWorldX = this.transform.TransformPoint( mesh.vertices[v] );
				vertWorldX.y = joints[j].position.y;

				if ( vertWorldX.x >= joints[j].position.x - radius && 
					 vertWorldX.x <= joints[j].position.x + radius )
				{
					var vertDistX = 1;
					weights[v].boneIndex1 = j;
					weights[v].weight1 = vertDistX * radius * mesh.uv[v].y;
				}
				
				weights[v].boneIndex0 = 0;
				weights[v].weight0 = 1 - weights[v].weight1;
			}
		}

		// Set bind poses
		Matrix4x4[] bindPoses = new Matrix4x4[joints.Length];
		for ( int j=0;  j < joints.Length; j++ )
		{
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
		if (rootJoint == null) return;

		DestroyImmediate( rootJoint.gameObject );
		rootJoint = null;
		rend = this.GetComponent<Renderer>();
		if (rend.GetType() == typeof (SkinnedMeshRenderer) )
		{
			var r = (SkinnedMeshRenderer)rend;
			r.sharedMesh = initMesh;
			r.rootBone = null;
		}
		GC.Collect ();
	}
}
