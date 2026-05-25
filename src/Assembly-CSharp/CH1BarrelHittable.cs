using TMG.Core;
using UnityEngine;

public class CH1BarrelHittable : TMGMonoBehaviour, IHittable
{
	[SerializeField]
	private Rigidbody m_Rigidbody;

	[SerializeField]
	private CH1BarrelInk m_InkPrefab;

	[SerializeField]
	private GameObject m_ActiveBarrel;

	[SerializeField]
	private GameObject m_Broken;

	private int m_HitCount;

	private int m_HitMax = 5;

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
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
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
			m_ActiveBarrel.SetActive(false);
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
		if (Object.op_Implicit((Object)(object)m_InkPrefab))
		{
			CH1BarrelInk cH1BarrelInk = Object.Instantiate<CH1BarrelInk>(m_InkPrefab);
			cH1BarrelInk.transform.position = ((RaycastHit)(ref hit)).point;
			Vector3 val2 = ((RaycastHit)(ref hit)).point - base.transform.position;
			Quaternion rotation = Quaternion.LookRotation(val2);
			cH1BarrelInk.transform.rotation = rotation;
			if (weaponInfo.ImpactType != ImpactType.AXE)
			{
				Vector3 localEulerAngles = cH1BarrelInk.transform.localEulerAngles;
				localEulerAngles.z = Random.Range(0f, 360f);
				cH1BarrelInk.transform.localEulerAngles = localEulerAngles;
			}
			cH1BarrelInk.transform.SetParent(base.transform);
			cH1BarrelInk.Activate(weaponInfo.ImpactType);
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
