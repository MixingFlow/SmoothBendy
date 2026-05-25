using System.Collections.Generic;
using TMG.Core;
using UnityEngine;

public class DestructibleObject : TMGMonoBehaviour
{
	[Header("Objects")]
	[SerializeField]
	private GameObject m_Active;

	[SerializeField]
	private GameObject m_Broken;

	private List<Rigidbody> m_BrokenPieces = new List<Rigidbody>();

	public override void Init()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		base.Init();
		foreach (Transform item in m_Broken.transform)
		{
			Transform val = item;
			Rigidbody component = ((Component)val).GetComponent<Rigidbody>();
			if (Object.op_Implicit((Object)(object)component) && !m_BrokenPieces.Contains(component))
			{
				m_BrokenPieces.Add(component);
			}
		}
		m_Broken.SetActive(false);
	}

	public void Destroy(Vector3 explosionPosition, float force = 20f, float radius = 10f, float upwardModifier = 1f)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		m_Active.SetActive(false);
		m_Broken.SetActive(true);
		m_Broken.transform.SetParent((Transform)null);
		for (int i = 0; i < m_BrokenPieces.Count; i++)
		{
			m_BrokenPieces[i].AddExplosionForce(force, explosionPosition, radius, upwardModifier, (ForceMode)1);
		}
		Dispose();
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
