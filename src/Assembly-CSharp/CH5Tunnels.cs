using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class CH5Tunnels : BaseController
{
	[SerializeField]
	private CH5DemonWorthy m_Boat;

	[SerializeField]
	private Transform m_NodeParent;

	[SerializeField]
	private GameObject m_CaveSpawnPoint;

	[SerializeField]
	private GameObject m_DockSpawnPoint;

	[Header("Hand Boat Sequence")]
	[SerializeField]
	private EventTrigger m_HandTrigger;

	[SerializeField]
	private CH5BendyHandGrab m_BendyHandGrab;

	[SerializeField]
	private GameObject m_FrontBoatCollider;

	[SerializeField]
	private GameObject m_HandTriggersParent;

	[SerializeField]
	private EventTrigger[] m_HandTriggers;

	[Header("Hand Chase")]
	[SerializeField]
	private CH5BendyHandChase m_HandChase;

	private AudioClip m_HenryClogClip;

	private AudioClip m_TunnelMusic;

	private AudioObject m_MusicObject;

	private Vector3[] m_Nodes;

	private Vector3 m_InitialPosition;

	private Vector3 m_InitialRotation;

	private bool m_IsStarted;

	private bool m_IsClogged;

	public override void InitOnComplete()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected O, but got Unknown
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		base.InitOnComplete();
		List<Vector3> list = new List<Vector3>();
		foreach (Transform item in m_NodeParent)
		{
			Transform val = item;
			list.Add(val.position);
		}
		m_Nodes = list.ToArray();
		m_HandTrigger.SetActive(active: false);
		for (int i = 0; i < m_HandTriggers.Length; i++)
		{
			m_HandTriggers[i].SetActive(active: false);
		}
		m_CaveSpawnPoint.SetActive(true);
		m_DockSpawnPoint.SetActive(false);
		m_HandChase.SetHandLocation(m_Boat.HandLocation, m_Boat.HandStartLocation);
		m_HenryClogClip = GameManager.Instance.GetAudioClip("Audio/DIA/CH5/Henry/DIA_CH5_henry_soundslikesomethingisstuckinthepaddlwheel");
		m_TunnelMusic = GameManager.Instance.GetAudioClip("Audio/MUS/CH5/MUS_TheInkRiver");
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH5Data.TunnelsObjective.IsComplete)
		{
			ForceComplete();
		}
		else
		{
			InternalActivate();
		}
	}

	private void InternalActivate()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		m_BendyHandGrab.Activate();
		m_HandTrigger.OnEnter += HandleHandTriggerOnEnter;
		m_HandTrigger.SetActive(active: true);
		m_InitialPosition = m_Boat.transform.position;
		m_InitialRotation = m_Boat.transform.eulerAngles;
		m_Boat.OnThrottled += HandleBoatOnThrottled;
		m_Boat.Activate();
	}

	private void ForceComplete()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		m_Boat.transform.position = m_Nodes[m_Nodes.Length - 1];
		m_Boat.transform.eulerAngles = new Vector3(0f, 180f, 0f);
		m_Boat.Disable();
		m_Boat.RemoveColliders();
		m_Boat.ForceFinalPosition();
		m_CaveSpawnPoint.SetActive(false);
		m_DockSpawnPoint.SetActive(false);
		m_HandChase.gameObject.SetActive(false);
		m_BendyHandGrab.gameObject.SetActive(false);
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/CURRENT_OBJECTIVE_HEADER", "OBJECTIVES/OBJECTIVE_FIND_A_NEW_EXIT", string.Empty));
		SendOnComplete();
	}

	private void HandleGameUnpaused(object sender, EventArgs e)
	{
		ShortcutExtensions.DOPause((Component)(object)m_Boat.transform);
		m_Boat.Release();
	}

	private void HandleHandTriggerOnEnter(object sender, EventArgs e)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Expected O, but got Unknown
		m_HandTrigger.OnEnter -= HandleHandTriggerOnEnter;
		m_IsClogged = true;
		m_Boat.Clog();
		Sequence val = DOTween.Sequence();
		TweenSettingsExtensions.InsertCallback(val, 1f, (TweenCallback)delegate
		{
			GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryClogClip, "DIACH5/DIA_CH5_HENRY_PADDLE_WHEEL"));
		});
		if (Object.op_Implicit((Object)(object)GameManager.Instance.Player.WeaponGameObject))
		{
			TweenSettingsExtensions.Insert(val, 4f, (Tween)(object)ShortcutExtensions.DOLocalMoveY(GameManager.Instance.Player.WeaponGameObject.transform, -5f, 0.5f, false));
			TweenSettingsExtensions.Insert(val, 6f, (Tween)(object)ShortcutExtensions.DOLocalMoveY(GameManager.Instance.Player.WeaponGameObject.transform, 0f, 0.5f, false));
		}
		TweenSettingsExtensions.InsertCallback(val, 4.5f, (TweenCallback)delegate
		{
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			GameManager.Instance.HideCrosshair();
			m_MusicObject = GameManager.Instance.AudioManager.Play(m_TunnelMusic, AudioObjectType.MUSIC, -1);
			Transform val2 = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
			val2.SetParent(m_Boat.Content);
			ShortcutExtensions.DOLookAt(val2, m_BendyHandGrab.transform.position, 1.5f, (AxisConstraint)0, (Vector3?)null);
			m_BendyHandGrab.OnComplete += HandleBendyHandGrabOnComplete;
			m_BendyHandGrab.Play();
		});
		TweenSettingsExtensions.InsertCallback(val, 6f, (TweenCallback)delegate
		{
			GameManager.Instance.ShowCrosshair();
			GameManager.Instance.GameCamera.ExitFreeRoamCam();
		});
	}

	private void HandleBendyHandGrabOnComplete(object sender, EventArgs e)
	{
		m_BendyHandGrab.OnComplete -= HandleBendyHandGrabOnComplete;
		if (m_IsClogged)
		{
			m_HandChase.ActivateHand();
		}
		m_HandChase.OnHit += HandleHandChaseOnHit;
		for (int i = 0; i < m_HandTriggers.Length; i++)
		{
			m_HandTriggers[i].OnEnter += HandleHandTriggersOnEnter;
			m_HandTriggers[i].SetActive(active: true);
		}
	}

	private void HandleHandTriggersOnEnter(object sender, EventArgs e)
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected O, but got Unknown
		(sender as EventTrigger).OnEnter -= HandleHandTriggersOnEnter;
		if ((Object)(object)m_MusicObject == (Object)null)
		{
			m_MusicObject = GameManager.Instance.AudioManager.Play(m_TunnelMusic, AudioObjectType.MUSIC, -1);
		}
		m_IsClogged = true;
		m_Boat.Clog();
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 2f, new TweenCallback(m_HandChase.ActivateHand));
	}

	private void HandleHandChaseOnHit(object sender, EventArgs e)
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected O, but got Unknown
		GameManager.Instance.Player.transform.SetParent((Transform)null);
		for (int i = 0; i < 5; i++)
		{
			GameManager.Instance.ShowHurtBorder(isSilent: true);
		}
		GameManager.Instance.ShowHurtBorder();
		if (Object.op_Implicit((Object)(object)m_MusicObject))
		{
			TweenSettingsExtensions.OnComplete<Tweener>(m_MusicObject.AudioSource.DOFade(0f, 1f), (TweenCallback)delegate
			{
				if (Object.op_Implicit((Object)(object)m_MusicObject))
				{
					m_MusicObject.Clear();
					m_MusicObject = null;
				}
			});
		}
		ShortcutExtensions.DOKill((Component)(object)m_Boat.transform, false);
		m_Boat.transform.position = m_InitialPosition;
		m_Boat.transform.eulerAngles = m_InitialRotation;
		m_Boat.ForceMidPosition();
		m_Boat.ForceUnclog();
		m_Boat.UnlockBoat();
		m_IsStarted = false;
		m_FrontBoatCollider.SetActive(true);
		for (int num = 0; num < m_HandTriggers.Length; num++)
		{
			EventTrigger eventTrigger = m_HandTriggers[num];
			eventTrigger.OnEnter -= HandleHandTriggersOnEnter;
			eventTrigger.OnEnter += HandleHandTriggersOnEnter;
			eventTrigger.ResetTrigger();
			eventTrigger.SetActive(active: true);
		}
		m_Boat.OnReleased -= HandleBoatOnReleased;
		m_Boat.OnThrottled += HandleBoatOnThrottled;
	}

	private void HandleBoatOnThrottled(object sender, EventArgs e)
	{
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Expected O, but got Unknown
		m_Boat.OnThrottled -= HandleBoatOnThrottled;
		GameManager.Instance.OnUnpaused -= HandleGameUnpaused;
		GameManager.Instance.OnUnpaused += HandleGameUnpaused;
		m_HandTriggersParent.SetActive(true);
		if (!m_IsStarted)
		{
			m_IsStarted = true;
			m_CaveSpawnPoint.SetActive(false);
			m_DockSpawnPoint.SetActive(true);
			m_FrontBoatCollider.SetActive(false);
			TweenSettingsExtensions.OnComplete<TweenerCore<Vector3, Path, PathOptions>>(TweenSettingsExtensions.SetEase<TweenerCore<Vector3, Path, PathOptions>>(TweenSettingsExtensions.SetLookAt(ShortcutExtensions.DOPath(m_Boat.transform, m_Nodes, 140f, (PathType)1, (PathMode)1, 10, (Color?)null), 0.01f, (Vector3?)base.transform.forward, (Vector3?)null), (Ease)1), new TweenCallback(BoatOnComplete));
		}
		else
		{
			ShortcutExtensions.DOPlay((Component)(object)m_Boat.transform);
		}
		m_Boat.OnReleased += HandleBoatOnReleased;
	}

	private void HandleBoatOnReleased(object sender, EventArgs e)
	{
		m_Boat.OnReleased -= HandleBoatOnReleased;
		m_HandTriggersParent.SetActive(false);
		ShortcutExtensions.DOPause((Component)(object)m_Boat.transform);
		m_Boat.OnThrottled += HandleBoatOnThrottled;
	}

	private void BoatOnComplete()
	{
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		GameManager.Instance.GameData.CurrentSaveFile.CH5Data.TunnelsObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
		if (Object.op_Implicit((Object)(object)m_MusicObject))
		{
			TweenSettingsExtensions.OnComplete<Tweener>(m_MusicObject.AudioSource.DOFade(0f, 1f), (TweenCallback)delegate
			{
				if (Object.op_Implicit((Object)(object)m_MusicObject))
				{
					m_MusicObject.Clear();
					m_MusicObject = null;
				}
			});
		}
		m_Boat.Disable();
		m_Boat.RemoveColliders();
		TweenSettingsExtensions.OnComplete<Sequence>(m_Boat.ReleaseSequence(), new TweenCallback(OnCompleteController));
	}

	private void OnCompleteController()
	{
		GameManager.Instance.OnUnpaused -= HandleGameUnpaused;
		if ((Object)(object)m_Boat != (Object)null)
		{
			m_Boat.OnReleased -= HandleBoatOnReleased;
			m_Boat.OnThrottled -= HandleBoatOnThrottled;
		}
		m_CaveSpawnPoint.SetActive(false);
		m_DockSpawnPoint.SetActive(false);
		m_Boat.FinalBoatPosition();
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/CURRENT_OBJECTIVE_HEADER", "OBJECTIVES/OBJECTIVE_FIND_A_NEW_EXIT", string.Empty));
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		GameManager.Instance.OnUnpaused -= HandleGameUnpaused;
		if ((Object)(object)m_Boat != (Object)null)
		{
			m_Boat.OnReleased -= HandleBoatOnReleased;
			m_Boat.OnThrottled -= HandleBoatOnThrottled;
		}
		m_Nodes = null;
		m_TunnelMusic = null;
		if (Object.op_Implicit((Object)(object)m_MusicObject))
		{
			m_MusicObject.Clear();
			m_MusicObject = null;
		}
		base.OnDisposed();
	}
}
