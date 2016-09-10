using UnityEngine;
using System.Collections;

[RequireComponent ( typeof ( Rigidbody2D ))]
public class WaterJointController : MonoBehaviour
{
	public Rigidbody2D rigid2D;
	public float forceScalar = 0.005f;

	private void OnTriggerEnter2D ( Collider2D collision )
	{
		// early outs
		if (rigid2D == null)
		{
			Debug.LogWarning ( this.name + " has no Rigidbody2D assigned." );
			return;
		}

		if ( collision.attachedRigidbody == null )
		{
			Debug.LogWarning( collision.name + " has no Rigidbody2D assigned." );
			return;
		}

		// apply force down
		var force = collision.attachedRigidbody.velocity.y * forceScalar;
		rigid2D.AddForce( Vector2.up * force );
		Debug.Log( "Applied: " + force );
	}
}
