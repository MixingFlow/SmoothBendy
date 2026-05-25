using System;
using Ai;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class CH4MaintenanceController : BaseController
{
	[Header("[Power Station]")]
	[SerializeField]
	private CH4WarehousePowerStation m_PowerStation;

	[Header("Objective: Maintenance Power")]
	[SerializeField]
	private CH3LeverLight m_MaintenanceLever;

	[SerializeField]
	private EventTrigger m_MaintenanceEnterTrigger;

	[SerializeField]
	private GenericDoorController m_MaintenanceDoor;

	[SerializeField]
	private ProjectionistAi m_Projectionist;

	[SerializeField]
	private WaypointList m_StartWatpointList;

	[SerializeField]
	private WaypointList m_DownstairsWaypoints;

	[Header("Fight")]
	[SerializeField]
	private EventTrigger m_ForceProjectionistChase;

	[SerializeField]
	private LittleMiracleStationController m_LMS;

	[SerializeField]
	private GenericDoorController m_HatchDoor;

	[SerializeField]
	private GameObject m_BrokenHatchDoor;

	[SerializeField]
	private CH4ProjectionistBendyFight m_BendyProjFight;

	[SerializeField]
	private ProjectionistAi m_ChaseProjectionist;

	[Header("<Pallet Lift>")]
	[SerializeField]
	private InteractablePowerLever m_PalletLiftLever;

	[SerializeField]
	private Transform m_PalletLift;

	[SerializeField]
	private Transform m_PalletLoweredLocation;

	[SerializeField]
	private Transform m_PalletRaisedLocation;

	[Header("Other")]
	[SerializeField]
	private InteractableTrunk m_Trunk;

	[SerializeField]
	private Interactable m_Disc;

	[SerializeField]
	private Transform m_ProjectionistStandPosition;

	[SerializeField]
	private Interactable m_InteractableHearts;

	[Header("<DEV CHEAT POSITIONS>")]
	[SerializeField]
	private Transform m_CheatPoint;

	private WaypointList m_ActiveWaypoints;

	private AudioClip m_LightClip;

	private AudioClip m_LeverClip;

	private AudioClip m_HeartClip;

	private AudioObject m_MusicObject;

	private AudioClip m_MusicClip;

	private AudioClip m_EntranceMusicClip;

	private bool m_MusicIsPlaying;

	private Vector3 m_ChaseProjectionistStartPosition;

	private Quaternion m_ChaseProjectionistStartRotation;

	private bool m_HasInteractedWithHearts;

	public override void Init()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		base.Init();
		m_PalletLift.position = m_PalletLoweredLocation.position;
		m_Projectionist.SetThought(AiThought.Inactive);
	}

	public override void InitOnComplete()
	{
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		base.InitOnComplete();
		m_HeartClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_thickinkpickup");
		m_LightClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Light_Switch_Sammys_Room_01");
		m_LeverClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Mainr_Power_Lever_Turn_On_01");
		m_MusicClip = GameManager.Instance.GetAudioClip("Audio/MUS/CH3/MUS_CH3_reelfearloop");
		m_EntranceMusicClip = GameManager.Instance.GetAudioClip("Audio/MUS/CH4/MUS_OldLightHead");
		m_MaintenanceLever.Disable();
		m_MaintenanceEnterTrigger.SetActive(active: false);
		m_ForceProjectionistChase.SetActive(active: false);
		m_BrokenHatchDoor.SetActive(false);
		m_HatchDoor.ForceOpen();
		m_Trunk.SetActive(active: false);
		m_Disc.SetActive(active: false);
		m_InteractableHearts.SetActive(active: false);
		m_ChaseProjectionistStartPosition = m_ChaseProjectionist.transform.position;
		m_ChaseProjectionistStartRotation = m_ChaseProjectionist.transform.rotation;
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH4Data.MaintenanceObjective.IsComplete)
		{
			ForceComplete();
			return;
		}
		if (GameManager.Instance.GameData.CurrentSaveFile.CH4Data.MaintenanceObjective.IsStarted)
		{
			ForceStart();
			return;
		}
		m_Trunk.SetActive(active: false);
		m_Disc.SetActive(active: false);
		m_MaintenanceDoor.Open();
		GameManager.Instance.CurrentChapter.DeathController.OnSpawned += HandlePlayerOnSpawned;
		m_MaintenanceEnterTrigger.OnEnter += HandleMaintenanceEnterTriggerOnEnter;
		m_MaintenanceEnterTrigger.SetActive(active: true);
	}

	private void ForceStart()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		m_Trunk.SetActive(active: true);
		m_Disc.OnInteracted += HandleDiscOnInteracted;
		m_Disc.SetActive(active: true);
		m_MaintenanceLever.ForceComplete();
		m_PalletLiftLever.ForceOpen();
		m_PalletLift.position = m_PalletRaisedLocation.position;
		m_MaintenanceDoor.ForceOpen();
		CheckInternecion();
	}

	private void ForceComplete()
	{
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		S13AudioManager.Instance.InvokeEvent("evt_CH4_save_point_11");
		if (Object.op_Implicit((Object)(object)m_Projectionist))
		{
			m_Projectionist.Dispose();
		}
		if (Object.op_Implicit((Object)(object)m_ChaseProjectionist))
		{
			m_ChaseProjectionist.Dispose();
		}
		if (GameManager.Instance.GameData.CurrentSaveFile.CH4Data.HasPlunger)
		{
			m_Disc.Dispose();
		}
		else
		{
			m_Disc.OnInteracted += HandleDiscOnInteracted;
			m_Disc.SetActive(active: true);
		}
		if (GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionBools[4] == GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionValues[4] * 414)
		{
			m_HatchDoor.ForceOpen();
		}
		else
		{
			m_BendyProjFight.ForceComplete();
			m_BrokenHatchDoor.SetActive(true);
			m_HatchDoor.gameObject.SetActive(false);
		}
		m_MaintenanceLever.ForceComplete();
		m_PalletLiftLever.ForceOpen();
		m_PalletLift.position = m_PalletRaisedLocation.position;
		m_Trunk.SetActive(active: true);
		m_Disc.OnInteracted += HandleDiscOnInteracted;
		m_Disc.SetActive(active: true);
		m_MaintenanceDoor.ForceOpen();
		m_PowerStation.ForceActivatePower();
		SendOnComplete();
	}

	private void HandleMaintenanceEnterTriggerOnEnter(object sender, EventArgs e)
	{
		m_MaintenanceEnterTrigger.OnEnter -= HandleMaintenanceEnterTriggerOnEnter;
		m_HatchDoor.ForceClose();
		m_MaintenanceDoor.Close();
		m_Projectionist.SetThought(AiThought.UseWaypoints);
		ActivateProjectionist();
		GameManager.Instance.AudioManager.Play(m_EntranceMusicClip, AudioObjectType.MUSIC);
		m_LMS.OnInteract += HandleLMSPreFightInteract;
		m_InteractableHearts.OnInteracted += HandleInteractableHeartsOnInteracted;
		m_InteractableHearts.SetActive(active: true);
		m_PalletLiftLever.OnInteracted += HandleOnLiftLeverActivated;
		m_PalletLiftLever.SetActive(active: true);
	}

	private void HandleInteractableHeartsOnInteracted(object sender, EventArgs e)
	{
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Expected O, but got Unknown
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (!m_HasInteractedWithHearts)
		{
			m_HasInteractedWithHearts = true;
			GameManager.Instance.AudioManager.Play(m_HeartClip);
			if (Object.op_Implicit((Object)(object)m_Projectionist) && m_Projectionist.CurrentThought != AiThought.Die)
			{
				m_Projectionist.SetVectorPoint(GameManager.Instance.Player.transform.position);
				m_Projectionist.SetUseRunForWaypoints(useRun: true);
				m_Projectionist.SetThought(AiThought.MoveToPoint);
			}
			TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 2f, (TweenCallback)delegate
			{
				m_HasInteractedWithHearts = false;
			});
		}
	}

	private void HandleOnLiftLeverActivated(object sender, EventArgs e)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		m_PalletLiftLever.OnInteracted -= HandleOnLiftLeverActivated;
		GameManager.Instance.AudioManager.Play(m_LeverClip);
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveY(m_PalletLift, m_PalletRaisedLocation.position.y, 3f, false), (Ease)7);
		m_ActiveWaypoints = m_StartWatpointList;
		m_Projectionist.UpdateWaypointList(m_ActiveWaypoints.Waypoints, SetThoughtToWaypoint: false);
		if (!Object.op_Implicit((Object)(object)m_Projectionist.CurrentTarget))
		{
			m_Projectionist.SetTarget(GameManager.Instance.Player.transform);
			m_Projectionist.ForceOnSpotted();
		}
		m_MaintenanceLever.OnComplete += HandleMaintenanceLeverOnComplete;
		m_MaintenanceLever.Activate();
	}

	private void HandleMaintenanceLeverOnComplete(object sender, EventArgs e)
	{
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Expected O, but got Unknown
		m_MaintenanceLever.OnComplete -= HandleMaintenanceLeverOnComplete;
		if (Object.op_Implicit((Object)(object)m_Projectionist))
		{
			m_Projectionist.Dispose();
		}
		StopMusic();
		Sequence val = DOTween.Sequence();
		float num = 0f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(DOTweenUtil.DOAmbientLightColor(6f, 0.2f), (Ease)1));
		num += 0.2f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(DOTweenUtil.DOAmbientLightColor(0f, 0.01f), (Ease)1));
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			GameManager.Instance.AudioManager.Play(m_LightClip);
		});
		num += 2f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			TweenSettingsExtensions.SetEase<Tweener>(DOTweenUtil.DOAmbientLightColor(1f, 4.5f), (Ease)1);
		});
		if (GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionBools[4] == GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionValues[4] * 414)
		{
			m_HatchDoor.ForceOpen();
			m_MaintenanceDoor.Open();
			ActivatePowerStation();
		}
		else
		{
			m_ForceProjectionistChase.OnEnter += HandleOnChaseTriggerEnter;
			m_ForceProjectionistChase.SetActive(active: true);
		}
	}

	private void CheckInternecion()
	{
		if (Object.op_Implicit((Object)(object)m_Projectionist))
		{
			m_Projectionist.Dispose();
		}
		if (Object.op_Implicit((Object)(object)m_ChaseProjectionist))
		{
			m_ChaseProjectionist.Dispose();
		}
		if (GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionBools[4] == GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionValues[4] * 414)
		{
			m_HatchDoor.ForceOpen();
			m_MaintenanceDoor.Open();
			ActivatePowerStation();
		}
		else
		{
			m_BendyProjFight.ForceComplete();
			m_BrokenHatchDoor.SetActive(true);
			m_HatchDoor.gameObject.SetActive(false);
			ActivatePowerStation();
		}
	}

	private void ActivatePowerStation()
	{
		GameManager.Instance.GameData.CurrentSaveFile.CH4Data.MaintenanceObjective.IsStarted = true;
		GameManager.Instance.GameDataManager.Save();
		m_PowerStation.OnPowerActivated += HandlePowerStationOnPowerActivated;
		m_PowerStation.ActivatePower();
	}

	private void ActivateProjectionist()
	{
		m_Projectionist.gameObject.SetActive(true);
		m_ActiveWaypoints = m_DownstairsWaypoints;
		m_Projectionist.OnRetreat += HandleProjectionistOnRetreat;
		m_Projectionist.OnSpotted += HandleProjectionstOnSpotted;
		m_Projectionist.OnWaypointComplete += HandleProjectionistWaypointComplete;
		m_Projectionist.OnDeath += HandleProjectionistOnDeath;
		m_Projectionist.UpdateWaypointList(m_ActiveWaypoints.Waypoints);
	}

	private void HandleProjectionstOnSpotted(object sender, EventArgs e)
	{
		if (!m_MusicIsPlaying)
		{
			if (Object.op_Implicit((Object)(object)m_MusicObject))
			{
				ShortcutExtensions.DOKill((Component)(object)m_MusicObject.AudioSource, false);
				m_MusicObject.Clear();
				m_MusicObject = null;
			}
			m_MusicObject = GameManager.Instance.AudioManager.Play(m_MusicClip, AudioObjectType.MUSIC, -1);
			m_MusicIsPlaying = true;
		}
	}

	private void HandleProjectionistOnRetreat(object sender, EventArgs e)
	{
		StopMusic();
	}

	private void HandleProjectionistOnDeath(object sender, EventArgs e)
	{
		StopMusic();
		m_Projectionist.OnRetreat -= HandleProjectionistOnRetreat;
		m_Projectionist.OnSpotted -= HandleProjectionstOnSpotted;
		m_Projectionist.OnWaypointComplete -= HandleProjectionistWaypointComplete;
		m_Projectionist.OnSpotted -= HandleProjectionistOnDeath;
		if (GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionBools[4] != -1)
		{
			GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionBools[4] = GameManager.Instance.GameData.CurrentSaveFile.CH4Data.InternecionValues[4] * 414;
			GameManager.Instance.GameDataManager.Save(isObjectiveDataOnly: true, shouldShowSaveIndicator: false);
		}
	}

	private void HandleProjectionistWaypointComplete(object sender, EventArgs e)
	{
		m_Projectionist.OnWaypointComplete -= HandleProjectionistWaypointComplete;
		m_Projectionist.OnWaypointComplete += HandleProjectionistWaypointComplete;
		m_Projectionist.UpdateWaypointList(m_ActiveWaypoints.Waypoints);
	}

	private void HandleOnChaseTriggerEnter(object sender, EventArgs e)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		m_ForceProjectionistChase.OnEnter -= HandleMaintenanceEnterTriggerOnEnter;
		m_ForceProjectionistChase.SetActive(active: false);
		m_ChaseProjectionist.gameObject.SetActive(true);
		m_ChaseProjectionist.OnSpotted += HandleProjectionstOnSpotted;
		m_ChaseProjectionist.transform.position = m_ChaseProjectionistStartPosition;
		m_ChaseProjectionist.Reset();
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 0.1f, (TweenCallback)delegate
		{
			m_ChaseProjectionist.SetTarget(GameManager.Instance.Player.transform);
			m_ChaseProjectionist.ForceOnSpotted();
			m_ChaseProjectionist.SetThought(AiThought.Attack);
		});
		m_LMS.OnInteract -= HandleLMSPreFightInteract;
		m_LMS.OnInteract += HandleLMSOnInteract;
	}

	private void HandleLMSPreFightInteract(object sender, EventArgs e)
	{
		StopMusic();
	}

	private void HandleLMSOnInteract(object sender, EventArgs e)
	{
		m_LMS.OnInteract -= HandleLMSOnInteract;
		GameManager.Instance.Player.SetInteraction(active: false);
		StopMusic();
		m_Trunk.SetActive(active: true);
		m_Disc.OnInteracted += HandleDiscOnInteracted;
		m_Disc.SetActive(active: true);
		m_ChaseProjectionist.gameObject.SetActive(false);
		m_BendyProjFight.OnComplete += HandleBendyProjFightOnComplete;
		m_BendyProjFight.Activate();
	}

	private void HandleBendyProjFightOnComplete(object sender, EventArgs e)
	{
		m_BendyProjFight.OnComplete -= HandleBendyProjFightOnComplete;
		GameManager.Instance.Player.SetInteraction(active: true);
		m_BrokenHatchDoor.SetActive(true);
		m_HatchDoor.gameObject.SetActive(false);
		m_MaintenanceDoor.Open();
		ActivatePowerStation();
	}

	private void HandleDiscOnInteracted(object sender, EventArgs e)
	{
		m_Disc.OnInteracted -= HandleDiscOnInteracted;
		m_Disc.Dispose();
		GameManager.Instance.GameData.CurrentSaveFile.CH4Data.HasPlunger = true;
	}

	private void HandlePowerStationOnPowerActivated(object sender, EventArgs e)
	{
		m_PowerStation.OnPowerActivated -= HandlePowerStationOnPowerActivated;
		GameManager.Instance.CurrentChapter.DeathController.OnSpawned -= HandlePlayerOnSpawned;
		GameManager.Instance.GameData.CurrentSaveFile.CH4Data.MaintenanceObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
		SendOnComplete();
	}

	private void HandlePlayerOnSpawned(object sender, EventArgs e)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		StopMusic();
		if (Object.op_Implicit((Object)(object)m_Projectionist))
		{
			m_Projectionist.transform.position = m_ActiveWaypoints.Waypoints[0].transform.position;
			m_Projectionist.OnWaypointComplete -= HandleProjectionistWaypointComplete;
			m_Projectionist.OnWaypointComplete += HandleProjectionistWaypointComplete;
			m_Projectionist.UpdateWaypointList(m_ActiveWaypoints.Waypoints);
		}
		if (((Behaviour)m_ChaseProjectionist).isActiveAndEnabled)
		{
			m_ChaseProjectionist.transform.position = m_ProjectionistStandPosition.position;
			m_ChaseProjectionist.Reset();
			m_ChaseProjectionist.transform.rotation = m_ChaseProjectionistStartRotation;
			m_ForceProjectionistChase.OnEnter += HandleOnChaseTriggerEnter;
			m_ForceProjectionistChase.ResetTrigger();
			m_ForceProjectionistChase.SetActive(active: true);
		}
	}

	private void StopMusic()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		m_MusicIsPlaying = false;
		if (!Object.op_Implicit((Object)(object)m_MusicObject))
		{
			return;
		}
		ShortcutExtensions.DOKill((Component)(object)m_MusicObject.AudioSource, false);
		TweenSettingsExtensions.OnComplete<Tweener>(m_MusicObject.AudioSource.DOFade(0f, 2f), (TweenCallback)delegate
		{
			if (Object.op_Implicit((Object)(object)m_MusicObject))
			{
				m_MusicObject.Clear();
				m_MusicObject = null;
			}
		});
	}

	protected override void OnDisposed()
	{
		if (Object.op_Implicit((Object)(object)m_MaintenanceLever))
		{
			m_MaintenanceLever.OnComplete -= HandleMaintenanceLeverOnComplete;
		}
		GameManager.Instance.CurrentChapter.DeathController.OnSpawned -= HandlePlayerOnSpawned;
		m_HeartClip = null;
		m_LightClip = null;
		m_LeverClip = null;
		m_MusicClip = null;
		m_MusicObject = null;
		base.OnDisposed();
	}
}
