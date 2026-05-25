using System;
using DG.Tweening;
using UnityEngine;

public class CH3Dropbox : Interactable
{
	[Header("Dropbox")]
	[SerializeField]
	private Transform m_Door;

	public event EventHandler OnDopped;

	public override void OnInteract()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		ShortcutExtensions.DOKill((Component)(object)m_Door, false);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_Door, new Vector3(0f, 0f, 360f), 2f, (RotateMode)3), (Ease)24), new TweenCallback(SendOnDropped));
	}

	private void SendOnDropped()
	{
		this.OnDopped.Send(this);
	}

	protected override void OnDisposed()
	{
		this.OnDopped = null;
		base.OnDisposed();
	}
}
