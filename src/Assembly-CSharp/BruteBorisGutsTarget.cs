using TMG.Core;
using UnityEngine;

public class BruteBorisGutsTarget : TMGMonoBehaviour, IHittable
{
	[SerializeField]
	private BruteBorisAi m_Boris;

	public void Hit(RaycastHit hit, WeaponInfo weaponInfo = null)
	{
		if (weaponInfo != null && Object.op_Implicit((Object)(object)m_Boris))
		{
			m_Boris.Hit(weaponInfo.IsBullet);
		}
	}
}
