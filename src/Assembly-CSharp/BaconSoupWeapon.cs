using System.Collections.Generic;
using UnityEngine;

public class BaconSoupWeapon : ThrowableObject
{
	[SerializeField]
	private GameObject m_Active;

	[SerializeField]
	private Rigidbody[] m_BrokenPieces;

	public Rigidbody[] BrokenPieces => m_BrokenPieces;

	public override void DoHitStuff(Vector3 hitPosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		base.DoHitStuff(hitPosition);
		SendOnHit();
		GameManager.Instance.AiGlobalNetwork.SetNoiseLocation(hitPosition);
		if (m_BrokenPieces.Length > 0)
		{
			m_Active.SetActive(false);
			for (int i = 0; i < m_BrokenPieces.Length; i++)
			{
				Rigidbody val = m_BrokenPieces[i];
				if (Object.op_Implicit((Object)(object)val))
				{
					((Component)val).transform.SetParent((Transform)null);
					((Component)val).gameObject.SetActive(true);
					val.velocity = ((Component)this).GetComponent<Rigidbody>().velocity;
					val.AddExplosionForce(5f, base.transform.position, 15f, 2f, (ForceMode)1);
				}
			}
		}
		if (Object.op_Implicit((Object)(object)m_Respawner))
		{
			List<MeshRenderer> list = new List<MeshRenderer>();
			for (int j = 0; j < m_BrokenPieces.Length; j++)
			{
				list.Add(((Component)m_BrokenPieces[j]).GetComponent<MeshRenderer>());
			}
			m_Respawner.CheckMeshVisibility(list);
			m_Respawner.Respawn();
		}
	}
}
