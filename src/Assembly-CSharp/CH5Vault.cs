using System;
using System.Collections.Generic;
using Ai;
using DG.Tweening;
using UnityEngine;

public class CH5Vault : BaseController
{
	[SerializeField]
	private CH5CardboardBox m_Box;

	[SerializeField]
	private Transform m_BoxLookLocation;

	[SerializeField]
	private Transform m_AllyLookLocation;

	[SerializeField]
	private BaseDoorController[] m_EntranceDoors;

	[SerializeField]
	private CH5VaultDoor m_VaultDoor;

	[Header("Allies")]
	[SerializeField]
	private AllyAiController m_Allison;

	[SerializeField]
	private AllyAiController m_Tom;

	[SerializeField]
	private GameObject[] m_AllyWeapons;

	[SerializeField]
	private WaypointNode m_AllisonWaypoint;

	[SerializeField]
	private WaypointNode m_TomWaypoint;

	[Header("Extras")]
	[SerializeField]
	private GameObject m_ButcherGang;

	[SerializeField]
	private EventTrigger m_OldDeskTrigger;

	[SerializeField]
	private EventTrigger m_BendyTrigger;

	[SerializeField]
	private CH5BendyScare m_BendyScare;

	private AudioClip m_HenryClip11;

	private AudioClip m_DoorSmashClip;

	private AudioClip[] m_VaultClips;

