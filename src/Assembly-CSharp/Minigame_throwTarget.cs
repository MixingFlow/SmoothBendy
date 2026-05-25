using TMG.Core;
using UnityEngine;

public class Minigame_throwTarget : TMGMonoBehaviour, IHittable
{
	[SerializeField]
	private WinnableMiniGameBaseController m_MiniGame;

	[SerializeField]
	private int m_Score;

	public void Hit(RaycastHit hit, WeaponInfo weaponInfo = null)
	{
		if (weaponInfo != null && (Object)(object)m_MiniGame != (Object)null)
		{
			m_MiniGame.AddScore(m_Score);
		}
	}
}
