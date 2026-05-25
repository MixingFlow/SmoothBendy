using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DoorController : Interactable
{
	[Header("Transforms")]
	[SerializeField]
	private Transform m_DoorHinge;

	[Header("Collider")]
	[SerializeField]
	private Collider m_DoorCollider;

	[Header("Options")]
	[SerializeField]
	private bool m_IsPush;

	[SerializeField]
	private bool m_IsLocked;

	[SerializeField]
	private List<AudioClip> m_LockAudioClips;

	private AudioObject m_LockAudioObject;

	public bool IsSilent { get; private set; }

	public bool IsLocked { get; private set; }

	public bool IsOpen { get; private set; }

	public event EventHandler OnOpened;

	public event EventHandler OnClosed;

	public override void Init()
	{
		base.Init();
		IsLocked = m_IsLocked;
	}

	public override void OnInteract()
	{
		if (!IsOpen)
		{
			Open();
		}
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
		Open(1f, (Ease)6, null);
	}

	public void Open(float speed)
	{
		Open(speed, (Ease)6, null);
	}

	public void Open(float speed, float rotation)
	{
		Open(speed, (Ease)6, null, rotation);
	}

	public void Open(float speed, Ease ease, Action onComplete, float rotation = 145f)
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		if (IsLocked)
		{
			if ((Object)(object)m_LockAudioObject == (Object)null)
			{
				m_LockAudioObject = PlayLockedAudio();
				m_LockAudioObject.OnComplete += HandleLockAudioOnComplete;
			}
			return;
		}
		if (!IsSilent)
		{
			GameManager.Instance.AudioManager.PlayAtPosition("Audio/SFX/Door/SFX_Door_Generic_Open_01", base.transform.position);
		}
		IsOpen = true;
		rotation *= (float)((!m_IsPush) ? 1 : (-1));
		AnimateDoor(new Vector3(0f, rotation, 0f), speed, ease, onComplete);
		this.OnOpened.Send(this);
	}

	private void HandleLockAudioOnComplete(object sender, EventArgs e)
	{
		m_LockAudioObject.OnComplete -= HandleLockAudioOnComplete;
		m_LockAudioObject.Clear();
		m_LockAudioObject = null;
	}

	public void Close()
	{
		Close(1f, (Ease)5, null);
	}

	public void Close(float speed)
	{
		Close(speed, (Ease)5, null);
	}

	public void Close(float speed, Ease ease, Action onComplete)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (!IsSilent)
		{
			GameManager.Instance.AudioManager.PlayAtPosition("Audio/SFX/Door/SFX_Door_Generic_Close_01", base.transform.position);
		}
		IsOpen = false;
		AnimateDoor(Vector3.zero, speed, ease, onComplete);
		this.OnClosed.Send(this);
	}

	public void SetSilent(bool isSilent)
	{
		IsSilent = isSilent;
	}

	private void AnimateDoor(Vector3 rotation, float speed, Ease ease, Action onComplete)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		m_DoorCollider.enabled = false;
		ShortcutExtensions.DOKill((Component)(object)m_DoorHinge, false);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_DoorHinge, rotation, speed, (RotateMode)0), ease), (TweenCallback)delegate
		{
			m_DoorCollider.enabled = true;
			if (onComplete != null)
			{
				onComplete();
			}
		});
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

	protected override void OnDisposed()
	{
		ShortcutExtensions.DOKill((Component)(object)m_DoorHinge, false);
		base.OnDisposed();
	}
}
