using System;
using System.Collections.Generic;
using DG.Tweening;
using TMG.Core;
using UnityEngine;

public class CH3LiftController : TMGMonoBehaviour
{
	private const float LIFT_SPEED_ORIGIN = 12f;

	[Header("EventTriggers")]
	[SerializeField]
	private EventTrigger m_ActivateButtonTrigger;

	[Header("GameObjects")]
	[SerializeField]
	private GameObject m_ExteriorCollision;

	[Header("Transforms")]
	[SerializeField]
	private Transform m_Lift;

	[SerializeField]
	private Transform m_Cables;

	[SerializeField]
	private Transform m_LiftAudio;

	[Header("Buttons")]
	[SerializeField]
	private List<CH3LiftButton> m_Buttons;

	[SerializeField]
	private Interactable m_ButtonOpen;

	[Header("Lift Containers")]
	[SerializeField]
	private CH3LiftContainer m_AliceFloor;

	[Header("Animations")]
	[SerializeField]
	private AnimationCurve m_AnimationCure;

	[Header("Boris")]
	[SerializeField]
	private Transform m_BorisIdlePosition;

	private CH3LiftContainer m_CurrentFloor;

	private Sequence m_LiftSequence;

	private AudioClip[] m_AliceLiftEntranceClips;

	private AudioClip[] m_AliceLiftExitClips;

	private AudioClip[] m_ButtonClips;

	private AudioClip m_LiftDepartClip;

	private AudioClip m_LiftLoop;

	private AudioClip m_LiftAriveClip;

	private float m_LiftSpeed;

	private float m_LiftSpeedMultiplier;

	private bool m_WasCalled;

	private bool m_IsInitialArrival;

	private bool m_CanGoToBasement;

	private bool m_HasTimer;

	private float m_Timer;

	private float m_TimerLimit = 7f;

	private AudioObject m_LiftMoveAudio;

	[HideInInspector]
	public bool HasDialogie;

	private BorisAi m_Boris => GameManager.Instance.CharacterManager.Boris;

	public bool IsInCart { get; private set; }

	public CH3LiftContainer CurrentFloor => m_CurrentFloor;

	public Transform Lift => m_Lift;

	public event EventHandler OnAliceDoorClosed;

	public override void InitOnComplete()
	{
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		base.InitOnComplete();
		m_AliceLiftEntranceClips = GameManager.Instance.GetAudioClips("Audio/DIA/CH3/Alice/MonologueLift/");
		m_AliceLiftExitClips = GameManager.Instance.GetAudioClips("Audio/DIA/CH3/Alice/MonologueLiftExit/");
		m_ButtonClips = GameManager.Instance.GetAudioClips("Audio/SFX/GenericButtons/");
		m_LiftDepartClip = GameManager.Instance.GetAudioClip("Audio/SFX/Lift/SFX_Lift_Depart");
		m_LiftLoop = GameManager.Instance.GetAudioClip("Audio/SFX/Lift/SFX_Lift_Loop");
		m_LiftAriveClip = GameManager.Instance.GetAudioClip("Audio/SFX/Lift/SFX_Lift_Arrive");
		for (int i = 0; i < m_Buttons.Count; i++)
		{
			CH3LiftContainer floor = m_Buttons[i].Floor;
			floor.SetID(i);
			floor.OnCalled += HandleLiftOnCalled;
		}
		m_ActivateButtonTrigger.SetActive(active: true);
		m_ActivateButtonTrigger.OnEnter += HandleLiftOnEnter;
		m_ActivateButtonTrigger.OnExit += HandleLiftOnExit;
		m_LiftSpeed = 12f;
		m_CurrentFloor = m_Buttons[1].Floor;
		m_Lift.position = m_CurrentFloor.CarPosition;
		m_ExteriorCollision.SetActive(false);
		m_ButtonOpen.SetActive(active: false);
		HasDialogie = false;
	}

	private void HandleLiftOnExit(object sender, EventArgs e)
	{
		IsInCart = false;
	}

	private void HandleLiftOnEnter(object sender, EventArgs e)
	{
		IsInCart = true;
	}

