using UnityEngine;
using System.Collections;


[RequireComponent ( typeof ( Renderer ) )]
public class WaterVisualController : MonoBehaviour
{
	public Vector2 waveSpeed = Vector2.zero;
	public Vector2 waveSpeedTarget = Vector2.zero;
	private Renderer rend;

	public void Awake ()
	{
		rend = this.GetComponent<Renderer> ();
	}

	private void LateUpdate ()
	{
		// moves the bump map offset 
		var offset = rend.material.GetTextureOffset ( "_BumpMap0" );
		offset += waveSpeed * Time.deltaTime;
		offset = new Vector2 ( offset.x % 1, offset.y % 1 );
		rend.material.SetTextureOffset ( "_BumpMap0", offset );

		offset = rend.material.GetTextureOffset ( "_BumpMap1" );
		offset += waveSpeedTarget * Time.deltaTime;
		offset = new Vector2 ( offset.x % 1, offset.y % 1 );
		rend.material.SetTextureOffset ( "_BumpMap1", offset );
	}
}
