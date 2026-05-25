using System;
using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class CH5SafehouseDoor : TMGMonoBehaviour
{
	[SerializeField]
	private Transform m_Door;

	private OcclusionPortal m_Portal;

	public event EventHandler OnOpen;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_Portal = base.gameObject.GetComponent<OcclusionPortal>();
		if (Object.op_Implicit((Object)(object)m_Portal))
		{
			m_Portal.open = false;
		}
	}

	public void ForceOpen()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)m_Portal))
		{
			m_Portal.open = true;
		}
		((Component)m_Door).transform.localEulerAngles = new Vector3(0f, 135f, 0f);
	}

	public void ForceClose()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)m_Portal))
		{
			m_Portal.open = false;
		}
		((Component)m_Door).transform.localEulerAngles = new Vector3(0f, 0f, 0f);
	}

	public void CloseDoor()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(((Component)m_Door).transform, Vector3.zero, 1f, (RotateMode)0), (Ease)5), new TweenCallback(CloseDoorOnComplete));
	}

	private void CloseDoorOnComplete()
	{
		if (Object.op_Implicit((Object)(object)m_Portal))
		{
			m_Portal.open = false;
		}
	}

	public void OpenDoor()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)m_Portal))
		{
			m_Portal.open = true;
		}
		ShortcutExtensions.DOLocalRotate(((Component)m_Door).transform, new Vector3(0f, 135f, 0f), 1f, (RotateMode)0);
	}

	private void SendOnOpen()
	{
		this.OnOpen.Send(this);
	}

	protected override void OnDisposed()
	{
		this.OnOpen = null;
		base.OnDisposed();
	}
}
