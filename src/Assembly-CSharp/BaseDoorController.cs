using System;
using System.Collections.Generic;
using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class BaseDoorController : TMGMonoBehaviour
{
	[Header("Transforms")]
	[SerializeField]
	private Transform m_Door;

	[Header("Interactables")]
	[SerializeField]
	private Interactable m_DoorFront;

	[SerializeField]
	private Interactable m_DoorBack;

	[Header("Audio")]
	[SerializeField]
	private List<AudioClip> m_LockAudioClips;

	[Header("Door Options")]
	[SerializeField]
	public bool IsUnlockedAtStart = true;

	private OcclusionPortal m_Portal;

	private AudioClip m_DoorOpenClip;

	private AudioClip m_DoorCloseClip;

	private AudioObject m_LockAudioObject;

	public bool IsSilent { get; private set; }

	public bool IsLocked { get; private set; }

	public bool IsOpen { get; private set; }

	public event EventHandler OnOpen;

	public event EventHandler OnClose;

	public event EventHandler OnInteracted;

	public override void Init()
	{
		base.Init();
		IsLocked = !IsUnlockedAtStart;
		m_Portal = ((Component)this).GetComponent<OcclusionPortal>();
	}

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_DoorOpenClip = GameManager.Instance.GetAudioClip("Audio/SFX/Door/SFX_Door_Generic_Open_01");
		m_DoorCloseClip = GameManager.Instance.GetAudioClip("Audio/SFX/Door/SFX_Door_Generic_Close_01");
		AddListeners();
	}

	private void InternalOpen(float speed, Ease ease, float rotation)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (!IsSilent && !IsOpen)
		{
			GameManager.Instance.AudioManager.PlayAtPosition(m_DoorOpenClip, base.transform.position);
		}
		IsOpen = true;
		if (Object.op_Implicit((Object)(object)m_Portal))
		{
			m_Portal.open = true;
		}
		TweenSettingsExtensions.OnComplete<Tweener>(AnimateDoor(new Vector3(0f, rotation, 0f), 1f, (Ease)6), new TweenCallback(SendOpenOnComplete));
	}

	public void ForceOpen(float rotation)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)m_Portal))
		{
			m_Portal.open = true;
		}
		m_Door.localEulerAngles = new Vector3(0f, rotation, 0f);
		IsOpen = true;
		IsSilent = true;
	}

	public void Open(float speed, Ease ease, float rotation)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (!CheckLocked())
		{
			if (Object.op_Implicit((Object)(object)m_Portal))
			{
				m_Portal.open = true;
			}
			InternalOpen(speed, ease, rotation);
		}
	}

	public void ForceClose()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		m_Door.localEulerAngles = Vector3.zero;
		if (Object.op_Implicit((Object)(object)m_Portal))
		{
			m_Portal.open = false;
		}
		IsOpen = false;
		IsSilent = false;
	}

	public void Close()
	{
		Close(1f, (Ease)5);
	}

	public void Close(float speed, Ease ease)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (!IsSilent && IsOpen)
		{
			GameManager.Instance.AudioManager.PlayAtPosition(m_DoorCloseClip, base.transform.position);
		}
		IsOpen = false;
		TweenSettingsExtensions.OnComplete<Tweener>(AnimateDoor(Vector3.zero, speed, ease), new TweenCallback(SendCloseOnComplete));
	}

	private Tweener AnimateDoor(Vector3 rotation, float speed, Ease ease)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		ShortcutExtensions.DOKill((Component)(object)m_Door, false);
		return TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_Door, rotation, speed, (RotateMode)0), ease);
	}

	private AudioObject PlayLockedAudio()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (m_LockAudioClips.Count <= 0 || IsOpen)
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

	private void HandleDoorBackOnInteracted(object sender, EventArgs e)
	{
		if (!CheckLocked())
		{
			this.OnInteracted.Send(this);
			Lock();
			InternalOpen(1f, (Ease)6, 145f);
		}
	}

	private void HandleDoorFrontOnInteracted(object sender, EventArgs e)
	{
		if (!CheckLocked())
		{
			this.OnInteracted.Send(this);
			Lock();
			InternalOpen(1f, (Ease)6, -145f);
		}
	}

	public bool CheckLocked()
	{
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
			return true;
		}
		return false;
	}

	public void Lock()
	{
		IsLocked = true;
	}

	public void Unlock()
	{
		IsLocked = false;
	}

	public void UnlockAndActivate()
	{
		Unlock();
	}

	private void HandleLockAudioOnComplete(object sender, EventArgs e)
	{
		m_LockAudioObject.OnComplete -= HandleLockAudioOnComplete;
		m_LockAudioObject.Clear();
		m_LockAudioObject = null;
	}

	private void AddListeners()
	{
		m_DoorFront.OnInteracted += HandleDoorFrontOnInteracted;
		m_DoorFront.SetActive(active: true);
		m_DoorBack.OnInteracted += HandleDoorBackOnInteracted;
		m_DoorBack.SetActive(active: true);
	}

	private void RemoveListeners()
	{
		m_DoorBack.OnInteracted -= HandleDoorBackOnInteracted;
		m_DoorBack.SetActive(active: false);
		m_DoorFront.OnInteracted -= HandleDoorFrontOnInteracted;
		m_DoorFront.SetActive(active: false);
	}

	private void SendCloseOnComplete()
	{
		if (Object.op_Implicit((Object)(object)m_Portal))
		{
			m_Portal.open = true;
		}
		this.OnClose.Send(this);
	}

	private void SendOpenOnComplete()
	{
		this.OnOpen.Send(this);
	}

	protected override void OnDisposed()
	{
		RemoveListeners();
		ShortcutExtensions.DOKill((Component)(object)m_Door, false);
		this.OnClose = null;
		this.OnOpen = null;
		this.OnInteracted = null;
		m_Portal = null;
		m_DoorOpenClip = null;
		m_DoorCloseClip = null;
		m_LockAudioObject = null;
		base.OnDisposed();
	}
}
