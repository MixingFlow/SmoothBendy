using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class TimedDisposer : TMGMonoBehaviour
{
	[SerializeField]
	private float m_TimeDelay;

	private Sequence m_Sequence;

	public override void OnEnable()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		base.OnEnable();
		m_Sequence = DOTween.Sequence();
		TweenSettingsExtensions.InsertCallback(m_Sequence, m_TimeDelay, new TweenCallback(base.Dispose));
	}

	public override void OnDisable()
	{
		base.OnDisable();
		KillSequence();
	}

	private void KillSequence()
	{
		if (m_Sequence != null)
		{
			TweenExtensions.Kill((Tween)(object)m_Sequence, false);
			m_Sequence = null;
		}
	}

	protected override void OnDisposed()
	{
		KillSequence();
		base.OnDisposed();
	}
}
