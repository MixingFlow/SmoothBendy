using System;
using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class CH3MechDoor : TMGMonoBehaviour
{
	[SerializeField]
	private Transform m_LeftDoor;

	[SerializeField]
	private Transform m_RightDoor;

	private Sequence m_Sequence;

	private float m_BaseSpeed = 5f;

	public event EventHandler OnOpen;

	public event EventHandler OnClose;

	public void ForceOpen()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		m_LeftDoor.localPosition = new Vector3(0f, 0f, -3.4f);
		m_RightDoor.localPosition = new Vector3(0f, 0f, 3.4f);
	}

	public void Open()
	{
		Open(m_BaseSpeed);
	}

	public void Open(float speed)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		ResetSequence();
		TweenSettingsExtensions.Insert(m_Sequence, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveZ(m_LeftDoor, -3.4f, speed, false), (Ease)7));
		TweenSettingsExtensions.Insert(m_Sequence, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveZ(m_RightDoor, 3.4f, speed, false), (Ease)7));
		TweenSettingsExtensions.OnComplete<Sequence>(m_Sequence, new TweenCallback(OpenOnComplete));
	}

	private void OpenOnComplete()
	{
		this.OnOpen.Send(this);
	}

	public void ForceClose()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		m_LeftDoor.localPosition = Vector3.zero;
		m_RightDoor.localPosition = Vector3.zero;
	}

	public void Close()
	{
		Close(m_BaseSpeed);
	}

	public void Close(float speed)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		ResetSequence();
		TweenSettingsExtensions.Insert(m_Sequence, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveZ(m_LeftDoor, 0f, speed, false), (Ease)7));
		TweenSettingsExtensions.Insert(m_Sequence, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveZ(m_RightDoor, 0f, speed, false), (Ease)7));
		TweenSettingsExtensions.OnComplete<Sequence>(m_Sequence, new TweenCallback(CloseOnComplete));
	}

	private void CloseOnComplete()
	{
		this.OnClose.Send(this);
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
		this.OnOpen = null;
		this.OnClose = null;
		base.OnDisposed();
	}
}
