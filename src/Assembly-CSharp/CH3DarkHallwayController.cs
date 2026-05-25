using System;
using System.Collections.Generic;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class CH3DarkHallwayController : BaseController
{
	[Header("<Controllers>")]
	[SerializeField]
	private BorisAi m_Boris;

	[Header("Triggers")]
	[SerializeField]
	private EventTrigger m_HallwayTrigger;

	[SerializeField]
	private EventTrigger m_DuctCrawlingTrigger;

	[Header("Boris Stuff")]
	[SerializeField]
	private WaypointList m_BorisInitialPath;

	[Header("Objective: Get The Flashlight!")]
	[SerializeField]
	private GameObject m_DarknessEntranceBlocker;

	[SerializeField]
	private EventTrigger m_HandCrowbarDialogue;

	[SerializeField]
	private CH3Flashlight m_Flashlight;

	[SerializeField]
	private EventTrigger m_DarkHallwayEnterLockedIn;

	[SerializeField]
	private EventTrigger m_DarkHallwayExitLockedIn;

	[SerializeField]
	private GenericDoorController m_HallwayDoor;

	[SerializeField]
	private Collider m_DarkenerCollider;

	[SerializeField]
	private WaypointList m_BorisDarknessPath;

	[SerializeField]
	private WaypointList m_BorisDarknessHallwayPath;

	[Header("Objective: Open The Door!")]
	[SerializeField]
	private Transform m_Vent;

	[SerializeField]
	private GenericDoorController m_ClosedDoor;

	[SerializeField]
	private GenericDoorController m_OpenedDoor;

	[SerializeField]
	private Transform m_BorisHandParent;

	[SerializeField]
	private GameObject m_BlockerCollider;

	[SerializeField]
	private EventTrigger m_DoorCloseTrigger;

	[SerializeField]
	private EventTrigger m_BorisTrigger;

	[SerializeField]
	private WaypointList m_BorisFinalPath;

	[SerializeField]
	private Transform m_BorisWarpPosition;

	[Header("<DEV CHEAT POSITIONS>")]
	[SerializeField]
	private Transform m_CheatPoint;

	private AudioClip m_DuctCrawlingClip;

	private AudioClip m_HenryDontBeScaredDialogueClip;

	private AudioClip m_HenryDarkDialogueClip;

	private AudioClip m_FlashlightClip;

	private AudioClip m_VentCoverClip;

	private AudioClip m_BorisEnterVentClip;

	private AudioClip[] m_HenryDidYouHearThatClip;

	private AudioClip[] m_HenryDeadEndClip;

	private bool m_IsBorisInDarkness;

	private bool m_IsPlayerInDarkness;

	private GameObject m_FlashlightToDestroy;

	private Vector3 borisVentPosition;

	private Vector3 borisVentEuler;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_Boris = GameManager.Instance.CharacterManager.Boris;
		m_DarknessEntranceBlocker.SetActive(false);
		m_HandCrowbarDialogue.SetActive(active: false);
		m_DarkHallwayEnterLockedIn.SetActive(active: false);
		m_DarkHallwayExitLockedIn.SetActive(active: false);
		m_DuctCrawlingClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_runningoverhead");
		m_HenryDontBeScaredDialogueClip = GameManager.Instance.GetAudioClip("Audio/DIA/CH3/Henry/ch3_henry_30_dontbescaredboris");
		m_HenryDarkDialogueClip = GameManager.Instance.GetAudioClip("Audio/DIA/CH3/Henry/ch3_henry_08_lookslikeitsdarkupahead");
		m_FlashlightClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_flashlighturnon");
		m_VentCoverClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_borisventcover");
		m_BorisEnterVentClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_borisducts");
		m_HenryDidYouHearThatClip = GameManager.Instance.GetAudioClips("Audio/DIA/CH3/Henry/DidYouHearThat/");
		m_HenryDeadEndClip = GameManager.Instance.GetAudioClips("Audio/DIA/CH3/Henry/DeadEnd/");
		m_OpenedDoor.ForceOpen();
		m_ClosedDoor.ForceClose();
		m_HallwayDoor.ForceOpen();
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.DarkHallwayObjective.IsComplete)
		{
			ForceComplete();
			return;
		}
		m_HallwayTrigger.OnEnter += HandleHallwayOnEnter;
		m_HallwayTrigger.OnExit += HandleHallwayOnExit;
		m_DoorCloseTrigger.SetActive(active: false);
		m_BorisTrigger.SetActive(active: false);
		m_HandCrowbarDialogue.SetActive(active: true);
		m_HandCrowbarDialogue.OnEnter += HandleHandoverDialogueOnEnter;
		m_Boris.OnWaypointComplete += HandleFirstWaypointOnComplete;
		m_Boris.UpdateWaypointList(m_BorisInitialPath.Waypoints);
	}

	private void HandleHandoverDialogueOnEnter(object sender, EventArgs e)
	{
		m_HandCrowbarDialogue.OnEnter -= HandleHandoverDialogueOnEnter;
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryDarkDialogueClip, "DIACH3/DIA_CH3_HENRY_08")).OnComplete += delegate
		{
			GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_07", "OBJECTIVES/CH3_OBJECTIVE_07_TIP", 4f));
			m_Flashlight.OnInteracted += HandleFlashlightOnInteracted;
			m_Flashlight.Activate();
		};
	}

	private void HandleFirstWaypointOnComplete(object sender, EventArgs e)
	{
		m_Boris.OnWaypointComplete -= HandleFirstWaypointOnComplete;
		m_Boris.StopWaypointPathing();
		m_BorisInitialPath.Dispose();
	}

	private void HandleFlashlightOnInteracted(object sender, EventArgs e)
	{
		m_Flashlight.OnInteracted -= HandleFlashlightOnInteracted;
		GameManager.Instance.AudioManager.Play(m_FlashlightClip);
		m_DarkenerCollider.enabled = false;
		m_Boris.OnWaypointComplete -= HandleFirstWaypointOnComplete;
		m_Boris.OnWaypointComplete += HandleDarknessWaypointOnComplete;
		m_Boris.AddToWaypointList(m_BorisDarknessPath.Waypoints);
		m_DarkHallwayEnterLockedIn.SetActive(active: true);
		m_DarkHallwayEnterLockedIn.OnEnter += HandleDarkHallwayEnterLockedInOnEnter;
		m_DuctCrawlingTrigger.SetActive(active: true);
		m_DuctCrawlingTrigger.OnEnter += HandleDuctCrawlingTriggerOnEnter;
	}

	private void HandleDuctCrawlingTriggerOnEnter(object sender, EventArgs e)
	{
		m_DuctCrawlingTrigger.OnEnter -= HandleDuctCrawlingTriggerOnEnter;
		m_Boris.LookAround();
		GameManager.Instance.AudioManager.Play(m_DuctCrawlingClip).OnComplete += HandleDuctAudioOnComplete;
	}

	private void HandleDuctAudioOnComplete(object sender, EventArgs e)
	{
		(sender as AudioObject).OnComplete -= HandleDuctAudioOnComplete;
		for (int i = 0; i < m_HenryDidYouHearThatClip.Length; i++)
		{
			GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryDidYouHearThatClip[i], SubtitleConstants.DIA_CH3_HENRY_DID_YOU_HEAR_THAT[i], isTrimmed: true));
		}
	}

	private void HandleDarknessWaypointOnComplete(object sender, EventArgs e)
	{
		m_Boris.OnWaypointComplete -= HandleDarknessWaypointOnComplete;
		m_Boris.StopWaypointPathing();
		m_IsBorisInDarkness = true;
		m_BorisDarknessPath.Dispose();
		if ((Object)(object)m_BorisInitialPath != (Object)null)
		{
			m_BorisInitialPath.Dispose();
		}
		if (CheckInDarkness())
		{
			CloseDarknessDoor();
		}
	}

	private void HandleDarkHallwayEnterLockedInOnEnter(object sender, EventArgs e)
	{
		m_DarkHallwayEnterLockedIn.OnEnter -= HandleDarkHallwayEnterLockedInOnEnter;
		m_DarkHallwayEnterLockedIn.SetActive(active: false);
		m_IsPlayerInDarkness = true;
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryDontBeScaredDialogueClip, "DIACH3/DIA_CH3_HENRY_30"));
		if (CheckInDarkness())
		{
			CloseDarknessDoor();
			return;
		}
		m_DarkHallwayExitLockedIn.ResetTrigger();
		m_DarkHallwayExitLockedIn.SetActive(active: true);
		m_DarkHallwayExitLockedIn.OnEnter += HandleDarkHallwayExitLocedInOnEnter;
	}

	private void HandleDarkHallwayExitLocedInOnEnter(object sender, EventArgs e)
	{
		m_DarkHallwayExitLockedIn.OnEnter -= HandleDarkHallwayExitLocedInOnEnter;
		m_DarkHallwayExitLockedIn.SetActive(active: false);
		m_IsPlayerInDarkness = false;
		m_DarkHallwayEnterLockedIn.ResetTrigger();
		m_DarkHallwayEnterLockedIn.SetActive(active: true);
		m_DarkHallwayEnterLockedIn.OnEnter += HandleDarkHallwayEnterLockedInOnEnter;
	}

	private bool CheckInDarkness()
	{
		return m_IsPlayerInDarkness && m_IsBorisInDarkness;
	}

	private void CloseDarknessDoor()
	{
		m_DarknessEntranceBlocker.SetActive(true);
		m_HallwayDoor.Close();
		m_Boris.StopWaypointPathing();
		m_Boris.SlowSpeeds();
		m_Boris.UpdateWaypointList(m_BorisDarknessHallwayPath.Waypoints);
		m_BorisTrigger.SetActive(active: true);
		m_BorisTrigger.OnEnter += HandleBorisTriggerOnEnter;
	}

	private void HandleBorisTriggerOnEnter(object sender, EventArgs e)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		m_BorisTrigger.OnEnter -= HandleBorisTriggerOnEnter;
		m_Boris.ResetSpeeds();
		borisVentPosition = m_BorisFinalPath.Waypoints[m_BorisFinalPath.Waypoints.Count - 1].transform.position;
		borisVentEuler = m_BorisFinalPath.Waypoints[m_BorisFinalPath.Waypoints.Count - 1].transform.eulerAngles;
		m_Boris.OnWaypointComplete += HandleBorisOnWaypointComplete;
		m_Boris.UpdateWaypointList(m_BorisFinalPath.Waypoints);
		m_DoorCloseTrigger.SetActive(active: true);
		m_DoorCloseTrigger.OnEnter += HandleDoorCloseTriggerOnEnter;
	}

	private void HandleDoorCloseTriggerOnEnter(object sender, EventArgs e)
	{
		m_DoorCloseTrigger.OnEnter -= HandleDoorCloseTriggerOnEnter;
		CloseDoor();
	}

	private void HandleBorisOnWaypointComplete(object sender, EventArgs e)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		m_Boris.OnWaypointComplete -= HandleBorisOnWaypointComplete;
		m_BorisFinalPath.Dispose();
		m_Boris.transform.position = borisVentPosition;
		m_Boris.transform.eulerAngles = borisVentEuler;
		m_Boris.StopLooking();
		for (int i = 0; i < m_HenryDeadEndClip.Length; i++)
		{
			AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryDeadEndClip[i], SubtitleConstants.DIA_CH3_HENRY_DEAD_END[i], isTrimmed: true));
			if (i == 1)
			{
				audioObject.OnComplete += HandleDeadEndOnComplete;
			}
		}
	}

	private void HandleDeadEndOnComplete(object sender, EventArgs e)
	{
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_08", string.Empty, 4f));
		m_Boris.Interact.SetActive(active: true);
		m_Boris.Interact.OnInteracted += HandleBorisOnInteracted;
	}

	private void CloseDoor()
	{
		m_BlockerCollider.SetActive(true);
		m_OpenedDoor.OnClose += HandleOpenedDoorOnClosed;
		m_OpenedDoor.Close();
	}

	private void HandleOpenedDoorOnClosed(object sender, EventArgs e)
	{
		m_OpenedDoor.OnClose -= HandleOpenedDoorOnClosed;
		m_HallwayTrigger.OnEnter -= HandleHallwayOnEnter;
		m_HallwayTrigger.OnExit -= HandleHallwayOnExit;
		m_HallwayTrigger.Dispose();
	}

	private void HandleBorisOnInteracted(object sender, EventArgs e)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected O, but got Unknown
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Expected O, but got Unknown
		m_Boris.Interact.OnInteracted -= HandleBorisOnInteracted;
		m_Boris.Interact.SetActive(active: false);
		GameManager.Instance.Player.WeaponGameObject.transform.SetParent(m_BorisHandParent);
		GameManager.Instance.Player.WeaponGameObject.transform.localPosition = Vector3.zero;
		GameManager.Instance.Player.WeaponGameObject.transform.localEulerAngles = Vector3.zero;
		GameManager.Instance.Player.WeaponGameObject.layer = LayerMask.NameToLayer("IgnoreLight");
		foreach (Transform item in GameManager.Instance.Player.WeaponGameObject.transform)
		{
			Transform val = item;
			((Component)val).gameObject.layer = LayerMask.NameToLayer("IgnoreLight");
		}
		m_FlashlightToDestroy = GameManager.Instance.Player.WeaponGameObject;
		GameManager.Instance.Player.WeaponGameObject = null;
		float ventPosition = m_Vent.localPosition.y;
		GameManager.Instance.AudioManager.Play(m_VentCoverClip);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(TweenSettingsExtensions.SetDelay<Tweener>(ShortcutExtensions.DOLocalMoveY(m_Vent, ventPosition + 3.5f, 0.5f, false), 0.75f), (Ease)7), (TweenCallback)delegate
		{
			TweenSettingsExtensions.SetEase<Tweener>(TweenSettingsExtensions.SetDelay<Tweener>(ShortcutExtensions.DOLocalMoveY(m_Vent, ventPosition, 0.5f, false), 7f), (Ease)7);
			VentOpenOnComplete();
		});
	}

	private void VentOpenOnComplete()
	{
		m_Boris.EnterVent();
		GameManager.Instance.AudioManager.Play(m_BorisEnterVentClip).OnComplete += HandleBorisDuctAudioOnComplete;
	}

	private void HandleBorisDuctAudioOnComplete(object sender, EventArgs e)
	{
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		(sender as AudioObject).OnComplete -= HandleBorisDuctAudioOnComplete;
		Object.Destroy((Object)(object)m_FlashlightToDestroy);
		m_ClosedDoor.Open();
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetDelay<Tweener>(ShortcutExtensions.DOShakePosition(GameManager.Instance.GameCamera.transform, 8f, 0.1f, 7, 90f, false, true), 1f), (TweenCallback)delegate
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			GameManager.Instance.GameCamera.transform.localPosition = Vector3.zero;
		});
		m_Boris.transform.position = m_BorisWarpPosition.position;
		Vector3 eulerAngles = m_BorisWarpPosition.eulerAngles;
		eulerAngles.x = 0f;
		eulerAngles.z = 0f;
		m_Boris.transform.eulerAngles = eulerAngles;
		m_Boris.ResetAll();
		m_Boris.LookAtPlayer();
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_09", string.Empty, 4f));
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.DarkHallwayObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
		SendOnComplete();
	}

	private void HandleHallwayOnEnter(object sender, EventArgs e)
	{
		DOTween.Kill((object)1f, false);
		DOTweenUtil.DOAmbientLightColor(0f, 1f);
		GameManager.Instance.Player.SetSlowed(active: true);
	}

	private void HandleHallwayOnExit(object sender, EventArgs e)
	{
		DOTween.Kill((object)0f, false);
		DOTweenUtil.DOAmbientLightColor(1f, 1f);
		GameManager.Instance.Player.SetSlowed(active: false);
	}

	private void ForceComplete()
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		S13AudioManager.Instance.InvokeEvent("evt_CH3_save_point_02");
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_09", string.Empty, 4f));
		m_HallwayDoor.ForceClose();
		m_OpenedDoor.ForceClose();
		m_ClosedDoor.ForceOpen();
		m_Boris.StopWaypointPathing();
		m_Boris.UpdateWaypointList(new List<WaypointNode>());
		m_Boris.transform.position = m_BorisWarpPosition.position;
		Vector3 eulerAngles = m_BorisWarpPosition.eulerAngles;
		eulerAngles.x = 0f;
		eulerAngles.z = 0f;
		m_Boris.transform.eulerAngles = eulerAngles;
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		m_HallwayTrigger.OnEnter -= HandleHallwayOnEnter;
		m_HallwayTrigger.OnExit -= HandleHallwayOnExit;
		m_OpenedDoor.OnClose -= HandleOpenedDoorOnClosed;
		m_DuctCrawlingClip = null;
		m_HenryDontBeScaredDialogueClip = null;
		m_HenryDarkDialogueClip = null;
		m_FlashlightClip = null;
		m_VentCoverClip = null;
		m_BorisEnterVentClip = null;
		m_HenryDidYouHearThatClip = null;
		m_HenryDeadEndClip = null;
		base.OnDisposed();
	}
}
