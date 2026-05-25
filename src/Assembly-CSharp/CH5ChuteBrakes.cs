using System;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class CH5ChuteBrakes : Interactable
{
	[SerializeField]
	private Transform m_Handbrake;

	[SerializeField]
	private Transform[] m_Brakes;

	public event EventHandler OnGo;

	public void Activate()
	{
		SetActive(active: true);
	}

	public override void OnInteract()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Expected O, but got Unknown
		SetActive(active: false);
		Sequence val = DOTween.Sequence();
		float num = 0f;
		S13AudioManager.Instance.InvokeEvent("evt_chute_brake_active");
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_Handbrake, new Vector3(-50f, 0f, 0f), 0.5f, (RotateMode)3), (Ease)1));
		TweenSettingsExtensions.InsertCallback(val, num, new TweenCallback(SendOnGo));
		for (int i = 0; i < m_Brakes.Length; i++)
		{
			Transform val2 = m_Brakes[i];
			TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(val2, -1.5f, 0.5f, false), (Ease)1));
		}
		num += 5f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_Handbrake, new Vector3(0f, 0f, 0f), 0.5f, (RotateMode)0), (Ease)1));
		for (int j = 0; j < m_Brakes.Length; j++)
		{
			Transform val3 = m_Brakes[j];
			TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(val3, 0f, 0.5f, false), (Ease)1));
		}
		TweenSettingsExtensions.OnComplete<Sequence>(val, new TweenCallback(InteractOnComplete));
	}

	private void InteractOnComplete()
	{
		SetActive(active: true);
	}

	private void SendOnGo()
	{
		this.OnGo.Send(this);
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
