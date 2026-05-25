using System;
using System.Collections;
using System.Collections.Generic;
using TMG.Core;
using UnityEngine;

public class ThrowWeaponRespawner : TMGMonoBehaviour
{
	[SerializeField]
	private MeshRenderer m_InvisibleChecker;

	[SerializeField]
	private ThrowWeapon m_ThrowWeaponPrefab;

	[SerializeField]
	private bool m_IsRespawner = true;

	private ThrowWeapon m_ThrowWeapon;

	private List<MeshRenderer> m_BrokenPieces = new List<MeshRenderer>();

	private bool m_IsSpawnerActive;

	private bool m_IsRespawned;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		GenerateThrowWeapon();
	}

	private void HandleThrowWeaponOnInteract(object sender, EventArgs e)
	{
		m_ThrowWeapon.OnInteract -= HandleThrowWeaponOnInteract;
		ThrowWeapon throwWeapon = m_ThrowWeapon;
		if (Object.op_Implicit((Object)(object)GameManager.Instance.Player.WeaponGameObject) && Object.op_Implicit((Object)(object)GameManager.Instance.Player.WeaponGameObject.GetComponent<ThrowWeapon>()))
		{
			m_ThrowWeapon.Dispose();
			throwWeapon = GameManager.Instance.Player.WeaponGameObject.GetComponent<ThrowWeapon>();
		}
		throwWeapon.AddAmmo(this);
		throwWeapon.OnThrow += HandleBaconSoupOnThrow;
	}

	private void HandleBaconSoupOnThrow(object sender, EventArgs e)
	{
		ThrowWeapon throwWeapon = sender as ThrowWeapon;
		throwWeapon.OnThrow -= HandleBaconSoupOnThrow;
		if (!m_IsRespawner)
		{
			Dispose();
		}
	}

	public void CheckMeshVisibility(List<MeshRenderer> meshes)
	{
		m_IsRespawned = false;
		m_BrokenPieces = meshes;
	}

	public void Respawn()
	{
		((MonoBehaviour)this).StartCoroutine(OnRespawn());
	}

	private IEnumerator OnRespawn()
	{
		yield return (object)new WaitForSeconds(30f);
		yield return (object)new WaitForEndOfFrame();
		if (base.IsDisposed)
		{
			yield return null;
		}
		if (!m_IsRespawned)
		{
			m_IsSpawnerActive = true;
		}
	}

	private void Update()
	{
		if (!m_IsSpawnerActive || ((Renderer)m_InvisibleChecker).isVisible)
		{
			return;
		}
		bool flag = false;
		if (!((m_BrokenPieces != null) & (m_BrokenPieces.Count > 0)))
		{
			return;
		}
		for (int i = 0; i < m_BrokenPieces.Count; i++)
		{
			MeshRenderer val = m_BrokenPieces[i];
			if (Object.op_Implicit((Object)(object)val) && ((Renderer)val).isVisible)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			return;
		}
		for (int j = 0; j < m_BrokenPieces.Count; j++)
		{
			MeshRenderer val2 = m_BrokenPieces[j];
			if (Object.op_Implicit((Object)(object)val2))
			{
				Object.Destroy((Object)(object)((Component)m_BrokenPieces[j]).gameObject);
			}
		}
		GenerateThrowWeapon();
	}

	private void GenerateThrowWeapon()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		m_IsSpawnerActive = false;
		m_IsRespawned = true;
		m_BrokenPieces.Clear();
		m_ThrowWeapon = Object.Instantiate<ThrowWeapon>(m_ThrowWeaponPrefab, base.transform.position, base.transform.rotation);
		m_ThrowWeapon.OnInteract += HandleThrowWeaponOnInteract;
		if (!m_IsRespawner)
		{
			m_ThrowWeapon.transform.SetParent(base.transform.parent);
		}
	}

	protected override void OnDisposed()
	{
		if (Object.op_Implicit((Object)(object)m_ThrowWeapon) && m_IsRespawner)
		{
			m_ThrowWeapon.Dispose();
		}
		base.OnDisposed();
	}
}
