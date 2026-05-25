using System;
using System.Collections.Generic;
using Ai;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class CH3ProjectionistTaskController : CH3BaseTaskController
{
	private const int HEART_MAX = 5;

	[Header("<Controllers>")]
	[SerializeField]
	private MeatlyController m_MeatlyController;

	[SerializeField]
	private CH3LiftController m_LiftController;

	[SerializeField]
	private CH3ServiceController m_ServiceController;

	[Header("Hearts")]
	[SerializeField]
	private Interactable m_FirstHeart;

	[SerializeField]
	private List<Interactable> m_Hearts;

	[Header("Projectionist")]
	[SerializeField]
	private WaypointList m_StartWatpointList;

	[SerializeField]
	private List<WaypointList> m_WaypointLists;

	[SerializeField]
	private ProjectionistAi m_Projectionist;

	[SerializeField]
	private EventTrigger m_ProjectionistTrigger;

	[SerializeField]
	private EventTrigger m_LightenTrigger;

	[SerializeField]
	private List<EventTrigger> m_DarkenTriggers;

	[SerializeField]
	private EventTrigger m_SlownessTrigger;

	[Header("Henry Audio")]
	[SerializeField]
	private Interactable m_HenryValve;

	[SerializeField]
	private GameObject m_HenryAudioBlocker;

	private List<Interactable> m_ActiveHearts = new List<Interactable>();

	private CH3MeltingTommyGun m_WeaponFake;

	private AudioClip[] m_MissionStartDialogue;

	private AudioClip m_AliceDialogue01;

	private AudioClip m_AliceDialogue02;

	private AudioClip m_AliceTommyGunFakeClip;

	private AudioClip m_MissionEndClip;

	private AudioClip m_HeartClip;

	private AudioClip m_ValveTurnClip;

	private AudioClip m_ProjectionistScreamClip;

	private AudioClip m_ProjectionistMusic;

	private AudioObject m_MusicObject;

	private Sprite m_ObjectiveSprite;

	private int m_HeartCount;

	private bool m_CanHaveTommyGun = true;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_ObjectiveSprite = GameManager.Instance.AssetManager.GetAsset<Sprite>("UI/ObjectiveIcons/heart_icon");
		m_MissionStartDialogue = GameManager.Instance.GetAudioClips("Audio/DIA/CH3/Alice/TaskProjectionist/");
		m_AliceDialogue01 = GameManager.Instance.GetAudioClip("Audio/DIA/CH3/Alice/ch3_alice_44_shhthereheistheprojectionist");
		m_AliceDialogue02 = GameManager.Instance.GetAudioClip("Audio/DIA/CH3/Alice/ch3_alice_45_besuretostayoutofhislight");
		m_AliceTommyGunFakeClip = GameManager.Instance.GetAudioClip("Audio/DIA/CH3/Alice/ch3_alice_43_projectionistmissionintroE");
		m_MissionEndClip = GameManager.Instance.GetAudioClip("Audio/DIA/CH3/Alice/ch3_alice_44_projectionistmissionendA");
		m_HeartClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_thickinkpickup");
		m_ValveTurnClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Valve_Turn_01");
		m_ProjectionistScreamClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_projectionist_scream");
		m_ProjectionistMusic = GameManager.Instance.GetAudioClip("Audio/MUS/CH3/MUS_CH3_reelfearloop");
		m_WeaponStationController.OnOpenComplete += HandleOnWeaponStationOpen;
		m_WeaponFake = m_WeaponStationController.WeaponStation.m_TommyGunFake;
		if (Object.op_Implicit((Object)(object)m_WeaponFake))
		{
			m_WeaponFake.gameObject.SetActive(false);
		}
		SetWeapon(m_WeaponStationController.WeaponStation.m_TommyGun);
		m_ProjectionistTrigger.SetActive(active: false);
		m_LightenTrigger.SetActive(active: false);
		m_SlownessTrigger.SetActive(active: false);
		m_HenryValve.SetActive(active: false);
		for (int i = 0; i < m_DarkenTriggers.Count; i++)
		{
			m_DarkenTriggers[i].SetActive(active: false);
		}
		m_FirstHeart.SetActive(active: false);
		for (int j = 0; j < m_Hearts.Count; j++)
		{
			m_Hearts[j].SetActive(active: false);
		}
		m_Hearts.Shuffle();
		for (int k = 0; k < m_Hearts.Count; k++)
		{
			if (k < 4)
			{
				m_ActiveHearts.Add(m_Hearts[k]);
			}
			else
			{
				m_Hearts[k].gameObject.SetActive(false);
			}
		}
		if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.IsHenryUnlocked)
		{
			m_HenryAudioBlocker.SetActive(false);
		}
	}

	public override void Activate()
	{
		base.Activate();
		m_ServiceController.Activate();
		m_MeatlyController.Activate();
		m_LiftController.UnlockBasement();
		m_CanHaveTommyGun = CheckTommyGun();
		m_Weapon.gameObject.SetActive(m_CanHaveTommyGun);
		m_WeaponFake.gameObject.SetActive(!m_CanHaveTommyGun);
		m_SlownessTrigger.SetActive(active: true);
		m_SlownessTrigger.OnEnter += HandleSlownessTriggerOnEnter;
		m_SlownessTrigger.OnExit += HandleSlownessTriggerOnExit;
		if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.AliceTasksObjective.IsStarted)
		{
			GoToNextController();
		}
		else if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.HeartTask.Status.IsComplete)
		{
			ForceComplete();
		}
		else if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.HeartTask.Status.IsStarted)
		{
			ForceStart();
		}
		else
		{
			InternalActivate();
		}
	}

	private void GoToNextController()
	{
		for (int i = 0; i < m_ActiveHearts.Count; i++)
		{
			Interactable interactable = m_ActiveHearts[i];
			if (Object.op_Implicit((Object)(object)interactable))
			{
				interactable.Dispose();
			}
		}
		if (Object.op_Implicit((Object)(object)m_Weapon))
		{
			m_Weapon.Dispose();
		}
		if (!GameManager.Instance.GameData.CurrentSaveFile.CH3Data.IsProjectionistKilled)
		{
			ActivateProjectionist();
		}
		SendOnComplete();
	}

	private void InternalActivate()
	{
		m_WeaponStationController.Block();
		for (int i = 0; i < m_MissionStartDialogue.Length; i++)
		{
			AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_MissionStartDialogue[i], SubtitleConstants.DIA_CH3_ALICE_PROJECTIONIST_START[i], isTrimmed: true));
			if (i >= m_MissionStartDialogue.Length - 1)
			{
				audioObject.OnComplete += HandleMissionStartDialogueOnComplete;
			}
		}
		m_Weapon.Interaction.SetActive(active: false);
		m_HenryValve.SetActive(active: true);
		m_HenryValve.OnInteracted += HandleHenryLeverOnInteracted;
	}

	private void ForceStart()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		m_LiftController.GoToFloor(GameManager.Instance.GameData.CurrentSaveFile.CH3Data.LiftFloor);
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 0.1f, (TweenCallback)delegate
		{
			m_LiftController.ForceCloseLift();
		});
		UpdateObjective();
		if (m_CanHaveTommyGun)
		{
			GetWeapon();
		}
		if (!GameManager.Instance.GameData.CurrentSaveFile.CH3Data.IsHenryUnlocked)
		{
			m_HenryValve.SetActive(active: true);
			m_HenryValve.OnInteracted += HandleHenryLeverOnInteracted;
		}
		EnableTask();
	}

	private void ForceComplete()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_RETURN_TO_THE_ANGEL", "OBJECTIVES/CH3_OBJECTIVE_TASK_PROJECTIONIST_COMPLETE_TIP"));
		m_LiftController.GoToFloor(GameManager.Instance.GameData.CurrentSaveFile.CH3Data.LiftFloor);
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 0.1f, (TweenCallback)delegate
		{
			m_LiftController.ForceCloseLift();
		});
		m_BendyController.SetActive(active: true);
		if (m_CanHaveTommyGun)
		{
			GetWeapon();
		}
		for (int num = 0; num < m_ActiveHearts.Count; num++)
		{
			Interactable interactable = m_ActiveHearts[num];
			if (Object.op_Implicit((Object)(object)interactable))
			{
				interactable.Dispose();
			}
		}
		if (!GameManager.Instance.GameData.CurrentSaveFile.CH3Data.IsProjectionistKilled)
		{
			ActivateProjectionist();
		}
		ActivateDropbox();
	}

	private void UpdateObjective()
	{
		ObjectiveDataVO objectiveDataVO = ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_TASK_PROJECTIONIST", "OBJECTIVES/CH3_OBJECTIVE_TASK_PROJECTIONIST_TIP");
		for (int i = 0; i < GameManager.Instance.GameData.CurrentSaveFile.CH3Data.HeartTask.Object.Length; i++)
		{
			if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.HeartTask.Object[i].IsComplete)
			{
				m_HeartCount++;
			}
		}
		objectiveDataVO.AddItemCounter(m_ObjectiveSprite, m_HeartCount);
		GameManager.Instance.UpdateObjective(objectiveDataVO);
	}

	private void GetWeapon()
	{
		if (Object.op_Implicit((Object)(object)m_Weapon))
		{
			PlayerController player = GameManager.Instance.Player;
			player.InactiveWeapon = player.WeaponGameObject;
			player.InactiveWeapon.gameObject.SetActive(false);
			player.WeaponGameObject = m_Weapon.gameObject;
			m_Weapon.SetParentAndAlign(player.WeaponParent);
			m_Weapon.Equip();
			m_Weapon.Interaction.SetActive(active: false);
			m_Weapon.Interaction.Dispose();
		}
	}

	protected override void BeginTask()
	{
		ObjectiveDataVO objectiveDataVO = ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_TASK_PROJECTIONIST", "OBJECTIVES/CH3_OBJECTIVE_TASK_PROJECTIONIST_TIP", 4f);
		objectiveDataVO.AddItemCounter(m_ObjectiveSprite, m_HeartCount);
		GameManager.Instance.ShowObjective(objectiveDataVO);
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.HeartTask.Status.IsStarted = true;
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.LiftFloor = m_LiftController.CurrentFloor.ID;
		GameManager.Instance.GameDataManager.Save();
	}

	private void HandleMissionStartDialogueOnComplete(object sender, EventArgs e)
	{
		(sender as AudioObject).OnComplete -= HandleMissionStartDialogueOnComplete;
		ObjectiveDataVO data = ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_TASK_PROJECTIONIST_START", string.Empty, 4f);
		GameManager.Instance.ShowObjective(data);
		if (m_CanHaveTommyGun)
		{
			m_WeaponStationController.OnOpenComplete += HandleWeaponStationOnOpenComplete;
		}
		else
		{
			m_WeaponFake.SetActive(active: true);
			m_WeaponFake.OnInteracted += HandleGunOnInteracted;
		}
		m_WeaponStationController.Open();
	}

	private void HandleWeaponStationOnOpenComplete(object sender, EventArgs e)
	{
		m_WeaponStationController.OnOpenComplete -= HandleWeaponStationOnOpenComplete;
		m_Weapon.Interaction.OnInteracted += HandleGunOnInteracted;
		m_Weapon.Interaction.SetActive(active: true);
	}

	private bool CheckTommyGun()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.ChoseDevilsPath && !GameManager.Instance.GameData.CurrentSaveFile.CH3Data.HasDied && GameManager.Instance.GameData.CurrentSaveFile.CH3Data.Toy == 3)
		{
			GameManager.Instance.GameData.CurrentSaveFile.CH3Data.HasTommyGun = true;
			return true;
		}
		return false;
	}

	private void HandleOnWeaponStationOpen(object sender, EventArgs e)
	{
		if (m_CanHaveTommyGun)
		{
			m_Weapon.Interaction.SetActive(active: true);
		}
	}

	private void HandleSlownessTriggerOnEnter(object sender, EventArgs e)
	{
		GameManager.Instance.Player.SetJump(active: false);
		GameManager.Instance.Player.SetRun(active: false);
	}

	private void HandleSlownessTriggerOnExit(object sender, EventArgs e)
	{
		GameManager.Instance.Player.SetJump(active: true);
		GameManager.Instance.Player.SetRun(active: true);
	}

	private void HandleHenryLeverOnInteracted(object sender, EventArgs e)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected O, but got Unknown
		m_HenryValve.OnInteracted -= HandleHenryLeverOnInteracted;
		GameManager.Instance.AudioManager.Play(m_ValveTurnClip);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_HenryValve.transform, new Vector3(0f, 0f, 180f), 2f, (RotateMode)3), (Ease)7), (TweenCallback)delegate
		{
			GameManager.Instance.GameData.CurrentSaveFile.CH3Data.IsHenryUnlocked = true;
			GameManager.Instance.GameDataManager.Save();
			S13AudioManager.Instance.InvokeEvent("evt_ch3_secret_flood_empty");
			m_HenryAudioBlocker.SetActive(false);
		});
	}

	private void HandleGunOnInteracted(object sender, EventArgs e)
	{
		(sender as Interactable).OnInteracted -= HandleGunOnInteracted;
		if (!m_CanHaveTommyGun)
		{
			GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_AliceTommyGunFakeClip, "DIACH3/DIA_CH3_ALICE_22"));
		}
		else
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.BLAZING_METAL);
		}
		ObjectiveDataVO objectiveDataVO = ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_TASK_PROJECTIONIST", "OBJECTIVES/CH3_OBJECTIVE_TASK_PROJECTIONIST_TIP", 4f);
		objectiveDataVO.AddItemCounter(m_ObjectiveSprite, 0);
		GameManager.Instance.ShowObjective(objectiveDataVO);
		m_WeaponStationController.Unblock();
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.HeartTask.Status.IsStarted = true;
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.LiftFloor = m_LiftController.CurrentFloor.ID;
		GameManager.Instance.GameDataManager.Save();
		EnableTask();
	}

	private void EnableTask()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.HeartTask.Object[4].IsComplete)
		{
			if (Object.op_Implicit((Object)(object)m_FirstHeart))
			{
				m_FirstHeart.Dispose();
			}
		}
		else if (Object.op_Implicit((Object)(object)m_FirstHeart))
		{
			m_FirstHeart.SetActive(active: true);
			m_FirstHeart.OnInteracted += HandleFirstHeartOnInteracted;
		}
		for (int i = 0; i < m_ActiveHearts.Count; i++)
		{
			Interactable interactable = m_ActiveHearts[i];
			if (Object.op_Implicit((Object)(object)interactable))
			{
				if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.HeartTask.Object[i].ID == i && GameManager.Instance.GameData.CurrentSaveFile.CH3Data.HeartTask.Object[i].IsComplete)
				{
					interactable.Dispose();
					continue;
				}
				interactable.SetActive(active: true);
				interactable.OnInteracted += HandleHeartOnInteracted;
			}
		}
		m_LightenTrigger.SetActive(active: true);
		m_LightenTrigger.OnEnter += HandleRegularLightOnEnter;
		for (int j = 0; j < m_DarkenTriggers.Count; j++)
		{
			m_DarkenTriggers[j].SetActive(active: true);
			m_DarkenTriggers[j].OnEnter += HandleDarknessOnEnter;
		}
		if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.HeartTask.Object[4].IsComplete && m_HeartCount == 1)
		{
			m_ProjectionistTrigger.SetActive(active: true);
			m_ProjectionistTrigger.OnEnter += HandleProjectionistTriggerOnEnter;
		}
		if (m_HeartCount > 0)
		{
			m_WeaponStationController.Close();
			if (!GameManager.Instance.GameData.CurrentSaveFile.CH3Data.IsProjectionistKilled)
			{
				ActivateProjectionist();
			}
		}
		else
		{
			m_ProjectionistTrigger.SetActive(active: true);
			m_ProjectionistTrigger.OnEnter += HandleProjectionistTriggerOnEnter;
		}
	}

	private void HandleProjectionistTriggerOnEnter(object sender, EventArgs e)
	{
		m_ProjectionistTrigger.OnEnter -= HandleProjectionistTriggerOnEnter;
		m_WeaponStationController.Close();
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_AliceDialogue01, "DIACH3/DIA_CH3_ALICE_23", isTrimmed: true));
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_AliceDialogue02, "DIACH3/DIA_CH3_ALICE_24", isTrimmed: true));
		if (!GameManager.Instance.GameData.CurrentSaveFile.CH3Data.IsProjectionistKilled)
		{
			ActivateProjectionist();
		}
	}

	private void ActivateProjectionist()
	{
		m_Projectionist.OnDeath += HandleProjectionistOnDeath;
		m_Projectionist.OnSpotted += HandlePlayerOnSpotted;
		m_Projectionist.gameObject.SetActive(true);
		m_Projectionist.OnWaypointComplete += HandleProjectionistWaypointComplete;
		m_Projectionist.UpdateWaypointList(m_StartWatpointList.Waypoints);
	}

	private void HandlePlayerOnSpotted(object sender, EventArgs e)
	{
		m_Projectionist.OnSpotted -= HandlePlayerOnSpotted;
		if ((Object)(object)m_MusicObject == (Object)null)
		{
			m_MusicObject = GameManager.Instance.AudioManager.Play(m_ProjectionistMusic, AudioObjectType.MUSIC, -1);
		}
		else
		{
			ShortcutExtensions.DOKill((Component)(object)m_MusicObject.AudioSource, false);
			m_MusicObject.AudioSource.volume = GameManager.Instance.PlayerSettings.MusicVolume;
		}
		m_Projectionist.OnRetreat += HandleProjectionistOnRetreat;
	}

	private void HandleProjectionistOnDeath(object sender, EventArgs e)
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		m_Projectionist.OnDeath -= HandleProjectionistOnDeath;
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.IsProjectionistKilled = true;
		GameManager.Instance.GameDataManager.Save();
		if ((Object)(object)m_MusicObject != (Object)null)
		{
			TweenSettingsExtensions.OnComplete<Tweener>(m_MusicObject.AudioSource.DOFade(0f, 2f), (TweenCallback)delegate
			{
				m_MusicObject.Clear();
				m_MusicObject = null;
			});
		}
		GameManager.Instance.AchievementManager.SetAchievement(AchievementName.NORMANS_FATE);
	}

	private void HandleProjectionistOnRetreat(object sender, EventArgs e)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		m_Projectionist.OnRetreat -= HandleProjectionistOnRetreat;
		if ((Object)(object)m_MusicObject != (Object)null)
		{
			TweenSettingsExtensions.OnComplete<Tweener>(m_MusicObject.AudioSource.DOFade(0f, 2f), (TweenCallback)delegate
			{
				m_MusicObject.Clear();
				m_MusicObject = null;
			});
		}
		m_Projectionist.OnSpotted += HandlePlayerOnSpotted;
	}

	private void HandleProjectionistWaypointComplete(object sender, EventArgs e)
	{
		m_Projectionist.OnWaypointComplete -= HandleProjectionistWaypointComplete;
		m_Projectionist.OnWaypointComplete += HandleProjectionistWaypointComplete;
		m_Projectionist.UpdateWaypointList(m_WaypointLists[Random.Range(0, m_WaypointLists.Count)].Waypoints);
	}

	private void HandleFirstHeartOnInteracted(object sender, EventArgs e)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		Interactable interactable = (Interactable)sender;
		interactable.OnInteracted -= HandleHeartOnInteracted;
		interactable.Dispose();
		if (m_HeartCount == 0)
		{
			GameManager.Instance.AudioManager.PlayAtPosition(m_ProjectionistScreamClip, GameManager.Instance.Player.transform.position - GameManager.Instance.Player.transform.forward * 10f);
		}
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.HeartTask.Object[4].IsComplete = true;
		OnHeartCollected();
	}

	private void HandleHeartOnInteracted(object sender, EventArgs e)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		Interactable interactable = (Interactable)sender;
		interactable.OnInteracted -= HandleHeartOnInteracted;
		interactable.Dispose();
		if (m_Projectionist.CurrentThought != AiThought.Die)
		{
			m_Projectionist.SetVectorPoint(interactable.gameObject.transform.position);
			m_Projectionist.SetUseRunForWaypoints(useRun: true);
			m_Projectionist.SetThought(AiThought.MoveToPoint);
		}
		OnHeartCollected();
	}

	private void OnHeartCollected()
	{
		GameManager.Instance.AudioManager.Play(m_HeartClip);
		m_HeartCount++;
		GameManager.Instance.CurrentObjective.ItemCounter++;
		if (m_HeartCount - 1 != 4)
		{
			GameManager.Instance.GameData.CurrentSaveFile.CH3Data.HeartTask.Object[m_HeartCount - 1].IsComplete = true;
		}
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.LiftFloor = m_LiftController.CurrentFloor.ID;
		if (m_HeartCount > 1 && m_HeartCount < 5)
		{
			GameManager.Instance.GameDataManager.Save();
		}
		CheckStatus();
	}

	private void CheckStatus()
	{
		if (m_HeartCount < 5)
		{
			return;
		}
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_MissionEndClip, "DIACH3/DIA_CH3_ALICE_25")).OnComplete += HandleMissionEndDialogueOnComplete;
		for (int i = 0; i < m_ActiveHearts.Count; i++)
		{
			if (Object.op_Implicit((Object)(object)m_ActiveHearts[i]))
			{
				m_ActiveHearts[i].Dispose();
			}
		}
		m_ActiveHearts.Clear();
		CollectAllHearts();
	}

	private void CollectAllHearts()
	{
		GameManager.Instance.AchievementManager.SetAchievement(AchievementName.DARKER_PLACES);
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.HeartTask.Status.IsComplete = true;
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.LiftFloor = m_LiftController.CurrentFloor.ID;
		GameManager.Instance.GameDataManager.Save();
		ActivateDropbox();
	}

	private void HandleMissionEndDialogueOnComplete(object sender, EventArgs e)
	{
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_RETURN_TO_THE_ANGEL", "OBJECTIVES/CH3_OBJECTIVE_TASK_PROJECTIONIST_COMPLETE_TIP", 4f));
		ActivateDropbox();
	}

	private void HandleDarknessOnEnter(object sender, EventArgs e)
	{
		DOTweenUtil.DOAmbientLightColor(0.75f, 0.75f);
	}

	private void HandleRegularLightOnEnter(object sender, EventArgs e)
	{
		DOTweenUtil.DOAmbientLightColor(1f, 0.75f);
	}

	protected override void OnDisposed()
	{
		if ((Object)(object)m_MusicObject != (Object)null)
		{
			m_MusicObject.Clear();
			m_MusicObject = null;
		}
		if (m_ActiveHearts != null)
		{
			m_ActiveHearts.Clear();
			m_ActiveHearts = null;
		}
		m_WeaponFake = null;
		m_MissionStartDialogue = null;
		m_AliceDialogue01 = null;
		m_AliceDialogue02 = null;
		m_AliceTommyGunFakeClip = null;
		m_MissionEndClip = null;
		m_HeartClip = null;
		m_ValveTurnClip = null;
		m_ProjectionistScreamClip = null;
		m_ProjectionistMusic = null;
		m_MusicObject = null;
		m_ObjectiveSprite = null;
		base.OnDisposed();
	}
}
