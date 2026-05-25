using System;
using System.Collections.Generic;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class CH3AliceLairController : BaseController
{
	[Header("<Controllers>")]
	[SerializeField]
	private CH3LiftController m_LiftController;

	[SerializeField]
	private CH3StairwellController m_StairwellController;

	[Header("Objective: Get Out Of The Lift!")]
	[SerializeField]
	private EventTrigger m_BorisExitLiftTrigger;

	[SerializeField]
	private EventTrigger m_LiftExitTrigger;

	[SerializeField]
	private WaypointList m_ExitLiftPath;

	[SerializeField]
	private WaypointList m_AliceLairPath;

	[Header("Objective: Enter The Lair!")]
	[SerializeField]
	private GenericDoorController m_LairEntrance;

	[SerializeField]
	private EventTrigger m_EntranceTrigger;

	[SerializeField]
	private EventTrigger m_LeaveTrigger;

	[SerializeField]
	private WaypointList m_BodyRoomPath;

	[Header("Boris Meets Boris")]
	[SerializeField]
	private EventTrigger m_BodyRoomTrigger;

	[SerializeField]
	private EventTrigger m_TortureRoomTrigger;

	[SerializeField]
	private GenericDoorController m_TortureRoomEntrance;

	[SerializeField]
	private EventTrigger m_AliceDeadBodiesTrigger;

	[SerializeField]
	private EventTrigger m_WalkTrigger;

	[SerializeField]
	private EventTrigger m_HorrorCueTrigger;

	[SerializeField]
	private Transform m_DeadBorisHeadPosition;

	[Header("Alice Cutscene")]
	[SerializeField]
	private CH3AliceButtonController m_AliceController;

	[SerializeField]
	private AudioSource m_TortureAudio;

	[SerializeField]
	private EventTrigger m_TortureDoorCloseTrigger;

	[SerializeField]
	private LightFlicker m_PiperLight;

	[SerializeField]
	private ParticleSystem m_PiperSparks;

	[SerializeField]
	private ParticleSystem m_PiperTriggerSparks;

	[SerializeField]
	private Animator m_AnimationController;

	[SerializeField]
	private Animator m_PiperAnimationController;

	[SerializeField]
	private GenericDoorController m_TortureRoomLockout;

	[SerializeField]
	private GameObject m_Alice;

	[SerializeField]
	private EventTrigger m_AliceCutsceneTrigger;

	[SerializeField]
	private List<Transform> m_WindowShutters;

	[Header("<DEV CHEAT POSITIONS>")]
	[SerializeField]
	private Transform[] m_CheatPoints;

	private int m_CurrentCheatPoint;

	private AudioClip[] m_AliceMonologueClips;

	private AudioClip[] m_AliceLiftDeadBodiesClips;

	private AudioClip m_HorrorCue;

	private AudioClip m_TortureEndClip;

	private AudioClip m_MonologueMusic;

	private AudioClip m_BeautyMusic;

	private AudioClip m_AliceControllerButtonClip;

	private AudioClip m_GateOpenClip;

	private AudioClip m_GateCloseClip;

	private BorisAi m_Boris => GameManager.Instance.CharacterManager.Boris;

	public override void InitOnComplete()
	{
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		base.InitOnComplete();
		m_AliceLiftDeadBodiesClips = GameManager.Instance.GetAudioClips("Audio/DIA/CH3/Alice/MonologueDeadBodies/");
		m_AliceMonologueClips = GameManager.Instance.GetAudioClips("Audio/DIA/CH3/Alice/MonologueMain/");
		m_HorrorCue = GameManager.Instance.GetAudioClip("Audio/MUS/CH2/MUS_Horror_Cue_02");
		m_TortureEndClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_pipertorturestop");
		m_MonologueMusic = GameManager.Instance.GetAudioClip("Audio/MUS/CH3/MUS_CH3_thedarkpuddles");
		m_BeautyMusic = GameManager.Instance.GetAudioClip("Audio/MUS/CH3/MUS_CH3_thepriceofbeauty");
		m_AliceControllerButtonClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_alicepressbutton");
		m_GateOpenClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Gate_Open_Slow_01");
		m_GateCloseClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Gate_Close_01");
		m_AliceDeadBodiesTrigger.SetActive(active: false);
		m_BorisExitLiftTrigger.SetActive(active: false);
		m_LiftExitTrigger.SetActive(active: false);
		m_EntranceTrigger.SetActive(active: false);
		m_BodyRoomTrigger.SetActive(active: false);
		m_AliceCutsceneTrigger.SetActive(active: false);
		m_LeaveTrigger.SetActive(active: false);
		m_WalkTrigger.SetActive(active: false);
		m_HorrorCueTrigger.SetActive(active: false);
		m_Alice.SetActive(false);
		m_TortureRoomTrigger.SetActive(active: false);
		m_TortureDoorCloseTrigger.SetActive(active: false);
		m_TortureRoomLockout.ForceOpen();
		for (int i = 0; i < m_WindowShutters.Count; i++)
		{
			Transform obj = m_WindowShutters[i];
			obj.localPosition += new Vector3(0f, 15f, 0f);
		}
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.GearTask.Status.IsStarted)
		{
			ClearLair();
			m_LairEntrance.ForceClose();
			SendOnComplete();
		}
		else if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.AliceLairObjective.IsComplete)
		{
			ForceComplete();
		}
		else if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.AliceLairObjective.IsStarted)
		{
			ForceStart();
		}
		else
		{
			InternalActivate();
		}
	}

	private void InternalActivate()
	{
		m_LiftExitTrigger.OnEnter += HandleLiftExitTriggerOnEnter;
		m_LiftExitTrigger.SetActive(active: true);
		m_BorisExitLiftTrigger.OnEnter += HandleBorisExitLiftTriggerOnEnter;
		m_BorisExitLiftTrigger.SetActive(active: true);
	}

	private void ForceStart()
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_16", "OBJECTIVES/CH3_OBJECTIVE_16_TIP"));
		m_LairEntrance.ForceOpen();
		m_Boris.StopWaypointPathing();
		m_Boris.transform.position = m_BodyRoomPath.Waypoints[m_BodyRoomPath.Waypoints.Count - 1].transform.position;
		m_Boris.transform.eulerAngles = m_BodyRoomPath.Waypoints[m_BodyRoomPath.Waypoints.Count - 1].transform.eulerAngles;
		m_Boris.LookAtTarget(m_DeadBorisHeadPosition);
		m_LiftController.GoToFloor(3, isInitialArrival: true, wasCalled: true);
		m_AliceDeadBodiesTrigger.SetActive(active: true);
		m_AliceDeadBodiesTrigger.OnEnter += HandleAliceDeadBodiesTriggerOnEnter;
		m_WalkTrigger.SetActive(active: true);
		m_WalkTrigger.OnEnter += HandleWalkTriggerOnEnter;
		m_WalkTrigger.OnExit += HandleWalkTriggerOnExit;
		m_TortureRoomTrigger.OnEnter += HandleTortureRoomTriggerOnEnter;
		m_TortureRoomTrigger.SetActive(active: true);
	}

	private void ForceComplete()
	{
		ClearLair();
		m_LeaveTrigger.SetActive(active: true);
		m_LeaveTrigger.OnEnter += HandleLeaveTriggerOnEnter;
	}

	private void ClearLair()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Expected O, but got Unknown
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_17", "OBJECTIVES/CH3_OBJECTIVE_17_TIP"));
		EmissionModule emission = m_PiperSparks.emission;
		((EmissionModule)(ref emission)).enabled = false;
		for (int i = 0; i < m_WindowShutters.Count; i++)
		{
			Transform obj = m_WindowShutters[i];
			obj.localPosition -= new Vector3(0f, 15f, 0f);
		}
		m_WalkTrigger.SetActive(active: true);
		m_WalkTrigger.OnEnter += HandleWalkTriggerOnEnter;
		m_WalkTrigger.OnExit += HandleWalkTriggerOnExit;
		m_LairEntrance.ForceOpen();
		m_TortureRoomEntrance.ForceOpen();
		m_LiftController.GoToAlice(0f, hasDialogue: false);
		m_LiftController.EnableBoris();
		m_LiftController.EnableLift();
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 0.1f, (TweenCallback)delegate
		{
			m_LiftController.ForceCloseLift();
		});
		m_StairwellController.UnlockAllFloors();
	}

	private void HandleWalkTriggerOnEnter(object sender, EventArgs e)
	{
		GameManager.Instance.Player.SetRun(active: false);
	}

	private void HandleWalkTriggerOnExit(object sender, EventArgs e)
	{
		GameManager.Instance.Player.SetRun(active: true);
	}

	private void HandleAliceDeadBodiesTriggerOnEnter(object sender, EventArgs e)
	{
		m_AliceDeadBodiesTrigger.OnEnter -= HandleAliceDeadBodiesTriggerOnEnter;
		GameManager.Instance.AudioManager.Play(m_BeautyMusic, AudioObjectType.MUSIC);
		for (int i = 0; i < m_AliceLiftDeadBodiesClips.Length; i++)
		{
			GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_AliceLiftDeadBodiesClips[i], SubtitleConstants.DIALOGUE_CH3_ALICE_DEAD_BODIES[i], isTrimmed: true));
		}
	}

	private void HandleBorisExitLiftTriggerOnEnter(object sender, EventArgs e)
	{
		m_BorisExitLiftTrigger.OnEnter -= HandleBorisExitLiftTriggerOnEnter;
		m_LiftController.DisableLift();
		m_Boris.OnWaypointComplete += HandleBorisExitLiftNodeOnComplete;
		m_Boris.UpdateWaypointList(m_ExitLiftPath.Waypoints);
	}

	private void HandleLiftExitTriggerOnEnter(object sender, EventArgs e)
	{
		m_LiftExitTrigger.OnEnter -= HandleLiftExitTriggerOnEnter;
		m_LiftExitTrigger.SetActive(active: false);
		m_Boris.OnWaypointComplete += HandleBorisAliceDoorNodeOnComplete;
		m_Boris.AddToWaypointList(m_AliceLairPath.Waypoints);
	}

	private void HandleBorisExitLiftNodeOnComplete(object sender, EventArgs e)
	{
		m_Boris.OnWaypointComplete -= HandleBorisExitLiftNodeOnComplete;
		m_Boris.StopWaypointPathing();
		m_ExitLiftPath.Dispose();
	}

	private void HandleBorisAliceDoorNodeOnComplete(object sender, EventArgs e)
	{
		m_Boris.OnWaypointComplete -= HandleBorisExitLiftNodeOnComplete;
		m_Boris.OnWaypointComplete -= HandleBorisAliceDoorNodeOnComplete;
		m_Boris.StopWaypointPathing();
		m_AliceLairPath.Dispose();
		if ((Object)(object)m_ExitLiftPath != (Object)null)
		{
			m_ExitLiftPath.Dispose();
		}
		m_EntranceTrigger.SetActive(active: true);
		m_EntranceTrigger.OnEnter += HandleEntranceOnEnter;
	}

	private void HandleEntranceOnEnter(object sender, EventArgs e)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		m_EntranceTrigger.OnEnter -= HandleEntranceOnEnter;
		Sequence val = DOTween.Sequence();
		GameManager.Instance.AudioManager.Play(m_GateOpenClip);
		TweenSettingsExtensions.InsertCallback(val, 2f, (TweenCallback)delegate
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Expected O, but got Unknown
			TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetDelay<Tweener>(ShortcutExtensions.DOShakePosition(GameManager.Instance.GameCamera.transform, 8f, 0.1f, 7, 90f, false, true), 1f), (TweenCallback)delegate
			{
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				GameManager.Instance.GameCamera.transform.localPosition = Vector3.zero;
			});
			m_LairEntrance.Open();
		});
		TweenSettingsExtensions.InsertCallback(val, 7f, (TweenCallback)delegate
		{
			m_Boris.OnWaypointComplete += HandleBorisDeadNodeOnComplete;
			m_Boris.gameObject.layer = LayerMask.NameToLayer("SelfDefault");
			m_Boris.LookAtTarget(m_DeadBorisHeadPosition);
			m_Boris.AddToWaypointList(m_BodyRoomPath.Waypoints, isRunning: true);
		});
		m_BodyRoomTrigger.SetActive(active: true);
		m_BodyRoomTrigger.OnEnter += HandleBodyRoomTriggerOnEnter;
		m_AliceDeadBodiesTrigger.SetActive(active: true);
		m_AliceDeadBodiesTrigger.OnEnter += HandleAliceDeadBodiesTriggerOnEnter;
		m_WalkTrigger.SetActive(active: true);
		m_WalkTrigger.OnEnter += HandleWalkTriggerOnEnter;
		m_WalkTrigger.OnExit += HandleWalkTriggerOnExit;
		m_HorrorCueTrigger.SetActive(active: true);
		m_HorrorCueTrigger.OnEnter += HandleHorrorCueTriggerOnEnter;
	}

	private void HandleHorrorCueTriggerOnEnter(object sender, EventArgs e)
	{
		m_HorrorCueTrigger.OnEnter -= HandleHorrorCueTriggerOnEnter;
		GameManager.Instance.AudioManager.Play(m_HorrorCue);
	}

	private void HandleBorisDeadNodeOnComplete(object sender, EventArgs e)
	{
		m_Boris.OnWaypointComplete -= HandleBorisExitLiftNodeOnComplete;
		m_Boris.OnWaypointComplete -= HandleBorisAliceDoorNodeOnComplete;
		m_Boris.OnWaypointComplete -= HandleBorisDeadNodeOnComplete;
		m_Boris.gameObject.layer = LayerMask.NameToLayer("Ai");
		m_Boris.StopWaypointPathing();
		m_Boris.LookAtTarget(m_DeadBorisHeadPosition);
		m_BodyRoomPath.Dispose();
		if ((Object)(object)m_AliceLairPath != (Object)null)
		{
			m_AliceLairPath.Dispose();
		}
		if ((Object)(object)m_ExitLiftPath != (Object)null)
		{
			m_ExitLiftPath.Dispose();
		}
		m_BodyRoomPath.Dispose();
	}

	private void HandleBodyRoomTriggerOnEnter(object sender, EventArgs e)
	{
		m_BodyRoomTrigger.OnEnter -= HandleBodyRoomTriggerOnEnter;
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.AliceLairObjective.IsStarted = true;
		GameManager.Instance.GameDataManager.Save();
		m_TortureRoomTrigger.OnEnter += HandleTortureRoomTriggerOnEnter;
		m_TortureRoomTrigger.SetActive(active: true);
	}

	private void HandleTortureRoomTriggerOnEnter(object sender, EventArgs e)
	{
		m_TortureRoomTrigger.OnEnter -= HandleTortureRoomTriggerOnEnter;
		m_Alice.SetActive(true);
		m_TortureRoomEntrance.Open();
		m_TortureDoorCloseTrigger.OnEnter += HandleTortureDoorCloseTriggerOnEnter;
		m_TortureDoorCloseTrigger.SetActive(active: true);
	}

	private void HandleTortureDoorCloseTriggerOnEnter(object sender, EventArgs e)
	{
		m_TortureDoorCloseTrigger.OnEnter -= HandleTortureDoorCloseTriggerOnEnter;
		m_TortureRoomLockout.Close();
		S13AudioManager.Instance.ToSnapshot("TMGAudioMixer", "mxs_alice_monologues", 1f);
		m_AliceCutsceneTrigger.OnEnter += HandleAliceCutsceneOnEnter;
		m_AliceCutsceneTrigger.SetActive(active: true);
	}

	private void HandleAliceCutsceneOnEnter(object sender, EventArgs e)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		m_AliceCutsceneTrigger.OnEnter -= HandleAliceCutsceneOnEnter;
		m_PiperAnimationController.SetTrigger("Dead");
		m_PiperLight.TurnOff();
		m_PiperTriggerSparks.Emit(30);
		EmissionModule emission = m_PiperSparks.emission;
		((EmissionModule)(ref emission)).enabled = false;
		m_TortureAudio.Stop();
		m_TortureAudio.volume = 0f;
		GameManager.Instance.AudioManager.Play(m_TortureEndClip).OnComplete += HandleTortureEndOnComplete;
	}

	private void HandleTortureEndOnComplete(object sender, EventArgs e)
	{
		(sender as AudioObject).OnComplete -= HandleTortureEndOnComplete;
		GameManager.Instance.AudioManager.Play(m_MonologueMusic, AudioObjectType.MUSIC);
		m_AliceController.OnPressed += HandleAliceControllerOnPressed;
		m_AliceController.Activate();
		for (int i = 0; i < m_AliceMonologueClips.Length; i++)
		{
			GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_AliceMonologueClips[i], SubtitleConstants.DIALOGUE_CH3_ALICE_MAIN_MONOLOGUE[i], isTrimmed: true));
		}
	}

	private void HandleAliceControllerOnPressed(object sender, EventArgs e)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Expected O, but got Unknown
		m_AliceController.OnPressed -= HandleAliceControllerOnPressed;
		Sequence val = DOTween.Sequence();
		float num = 0.25f;
		float num2 = 0.5f;
		GameManager.Instance.AudioManager.Play(m_AliceControllerButtonClip);
		for (int i = 0; i < m_WindowShutters.Count; i++)
		{
			Transform val2 = m_WindowShutters[i];
			TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
			{
				GameManager.Instance.AudioManager.Play(m_GateCloseClip);
			});
			TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(val2, val2.localPosition.y - 15f, num2, false), (Ease)1));
			num += num2 / 2f;
		}
		TweenSettingsExtensions.OnComplete<Sequence>(val, new TweenCallback(ShuttersOnComplete));
	}

	private void ShuttersOnComplete()
	{
		m_TortureAudio.Play();
		m_TortureAudio.DOFade(1f, 0.75f);
		m_TortureRoomLockout.Open();
		S13AudioManager.Instance.ToSnapshot("TMGAudioMixer", "mxs_base", 1f);
		m_LiftController.EnableBoris();
		m_LiftController.CloseLift();
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_17", "OBJECTIVES/CH3_OBJECTIVE_17_TIP", 4f));
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.AliceLairObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
		m_LeaveTrigger.SetActive(active: true);
		m_LeaveTrigger.OnEnter += HandleLeaveTriggerOnEnter;
	}

	private void HandleLeaveTriggerOnEnter(object sender, EventArgs e)
	{
		m_LeaveTrigger.OnEnter -= HandleLeaveTriggerOnEnter;
		m_LairEntrance.OnClose += HandleGateCloseOnComplete;
		m_LairEntrance.Close();
	}

	private void HandleGateCloseOnComplete(object sender, EventArgs e)
	{
		m_LairEntrance.OnClose -= HandleGateCloseOnComplete;
		m_StairwellController.UnlockAllFloors();
		m_LiftController.EnableLift();
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		m_AliceLiftDeadBodiesClips = null;
		m_AliceMonologueClips = null;
		m_HorrorCue = null;
		m_TortureEndClip = null;
		m_MonologueMusic = null;
		m_BeautyMusic = null;
		m_AliceControllerButtonClip = null;
		m_GateCloseClip = null;
		m_GateOpenClip = null;
		base.OnDisposed();
	}
}
