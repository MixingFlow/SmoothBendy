using System;
using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class CH5ReelStation : TMGMonoBehaviour
{
	[SerializeField]
	private Interactable m_EmptyReel;

	[SerializeField]
	private Transform m_Reel;

	public event EventHandler OnInteracted;

	public event EventHandler OnComplete;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_EmptyReel.gameObject.SetActive(false);
		((Component)m_Reel).gameObject.SetActive(false);
	}

	public void Activate()
	{
		m_EmptyReel.gameObject.SetActive(true);
		m_EmptyReel.OnInteracted += HandleEmptyReelOnInteracted;
		m_EmptyReel.SetActive(active: true);
	}

	private void HandleEmptyReelOnInteracted(object sender, EventArgs e)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Expected O, but got Unknown
		m_EmptyReel.OnInteracted -= HandleEmptyReelOnInteracted;
		m_EmptyReel.Dispose();
		this.OnInteracted.Send(this);
		Vector3 localPosition = m_Reel.localPosition;
		Vector3 localPosition2 = localPosition - new Vector3(0.2f, 0f, 0f);
		m_Reel.localPosition = localPosition2;
		((Component)m_Reel).gameObject.SetActive(true);
		Sequence val = DOTween.Sequence();
		TweenSettingsExtensions.Insert(val, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_Reel, localPosition, 0.5f, false), (Ease)7));
		TweenSettingsExtensions.Insert(val, 0.5f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(TweenSettingsExtensions.SetDelay<Tweener>(ShortcutExtensions.DOLocalRotate(m_Reel, new Vector3(720f, 0f, 0f), 4f, (RotateMode)3), 0.5f), (Ease)1));
		TweenSettingsExtensions.InsertCallback(val, 2f, (TweenCallback)delegate
		{
			this.OnComplete.Send(this);
		});
	}

	protected override void OnDisposed()
	{
		this.OnComplete = null;
		base.OnDisposed();
	}
}
