using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class InkPipeController : TMGMonoBehaviour
{
	[Header("Transforms")]
	[SerializeField]
	private Transform m_PipeContainer;

	[Header("Options")]
	[SerializeField]
	private float m_ScaleAmount = 1.05f;

	public void TurnOn()
	{
		ShortcutExtensions.DOKill((Component)(object)m_PipeContainer, false);
		TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScaleZ(m_PipeContainer, m_ScaleAmount, 0.2f), (Ease)7), -1, (LoopType)1);
	}

	public void TurnOff()
	{
		ShortcutExtensions.DOKill((Component)(object)m_PipeContainer, false);
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale(m_PipeContainer, 1f, 0.5f), (Ease)6);
	}

	protected override void OnDisposed()
	{
		ShortcutExtensions.DOKill((Component)(object)m_PipeContainer, false);
		base.OnDisposed();
	}
}
