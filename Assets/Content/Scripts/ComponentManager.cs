using UnityEngine;
using System.Collections;

public class ComponentManager
{
	private SkinnedMeshRenderer skinnedMeshRend;
	public SkinnedMeshRenderer GetSkinnedMeshRenderer( Transform trans )
	{
		if ( skinnedMeshRend != null )
		{
			return skinnedMeshRend;
		}
		return skinnedMeshRend = trans.GetComponent<SkinnedMeshRenderer>();
	}

	private MeshGenerator meshGenerator;
	public MeshGenerator GetMeshGenerator( Transform trans )
	{
		if (skinnedMeshRend != null)
		{
			return meshGenerator;
		}
		return meshGenerator = trans.GetComponent<MeshGenerator>();
	}
}
