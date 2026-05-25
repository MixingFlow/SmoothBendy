using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class CH1BarrelInk : TMGMonoBehaviour
{
	[SerializeField]
	private AudioSource m_AudioSource;

	[SerializeField]
	private GameObject m_AxeDecal;

	[SerializeField]
	private GameObject m_BluntDecal;

	public void Activate(ImpactType impactType)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		if (impactType != ImpactType.NONE)
		{
			m_AxeDecal.SetActive(impactType == ImpactType.AXE);
			m_BluntDecal.SetActive(impactType == ImpactType.BLUNT);
			Sequence val = DOTween.Sequence();
			TweenSettingsExtensions.Insert(val, 3.5f, (Tween)(object)m_AudioSource.DOFade(0f, 1f));
			TweenSettingsExtensions.InsertCallback(val, 10f, new TweenCallback(base.Dispose));
		}
	}

	protected override void OnDisposed()
	{
		ShortcutExtensions.DOKill((Component)(object)m_AudioSource, false);
		base.OnDisposed();
	}
}
