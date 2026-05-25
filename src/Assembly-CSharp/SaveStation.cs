using System;
using DG.Tweening;
using S13Audio;
using TMG.Core;
using UnityEngine;

public class SaveStation : TMGMonoBehaviour
{
	[SerializeField]
	private Transform m_Content;

	[SerializeField]
	private Transform m_Card;

	[SerializeField]
	private Interactable m_Interact;

	private Sequence m_Sequence;

	public bool CanSave { get; private set; }

	public event EventHandler OnSaving;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		EnableSaving();
	}

	public void EnableSaving()
	{
		CanSave = true;
		m_Interact.SetActive(active: true);
		m_Interact.OnInteracted += HandleOnInteracted;
	}

	public void DisableSaving()
	{
		CanSave = false;
		m_Interact.OnInteracted -= HandleOnInteracted;
		m_Interact.SetActive(active: false);
	}

	private void HandleOnInteracted(object sender, EventArgs e)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		DisableSaving();
		S13AudioManager.Instance.InvokeEvent("evt_save_punchin");
		this.OnSaving.Send(this);
		TweenSettingsExtensions.OnComplete<Sequence>(OnSave(), new TweenCallback(OnSaveComplete));
	}

	private Sequence OnSave()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		ResetSequence();
		float num = 0f;
		Vector3 localPosition = m_Card.localPosition;
		Vector3 val = localPosition + new Vector3(-0.075f, -0.2f, 0f);
		TweenSettingsExtensions.Insert(m_Sequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_Card, val, 0.25f, false), (Ease)1));
		num += 0.25f;
		TweenSettingsExtensions.Insert(m_Sequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScaleY(m_Content, 1.01f, 0.05f), (Ease)6));
		num += 0.05f;
		TweenSettingsExtensions.Insert(m_Sequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScaleY(m_Content, 1f, 0.05f), (Ease)5));
		num += 0.05f;
		TweenSettingsExtensions.Insert(m_Sequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_Card, localPosition, 0.25f, false), (Ease)1));
		return m_Sequence;
	}

	private void OnSaveComplete()
	{
		EnableSaving();
		if (GameManager.Instance.GameData.NoSaveFile == null || GameManager.Instance.GameData.CurrentSaveFile.ID != GameManager.Instance.GameData.NoSaveFile.ID)
		{
			GameManager.Instance.GameDataManager.Save();
		}
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
