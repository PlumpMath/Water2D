using UnityEngine;

//[RequireComponent( typeof (Rigidbody2D), typeof (SpringJoint2D), typeof(CircleCollider2D) )]
public class WaterJointController : MonoBehaviour
{
	public float forceScalar = 0.005f;
	private Rigidbody2D rigid2D;
	private CircleCollider2D circleCollider2D;
	private SpringJoint2D springJoint2D;
	private float distanceToBottom;
	//private float maxVelocity = 10f;

	private void Update()
	{
		if ( rigid2D.velocity.magnitude > 0 )
		{
			//rigid2D.velocity = Vector2.ClampMagnitude ( rigid2D.velocity, maxVelocity );
			var dist = Vector2.Distance ( this.transform.position, this.springJoint2D.connectedAnchor );
			if (dist < distanceToBottom)
			{
				springJoint2D.dampingRatio = (1 - (0.5f + (dist / distanceToBottom))) * 10;
				if (this.name == "Joint_5")
					Debug.Log ( this.name + " Damping Ratio: " + springJoint2D.dampingRatio );
			}
		}

		if (this.transform.position.y < this.springJoint2D.connectedAnchor.y)
		{
			this.transform.position = this.springJoint2D.connectedAnchor + (Vector2.up * 0.1f);
		}
	}

	private void OnTriggerEnter2D( Collider2D collision )
	{
		// early outs
		if ( rigid2D == null )
		{
			Debug.LogWarning( this.name + " has no Rigidbody2D assigned." );
			return;
		}

		if ( collision.attachedRigidbody == null )
		{
			Debug.LogWarning( collision.name + " has no Rigidbody2D assigned." );
			return;
		}

		// apply impact force
		var force = collision.attachedRigidbody.velocity.y * forceScalar;
		rigid2D.AddForce( Vector2.up * force );
	}

	public void Initialize ( Vector2 position, Vector2 min, float radius, float density )
	{
		// set transform
		this.transform.position = position;
		this.transform.localRotation = Quaternion.identity;
		this.transform.localScale = Vector3.one;

		// add rigidbody
		this.rigid2D = this.gameObject.AddComponent<Rigidbody2D>();
		this.rigid2D.mass = 0;
		this.rigid2D.drag = 0;
		this.rigid2D.angularDrag = 0;
		this.rigid2D.gravityScale = 0;
		this.rigid2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

		// add collider
		this.circleCollider2D = this.gameObject.AddComponent<CircleCollider2D> ();
		this.circleCollider2D.isTrigger = true;
		this.circleCollider2D.radius = radius;

		// add spring joint
		this.springJoint2D = this.gameObject.AddComponent<SpringJoint2D> ();
		this.springJoint2D.autoConfigureDistance = false;
		this.springJoint2D.distance = distanceToBottom = Mathf.Abs ( Mathf.Abs ( position.y ) - Mathf.Abs ( min.y ) );
		this.springJoint2D.connectedAnchor = new Vector2 ( position.x, min.y );
		this.springJoint2D.frequency = density;
		this.springJoint2D.dampingRatio = density / 10;
	}

	public void Initialize( Vector2 position, Vector2 min, float radius, float density, Rigidbody2D previousRigidbody )
	{
		Initialize( position, min, radius, density );
		if ( previousRigidbody == null ) return;
		var springConnect = this.gameObject.AddComponent<SpringJoint2D> ();
		springConnect.autoConfigureDistance = false;
		springConnect.distance = radius * 2;
		springConnect.connectedBody = previousRigidbody;
	}
}
