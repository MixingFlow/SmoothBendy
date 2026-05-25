using TMG.Core;
using UnityEngine;

public class CH4PalletStackHittable : TMGMonoBehaviour, IHittable
{
	[SerializeField]
	private GameObject m_Active;

	[SerializeField]
	private GameObject m_Broken;

	public void Hit(RaycastHit hit, WeaponInfo weaponInfo)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected O, but got Unknown
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		if (weaponInfo == null)
		{
			return;
		}
		if (weaponInfo.Audio != null && weaponInfo.Audio.Count > 0)
		{
			GameManager.Instance.AudioManager.PlayAtPosition(weaponInfo.Audio[Random.Range(0, weaponInfo.Audio.Count)], ((RaycastHit)(ref hit)).point);
		}
		m_Active.SetActive(false);
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
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
