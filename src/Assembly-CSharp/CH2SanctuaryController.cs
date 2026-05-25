using System;
using System.Collections.Generic;
using DG.Tweening;
using S13Audio;
using UnityEngine;

public class CH2SanctuaryController : BaseController
{
	[Header("Objective: Turn Valve")]
	[SerializeField]
	private GameObject m_InkBlockage;

	[SerializeField]
	private Interactable m_Valve;

	[SerializeField]
	private List<InkPipeController> m_PipeControllers;

	[Header("Achievement: Strike Up The Band")]
	[SerializeField]
	private GameObject m_StrikeUpTheBand;

	[Header("Event Triggers")]
	[SerializeField]
	private EventTrigger m_JumpscareTrigger;

	[Header("Steam Particles")]
	[SerializeField]
	private ParticleSystem m_SteamJet;

	[Header("Cutout Jumpscare")]
	[SerializeField]
	private MeshRenderer m_CutoutMeshRenderer;

	[SerializeField]
	private Transform m_Cutout;

	[SerializeField]
	private Transform m_CutoutStartPosition;

	[SerializeField]
	private Transform m_CutoutScarePosition;

	[SerializeField]
	private Transform m_CutoutEndPosition;

	[Header("Ink")]
	[SerializeField]
	private GameObject m_ScareInk;

	[Header("DEV CHEATS")]
	[SerializeField]
	private Transform m_CheatPoint;

	private AudioClip m_ValveClip;

	private AudioClip m_JumpscareClip;

	private AudioClip m_HenryClip;

