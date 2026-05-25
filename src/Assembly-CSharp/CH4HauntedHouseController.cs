using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using S13Audio;
using UnityEngine;

public class CH4HauntedHouseController : BaseController
{
	[Serializable]
	private class SkullDoors
	{
		public GenericDoorController Door;

		public EventTrigger Trigger;

		public bool HasMusic { get; private set; }

		public void SetMusic()
		{
			HasMusic = true;
		}
	}

	[Serializable]
	private class Popups
	{
		public CH4PopupGhost Ghost;

		public CH4PopupSkeleton Skeleton;

		public EventTrigger Trigger;
	}

	[Header("<Activating Haunted House>")]
	[SerializeField]
	private InteractablePowerLever m_PowerLever;

	[SerializeField]
	private MeshRenderer[] m_PowerCables;

	[SerializeField]
	private OcclusionPortal m_HauntedHouseOcclusionPortal;

	[SerializeField]
	private List<Transform> m_ShuttersLeft;

	[SerializeField]
	private List<Transform> m_ShuttersRight;

	[SerializeField]
	private Transform m_Eyes;

	[SerializeField]
	private Transform m_EyesLeft;

	[SerializeField]
	private Transform m_EyesRight;

	[SerializeField]
	private GameObject m_DarknessBlocker;

	[SerializeField]
	private Transform m_GateLeft;

	[SerializeField]
	private Transform m_GateRight;

	[Header("<Enter Haunted House")]
	[SerializeField]
	private CH4CartRide m_Cart;

	[SerializeField]
	private Transform m_CartWaypoints;

	[SerializeField]
	private List<SkullDoors> m_SkullDoors;

	[SerializeField]
	private List<Popups> m_Popups;

	[Header("Boris")]
	[SerializeField]
	private BruteBorisAi m_Boris;

	[Header("<DEV CHEAT POSITIONS>")]
	[SerializeField]
	private Transform m_CheatPoint;

	private List<Vector3> m_WaypointPositions = new List<Vector3>();

	private Dictionary<EventTrigger, Popups> m_PopupDict = new Dictionary<EventTrigger, Popups>();

	private AudioClip m_LeverClip;

	private AudioClip m_MonsterWaltzClip;

	private AudioObject m_MonsterMusic;

	private AudioClip[] m_AliceHauntedHouseStart;

	private AudioClip[] m_AliceHauntedHouseRide;

	private bool m_HasPopup;

