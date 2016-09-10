using UnityEngine;
using System.Collections;

public class ForceTest : MonoBehaviour
{
	public KeyCode key = KeyCode.None;
	public float force = 0.01f;
	private Rigidbody2D rigid;

	public void Awake ()
	{
		rigid = this.GetComponent<Rigidbody2D>();
	}

	public void Update ()
	{
		if (Input.GetKeyDown ( key ))
		{
			//Debug.Log ( "AddForce" );
			rigid.AddForce( Vector2.down * force );
		}
	}
}
