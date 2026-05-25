using System;
using DG.Tweening;
using UnityEngine;

public class CH2SammyHallwayController : BaseController
{
	[Header("< Audio Event >")]
	[SerializeField]
	private EventTrigger m_LobbyEventTrigger;

	[Header("Jumpscare: Sammy!")]
	[SerializeField]
	private Transform m_Sammy;

	[SerializeField]
	private Transform m_SammyStartPos;

	[SerializeField]
	private Transform m_SammyEndPos;

	[SerializeField]
	private Transform m_AudioPosition;

	[SerializeField]
	private EventTrigger m_SammyEvent;

	[SerializeField]
	private EventTrigger m_SammyEndEvent;

	[SerializeField]
	private float m_WalkDuration = 5.58f;

	[Header("DEV CHEATS")]
	[SerializeField]
	private Transform m_CheatPoint;

	private AudioObject m_DialogueAudio;

	private AudioClip m_HorrorCueAudioClip;

	private AudioClip m_HenryClip03;

	private AudioClip m_HenryClip04;

	private AudioClip m_HenryClip05;

	private AudioClip m_SammyDialogueClip_01;

	public override void InitOnComplete()
	{
		base.InitOnComplete();
		m_HorrorCueAudioClip = GameManager.Instance.GetAudioClip("Audio/MUS/CH2/MUS_Horror_Cue_02");
		m_HenryClip03 = GameManager.Instance.GetAudioClip("Audio/DIA/CH2/Henry/DIA_CH2_HENRY_03");
		m_HenryClip04 = GameManager.Instance.GetAudioClip("Audio/DIA/CH2/Henry/DIA_CH2_HENRY_04");
		m_HenryClip05 = GameManager.Instance.GetAudioClip("Audio/DIA/CH2/Henry/DIA_CH2_HENRY_05");
		m_SammyDialogueClip_01 = GameManager.Instance.GetAudioClip("Audio/DIA/CH2/Sammy/DIA_Sammy_01");
		((Component)m_Sammy).gameObject.SetActive(false);
		m_SammyEvent.SetActive(active: false);
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH2Data.GateObjective.IsStarted)
		{
			ForceComplete();
			return;
		}
		m_SammyEvent.OnEnter += HandleSammyEventOnEnter;
		m_SammyEvent.SetActive(active: true);
		m_LobbyEventTrigger.OnEnter += HandleLobbyEventTriggerOnEnter;
		m_LobbyEventTrigger.SetActive(active: true);
	}

	private void HandleLobbyEventTriggerOnEnter(object sender, EventArgs e)
	{
		m_LobbyEventTrigger.OnEnter -= HandleLobbyEventTriggerOnEnter;
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryClip03, "DIACH2/DIA_CH2_HENRY_03", isTrimmed: true));
	}

	private void HandleSammyEventOnEnter(object sender, EventArgs e)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		m_SammyEvent.OnEnter -= HandleSammyEventOnEnter;
		((Component)m_Sammy).gameObject.SetActive(true);
		m_Sammy.localPosition = m_SammyStartPos.localPosition;
		GameManager.Instance.AudioManager.Play(m_HorrorCueAudioClip);
		m_DialogueAudio = GameManager.Instance.AudioManager.PlayAtPosition(m_SammyDialogueClip_01, m_AudioPosition.position, AudioObjectType.DIALOGUE, 0, isQueued: false, m_Sammy);
		TweenSettingsExtensions.OnComplete<Sequence>(DOSammyEvent(), new TweenCallback(HandleSammyEventOnComplete));
	}

	private Sequence DOSammyEvent()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		Sequence val = DOTween.Sequence();
		float num = 0f;
		TweenSettingsExtensions.Insert(val, num, (Tween)(object)TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_Sammy, m_SammyEndPos.localPosition, m_WalkDuration, false), (Ease)1), new TweenCallback(HandleSammyMoveOnComplete)));
		num += 1f;
		TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
		{
			GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryClip04, "DIACH2/DIA_CH2_HENRY_04", isTrimmed: true));
		});
		return val;
	}

	private void HandleSammyEventOnComplete()
	{
		m_SammyEndEvent.SetActive(active: true);
		m_SammyEndEvent.OnEnter += HandleSammyEndEventOnEnter;
	}

	private void HandleSammyEndEventOnEnter(object sender, EventArgs e)
	{
		m_SammyEndEvent.OnEnter -= HandleSammyEndEventOnEnter;
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryClip05, "DIACH2/DIA_CH2_HENRY_05", isTrimmed: true));
		SendOnComplete();
	}

	private void HandleSammyMoveOnComplete()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)m_DialogueAudio))
		{
			return;
		}
		m_DialogueAudio.transform.SetParent((Transform)null);
		ShortcutExtensions.DOLocalMoveX(m_DialogueAudio.transform, m_DialogueAudio.transform.localPosition.x + 25f, 5f, false);
		m_DialogueAudio.OnComplete += delegate
		{
			if (Object.op_Implicit((Object)(object)m_DialogueAudio))
			{
				ShortcutExtensions.DOKill((Component)(object)m_DialogueAudio.transform, false);
				m_DialogueAudio.Clear();
				m_DialogueAudio = null;
			}
		};
		Object.Destroy((Object)(object)((Component)m_Sammy).gameObject);
	}

	private void ForceComplete()
	{
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH2_OBJECTIVE_GATE", "OBJECTIVES/CH2_OBJECTIVE_GATE_TIP"));
		Object.Destroy((Object)(object)((Component)m_Sammy).gameObject);
		SendOnComplete();
	}

	protected override void OnDisposed()
	{
		if (Object.op_Implicit((Object)(object)m_SammyEvent))
		{
			m_SammyEvent.OnEnter -= HandleSammyEventOnEnter;
		}
		if (Object.op_Implicit((Object)(object)m_SammyEndEvent))
		{
			m_SammyEndEvent.OnEnter -= HandleSammyEndEventOnEnter;
		}
		if (Object.op_Implicit((Object)(object)m_LobbyEventTrigger))
		{
			m_LobbyEventTrigger.OnEnter -= HandleLobbyEventTriggerOnEnter;
		}
		m_HorrorCueAudioClip = null;
		m_HenryClip03 = null;
		m_HenryClip04 = null;
		m_HenryClip05 = null;
		m_SammyDialogueClip_01 = null;
		m_DialogueAudio = null;
		base.OnDisposed();
	}
}
