using System;
using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class GenericDoorController : TMGMonoBehaviour
{
	public enum DOOR_TYPE
	{
		METAL,
		GATE,
		SKULL_DOORS,
		HATCH,
		BIG_DOOR
	}

	[Serializable]
	public class DoorInfo
	{
		public bool IsOpen;

		public Transform Door;

		public Transform Open;

		public Transform Slide;

		public Vector3 InitialPosition { get; set; }

		public Vector3 InitialRotation { get; set; }
	}

	[Header("[Single Door Options]")]
	[SerializeField]
	private bool m_EnableSingleDoor = true;

	[SerializeField]
	private DoorInfo m_SingleDoor;

	[Header("[Double Door Options]")]
	[SerializeField]
	private bool m_EnableDoubleDoor;

	[SerializeField]
	private DoorInfo m_LeftDoor;

	[SerializeField]
	private DoorInfo m_RightDoor;

	[Header("Open Options")]
	[SerializeField]
	private float m_OpenDelay;

	[SerializeField]
	private float m_OpenSpeed = 2f;

	[SerializeField]
	private Ease m_OpenEase = (Ease)7;

	[Header("Close Options")]
	[SerializeField]
	private float m_CloseDelay;

	[SerializeField]
	private float m_CloseSpeed = 2f;

	[SerializeField]
	private Ease m_CloseEase = (Ease)7;

	[Header("Audio Options")]
	[SerializeField]
	private DOOR_TYPE m_DoorType;

	private Sequence m_Sequence;

	private OcclusionPortal m_OcclusionPortal;

	private AudioObject m_OpeningAudioObject;

	private AudioClip m_MechanicalOpenClip;

	private AudioClip m_GateOpenClip;

	private AudioClip m_GateCloseClip;

	private AudioClip m_BigDoorClip;

	public bool IsOpen { get; private set; }

	public event EventHandler OnOpen;

	public event EventHandler OnOpened;

	public event EventHandler OnClose;

	public event EventHandler OnClosed;

	public override void Init()
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		base.Init();
		m_EnableDoubleDoor = !m_EnableSingleDoor;
		m_EnableSingleDoor = !m_EnableDoubleDoor;
		if (m_EnableSingleDoor)
		{
			m_SingleDoor.InitialPosition = m_SingleDoor.Door.localPosition;
			m_SingleDoor.InitialRotation = m_SingleDoor.Door.localEulerAngles;
		}
		else
		{
			m_LeftDoor.InitialPosition = m_LeftDoor.Door.localPosition;
			m_LeftDoor.InitialRotation = m_LeftDoor.Door.localEulerAngles;
			m_RightDoor.InitialPosition = m_RightDoor.Door.localPosition;
			m_RightDoor.InitialRotation = m_RightDoor.Door.localEulerAngles;
		}
		m_OcclusionPortal = ((Component)this).GetComponentInChildren<OcclusionPortal>();
		IsOpen = false;
	}

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_MechanicalOpenClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_metaldoorsopening");
		m_GateOpenClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Gate_Open_Slow_01");
		m_GateCloseClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Gate_Close_01");
		m_BigDoorClip = GameManager.Instance.GetAudioClip("Audio/SFX/sfx_big_door_open");
	}

	public void ForceOpen()
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		KillSequence();
		if (Object.op_Implicit((Object)(object)m_OcclusionPortal))
		{
			m_OcclusionPortal.open = true;
		}
		if (m_EnableSingleDoor)
		{
			if (m_SingleDoor.IsOpen)
			{
				m_SingleDoor.Door.localEulerAngles = m_SingleDoor.Open.localEulerAngles;
			}
			else
			{
				m_SingleDoor.Door.localPosition = m_SingleDoor.Slide.localPosition;
			}
		}
		else if (m_EnableDoubleDoor)
		{
			if (m_LeftDoor.IsOpen)
			{
				m_LeftDoor.Door.localEulerAngles = m_LeftDoor.Open.localEulerAngles;
			}
			else
			{
				m_LeftDoor.Door.localPosition = m_LeftDoor.Slide.localPosition;
			}
			if (m_RightDoor.IsOpen)
			{
				m_RightDoor.Door.localEulerAngles = m_RightDoor.Open.localEulerAngles;
			}
			else
			{
				m_RightDoor.Door.localPosition = m_RightDoor.Slide.localPosition;
			}
		}
	}

	public void Open()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Expected O, but got Unknown
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		this.OnOpen.Send(this);
		ResetSequence();
		if (Object.op_Implicit((Object)(object)m_OcclusionPortal))
		{
			m_OcclusionPortal.open = true;
		}
		if (m_DoorType == DOOR_TYPE.METAL)
		{
			m_OpeningAudioObject = GameManager.Instance.AudioManager.PlayAtPosition(m_MechanicalOpenClip, base.transform.position);
		}
		else if (m_DoorType == DOOR_TYPE.GATE)
		{
			m_OpeningAudioObject = GameManager.Instance.AudioManager.PlayAtPosition(m_GateOpenClip, base.transform.position);
		}
		else if (m_DoorType == DOOR_TYPE.BIG_DOOR)
		{
			m_OpeningAudioObject = GameManager.Instance.AudioManager.PlayAtPosition(m_BigDoorClip, base.transform.position + Vector3.down * 6f);
		}
		if (m_EnableSingleDoor)
		{
			if (m_SingleDoor.IsOpen)
			{
				TweenSettingsExtensions.Insert(m_Sequence, m_OpenDelay, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_SingleDoor.Door, m_SingleDoor.Open.localEulerAngles, m_OpenSpeed, (RotateMode)0), m_OpenEase));
			}
			else
			{
				TweenSettingsExtensions.Insert(m_Sequence, m_OpenDelay, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_SingleDoor.Door, m_SingleDoor.Slide.localPosition, m_OpenSpeed, false), m_OpenEase));
			}
		}
		else if (m_EnableDoubleDoor)
		{
			if (m_LeftDoor.IsOpen)
			{
				TweenSettingsExtensions.Insert(m_Sequence, m_OpenDelay, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_LeftDoor.Door, m_LeftDoor.Open.localEulerAngles, m_OpenSpeed, (RotateMode)0), m_OpenEase));
			}
			else
			{
				TweenSettingsExtensions.Insert(m_Sequence, m_OpenDelay, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_LeftDoor.Door, m_LeftDoor.Slide.localPosition, m_OpenSpeed, false), m_OpenEase));
			}
			if (m_RightDoor.IsOpen)
			{
				TweenSettingsExtensions.Insert(m_Sequence, m_OpenDelay, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_RightDoor.Door, m_RightDoor.Open.localEulerAngles, m_OpenSpeed, (RotateMode)0), m_OpenEase));
			}
			else
			{
				TweenSettingsExtensions.Insert(m_Sequence, m_OpenDelay, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_RightDoor.Door, m_RightDoor.Slide.localPosition, m_OpenSpeed, false), m_OpenEase));
			}
		}
		TweenSettingsExtensions.OnComplete<Sequence>(m_Sequence, new TweenCallback(OpenOnComplete));
	}

	private void OpenOnComplete()
	{
		IsOpen = true;
		this.OnOpened.Send(this);
	}

	public void ForceClose()
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		KillSequence();
		if (Object.op_Implicit((Object)(object)m_OcclusionPortal))
		{
			m_OcclusionPortal.open = false;
		}
		if (m_EnableSingleDoor)
		{
			if (m_SingleDoor.IsOpen)
			{
				m_SingleDoor.Door.localEulerAngles = m_SingleDoor.InitialRotation;
			}
			else
			{
				m_SingleDoor.Door.localPosition = m_SingleDoor.InitialPosition;
			}
		}
		else if (m_EnableDoubleDoor)
		{
			if (m_LeftDoor.IsOpen)
			{
				m_LeftDoor.Door.localEulerAngles = m_LeftDoor.InitialRotation;
			}
			else
			{
				m_LeftDoor.Door.localPosition = m_LeftDoor.InitialPosition;
			}
			if (m_RightDoor.IsOpen)
			{
				m_RightDoor.Door.localEulerAngles = m_RightDoor.InitialRotation;
			}
			else
			{
				m_RightDoor.Door.localPosition = m_RightDoor.InitialPosition;
			}
		}
	}

	public void Close()
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Expected O, but got Unknown
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		this.OnClose.Send(this);
		ResetSequence();
		if ((Object)(object)m_OpeningAudioObject != (Object)null && m_OpeningAudioObject.AudioSource.isPlaying)
		{
			m_OpeningAudioObject.Stop();
			m_OpeningAudioObject = null;
		}
		if (m_DoorType == DOOR_TYPE.METAL || m_DoorType == DOOR_TYPE.BIG_DOOR)
		{
			GameManager.Instance.AudioManager.PlayAtPosition(m_GateCloseClip, base.transform.position);
		}
		if (m_EnableSingleDoor)
		{
			if (m_SingleDoor.IsOpen)
			{
				TweenSettingsExtensions.Insert(m_Sequence, m_CloseDelay, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_SingleDoor.Door, m_SingleDoor.InitialRotation, m_CloseSpeed, (RotateMode)0), m_CloseEase));
			}
			else
			{
				TweenSettingsExtensions.Insert(m_Sequence, m_CloseDelay, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_SingleDoor.Door, m_SingleDoor.InitialPosition, m_CloseSpeed, false), m_CloseEase));
			}
		}
		else if (m_EnableDoubleDoor)
		{
			if (m_LeftDoor.IsOpen)
			{
				TweenSettingsExtensions.Insert(m_Sequence, m_CloseDelay, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_LeftDoor.Door, m_LeftDoor.InitialRotation, m_CloseSpeed, (RotateMode)0), m_CloseEase));
			}
			else
			{
				TweenSettingsExtensions.Insert(m_Sequence, m_CloseDelay, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_LeftDoor.Door, m_LeftDoor.InitialPosition, m_CloseSpeed, false), m_CloseEase));
			}
			if (m_RightDoor.IsOpen)
			{
				TweenSettingsExtensions.Insert(m_Sequence, m_CloseDelay, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_RightDoor.Door, m_RightDoor.InitialRotation, m_CloseSpeed, (RotateMode)0), m_CloseEase));
			}
			else
			{
				TweenSettingsExtensions.Insert(m_Sequence, m_CloseDelay, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_RightDoor.Door, m_RightDoor.InitialPosition, m_CloseSpeed, false), m_CloseEase));
			}
		}
		TweenSettingsExtensions.OnComplete<Sequence>(m_Sequence, new TweenCallback(CloseOnComplete));
	}

	private void CloseOnComplete()
	{
		if (Object.op_Implicit((Object)(object)m_OcclusionPortal))
		{
			m_OcclusionPortal.open = false;
		}
		IsOpen = false;
		this.OnClosed.Send(this);
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
		this.OnClose = null;
		this.OnClosed = null;
		this.OnOpen = null;
		this.OnOpened = null;
		m_GateOpenClip = null;
		m_GateCloseClip = null;
		m_MechanicalOpenClip = null;
		m_BigDoorClip = null;
		base.OnDisposed();
	}
}
