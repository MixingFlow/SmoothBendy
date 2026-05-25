using DG.Tweening;

public class CH5ClosingSequenceController : BaseController
{
	private Sequence m_Sequence;

	public override void Activate()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		TweenSettingsExtensions.OnComplete<Sequence>(DOSequence(), new TweenCallback(base.SendOnComplete));
	}

	private Sequence DOSequence()
	{
		ResetSequence();
		TweenSettingsExtensions.Insert(m_Sequence, 0f, (Tween)(object)DOTweenUtil.DOAudioListenerVolume(0f, 2f));
		return m_Sequence;
	}

	private void ResetSequence()
	{
		KillSequence();
		m_Sequence = DOTween.Sequence();
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
