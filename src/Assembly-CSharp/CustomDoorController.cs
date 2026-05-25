using System;
using System.Collections.Generic;
using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class CustomDoorController : TMGMonoBehaviour
{
	[Header("Interactable")]
	[SerializeField]
	private Interactable m_DoorInteractable;

	[Header("Transforms")]
	[SerializeField]
	private Transform m_DoorHinge;

	[Header("Options")]
	[SerializeField]
	private bool m_IsLocked;

	[SerializeField]
	private bool m_IsSingleInteraction;

	[SerializeField]
	private Vector3 m_OpenRotation;

	[SerializeField]
	private List<AudioClip> m_LockAudioClips;

	private AudioObject m_LockAudioObject;

	private bool m_IsSilent;

	public float DoorSpeed = 1f;

	public bool IsLocked { get; private set; }

	public bool IsOpen { get; private set; }

	public event EventHandler OnInteract;

	public event EventHandler OnOpened;

	public event EventHandler OnClosed;

	public override void Init()
	{
		base.Init();
		IsLocked = m_IsLocked;
	}

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		if ((Object)(object)m_DoorInteractable != (Object)null)
		{
			m_DoorInteractable.OnInteracted += HandleDoorOnInteracted;
		}
	}

	private void HandleDoorOnInteracted(object sender, EventArgs e)
	{
		Open();
	}

	public void Lock()
	{
		IsLocked = true;
	}

	public void Unlock()
	{
		IsLocked = false;
	}

	public void Open()
	{
		Open(DoorSpeed, (Ease)6);
	}

	public void Open(float speed)
	{
		Open(speed, (Ease)6);
	}

	public void Open(float speed, float rotation)
	{
		Open(speed, (Ease)6, rotation);
	}

	public void Open(float speed, Ease ease, float rotation = 145f)
	{
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if (IsLocked)
		{
			if ((Object)(object)m_LockAudioObject == (Object)null)
			{
				m_LockAudioObject = PlayLockedAudio();
				if ((Object)(object)m_LockAudioObject != (Object)null)
				{
					m_LockAudioObject.OnComplete += HandleLockAudioOnComplete;
				}
			}
		}
		else
		{
			if (!m_IsSilent)
			{
				GameManager.Instance.AudioManager.PlayAtPosition("Audio/SFX/Door/SFX_Door_Generic_Open_01", base.transform.position);
			}
			IsOpen = true;
			this.OnInteract.Send(this);
			AnimateDoor(m_OpenRotation, speed, ease);
		}
	}

	private void HandleLockAudioOnComplete(object sender, EventArgs e)
	{
		m_LockAudioObject.OnComplete -= HandleLockAudioOnComplete;
		m_LockAudioObject.Clear();
		m_LockAudioObject = null;
	}

	public void ForceClose()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		IsOpen = false;
		m_DoorHinge.localEulerAngles = Vector3.zero;
	}

	public void Close()
	{
		Close(1f, (Ease)5);
	}

	public void Close(float speed)
	{
		Close(speed, (Ease)5);
	}

	public void Close(float speed, Ease ease)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		GameManager.Instance.AudioManager.PlayAtPosition("Audio/SFX/Door/SFX_Door_Generic_Close_01", base.transform.position);
		IsOpen = false;
		AnimateDoor(Vector3.zero, speed, ease);
	}

	private void AnimateDoor(Vector3 rotation, float speed, Ease ease)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		ShortcutExtensions.DOKill((Component)(object)m_DoorHinge, false);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_DoorHinge, rotation, speed, (RotateMode)0), ease), new TweenCallback(AnimateDoorOnComplete));
	}

	private void AnimateDoorOnComplete()
	{
		if (m_IsSingleInteraction)
		{
			IsLocked = true;
		}
		if (IsOpen)
		{
			this.OnOpened.Send(this);
		}
		else
		{
			this.OnClosed.Send(this);
		}
	}

	private AudioObject PlayLockedAudio()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (m_LockAudioClips.Count <= 0)
		{
			return null;
		}
		int index = Random.Range(0, m_LockAudioClips.Count);
		AudioClip val = m_LockAudioClips[index];
		AudioObject result = GameManager.Instance.AudioManager.PlayAtPosition(val, base.transform.position);
		m_LockAudioClips[index] = m_LockAudioClips[0];
		m_LockAudioClips[0] = val;
		return result;
	}

	public void SetSilent(bool active)
	{
		m_IsSilent = active;
	}

	protected override void OnDisposed()
	{
		if ((Object)(object)m_DoorInteractable != (Object)null)
		{
			m_DoorInteractable.OnInteracted -= HandleDoorOnInteracted;
		}
		ShortcutExtensions.DOKill((Component)(object)m_DoorHinge, false);
		base.OnDisposed();
	}
}
