using System;
using DG.Tweening;
using UnityEngine;

public class InteractablePowerLever : Interactable
{
	[Header("Options")]
	[SerializeField]
	private Transform m_Lever;

	public event EventHandler OnComplete;

	public override void Init()
	{
		base.Init();
		SetActive(active: false);
	}

	public override void OnInteract()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		base.OnInteract();
		SetActive(active: false);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_Lever, new Vector3(0f, -85f, 0f), 0.75f, (RotateMode)0), (Ease)27), new TweenCallback(HandleLeverRotationOnComplete));
	}

	private void HandleLeverRotationOnComplete()
	{
		this.OnComplete.Send(this);
	}

	public void ForceOpen()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		SetActive(active: false);
		m_Lever.localEulerAngles = new Vector3(0f, -85f, 0f);
	}

	protected override void OnDisposed()
	{
		ShortcutExtensions.DOKill((Component)(object)m_Lever, false);
		base.OnDisposed();
	}
}
