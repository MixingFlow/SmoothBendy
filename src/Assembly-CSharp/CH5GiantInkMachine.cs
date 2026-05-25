using System;
using System.Collections.Generic;
using Ai;
using DG.Tweening;
using UnityEngine;

public class CH5GiantInkMachine : BaseController
{
	[SerializeField]
	private Transform m_InkMachine;

	[SerializeField]
	private InteractablePowerLever m_DoorLever;

	[SerializeField]
	private GenericDoorController m_ThroneRoomDoor;

	[SerializeField]
	private CH5BendyScare m_BendyScare;

	[Header("Cutscene")]
	[SerializeField]
	private GameObject m_Darkness;

	[SerializeField]
	private AllyAiController m_Allison;

	[SerializeField]
	private AllyAiController m_Tom;

	[SerializeField]
	private EventTrigger m_CutsceneTrigger;

	[SerializeField]
	private Transform m_PlayerLookLocation;

	[SerializeField]
	private AnimationClip m_AllisonCutscene;

	[SerializeField]
	private Transform m_AllisonCutsceneLocation;

	[SerializeField]
	private Transform m_TomCutsceneLocation;

	[SerializeField]
	private List<WaypointNode> m_AllisonFinalLocation;

	[SerializeField]
	private List<WaypointNode> m_TomFinalLocation;

	[SerializeField]
	private EventTrigger m_WeaponTrigger;

	[SerializeField]
	private Transform m_CameraLook;

	[SerializeField]
	private CharacterLook m_CharacterLook;

	private Vector3 m_InkMachineFinalPosition;

	private AudioClip[] m_AllisonClip;

	private AudioClip m_HenryWeaponClip;

	private AudioClip m_TrueInkMachineMusic;

	private AudioClip m_LeverClip;

	private bool m_CanMoveCamera;

