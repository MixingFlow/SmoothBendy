using System;
using DG.Tweening;
using TMG.UI.Controls;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuitPromptController : AbstractPromptController
{
	[Header("Blockers")]
	[SerializeField]
	private Image m_FullBlocker;

	[Header("Buttons")]
	[SerializeField]
	private BaseUIButton YesBtn;

	[SerializeField]
	private BaseUIButton NoBtn;

	private bool m_IsQuitting;

	public override void InitController(object _data)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		base.InitController(_data);
		((Behaviour)m_FullBlocker).enabled = false;
		((Graphic)m_FullBlocker).color = new Color(((Graphic)m_FullBlocker).color.r, ((Graphic)m_FullBlocker).color.g, ((Graphic)m_FullBlocker).color.b, 0f);
	}

	public override void PlayInComplete()
	{
		AddListeners();
	}

	private void HandleYesBtnOnClick(object sender, EventArgs e)
	{
		RemoveListeners();
		m_IsQuitting = true;
		Kill();
	}

	private void HandleNoBtnOnClick(object sender, EventArgs e)
	{
		RemoveListeners();
		m_IsQuitting = false;
		Kill();
	}

	public override void PlayOutComplete()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		if (m_IsQuitting)
		{
			DOTweenUtil.DOAudioListenerVolume(0f, 0.4f);
			((Behaviour)m_FullBlocker).enabled = true;
			TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(m_FullBlocker.DOFade(1f, 0.5f), (Ease)1), (TweenCallback)delegate
			{
				SceneManager.LoadScene("Reset");
				base.PlayOutComplete();
			});
		}
		else
		{
			base.PlayOutComplete();
		}
	}

	private void AddListeners()
	{
		YesBtn.OnClick += HandleYesBtnOnClick;
		NoBtn.OnClick += HandleNoBtnOnClick;
	}

	private void RemoveListeners()
	{
		YesBtn.OnClick -= HandleYesBtnOnClick;
		NoBtn.OnClick -= HandleNoBtnOnClick;
	}

	protected override void OnDisposed()
	{
		RemoveListeners();
		ShortcutExtensions.DOKill((Component)(object)m_FullBlocker, false);
		base.OnDisposed();
	}
}
