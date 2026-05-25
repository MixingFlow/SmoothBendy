using System.Collections.Generic;
using TMG.Core;
using UnityEngine;

public class BrokenWeapon : TMGMonoBehaviour
{
	[SerializeField]
	private List<Rigidbody> m_Pieces;

	public void Break(Transform weapon, Vector3 fromPosition)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		base.transform.position = weapon.position;
		base.transform.rotation = weapon.rotation;
		for (int i = 0; i < m_Pieces.Count; i++)
		{
			((Component)m_Pieces[i]).transform.SetParent((Transform)null);
			m_Pieces[i].AddExplosionForce(5f, fromPosition, 15f, 1f, (ForceMode)1);
		}
		Dispose();
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
