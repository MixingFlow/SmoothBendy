using System;
using System.Collections.Generic;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class CH1InkMachineRevealController : BaseController
{
	[Serializable]
	private class BatterySlots
	{
		public Interactable Slot;

		public Transform StartPos;

		public Transform EndPos;
	}

	[Header("Objective: Turn On Ink Machine")]
	[SerializeField]
	private InkMachineController m_InkMachineController;

	[SerializeField]
	private EventTrigger m_InkMachineEventTrigger;

	[SerializeField]
	private Transform m_InkMachine;

	[SerializeField]
	private Transform m_InkMachineRevealPoint;

	[SerializeField]
	private Transform m_Chains;

	[SerializeField]
	private Transform m_ChainsEndPoint;

	[SerializeField]
	private Transform m_Generator;

	[SerializeField]
	private List<Interactable> m_Batteries;

	[SerializeField]
	private List<BatterySlots> m_BatterySlots;

	[SerializeField]
	private CH3LeverLight m_Lever;

	[Header("Other")]
	[SerializeField]
	private InteractableTrunk m_Trunk;

	[SerializeField]
	private BasicAnimationController m_Machinery;

	[SerializeField]
	private GenericDoorController m_Gate;

	[SerializeField]
	private BaseDoorController m_BreakRoomDoor;

	[SerializeField]
	private GameObject m_InteractTutorial;

	private List<GameObject> m_CollectedBatteries = new List<GameObject>();

	private AudioClip m_HenryClip02;

	private AudioClip m_HenryClip03;

	private AudioClip m_InkMachineMusicClip;

	private int m_BatteryCount;

	private int m_MaxBatteries = 2;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_HenryClip02 = GameManager.Instance.GetAudioClip("Audio/DIA/CH1/Henry/DIA_CH1_HENRY_02");
		m_HenryClip03 = GameManager.Instance.GetAudioClip("Audio/DIA/CH1/Henry/DIA_CH1_HENRY_03");
		m_InkMachineMusicClip = GameManager.Instance.GetAudioClip("Audio/MUS/CH1/MUS_MachineRevealed");
		m_Gate.ForceClose();
		m_Machinery.Stop();
		m_Trunk.SetActive(active: false);
		m_Lever.SetSingleInteraction(active: true);
		m_InteractTutorial.SetActive(false);
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH1Data.InkMachineRevealObjective.IsComplete)
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
		m_InkMachineEventTrigger.OnEnter += HandleInkMachineTriggerOnEnter;
		m_InkMachineEventTrigger.SetActive(active: true);
	}

	private void ForceComplete()
	{
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		S13AudioManager.Instance.InvokeEvent("evt_CH1_save_point_01");
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH1_OBJ_02", "OBJECTIVES/CH1_OBJ_02_TIP"));
		m_InkMachineEventTrigger.OnEnter -= HandleInkMachineTriggerOnEnter;
		m_InkMachineEventTrigger.SetActive(active: false);
		TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScaleY(m_Generator, 1.01f, 0.2f), (Ease)7), -1, (LoopType)1);
		m_Trunk.ForceOpen();
		m_Trunk.SetActive(active: false);
		m_Trunk.ForceRemoveEffects();
		m_Lever.Activate(isAlreadyActive: true);
		m_Gate.ForceOpen();
		m_BreakRoomDoor.ForceOpen(-145f);
		m_Machinery.Play();
		m_InkMachine.position = m_InkMachineRevealPoint.position;
		for (int i = 0; i < m_Batteries.Count; i++)
		{
			Interactable interactable = m_Batteries[i];
			interactable.SetActive(active: false);
			interactable.transform.SetParent(m_Generator);
			interactable.transform.position = m_BatterySlots[i].EndPos.position;
			interactable.transform.eulerAngles = m_BatterySlots[i].EndPos.eulerAngles;
		}
		SendOnComplete();
	}

	private void HandleInkMachineTriggerOnEnter(object sender, EventArgs e)
	{
		m_InkMachineEventTrigger.OnEnter -= HandleInkMachineTriggerOnEnter;
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryClip02, "DIACH1/DIA_CH1_HENRY_02")).OnComplete += HandleInitialDialogueOnComplete;
	}

	private void HandleInitialDialogueOnComplete(object sender, EventArgs e)
	{
		(sender as AudioObject).OnComplete -= HandleInitialDialogueOnComplete;
		m_InteractTutorial.SetActive(true);
		m_Trunk.SetActive(active: true);
		for (int i = 0; i < m_BatterySlots.Count; i++)
		{
			Interactable slot = m_BatterySlots[i].Slot;
			((Component)slot).GetComponent<Collider>().enabled = false;
			slot.SetActive(active: true);
		}
		for (int j = 0; j < m_Batteries.Count; j++)
		{
			Interactable interactable = m_Batteries[j];
			interactable.OnInteracted += HandleBatteryOnInteracted;
			interactable.SetActive(active: true);
		}
	}

	private void HandleBatteryOnInteracted(object sender, EventArgs e)
	{
		Interactable interactable = sender as Interactable;
		interactable.OnInteracted -= HandleBatteryOnInteracted;
		interactable.SetActive(active: false);
		interactable.gameObject.SetActive(false);
		GameManager.Instance.Player.PlayPickUpSound();
		m_CollectedBatteries.Add(interactable.gameObject);
		if (m_CollectedBatteries.Count < 2)
		{
			for (int i = 0; i < m_BatterySlots.Count; i++)
			{
				Interactable slot = m_BatterySlots[i].Slot;
				((Component)slot).GetComponent<Collider>().enabled = true;
				slot.OnInteracted += HandleEmptyBatteryOnInteracted;
			}
		}
	}

	private void HandleEmptyBatteryOnInteracted(object sender, EventArgs e)
	{
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Expected O, but got Unknown
		Interactable interactable = sender as Interactable;
		BatterySlots batterySlots = null;
		for (int i = 0; i < m_BatterySlots.Count; i++)
		{
			if ((Object)(object)m_BatterySlots[i].Slot == (Object)(object)interactable)
			{
				batterySlots = m_BatterySlots[i];
				m_BatterySlots.RemoveAt(i);
				break;
			}
		}
		if (m_BatterySlots.Count > 0 && (Object)(object)m_BatterySlots[0].Slot == (Object)(object)interactable)
		{
			return;
		}
		interactable.OnInteracted -= HandleBatteryOnInteracted;
		interactable.SetActive(active: false);
		interactable.gameObject.SetActive(false);
		GameObject val = m_CollectedBatteries[0];
		m_CollectedBatteries.RemoveAt(0);
		val.SetActive(true);
		val.transform.SetParent(m_Generator);
		val.transform.position = batterySlots.StartPos.position;
		val.transform.eulerAngles = batterySlots.StartPos.eulerAngles;
		if (m_CollectedBatteries.Count <= 0)
		{
			for (int j = 0; j < m_BatterySlots.Count; j++)
			{
				Interactable slot = m_BatterySlots[j].Slot;
				((Component)slot).GetComponent<Collider>().enabled = false;
				slot.OnInteracted -= HandleEmptyBatteryOnInteracted;
			}
		}
		m_BatteryCount++;
		bool isComplete = m_BatteryCount >= m_MaxBatteries;
		S13AudioManager.Instance.PlayAudio("sfx_battery_added");
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(TweenSettingsExtensions.SetDelay<Tweener>(ShortcutExtensions.DOMove(val.transform, batterySlots.EndPos.position, 0.5f, false), 0.1f), (Ease)5), (TweenCallback)delegate
		{
			if (isComplete)
			{
				TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScaleY(m_Generator, 1.01f, 0.2f), (Ease)7), -1, (LoopType)1);
				S13AudioManager.Instance.InvokeEvent("evt_battery_pack_on");
				GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryClip03, "DIACH1/DIA_CH1_HENRY_03"));
				m_Lever.OnComplete += HandleLeverOnComplete;
				m_Lever.Activate();
			}
		});
	}

	private void HandleLeverOnComplete(object sender, EventArgs e)
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Expected O, but got Unknown
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Expected O, but got Unknown
		m_Lever.OnComplete += HandleLeverOnComplete;
		GameManager.Instance.AudioManager.Play(m_InkMachineMusicClip, AudioObjectType.MUSIC);
		S13AudioManager.Instance.InvokeEvent("evt_ink_machine_reveal_start");
		m_BreakRoomDoor.ForceOpen(-20f);
		m_BreakRoomDoor.Unlock();
		Sequence val = DOTween.Sequence();
		float num = 0f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_Chains, m_ChainsEndPoint.position, 22f, false), (Ease)6));
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_InkMachine, m_InkMachineRevealPoint.position, 22f, false), (Ease)6));
		TweenSettingsExtensions.InsertCallback(val, num, new TweenCallback(m_Machinery.Play));
		TweenSettingsExtensions.InsertCallback(val, num + 10f, new TweenCallback(m_Gate.Open));
		TweenSettingsExtensions.OnComplete<Sequence>(val, new TweenCallback(OnMachineRevealComplete));
	}

	private void OnMachineRevealComplete()
	{
		m_InkMachineController.ShowSteam();
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH1_OBJ_02", "OBJECTIVES/CH1_OBJ_02_TIP", 4f));
		S13AudioManager.Instance.InvokeEvent("evt_ink_machine_reveal_stop");
		GameManager.Instance.GameData.CurrentSaveFile.CH1Data.InkMachineRevealObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		if (m_CollectedBatteries != null)
		{
			m_CollectedBatteries.Clear();
			m_CollectedBatteries = null;
		}
		m_HenryClip02 = null;
		m_HenryClip03 = null;
		m_InkMachineMusicClip = null;
		base.OnDisposed();
	}
}
