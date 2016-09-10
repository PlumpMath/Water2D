using UnityEngine;
using System.Collections;

[RequireComponent ( typeof ( Rigidbody2D ) )]
public class TestSphereController : MonoBehaviour
{
	public float speedHorizontal = 1f;
	public float forceUp = 1f;
	private Rigidbody2D rigid2D;
	private Vector3 initPos;

	private void Awake ()
	{
		rigid2D = this.GetComponent<Rigidbody2D>();
		initPos = this.transform.position;
	}

	private void Update ()
	{
		if ( Input.GetKeyDown( KeyCode.KeypadEnter ) )
		{
			this.transform.position = initPos;
		}

		var h = Input.GetAxis( "Horizontal" );
		if ( Mathf.Abs( h ) > 0 || Mathf.Abs( h ) < 0 )
		{
			rigid2D.AddForce( Vector2.right * h * speedHorizontal );
		}
		else
		{
			var velo = rigid2D.velocity;
			velo.x = 0;
			rigid2D.velocity = velo;
		}

		if ( Input.GetButtonDown( "Jump" ) )
		{
			rigid2D.AddForce( Vector2.up * forceUp );
		}
	}
}
