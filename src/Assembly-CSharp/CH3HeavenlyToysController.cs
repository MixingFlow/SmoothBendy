using System;
using S13Audio;
using UnityEngine;

public class CH3HeavenlyToysController : BaseController
{
	[Header("Entrance!")]
	[SerializeField]
	private EventTrigger m_EntranceTrigger;

	[SerializeField]
	private EventTrigger m_EntranceDialogueTrigger;

	[Header("Objective: Clear Blockage!")]
	[SerializeField]
	private CH3ToyMachine m_ToyMachine;

	[SerializeField]
	private InteractablePowerLever m_ToyMachineLever;

	[SerializeField]
	private LightFixtureController m_LightFixture;

	[SerializeField]
	private BaseDoorController m_BlockedDoor;

	[SerializeField]
	private EventTrigger m_BlockageEventTrigger;

	[Header("<DEV CHEAT POSITIONS>")]
	[SerializeField]
	private Transform m_CheatPoint;

	private AudioClip m_EntranceMusicClip;

	private AudioClip m_HummingClip;

	private AudioClip m_PowerLeverClip;

	private AudioClip m_Henry13Clip;

	private AudioClip[] m_HenryBlockingTheWayClip;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_EntranceMusicClip = GameManager.Instance.GetAudioClip("Audio/MUS/CH3/MUS_CH3_formerglory");
		m_HummingClip = GameManager.Instance.GetAudioClip("Audio/DIA/CH3/Alice/ch3_alice_01_lobbyhumming");
		m_PowerLeverClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Mainr_Power_Lever_Turn_On_01");
		m_Henry13Clip = GameManager.Instance.GetAudioClip("Audio/DIA/CH3/Henry/ch3_henry_13_wowidontrememberanyofthis");
		m_HenryBlockingTheWayClip = GameManager.Instance.GetAudioClips("Audio/DIA/CH3/Henry/BlockingTheWay/");
		m_EntranceTrigger.SetActive(active: false);
		m_EntranceDialogueTrigger.SetActive(active: false);
		m_BlockageEventTrigger.SetActive(active: false);
		m_ToyMachineLever.SetActive(active: false);
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.HeavenlyToysObjective.IsComplete)
		{
			ForceComplete();
		}
		else if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.HeavenlyToysObjective.IsStarted)
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
		m_BlockedDoor.Lock();
		m_BlockageEventTrigger.OnEnter += HandleBlockageTriggerOnEnter;
		m_BlockageEventTrigger.SetActive(active: true);
		m_EntranceTrigger.OnEnter += HandleEntranceMusicOnEnter;
		m_EntranceTrigger.SetActive(active: true);
	}

	private void ForceStart()
	{
		S13AudioManager.Instance.InvokeEvent("evt_CH3_save_point_03");
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_10", "OBJECTIVES/CH3_OBJECTIVE_10_TIP"));
		m_BlockedDoor.Lock();
		m_ToyMachineLever.OnInteracted += HandleToyMachineLeverOnInteracted;
		m_ToyMachineLever.OnComplete += HandleToyMachineLeverOnComplete;
		m_ToyMachineLever.SetActive(active: true);
	}

	private void ForceComplete()
	{
		S13AudioManager.Instance.InvokeEvent("evt_CH3_save_point_03");
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_12", string.Empty));
		m_ToyMachineLever.ForceOpen();
		m_LightFixture.TurnOn();
		m_ToyMachine.Activate(isComplete: true);
		m_BlockedDoor.Unlock();
		SendOnComplete();
	}

	private void HandleEntranceMusicOnEnter(object sender, EventArgs e)
	{
		m_EntranceTrigger.OnEnter -= HandleEntranceMusicOnEnter;
		GameManager.Instance.AudioManager.Play(m_EntranceMusicClip, AudioObjectType.MUSIC).OnComplete += delegate
		{
			GameManager.Instance.AudioManager.Play(m_HummingClip);
		};
		m_EntranceDialogueTrigger.OnEnter += HandleEntranceDialogueOnEnter;
		m_EntranceDialogueTrigger.SetActive(active: true);
	}

	private void HandleEntranceDialogueOnEnter(object sender, EventArgs e)
	{
		m_EntranceDialogueTrigger.OnEnter -= HandleEntranceDialogueOnEnter;
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_Henry13Clip, "DIACH3/DIA_CH3_HENRY_13"));
	}

	private void HandleBlockageTriggerOnEnter(object sender, EventArgs e)
	{
		m_BlockageEventTrigger.OnEnter -= HandleBlockageTriggerOnEnter;
		for (int i = 0; i < m_HenryBlockingTheWayClip.Length; i++)
		{
			AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryBlockingTheWayClip[i], SubtitleConstants.DIA_CH3_HENRY_BLOCKING_THE_WAY[i]));
			if (i >= m_HenryBlockingTheWayClip.Length - 1)
			{
				audioObject.OnComplete += HandleBlockingTheWayOnComplete;
			}
		}
	}

	private void HandleBlockingTheWayOnComplete(object sender, EventArgs e)
	{
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_10", "OBJECTIVES/CH3_OBJECTIVE_10_TIP", 4f));
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.HeavenlyToysObjective.IsStarted = true;
		GameManager.Instance.GameDataManager.Save();
		m_ToyMachineLever.OnInteracted += HandleToyMachineLeverOnInteracted;
		m_ToyMachineLever.OnComplete += HandleToyMachineLeverOnComplete;
		m_ToyMachineLever.SetActive(active: true);
	}

	private void HandleToyMachineLeverOnInteracted(object sender, EventArgs e)
	{
		m_ToyMachineLever.OnInteracted -= HandleToyMachineLeverOnInteracted;
		m_ToyMachineLever.SetActive(active: false);
		GameManager.Instance.AudioManager.Play(m_PowerLeverClip);
	}

	private void HandleToyMachineLeverOnComplete(object sender, EventArgs e)
	{
		m_ToyMachineLever.OnComplete -= HandleToyMachineLeverOnComplete;
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_11", "OBJECTIVES/CH3_OBJECTIVE_11_TIP", 4f));
		m_LightFixture.TurnOn();
		m_ToyMachine.OnComplete += HandleToyMachineOnComplete;
		m_ToyMachine.Activate();
	}

	private void HandleToyMachineOnComplete(object sender, EventArgs e)
	{
		m_ToyMachine.OnComplete -= HandleToyMachineOnComplete;
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_12", string.Empty, 4f));
		m_BlockedDoor.Unlock();
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.HeavenlyToysObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		if (Object.op_Implicit((Object)(object)m_BlockageEventTrigger))
		{
			m_BlockageEventTrigger.OnEnter -= HandleBlockageTriggerOnEnter;
		}
		if (Object.op_Implicit((Object)(object)m_ToyMachineLever))
		{
			m_ToyMachineLever.OnInteracted -= HandleToyMachineLeverOnInteracted;
		}
		if (Object.op_Implicit((Object)(object)m_ToyMachineLever))
		{
			m_ToyMachineLever.OnComplete -= HandleToyMachineLeverOnComplete;
		}
		if (Object.op_Implicit((Object)(object)m_ToyMachine))
		{
			m_ToyMachine.OnComplete -= HandleToyMachineOnComplete;
		}
		m_EntranceMusicClip = null;
		m_HummingClip = null;
		m_PowerLeverClip = null;
		m_Henry13Clip = null;
		m_HenryBlockingTheWayClip = null;
		base.OnDisposed();
	}
}
