using System;
using System.Collections.Generic;
using Ai;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class CH3LiftEntranceController : BaseController
{
	[Header("<Controllers>")]
	[SerializeField]
	private BorisAi m_Boris;

	[Header("Objective: Kill The Piper!")]
	[SerializeField]
	private PiperAi m_Piper;

	[SerializeField]
	private EventTrigger m_DialogueTrigger;

	[SerializeField]
	private Interactable m_Lever;

	[SerializeField]
	private Transform m_BorisLever;

	[SerializeField]
	private GenericDoorController m_ClosedDoor;

	[SerializeField]
	private WaypointList m_LeverPath;

	[SerializeField]
	private List<LightFlicker> m_Lights;

	[SerializeField]
	private List<GameObject> m_Poster;

	[SerializeField]
	private MeshRenderer m_Cables;

	[Header("Objective: Get To The Lift!")]
	[SerializeField]
	private EventTrigger m_BorisGoTrigger;

	[SerializeField]
	private WaypointList m_BorisLeverPulledPath;

	[Header("Ink Machine!")]
	[SerializeField]
	private EventTrigger m_InkMachineTrigger;

	[SerializeField]
	private InkMachineLoopController m_InkMachineLoop;

	[Header("<DEV CHEAT POSITIONS>")]
	[SerializeField]
	private Transform m_CheatPoint;

	private AudioObject m_PiperMusic;

	private AudioClip m_PiperMusicClip;

	private AudioClip m_OldFriendsClip;

	private AudioClip m_PiperJumpscareClip;

	private AudioClip m_JumpscarePaperClip;

	private AudioClip m_HenryClipCH3_20;

	private AudioClip m_HenryClipCH3_21;

	private AudioClip m_MainPowerTurnOnClip;

	private bool m_IsPlayerReady;

	private bool m_IsBorisReady;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_Boris = GameManager.Instance.CharacterManager.Boris;
		m_PiperMusicClip = GameManager.Instance.GetAudioClip("Audio/MUS/CH3/MUS_CH3_whoslaughingnow");
		m_OldFriendsClip = GameManager.Instance.GetAudioClip("Audio/MUS/CH3/MUS_CH3_oldfriendsnewfaces");
		m_PiperJumpscareClip = GameManager.Instance.GetAudioClip("Audio/SFX/Characters/ButcherGang/CH3_BUTCHER_GANG_JUMPSCARE");
		m_JumpscarePaperClip = GameManager.Instance.GetAudioClip("Audio/SFX/CH3/SFX_CH3_piperrippingthroughposter");
		m_HenryClipCH3_20 = GameManager.Instance.GetAudioClip("Audio/DIA/CH3/Henry/ch3_henry_20_twoleversatonce");
		m_HenryClipCH3_21 = GameManager.Instance.GetAudioClip("Audio/DIA/CH3/Henry/ch3_henry_21_yougetthisoneillfindtheother");
		m_MainPowerTurnOnClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Mainr_Power_Lever_Turn_On_01");
		m_BorisGoTrigger.SetActive(active: false);
		m_DialogueTrigger.SetActive(active: false);
		m_Lever.SetActive(active: false);
		m_InkMachineTrigger.SetActive(active: false);
		m_InkMachineLoop.gameObject.SetActive(false);
		m_Piper.gameObject.SetActive(false);
		((Renderer)m_Cables).material.SetFloat("_Shimmer", 0f);
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH3Data.PosterPiperObjective.IsComplete)
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
		m_DialogueTrigger.OnEnter += HandleDialogueTriggerOnEnter;
		m_DialogueTrigger.SetActive(active: true);
		m_Boris.OnWaypointComplete += HandleBorisLeverNodeOnComplete;
		m_Boris.UpdateWaypointList(m_LeverPath.Waypoints);
	}

	private void ForceComplete()
	{
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		S13AudioManager.Instance.InvokeEvent("evt_CH3_save_point_07");
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/OBJECTIVE_FIND_A_NEW_EXIT", string.Empty));
		for (int i = 0; i < m_Lights.Count; i++)
		{
			m_Lights[i].TurnOff();
			((Behaviour)m_Lights[i]).enabled = false;
		}
		m_InkMachineLoop.gameObject.SetActive(true);
		m_InkMachineLoop.Activate();
		m_Piper.Dispose();
		for (int j = 0; j < m_Poster.Count; j++)
		{
			m_Poster[j].SetActive(!m_Poster[j].activeSelf);
		}
		((Renderer)m_Cables).material.SetFloat("_Shimmer", 0f);
		Transform borisLever = m_BorisLever;
		borisLever.localEulerAngles += new Vector3(80f, 0f, 0f);
		Transform obj = m_Lever.transform;
		obj.localEulerAngles += new Vector3(-80f, 0f, 0f);
		m_ClosedDoor.ForceOpen();
		m_Boris.StopWaypointPathing();
		m_Boris.transform.position = m_BorisLeverPulledPath.Waypoints[m_BorisLeverPulledPath.Waypoints.Count - 1].transform.position;
		SendOnComplete();
	}

	private void HandleBorisLeverNodeOnComplete(object sender, EventArgs e)
	{
		m_Boris.OnWaypointComplete -= HandleBorisLeverNodeOnComplete;
		m_Boris.StopWaypointPathing();
		m_LeverPath.Dispose();
		m_IsBorisReady = true;
		if (CheckReady())
		{
			EnableLever();
		}
	}

	private void HandleDialogueTriggerOnEnter(object sender, EventArgs e)
	{
		m_DialogueTrigger.OnEnter -= HandleDialogueTriggerOnEnter;
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryClipCH3_20, "DIACH3/DIA_CH3_HENRY_16"));
		AudioObject audioObject = GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryClipCH3_21, "DIACH3/DIA_CH3_HENRY_17"));
		audioObject.OnComplete += delegate
		{
			GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH3_OBJECTIVE_14", "OBJECTIVES/CH3_OBJECTIVE_14_TIP", 4f));
		};
		m_Piper.OnActivate += HandlePiperTriggerOnEnter;
		m_Piper.OnDeath += HandlePiperOnDeath;
		m_Piper.gameObject.SetActive(true);
		m_InkMachineTrigger.OnEnter += HandleInkMachineTriggerOnEnter;
		m_InkMachineTrigger.SetActive(active: true);
	}

	private void HandleInkMachineTriggerOnEnter(object sender, EventArgs e)
	{
		m_InkMachineTrigger.OnEnter -= HandleInkMachineTriggerOnEnter;
		m_InkMachineLoop.gameObject.SetActive(true);
		m_InkMachineLoop.Activate();
	}

	private void HandlePiperTriggerOnEnter(object sender, EventArgs e)
	{
		m_Piper.OnActivate -= HandlePiperTriggerOnEnter;
		m_Piper.SetStartingThought(AiThought.Idle);
		for (int i = 0; i < m_Poster.Count; i++)
		{
			m_Poster[i].SetActive(!m_Poster[i].activeSelf);
		}
		GameManager.Instance.AudioManager.Play(m_PiperJumpscareClip);
		GameManager.Instance.AudioManager.Play(m_JumpscarePaperClip);
		m_PiperMusic = GameManager.Instance.AudioManager.Play(m_PiperMusicClip, AudioObjectType.MUSIC, -1);
		m_Boris.SetCower(active: true);
	}

	private void HandlePiperOnDeath(object sender, EventArgs e)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		m_Piper.OnDeath += HandlePiperOnDeath;
		TweenSettingsExtensions.OnComplete<Tweener>(m_PiperMusic.AudioSource.DOFade(0f, 1f), (TweenCallback)delegate
		{
			m_PiperMusic.Clear();
		});
		GameManager.Instance.AudioManager.Play(m_OldFriendsClip, AudioObjectType.MUSIC);
		m_Boris.SetCower(active: false);
		m_IsPlayerReady = true;
		if (CheckReady())
		{
			EnableLever();
		}
	}

	private bool CheckReady()
	{
		return m_IsPlayerReady && m_IsBorisReady;
	}

	private void EnableLever()
	{
		m_Lever.SetActive(active: true);
		m_Lever.OnInteracted += HandleLeverOnInteracted;
	}

	private void HandleLeverOnInteracted(object sender, EventArgs e)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Expected O, but got Unknown
		((Renderer)m_Cables).material.SetFloat("_Shimmer", 1f);
		GameManager.Instance.AudioManager.Play(m_MainPowerTurnOnClip);
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_BorisLever, new Vector3(-80f, 0f, 0f), 0.5f, (RotateMode)3), (Ease)27);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_Lever.transform, new Vector3(-80f, 0f, 0f), 0.5f, (RotateMode)3), (Ease)27), new TweenCallback(LeverOnComplete));
	}

	private void LeverOnComplete()
	{
		for (int i = 0; i < m_Lights.Count; i++)
		{
			m_Lights[i].TurnOff();
			((Behaviour)m_Lights[i]).enabled = false;
		}
		m_ClosedDoor.Open();
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/OBJECTIVE_FIND_A_NEW_EXIT", string.Empty, 4f));
		m_Boris.OnWaypointComplete += HandleLeverPulledPathOnComplete;
		m_Boris.UpdateWaypointList(m_BorisLeverPulledPath.Waypoints);
		m_BorisGoTrigger.SetActive(active: true);
		m_BorisGoTrigger.OnEnter += HandleBorisGoTriggerOnEnter;
	}

	private void HandleLeverPulledPathOnComplete(object sender, EventArgs e)
	{
		m_Boris.OnWaypointComplete -= HandleLeverPulledPathOnComplete;
		m_BorisLeverPulledPath.Dispose();
		m_Boris.StopWaypointPathing();
	}

	private void HandleBorisGoTriggerOnEnter(object sender, EventArgs e)
	{
		m_BorisGoTrigger.OnEnter -= HandleBorisGoTriggerOnEnter;
		((Renderer)m_Cables).material.SetFloat("_Shimmer", 0f);
		GameManager.Instance.GameData.CurrentSaveFile.CH3Data.PosterPiperObjective.IsComplete = true;
		GameManager.Instance.GameDataManager.Save();
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		m_PiperMusic = null;
		m_PiperMusicClip = null;
		m_OldFriendsClip = null;
		m_PiperJumpscareClip = null;
		m_JumpscarePaperClip = null;
		m_HenryClipCH3_20 = null;
		m_HenryClipCH3_21 = null;
		m_MainPowerTurnOnClip = null;
		base.OnDisposed();
	}
}
