using TMG.Core;
using UnityEngine;

public class CH1ChairHittable : TMGMonoBehaviour, IHittable
{
	[SerializeField]
	private Rigidbody m_Rigidbody;

	[SerializeField]
	private GameObject m_ActiveChair;

	[SerializeField]
	private GameObject m_Broken;

	private int m_HitCount;

	private int m_HitMax = 3;

	public override void Init()
	{
		base.Init();
		if ((Object)(object)m_Rigidbody == (Object)null)
		{
			m_Rigidbody = ((Component)this).GetComponent<Rigidbody>();
		}
	}

	public void Hit(RaycastHit hit, WeaponInfo weaponInfo)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Expected O, but got Unknown
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		if (weaponInfo == null)
		{
			return;
		}
		if (weaponInfo.Audio != null && weaponInfo.Audio.Count > 0)
		{
			GameManager.Instance.AudioManager.PlayAtPosition(weaponInfo.Audio[Random.Range(0, weaponInfo.Audio.Count)], ((RaycastHit)(ref hit)).point);
		}
		if (weaponInfo.ImpactType == ImpactType.AXE)
		{
			m_HitCount++;
		}
		else if (weaponInfo.ImpactType == ImpactType.INSTANT_DESTROY)
		{
			m_HitCount = m_HitMax;
		}
		if ((Object)(object)m_Rigidbody != (Object)null && m_HitCount >= m_HitMax)
		{
			m_ActiveChair.SetActive(false);
			m_Broken.SetActive(true);
			m_Broken.transform.SetParent((Transform)null);
			foreach (Transform item in m_Broken.transform)
			{
				Transform val = item;
				Rigidbody component = ((Component)val).GetComponent<Rigidbody>();
				if ((Object)(object)component != (Object)null)
				{
					component.AddExplosionForce(20f, ((RaycastHit)(ref hit)).point, 10f, 2f, (ForceMode)1);
				}
			}
			Dispose();
			return;
		}
		string text = string.Empty;
		float z = 0f;
		if (weaponInfo.ImpactType == ImpactType.AXE)
		{
			text = "GamePlay/Decals/Axe_Hit_Effect";
		}
		else if (weaponInfo.ImpactType == ImpactType.BLUNT)
		{
			text = "GamePlay/Decals/Blunt_Hit_Effect";
			z = Random.Range(0f, 360f);
		}
		if (!string.IsNullOrEmpty(text))
		{
			GameObject fromPool = GameManager.Instance.PoolingManager.GetFromPool(text);
			fromPool.transform.position = ((RaycastHit)(ref hit)).point;
			fromPool.transform.rotation = Quaternion.FromToRotation(Vector3.forward, ((RaycastHit)(ref hit)).normal);
			Vector3 localEulerAngles = fromPool.transform.localEulerAngles;
			localEulerAngles.z = z;
			fromPool.transform.localEulerAngles = localEulerAngles;
			fromPool.transform.SetParent(base.transform);
		}
		if ((Object)(object)m_Rigidbody != (Object)null)
		{
			m_Rigidbody.AddExplosionForce(10f, ((RaycastHit)(ref hit)).point, 10f, 1f, (ForceMode)1);
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