	public override void InitOnComplete()
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		m_Valve.SetActive(active: false);
		m_ValveClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Valve_Turn_Steam_Release_01");
		m_JumpscareClip = GameManager.Instance.GetAudioClip("Audio/SFX/SFX_Jumpscare_01");
		m_HenryClip = GameManager.Instance.GetAudioClip("Audio/DIA/CH3/Henry/ch3_henry_27_valvepuzzle_trueA");
		for (int i = 0; i < m_PipeControllers.Count; i++)
		{
			m_PipeControllers[i].TurnOn();
		}
		m_ScareInk.SetActive(false);
		m_Cutout.position = m_CutoutStartPosition.position;
		m_Cutout.rotation = m_CutoutStartPosition.rotation;
		((Component)m_Cutout).gameObject.SetActive(false);
	}

	public override void Activate()
	{
		if (GameManager.Instance.GameData.CurrentSaveFile.CH2Data.SanctuaryObjective.IsComplete)
		{
			ForceComplete();
		}
		else if (GameManager.Instance.GameData.CurrentSaveFile.CH2Data.SanctuaryObjective.IsStarted)
		{
			GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH2_OBJECTIVE_SECOND_VALVE", string.Empty));
			S13AudioManager.Instance.InvokeEvent("evt_office_stairs_ink_drained");
			m_JumpscareTrigger.OnEnter += HandleJumpscareTriggerOnEnter;
			m_JumpscareTrigger.SetActive(active: true);
			((Component)m_Cutout).gameObject.SetActive(true);
			m_StrikeUpTheBand.SetActive(false);
			m_InkBlockage.SetActive(false);
			for (int i = 0; i < m_PipeControllers.Count; i++)
			{
				m_PipeControllers[i].TurnOff();
			}
		}
		else
		{
			GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH2_OBJECTIVE_ENTER_SANCTUARY", string.Empty));
			m_Valve.OnInteracted += HandleValveOnInteracted;
			m_Valve.SetActive(active: true);
		}
	}

	private void HandleValveOnInteracted(object sender, EventArgs e)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected O, but got Unknown
		m_Valve.OnInteracted -= HandleValveOnInteracted;
		m_JumpscareTrigger.OnEnter += HandleJumpscareTriggerOnEnter;
		((Component)m_Cutout).gameObject.SetActive(true);
		m_JumpscareTrigger.SetActive(active: true);
		m_StrikeUpTheBand.SetActive(false);
		GameManager.Instance.AudioManager.PlayAtPosition(m_ValveClip, m_Valve.transform.position);
		TweenSettingsExtensions.InsertCallback(DOTween.Sequence(), 7.5f, (TweenCallback)delegate
		{
			m_SteamJet.Emit(10);
		});
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(m_Valve.transform, new Vector3(360f, 0f, 0f), 8f, (RotateMode)3), (Ease)7), new TweenCallback(HandleValveHandleRotateOnComplete));
		GameManager.Instance.ShowDialogue(DialogueDataVO.Create(m_HenryClip, "DIACH3/DIA_CH3_HENRY_28", isTrimmed: true)).OnComplete += HandleValveDialogueOnComplete;
		GameManager.Instance.GameData.CurrentSaveFile.CH2Data.SanctuaryObjective.IsStarted = true;
		GameManager.Instance.GameDataManager.Save();
	}

	private void HandleValveDialogueOnComplete(object sender, EventArgs e)
	{
		(sender as AudioObject).OnComplete -= HandleValveDialogueOnComplete;
		GameManager.Instance.ShowObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH2_OBJECTIVE_SECOND_VALVE", string.Empty, 4f));
	}

	private void HandleValveHandleRotateOnComplete()
	{
		m_InkBlockage.SetActive(false);
		S13AudioManager.Instance.InvokeEvent("evt_office_stairs_ink_drained");
		for (int i = 0; i < m_PipeControllers.Count; i++)
		{
			m_PipeControllers[i].TurnOff();
		}
	}

	private void HandleJumpscareTriggerOnEnter(object sender, EventArgs e)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		m_JumpscareTrigger.OnEnter -= HandleJumpscareTriggerOnEnter;
		m_JumpscareTrigger.SetActive(active: false);
		TweenSettingsExtensions.OnComplete<Sequence>(DOCutoutSequence(), new TweenCallback(HandleCutoutSequenceOnComplete));
	}

	private Sequence DOCutoutSequence()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		GameManager.Instance.AudioManager.Play(m_JumpscareClip);
		Sequence val = DOTween.Sequence();
		float num = 0.25f;
		float num2 = 0f;
		TweenSettingsExtensions.Insert(val, num2, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(m_Cutout, m_CutoutScarePosition.localPosition, num, false), (Ease)27));
		TweenSettingsExtensions.Insert(val, num2, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotateQuaternion(m_Cutout, m_CutoutScarePosition.localRotation, num), (Ease)27));
		num2 += 0.55f;
		TweenSettingsExtensions.Insert(val, num2, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(m_Cutout, m_CutoutStartPosition.localPosition, num, false), (Ease)26));
		TweenSettingsExtensions.Insert(val, num2, (Tween)(object)TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotateQuaternion(m_Cutout, m_CutoutStartPosition.localRotation, num), (Ease)26));
		return val;
	}

	private void HandleCutoutSequenceOnComplete()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		m_Cutout.position = m_CutoutEndPosition.position;
		m_Cutout.rotation = m_CutoutEndPosition.rotation;
		m_ScareInk.SetActive(true);
		SendOnComplete();
	}

	private void ForceComplete()
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		GameManager.Instance.UpdateObjective(ObjectiveDataVO.Create("OBJECTIVES/NEW_OBJECTIVE_HEADER", "OBJECTIVES/CH2_OBJECTIVE_SECOND_VALVE", string.Empty));
		m_StrikeUpTheBand.SetActive(false);
		m_Valve.SetActive(active: false);
		m_Valve.transform.localEulerAngles = new Vector3(360f, 0f, 0f);
		HandleValveHandleRotateOnComplete();
		((Component)m_Cutout).gameObject.SetActive(true);
		HandleCutoutSequenceOnComplete();
	}

	protected override void OnDisposed()
	{
		if (Object.op_Implicit((Object)(object)m_Valve))
		{
			m_Valve.OnInteracted -= HandleValveOnInteracted;
		}
		if ((Object)(object)m_JumpscareTrigger != (Object)null)
		{
			m_JumpscareTrigger.OnEnter -= HandleJumpscareTriggerOnEnter;
		}
		m_ValveClip = null;
		m_JumpscareClip = null;
		m_HenryClip = null;
		base.OnDisposed();
	}
}