	public override void InitOnComplete()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		base.InitOnComplete();
		m_CutsceneTrigger.SetActive(active: false);
		m_DoorLever.SetActive(active: false);
		m_InkMachineFinalPosition = m_InkMachine.position;
		Transform inkMachine = m_InkMachine;
		inkMachine.position += new Vector3(0f, 20f, 0f);
		m_LeverClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Mainr_Power_Lever_Turn_On_01");
		m_HenryWeaponClip = GameManager.Instance.GetAudioClip("Audio/DIA/CH5/Henry/DIA_CH5_henry_theycouldhaveatleastgivenmeaweapon");
		m_AllisonClip = GameManager.Instance.GetAudioClips("Audio/DIA/CH5/AliceA/GiantInkMachine");
		m_TrueInkMachineMusic = GameManager.Instance.GetAudioClip("Audio/MUS/CH5/MUS_TheTrueInkMachine");
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH5Data.GiantInkMachineObjective.IsComplete)
		{
			ForceComplete();
		}
		else if (GameManager.Instance.GameData.CurrentSaveFile.CH5Data.GiantInkMachineObjective.IsStarted)
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
		m_CutsceneTrigger.OnEnter += HandleCutsceneTriggerOnEnter;
		m_CutsceneTrigger.SetActive(active: true);
	}

	private void ForceStart()
	{
		SetAllisonAndTomFinal();
		m_DoorLever.OnInteracted += HandleDoorLeverOnInteracted;
		m_DoorLever.OnComplete += HandleDoorLeverOnComplete;
		m_DoorLever.SetActive(active: true);
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/CURRENT_OBJECTIVE_HEADER", "OBJECTIVES/CH5_OBJECTIVE_ENTER_THE_MACHINE", string.Empty));
	}

	private void ForceComplete()
	{
		SetAllisonAndTomFinal();
		m_DoorLever.ForceOpen();
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/CURRENT_OBJECTIVE_HEADER", "OBJECTIVES/CH5_OBJECTIVE_ENTER_THE_MACHINE", string.Empty));
		SendOnComplete();
	}

	private void Update()
	{
		if (m_CanMoveCamera && !GameManager.Instance.isPaused && Object.op_Implicit((Object)(object)GameManager.Instance.GameCamera.FreeRoamCam))
		{
			m_CharacterLook.Rotation(m_CameraLook, GameManager.Instance.GameCamera.FreeRoamCam);
			m_CharacterLook.GetInput();
		}
	}

	private void SetAllisonAndTomFinal()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		m_Allison.gameObject.SetActive(true);
		m_Allison.ExitCombat();
		m_Allison.SetTarget(null);
		m_Allison.transform.position = m_AllisonFinalLocation[0].transform.position;
		m_Allison.transform.eulerAngles = m_AllisonFinalLocation[0].transform.eulerAngles;
		m_Allison.SetThought(AiThought.Idle);
		m_Tom.gameObject.SetActive(true);
		m_Tom.ForceStartIdle();
		m_Tom.ExitCombat();
		m_Tom.SetTarget(null);
		m_Tom.transform.position = m_TomFinalLocation[0].transform.position;
		m_Tom.transform.eulerAngles = m_TomFinalLocation[0].transform.eulerAngles;
		m_Tom.SetThought(AiThought.Idle);
	}

	private void HandleCutsceneTriggerOnEnter(object sender, EventArgs e)
	{
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Expected O, but got Unknown
		m_CutsceneTrigger.OnEnter -= HandleCutsceneTriggerOnEnter;
		if (Object.op_Implicit((Object)(object)m_BendyScare))
		{
			m_BendyScare.Dispose();
		}
		GameManager.Instance.AudioManager.Play(m_TrueInkMachineMusic, AudioObjectType.MUSIC);
		GameManager.Instance.HideCrosshair();
		Transform val = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		GameManager.Instance.Player.GoToAndLookAt(m_PlayerLookLocation);
		GameManager.Instance.Player.SetLock(active: true);
		Sequence val2 = DOTween.Sequence();
		float num = 0f;
		TweenSettingsExtensions.Insert(val2, num, (Tween)(object)ShortcutExtensions.DOMove(((Component)m_InkMachine).transform, m_InkMachineFinalPosition, 15f, false));
		TweenSettingsExtensions.Insert(val2, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(val, GameManager.Instance.Player.HeadContainer.position, 5f, false), (Ease)6));
		TweenSettingsExtensions.Insert(val2, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(val, m_PlayerLookLocation.eulerAngles, 5f, (RotateMode)0), (Ease)6));
		num += 6.5f;
		Vector3 eulerAngles = m_PlayerLookLocation.eulerAngles;
		eulerAngles.x = 0f;
		TweenSettingsExtensions.Insert(val2, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(m_PlayerLookLocation, eulerAngles, 2f, (RotateMode)0), (Ease)7));
		TweenSettingsExtensions.InsertCallback(val2, num, (TweenCallback)delegate
		{
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			GameManager.Instance.GameCamera.FreeRoamCam.SetParent(m_CameraLook);
			m_CharacterLook.Init(m_CameraLook, GameManager.Instance.GameCamera.FreeRoamCam);
			m_CharacterLook.HorizontalClampSetActive(active: true);
			m_CharacterLook.SetHorizontalClamp(45f);
			m_CanMoveCamera = true;
			m_Darkness.SetActive(false);
			m_Tom.SetTarget(null);
			m_Tom.transform.position = m_TomCutsceneLocation.position;
			m_Tom.transform.eulerAngles = m_TomCutsceneLocation.eulerAngles;
			m_Allison.SetTarget(null);
			m_Allison.SetCollider(active: false);
			m_Allison.SetHeadTracking(active: false);
			m_Allison.PlayAnimation(((Object)m_AllisonCutscene).name);
			m_Allison.transform.position = m_AllisonCutsceneLocation.position;
			m_Allison.transform.eulerAngles = m_AllisonCutsceneLocation.eulerAngles;
			for (int i = 0; i < m_AllisonClip.Length; i++)
			{
				AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_AllisonClip[i], SubtitleConstants.DIA_CH5_ALISONA_GIANT_INK_MACHINE[i], isTrimmed: true));
				if (i == m_AllisonClip.Length - 1)
				{
					audioObject.OnComplete += HandleCutsceneOnComplete;
				}
				else if (i == m_AllisonClip.Length - 7)
				{
					audioObject.OnComplete += HandleCutsceneColliderTrigger;
				}
			}
		});
	}

	private void HandleCutsceneColliderTrigger(object sender, EventArgs e)
	{
		(sender as AudioObject).OnComplete -= HandleCutsceneColliderTrigger;
		m_Allison.SetCollider(active: true);
	}

	private void HandleCutsceneOnComplete(object sender, EventArgs e)
	{
		(sender as AudioObject).OnComplete -= HandleCutsceneOnComplete;
		GameManager.Instance.GameCamera.FreeRoamCam.SetParent((Transform)null);
		GameManager.Instance.GameCamera.ExitFreeRoamCam();
		GameManager.Instance.Player.SetLock(active: false);
		m_Allison.SetTarget(null);
		m_Allison.UpdateWaypointList(m_AllisonFinalLocation);
		m_Allison.SetHeadTracking(active: true);
		m_Tom.SetTarget(null);
		m_Tom.UpdateWaypointList(m_TomFinalLocation);
		m_CanMoveCamera = false;
		GameManager.Instance.ShowCrosshair();
		m_WeaponTrigger.OnEnter += HandleWeaponTriggerOnEnter;
		m_WeaponTrigger.SetActive(active: true);
		m_DoorLever.OnInteracted += HandleDoorLeverOnInteracted;
		m_DoorLever.OnComplete += HandleDoorLeverOnComplete;
		m_DoorLever.SetActive(active: true);
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH5_OBJECTIVE_ENTER_THE_MACHINE", string.Empty, 4f));
	}

	private void HandleDoorLeverOnInteracted(object sender, EventArgs e)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		m_DoorLever.OnInteracted -= HandleDoorLeverOnInteracted;
		GameManager.Instance.AudioManager.PlayAtPosition(m_LeverClip, m_DoorLever.transform.position);
	}

	private void HandleWeaponTriggerOnEnter(object sender, EventArgs e)
	{
		m_WeaponTrigger.OnEnter -= HandleWeaponTriggerOnEnter;
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryWeaponClip, "DIACH5/DIA_CH5_HENRY_WEAPON"));
		GameManager.Instance.GameData.CurrentSaveFile.CH5Data.GiantInkMachineObjective.IsStarted = true;
		GameManager.Instance.GameDataManager.Save();
	}

	private void HandleDoorLeverOnComplete(object sender, EventArgs e)
	{
		m_DoorLever.OnComplete -= HandleDoorLeverOnComplete;
		m_ThroneRoomDoor.OnOpened += HandleThroneRoomDoorOnOpened;
		m_ThroneRoomDoor.Open();
	}

	private void HandleThroneRoomDoorOnOpened(object sender, EventArgs e)
	{
		m_ThroneRoomDoor.OnOpened -= HandleThroneRoomDoorOnOpened;
		GameManager.Instance.GameData.CurrentSaveFile.CH5Data.GiantInkMachineObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		m_HenryWeaponClip = null;
		m_AllisonClip = null;
		base.OnDisposed();
	}
}