	public override void InitOnComplete()
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Expected O, but got Unknown
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		base.InitOnComplete();
		m_LeverClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Mainr_Power_Lever_Turn_On_01");
		m_MonsterWaltzClip = GameManager.Instance.GetAudioClip("Audio/MUS/CH4/MUS_TheMonsterWaltz");
		m_AliceHauntedHouseStart = GameManager.Instance.GetAudioClips("Audio/DIA/CH4/Alice/HauntedHouse/Start");
		m_AliceHauntedHouseRide = GameManager.Instance.GetAudioClips("Audio/DIA/CH4/Alice/HauntedHouse/Ride");
		m_Cart.SetActive(active: false);
		foreach (Transform cartWaypoint in m_CartWaypoints)
		{
			Transform val = cartWaypoint;
			m_WaypointPositions.Add(val.position);
		}
		for (int i = 0; i < m_Popups.Count; i++)
		{
			Popups popups = m_Popups[i];
			popups.Trigger.SetActive(active: false);
			m_PopupDict.Add(popups.Trigger, popups);
		}
		for (int j = 0; j < m_SkullDoors.Count; j++)
		{
			m_SkullDoors[j].Trigger.SetActive(active: false);
		}
		m_PowerLever.SetActive(active: false);
	}

	public override void Activate()
	{
		m_PowerLever.OnInteracted += HandlePowerLeverOnInteracted;
		m_PowerLever.OnComplete += HandlePowerLeverOnComplete;
		m_PowerLever.SetActive(active: true);
	}

	private void HandlePowerLeverOnInteracted(object sender, EventArgs e)
	{
		m_PowerLever.OnInteracted -= HandlePowerLeverOnInteracted;
		GameManager.Instance.AudioManager.Play(m_LeverClip);
	}

	private void HandlePowerLeverOnComplete(object sender, EventArgs e)
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Expected O, but got Unknown
		m_PowerLever.OnComplete -= HandlePowerLeverOnComplete;
		for (int i = 0; i < m_PowerCables.Length; i++)
		{
			((Renderer)m_PowerCables[i]).material.SetFloat("_Shimmer", 0f);
		}
		m_DarknessBlocker.SetActive(false);
		m_HauntedHouseOcclusionPortal.open = true;
		for (int j = 0; j < m_ShuttersLeft.Count; j++)
		{
			Transform val = m_ShuttersLeft[j];
			TweenSettingsExtensions.SetEase<Tweener>(TweenSettingsExtensions.SetDelay<Tweener>(ShortcutExtensions.DOLocalRotate(val, new Vector3(0f, 180f, 0f), 1f, (RotateMode)3), 3f), (Ease)30);
		}
		for (int k = 0; k < m_ShuttersRight.Count; k++)
		{
			Transform val2 = m_ShuttersRight[k];
			TweenSettingsExtensions.SetEase<Tweener>(TweenSettingsExtensions.SetDelay<Tweener>(ShortcutExtensions.DOLocalRotate(val2, new Vector3(0f, -180f, 0f), 1f, (RotateMode)3), 3f), (Ease)30);
		}
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveZ(m_GateLeft, 12.6f, 10f, false), (Ease)7);
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveZ(m_GateRight, -12f, 10f, false), (Ease)7);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_Eyes, m_EyesLeft.localPosition, 1f, false), (Ease)7), new TweenCallback(DOEyeLookYoYo));
		m_Cart.OnEnter += HandleCartOnEnter;
		m_Cart.SetActive(active: true);
		GameManager.Instance.AchievementManager.SetAchievement(AchievementName.A_HAUNTING_WE_WILL_GO);
		S13AudioManager.Instance.InvokeEvent("evt_haunted_house_start");
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH4_OBJECTIVE_RESCUE_BORIS", string.Empty, 4f));
		GameManager.Instance.GameData.CurrentSaveFile.CH4Data.HauntedHouseObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
	}

	private void HandleCartOnEnter(object sender, EventArgs e)
	{
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Expected O, but got Unknown
		m_Cart.OnEnter -= HandleCartOnEnter;
		GameManager.Instance.AiGlobalNetwork.ClearAndDisposeSoundAi();
		for (int i = 0; i < m_SkullDoors.Count; i++)
		{
			SkullDoors skullDoors = m_SkullDoors[i];
			skullDoors.Trigger.OnEnter += HandleSkullDoorTriggerOnEnter;
			skullDoors.Trigger.OnExit += HandleSkullDoorTriggerOnExit;
			skullDoors.Trigger.SetActive(active: true);
			if (i == m_SkullDoors.Count - 2)
			{
				skullDoors.SetMusic();
			}
		}
		for (int j = 0; j < m_Popups.Count; j++)
		{
			Popups popups = m_Popups[j];
			popups.Trigger.OnEnter += HandlePopupTriggerOnEnter;
			popups.Trigger.SetActive(active: true);
		}
		for (int k = 0; k < m_AliceHauntedHouseStart.Length; k++)
		{
			GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_AliceHauntedHouseStart[k], SubtitleConstants.DIALOBUE_CH4_ALICE_HAUNTED_HOUSE_START[k], isTrimmed: true));
		}
		S13AudioManager.Instance.InvokeEvent("evt_haunted_house_cart_start");
		TweenSettingsExtensions.OnComplete<TweenerCore<Vector3, Path, PathOptions>>(TweenSettingsExtensions.SetEase<TweenerCore<Vector3, Path, PathOptions>>(TweenSettingsExtensions.SetLookAt(ShortcutExtensions.DOPath(m_Cart.transform, m_WaypointPositions.ToArray(), 120f, (PathType)1, (PathMode)1, 10, (Color?)null), 0.01f, (Vector3?)base.transform.forward, (Vector3?)null), (Ease)1), new TweenCallback(CartRideOnComplete));
	}

	private void HandlePopupTriggerOnEnter(object sender, EventArgs e)
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		EventTrigger eventTrigger = sender as EventTrigger;
		eventTrigger.OnEnter -= HandlePopupTriggerOnEnter;
		Popups popups = m_PopupDict[eventTrigger];
		if (Object.op_Implicit((Object)(object)popups.Skeleton))
		{
			popups.Skeleton.Activate();
		}
		if (Object.op_Implicit((Object)(object)popups.Ghost))
		{
			popups.Ghost.Activate();
		}
		if (m_HasPopup)
		{
			return;
		}
		m_HasPopup = true;
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 5f, (TweenCallback)delegate
		{
			for (int i = 0; i < m_AliceHauntedHouseRide.Length; i++)
			{
				GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_AliceHauntedHouseRide[i], SubtitleConstants.DIALOBUE_CH4_ALICE_HAUNTED_HOUSE_RIDE[i], isTrimmed: true));
			}
		});
	}

	private void HandleSkullDoorTriggerOnEnter(object sender, EventArgs e)
	{
		EventTrigger eventTrigger = sender as EventTrigger;
		eventTrigger.OnEnter -= HandleSkullDoorTriggerOnEnter;
		SkullDoors skullDoors = null;
		for (int i = 0; i < m_SkullDoors.Count; i++)
		{
			SkullDoors skullDoors2 = m_SkullDoors[i];
			if ((Object)(object)skullDoors2.Trigger == (Object)(object)eventTrigger)
			{
				skullDoors = skullDoors2;
				break;
			}
		}
		if (skullDoors != null)
		{
			if (skullDoors.HasMusic)
			{
				m_MonsterMusic = GameManager.Instance.AudioManager.Play(m_MonsterWaltzClip, AudioObjectType.MUSIC);
			}
			skullDoors.Door.Open();
			S13AudioSource componentInChildren = ((Component)skullDoors.Door).GetComponentInChildren<S13AudioSource>();
			if ((Object)(object)componentInChildren != (Object)null)
			{
				componentInChildren.Play();
			}
		}
	}

	private void HandleSkullDoorTriggerOnExit(object sender, EventArgs e)
	{
		EventTrigger eventTrigger = sender as EventTrigger;
		eventTrigger.OnExit -= HandleSkullDoorTriggerOnExit;
		SkullDoors skullDoors = null;
		for (int i = 0; i < m_SkullDoors.Count; i++)
		{
			SkullDoors skullDoors2 = m_SkullDoors[i];
			if ((Object)(object)skullDoors2.Trigger == (Object)(object)eventTrigger)
			{
				skullDoors = skullDoors2;
				break;
			}
		}
		skullDoors?.Door.Close();
	}

	private void CartRideOnComplete()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		m_Cart.transform.SetParent(m_Boris.CartParent);
		TweenSettingsExtensions.OnComplete<Tweener>(m_MonsterMusic.AudioSource.DOFade(0f, 1f), (TweenCallback)delegate
		{
			if (Object.op_Implicit((Object)(object)m_MonsterMusic))
			{
				m_MonsterMusic.Clear();
				m_MonsterMusic.AudioSource.volume = 1f;
				m_MonsterMusic = null;
			}
		});
		S13AudioManager.Instance.InvokeEvent("evt_haunted_house_cart_stop");
		SendOnComplete();
	}

	private void DOEyeLookYoYo()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_Eyes, m_EyesRight.localPosition, 2f, false), (Ease)7), -1, (LoopType)1);
	}

	protected override void OnDisposed()
	{
		if (Object.op_Implicit((Object)(object)m_PowerLever))
		{
			m_PowerLever.OnInteracted -= HandlePowerLeverOnInteracted;
		}
		m_WaypointPositions.Clear();
		m_WaypointPositions = null;
		m_PopupDict.Clear();
		m_PopupDict = null;
		m_LeverClip = null;
		m_MonsterWaltzClip = null;
		m_MonsterMusic = null;
		m_AliceHauntedHouseStart = null;
		m_AliceHauntedHouseRide = null;
		base.OnDisposed();
	}
}
