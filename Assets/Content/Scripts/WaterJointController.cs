using UnityEngine;
using System.Collections;

[RequireComponent ( typeof ( Rigidbody2D ), typeof ( CircleCollider2D ) )]
public class WaterJointController : MonoBehaviour
{
	private Rigidbody2D rigid;

	public WaterJointController( Rigidbody2D newRigid )
	{
		rigid = newRigid;
	}

	private void OnTriggerEnter2D ( Collider2D collision )
	{
		var force = collision.attachedRigidbody.velocity.y;
		rigid.AddForce( Vector2.up * force );
	}
}
