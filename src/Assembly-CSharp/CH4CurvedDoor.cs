using System;
using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class CH4CurvedDoor : TMGMonoBehaviour
{
	[SerializeField]
	private Transform m_DoorLeft;

	[SerializeField]
	private Transform m_DoorRight;

	private Sequence m_Sequence;

	private OcclusionPortal m_Portal;

	private float m_BaseSpeed = 5f;

	public event EventHandler OnOpen;

	public event EventHandler OnClose;

	public override void Init()
	{
		base.Init();
		m_Portal = ((Component)this).GetComponent<OcclusionPortal>();
	}

	public void ForceOpen()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		m_Portal.open = true;
		m_DoorLeft.localEulerAngles = new Vector3(0f, -80f, 0f);
		m_DoorRight.localEulerAngles = new Vector3(0f, 80f, 0f);
	}

	public void Open()
	{
		m_Portal.open = true;
		Open(m_BaseSpeed);
	}

	public void Open(float speed)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Expected O, but got Unknown
		m_Portal.open = true;
		ResetSequence();
		TweenSettingsExtensions.Insert(m_Sequence, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_DoorLeft, new Vector3(0f, -80f, 0f), speed, (RotateMode)0), (Ease)7));
		TweenSettingsExtensions.Insert(m_Sequence, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_DoorRight, new Vector3(0f, 80f, 0f), speed, (RotateMode)0), (Ease)7));
		TweenSettingsExtensions.OnComplete<Sequence>(m_Sequence, new TweenCallback(OpenOnComplete));
	}

	private void OpenOnComplete()
	{
		this.OnOpen.Send(this);
	}

	public void ForceClose()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		m_Portal.open = false;
		m_DoorLeft.localEulerAngles = Vector3.zero;
		m_DoorRight.localEulerAngles = Vector3.zero;
	}

	public void Close()
	{
		Close(m_BaseSpeed);
	}

	public void Close(float speed)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		ResetSequence();
		TweenSettingsExtensions.Insert(m_Sequence, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_DoorLeft, Vector3.zero, speed, (RotateMode)0), (Ease)7));
		TweenSettingsExtensions.Insert(m_Sequence, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_DoorRight, Vector3.zero, speed, (RotateMode)0), (Ease)7));
		TweenSettingsExtensions.OnComplete<Sequence>(m_Sequence, new TweenCallback(CloseOnComplete));
	}

	private void CloseOnComplete()
	{
		m_Portal.open = false;
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
