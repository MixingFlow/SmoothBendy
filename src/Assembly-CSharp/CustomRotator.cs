using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class CustomRotator : TMGMonoBehaviour
{
	[Header("[For Child Rotating Only]")]
	[SerializeField]
	private Transform m_RotatorTransform;

	[SerializeField]
	private Renderer m_Renderer;

	[Header("Duration")]
	[SerializeField]
	private float m_Duration = 1f;

	[Header("Relative Rotation")]
	[SerializeField]
	private Vector3 m_Rotation;

	[Header("Ease")]
	[SerializeField]
	private Ease m_Ease = (Ease)1;

	[Header("Loops (-1 is infinite)")]
	[SerializeField]
	private int m_Loops;

	[SerializeField]
	private LoopType m_LoopType;

	private Transform m_Rotator;

	private Tweener m_RotationTween;

	public override void InitOnComplete()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		base.InitOnComplete();
		m_Rotator = ((!((Object)(object)m_RotatorTransform != (Object)null)) ? base.transform : m_RotatorTransform);
		if ((Object)(object)m_Renderer == (Object)null)
		{
			m_Renderer = ((Component)this).GetComponent<Renderer>();
		}
		m_RotationTween = TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_Rotator, m_Rotation, m_Duration, (RotateMode)3), m_Ease), m_Loops, m_LoopType);
	}

	private void Update()
	{
		if ((Object)(object)m_Renderer == (Object)null || m_RotationTween == null || GameManager.Instance.isPaused)
		{
			return;
		}
		if (m_Renderer.isVisible)
		{
			if (!TweenExtensions.IsPlaying((Tween)(object)m_RotationTween))
			{
				TweenExtensions.Play<Tweener>(m_RotationTween);
			}
		}
		else if (TweenExtensions.IsPlaying((Tween)(object)m_RotationTween))
		{
			TweenExtensions.Pause<Tweener>(m_RotationTween);
		}
	}

	protected override void OnDisposed()
	{
		if (m_RotationTween != null)
		{
			TweenExtensions.Kill((Tween)(object)m_RotationTween, false);
			m_RotationTween = null;
		}
		ShortcutExtensions.DOKill((Component)(object)m_Rotator, false);
		m_Rotator = null;
		m_Renderer = null;
		base.OnDisposed();
	}
}
