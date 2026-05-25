using System;
using System.Collections.Generic;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class CH2SewerController : BaseController
{
	[Header("Searcher Audio")]
	[SerializeField]
	private EventTrigger m_SearcherAudioTrigger;

	[SerializeField]
	private Transform m_SearcherAudioLocation;

	[Header("Objective: Get The Valve")]
	[SerializeField]
	private CH1PipeValve m_Valve;

	[SerializeField]
	private GameObject m_SammyDoorBlockage;

	[SerializeField]
	private BaseDoorController m_Door;

	[SerializeField]
	private CH2SwollenJack m_Jack;

	[SerializeField]
	private List<Transform> m_JackLocations;

	[SerializeField]
	private Transform m_JackDeathLocation;

	[SerializeField]
	private Transform m_JackStartLocation;

	[SerializeField]
	private Collider m_SammyDoorInkVolume;

	[SerializeField]
	private GameObject m_ShadowSammy;

	[Header("<Crusher>")]
	[SerializeField]
	private CH2CSmasherController m_Smasher;

	[SerializeField]
	private Transform m_HatLocation;

	[SerializeField]
	private Transform m_ValveLocation;

	[SerializeField]
	private EventTrigger m_JackTrigger;

	[Header("Breakables")]
	[SerializeField]
	private GameObject[] m_BreakablePlanks;

	[Header("DEV CHEATS")]
	[SerializeField]
	private Transform m_CheatPoint;

	private Transform m_Target;

	private Transform m_CurrentLocation;

	private AudioObject m_SearcherIdleObject;

	private AudioClip m_ValveClip;

	private AudioClip m_ValvePlaceClip;

	private AudioClip m_HenryClip12;

	private AudioClip m_HenryClip13;

	private AudioClip m_HenryClip14;

	private AudioClip m_SearcherIdleClip;

	private Interactable m_ValvePickup;

	public bool m_CanKillJack;

	private bool m_IsReady;

	public override void InitOnComplete()
	{
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		base.InitOnComplete();
		m_ValveClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Valve_Turn_01");
		m_ValvePlaceClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/Valves/SFX_CH3_valvepuzzle_valvearray_02");
		m_HenryClip12 = GameManager.Instance.GetAudioClip("Audio/DIA/CH2/Henry/DIA_CH2_HENRY_12");
		m_HenryClip13 = GameManager.Instance.GetAudioClip("Audio/DIA/CH2/Henry/DIA_CH2_HENRY_13");
		m_HenryClip14 = GameManager.Instance.GetAudioClip("Audio/DIA/CH2/Henry/DIA_CH2_HENRY_14");
		m_SearcherIdleClip = GameManager.Instance.GetAudioClip("Audio/SFX/Characters/SwollenSearchers/CH3_SWOLLEN_SEARCHER_IDLE");
		m_Jack.Valve.SetActive(active: false);
		m_Jack.transform.position = m_JackStartLocation.position;
		m_Jack.transform.eulerAngles = m_JackStartLocation.eulerAngles;
		m_JackTrigger.SetActive(active: false);
		m_SearcherAudioTrigger.SetActive(active: false);
	}

	public override void Activate()
	{
		m_Target = GameManager.Instance.Player.transform;
		if (GameManager.Instance.GameData.CurrentSaveFile.CH2Data.SewersObjective.IsComplete)
		{
			ForceComplete();
		}
		else if (GameManager.Instance.GameData.CurrentSaveFile.CH2Data.SewersObjective.IsStarted)
		{
			for (int i = 0; i < m_BreakablePlanks.Length; i++)
			{
				m_BreakablePlanks[i].SetActive(false);
			}
			if ((Object)(object)m_SearcherIdleObject != (Object)null)
			{
				m_SearcherIdleObject.Clear();
				m_SearcherIdleObject = null;
			}
			m_IsReady = false;
			ForceMoveJack();
			m_Jack.PlayAudio();
		}
		else
		{
			m_Jack.Show();
			m_CanKillJack = true;
			m_JackTrigger.OnEnter += HandleJackTriggerOnEnter;
			m_JackTrigger.SetActive(active: true);
			m_SearcherAudioTrigger.OnEnter += HandleSearcherAudioTriggerOnEnter;
			m_SearcherAudioTrigger.SetActive(active: true);
			m_Jack.PlayAudio();
		}
	}

	private void HandleSearcherAudioTriggerOnEnter(object sender, EventArgs e)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		m_SearcherAudioTrigger.OnEnter -= HandleSearcherAudioTriggerOnEnter;
		m_SearcherIdleObject = GameManager.Instance.AudioManager.PlayAtPosition(m_SearcherIdleClip, m_SearcherAudioLocation.position);
	}

	private void Update()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (m_IsReady && Object.op_Implicit((Object)(object)m_Target) && Vector3.Distance(m_Jack.transform.position, m_Target.position) < 15f)
		{
			m_IsReady = false;
			HideJack();
		}
	}

	private void HandleJackTriggerOnEnter(object sender, EventArgs e)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		m_JackTrigger.OnEnter -= HandleJackTriggerOnEnter;
		TweenSettingsExtensions.OnComplete<Tweener>(m_SearcherIdleObject.AudioSource.DOFade(0f, 1f), (TweenCallback)delegate
		{
			if ((Object)(object)m_SearcherIdleObject != (Object)null)
			{
				m_SearcherIdleObject.Clear();
				m_SearcherIdleObject = null;
			}
		});
		m_IsReady = false;
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryClip12, "DIACH2/DIA_CH2_HENRY_12"));
		HideJack();
		GameManager.Instance.GameData.CurrentSaveFile.CH2Data.SewersObjective.IsStarted = true;
		GameManager.Instance.GameDataManager.Save();
	}

	public void KillJack()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		m_IsReady = false;
		m_ValvePickup = m_Jack.Valve;
		m_ValvePickup.transform.SetParent(m_ValveLocation);
		m_ValvePickup.transform.localPosition = Vector3.zero;
		m_ValvePickup.transform.localEulerAngles = Vector3.zero;
		Transform hat = m_Jack.Hat;
		hat.SetParent(m_HatLocation);
		hat.localPosition = Vector3.zero;
		hat.localEulerAngles = Vector3.zero;
		m_Jack.Kill();
		m_ShadowSammy.SetActive(false);
		m_ValvePickup.OnInteracted += HandleValvePickupOnInteracted;
		m_ValvePickup.SetActive(active: true);
	}

	private void HandleValvePickupOnInteracted(object sender, EventArgs e)
	{
		m_ValvePickup.OnInteracted -= HandleValvePickupOnInteracted;
		m_ValvePickup.Dispose();
		GameManager.Instance.AchievementManager.SetAchievement(AchievementName.A_SPECIAL_HAT);
		GameManager.Instance.Player.PlayPickUpSound();
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryClip13, "DIACH2/DIA_CH2_HENRY_13"));
		m_Valve.OnInteracted += HandleValveEmptyOnInteracted;
		m_Valve.SetEmptyCollision(active: true);
	}

	private void HandleValveEmptyOnInteracted(object sender, EventArgs e)
	{
		m_Valve.OnInteracted -= HandleValveEmptyOnInteracted;
		GameManager.Instance.AudioManager.Play(m_ValvePlaceClip);
		m_Valve.OnInteracted += HandleValveOnInteracted;
		m_Valve.Activate();
	}

	private void HandleValveOnInteracted(object sender, EventArgs e)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		GameManager.Instance.AudioManager.PlayAtPosition(m_ValveClip, m_Valve.transform.position);
		TweenSettingsExtensions.OnComplete<Tweener>(m_Valve.DORotate(2f), new TweenCallback(ValveRotationOnComplete));
	}

	private void ValveRotationOnComplete()
	{
		m_SammyDoorBlockage.SetActive(false);
		m_SammyDoorInkVolume.enabled = false;
		m_Door.Unlock();
		S13AudioManager.Instance.InvokeEvent("evt_office_door_ink_drained");
		GameManager.Instance.GameData.CurrentSaveFile.CH2Data.SewersObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
		AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryClip14, "DIACH2/DIA_CH2_HENRY_14", isTrimmed: true));
		audioObject.OnComplete += delegate
		{
			GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH2_OBJECTIVE_SAMMYS_OFFICE", string.Empty, 4f));
			SendOnComplete();
		};
	}

	private void HideJack()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		m_Jack.Hide();
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 1.5f, new TweenCallback(MoveJack));
	}

	private void ForceMoveJack()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		GoToNextPosition();
		if (Object.op_Implicit((Object)(object)m_Target))
		{
			while (Vector3.Distance(m_Jack.transform.position, m_Target.position) < 20f)
			{
				GoToNextPosition();
			}
		}
		m_Jack.Show();
		m_CanKillJack = (Object)(object)m_CurrentLocation == (Object)(object)m_JackDeathLocation;
		m_IsReady = true;
	}

	private void MoveJack()
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected O, but got Unknown
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		GoToNextPosition();
		if (Object.op_Implicit((Object)(object)m_Target))
		{
			while (Vector3.Distance(m_Jack.transform.position, m_Target.position) < 20f)
			{
				GoToNextPosition();
			}
		}
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 1f, new TweenCallback(ShowJack));
	}

	private void ShowJack()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		m_Jack.Show();
		m_CanKillJack = (Object)(object)m_CurrentLocation == (Object)(object)m_JackDeathLocation;
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 1f, (TweenCallback)delegate
		{
			m_IsReady = true;
		});
	}

	private void GoToNextPosition()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Transform nextPosition = GetNextPosition();
		m_Jack.transform.position = nextPosition.position;
		m_Jack.transform.eulerAngles = nextPosition.eulerAngles;
	}

	private Transform GetNextPosition()
	{
		int index = 0;
		Transform val = m_JackLocations[index];
		if (m_Smasher.IsDown && (Object)(object)val == (Object)(object)m_JackDeathLocation)
		{
			index = 1;
			val = m_JackLocations[index];
		}
		m_CurrentLocation = val;
		m_JackLocations.Add(val);
		m_JackLocations.RemoveAt(index);
		return val;
	}

	private void Complete()
	{
		m_Valve.Disable();
		m_SammyDoorBlockage.SetActive(false);
		m_Door.Unlock();
		SendOnComplete();
	}

	private void ForceComplete()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		S13AudioManager.Instance.InvokeEvent("evt_CH2_save_point_10");
		for (int i = 0; i < m_BreakablePlanks.Length; i++)
		{
			m_BreakablePlanks[i].SetActive(false);
		}
		m_Jack.transform.position = m_JackDeathLocation.position;
		m_Jack.transform.eulerAngles = m_JackDeathLocation.eulerAngles;
		Transform hat = m_Jack.Hat;
		hat.SetParent(m_HatLocation);
		hat.localPosition = Vector3.zero;
		hat.localEulerAngles = Vector3.zero;
		m_Jack.Kill(isSilent: true);
		m_Jack.Valve.Dispose();
		m_Valve.Disable();
		m_SammyDoorBlockage.SetActive(false);
		if (GameManager.Instance.GameData.CurrentSaveFile.CH2Data.SammysOfficeObjective.IsComplete)
		{
			m_Door.ForceOpen(-145f);
			m_Door.Lock();
		}
		else
		{
			m_Door.Unlock();
		}
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		if (Object.op_Implicit((Object)(object)m_ValvePickup))
		{
			m_ValvePickup.OnInteracted -= HandleValvePickupOnInteracted;
		}
		m_Smasher = null;
		m_Target = null;
		m_CurrentLocation = null;
		m_HenryClip12 = null;
		m_HenryClip13 = null;
		m_HenryClip14 = null;
		m_ValveClip = null;
		m_SearcherIdleClip = null;
		m_SearcherIdleObject = null;
		base.OnDisposed();
	}
}
