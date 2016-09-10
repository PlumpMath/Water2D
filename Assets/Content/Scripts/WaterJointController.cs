using UnityEngine;
using System.Collections;

[RequireComponent ( typeof ( Rigidbody2D ), typeof ( SpringJoint2D ) )]
public class WaterJointController : MonoBehaviour
{
	public Rigidbody2D rigid2D;
	public float forceScalar = 0.005f;
	public SpringJoint2D springJoint2D;

	private void Update ()
	{
		if ( this.transform.position.y <= springJoint2D.connectedAnchor.y )
		{
			var targetPos = springJoint2D.connectedAnchor;
			targetPos.y += 1;
			this.transform.position = targetPos;
		}
	}

	private void OnTriggerEnter2D ( Collider2D collision )
	{
		// early outs
		if (rigid2D == null)
		{
			Debug.LogWarning ( this.name + " has no Rigidbody2D assigned." );
			return;
		}

		if (collision.attachedRigidbody == null)
		{
			Debug.LogWarning ( collision.name + " has no Rigidbody2D assigned." );
			return;
		}

		// apply force down
		var force = collision.attachedRigidbody.velocity.y * forceScalar;
		rigid2D.AddForce ( Vector2.up * force );
		//Debug.Log( "Applied: " + force );
	}
}
