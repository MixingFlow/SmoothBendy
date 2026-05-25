using System;
using System.Collections.Generic;
using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class CH3LiftContainer : TMGMonoBehaviour
{
	[Serializable]
	public class Gates
	{
		public Transform Gate_01;

		public Transform Gate_02;

		public Transform Gate_03;
	}

	[Header("Transforms")]
	[SerializeField]
	private Transform m_CarPosition;

	[Header("Interactables")]
	[SerializeField]
	private Interactable m_CallButton;

	[Header("Lights")]
	[SerializeField]
	private Light m_Light;

	[Header("Gates")]
	[SerializeField]
	private Gates m_LeftGate;

	[SerializeField]
	private Gates m_RightGate;

	[Header("Triggers")]
	[SerializeField]
	private List<EventTrigger> m_CloseEvents;

	private float m_LeftGateOriginPos_01;

	private float m_LeftGateOriginPos_02;

	private float m_LeftGateOriginPos_03;

	private float m_RightGateOriginPos_01;

	private float m_RightGateOriginPos_02;

	private float m_RightGateOriginPos_03;

	private float m_ScaleOrigin;

	private Vector3 m_DoorAudioPosition;

	private AudioClip[] m_ButtonClips;

	private Sequence m_DoorSequence;

	private AudioClip m_LiftDoorOpenClip;

	private AudioClip m_LiftDoorCloseClip;

	private AudioClip m_DingClip;

	public bool CanCall;

	public int ID { get; private set; }

	public bool IsClosed { get; private set; }

	public bool IsAnimating { get; private set; }

	public Vector3 CarPosition => m_CarPosition.position;

	public event EventHandler OnClosing;

	public event EventHandler OnClosed;

	public event EventHandler OnOpening;

	public event EventHandler OnOpened;

	public event EventHandler OnCalled;

	public override void Init()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		base.Init();
		m_LeftGateOriginPos_01 = m_LeftGate.Gate_01.localPosition.z;
		m_LeftGateOriginPos_02 = m_LeftGate.Gate_02.localPosition.z;
		m_LeftGateOriginPos_03 = m_LeftGate.Gate_03.localPosition.z;
		m_RightGateOriginPos_01 = m_RightGate.Gate_01.localPosition.x;
		m_RightGateOriginPos_02 = m_RightGate.Gate_02.localPosition.x;
		m_RightGateOriginPos_03 = m_RightGate.Gate_03.localPosition.x;
		m_ScaleOrigin = m_LeftGate.Gate_01.localScale.x;
	}

	public override void InitOnComplete()
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		base.InitOnComplete();
		m_LiftDoorOpenClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_elevatordooropen");
		m_LiftDoorCloseClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_elevatordoorclose");
		m_DingClip = GameManager.Instance.GetAudioClip("Audio/SFX/Lift/SFX_Lift_Ding");
		m_ButtonClips = GameManager.Instance.GetAudioClips("Audio/SFX/GenericButtons/");
		m_DoorAudioPosition = Vector3.Lerp(m_LeftGate.Gate_02.position, m_RightGate.Gate_02.position, 0.5f);
		IsClosed = true;
		((Behaviour)m_Light).enabled = false;
		m_CallButton.SetActive(active: false);
		for (int i = 0; i < m_CloseEvents.Count; i++)
		{
			m_CloseEvents[i].SetActive(active: false);
		}
	}

	public void SetID(int id)
	{
		ID = id;
	}

	private void HandleCallButtonOnInteracted(object sender, EventArgs e)
	{
		if (CanCall)
		{
			GameManager.Instance.AudioManager.Play(m_DingClip);
			PlayAudio(ref m_ButtonClips);
			((Behaviour)m_Light).enabled = true;
			this.OnCalled.Send(this);
		}
	}

	public void DisableCalling()
	{
		m_CallButton.OnInteracted -= HandleCallButtonOnInteracted;
		m_CallButton.SetActive(active: false);
	}

	public void EnableCalling()
	{
		m_CallButton.OnInteracted -= HandleCallButtonOnInteracted;
		m_CallButton.OnInteracted += HandleCallButtonOnInteracted;
		m_CallButton.SetActive(active: true);
	}

	public void Open(bool wasCalled = false, bool isInitialArrival = false)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (m_DoorSequence != null && TweenExtensions.IsActive((Tween)(object)m_DoorSequence) && TweenExtensions.IsPlaying((Tween)(object)m_DoorSequence))
		{
			DebugLog("[LIFT] - (" + ((Object)base.gameObject).name + ") Can't Open, door's are currently animating");
			return;
		}
		if (!isInitialArrival)
		{
			GameManager.Instance.AudioManager.PlayAtPosition(m_LiftDoorOpenClip, m_DoorAudioPosition);
		}
		float left_ = m_LeftGateOriginPos_01 + 1.5f;
		float left_2 = m_LeftGateOriginPos_02 + 5f;
		float left_3 = m_LeftGateOriginPos_03 + 9f;
		float right_ = m_RightGateOriginPos_01 - 1.5f;
		float right_2 = m_RightGateOriginPos_02 - 5f;
		float right_3 = m_RightGateOriginPos_03 - 9f;
		if (!wasCalled)
		{
			RemoveCloseListeners();
			AddCloseListeners();
		}
		this.OnOpening.Send(this);
		Animate(this.OnOpened, closedStatus: false, m_ScaleOrigin * 0.6f, left_, left_2, left_3, right_, right_2, right_3);
	}

	public void ForceOpen(bool wasCalled = false)
	{
		float left_ = m_LeftGateOriginPos_01 + 1.5f;
		float left_2 = m_LeftGateOriginPos_02 + 5f;
		float left_3 = m_LeftGateOriginPos_03 + 9f;
		float right_ = m_RightGateOriginPos_01 - 1.5f;
		float right_2 = m_RightGateOriginPos_02 - 5f;
		float right_3 = m_RightGateOriginPos_03 - 9f;
		if (!wasCalled)
		{
			RemoveCloseListeners();
			AddCloseListeners();
		}
		Animate(null, closedStatus: false, m_ScaleOrigin * 0.6f, left_, left_2, left_3, right_, right_2, right_3, 0f);
	}

	public void ForceClose()
	{
		RemoveCloseListeners();
		Animate(null, closedStatus: true, m_ScaleOrigin, m_LeftGateOriginPos_01, m_LeftGateOriginPos_02, m_LeftGateOriginPos_03, m_RightGateOriginPos_01, m_RightGateOriginPos_02, m_RightGateOriginPos_03, 0f);
	}

	public void Close(bool isInitialArrival = false)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (m_DoorSequence != null && TweenExtensions.IsActive((Tween)(object)m_DoorSequence) && TweenExtensions.IsPlaying((Tween)(object)m_DoorSequence))
		{
			DebugLog("[LIFT] - (" + ((Object)base.gameObject).name + ") Can't Close, door's are currently animating");
			return;
		}
		if (!isInitialArrival)
		{
			GameManager.Instance.AudioManager.PlayAtPosition(m_LiftDoorCloseClip, m_DoorAudioPosition);
		}
		RemoveCloseListeners();
		((Behaviour)m_Light).enabled = false;
		this.OnClosing.Send(this);
		Animate(this.OnClosed, closedStatus: true, m_ScaleOrigin, m_LeftGateOriginPos_01, m_LeftGateOriginPos_02, m_LeftGateOriginPos_03, m_RightGateOriginPos_01, m_RightGateOriginPos_02, m_RightGateOriginPos_03);
	}

	private void Animate(EventHandler _event, bool closedStatus, float _scale, float left_01, float left_02, float left_03, float right_01, float right_02, float right_03, float duration = 2.5f)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Expected O, but got Unknown
		IsAnimating = true;
		ResetDoorSequence();
		Ease ease = (Ease)7;
		TweenSettingsExtensions.Insert(m_DoorSequence, 0f, (Tween)(object)DOLocalMoveZ(m_LeftGate.Gate_01, left_01, duration, ease));
		TweenSettingsExtensions.Insert(m_DoorSequence, 0f, (Tween)(object)DOLocalMoveZ(m_LeftGate.Gate_02, left_02, duration, ease));
		TweenSettingsExtensions.Insert(m_DoorSequence, 0f, (Tween)(object)DOLocalMoveZ(m_LeftGate.Gate_03, left_03, duration, ease));
		TweenSettingsExtensions.Insert(m_DoorSequence, 0f, (Tween)(object)DOLocalMoveX(m_RightGate.Gate_01, right_01, duration, ease));
		TweenSettingsExtensions.Insert(m_DoorSequence, 0f, (Tween)(object)DOLocalMoveX(m_RightGate.Gate_02, right_02, duration, ease));
		TweenSettingsExtensions.Insert(m_DoorSequence, 0f, (Tween)(object)DOLocalMoveX(m_RightGate.Gate_03, right_03, duration, ease));
		TweenSettingsExtensions.Insert(m_DoorSequence, 0f, (Tween)(object)DOScaleX(m_LeftGate.Gate_01, _scale, duration, ease));
		TweenSettingsExtensions.Insert(m_DoorSequence, 0f, (Tween)(object)DOScaleX(m_LeftGate.Gate_02, _scale, duration, ease));
		TweenSettingsExtensions.Insert(m_DoorSequence, 0f, (Tween)(object)DOScaleX(m_LeftGate.Gate_03, _scale, duration, ease));
		TweenSettingsExtensions.Insert(m_DoorSequence, 0f, (Tween)(object)DOScaleX(m_RightGate.Gate_01, _scale, duration, ease));
		TweenSettingsExtensions.Insert(m_DoorSequence, 0f, (Tween)(object)DOScaleX(m_RightGate.Gate_02, _scale, duration, ease));
		TweenSettingsExtensions.Insert(m_DoorSequence, 0f, (Tween)(object)DOScaleX(m_RightGate.Gate_03, _scale, duration, ease));
		TweenSettingsExtensions.OnComplete<Sequence>(m_DoorSequence, (TweenCallback)delegate
		{
			IsAnimating = false;
			IsClosed = closedStatus;
			DebugLog("[LIFT] - (" + ((Object)base.gameObject).name + ") - [Animation Complete] IsClosed = " + IsClosed);
			if (IsClosed)
			{
				EnableCalling();
			}
			_event.Send(this);
		});
	}

	private void HandleCloseEventsOnEnter(object sender, EventArgs e)
	{
		Close();
	}

	private Tweener DOLocalMoveZ(Transform _trans, float endPosition, float duration, Ease ease)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveZ(_trans, endPosition, duration, false), ease);
	}

	private Tweener DOLocalMoveX(Transform _trans, float endPosition, float duration, Ease ease)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveX(_trans, endPosition, duration, false), ease);
	}

	private Tweener DOScaleX(Transform _trans, float endScale, float duration, Ease ease)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScaleX(_trans, endScale, duration), ease);
	}

	private AudioObject PlayAudio(ref AudioClip[] audioClips)
	{
		if (audioClips == null || audioClips.Length <= 0)
		{
			return null;
		}
		int num = Random.Range(0, audioClips.Length);
		AudioClip val = audioClips[num];
		AudioObject result = GameManager.Instance.AudioManager.Play(val);
		audioClips[num] = audioClips[0];
		audioClips[0] = val;
		return result;
	}

	private void AddCloseListeners()
	{
		for (int i = 0; i < m_CloseEvents.Count; i++)
		{
			EventTrigger eventTrigger = m_CloseEvents[i];
			eventTrigger.OnEnter -= HandleCloseEventsOnEnter;
			eventTrigger.OnEnter += HandleCloseEventsOnEnter;
			eventTrigger.SetActive(active: true);
		}
	}

	private void RemoveCloseListeners()
	{
		for (int i = 0; i < m_CloseEvents.Count; i++)
		{
			EventTrigger eventTrigger = m_CloseEvents[i];
			eventTrigger.OnEnter -= HandleCloseEventsOnEnter;
			eventTrigger.SetActive(active: false);
		}
	}

	private void ResetDoorSequence()
	{
		KillDoorSequence();
		m_DoorSequence = DOTween.Sequence();
	}

	private void KillDoorSequence()
	{
		if (m_DoorSequence != null)
		{
			TweenExtensions.Kill((Tween)(object)m_DoorSequence, false);
			m_DoorSequence = null;
		}
	}

	public void ClearAllEventHandlers()
	{
		this.OnOpened = null;
		this.OnClosed = null;
		this.OnCalled = null;
		this.OnClosing = null;
		this.OnOpening = null;
	}

	protected override void OnDisposed()
	{
		ClearAllEventHandlers();
		base.OnDisposed();
	}
}