	private void Update()
	{
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		if (GameManager.Instance.isPaused || !Object.op_Implicit((Object)(object)GameManager.Instance.Player))
		{
			return;
		}
		if (m_HasTimer && (Object)(object)m_CurrentFloor != (Object)null && !m_CurrentFloor.IsClosed)
		{
			m_Timer += Time.deltaTime;
			if (m_Timer > m_TimerLimit && !IsInCart)
			{
				m_HasTimer = false;
				m_Timer = 0f;
				m_ExteriorCollision.SetActive(true);
				m_CurrentFloor.OnClosed += HandleLiftGateAutoOnClosed;
				for (int i = 0; i < m_Buttons.Count; i++)
				{
					m_Buttons[i].Floor.Close();
				}
			}
		}
		if ((Object)(object)m_LiftMoveAudio != (Object)null && m_LiftMoveAudio.AudioSource.isPlaying)
		{
			Vector3 val = GameManager.Instance.Player.transform.position + GameManager.Instance.Player.transform.forward * 2f + Vector3.down * 2f;
			Vector3 val2 = val - m_Lift.position;
			m_LiftAudio.position = m_Lift.position + Vector3.ClampMagnitude(val2, 8f);
		}
	}

	public void InitialArrival()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		m_CurrentFloor = m_Buttons[0].Floor;
		m_WasCalled = true;
		m_IsInitialArrival = true;
		TweenSettingsExtensions.OnComplete<Sequence>(DOLiftMove(m_CurrentFloor), new TweenCallback(HandleMoveOnComplete));
	}

	public void GoToAlice(float speedOffset = 0f, bool hasDialogue = true)
	{
		RemoveListeners();
		DisableLift();
		m_IsInitialArrival = true;
		HasDialogie = hasDialogue;
		m_LiftSpeedMultiplier = 33f - speedOffset;
		m_CurrentFloor.OnClosed += HandleLiftGateOnClosed;
		m_CurrentFloor = m_AliceFloor;
		if (HasDialogie)
		{
			for (int i = 0; i < m_AliceLiftEntranceClips.Length; i++)
			{
				AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_AliceLiftEntranceClips[i], SubtitleConstants.DIALOGUE_CH3_ALICE_LIFT_01[i], isTrimmed: true));
				if (i == m_AliceLiftEntranceClips.Length - 1)
				{
					audioObject.OnComplete += delegate
					{
						GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_16", "OBJECTIVES/CH3_OBJECTIVE_16_TIP", 4f));
					};
				}
			}
		}
		if (HasDialogie)
		{
			EnterLift();
		}
		for (int num = 0; num < m_Buttons.Count; num++)
		{
			m_Buttons[num].Floor.Close();
		}
	}

	public void ForceCloseLift()
	{
		for (int i = 0; i < m_Buttons.Count; i++)
		{
			m_Buttons[i].Floor.ForceClose();
		}
	}

	public void GoToFloor(int floor, bool isInitialArrival = false, bool wasCalled = false)
	{
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Expected O, but got Unknown
		RemoveListeners();
		DisableLift();
		m_WasCalled = wasCalled;
		m_IsInitialArrival = isInitialArrival;
		m_LiftSpeedMultiplier = 0f - m_LiftSpeed;
		m_CurrentFloor = m_Buttons[floor].Floor;
		for (int i = 0; i < m_Buttons.Count; i++)
		{
			bool isInitialArrival2 = (Object)(object)m_Buttons[i].Floor == (Object)(object)m_CurrentFloor;
			m_Buttons[i].Floor.Close(isInitialArrival2);
		}
		TweenSettingsExtensions.OnComplete<Sequence>(DOLiftMove(m_CurrentFloor), new TweenCallback(HandleGoToFloorOnComplete));
	}

	private void HandleGoToFloorOnComplete()
	{
		DebugLog("[LIFT] - GoToFloorOnComplete");
		for (int i = 0; i < m_Buttons.Count; i++)
		{
			m_Buttons[i].Floor.CanCall = true;
		}
		m_LiftMoveAudio.Clear();
		m_ExteriorCollision.SetActive(false);
		m_CurrentFloor.OnClosing += HandleLiftGateOnClosing;
		m_CurrentFloor.OnOpened += HandleLiftGateOnOpened;
		m_CurrentFloor.ForceOpen(m_WasCalled);
	}

	public void GoToRandomFloor()
	{
		DebugLog("[LIFT] - Going to random floor!");
		CH3LiftContainer floor = m_Buttons[Random.Range(0, m_Buttons.Count - 1)].Floor;
		if (!((Object)(object)floor == (Object)(object)m_CurrentFloor))
		{
			RemoveListeners();
			m_LiftSpeedMultiplier = 0f;
			m_CurrentFloor.OnClosed += HandleLiftGateOnClosed;
			m_CurrentFloor = floor;
			m_WasCalled = true;
			for (int i = 0; i < m_Buttons.Count; i++)
			{
				m_Buttons[i].Floor.Close();
			}
		}
	}

	public void EnableBoris()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		m_Boris.StopWaypointPathing();
		m_Boris.transform.position = m_BorisIdlePosition.position;
		m_Boris.transform.eulerAngles = m_BorisIdlePosition.eulerAngles;
		m_Boris.transform.SetParent(m_Lift);
		m_Boris.LookAtPlayer();
	}

	public void CloseLift()
	{
		DebugLog("[LIFT] - CloseLift - Collision Enabled");
		m_ExteriorCollision.SetActive(true);
		for (int i = 0; i < m_Buttons.Count; i++)
		{
			m_Buttons[i].Floor.Close();
		}
	}

	private void HandleLiftGateAutoOnClosed(object sender, EventArgs e)
	{
		DebugLog("[LIFT] - Auto Closing");
		(sender as CH3LiftContainer).OnClosed -= HandleLiftGateAutoOnClosed;
		if (IsInCart)
		{
			RemoveListeners();
			AddListeners();
		}
		EnableLift();
	}

	private void HandleLiftOnCalled(object sender, EventArgs e)
	{
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Expected O, but got Unknown
		CH3LiftContainer cH3LiftContainer = (CH3LiftContainer)sender;
		DebugLog("[LIFT] - Calling Lift From Floor: " + ((Object)cH3LiftContainer.gameObject).name);
		m_WasCalled = true;
		m_LiftSpeedMultiplier = 8f;
		DisableLift();
		if (cH3LiftContainer.ID == m_CurrentFloor.ID)
		{
			m_HasTimer = true;
			m_Timer = 0f;
			m_ExteriorCollision.SetActive(false);
			cH3LiftContainer.OnOpened += HandleLiftGateOnOpened;
			for (int i = 0; i < m_Buttons.Count; i++)
			{
				if (!((object)m_Buttons[i].Floor).Equals((object)cH3LiftContainer))
				{
					m_Buttons[i].Floor.Close();
				}
			}
			cH3LiftContainer.Open(m_WasCalled);
		}
		else
		{
			m_CurrentFloor = cH3LiftContainer;
			for (int j = 0; j < m_Buttons.Count; j++)
			{
				m_Buttons[j].Floor.Close();
			}
			TweenSettingsExtensions.OnComplete<Sequence>(DOLiftMove(cH3LiftContainer), new TweenCallback(HandleMoveOnComplete));
		}
	}

	private void HandleButtonOnInteracted(object sender, EventArgs e)
	{
		if (m_CurrentFloor.IsAnimating)
		{
			DebugLog("[LIFT] - (" + ((Object)m_CurrentFloor.gameObject).name + ") - Wait for doors to stop animating, then press a floor");
			return;
		}
		CH3LiftButton cH3LiftButton = (CH3LiftButton)sender;
		DebugLog("[LIFT] - Floor (" + ((Object)cH3LiftButton.gameObject).name + ") Pressed.");
		PlayAudio(ref m_ButtonClips);
		RemoveListeners();
		m_LiftSpeedMultiplier = 0f;
		m_CurrentFloor.OnClosed += HandleLiftGateOnClosed;
		m_CurrentFloor = cH3LiftButton.Floor;
		m_WasCalled = false;
		EnterLift();
		for (int i = 0; i < m_Buttons.Count; i++)
		{
			m_Buttons[i].Floor.Close();
		}
	}

	private void HandleLiftGateOnClosed(object sender, EventArgs e)
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected O, but got Unknown
		DebugLog("[LIFT] - Floor (" + ((Object)(sender as CH3LiftContainer).gameObject).name + ") Doors Closing");
		for (int i = 0; i < m_Buttons.Count; i++)
		{
			m_Buttons[i].Floor.OnClosed -= HandleLiftGateOnClosed;
			if (!m_IsInitialArrival)
			{
				m_Buttons[i].Floor.EnableCalling();
			}
		}
		TweenSettingsExtensions.OnComplete<Sequence>(DOLiftMove(m_CurrentFloor), new TweenCallback(HandleMoveOnComplete));
	}

	private void EnterLift()
	{
		DebugLog("[LIFT] - EnterLift - Collision Enabled");
		m_ExteriorCollision.SetActive(true);
		GameManager.Instance.Player.transform.SetParent(m_Lift);
	}

	private Sequence DOLiftMove(CH3LiftContainer floor)
	{
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		DebugLog("[LIFT] - Moving Lift To Floor: " + ((Object)floor.gameObject).name);
		for (int i = 0; i < m_Buttons.Count; i++)
		{
			m_Buttons[i].Floor.CanCall = false;
		}
		float num = (float)floor.ID * 0.75f;
		if (num < 1f)
		{
			num = 1f;
		}
		m_LiftMoveAudio = GameManager.Instance.AudioManager.PlayAtPosition(m_LiftDepartClip, m_LiftAudio.position);
		m_LiftMoveAudio.OnComplete += delegate
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			m_LiftMoveAudio.Clear();
			m_LiftMoveAudio = GameManager.Instance.AudioManager.PlayAtPosition(m_LiftLoop, m_LiftAudio.position, AudioObjectType.SOUND_EFFECT, -1, isQueued: false, m_LiftAudio);
		};
		ResetSequence();
		TweenSettingsExtensions.Insert(m_LiftSequence, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScaleY(m_Cables, num, m_LiftSpeed + m_LiftSpeedMultiplier), m_AnimationCure));
		TweenSettingsExtensions.Insert(m_LiftSequence, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_Lift, floor.CarPosition, m_LiftSpeed + m_LiftSpeedMultiplier, false), m_AnimationCure));
		return m_LiftSequence;
	}

	private void HandleMoveOnComplete()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		DebugLog("[LIFT] - Lift Arrived");
		for (int i = 0; i < m_Buttons.Count; i++)
		{
			m_Buttons[i].Floor.CanCall = true;
		}
		m_LiftMoveAudio.Clear();
		m_LiftMoveAudio = GameManager.Instance.AudioManager.PlayAtPosition(m_LiftAriveClip, m_LiftAudio.position);
		m_ExteriorCollision.SetActive(false);
		if ((Object)(object)GameManager.Instance.Player.transform.parent != (Object)null)
		{
			GameManager.Instance.Player.transform.SetParent((Transform)null);
		}
		m_CurrentFloor.OnClosing += HandleLiftGateOnClosing;
		m_CurrentFloor.OnOpened += HandleLiftGateOnOpened;
		if (HasDialogie)
		{
			HasDialogie = false;
			m_WasCalled = false;
			for (int j = 0; j < m_AliceLiftExitClips.Length; j++)
			{
				GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_AliceLiftExitClips[j], SubtitleConstants.DIALOGUE_CH3_ALICE_LIFT_02[j], isTrimmed: true));
			}
			m_CurrentFloor.Open(wasCalled: true);
		}
		else
		{
			m_CurrentFloor.Open(m_WasCalled, m_IsInitialArrival);
		}
	}

	private void HandleLiftGateOnOpened(object sender, EventArgs e)
	{
		if (m_IsInitialArrival)
		{
			m_IsInitialArrival = false;
			return;
		}
		RemoveListeners();
		AddListeners();
		EnableLift();
		(sender as CH3LiftContainer).DisableCalling();
	}

	private void HandleLiftGateOnClosing(object sender, EventArgs e)
	{
		DebugLog("[LIFT] - Closing Door on Floor: " + ((Object)(sender as CH3LiftContainer).gameObject).name);
		(sender as CH3LiftContainer).OnClosing -= HandleLiftGateOnClosing;
		m_ExteriorCollision.SetActive(true);
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

	public void EnableLift()
	{
		for (int i = 0; i < m_Buttons.Count; i++)
		{
			m_Buttons[i].Floor.DisableCalling();
			m_Buttons[i].Floor.EnableCalling();
		}
	}

	public void DisableLift()
	{
		DebugLog("[LIFT] - Disable All Calling");
		for (int i = 0; i < m_Buttons.Count; i++)
		{
			m_Buttons[i].Floor.DisableCalling();
		}
	}

	public void ActivateFinale()
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected O, but got Unknown
		RemoveListeners();
		DisableLift();
		m_WasCalled = true;
		m_IsInitialArrival = true;
		m_HasTimer = false;
		if ((Object)(object)m_CurrentFloor == (Object)(object)m_AliceFloor)
		{
			HandleGoToAliceFinaleFloorOnComplete();
			return;
		}
		m_CurrentFloor = m_AliceFloor;
		m_LiftSpeedMultiplier = 8f;
		for (int i = 0; i < m_Buttons.Count; i++)
		{
			m_Buttons[i].Floor.Close();
		}
		TweenSettingsExtensions.OnComplete<Sequence>(DOLiftMove(m_CurrentFloor), new TweenCallback(HandleGoToAliceFinaleFloorOnComplete));
	}

	private void HandleGoToAliceFinaleFloorOnComplete()
	{
		for (int i = 0; i < m_Buttons.Count; i++)
		{
			CH3LiftContainer floor = m_Buttons[i].Floor;
			floor.ClearAllEventHandlers();
			floor.DisableCalling();
			floor.CanCall = false;
		}
		m_ExteriorCollision.SetActive(false);
		m_CurrentFloor.Open(m_WasCalled);
		DisableLift();
		RemoveListeners();
	}

	public void FinaleCloseAliceFloor()
	{
		m_AliceFloor.OnClosed += HandleFinalAliceDoorOnClosed;
		DisableLift();
		RemoveListeners();
		m_AliceFloor.Close();
		m_HasTimer = false;
	}

	private void HandleFinalAliceDoorOnClosed(object sender, EventArgs e)
	{
		m_AliceFloor.OnClosed -= HandleFinalAliceDoorOnClosed;
		DisableLift();
		RemoveListeners();
		m_HasTimer = false;
		this.OnAliceDoorClosed.Send(this);
	}

	public void UnlockBasement()
	{
		m_CanGoToBasement = true;
		RemoveListeners();
		AddListeners();
		EnableLift();
	}

	private void ResetSequence()
	{
		KillSequence();
		m_LiftSequence = DOTween.Sequence();
	}

	private void KillSequence()
	{
		if (m_LiftSequence != null)
		{
			TweenExtensions.Kill((Tween)(object)m_LiftSequence, false);
			m_LiftSequence = null;
		}
	}

	private void HandleButtonOpenOnInteracted(object sender, EventArgs e)
	{
		DebugLog("[LIFT BUTTON] - Door Button Pressed");
		PlayAudio(ref m_ButtonClips);
		if (m_CurrentFloor.IsAnimating)
		{
			DebugLog("[LIFT BUTTON] - Canceled, door is currently animating.");
		}
		else if (m_CurrentFloor.IsClosed)
		{
			m_ExteriorCollision.SetActive(false);
			m_CurrentFloor.Open();
		}
		else
		{
			m_ExteriorCollision.SetActive(true);
			m_CurrentFloor.Close();
		}
	}

	private void AddListeners()
	{
		DebugLog("[LIFT] - Enable All Buttons");
		for (int i = 0; i < m_Buttons.Count; i++)
		{
			CH3LiftButton cH3LiftButton = m_Buttons[i];
			if (m_CurrentFloor.ID == i)
			{
				DebugLog("[LIFT] - (" + ((Object)m_CurrentFloor.gameObject).name + ") - Disabling Current Floor");
				cH3LiftButton.Disable();
				cH3LiftButton.OnPressed -= HandleButtonOnInteracted;
			}
			else if (i == m_Buttons.Count - 1 && !m_CanGoToBasement)
			{
				DebugLog("[LIFT] - (" + ((Object)m_CurrentFloor.gameObject).name + ") - Disabling Bottom Floor (m_CanGoToBasement =" + m_CanGoToBasement + ")");
				cH3LiftButton.Disable();
				cH3LiftButton.OnPressed -= HandleButtonOnInteracted;
			}
			else
			{
				cH3LiftButton.Enable();
				cH3LiftButton.OnPressed += HandleButtonOnInteracted;
			}
		}
		m_ButtonOpen.SetActive(active: true);
		m_ButtonOpen.OnInteracted += HandleButtonOpenOnInteracted;
	}

	private void RemoveListeners()
	{
		DebugLog("[LIFT] - Disaable All Buttons");
		for (int i = 0; i < m_Buttons.Count; i++)
		{
			CH3LiftButton cH3LiftButton = m_Buttons[i];
			cH3LiftButton.Disable();
			cH3LiftButton.OnPressed -= HandleButtonOnInteracted;
		}
		m_ButtonOpen.SetActive(active: false);
		m_ButtonOpen.OnInteracted -= HandleButtonOpenOnInteracted;
	}

	protected override void OnDisposed()
	{
		RemoveListeners();
		m_ActivateButtonTrigger.OnEnter -= HandleLiftOnEnter;
		m_ActivateButtonTrigger.OnExit -= HandleLiftOnExit;
		if (m_LiftSequence != null)
		{
			TweenExtensions.Kill((Tween)(object)m_LiftSequence, false);
			m_LiftSequence = null;
		}
		m_LiftMoveAudio = null;
		m_AliceLiftEntranceClips = null;
		m_AliceLiftExitClips = null;
		m_ButtonClips = null;
		m_LiftDepartClip = null;
		m_LiftLoop = null;
		m_LiftAriveClip = null;
		base.OnDisposed();
	}
}