	public override void Init()
	{
		base.Init();
		m_Allison.gameObject.SetActive(false);
		m_Tom.gameObject.SetActive(false);
	}

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_OldDeskTrigger.SetActive(active: false);
		m_BendyTrigger.SetActive(active: false);
		for (int i = 0; i < m_AllyWeapons.Length; i++)
		{
			m_AllyWeapons[i].SetActive(false);
		}
		m_DoorSmashClip = GameManager.Instance.GetAudioClip("Audio/SFX/sfx_doorsmash");
		m_HenryClip11 = GameManager.Instance.GetAudioClip("Audio/DIA/CH1/Henry/DIA_CH1_HENRY_11");
		m_VaultClips = GameManager.Instance.GetAudioClips("Audio/DIA/CH5/AliceA/Vault");
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH5Data.VaultObjective.IsComplete)
		{
			ForceComplete();
		}
		else if (GameManager.Instance.GameData.CurrentSaveFile.CH5Data.VaultObjective.IsStarted)
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
		m_Box.OnInteracted += HandleInteractableOnInteracted;
		m_Box.Activate();
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH5_OBJECTIVE_SEARCH_THE_VAULT", string.Empty, 4f));
	}

	private void ForceStart()
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Expected O, but got Unknown
		m_ButcherGang.SetActive(false);
		EnableTriggers();
		m_VaultDoor.ForceOpen();
		for (int i = 0; i < m_EntranceDoors.Length; i++)
		{
			m_EntranceDoors[i].ForceClose();
			m_EntranceDoors[i].Lock();
		}
		m_Allison.transform.position = m_AllisonWaypoint.transform.position;
		m_Allison.transform.eulerAngles = m_AllisonWaypoint.transform.eulerAngles;
		m_Allison.gameObject.SetActive(true);
		m_Allison.ExitCombat();
		m_Tom.transform.position = m_TomWaypoint.transform.position;
		m_Tom.transform.eulerAngles = m_TomWaypoint.transform.eulerAngles;
		m_Tom.gameObject.SetActive(true);
		m_Tom.ExitCombat();
		m_Tom.ForceStartIdle();
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 0.05f, (TweenCallback)delegate
		{
			m_Tom.SetThought(AiThought.Idle);
			m_Tom.SetTarget(GameManager.Instance.Player.transform);
			m_Allison.SetThought(AiThought.Idle);
			m_Allison.SetTarget(GameManager.Instance.Player.transform);
		});
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/CURRENT_OBJECTIVE_HEADER", "OBJECTIVES/CH5_OBJECTIVE_SEARCH_THE_VAULT", string.Empty));
	}

	private void ForceComplete()
	{
		m_ButcherGang.SetActive(false);
		m_VaultDoor.ForceOpen();
		for (int i = 0; i < m_EntranceDoors.Length; i++)
		{
			m_EntranceDoors[i].ForceClose();
			m_EntranceDoors[i].Lock();
		}
		SendOnComplete();
	}

	private void EnableTriggers()
	{
		m_OldDeskTrigger.OnEnter += HandleOldDeskTriggerOnEnter;
		m_OldDeskTrigger.SetActive(active: true);
		m_BendyTrigger.OnEnter += HandleBendyTriggerOnEnter;
		m_BendyTrigger.SetActive(active: true);
	}

	private void HandleBendyTriggerOnEnter(object sender, EventArgs e)
	{
		m_BendyTrigger.OnEnter -= HandleBendyTriggerOnEnter;
		m_BendyScare.SpawnBendy();
		GameManager.Instance.GameData.CurrentSaveFile.CH5Data.VaultObjective.IsComplete = true;
		SendOnComplete();
	}

	private void HandleOldDeskTriggerOnEnter(object sender, EventArgs e)
	{
		m_OldDeskTrigger.OnEnter -= HandleOldDeskTriggerOnEnter;
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryClip11, "DIACH1/DIA_CH1_HENRY_11"));
	}

	private void HandleInteractableOnInteracted(object sender, EventArgs e)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Expected O, but got Unknown
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Expected O, but got Unknown
		m_Box.OnInteracted -= HandleInteractableOnInteracted;
		m_ButcherGang.SetActive(false);
		Transform val = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		GameManager.Instance.Player.GoToAndLookAt(m_AllyLookLocation);
		Sequence val2 = DOTween.Sequence();
		TweenSettingsExtensions.Insert(val2, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(val, m_BoxLookLocation.position, 1.5f, false), (Ease)6));
		TweenSettingsExtensions.Insert(val2, 0f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(val, m_BoxLookLocation.eulerAngles, 1.5f, (RotateMode)0), (Ease)6));
		TweenSettingsExtensions.InsertCallback(val2, 1.5f, new TweenCallback(DoDialogue));
		TweenSettingsExtensions.Insert(val2, 1.6f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(val, GameManager.Instance.Player.HeadContainer.position, 1f, false), (Ease)7));
		TweenSettingsExtensions.Insert(val2, 1.6f, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(val, m_AllyLookLocation.eulerAngles, 1f, (RotateMode)0), (Ease)7));
		TweenSettingsExtensions.OnComplete<Sequence>(val2, new TweenCallback(GameManager.Instance.GameCamera.ExitFreeRoamCam));
	}

	private void DoDialogue()
	{
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Expected O, but got Unknown
		EnableTriggers();
		for (int i = 0; i < m_EntranceDoors.Length; i++)
		{
			m_EntranceDoors[i].ForceClose();
			m_EntranceDoors[i].Lock();
		}
		m_Allison.gameObject.SetActive(true);
		m_Allison.ExitCombat();
		List<WaypointNode> list = new List<WaypointNode>();
		list.Add(m_AllisonWaypoint);
		m_Allison.UpdateWaypointList(list);
		m_Tom.gameObject.SetActive(true);
		m_Tom.ExitCombat();
		m_Tom.ForceStartIdle();
		List<WaypointNode> list2 = new List<WaypointNode>();
		list2.Add(m_TomWaypoint);
		m_Tom.UpdateWaypointList(list2);
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 0.05f, (TweenCallback)delegate
		{
			m_Allison.SetThought(AiThought.UseWaypoints);
			m_Tom.SetThought(AiThought.UseWaypoints);
		});
		m_Allison.DoSpeak(m_VaultClips[0].length - 0.25f);
		for (int num = 0; num < m_VaultClips.Length; num++)
		{
			AudioClip clip = m_VaultClips[num];
			AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_VaultClips[num], SubtitleConstants.DIA_CH5_ALISONA_VAULT[num], isTrimmed: true));
			if (num == m_VaultClips.Length - 1)
			{
				audioObject.OnComplete += DialogueOnComplete;
			}
			else if (num == m_VaultClips.Length - 3)
			{
				audioObject.OnComplete += HandleTomOnSmashDoor;
			}
			switch (num)
			{
			case 1:
			case 2:
			case 6:
			case 7:
			case 9:
			case 10:
			case 11:
			case 12:
			case 13:
			case 14:
				audioObject.OnComplete += delegate
				{
					m_Allison.DoSpeak(clip.length - 0.25f);
				};
				break;
			}
		}
	}

	private void DialogueOnComplete(object sender, EventArgs e)
	{
		(sender as AudioObject).OnComplete -= DialogueOnComplete;
		m_Allison.SetThought(AiThought.Idle);
		m_Allison.SetTarget(GameManager.Instance.Player.transform);
		GameManager.Instance.GameData.CurrentSaveFile.CH5Data.VaultObjective.IsStarted = true;
		GameManager.Instance.GameDataManager.Save();
	}

	private void HandleTomOnSmashDoor(object sender, EventArgs e)
	{
		(sender as AudioObject).OnComplete -= HandleTomOnSmashDoor;
		m_Tom.OnDoorSmashed += HandleOnDoorSmashed;
		m_Tom.gameObject.layer = LayerMask.NameToLayer("SelfDefault");
		m_Tom.SetThought(AiThought.Idle);
		m_Tom.EnterCombat();
		m_Tom.SetTarget(m_VaultDoor.AttackLocation);
	}

	private void HandleOnDoorSmashed(object sender, EventArgs e)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		m_Tom.OnDoorSmashed -= HandleOnDoorSmashed;
		GameManager.Instance.AudioManager.PlayAtPosition(m_DoorSmashClip, m_VaultDoor.transform.position);
	}

	protected override void OnDisposed()
	{
		m_HenryClip11 = null;
		m_VaultClips = null;
		base.OnDisposed();
	}
}
