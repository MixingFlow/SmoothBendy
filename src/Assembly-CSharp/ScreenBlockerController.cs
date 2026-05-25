using System;
using DG.Tweening;
using TMG.UI;
using UnityEngine;

public class ScreenBlockerController : BaseUIController
{
	[Header("Canvas Groups")]
	[SerializeField]
	private CanvasGroup m_Blocker;

	public event EventHandler OnShow;

	public event EventHandler OnHide;

	public override void InitController(object _data)
	{
		base.InitController(_data);
		m_Blocker.alpha = 0f;
		((Component)m_Blocker).gameObject.SetActive(false);
	}

	public void Show(float duration = 0.5f, float delay = 0f)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		TweenSettingsExtensions.OnComplete<Tweener>(DOFade(1f, duration, delay), (TweenCallback)delegate
		{
			this.OnShow.Send(this);
		});
	}

	public void Hide(float duration = 0.5f, float delay = 0f)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		TweenSettingsExtensions.OnComplete<Tweener>(DOFade(0f, duration, delay), new TweenCallback(DisableBlocker));
	}

	public void KillFadeTween()
	{
		ShortcutExtensions.DOKill((Component)(object)m_Blocker, false);
		this.OnShow = null;
		this.OnHide = null;
	}

	private Tweener DOFade(float alpha, float duration, float delay)
	{
		ShortcutExtensions.DOKill((Component)(object)m_Blocker, false);
		((Component)m_Blocker).gameObject.SetActive(true);
		return TweenSettingsExtensions.SetEase<Tweener>(TweenSettingsExtensions.SetDelay<Tweener>(m_Blocker.DOFade(alpha, duration), delay), (Ease)1);
	}

	private void DisableBlocker()
	{
		((Component)m_Blocker).gameObject.SetActive(false);
		this.OnHide.Send(this);
		Kill();
	}

	protected override void OnDisposed()
	{
		ShortcutExtensions.DOKill((Component)(object)m_Blocker, false);
		this.OnShow = null;
		this.OnHide = null;
		base.OnDisposed();
	}
}
