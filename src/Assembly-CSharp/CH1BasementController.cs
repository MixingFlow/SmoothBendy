using System;
using System.Collections.Generic;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class CH1BasementController : BaseController
{
	[Header("Objective: Clear Ink")]
	[SerializeField]
	private Transform m_Ink;

	[SerializeField]
	private List<Transform> m_InkLevels;

	[SerializeField]
	private List<CH1PipeValve> m_Valves;

	[SerializeField]
	private List<DisposableObject> m_Blockers;

	[SerializeField]
	private BaseDoorController m_Door;

	[Header("DEV CHEATS")]
	[SerializeField]
	private Transform m_CheatPoint;

	private CH1PipeValve m_ActiveValve;

	private Transform m_NextInkLevel;

	private Sequence m_Sequence;

	private AudioClip m_ValveClip;

	private bool m_IsActivated;

	private int m_CurrentValve;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_Door.Lock();
		m_ValveClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Valve_Turn_01");
	}

	public override void Activate()
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		m_CurrentValve = -1;
		SetNextValve();
		m_IsActivated = true;
		if (GameManager.Instance.GameData.CurrentSaveFile.CH1Data.BasementObjective.IsComplete)
		{
			ForceComplete();
		}
		else if (!GameManager.Instance.GameData.CurrentSaveFile.CH1Data.BasementObjective.IsStarted)
		{
			TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 2f, (TweenCallback)delegate
			{
				GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH1_OBJ_07", "OBJECTIVES/CH1_OBJ_07_TIP", 4f));
				GameManager.Instance.GameData.CurrentSaveFile.CH1Data.BasementObjective.IsStarted = true;
				GameManager.Instance.GameDataManager.Save();
			});
		}
	}

	private void Update()
	{
		if (m_IsActivated && !base.IsDisposed)
		{
			bool flag = GameManager.Instance.Player.CurrentFootstepType == FootstepTypes.INK_DEEP;
			GameManager.Instance.Player.SetJump(!flag);
			GameManager.Instance.Player.SetSlowed(flag);
		}
	}

	private void SetNextValve()
	{
		m_CurrentValve++;
		if (m_CurrentValve > m_Valves.Count - 1)
		{
			m_Door.Unlock();
			m_Door.OnOpen += HandleDoorOnOpen;
		}
		else
		{
			SetValve(m_Valves[m_CurrentValve]);
		}
	}

	private void HandleDoorOnOpen(object sender, EventArgs e)
	{
		m_Door.OnOpen -= HandleDoorOnOpen;
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH1_OBJ_08", "OBJECTIVES/CH1_OBJ_08_TIP", 4f));
		GameManager.Instance.GameData.CurrentSaveFile.CH1Data.BasementObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
		SendOnComplete();
	}

	private void SetValve(CH1PipeValve valve)
	{
		m_NextInkLevel = m_InkLevels[m_CurrentValve];
		m_ActiveValve = valve;
		m_ActiveValve.Activate();
		m_ActiveValve.OnInteracted += HandleValveOnInteracted;
	}

	private void HandleValveOnInteracted(object sender, EventArgs e)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		m_ActiveValve.OnInteracted -= HandleValveOnInteracted;
		TweenSettingsExtensions.OnComplete<Sequence>(DOValveInteract(), new TweenCallback(SetNextValve));
	}

	private Sequence DOValveInteract()
	{
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Expected O, but got Unknown
		ResetSequence();
		float num = 0f;
		float num2 = 1f;
		if (m_CurrentValve == 0)
		{
			S13AudioManager.Instance.InvokeEvent("evt_stairwell_valve1");
		}
		else if (m_CurrentValve == 1)
		{
			S13AudioManager.Instance.InvokeEvent("evt_stairwell_valve2");
		}
		else if (m_CurrentValve == 2)
		{
			S13AudioManager.Instance.InvokeEvent("evt_stairwell_valve3");
		}
		GameManager.Instance.AudioManager.Play(m_ValveClip);
		TweenSettingsExtensions.Insert(m_Sequence, num, (Tween)(object)m_ActiveValve.DORotate(num2 + 1f));
		num += num2 / 2f;
		num2 = 14f / (float)(m_CurrentValve + 1);
		if (m_CurrentValve < m_Valves.Count - 1)
		{
			DisposableObject disposableObject = m_Blockers[m_CurrentValve];
			TweenSettingsExtensions.InsertCallback(m_Sequence, num + 2f, new TweenCallback(disposableObject.Dispose));
		}
		TweenSettingsExtensions.Insert(m_Sequence, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(m_Ink, m_NextInkLevel.localPosition.y, num2, false), (Ease)7));
		return m_Sequence;
	}

	private void ResetSequence()
	{
		KillSequence();
		m_Sequence = DOTween.Sequence();
	}

	private void KillSequence()
	{
		if (m_Sequence != null)
		{
			TweenExtensions.Kill((Tween)(object)m_Sequence, false);
			m_Sequence = null;
		}
	}

	private void ForceComplete()
	{
		S13AudioManager.Instance.InvokeEvent("evt_CH1_save_point_06");
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH1_OBJ_08", "OBJECTIVES/CH1_OBJ_08_TIP"));
		m_Door.ForceOpen(145f);
		m_Door.Lock();
		((Component)m_Ink).gameObject.SetActive(false);
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		KillSequence();
		m_ActiveValve = null;
		m_NextInkLevel = null;
		m_ValveClip = null;
		base.OnDisposed();
	}
}
