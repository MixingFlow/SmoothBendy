using System;
using System.Collections;
using Ai;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class CH5Administration : BaseController
{
	[Header("Objective: Open Administration!")]
	[SerializeField]
	private EventTrigger m_MusicTrigger;

	[SerializeField]
	private EventTrigger m_PuzzleTrigger;

	[SerializeField]
	private CH3LeverLight m_Lever;

	[SerializeField]
	private GenericDoorController m_AdminDoor;

	[SerializeField]
	private BaseDoorController[] m_AdminFrontDoors;

	[SerializeField]
	private BaseDoorController[] m_AdminValutDoors;

	[Header("[PIPE PUZZLE]")]
	[Header("Ink Maker")]
	[SerializeField]
	private CH5InkMakerController m_InkMaker;

	[SerializeField]
	private Transform m_InkBlockage;

	[SerializeField]
	private BaseDoorController m_BlockedDoor;

	[SerializeField]
	private Transform m_ThickInkLocation;

	[SerializeField]
	private Holdable m_ThickInkPrefab;

	[Header("Maker Objects")]
	[SerializeField]
	private Interactable m_PipeBasicEmpty;

	[SerializeField]
	private Interactable m_PipeCornerEmpty;

	[SerializeField]
	private Interactable m_PipeThreeWayEmpty;

	[Header("Solid Pipes")]
	[SerializeField]
	private GameObject m_PipeBasicObject;

	[SerializeField]
	private GameObject m_PipeCornerObject;

	[SerializeField]
	private GameObject m_PipeThreeWayObject;

	[Header("Butcher Gang")]
	[SerializeField]
	private GameObject m_DoorBlockers;

	[SerializeField]
	private ButcherGangAi m_Piper;

	[SerializeField]
	private WaypointList m_PiperPath;

	[SerializeField]
	private ButcherGangAi m_Striker;

	[SerializeField]
	private WaypointList m_StrikerPath;

	[SerializeField]
	private ButcherGangAi m_Fisher;

	[SerializeField]
	private WaypointList m_FisherPath;

	[Header("Bendy")]
	[SerializeField]
	private BendySpawnerList m_BendySpawners;

	[SerializeField]
	private BendyAi m_BendyPrefab;

	[Header("TheMeatly")]
	[SerializeField]
	private MeatlyController m_MeatlyController;

	private ButcherGangAi[] m_ButcherGang = new ButcherGangAi[3];

	private AudioClip m_HenryButcherGangClip;

	private AudioClip m_HenryInterestingClip;

	private AudioClip m_ThickInkPickup;

	private AudioClip m_LonelyMusic;

	private AudioClip m_PipePickupClip;

	private AudioClip m_PipePlace1Clip;

	private AudioClip m_PipePlace2Clip;

	private AudioClip m_PipePlace3Clip;

	private AudioObject m_MusicObject;

	private Holdable m_ThickInk;

	private int m_PipePuzzlePiecesNeeded = 3;

	private int m_PipePuzzlePieces;

	private bool m_HasUsedMachine;

	private bool m_HasSeenButcherGang;

	private bool m_WasSeen;

	public BendyAi Bendy { get; private set; }

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_PipePickupClip = GameManager.Instance.GetAudioClip("Audio/SFX/Weapons/GentPipe/SFX_GentPipe_Pickup");
		m_PipePlace1Clip = GameManager.Instance.GetAudioClip("Audio/SFX/Weapons/GentPipe/SFX_GentPipe_Hit_06");
		m_PipePlace2Clip = GameManager.Instance.GetAudioClip("Audio/SFX/Weapons/GentPipe/SFX_GentPipe_Hit_07");
		m_PipePlace3Clip = GameManager.Instance.GetAudioClip("Audio/SFX/Weapons/GentPipe/SFX_GentPipe_Hit_08");
		m_HenryButcherGangClip = GameManager.Instance.GetAudioClip("Audio/DIA/CH5/Henry/DIA_CH5_henry_nottheseguysagainibetterstayoutofsight");
		m_HenryInterestingClip = GameManager.Instance.GetAudioClip("Audio/DIA/CH5/Henry/DIA_CH5_henry_nowthatsinteresting");
		m_ThickInkPickup = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_swollensearcherpop");
		m_LonelyMusic = GameManager.Instance.GetAudioClip("Audio/MUS/CH5/MUS_LonelyAngelClarinetEdition");
		m_Lever.Disable();
		m_MusicTrigger.SetActive(active: false);
		m_PuzzleTrigger.SetActive(active: false);
		m_PipeBasicEmpty.gameObject.SetActive(false);
		m_PipeCornerEmpty.gameObject.SetActive(false);
		m_PipeThreeWayEmpty.gameObject.SetActive(false);
		m_PipeBasicObject.SetActive(GameManager.Instance.GameData.CurrentSaveFile.CH5Data.PipePuzzleBasic.IsComplete);
		m_PipeCornerObject.SetActive(GameManager.Instance.GameData.CurrentSaveFile.CH5Data.PipePuzzleCorner.IsComplete);
		m_PipeThreeWayObject.SetActive(GameManager.Instance.GameData.CurrentSaveFile.CH5Data.PipePuzzleThreeWay.IsComplete);
		m_ButcherGang[0] = m_Piper;
		m_ButcherGang[1] = m_Striker;
		m_ButcherGang[2] = m_Fisher;
		m_BlockedDoor.Lock();
		for (int i = 0; i < m_AdminFrontDoors.Length; i++)
		{
			m_AdminFrontDoors[i].Lock();
		}
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH5Data.AdministrationObjective.IsComplete)
		{
			ForceComplete();
		}
		else if (GameManager.Instance.GameData.CurrentSaveFile.CH5Data.AdministrationObjective.IsStarted)
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
		CheckPipePuzzle();
		m_MusicTrigger.OnEnter += HandleMusicTriggerOnEnter;
		m_MusicTrigger.SetActive(active: true);
		m_PuzzleTrigger.OnEnter += HandlePuzzleTriggerOnEnter;
		m_PuzzleTrigger.SetActive(active: true);
		m_IsActive = true;
	}

	private void ForceStart()
	{
		m_HasSeenButcherGang = true;
		OpenDoors();
		m_MusicObject = GameManager.Instance.AudioManager.Play(m_LonelyMusic, AudioObjectType.MUSIC, 1);
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/CURRENT_OBJECTIVE_HEADER", "OBJECTIVES/CH5_OBJECTIVE_DRAIN_THE_PASSAGE", "OBJECTIVES/CH5_OBJECTIVE_DRAIN_THE_PASSAGE_TIP"));
		CheckPipePuzzle();
		GetThickInk();
		EnableButcherGang();
	}

	private void ForceComplete()
	{
		OpenDoors();
		((Component)m_InkBlockage).gameObject.SetActive(false);
		GetThickInk();
		EnableButcherGang();
		CheckPipePuzzle();
		m_BlockedDoor.ForceOpen(-145f);
		m_MeatlyController.Activate();
		SendOnComplete();
	}

	private void Update()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		if (!m_IsActive || m_HasSeenButcherGang)
		{
			return;
		}
		RaycastHit val2 = default(RaycastHit);
		for (int i = 0; i < m_ButcherGang.Length; i++)
		{
			Vector3 val = m_ButcherGang[i].transform.position + Vector3.up * 3f;
			if (!Physics.Linecast(GameManager.Instance.GameCamera.transform.position, val, ref val2))
			{
				continue;
			}
			ButcherGangAi component = ((Component)((RaycastHit)(ref val2)).collider).GetComponent<ButcherGangAi>();
			if (!Object.op_Implicit((Object)(object)component))
			{
				continue;
			}
			IEnumerator enumerator = ((Component)component.AnimationController).transform.GetEnumerator();
			component.AnimationController.updateMode = (AnimatorUpdateMode)0;
			try
			{
				while (enumerator.MoveNext())
				{
					SkinnedMeshRenderer component2 = ((Component)(Transform)enumerator.Current).GetComponent<SkinnedMeshRenderer>();
					if (Object.op_Implicit((Object)(object)component2) && ((Renderer)component2).isVisible)
					{
						m_HasSeenButcherGang = true;
						GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryButcherGangClip, "DIACH5/DIA_CH5_HENRY_BUTCHER_GANG"));
						return;
					}
				}
			}
			finally
			{
				if (enumerator is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
		}
	}

	private void OpenDoors()
	{
		m_AdminDoor.ForceOpen();
		for (int i = 0; i < m_AdminFrontDoors.Length; i++)
		{
			m_AdminFrontDoors[i].ForceOpen(-145f);
		}
		for (int j = 0; j < m_AdminValutDoors.Length; j++)
		{
			m_AdminValutDoors[j].ForceOpen(145f);
		}
	}

	private void HandleMusicTriggerOnEnter(object sender, EventArgs e)
	{
		m_MusicTrigger.OnEnter -= HandleMusicTriggerOnEnter;
		m_MusicObject = GameManager.Instance.AudioManager.Play(m_LonelyMusic, AudioObjectType.MUSIC, 1);
	}

	private void HandlePuzzleTriggerOnEnter(object sender, EventArgs e)
	{
		m_PuzzleTrigger.OnEnter -= HandlePuzzleTriggerOnEnter;
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryInterestingClip, "DIACH5/DIA_CH5_HENRY_INTERESTING")).OnComplete += HandleHenryInterestingOnComplete;
	}

	private void HandleHenryInterestingOnComplete(object sender, EventArgs e)
	{
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH5_OBJECTIVE_DRAIN_THE_PASSAGE", "OBJECTIVES/CH5_OBJECTIVE_DRAIN_THE_PASSAGE_TIP", 4f));
		for (int i = 0; i < m_AdminFrontDoors.Length; i++)
		{
			m_AdminFrontDoors[i].Unlock();
		}
		m_Lever.OnComplete += HandleLeverOnComplete;
		m_Lever.Activate();
	}

	private void HandleLeverOnComplete(object sender, EventArgs e)
	{
		m_Lever.OnComplete -= HandleLeverOnComplete;
		GetThickInk();
		EnableButcherGang();
		m_AdminDoor.OnOpened += HandleAdminDoorOnOpened;
		m_AdminDoor.Open();
	}

	private void HandleAdminDoorOnOpened(object sender, EventArgs e)
	{
		m_AdminDoor.OnOpened -= HandleAdminDoorOnOpened;
		GameManager.Instance.GameData.CurrentSaveFile.CH5Data.AdministrationObjective.IsStarted = true;
		GameManager.Instance.GameDataManager.Save();
	}

	private void CheckPipePuzzle()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH5Data.PipePuzzleBasic.IsComplete)
		{
			m_PipePuzzlePieces++;
			m_PipeBasicObject.SetActive(true);
		}
		else
		{
			m_PipeBasicEmpty.gameObject.SetActive(true);
		}
		if (GameManager.Instance.GameData.CurrentSaveFile.CH5Data.PipePuzzleCorner.IsComplete)
		{
			m_PipePuzzlePieces++;
			m_PipeCornerObject.SetActive(true);
		}
		else
		{
			m_PipeCornerEmpty.gameObject.SetActive(true);
		}
		if (GameManager.Instance.GameData.CurrentSaveFile.CH5Data.PipePuzzleThreeWay.IsComplete)
		{
			m_PipePuzzlePieces++;
			m_PipeThreeWayObject.SetActive(true);
		}
		else
		{
			m_PipeThreeWayEmpty.gameObject.SetActive(true);
		}
		if (m_PipePuzzlePieces >= m_PipePuzzlePiecesNeeded)
		{
			((Component)m_InkBlockage).gameObject.SetActive(false);
			m_BlockedDoor.Unlock();
		}
	}

	private void CheckBlockage()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		m_PipePuzzlePieces++;
		if (m_PipePuzzlePieces >= m_PipePuzzlePiecesNeeded)
		{
			TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetRelative<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(m_InkBlockage, -15f, 5f, false), (Ease)5)), new TweenCallback(BlockageOnComplete));
			S13AudioManager.Instance.InvokeEvent("evt_film_vault_door_cleared");
		}
		S13AudioManager.Instance.InvokeEvent("evt_ch5_puzzle_piece_added");
	}

	private void BlockageOnComplete()
	{
		((Component)m_InkBlockage).gameObject.SetActive(false);
		if (!m_WasSeen)
		{
			GameManager.Instance.AchievementManager.SetAchievement(AchievementName.VALUED_EMPLOYEE);
		}
		m_BlockedDoor.OnOpen += HandleBlockedDoorOnOpen;
		m_BlockedDoor.Unlock();
	}

	private void HandleBlockedDoorOnOpen(object sender, EventArgs e)
	{
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		m_BlockedDoor.OnOpen -= HandleBlockedDoorOnOpen;
		m_MeatlyController.Activate();
		GameManager.Instance.GameData.CurrentSaveFile.CH5Data.AdministrationObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
		GameManager.Instance.AchievementManager.SetAchievement(AchievementName.PIPES_AND_PROBLEMS);
		if (Object.op_Implicit((Object)(object)m_MusicObject))
		{
			TweenSettingsExtensions.OnComplete<Tweener>(m_MusicObject.AudioSource.DOFade(0f, 2f), (TweenCallback)delegate
			{
				m_MusicObject.Clear();
				m_MusicObject = null;
				SendOnComplete();
			});
		}
		else
		{
			SendOnComplete();
		}
	}

	private void GetThickInk()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		m_ThickInk = Object.Instantiate<Holdable>(m_ThickInkPrefab);
		m_ThickInk.transform.SetParent(m_ThickInkLocation);
		m_ThickInk.transform.localPosition = Vector3.zero;
		m_ThickInk.transform.localEulerAngles = Vector3.zero;
		m_ThickInk.transform.localScale = Vector3.one;
		m_ThickInk.OnInteracted += HandleThickInkOnInteracted;
		m_ThickInk.SetActive(active: true);
	}

	private void HandleThickInkOnInteracted(object sender, EventArgs e)
	{
		m_ThickInk.OnInteracted -= HandleThickInkOnInteracted;
		GameManager.Instance.AudioManager.Play(m_ThickInkPickup);
		m_InkMaker.OnPipeBasicInteracted -= HandlePipeBasicOnInteracted;
		m_InkMaker.OnPipeBasicInteracted += HandlePipeBasicOnInteracted;
		m_InkMaker.OnPipeCornerInteracted -= HandlePipeCornerOnInteracted;
		m_InkMaker.OnPipeCornerInteracted += HandlePipeCornerOnInteracted;
		m_InkMaker.OnPipeThreeWayInteracted -= HandlePipeThreeWayOnInteracted;
		m_InkMaker.OnPipeThreeWayInteracted += HandlePipeThreeWayOnInteracted;
		m_InkMaker.OnTrayInteracted += HandleInkMakerOnTrayInteracted;
		m_InkMaker.OnComplete += HandleInkMakerOnComplete;
		m_InkMaker.Activate();
	}

	private void HandleInkMakerOnTrayInteracted(object sender, EventArgs e)
	{
		m_InkMaker.OnTrayInteracted -= HandleInkMakerOnTrayInteracted;
		m_ThickInk.Remove();
		m_ThickInk.Dispose();
	}

	private void HandleInkMakerOnComplete(object sender, EventArgs e)
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		m_InkMaker.OnComplete -= HandleInkMakerOnComplete;
		m_InkMaker.Deactivate();
		if (!m_HasUsedMachine)
		{
			m_HasUsedMachine = true;
			Bendy = Object.Instantiate<BendyAi>(m_BendyPrefab);
			BendySpawner bendySpawner = m_BendySpawners.BendySpawners[0];
			BendySpawner bendySpawner2 = m_BendySpawners.BendySpawners[1];
			m_BendySpawners.Use();
			Bendy.transform.position = bendySpawner.Watpoints[0].transform.position;
			Bendy.transform.eulerAngles = bendySpawner.Watpoints[0].transform.eulerAngles;
			Bendy.OnWaypointComplete += HandleBendyOnWaypointComplete;
			Bendy.UpdateWaypointList(bendySpawner2.Watpoints);
			Bendy.SetPassive(IsPassive: true);
		}
		GetThickInk();
	}

	private void HandleBendyOnWaypointComplete(object sender, EventArgs e)
	{
		Bendy.OnWaypointComplete -= HandleBendyOnWaypointComplete;
		m_BendySpawners.Reset();
		if (Object.op_Implicit((Object)(object)Bendy))
		{
			Bendy.Dispose();
			Bendy = null;
		}
	}

	private void HandlePipeBasicOnInteracted(object sender, EventArgs e)
	{
		m_InkMaker.OnPipeBasicInteracted -= HandlePipeBasicOnInteracted;
		GameManager.Instance.AudioManager.Play(m_PipePickupClip);
		GameManager.Instance.GameData.CurrentSaveFile.CH5Data.PipePuzzleBasic.IsStarted = true;
		m_PipeBasicEmpty.OnInteracted += HandlePipeBasicEmptyOnInteracted;
		((Component)m_PipeBasicEmpty).GetComponent<Collider>().enabled = true;
	}

	private void HandlePipeBasicEmptyOnInteracted(object sender, EventArgs e)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		m_PipeBasicEmpty.OnInteracted -= HandlePipeBasicEmptyOnInteracted;
		m_PipeBasicEmpty.Dispose();
		GameManager.Instance.AudioManager.PlayAtPosition(m_PipePlace1Clip, m_PipeBasicObject.transform.position);
		GameManager.Instance.GameData.CurrentSaveFile.CH5Data.PipePuzzleBasic.IsComplete = true;
		m_PipeBasicObject.SetActive(true);
		CheckBlockage();
	}

	private void HandlePipeCornerOnInteracted(object sender, EventArgs e)
	{
		m_InkMaker.OnPipeCornerInteracted -= HandlePipeCornerOnInteracted;
		GameManager.Instance.AudioManager.Play(m_PipePickupClip);
		GameManager.Instance.GameData.CurrentSaveFile.CH5Data.PipePuzzleCorner.IsStarted = true;
		m_PipeCornerEmpty.OnInteracted += HandlePipeCornerEmptyOnInteracted;
		((Component)m_PipeCornerEmpty).GetComponent<Collider>().enabled = true;
	}

	private void HandlePipeCornerEmptyOnInteracted(object sender, EventArgs e)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		m_PipeCornerEmpty.OnInteracted -= HandlePipeCornerEmptyOnInteracted;
		m_PipeCornerEmpty.Dispose();
		GameManager.Instance.AudioManager.PlayAtPosition(m_PipePlace2Clip, m_PipeCornerObject.transform.position);
		GameManager.Instance.GameData.CurrentSaveFile.CH5Data.PipePuzzleCorner.IsComplete = true;
		m_PipeCornerObject.SetActive(true);
		CheckBlockage();
	}

	private void HandlePipeThreeWayOnInteracted(object sender, EventArgs e)
	{
		m_InkMaker.OnPipeThreeWayInteracted -= HandlePipeThreeWayOnInteracted;
		GameManager.Instance.AudioManager.Play(m_PipePickupClip);
		GameManager.Instance.GameData.CurrentSaveFile.CH5Data.PipePuzzleThreeWay.IsStarted = true;
		m_PipeThreeWayEmpty.OnInteracted += HandlePipeThreeWayEmptyOnInteracted;
		((Component)m_PipeThreeWayEmpty).GetComponent<Collider>().enabled = true;
	}

	private void HandlePipeThreeWayEmptyOnInteracted(object sender, EventArgs e)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		m_PipeThreeWayEmpty.OnInteracted -= HandlePipeThreeWayEmptyOnInteracted;
		m_PipeThreeWayEmpty.Dispose();
		GameManager.Instance.AudioManager.PlayAtPosition(m_PipePlace3Clip, m_PipeThreeWayObject.transform.position);
		GameManager.Instance.GameData.CurrentSaveFile.CH5Data.PipePuzzleThreeWay.IsComplete = true;
		m_PipeThreeWayObject.SetActive(true);
		CheckBlockage();
	}

	private void EnableButcherGang()
	{
		m_Piper.OnSpotted += HandlePlayerOnSpotted;
		m_Piper.OnRetreat += HandleButcherGangOnRetreat;
		m_Piper.gameObject.SetActive(true);
		m_Piper.UpdateWaypointList(m_PiperPath.Waypoints);
		m_Striker.OnSpotted += HandlePlayerOnSpotted;
		m_Striker.OnRetreat += HandleButcherGangOnRetreat;
		m_Striker.gameObject.SetActive(true);
		m_Striker.UpdateWaypointList(m_StrikerPath.Waypoints);
		m_Fisher.OnSpotted += HandlePlayerOnSpotted;
		m_Fisher.OnRetreat += HandleButcherGangOnRetreat;
		m_Fisher.gameObject.SetActive(true);
		m_Fisher.AnimationController.animatePhysics = false;
		m_Fisher.UpdateWaypointList(m_FisherPath.Waypoints);
		GameManager.Instance.CurrentChapter.DeathController.OnSpawned += HandlePlayerOnDeath;
	}

	private void HandlePlayerOnSpotted(object sender, EventArgs e)
	{
		m_WasSeen = true;
		m_DoorBlockers.SetActive(false);
	}

	private void HandleButcherGangOnRetreat(object sender, EventArgs e)
	{
		m_DoorBlockers.SetActive(true);
	}

	private void HandlePlayerOnDeath(object sender, EventArgs e)
	{
		m_DoorBlockers.SetActive(true);
	}

	protected override void OnDisposed()
	{
		m_ThickInkPickup = null;
		m_ThickInk = null;
		m_LonelyMusic = null;
		m_MusicObject = null;
		base.OnDisposed();
	}
}
