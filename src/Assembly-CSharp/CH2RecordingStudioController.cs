using System;
using System.Collections.Generic;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class CH2RecordingStudioController : BaseController
{
	private class Instrument
	{
		public InstrumentType InstrumentType;

		public int AvailableMax;
	}

	public class Cutout
	{
		public Transform Trans;

		public Vector3 StagePosition;

		public Quaternion StageRotation;

		public Vector3 BalconyPosition;

		public Quaternion BalconyRotation;
	}

	[Header("<Controllers>")]
	[SerializeField]
	private CH2AudioLogsController m_AudioLogController;

	[Header("Wally Closet Door")]
	[SerializeField]
	private BaseDoorController m_ClosetDoor;

	[Header("Transforms")]
	[SerializeField]
	private GenericDoorController m_Gate;

	[SerializeField]
	private Transform m_PianoLid;

	[Header("Projector")]
	[SerializeField]
	private ProjectorController m_ProjectorController;

	[SerializeField]
	private Interactable m_ProjectorInteract;

	[Header("Light Fixture")]
	[SerializeField]
	private LightFixtureController m_LightController;

	[SerializeField]
	private GameObject m_Lights;

	[Header("Instruments")]
	[SerializeField]
	private List<InteractableMusicalInstrument> m_Instruments;

	[Header("Event Triggers")]
	[SerializeField]
	private EventTrigger m_PianoJumpscare;

	[SerializeField]
	private EventTrigger m_PuzzleTip;

	[Header("Strike Up The Band")]
	[SerializeField]
	private EventTrigger m_LowerTrigger;

	[SerializeField]
	private EventTrigger m_UpperTrigger;

	[SerializeField]
	private List<Transform> m_StagePositions;

	[SerializeField]
	private List<Transform> m_BalconyPositions;

	[SerializeField]
	private List<Transform> m_BendyCutouts;

	[Header("DEV CHEATS")]
	[SerializeField]
	private Transform m_CheatPoint;

	private List<Instrument> m_AvailableInstruments;

	private List<int> m_InstrumentsPlayed = new List<int>();

	private List<Cutout> m_ActiveCutouts = new List<Cutout>();

	private AudioClip m_PianoJumpscareClip;

	private AudioClip m_ProjectorRunningWildClip;

	private AudioClip m_ProjectorClip;

	private AudioClip m_HenryClip10;

	private AudioObject m_ProjectorRunningWildObject;

	private float m_Timer;

	private float m_TimerMax = 45f;

	private float m_SuccessTimer;

	private float m_SuccessTimerMax = 1f;

	private int m_StrikeUpTheBandCount = 1;

	private int m_StrikeUpTheBandMax;

	private bool m_IsCompleted;

	private bool m_HasTip;

	private bool m_CanSolve;

	public bool IsTiming { get; private set; }

	public bool IsSuccess { get; private set; }

	public bool IsDoorOpening { get; private set; }

	public List<int> InstrumentOrder { get; private set; }

	public override void InitOnComplete()
	{
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Expected O, but got Unknown
		m_ProjectorRunningWildClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Projector_Running_Wild_01");
		m_PianoJumpscareClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Piano_Lid_Close_Jumpscare");
		m_ProjectorClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Projector_Switch_Turn_On_01");
		m_HenryClip10 = GameManager.Instance.GetAudioClip("Audio/DIA/CH2/Henry/DIA_CH2_HENRY_10");
		m_StrikeUpTheBandMax = m_BendyCutouts.Count;
		for (int i = 0; i < m_Instruments.Count; i++)
		{
			InteractableMusicalInstrument interactableMusicalInstrument = m_Instruments[i];
			if (interactableMusicalInstrument.InstrumentType == InstrumentType.PIANO)
			{
				interactableMusicalInstrument.SetActive(active: false);
			}
			interactableMusicalInstrument.OnNotePlayed += HandleInstrumentOnNotePlayed;
		}
		GenerateCutoutPositions();
		m_LightController.TurnOff();
		foreach (Transform item in m_Lights.transform.parent)
		{
			Transform val = item;
			if (((Object)val).name == "Sanctuary_Light REPLACEMENT")
			{
				m_Lights = ((Component)val).gameObject;
				break;
			}
		}
		m_Lights.SetActive(false);
		m_ProjectorController.InitTurnOff();
		m_ProjectorInteract.OnInteracted += HandleProjectorOnInteracted;
		m_ProjectorInteract.SetActive(active: true);
		m_PianoJumpscare.OnEnter += HandlePianoJumpscareOnEnter;
		m_PianoJumpscare.SetActive(active: true);
		m_LowerTrigger.OnEnter += HandleLowerTriggerOnEnter;
		m_LowerTrigger.SetActive(active: true);
		m_UpperTrigger.OnEnter += HandleUpperTriggerOnEnter;
		m_UpperTrigger.SetActive(active: true);
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH2Data.MusicPuzzleObjective.IsComplete)
		{
			ForceComplete();
			return;
		}
		if (GameManager.Instance.GameData.CurrentSaveFile.CH2Data.MusicPuzzleObjective.IsStarted)
		{
			m_ClosetDoor.ForceOpen(145f);
			m_CanSolve = true;
			GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH2_OBJECTIVE_SANCTUARY", "OBJECTIVES/CH2_OBJECTIVE_SANCTUARY_TIP"));
		}
		else
		{
			m_AudioLogController.OnFavoriteSongObjective += HandleFavoriteSongObjectiveOnActive;
		}
		m_AudioLogController.ActivateFavoriteSong();
	}

	private void HandleFavoriteSongObjectiveOnActive(object sender, EventArgs e)
	{
		GameManager.Instance.GameData.CurrentSaveFile.CH2Data.MusicPuzzleObjective.IsStarted = true;
		GameManager.Instance.GameDataManager.Save();
		m_CanSolve = true;
	}

	private void GenerateCutoutPositions()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		m_BendyCutouts.Shuffle();
		m_StagePositions.Shuffle();
		m_BalconyPositions.Shuffle();
		for (int i = 0; i < m_StrikeUpTheBandMax; i++)
		{
			Cutout cutout = new Cutout();
			cutout.Trans = m_BendyCutouts[i];
			cutout.StagePosition = m_StagePositions[i].position;
			cutout.StageRotation = m_StagePositions[i].rotation;
			cutout.BalconyPosition = m_BalconyPositions[i].position;
			cutout.BalconyRotation = m_BalconyPositions[i].rotation;
			Cutout item = cutout;
			m_ActiveCutouts.Add(item);
		}
	}

	public void GeneratePuzzle()
	{
		InstrumentOrder = new List<int>();
		SetupInstruments();
		InstrumentOrder.Shuffle();
	}

	private void SetupInstruments()
	{
		if (m_AvailableInstruments != null)
		{
			m_AvailableInstruments.Clear();
			m_AvailableInstruments = null;
		}
		m_AvailableInstruments = new List<Instrument>();
		m_AvailableInstruments.Add(new Instrument
		{
			InstrumentType = InstrumentType.BANJO,
			AvailableMax = 2
		});
		m_AvailableInstruments.Add(new Instrument
		{
			InstrumentType = InstrumentType.DRUM,
			AvailableMax = 2
		});
		m_AvailableInstruments.Add(new Instrument
		{
			InstrumentType = InstrumentType.BASS_FIDDLE,
			AvailableMax = 2
		});
		m_AvailableInstruments.Add(new Instrument
		{
			InstrumentType = InstrumentType.VIOLIN,
			AvailableMax = 2
		});
		m_AvailableInstruments.Add(new Instrument
		{
			InstrumentType = InstrumentType.PIANO,
			AvailableMax = 2
		});
		for (int i = 0; i < 4; i++)
		{
			FindNextInstrument();
		}
	}

	private void FindNextInstrument()
	{
		int num = Random.Range(0, m_AvailableInstruments.Count);
		if (m_AvailableInstruments[num].AvailableMax <= 0)
		{
			FindNextInstrument();
			return;
		}
		m_AvailableInstruments[num].AvailableMax--;
		InstrumentOrder.Add(num);
	}

	private void Update()
	{
		if (!GameManager.Instance.isPaused)
		{
			if (IsSuccess && m_IsCompleted && !IsDoorOpening && m_CanSolve)
			{
				DOSuccess();
			}
			if (IsTiming && !m_IsCompleted)
			{
				DOTimer();
			}
		}
	}

	private void DOTimer()
	{
		m_Timer += Time.deltaTime;
		if (m_Timer > m_TimerMax)
		{
			Reset();
		}
	}

	private void DOSuccess()
	{
		m_SuccessTimer += Time.deltaTime;
		if (m_SuccessTimer > m_SuccessTimerMax)
		{
			IsDoorOpening = true;
			GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH2_OBJECTIVE_ENTER_SANCTUARY", string.Empty, 4f));
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.MY_FAVORITE_SONG);
			m_LightController.TurnOn();
			m_Lights.SetActive(true);
			TurnOffProjector();
			OpenGate();
		}
	}

	private void HandlePianoJumpscareOnEnter(object sender, EventArgs e)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		m_PianoJumpscare.OnEnter -= HandlePianoJumpscareOnEnter;
		GameManager.Instance.AudioManager.PlayAtPosition(m_PianoJumpscareClip, m_PianoLid.position);
		ShortcutExtensions.DOKill((Component)(object)m_PianoLid, false);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_PianoLid, Vector3.zero, 0.5f, (RotateMode)0), (Ease)30), new TweenCallback(PianoJumpscareOnComplete));
	}

	private void PianoJumpscareOnComplete()
	{
		for (int i = 0; i < m_Instruments.Count; i++)
		{
			InteractableMusicalInstrument interactableMusicalInstrument = m_Instruments[i];
			if (interactableMusicalInstrument.InstrumentType == InstrumentType.PIANO)
			{
				interactableMusicalInstrument.SetActive(active: true);
			}
		}
	}

	private void HandleInstrumentOnNotePlayed(object sender, EventArgs e)
	{
		if (!IsTiming || m_IsCompleted)
		{
			return;
		}
		InteractableMusicalInstrument interactableMusicalInstrument = (InteractableMusicalInstrument)sender;
		if ((Object)(object)interactableMusicalInstrument == (Object)null)
		{
			return;
		}
		m_InstrumentsPlayed.Add((int)interactableMusicalInstrument.InstrumentType);
		if (InstrumentOrder.Count == m_InstrumentsPlayed.Count)
		{
			for (int i = 0; i < InstrumentOrder.Count; i++)
			{
				if (InstrumentOrder[i] == m_InstrumentsPlayed[i])
				{
					IsSuccess = true;
					continue;
				}
				IsSuccess = false;
				break;
			}
		}
		if (m_InstrumentsPlayed.Count > 4)
		{
			Reset();
		}
		else if (IsSuccess)
		{
			IsTiming = false;
			m_IsCompleted = true;
			m_Timer = 0f;
			if (m_StrikeUpTheBandCount >= m_StrikeUpTheBandMax)
			{
				GameManager.Instance.AchievementManager.SetAchievement(AchievementName.STRIKE_UP_THE_BAND);
			}
		}
		else if (!IsSuccess && m_InstrumentsPlayed.Count == 4)
		{
			Reset();
		}
	}

	private void Reset()
	{
		IsSuccess = false;
		m_Timer = 0f;
		IsTiming = false;
		IsSuccess = false;
		TurnOffProjector();
		m_ProjectorInteract.OnInteracted += HandleProjectorOnInteracted;
		m_ProjectorInteract.SetActive(active: true);
		if ((Object)(object)m_ProjectorRunningWildObject != (Object)null)
		{
			m_ProjectorRunningWildObject.Clear();
			m_ProjectorRunningWildObject = null;
		}
		if (m_InstrumentsPlayed != null)
		{
			m_InstrumentsPlayed.Clear();
		}
		m_StrikeUpTheBandCount++;
	}

	private void TurnOffProjector()
	{
		GameManager.Instance.AudioManager.Play(m_ProjectorClip);
		if ((Object)(object)m_ProjectorRunningWildObject != (Object)null)
		{
			m_ProjectorRunningWildObject.Clear();
			m_ProjectorRunningWildObject = null;
		}
		m_ProjectorController.TurnOffSilent();
	}

	private void HandleLowerTriggerOnEnter(object sender, EventArgs e)
	{
		MoveCutoutsToBalcony();
	}

	private void HandleUpperTriggerOnEnter(object sender, EventArgs e)
	{
		MoveCutoutsToStage();
	}

	private void MoveCutoutsToStage()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_StrikeUpTheBandCount && i < m_StrikeUpTheBandMax; i++)
		{
			Cutout cutout = m_ActiveCutouts[i];
			cutout.Trans.position = cutout.StagePosition;
			cutout.Trans.rotation = cutout.StageRotation;
		}
	}

	private void MoveCutoutsToBalcony()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_StrikeUpTheBandCount && i < m_StrikeUpTheBandMax; i++)
		{
			Cutout cutout = m_ActiveCutouts[i];
			cutout.Trans.position = cutout.BalconyPosition;
			cutout.Trans.rotation = cutout.BalconyRotation;
		}
	}

	private void OpenGate()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		m_Gate.Open();
		if ((Object)(object)m_PianoJumpscare != (Object)null)
		{
			m_PianoJumpscare.OnEnter -= HandlePianoJumpscareOnEnter;
		}
		m_PianoLid.localEulerAngles = Vector3.zero;
		PianoJumpscareOnComplete();
		for (int i = 0; i < m_Instruments.Count; i++)
		{
			m_Instruments[i].ForceRemoveEffects();
		}
		GameManager.Instance.GameData.CurrentSaveFile.CH2Data.MusicPuzzleObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
		SendOnComplete();
	}

	public void EnableTip()
	{
		m_PuzzleTip.OnEnter += HandlePuzzleTipOnEnter;
		m_PuzzleTip.SetActive(active: true);
		m_HasTip = true;
	}

	private void HandlePuzzleTipOnEnter(object sender, EventArgs e)
	{
		m_PuzzleTip.OnEnter -= HandlePuzzleTipOnEnter;
		if (m_HasTip)
		{
			GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryClip10, "DIACH2/DIA_CH2_HENRY_10"));
		}
	}

	private void HandleProjectorOnInteracted(object sender, EventArgs e)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		m_ProjectorInteract.SetActive(active: false);
		m_ProjectorInteract.OnInteracted -= HandleProjectorOnInteracted;
		m_ProjectorController.TurnOn();
		m_ProjectorRunningWildObject = GameManager.Instance.AudioManager.PlayAtPosition(m_ProjectorRunningWildClip, m_ProjectorController.transform.position, AudioObjectType.SOUND_EFFECT, -1);
		IsTiming = true;
		m_HasTip = false;
	}

	private void ForceComplete()
	{
		S13AudioManager.Instance.InvokeEvent("evt_CH2_save_point_06");
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH2_OBJECTIVE_ENTER_SANCTUARY", string.Empty));
		for (int i = 0; i < m_Instruments.Count; i++)
		{
			m_Instruments[i].ForceRemoveEffects();
		}
		m_AudioLogController.ActivateFavoriteSong();
		m_ClosetDoor.ForceOpen(145f);
		m_CanSolve = false;
		m_PuzzleTip.OnEnter -= HandlePuzzleTipOnEnter;
		m_PuzzleTip.SetActive(active: false);
		m_Gate.ForceOpen();
		m_LightController.TurnOn();
		m_Lights.SetActive(true);
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		m_ProjectorInteract.OnInteracted -= HandleProjectorOnInteracted;
		m_LowerTrigger.OnEnter -= HandleLowerTriggerOnEnter;
		m_UpperTrigger.OnEnter -= HandleUpperTriggerOnEnter;
		if ((Object)(object)m_PianoJumpscare != (Object)null)
		{
			m_PianoJumpscare.OnEnter -= HandlePianoJumpscareOnEnter;
		}
		for (int i = 0; i < m_Instruments.Count; i++)
		{
			m_Instruments[i].OnNotePlayed -= HandleInstrumentOnNotePlayed;
		}
		m_PianoJumpscareClip = null;
		m_ProjectorRunningWildClip = null;
		m_ProjectorClip = null;
		m_HenryClip10 = null;
		base.OnDisposed();
	}
}
